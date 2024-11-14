using System;
using System.Collections;
using System.Collections.Generic;
using OwlchemyVR;
using UnityEngine;

public class TrashCompactorController : MonoBehaviour
{
	[SerializeField]
	private ItemCollectionZone collectionZone;

	[SerializeField]
	private GrabbableSlider doorSlider;

	[SerializeField]
	private TrashCubeController trashCubePrefab;

	[SerializeField]
	private Transform cubeSpawnTransform;

	[SerializeField]
	private Transform shakeTransform;

	[SerializeField]
	private float compactDuration = 2f;

	[SerializeField]
	private GameObject particleSystemsRoot;

	[SerializeField]
	private WorldItem myWorldItem;

	private ParticleSystem[] particleSystems;

	private void Start()
	{
		if (particleSystemsRoot != null)
		{
			particleSystems = new ParticleSystem[particleSystemsRoot.transform.childCount];
			for (int i = 0; i < particleSystems.Length; i++)
			{
				particleSystems[i] = particleSystemsRoot.transform.GetChild(i).GetComponent<ParticleSystem>();
			}
		}
	}

	private void OnEnable()
	{
		doorSlider.OnLowerLocked += CompactItems;
		ItemCollectionZone itemCollectionZone = collectionZone;
		itemCollectionZone.OnItemsInCollectionAdded = (Action<ItemCollectionZone, PickupableItem>)Delegate.Combine(itemCollectionZone.OnItemsInCollectionAdded, new Action<ItemCollectionZone, PickupableItem>(ItemEnteredCompactorZone));
		ItemCollectionZone itemCollectionZone2 = collectionZone;
		itemCollectionZone2.OnItemsInCollectionRemoved = (Action<ItemCollectionZone, PickupableItem>)Delegate.Combine(itemCollectionZone2.OnItemsInCollectionRemoved, new Action<ItemCollectionZone, PickupableItem>(ItemExitedCompactorZone));
	}

	private void OnDisable()
	{
		doorSlider.OnLowerLocked -= CompactItems;
		ItemCollectionZone itemCollectionZone = collectionZone;
		itemCollectionZone.OnItemsInCollectionAdded = (Action<ItemCollectionZone, PickupableItem>)Delegate.Remove(itemCollectionZone.OnItemsInCollectionAdded, new Action<ItemCollectionZone, PickupableItem>(ItemEnteredCompactorZone));
		ItemCollectionZone itemCollectionZone2 = collectionZone;
		itemCollectionZone2.OnItemsInCollectionRemoved = (Action<ItemCollectionZone, PickupableItem>)Delegate.Remove(itemCollectionZone2.OnItemsInCollectionRemoved, new Action<ItemCollectionZone, PickupableItem>(ItemExitedCompactorZone));
	}

	private void CompactItems(GrabbableSlider door, bool isInitial)
	{
		if (!isInitial && collectionZone.ItemsInCollection.Count != 0)
		{
			if (myWorldItem != null)
			{
				GameEventsManager.Instance.ItemActionOccurred(myWorldItem.Data, "USED");
			}
			StartCoroutine(CompactItemsAsync());
		}
	}

	private void ItemEnteredCompactorZone(ItemCollectionZone zone, PickupableItem item)
	{
		WorldItem component = item.GetComponent<WorldItem>();
		if (component != null)
		{
			GameEventsManager.Instance.ItemAppliedToItemActionOccurred(component.Data, myWorldItem.Data, "ATTACHED_TO");
			GameEventsManager.Instance.ItemActionOccurred(myWorldItem.Data, "ACTIVATED");
		}
	}

	private void ItemExitedCompactorZone(ItemCollectionZone zone, PickupableItem item)
	{
		WorldItem component = item.GetComponent<WorldItem>();
		if (component != null)
		{
			GameEventsManager.Instance.ItemAppliedToItemActionOccurred(component.Data, myWorldItem.Data, "DEATTACHED_FROM");
			GameEventsManager.Instance.ItemActionOccurred(myWorldItem.Data, "DEACTIVATED");
		}
	}

	private IEnumerator CompactItemsAsync()
	{
		doorSlider.Grabbable.enabled = false;
		float time = 0f;
		float initShakeX = shakeTransform.localPosition.x;
		float newShakeX = initShakeX;
		StartParticles();
		while (time < compactDuration)
		{
			newShakeX = initShakeX + Mathf.Sin(time * 100f) / 200f;
			shakeTransform.localPosition = new Vector3(newShakeX, shakeTransform.localPosition.y, shakeTransform.localPosition.z);
			time += Time.deltaTime;
			yield return null;
		}
		StopParticles();
		List<Color> colors = new List<Color>(collectionZone.ItemsInCollection.Count);
		for (int i = 0; i < collectionZone.ItemsInCollection.Count; i++)
		{
			WorldItem item = collectionZone.ItemsInCollection[i].GetComponent<WorldItem>();
			if (item != null)
			{
				colors.Add(item.Data.OverallColor);
				GameEventsManager.Instance.ItemAppliedToItemActionOccurred(item.Data, myWorldItem.Data, "DESTROYED_BY");
				GameEventsManager.Instance.ItemActionOccurred(item.Data, "DESTROYED");
				GameEventsManager.Instance.ItemAppliedToItemActionOccurred(item.Data, myWorldItem.Data, "ADDED_TO");
			}
			UnityEngine.Object.Destroy(collectionZone.ItemsInCollection[i].gameObject);
		}
		shakeTransform.localPosition = new Vector3(initShakeX, shakeTransform.localPosition.y, shakeTransform.localPosition.z);
		TrashCubeController newCube = UnityEngine.Object.Instantiate(trashCubePrefab, cubeSpawnTransform.position, Quaternion.identity) as TrashCubeController;
		newCube.RecolorCube(colors.ToArray());
		newCube.transform.SetParent(GlobalStorage.Instance.ContentRoot, true);
		doorSlider.Grabbable.enabled = true;
	}

	private void StartParticles()
	{
		for (int i = 0; i < particleSystems.Length; i++)
		{
			particleSystems[i].Play();
		}
	}

	private void StopParticles()
	{
		for (int i = 0; i < particleSystems.Length; i++)
		{
			particleSystems[i].Stop();
		}
	}
}
