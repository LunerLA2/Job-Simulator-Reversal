using System;
using OwlchemyVR;
using UnityEngine;

public class DisableObjectsWhenCloseToHead : MonoBehaviour
{
	[SerializeField]
	private PlayerPartDetector playerPartDetector;

	[SerializeField]
	private GrabbableItem requiredGrabbable;

	[SerializeField]
	private GameObject[] deactivateWhenCloseToHead;

	private bool isCloseToHead;

	private bool isGrabbed;

	private void OnEnable()
	{
		PlayerPartDetector obj = playerPartDetector;
		obj.OnHeadEntered = (Action<PlayerPartDetector, HeadController>)Delegate.Combine(obj.OnHeadEntered, new Action<PlayerPartDetector, HeadController>(GotCloseToHead));
		PlayerPartDetector obj2 = playerPartDetector;
		obj2.OnHeadExited = (Action<PlayerPartDetector, HeadController>)Delegate.Combine(obj2.OnHeadExited, new Action<PlayerPartDetector, HeadController>(MovedAwayFromHead));
		GrabbableItem grabbableItem = requiredGrabbable;
		grabbableItem.OnGrabbed = (Action<GrabbableItem>)Delegate.Combine(grabbableItem.OnGrabbed, new Action<GrabbableItem>(Grabbed));
		GrabbableItem grabbableItem2 = requiredGrabbable;
		grabbableItem2.OnReleased = (Action<GrabbableItem>)Delegate.Combine(grabbableItem2.OnReleased, new Action<GrabbableItem>(Released));
	}

	private void OnDisable()
	{
		PlayerPartDetector obj = playerPartDetector;
		obj.OnHeadEntered = (Action<PlayerPartDetector, HeadController>)Delegate.Remove(obj.OnHeadEntered, new Action<PlayerPartDetector, HeadController>(GotCloseToHead));
		PlayerPartDetector obj2 = playerPartDetector;
		obj2.OnHeadExited = (Action<PlayerPartDetector, HeadController>)Delegate.Remove(obj2.OnHeadExited, new Action<PlayerPartDetector, HeadController>(MovedAwayFromHead));
		GrabbableItem grabbableItem = requiredGrabbable;
		grabbableItem.OnGrabbed = (Action<GrabbableItem>)Delegate.Remove(grabbableItem.OnGrabbed, new Action<GrabbableItem>(Grabbed));
		GrabbableItem grabbableItem2 = requiredGrabbable;
		grabbableItem2.OnReleased = (Action<GrabbableItem>)Delegate.Remove(grabbableItem2.OnReleased, new Action<GrabbableItem>(Released));
	}

	private void Grabbed(GrabbableItem item)
	{
		isGrabbed = true;
		if (!isCloseToHead)
		{
			SetStateOfObjects(true);
		}
	}

	private void Released(GrabbableItem item)
	{
		isGrabbed = false;
		if (!isCloseToHead)
		{
			SetStateOfObjects(false);
		}
	}

	private void GotCloseToHead(PlayerPartDetector ppd, HeadController head)
	{
		isCloseToHead = true;
		if (isGrabbed)
		{
			SetStateOfObjects(false);
		}
	}

	private void MovedAwayFromHead(PlayerPartDetector ppd, HeadController head)
	{
		isCloseToHead = false;
		SetStateOfObjects(true);
	}

	private void SetStateOfObjects(bool state)
	{
		for (int i = 0; i < deactivateWhenCloseToHead.Length; i++)
		{
			deactivateWhenCloseToHead[i].SetActive(state);
		}
	}
}
