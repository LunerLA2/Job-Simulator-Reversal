using System;
using OwlchemyVR;
using UnityEngine;

public class CarTrunkController : MonoBehaviour
{
	[SerializeField]
	private ItemCollectionZone collectionZone;

	[SerializeField]
	private WorldItem myWorldItem;

	[SerializeField]
	private GrabbableHinge grabbableHinge;

	public ItemCollectionZone CollectionZone
	{
		get
		{
			return collectionZone;
		}
	}

	private void OnEnable()
	{
		ItemCollectionZone itemCollectionZone = collectionZone;
		itemCollectionZone.OnItemsInCollectionAdded = (Action<ItemCollectionZone, PickupableItem>)Delegate.Combine(itemCollectionZone.OnItemsInCollectionAdded, new Action<ItemCollectionZone, PickupableItem>(ItemAddedToCollectionZone));
		ItemCollectionZone itemCollectionZone2 = collectionZone;
		itemCollectionZone2.OnItemsInCollectionRemoved = (Action<ItemCollectionZone, PickupableItem>)Delegate.Combine(itemCollectionZone2.OnItemsInCollectionRemoved, new Action<ItemCollectionZone, PickupableItem>(ItemRemovedFromCollectionZone));
	}

	private void OnDisable()
	{
		ItemCollectionZone itemCollectionZone = collectionZone;
		itemCollectionZone.OnItemsInCollectionAdded = (Action<ItemCollectionZone, PickupableItem>)Delegate.Remove(itemCollectionZone.OnItemsInCollectionAdded, new Action<ItemCollectionZone, PickupableItem>(ItemAddedToCollectionZone));
		ItemCollectionZone itemCollectionZone2 = collectionZone;
		itemCollectionZone2.OnItemsInCollectionRemoved = (Action<ItemCollectionZone, PickupableItem>)Delegate.Remove(itemCollectionZone2.OnItemsInCollectionRemoved, new Action<ItemCollectionZone, PickupableItem>(ItemRemovedFromCollectionZone));
	}

	private void ItemAddedToCollectionZone(ItemCollectionZone zone, PickupableItem pickupable)
	{
		WorldItem component = pickupable.GetComponent<WorldItem>();
		if (component != null)
		{
			GameEventsManager.Instance.ItemAppliedToItemActionOccurred(component.Data, myWorldItem.Data, "ADDED_TO");
		}
	}

	private void ItemRemovedFromCollectionZone(ItemCollectionZone zone, PickupableItem pickupable)
	{
		WorldItem component = pickupable.GetComponent<WorldItem>();
		if (component != null)
		{
			GameEventsManager.Instance.ItemAppliedToItemActionOccurred(component.Data, myWorldItem.Data, "REMOVED_FROM");
		}
	}
}
