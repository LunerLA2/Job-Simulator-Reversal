using System;
using System.Collections.Generic;
using OwlchemyVR;
using UnityEngine;

public class GroceryBagController : MonoBehaviour
{
	[SerializeField]
	private WorldItem bagWorldItem;

	[SerializeField]
	private ItemCollectionZone itemCollectionZone;

	private UniqueObject uniqueObject;

	[SerializeField]
	private GameObject levitate;

	[SerializeField]
	private AudioClip bagLeavingClip;

	private bool hasBeenGiven;

	private bool itemsLocked;

	private List<Transform> bagTransfroms = new List<Transform>();

	public bool ItemsLocked
	{
		get
		{
			return itemsLocked;
		}
	}

	private void Start()
	{
		levitate.SetActive(false);
		uniqueObject = GetComponent<UniqueObject>();
		ItemCollectionZone obj = itemCollectionZone;
		obj.OnItemsInCollectionAdded = (Action<ItemCollectionZone, PickupableItem>)Delegate.Combine(obj.OnItemsInCollectionAdded, new Action<ItemCollectionZone, PickupableItem>(ItemEnteredZone));
		ItemCollectionZone obj2 = itemCollectionZone;
		obj2.OnItemsInCollectionRemoved = (Action<ItemCollectionZone, PickupableItem>)Delegate.Combine(obj2.OnItemsInCollectionRemoved, new Action<ItemCollectionZone, PickupableItem>(ItemExitedZone));
		UniqueObject obj3 = uniqueObject;
		obj3.OnWasPulledIntoInventoryOfBot = (Action<Bot>)Delegate.Combine(obj3.OnWasPulledIntoInventoryOfBot, new Action<Bot>(LockItemsInsideBag));
	}

	private void OnDisable()
	{
		ItemCollectionZone obj = itemCollectionZone;
		obj.OnItemsInCollectionAdded = (Action<ItemCollectionZone, PickupableItem>)Delegate.Remove(obj.OnItemsInCollectionAdded, new Action<ItemCollectionZone, PickupableItem>(ItemEnteredZone));
		ItemCollectionZone obj2 = itemCollectionZone;
		obj2.OnItemsInCollectionRemoved = (Action<ItemCollectionZone, PickupableItem>)Delegate.Remove(obj2.OnItemsInCollectionRemoved, new Action<ItemCollectionZone, PickupableItem>(ItemExitedZone));
		UniqueObject obj3 = uniqueObject;
		obj3.OnWasPulledIntoInventoryOfBot = (Action<Bot>)Delegate.Remove(obj3.OnWasPulledIntoInventoryOfBot, new Action<Bot>(LockItemsInsideBag));
	}

	private void ItemEnteredZone(ItemCollectionZone zone, PickupableItem item)
	{
		if (!hasBeenGiven)
		{
			WorldItem component = item.GetComponent<WorldItem>();
			if (component != null && component.Data != null)
			{
				GameEventsManager.Instance.ItemAppliedToItemActionOccurred(component.Data, bagWorldItem.Data, "ADDED_TO");
				GameEventsManager.Instance.ItemActionOccurred(bagWorldItem.Data, "ACTIVATED");
			}
		}
	}

	private void ItemExitedZone(ItemCollectionZone zone, PickupableItem item)
	{
		if (!hasBeenGiven)
		{
			WorldItem component = item.GetComponent<WorldItem>();
			if (component != null && component.Data != null)
			{
				GameEventsManager.Instance.ItemAppliedToItemActionOccurred(component.Data, bagWorldItem.Data, "REMOVED_FROM");
				GameEventsManager.Instance.ItemActionOccurred(bagWorldItem.Data, "DEACTIVATED");
			}
		}
	}

	public void LockItemsInsideBag(Bot b)
	{
		for (int i = 0; i < itemCollectionZone.ItemsInCollection.Count; i++)
		{
			PickupableItem pickupableItem = itemCollectionZone.ItemsInCollection[i];
			SelectedChangeOutlineController component = pickupableItem.GetComponent<SelectedChangeOutlineController>();
			if (component != null)
			{
				component.ForceRefreshMeshes();
			}
			Rigidbody rigidbody = pickupableItem.Rigidbody;
			Transform transform = rigidbody.transform;
			bagTransfroms.Add(transform);
			if (pickupableItem.IsCurrInHand)
			{
				pickupableItem.CurrInteractableHand.TryRelease(false);
			}
			pickupableItem.enabled = false;
			if (rigidbody != null)
			{
				UnityEngine.Object.Destroy(rigidbody);
			}
			transform.SetParent(base.transform, true);
		}
		itemCollectionZone.enabled = false;
		itemsLocked = true;
	}
}
