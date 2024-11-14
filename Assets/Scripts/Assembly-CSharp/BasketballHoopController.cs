using System;
using System.Collections;
using System.Collections.Generic;
using OwlchemyVR;
using UnityEngine;

public class BasketballHoopController : MonoBehaviour
{
	[SerializeField]
	private WorldItemData recylingBinData;

	[SerializeField]
	private ItemCollectionZone hoopCollectionZone;

	private List<PickupableItem> destructionQueue = new List<PickupableItem>();

	private bool isQueueLoopRunning;

	private float minimumTimeBetweenDestroys = 0.5f;

	private bool isMoving;

	private Vector3 previousPosition;

	private void OnEnable()
	{
		ItemCollectionZone itemCollectionZone = hoopCollectionZone;
		itemCollectionZone.OnItemsInCollectionAdded = (Action<ItemCollectionZone, PickupableItem>)Delegate.Combine(itemCollectionZone.OnItemsInCollectionAdded, new Action<ItemCollectionZone, PickupableItem>(ItemEntered));
		previousPosition = base.transform.position;
	}

	private void OnDisable()
	{
		ItemCollectionZone itemCollectionZone = hoopCollectionZone;
		itemCollectionZone.OnItemsInCollectionAdded = (Action<ItemCollectionZone, PickupableItem>)Delegate.Remove(itemCollectionZone.OnItemsInCollectionAdded, new Action<ItemCollectionZone, PickupableItem>(ItemEntered));
	}

	private void ItemEntered(ItemCollectionZone zone, PickupableItem item)
	{
		if (!isMoving)
		{
			if (!destructionQueue.Contains(item))
			{
				destructionQueue.Add(item);
			}
			if (!isQueueLoopRunning)
			{
				isQueueLoopRunning = true;
				StartCoroutine(DestructionQueueLoop());
			}
		}
	}

	private IEnumerator DestructionQueueLoop()
	{
		while (destructionQueue.Count > 0)
		{
			if (destructionQueue[0] != null)
			{
				DoDestructionOnIndividualItem(destructionQueue[0]);
				destructionQueue.RemoveAt(0);
			}
			yield return new WaitForSeconds(minimumTimeBetweenDestroys);
		}
		isQueueLoopRunning = false;
	}

	private void DoDestructionOnIndividualItem(PickupableItem item)
	{
		WorldItemData worldItemData = item.InteractableItem.WorldItemData;
		if (worldItemData != null)
		{
			GameEventsManager.Instance.ItemAppliedToItemActionOccurred(worldItemData, recylingBinData, "ADDED_TO");
			GameEventsManager.Instance.ItemActionOccurred(recylingBinData, "USED");
			GameEventsManager.Instance.ItemActionOccurred(worldItemData, "DESTROYED");
		}
		UnityEngine.Object.Destroy(item.gameObject);
	}
}
