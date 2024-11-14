using System;
using System.Collections;
using System.Collections.Generic;
using OwlchemyVR;
using UnityEngine;

public class JumboSizerController : KitchenTool
{
	[SerializeField]
	private float scaleFactor = 2f;

	[SerializeField]
	private ItemCollectionZone itemCollectionZone;

	[SerializeField]
	private WorldItem worldItem;

	[SerializeField]
	private GameObject readyLight;

	[SerializeField]
	private GameObject notReadyLight;

	[SerializeField]
	private ParticleSystem effectParticle;

	[SerializeField]
	private AudioClip readySound;

	[SerializeField]
	private AudioClip biggenSound;

	[SerializeField]
	private WorldItemData[] worldItemsToNotAccept;

	[SerializeField]
	private JumboSizerWorldItemDataSwap[] worldItemDataSwaps;

	private List<Rigidbody> currentlyInsideRigidbodies = new List<Rigidbody>();

	private List<Rigidbody> alreadyScanned = new List<Rigidbody>();

	private bool isReady;

	private bool isBusy;

	private void OnEnable()
	{
		ItemCollectionZone obj = itemCollectionZone;
		obj.OnItemsInCollectionAdded = (Action<ItemCollectionZone, PickupableItem>)Delegate.Combine(obj.OnItemsInCollectionAdded, new Action<ItemCollectionZone, PickupableItem>(ItemEnteredZone));
		ItemCollectionZone obj2 = itemCollectionZone;
		obj2.OnItemsInCollectionRemoved = (Action<ItemCollectionZone, PickupableItem>)Delegate.Combine(obj2.OnItemsInCollectionRemoved, new Action<ItemCollectionZone, PickupableItem>(ItemExitedZone));
	}

	private void OnDisable()
	{
		ItemCollectionZone obj = itemCollectionZone;
		obj.OnItemsInCollectionAdded = (Action<ItemCollectionZone, PickupableItem>)Delegate.Remove(obj.OnItemsInCollectionAdded, new Action<ItemCollectionZone, PickupableItem>(ItemEnteredZone));
		ItemCollectionZone obj2 = itemCollectionZone;
		obj2.OnItemsInCollectionRemoved = (Action<ItemCollectionZone, PickupableItem>)Delegate.Remove(obj2.OnItemsInCollectionRemoved, new Action<ItemCollectionZone, PickupableItem>(ItemExitedZone));
	}

	private void Awake()
	{
		SetIsReady(false);
	}

	private void ItemEnteredZone(ItemCollectionZone zone, PickupableItem item)
	{
		Rigidbody rigidbody = item.Rigidbody;
		if (rigidbody != null && !currentlyInsideRigidbodies.Contains(rigidbody) && !alreadyScanned.Contains(rigidbody))
		{
			WorldItemData validWorldItemDataFromRigidbody = GetValidWorldItemDataFromRigidbody(rigidbody);
			if (validWorldItemDataFromRigidbody != null)
			{
				currentlyInsideRigidbodies.Add(rigidbody);
				RefreshList();
				SetIsReady(true);
			}
		}
	}

	private void ItemExitedZone(ItemCollectionZone zone, PickupableItem item)
	{
		Rigidbody rigidbody = item.Rigidbody;
		if (rigidbody != null && currentlyInsideRigidbodies.Contains(rigidbody))
		{
			currentlyInsideRigidbodies.Remove(rigidbody);
			RefreshList();
			if (currentlyInsideRigidbodies.Count == 0)
			{
				SetIsReady(false);
			}
		}
	}

	private void RefreshList()
	{
		for (int i = 0; i < currentlyInsideRigidbodies.Count; i++)
		{
			if (currentlyInsideRigidbodies[i] == null)
			{
				currentlyInsideRigidbodies.RemoveAt(i);
				i--;
			}
		}
	}

	private void SetIsReady(bool s)
	{
		if (s)
		{
			AudioManager.Instance.Play(base.transform.position, readySound, 1f, 1f);
		}
		isReady = s;
		readyLight.SetActive(s);
		notReadyLight.SetActive(!s);
	}

	public void ButtonPressed()
	{
		if (!isBusy && isReady)
		{
			StartCoroutine(DoJumboSize());
		}
	}

	private WorldItemData GetValidWorldItemDataFromRigidbody(Rigidbody r)
	{
		WorldItem component = r.GetComponent<WorldItem>();
		if (component != null && component.Data != null)
		{
			bool flag = true;
			for (int i = 0; i < worldItemsToNotAccept.Length; i++)
			{
				if (component.Data == worldItemsToNotAccept[i])
				{
					flag = false;
				}
			}
			if (flag)
			{
				return component.Data;
			}
		}
		return null;
	}

	private IEnumerator DoJumboSize()
	{
		isBusy = true;
		List<Rigidbody> tryToSize = new List<Rigidbody>();
		for (int j = 0; j < currentlyInsideRigidbodies.Count; j++)
		{
			if (currentlyInsideRigidbodies[j] != null)
			{
				tryToSize.Add(currentlyInsideRigidbodies[j]);
				continue;
			}
			currentlyInsideRigidbodies.RemoveAt(j);
			j--;
		}
		for (int i = 0; i < tryToSize.Count; i++)
		{
			if (currentlyInsideRigidbodies.Contains(tryToSize[i]) && !alreadyScanned.Contains(tryToSize[i]))
			{
				alreadyScanned.Add(tryToSize[i]);
				tryToSize[i].mass *= scaleFactor * scaleFactor;
				AudioManager.Instance.Play(tryToSize[i].transform, biggenSound, 1f, 1f);
				effectParticle.Play();
				WorldItem wi = tryToSize[i].GetComponent<WorldItem>();
				if (wi != null && !GenieManager.DoesContainGenieMode(GlobalStorage.Instance.CurrentGenieModes, JobGenieCartridge.GenieModeTypes.EndlessMode))
				{
					CheckForWorldItemDataSwap(wi);
				}
				else if (wi != null && GenieManager.DoesContainGenieMode(GlobalStorage.Instance.CurrentGenieModes, JobGenieCartridge.GenieModeTypes.EndlessMode))
				{
					GameEventsManager.Instance.ItemAppliedToItemActionOccurred(wi.Data, worldItem.Data, "CREATED_BY");
				}
				Go.to(tryToSize[i].transform, 0.5f, new GoTweenConfig().scale(scaleFactor));
				yield return new WaitForSeconds(0.4f);
			}
		}
		SetIsReady(false);
		isBusy = false;
	}

	private void CheckForWorldItemDataSwap(WorldItem wi)
	{
		for (int i = 0; i < worldItemDataSwaps.Length; i++)
		{
			if (worldItemDataSwaps[i].From == wi.Data)
			{
				wi.ManualSetData(worldItemDataSwaps[i].To);
				GameEventsManager.Instance.ItemAppliedToItemActionOccurred(worldItemDataSwaps[i].To, worldItem.Data, "CREATED_BY");
				break;
			}
		}
	}
}
