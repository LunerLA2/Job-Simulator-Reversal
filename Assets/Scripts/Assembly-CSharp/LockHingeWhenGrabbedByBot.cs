using System;
using UnityEngine;

public class LockHingeWhenGrabbedByBot : MonoBehaviour
{
	[SerializeField]
	private UniqueObject uniqueObject;

	[SerializeField]
	private GrabbableHinge hingeToLock;

	[SerializeField]
	private GrabbableHinge.InitialLockType lockWhenGrabbed;

	private void OnEnable()
	{
		UniqueObject obj = uniqueObject;
		obj.OnWasPulledIntoInventoryOfBot = (Action<Bot>)Delegate.Combine(obj.OnWasPulledIntoInventoryOfBot, new Action<Bot>(PulledIntoInventoryByBot));
	}

	private void OnDisable()
	{
		UniqueObject obj = uniqueObject;
		obj.OnWasPulledIntoInventoryOfBot = (Action<Bot>)Delegate.Remove(obj.OnWasPulledIntoInventoryOfBot, new Action<Bot>(PulledIntoInventoryByBot));
	}

	private void PulledIntoInventoryByBot(Bot bot)
	{
		if (lockWhenGrabbed == GrabbableHinge.InitialLockType.Lower)
		{
			if (!hingeToLock.IsLowerLocked)
			{
				hingeToLock.LockLower();
			}
		}
		else if (lockWhenGrabbed == GrabbableHinge.InitialLockType.Upper && !hingeToLock.IsUpperLocked)
		{
			hingeToLock.LockUpper();
		}
	}
}
