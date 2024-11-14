using System;
using OwlchemyVR;
using UnityEngine;
using UnityEngine.Serialization;

public class ChangeKinematicWhenGrabbed : MonoBehaviour
{
	[FormerlySerializedAs("pickupableToListenTo")]
	[SerializeField]
	private GrabbableItem grabbableToListenTo;

	[SerializeField]
	private Rigidbody rigidbodyToModify;

	[SerializeField]
	private bool setIsKinematicTo;

	[SerializeField]
	private bool setKinematicOnReleased;

	private void OnEnable()
	{
		GrabbableItem grabbableItem = grabbableToListenTo;
		grabbableItem.OnGrabbed = (Action<GrabbableItem>)Delegate.Combine(grabbableItem.OnGrabbed, new Action<GrabbableItem>(Grabbed));
		GrabbableItem grabbableItem2 = grabbableToListenTo;
		grabbableItem2.OnReleased = (Action<GrabbableItem>)Delegate.Combine(grabbableItem2.OnReleased, new Action<GrabbableItem>(Released));
	}

	private void OnDisable()
	{
		GrabbableItem grabbableItem = grabbableToListenTo;
		grabbableItem.OnGrabbed = (Action<GrabbableItem>)Delegate.Remove(grabbableItem.OnGrabbed, new Action<GrabbableItem>(Grabbed));
		GrabbableItem grabbableItem2 = grabbableToListenTo;
		grabbableItem2.OnReleased = (Action<GrabbableItem>)Delegate.Remove(grabbableItem2.OnReleased, new Action<GrabbableItem>(Released));
	}

	private void Grabbed(GrabbableItem item)
	{
		rigidbodyToModify.isKinematic = setIsKinematicTo;
	}

	private void Released(GrabbableItem item)
	{
		if (setKinematicOnReleased)
		{
			rigidbodyToModify.isKinematic = true;
		}
	}
}
