using System;
using OwlchemyVR;
using UnityEngine;

public class DastardlyCollectionZone : MonoBehaviour
{
	[SerializeField]
	private ItemCollectionZone collectionZone;

	[SerializeField]
	private WorldItem myWorldItem;

	private void Start()
	{
	}

	private void Update()
	{
		ItemCollectionZone itemCollectionZone = collectionZone;
		itemCollectionZone.OnItemsInCollectionAdded = (Action<ItemCollectionZone, PickupableItem>)Delegate.Combine(itemCollectionZone.OnItemsInCollectionAdded, new Action<ItemCollectionZone, PickupableItem>(ItemAddedToCollectionZone));
	}

	private void ItemAddedToCollectionZone(ItemCollectionZone zone, PickupableItem item)
	{
		WorldItem component = item.GetComponent<WorldItem>();
		if (component != null)
		{
			GameEventsManager.Instance.ItemAppliedToItemActionOccurred(component.Data, myWorldItem.Data, "ADDED_TO");
		}
	}
}
