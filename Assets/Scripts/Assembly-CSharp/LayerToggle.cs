using System;
using OwlchemyVR;
using UnityEngine;

public class LayerToggle : MonoBehaviour
{
	private int initialLayer;

	[SerializeField]
	private PickupableItem pickupableItem;

	private void Start()
	{
		initialLayer = base.gameObject.layer;
		PickupableItem obj = pickupableItem;
		obj.OnGrabbed = (Action<GrabbableItem>)Delegate.Combine(obj.OnGrabbed, new Action<GrabbableItem>(Grabbed));
		PickupableItem obj2 = pickupableItem;
		obj2.OnReleased = (Action<GrabbableItem>)Delegate.Combine(obj2.OnReleased, new Action<GrabbableItem>(Released));
	}

	private void OnDestroy()
	{
		PickupableItem obj = pickupableItem;
		obj.OnGrabbed = (Action<GrabbableItem>)Delegate.Remove(obj.OnGrabbed, new Action<GrabbableItem>(Grabbed));
		PickupableItem obj2 = pickupableItem;
		obj2.OnReleased = (Action<GrabbableItem>)Delegate.Remove(obj2.OnReleased, new Action<GrabbableItem>(Released));
	}

	private void Grabbed(GrabbableItem item)
	{
		base.gameObject.layer = 13;
	}

	private void Released(GrabbableItem item)
	{
		base.gameObject.layer = initialLayer;
	}
}
