using UnityEngine;

public class KitchenToolWithDoor : KitchenTool
{
	[SerializeField]
	private GrabbableHinge hingeToLock;

	[SerializeField]
	private GrabbableHinge.InitialLockType lockOnDismiss = GrabbableHinge.InitialLockType.Lower;

	public override void OnDismiss()
	{
		if (lockOnDismiss == GrabbableHinge.InitialLockType.Lower)
		{
			if (!hingeToLock.IsLowerLocked)
			{
				hingeToLock.LockLower();
			}
		}
		else if (lockOnDismiss == GrabbableHinge.InitialLockType.Upper && !hingeToLock.IsUpperLocked)
		{
			hingeToLock.LockUpper();
		}
		base.OnDismiss();
	}
}
