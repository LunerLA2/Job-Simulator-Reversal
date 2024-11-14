using System;
using System.Collections.Generic;
using OwlchemyVR;
using UnityEngine;

public class ItemCollectionZone : MonoBehaviour
{
	[SerializeField]
	private RigidbodyEnterExitTriggerEvents rigidbodyTriggerEvents;

	private List<PickupableItem> itemsInCollection = new List<PickupableItem>();

	public Action<ItemCollectionZone, PickupableItem> OnItemsInCollectionAdded;

	public Action<ItemCollectionZone, PickupableItem> OnItemsInCollectionRemoved;

	private List<PickupableItem> inCollectionZoneButCurrentlyInHand = new List<PickupableItem>();

	private bool isAnyHeldItemInside;

	private bool isSafeForAllPickupables;

	public RigidbodyEnterExitTriggerEvents RigidbodyTriggerEvents
	{
		get
		{
			return rigidbodyTriggerEvents;
		}
	}

	public List<PickupableItem> ItemsInCollection
	{
		get
		{
			return itemsInCollection;
		}
	}

	public int NumOfItemsInCollectionZone
	{
		get
		{
			return itemsInCollection.Count;
		}
	}

	public List<PickupableItem> InCollectionZoneButCurrentlyInHand
	{
		get
		{
			return inCollectionZoneButCurrentlyInHand;
		}
	}

	public bool IsAnyHeldItemInside
	{
		get
		{
			return isAnyHeldItemInside;
		}
	}

	public void SetIsSafeForAllPickupables(bool value)
	{
		isSafeForAllPickupables = value;
	}

	private void OnEnable()
	{
		RigidbodyEnterExitTriggerEvents rigidbodyEnterExitTriggerEvents = rigidbodyTriggerEvents;
		rigidbodyEnterExitTriggerEvents.OnRigidbodyEnterTrigger = (Action<Rigidbody>)Delegate.Combine(rigidbodyEnterExitTriggerEvents.OnRigidbodyEnterTrigger, new Action<Rigidbody>(RigidbodyEnteredTrigger));
		RigidbodyEnterExitTriggerEvents rigidbodyEnterExitTriggerEvents2 = rigidbodyTriggerEvents;
		rigidbodyEnterExitTriggerEvents2.OnRigidbodyExitTrigger = (Action<Rigidbody>)Delegate.Combine(rigidbodyEnterExitTriggerEvents2.OnRigidbodyExitTrigger, new Action<Rigidbody>(RigidbodyExitedTrigger));
		RigidbodyEnterExitTriggerEvents rigidbodyEnterExitTriggerEvents3 = rigidbodyTriggerEvents;
		rigidbodyEnterExitTriggerEvents3.OnUnknownRigidbodyDestroyedInsideOfTrigger = (Action)Delegate.Combine(rigidbodyEnterExitTriggerEvents3.OnUnknownRigidbodyDestroyedInsideOfTrigger, new Action(UnknownRigidbodyDestroyedInsideOfTrigger));
	}

	private void OnDisable()
	{
		RigidbodyEnterExitTriggerEvents rigidbodyEnterExitTriggerEvents = rigidbodyTriggerEvents;
		rigidbodyEnterExitTriggerEvents.OnRigidbodyEnterTrigger = (Action<Rigidbody>)Delegate.Remove(rigidbodyEnterExitTriggerEvents.OnRigidbodyEnterTrigger, new Action<Rigidbody>(RigidbodyEnteredTrigger));
		RigidbodyEnterExitTriggerEvents rigidbodyEnterExitTriggerEvents2 = rigidbodyTriggerEvents;
		rigidbodyEnterExitTriggerEvents2.OnRigidbodyExitTrigger = (Action<Rigidbody>)Delegate.Remove(rigidbodyEnterExitTriggerEvents2.OnRigidbodyExitTrigger, new Action<Rigidbody>(RigidbodyExitedTrigger));
		RigidbodyEnterExitTriggerEvents rigidbodyEnterExitTriggerEvents3 = rigidbodyTriggerEvents;
		rigidbodyEnterExitTriggerEvents3.OnUnknownRigidbodyDestroyedInsideOfTrigger = (Action)Delegate.Remove(rigidbodyEnterExitTriggerEvents3.OnUnknownRigidbodyDestroyedInsideOfTrigger, new Action(UnknownRigidbodyDestroyedInsideOfTrigger));
	}

	private void UnknownRigidbodyDestroyedInsideOfTrigger()
	{
		List<PickupableItem> list = new List<PickupableItem>();
		for (int i = 0; i < itemsInCollection.Count; i++)
		{
			if (itemsInCollection[i] == null)
			{
				itemsInCollection.RemoveAt(i);
				i--;
			}
			else if (itemsInCollection[i].Rigidbody == null)
			{
				Debug.Log(itemsInCollection[i].gameObject.name + " no longer has a rigidbody, removing it from ItemCollectionZone", base.gameObject);
				list.Add(itemsInCollection[i]);
			}
		}
		for (int j = 0; j < list.Count; j++)
		{
			RemoveItemFromCollectionZone(list[j]);
		}
	}

	private void RigidbodyEnteredTrigger(Rigidbody r)
	{
		PickupableItem component = r.GetComponent<PickupableItem>();
		if (component != null && (!component.IsHiddenFromZones || isSafeForAllPickupables))
		{
			if (component.IsCurrInHand)
			{
				AddItemInCollectionZoneButInHand(component);
			}
			else
			{
				AddItemToCollectionZone(component);
			}
		}
	}

	private void RigidbodyExitedTrigger(Rigidbody r)
	{
		PickupableItem component = r.GetComponent<PickupableItem>();
		if (component != null && (!component.IsHiddenFromZones || isSafeForAllPickupables))
		{
			if (inCollectionZoneButCurrentlyInHand.Contains(component))
			{
				RemoveItemFromCollectionZoneButInHand(component);
			}
			else
			{
				RemoveItemFromCollectionZone(component);
			}
		}
	}

	private void AddItemInCollectionZoneButInHand(PickupableItem item)
	{
		if (!inCollectionZoneButCurrentlyInHand.Contains(item))
		{
			isAnyHeldItemInside = true;
			inCollectionZoneButCurrentlyInHand.Add(item);
			item.OnReleased = (Action<GrabbableItem>)Delegate.Combine(item.OnReleased, new Action<GrabbableItem>(ItemInCollectionZoneReleased));
		}
		else
		{
			Debug.LogWarning("Attempted to add item to collection zone that was already in collection zone: " + item.gameObject.name, this);
		}
	}

	private void RemoveItemFromCollectionZoneButInHand(PickupableItem item)
	{
		if (inCollectionZoneButCurrentlyInHand.Contains(item))
		{
			inCollectionZoneButCurrentlyInHand.Remove(item);
			if (inCollectionZoneButCurrentlyInHand.Count == 0)
			{
				isAnyHeldItemInside = false;
			}
			item.OnReleased = (Action<GrabbableItem>)Delegate.Remove(item.OnReleased, new Action<GrabbableItem>(ItemInCollectionZoneReleased));
		}
		else
		{
			Debug.LogWarning("Attempted to remove item from collection zone that was not in collection zone: " + item.gameObject.name, this);
		}
	}

	private void ItemInCollectionZoneReleased(GrabbableItem grabbableItem)
	{
		PickupableItem pickupableItem = grabbableItem as PickupableItem;
		if (pickupableItem != null)
		{
			if (inCollectionZoneButCurrentlyInHand.Contains(pickupableItem))
			{
				RemoveItemFromCollectionZoneButInHand(pickupableItem);
				AddItemToCollectionZone(pickupableItem);
			}
		}
		else
		{
			Debug.LogWarning("Every item in a collection zone should be pickupable not just grabbable", this);
		}
	}

	private void ItemInCollectionZoneGrabbed(GrabbableItem grabbableItem)
	{
		PickupableItem pickupableItem = grabbableItem as PickupableItem;
		if (pickupableItem != null)
		{
			if (itemsInCollection.Contains(pickupableItem))
			{
				RemoveItemFromCollectionZone(pickupableItem);
				AddItemInCollectionZoneButInHand(pickupableItem);
			}
		}
		else
		{
			Debug.LogWarning("Every item in a collection zone should be pickupable not just grabbable", this);
		}
	}

	private void AddItemToCollectionZone(PickupableItem item)
	{
		if (!itemsInCollection.Contains(item))
		{
			itemsInCollection.Add(item);
			WorldItem worldItem = item.InteractableItem.WorldItem;
			worldItem.OnWorldItemDataWillChange = (Action<WorldItem>)Delegate.Combine(worldItem.OnWorldItemDataWillChange, new Action<WorldItem>(ItemInZoneWillChangeWorldItemData));
			WorldItem worldItem2 = item.InteractableItem.WorldItem;
			worldItem2.OnWorldItemDataHasChanged = (Action<WorldItem>)Delegate.Combine(worldItem2.OnWorldItemDataHasChanged, new Action<WorldItem>(ItemInZoneFinishedChangingWorldItemData));
			item.OnGrabbed = (Action<GrabbableItem>)Delegate.Combine(item.OnGrabbed, new Action<GrabbableItem>(ItemInCollectionZoneGrabbed));
			if (OnItemsInCollectionAdded != null)
			{
				OnItemsInCollectionAdded(this, item);
			}
		}
		else
		{
			Debug.LogWarning("Attempted to add item to collection zone that was already in collection zone: " + item.gameObject.name, this);
		}
	}

	private void RemoveItemFromCollectionZone(PickupableItem item)
	{
		if (itemsInCollection.Contains(item))
		{
			itemsInCollection.Remove(item);
			WorldItem worldItem = item.InteractableItem.WorldItem;
			worldItem.OnWorldItemDataWillChange = (Action<WorldItem>)Delegate.Remove(worldItem.OnWorldItemDataWillChange, new Action<WorldItem>(ItemInZoneWillChangeWorldItemData));
			WorldItem worldItem2 = item.InteractableItem.WorldItem;
			worldItem2.OnWorldItemDataHasChanged = (Action<WorldItem>)Delegate.Remove(worldItem2.OnWorldItemDataHasChanged, new Action<WorldItem>(ItemInZoneFinishedChangingWorldItemData));
			item.OnGrabbed = (Action<GrabbableItem>)Delegate.Remove(item.OnGrabbed, new Action<GrabbableItem>(ItemInCollectionZoneGrabbed));
			if (OnItemsInCollectionRemoved != null)
			{
				OnItemsInCollectionRemoved(this, item);
			}
		}
		else
		{
			Debug.LogWarning("Attempted to remove item from collection zone that was not in collection zone: " + item.gameObject.name, this);
		}
	}

	private void ItemInZoneWillChangeWorldItemData(WorldItem wi)
	{
		if (OnItemsInCollectionRemoved != null)
		{
			OnItemsInCollectionRemoved(this, wi.GetComponent<PickupableItem>());
		}
	}

	private void ItemInZoneFinishedChangingWorldItemData(WorldItem wi)
	{
		if (OnItemsInCollectionAdded != null)
		{
			OnItemsInCollectionAdded(this, wi.GetComponent<PickupableItem>());
		}
	}

	public void DestroyAllItemsInCollectionZone()
	{
		for (int i = 0; i < itemsInCollection.Count; i++)
		{
			PickupableItem pickupableItem = itemsInCollection[i];
			if (pickupableItem != null)
			{
				RemoveItemFromCollectionZone(itemsInCollection[i]);
				UnityEngine.Object.Destroy(pickupableItem.gameObject);
				i--;
			}
		}
		itemsInCollection.Clear();
	}

	public bool DoesItemCollectionZoneContainAtLeastOneUnitOf(WorldItemData worldItemData)
	{
		for (int i = 0; i < itemsInCollection.Count; i++)
		{
			if (itemsInCollection[i].InteractableItem.WorldItemData == worldItemData)
			{
				return true;
			}
		}
		return false;
	}

	public bool DoesItemCollectionZoneContainPickupable(PickupableItem item)
	{
		return itemsInCollection.Contains(item);
	}
}
