using System;
using OwlchemyVR;
using UnityEngine;

public class ChangeParentWhenGrabbed : MonoBehaviour
{
	[SerializeField]
	private GrabbableItem grabbableToListenTo;

	[SerializeField]
	private Transform setParentTo;

	private void OnEnable()
	{
		GrabbableItem grabbableItem = grabbableToListenTo;
		grabbableItem.OnGrabbed = (Action<GrabbableItem>)Delegate.Combine(grabbableItem.OnGrabbed, new Action<GrabbableItem>(Grabbed));
	}

	private void OnDisable()
	{
		GrabbableItem grabbableItem = grabbableToListenTo;
		grabbableItem.OnGrabbed = (Action<GrabbableItem>)Delegate.Remove(grabbableItem.OnGrabbed, new Action<GrabbableItem>(Grabbed));
	}

	private void Grabbed(GrabbableItem item)
	{
		if (setParentTo != null)
		{
			grabbableToListenTo.transform.SetParent(setParentTo, true);
		}
		else
		{
			grabbableToListenTo.transform.SetParent(GlobalStorage.Instance.ContentRoot, true);
		}
	}
}
