using System;
using OwlchemyVR;
using UnityEngine;

public class BoxCutterController : MonoBehaviour
{
	[SerializeField]
	private Animator anim;

	private PickupableItem pickupableItem;

	private WorldItem worldItem;

	private void Awake()
	{
		pickupableItem = GetComponent<PickupableItem>();
	}

	private void OnEnable()
	{
		PickupableItem obj = pickupableItem;
		obj.OnGrabbed = (Action<GrabbableItem>)Delegate.Combine(obj.OnGrabbed, new Action<GrabbableItem>(Grabbed));
		PickupableItem obj2 = pickupableItem;
		obj2.OnReleased = (Action<GrabbableItem>)Delegate.Combine(obj2.OnReleased, new Action<GrabbableItem>(Released));
	}

	private void OnDisable()
	{
		PickupableItem obj = pickupableItem;
		obj.OnGrabbed = (Action<GrabbableItem>)Delegate.Remove(obj.OnGrabbed, new Action<GrabbableItem>(Grabbed));
		PickupableItem obj2 = pickupableItem;
		obj2.OnReleased = (Action<GrabbableItem>)Delegate.Remove(obj2.OnReleased, new Action<GrabbableItem>(Released));
	}

	private void Grabbed(GrabbableItem item)
	{
		anim.Play("BoxCutterOpen");
	}

	private void Released(GrabbableItem item)
	{
		anim.Play("BoxCutterClose");
	}
}
