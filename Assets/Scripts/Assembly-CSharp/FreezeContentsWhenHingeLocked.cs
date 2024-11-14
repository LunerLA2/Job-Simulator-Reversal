using UnityEngine;

public class FreezeContentsWhenHingeLocked : FreezeContentsController
{
	[SerializeField]
	private GrabbableHinge hingeToWatch;

	[SerializeField]
	private bool freezeWhenUpperLocked;

	[SerializeField]
	private bool freezeWhenLowerLocked;

	private void OnEnable()
	{
		hingeToWatch.OnUpperLocked += UpperLocked;
		hingeToWatch.OnLowerLocked += LowerLocked;
		hingeToWatch.OnUpperUnlocked += Unlocked;
		hingeToWatch.OnLowerUnlocked += Unlocked;
	}

	private void OnDisable()
	{
		hingeToWatch.OnUpperLocked -= UpperLocked;
		hingeToWatch.OnLowerLocked -= LowerLocked;
		hingeToWatch.OnUpperUnlocked -= Unlocked;
		hingeToWatch.OnLowerUnlocked -= Unlocked;
	}

	private void Unlocked(GrabbableHinge hinge)
	{
		SetFreezeState(false);
	}

	private void UpperLocked(GrabbableHinge hinge, bool isInitial)
	{
		if (freezeWhenUpperLocked)
		{
			SetFreezeState(true);
		}
	}

	private void LowerLocked(GrabbableHinge hinge, bool isInitial)
	{
		if (freezeWhenLowerLocked)
		{
			SetFreezeState(true);
		}
	}
}
