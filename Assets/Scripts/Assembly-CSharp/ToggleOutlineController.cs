using System;
using OwlchemyVR;
using UnityEngine;

public class ToggleOutlineController : MonoBehaviour
{
	[SerializeField]
	private PickupableItem pickupableItem;

	[SerializeField]
	private SelectedChangeOutlineController outlineController;

	private void OnEnable()
	{
		PickupableItem obj = pickupableItem;
		obj.OnGrabbed = (Action<GrabbableItem>)Delegate.Combine(obj.OnGrabbed, new Action<GrabbableItem>(OnGrab));
		PickupableItem obj2 = pickupableItem;
		obj2.OnReleased = (Action<GrabbableItem>)Delegate.Combine(obj2.OnReleased, new Action<GrabbableItem>(OnRelease));
	}

	private void OnDisable()
	{
		PickupableItem obj = pickupableItem;
		obj.OnGrabbed = (Action<GrabbableItem>)Delegate.Remove(obj.OnGrabbed, new Action<GrabbableItem>(OnGrab));
		PickupableItem obj2 = pickupableItem;
		obj2.OnReleased = (Action<GrabbableItem>)Delegate.Remove(obj2.OnReleased, new Action<GrabbableItem>(OnRelease));
	}

	private void OnGrab(GrabbableItem gi)
	{
		outlineController.enabled = false;
	}

	private void OnRelease(GrabbableItem gi)
	{
		outlineController.enabled = true;
	}
}
