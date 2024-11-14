using System;
using OwlchemyVR;
using UnityEngine;

public class TicketListener : MonoBehaviour
{
	[SerializeField]
	private ItemCollectionZone[] itemCollectionZones;

	[SerializeField]
	private WorldItemData TicketWorldItem;

	private void OnEnable()
	{
		for (int i = 0; i < itemCollectionZones.Length; i++)
		{
			ItemCollectionZone obj = itemCollectionZones[i];
			obj.OnItemsInCollectionAdded = (Action<ItemCollectionZone, PickupableItem>)Delegate.Combine(obj.OnItemsInCollectionAdded, new Action<ItemCollectionZone, PickupableItem>(ItemAddedToOneZone));
		}
	}

	private void OnDisable()
	{
		for (int i = 0; i < itemCollectionZones.Length; i++)
		{
			ItemCollectionZone obj = itemCollectionZones[i];
			obj.OnItemsInCollectionAdded = (Action<ItemCollectionZone, PickupableItem>)Delegate.Remove(obj.OnItemsInCollectionAdded, new Action<ItemCollectionZone, PickupableItem>(ItemAddedToOneZone));
		}
	}

	private void ItemAddedToOneZone(ItemCollectionZone zone, PickupableItem item)
	{
		if (GenieManager.AreAnyJobGenieModesActive() && GenieManager.DoesContainGenieMode(GlobalStorage.Instance.CurrentGenieModes, JobGenieCartridge.GenieModeTypes.EndlessMode) && JobBoardManager.instance.EndlessModeStatusController.GetCurrentGoal() != null && !JobBoardManager.instance.EndlessModeStatusController.GetCurrentGoal().IsCompleted && item != null && item.InteractableItem.WorldItemData == TicketWorldItem)
		{
			JobBoardManager.instance.EndlessModeStatusController.ForceJobComplete(false, false);
		}
	}

	private void Update()
	{
	}
}
