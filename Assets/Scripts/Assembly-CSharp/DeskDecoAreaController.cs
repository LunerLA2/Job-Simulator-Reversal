using System;
using System.Collections;
using System.Collections.Generic;
using OwlchemyVR;
using UnityEngine;

public class DeskDecoAreaController : MonoBehaviour
{
	[SerializeField]
	private WorldItem myWorldItem;

	[SerializeField]
	private WorldItemData[] dataToTrack;

	[SerializeField]
	private CompoundItemCollectionZone itemCollectionZone;

	[SerializeField]
	private FilteredRigidbodyEnterExitTriggerEvents[] triggersToFilter;

	private void OnEnable()
	{
		CompoundItemCollectionZone compoundItemCollectionZone = itemCollectionZone;
		compoundItemCollectionZone.OnItemsInCollectionAdded = (Action<CompoundItemCollectionZone, PickupableItem>)Delegate.Combine(compoundItemCollectionZone.OnItemsInCollectionAdded, new Action<CompoundItemCollectionZone, PickupableItem>(ItemAdded));
		CompoundItemCollectionZone compoundItemCollectionZone2 = itemCollectionZone;
		compoundItemCollectionZone2.OnItemsInCollectionRemoved = (Action<CompoundItemCollectionZone, PickupableItem>)Delegate.Combine(compoundItemCollectionZone2.OnItemsInCollectionRemoved, new Action<CompoundItemCollectionZone, PickupableItem>(ItemRemoved));
	}

	private void OnDisable()
	{
		CompoundItemCollectionZone compoundItemCollectionZone = itemCollectionZone;
		compoundItemCollectionZone.OnItemsInCollectionAdded = (Action<CompoundItemCollectionZone, PickupableItem>)Delegate.Remove(compoundItemCollectionZone.OnItemsInCollectionAdded, new Action<CompoundItemCollectionZone, PickupableItem>(ItemAdded));
		CompoundItemCollectionZone compoundItemCollectionZone2 = itemCollectionZone;
		compoundItemCollectionZone2.OnItemsInCollectionRemoved = (Action<CompoundItemCollectionZone, PickupableItem>)Delegate.Remove(compoundItemCollectionZone2.OnItemsInCollectionRemoved, new Action<CompoundItemCollectionZone, PickupableItem>(ItemRemoved));
	}

	private void ItemAdded(CompoundItemCollectionZone zone, PickupableItem item)
	{
		bool flag = false;
		for (int i = 0; i < dataToTrack.Length; i++)
		{
			if (dataToTrack[i] == item.InteractableItem.WorldItemData)
			{
				flag = true;
				break;
			}
		}
		if (flag)
		{
			GameEventsManager.Instance.ItemAppliedToItemActionOccurred(item.InteractableItem.WorldItemData, myWorldItem.Data, "ADDED_TO");
			GameEventsManager.Instance.ItemActionOccurred(myWorldItem.Data, "DEACTIVATED");
		}
	}

	private void ItemRemoved(CompoundItemCollectionZone zone, PickupableItem item)
	{
		bool flag = false;
		for (int i = 0; i < dataToTrack.Length; i++)
		{
			if (dataToTrack[i] == item.InteractableItem.WorldItemData)
			{
				flag = true;
				break;
			}
		}
		if (flag)
		{
			GameEventsManager.Instance.ItemAppliedToItemActionOccurred(item.InteractableItem.WorldItemData, myWorldItem.Data, "REMOVED_FROM");
			GameEventsManager.Instance.ItemActionOccurred(myWorldItem.Data, "ACTIVATED");
		}
	}

	private void Start()
	{
		StartCoroutine(WaitAndFilterItems());
	}

	private IEnumerator WaitAndFilterItems()
	{
		yield return null;
		List<PickupableItem> defaultItems = new List<PickupableItem>();
		defaultItems.AddRange(itemCollectionZone.ItemsInCollection);
		List<Rigidbody> rbsToIgnore = new List<Rigidbody>();
		for (int j = 0; j < defaultItems.Count; j++)
		{
			if (defaultItems[j].Rigidbody != null)
			{
				rbsToIgnore.Add(defaultItems[j].Rigidbody);
			}
		}
		for (int i = 0; i < triggersToFilter.Length; i++)
		{
			triggersToFilter[i].AddToListOfRigidbodiesToIgnore(rbsToIgnore);
		}
	}
}
