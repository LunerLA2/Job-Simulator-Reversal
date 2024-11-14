using System;
using OwlchemyVR;
using UnityEngine;

public class ItemCollectionZoneGameEvents : MonoBehaviour
{
	[SerializeField]
	private ItemCollectionZone itemCollectionZone;

	[SerializeField]
	private WorldItem myWorldItem;

	[SerializeField]
	private bool activateAndDeactivateEvents;

	[SerializeField]
	private bool miscItemEvents;

	[SerializeField]
	private WorldItemData[] worldItemsToExclude;

	private void OnEnable()
	{
		ItemCollectionZone obj = itemCollectionZone;
		obj.OnItemsInCollectionAdded = (Action<ItemCollectionZone, PickupableItem>)Delegate.Combine(obj.OnItemsInCollectionAdded, new Action<ItemCollectionZone, PickupableItem>(ItemAdded));
		ItemCollectionZone obj2 = itemCollectionZone;
		obj2.OnItemsInCollectionRemoved = (Action<ItemCollectionZone, PickupableItem>)Delegate.Combine(obj2.OnItemsInCollectionRemoved, new Action<ItemCollectionZone, PickupableItem>(ItemRemoved));
	}

	private void OnDisable()
	{
		ItemCollectionZone obj = itemCollectionZone;
		obj.OnItemsInCollectionAdded = (Action<ItemCollectionZone, PickupableItem>)Delegate.Remove(obj.OnItemsInCollectionAdded, new Action<ItemCollectionZone, PickupableItem>(ItemAdded));
		ItemCollectionZone obj2 = itemCollectionZone;
		obj2.OnItemsInCollectionRemoved = (Action<ItemCollectionZone, PickupableItem>)Delegate.Remove(obj2.OnItemsInCollectionRemoved, new Action<ItemCollectionZone, PickupableItem>(ItemRemoved));
	}

	private void ItemAdded(ItemCollectionZone zone, PickupableItem item)
	{
		if (item.InteractableItem.WorldItemData != null && Array.IndexOf(worldItemsToExclude, item.InteractableItem.WorldItemData) <= -1)
		{
			GameEventsManager.Instance.ItemAppliedToItemActionOccurred(item.InteractableItem.WorldItemData, myWorldItem.Data, "ADDED_TO");
			if (activateAndDeactivateEvents)
			{
				GameEventsManager.Instance.ItemActionOccurred(myWorldItem.Data, "ACTIVATED");
			}
			if (miscItemEvents)
			{
				GameEventsManager.Instance.ItemActionOccurred(myWorldItem.Data, "MISC_ITEM_ADDED");
			}
		}
	}

	private void ItemRemoved(ItemCollectionZone zone, PickupableItem item)
	{
		if (item.InteractableItem.WorldItemData != null && Array.IndexOf(worldItemsToExclude, item.InteractableItem.WorldItemData) <= -1)
		{
			GameEventsManager.Instance.ItemAppliedToItemActionOccurred(item.InteractableItem.WorldItemData, myWorldItem.Data, "REMOVED_FROM");
			if (activateAndDeactivateEvents)
			{
				GameEventsManager.Instance.ItemActionOccurred(myWorldItem.Data, "DEACTIVATED");
			}
			if (miscItemEvents)
			{
				GameEventsManager.Instance.ItemActionOccurred(myWorldItem.Data, "MISC_ITEM_REMOVED");
			}
		}
	}
}
