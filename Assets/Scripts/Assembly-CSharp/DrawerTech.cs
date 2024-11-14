using System;
using System.Collections.Generic;
using OwlchemyVR;
using UnityEngine;

[RequireComponent(typeof(KinematicRigidbodyVelocityStore))]
public class DrawerTech : MonoBehaviour
{
	[SerializeField]
	private RigidbodyEnterExitTriggerEvents rigidbodyTriggerEvents;

	private List<PickupableItem> pickupableWatchList = new List<PickupableItem>();

	private KinematicRigidbodyVelocityStore velocityStore;

	[SerializeField]
	private Transform contentContainer;

	private void Awake()
	{
		velocityStore = GetComponent<KinematicRigidbodyVelocityStore>();
	}

	private void OnEnable()
	{
		RigidbodyEnterExitTriggerEvents rigidbodyEnterExitTriggerEvents = rigidbodyTriggerEvents;
		rigidbodyEnterExitTriggerEvents.OnRigidbodyEnterTrigger = (Action<Rigidbody>)Delegate.Combine(rigidbodyEnterExitTriggerEvents.OnRigidbodyEnterTrigger, new Action<Rigidbody>(RigidbodyEnteredTrigger));
		RigidbodyEnterExitTriggerEvents rigidbodyEnterExitTriggerEvents2 = rigidbodyTriggerEvents;
		rigidbodyEnterExitTriggerEvents2.OnRigidbodyExitTrigger = (Action<Rigidbody>)Delegate.Combine(rigidbodyEnterExitTriggerEvents2.OnRigidbodyExitTrigger, new Action<Rigidbody>(RigidbodyExitedTrigger));
	}

	private void OnDisable()
	{
		RigidbodyEnterExitTriggerEvents rigidbodyEnterExitTriggerEvents = rigidbodyTriggerEvents;
		rigidbodyEnterExitTriggerEvents.OnRigidbodyEnterTrigger = (Action<Rigidbody>)Delegate.Remove(rigidbodyEnterExitTriggerEvents.OnRigidbodyEnterTrigger, new Action<Rigidbody>(RigidbodyEnteredTrigger));
		RigidbodyEnterExitTriggerEvents rigidbodyEnterExitTriggerEvents2 = rigidbodyTriggerEvents;
		rigidbodyEnterExitTriggerEvents2.OnRigidbodyExitTrigger = (Action<Rigidbody>)Delegate.Remove(rigidbodyEnterExitTriggerEvents2.OnRigidbodyExitTrigger, new Action<Rigidbody>(RigidbodyExitedTrigger));
	}

	private void RigidbodyEnteredTrigger(Rigidbody r)
	{
		PickupableItem component = r.GetComponent<PickupableItem>();
		if (component != null)
		{
			component.OnGrabbed = (Action<GrabbableItem>)Delegate.Combine(component.OnGrabbed, new Action<GrabbableItem>(ItemGrabbed));
			component.OnReleased = (Action<GrabbableItem>)Delegate.Combine(component.OnReleased, new Action<GrabbableItem>(ItemReleased));
			if (!component.IsCurrInHand)
			{
				pickupableWatchList.Add(component);
			}
		}
	}

	private void RigidbodyExitedTrigger(Rigidbody r)
	{
		PickupableItem component = r.GetComponent<PickupableItem>();
		if (component != null)
		{
			component.OnGrabbed = (Action<GrabbableItem>)Delegate.Remove(component.OnGrabbed, new Action<GrabbableItem>(ItemGrabbed));
			component.OnReleased = (Action<GrabbableItem>)Delegate.Remove(component.OnReleased, new Action<GrabbableItem>(ItemReleased));
			pickupableWatchList.Remove(component);
		}
	}

	private void ItemGrabbed(GrabbableItem grabbableItem)
	{
		PickupableItem item = grabbableItem as PickupableItem;
		if (pickupableWatchList.Contains(item))
		{
			pickupableWatchList.Remove(item);
		}
	}

	private void ItemReleased(GrabbableItem grabbableItem)
	{
		if (pickupableWatchList.Contains(grabbableItem as PickupableItem))
		{
			Debug.LogWarning("Some how list still contains item I am releasing");
		}
		pickupableWatchList.Add((PickupableItem)grabbableItem);
	}

	private void Update()
	{
		if (pickupableWatchList.Count <= 0)
		{
			return;
		}
		for (int i = 0; i < pickupableWatchList.Count; i++)
		{
			PickupableItem pickupableItem = pickupableWatchList[i];
			if (pickupableItem != null)
			{
				if (!pickupableItem.IsCurrInHand && IsVelocityRelativeToDrawerCloseToZero(pickupableItem))
				{
					FixedJointInPlace(pickupableItem);
				}
			}
			else
			{
				pickupableWatchList.RemoveAt(i);
				i--;
			}
		}
	}

	private void FixedJointInPlace(PickupableItem item)
	{
		item.Rigidbody.velocity = Vector3.zero;
		item.Rigidbody.angularVelocity = Vector3.zero;
		item.Rigidbody.Sleep();
		pickupableWatchList.Remove(item);
	}

	private bool IsVelocityRelativeToDrawerCloseToZero(PickupableItem item)
	{
		float magnitude = (item.Rigidbody.velocity - velocityStore.CurrVelocity).magnitude;
		return magnitude < 0.04f;
	}
}
