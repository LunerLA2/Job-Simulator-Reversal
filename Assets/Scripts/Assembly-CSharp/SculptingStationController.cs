using System;
using OwlchemyVR;
using UnityEngine;

public class SculptingStationController : KitchenToolWithLooseItems
{
	[SerializeField]
	private WorldItem myWorldItem;

	[SerializeField]
	private Transform modelBase;

	[SerializeField]
	private ItemCollectionZone modelZone;

	[SerializeField]
	private RigidbodyEnterExitTriggerEvents sculptureZone;

	[SerializeField]
	private AttachablePoint sculptureAttachPoint;

	private SculptureController workingSculpture;

	private void OnEnable()
	{
		RigidbodyEnterExitTriggerEvents rigidbodyEnterExitTriggerEvents = sculptureZone;
		rigidbodyEnterExitTriggerEvents.OnRigidbodyExitTrigger = (Action<Rigidbody>)Delegate.Combine(rigidbodyEnterExitTriggerEvents.OnRigidbodyExitTrigger, new Action<Rigidbody>(SculptureRemoved));
		ItemCollectionZone itemCollectionZone = modelZone;
		itemCollectionZone.OnItemsInCollectionAdded = (Action<ItemCollectionZone, PickupableItem>)Delegate.Combine(itemCollectionZone.OnItemsInCollectionAdded, new Action<ItemCollectionZone, PickupableItem>(UpdateModelZoneCount));
		ItemCollectionZone itemCollectionZone2 = modelZone;
		itemCollectionZone2.OnItemsInCollectionRemoved = (Action<ItemCollectionZone, PickupableItem>)Delegate.Combine(itemCollectionZone2.OnItemsInCollectionRemoved, new Action<ItemCollectionZone, PickupableItem>(UpdateModelZoneCount));
	}

	private void OnDisable()
	{
		RigidbodyEnterExitTriggerEvents rigidbodyEnterExitTriggerEvents = sculptureZone;
		rigidbodyEnterExitTriggerEvents.OnRigidbodyExitTrigger = (Action<Rigidbody>)Delegate.Remove(rigidbodyEnterExitTriggerEvents.OnRigidbodyExitTrigger, new Action<Rigidbody>(SculptureRemoved));
		ItemCollectionZone itemCollectionZone = modelZone;
		itemCollectionZone.OnItemsInCollectionAdded = (Action<ItemCollectionZone, PickupableItem>)Delegate.Remove(itemCollectionZone.OnItemsInCollectionAdded, new Action<ItemCollectionZone, PickupableItem>(UpdateModelZoneCount));
		ItemCollectionZone itemCollectionZone2 = modelZone;
		itemCollectionZone2.OnItemsInCollectionRemoved = (Action<ItemCollectionZone, PickupableItem>)Delegate.Remove(itemCollectionZone2.OnItemsInCollectionRemoved, new Action<ItemCollectionZone, PickupableItem>(UpdateModelZoneCount));
	}

	private void Start()
	{
		SummonNewSculpture(true);
	}

	private void UpdateModelZoneCount(ItemCollectionZone zone, PickupableItem item)
	{
		Debug.Log("update number: " + zone.ItemsInCollection.Count);
		GameEventsManager.Instance.ItemActionOccurredWithAmount(myWorldItem.Data, "FILLED_TO_AMOUNT", zone.ItemsInCollection.Count);
	}

	private void SummonNewSculpture(bool immediate)
	{
		if (immediate)
		{
			sculptureAttachPoint.RefillOneItemImmediate();
		}
		else
		{
			sculptureAttachPoint.RefillOneItem();
		}
		AttachableObject attachedObject = sculptureAttachPoint.GetAttachedObject(0);
		if (attachedObject != null)
		{
			SculptureController component = attachedObject.GetComponent<SculptureController>();
			if (component != null)
			{
				workingSculpture = component;
				workingSculpture.OnFirstHit += SculptureFirstHit;
				SculptureController sculptureController = workingSculpture;
				sculptureController.OnLastHit = (Action<SculptureController>)Delegate.Combine(sculptureController.OnLastHit, new Action<SculptureController>(SculptureLastHit));
				SculptureController sculptureController2 = workingSculpture;
				sculptureController2.OnHit = (Action<SculptureController, float>)Delegate.Combine(sculptureController2.OnHit, new Action<SculptureController, float>(SculptureHit));
			}
		}
	}

	private void SculptureFirstHit(SculptureController sculpture)
	{
		if (sculpture != null && workingSculpture == sculpture)
		{
			sculpture.OnFirstHit -= SculptureFirstHit;
			sculpture.Build(modelBase, modelZone.ItemsInCollection.ToArray());
		}
	}

	private void SculptureHit(SculptureController sculpture, float percentage)
	{
		GameEventsManager.Instance.ItemActionOccurredWithAmount(myWorldItem.Data, "USED_PARTIALLY", percentage);
	}

	private void SculptureLastHit(SculptureController sculpture)
	{
		GameEventsManager.Instance.ItemActionOccurred(myWorldItem.Data, "USED");
	}

	private void SculptureRemoved(Rigidbody rb)
	{
		if (workingSculpture != null && rb.gameObject == workingSculpture.gameObject)
		{
			workingSculpture.OnFirstHit -= SculptureFirstHit;
			SculptureController sculptureController = workingSculpture;
			sculptureController.OnLastHit = (Action<SculptureController>)Delegate.Remove(sculptureController.OnLastHit, new Action<SculptureController>(SculptureLastHit));
			SculptureController sculptureController2 = workingSculpture;
			sculptureController2.OnHit = (Action<SculptureController, float>)Delegate.Remove(sculptureController2.OnHit, new Action<SculptureController, float>(SculptureHit));
			workingSculpture = null;
			SummonNewSculpture(false);
		}
	}
}
