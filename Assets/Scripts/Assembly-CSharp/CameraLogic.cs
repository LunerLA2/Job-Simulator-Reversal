using System;
using OwlchemyVR;
using UnityEngine;

public class CameraLogic : MonoBehaviour
{
	[SerializeField]
	private GrabbableItem grabbableItem;

	[SerializeField]
	private GameObject buttonCollider;

	private void Start()
	{
		GrabbableItem obj = grabbableItem;
		obj.OnGrabbed = (Action<GrabbableItem>)Delegate.Combine(obj.OnGrabbed, new Action<GrabbableItem>(GrabbedCallback));
		GrabbableItem obj2 = grabbableItem;
		obj2.OnReleased = (Action<GrabbableItem>)Delegate.Combine(obj2.OnReleased, new Action<GrabbableItem>(ReleasedCallback));
	}

	private void OnDestroy()
	{
		GrabbableItem obj = grabbableItem;
		obj.OnGrabbed = (Action<GrabbableItem>)Delegate.Remove(obj.OnGrabbed, new Action<GrabbableItem>(GrabbedCallback));
		GrabbableItem obj2 = grabbableItem;
		obj2.OnReleased = (Action<GrabbableItem>)Delegate.Remove(obj2.OnReleased, new Action<GrabbableItem>(ReleasedCallback));
	}

	private void GrabbedCallback(GrabbableItem item)
	{
		buttonCollider.SetActive(true);
	}

	private void ReleasedCallback(GrabbableItem item)
	{
		buttonCollider.SetActive(false);
	}
}
