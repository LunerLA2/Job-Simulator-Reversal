using System;
using System.Collections;
using System.Collections.Generic;
using OwlchemyVR;
using UnityEngine;

public class EnergyCanDisplay : MonoBehaviour
{
	[SerializeField]
	private Transform pickupsContainer;

	[SerializeField]
	private ItemCollectionZone collectionZone;

	[SerializeField]
	private WorldItemData canData;

	[SerializeField]
	private Animation buttonRevealAnimation;

	private bool buttonRevealed;

	[SerializeField]
	private float freezeTime = 0.25f;

	[SerializeField]
	private ParticleSystem particleToPlayOnFreeze;

	[SerializeField]
	private GameObject lightToPlayOnFreeze;

	[SerializeField]
	private AudioClip audioClipToPlayOnFreeze;

	[SerializeField]
	private PageData beginAcceptingCansWhenPageShown;

	[SerializeField]
	private WorldItem myWorldItem;

	private List<PickupableItem> pickups = new List<PickupableItem>();

	private bool isFrozen;

	private bool ready;

	private void Start()
	{
		if (particleToPlayOnFreeze != null)
		{
			particleToPlayOnFreeze.Stop();
		}
		if (lightToPlayOnFreeze != null)
		{
			lightToPlayOnFreeze.SetActive(false);
		}
	}

	private void OnEnable()
	{
		JobBoardManager instance = JobBoardManager.instance;
		instance.OnPageShown = (Action<PageStatusController>)Delegate.Combine(instance.OnPageShown, new Action<PageStatusController>(PageShown));
		ItemCollectionZone itemCollectionZone = collectionZone;
		itemCollectionZone.OnItemsInCollectionAdded = (Action<ItemCollectionZone, PickupableItem>)Delegate.Combine(itemCollectionZone.OnItemsInCollectionAdded, new Action<ItemCollectionZone, PickupableItem>(ItemEnteredZone));
		ItemCollectionZone itemCollectionZone2 = collectionZone;
		itemCollectionZone2.OnItemsInCollectionRemoved = (Action<ItemCollectionZone, PickupableItem>)Delegate.Combine(itemCollectionZone2.OnItemsInCollectionRemoved, new Action<ItemCollectionZone, PickupableItem>(ItemExitedZone));
	}

	private void OnDisable()
	{
		JobBoardManager instance = JobBoardManager.instance;
		instance.OnPageShown = (Action<PageStatusController>)Delegate.Remove(instance.OnPageShown, new Action<PageStatusController>(PageShown));
		ItemCollectionZone itemCollectionZone = collectionZone;
		itemCollectionZone.OnItemsInCollectionAdded = (Action<ItemCollectionZone, PickupableItem>)Delegate.Remove(itemCollectionZone.OnItemsInCollectionAdded, new Action<ItemCollectionZone, PickupableItem>(ItemEnteredZone));
		ItemCollectionZone itemCollectionZone2 = collectionZone;
		itemCollectionZone2.OnItemsInCollectionRemoved = (Action<ItemCollectionZone, PickupableItem>)Delegate.Remove(itemCollectionZone2.OnItemsInCollectionRemoved, new Action<ItemCollectionZone, PickupableItem>(ItemExitedZone));
	}

	private void PageShown(PageStatusController page)
	{
		if (page.Data == beginAcceptingCansWhenPageShown)
		{
			ready = true;
			collectionZone.gameObject.SetActive(true);
		}
	}

	public void ButtonPushed(MechanicalPushButtonController controller)
	{
		if (isFrozen || pickups.Count == 0)
		{
			return;
		}
		controller.GetComponent<Rigidbody>().isKinematic = true;
		controller.enabled = false;
		for (int i = 0; i < pickups.Count; i++)
		{
			if (pickups[i].IsCurrInHand && pickups[i].CurrInteractableHand != null)
			{
				pickups[i].CurrInteractableHand.ManuallyReleaseJoint();
			}
			pickups[i].Rigidbody.isKinematic = true;
			pickups[i].enabled = false;
			pickups[i].InteractableItem.enabled = false;
			pickups[i].transform.parent = pickupsContainer;
		}
		isFrozen = true;
		GameEventsManager.Instance.ItemActionOccurred(myWorldItem.Data, "ACTIVATED");
		StartCoroutine(FreezeEffects());
	}

	private IEnumerator FreezeEffects()
	{
		if (particleToPlayOnFreeze != null)
		{
			particleToPlayOnFreeze.Play();
		}
		if (lightToPlayOnFreeze != null)
		{
			lightToPlayOnFreeze.SetActive(true);
		}
		if (audioClipToPlayOnFreeze != null)
		{
			AudioManager.Instance.Play(base.transform, audioClipToPlayOnFreeze, 1f, 1f);
		}
		yield return new WaitForSeconds(freezeTime);
		if (particleToPlayOnFreeze != null)
		{
			particleToPlayOnFreeze.Stop();
		}
		if (lightToPlayOnFreeze != null)
		{
			lightToPlayOnFreeze.SetActive(false);
		}
	}

	private void ItemEnteredZone(ItemCollectionZone zone, PickupableItem item)
	{
		if (!pickups.Contains(item) && ready && item.InteractableItem.WorldItemData == canData)
		{
			GameEventsManager.Instance.ItemAppliedToItemActionOccurred(canData, myWorldItem.Data, "ADDED_TO");
			pickups.Add(item);
			if (pickups.Count >= 3 && !buttonRevealed)
			{
				buttonRevealed = true;
				buttonRevealAnimation.Play();
			}
		}
	}

	private void ItemExitedZone(ItemCollectionZone zone, PickupableItem item)
	{
		if (pickups.Contains(item) && ready && item.InteractableItem.WorldItemData == canData)
		{
			GameEventsManager.Instance.ItemAppliedToItemActionOccurred(canData, myWorldItem.Data, "REMOVED_FROM");
			pickups.Remove(item);
		}
	}
}
