using System;
using OwlchemyVR;
using UnityEngine;

public class EnableFluidOnPickup : MonoBehaviour
{
	[SerializeField]
	private ContainerFluidSystem containerFluidSystem;

	[SerializeField]
	private PickupableItem pickupableItem;

	[SerializeField]
	private bool KinematicOnAwake;

	private void Awake()
	{
		if (KinematicOnAwake)
		{
			GetComponent<Rigidbody>().isKinematic = true;
		}
		containerFluidSystem.SetIsPouringEnabled(false);
		PickupableItem obj = pickupableItem;
		obj.OnGrabbed = (Action<GrabbableItem>)Delegate.Combine(obj.OnGrabbed, new Action<GrabbableItem>(OnGrab));
	}

	private void OnGrab(GrabbableItem obj)
	{
		containerFluidSystem.SetIsPouringEnabled(true);
	}
}
