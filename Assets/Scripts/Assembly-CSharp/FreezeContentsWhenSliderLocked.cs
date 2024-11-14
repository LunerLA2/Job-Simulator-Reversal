using UnityEngine;

public class FreezeContentsWhenSliderLocked : FreezeContentsController
{
	[SerializeField]
	private GrabbableSlider sliderToWatch;

	[SerializeField]
	private bool freezeWhenUpperLocked;

	[SerializeField]
	private bool freezeWhenLowerLocked;

	private void OnEnable()
	{
		sliderToWatch.OnLowerLocked += LowerLocked;
		sliderToWatch.OnUpperLocked += UpperLocked;
		sliderToWatch.OnLowerUnlocked += Unlocked;
		sliderToWatch.OnUpperUnlocked += Unlocked;
	}

	private void OnDisable()
	{
		sliderToWatch.OnLowerLocked -= LowerLocked;
		sliderToWatch.OnUpperLocked -= UpperLocked;
		sliderToWatch.OnLowerUnlocked -= Unlocked;
		sliderToWatch.OnUpperUnlocked -= Unlocked;
	}

	private void Unlocked(GrabbableSlider slider)
	{
		SetFreezeState(false);
	}

	private void UpperLocked(GrabbableSlider slider, bool isInitial)
	{
		if (freezeWhenUpperLocked)
		{
			SetFreezeState(true);
		}
	}

	private void LowerLocked(GrabbableSlider slider, bool isInitial)
	{
		if (freezeWhenLowerLocked)
		{
			SetFreezeState(true);
		}
	}
}
