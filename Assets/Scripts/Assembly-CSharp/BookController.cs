using System;
using OwlchemyVR;
using UnityEngine;

public class BookController : MonoBehaviour
{
	[SerializeField]
	private HingeJoint[] bookHinges;

	[SerializeField]
	private PickupableItem pickupableItem;

	private float springForceWhenHolding = 1f;

	private float springForceWhenNotHolding = 8f;

	private void Awake()
	{
		Released(null);
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
		for (int i = 0; i < bookHinges.Length; i++)
		{
			SetSpringForce(bookHinges[i], springForceWhenHolding);
		}
	}

	private void Released(GrabbableItem item)
	{
		for (int i = 0; i < bookHinges.Length; i++)
		{
			SetSpringForce(bookHinges[i], springForceWhenNotHolding);
		}
	}

	private void SetSpringForce(HingeJoint joint, float springAmt)
	{
		JointSpring spring = default(JointSpring);
		spring.spring = springAmt;
		joint.spring = spring;
	}
}
