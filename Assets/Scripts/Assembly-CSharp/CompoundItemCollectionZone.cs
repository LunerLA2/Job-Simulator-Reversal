using System;
using System.Collections.Generic;
using OwlchemyVR;
using UnityEngine;

public class CompoundItemCollectionZone : MonoBehaviour
{
	[SerializeField]
	private ItemCollectionZone[] itemCollectionZones;

	private List<PickupableItem> itemsInCollection = new List<PickupableItem>();

	private List<int> itemZoneCounts = new List<int>();

	public Action<CompoundItemCollectionZone, PickupableItem> OnItemsInCollectionAdded;

	public Action<CompoundItemCollectionZone, PickupableItem> OnItemsInCollectionRemoved;

	public List<PickupableItem> ItemsInCollection
	{
		get
		{
			return itemsInCollection;
		}
	}

	private void OnEnable()
	{
		for (int i = 0; i < itemCollectionZones.Length; i++)
		{
			ItemCollectionZone obj = itemCollectionZones[i];
			obj.OnItemsInCollectionAdded = (Action<ItemCollectionZone, PickupableItem>)Delegate.Combine(obj.OnItemsInCollectionAdded, new Action<ItemCollectionZone, PickupableItem>(ItemAddedToOneZone));
			ItemCollectionZone obj2 = itemCollectionZones[i];
			obj2.OnItemsInCollectionRemoved = (Action<ItemCollectionZone, PickupableItem>)Delegate.Combine(obj2.OnItemsInCollectionRemoved, new Action<ItemCollectionZone, PickupableItem>(ItemRemovedFromOneZone));
		}
	}

	private void OnDisable()
	{
		for (int i = 0; i < itemCollectionZones.Length; i++)
		{
			ItemCollectionZone obj = itemCollectionZones[i];
			obj.OnItemsInCollectionAdded = (Action<ItemCollectionZone, PickupableItem>)Delegate.Remove(obj.OnItemsInCollectionAdded, new Action<ItemCollectionZone, PickupableItem>(ItemAddedToOneZone));
			ItemCollectionZone obj2 = itemCollectionZones[i];
			obj2.OnItemsInCollectionRemoved = (Action<ItemCollectionZone, PickupableItem>)Delegate.Remove(obj2.OnItemsInCollectionRemoved, new Action<ItemCollectionZone, PickupableItem>(ItemRemovedFromOneZone));
		}
	}

	private void ItemAddedToOneZone(ItemCollectionZone zone, PickupableItem item)
	{
		if (!itemsInCollection.Contains(item))
		{
			itemsInCollection.Add(item);
			itemZoneCounts.Add(1);
			if (OnItemsInCollectionAdded != null)
			{
				OnItemsInCollectionAdded(this, item);
			}
		}
		else
		{
			int num = itemsInCollection.IndexOf(item);
			List<int> list;
			List<int> list2 = (list = itemZoneCounts);
			int index;
			int index2 = (index = num);
			index = list[index];
			list2[index2] = index + 1;
		}
	}

	private void ItemRemovedFromOneZone(ItemCollectionZone zone, PickupableItem item)
	{
		if (itemsInCollection.Contains(item))
		{
			int num = itemsInCollection.IndexOf(item);
			List<int> list;
			List<int> list2 = (list = itemZoneCounts);
			int index;
			int index2 = (index = num);
			index = list[index];
			list2[index2] = index - 1;
			if (itemZoneCounts[num] <= 0)
			{
				itemsInCollection.RemoveAt(num);
				itemZoneCounts.RemoveAt(num);
				if (OnItemsInCollectionRemoved != null)
				{
					OnItemsInCollectionRemoved(this, item);
				}
			}
		}
		else
		{
			Debug.LogWarning("Tried to remove " + item.gameObject.name + " from CompoundItemCollectionZone but it was not being tracked.", base.gameObject);
		}
	}
}
