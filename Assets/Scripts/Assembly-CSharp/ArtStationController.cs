using UnityEngine;

public class ArtStationController : KitchenTool
{
	[SerializeField]
	private GrabbableSlider slider;

	[SerializeField]
	private KitchenToolStasher toolStasher;

	private void OnEnable()
	{
		slider.OnUpperLocked += UpperLocked;
		slider.OnLowerLocked += LowerLocked;
	}

	private void OnDisable()
	{
		slider.OnUpperLocked -= UpperLocked;
		slider.OnLowerLocked -= LowerLocked;
	}

	private void UpperLocked(GrabbableSlider slider, bool isInitial)
	{
		toolStasher.RequestModeChange(0);
	}

	private void LowerLocked(GrabbableSlider slider, bool isInitial)
	{
		toolStasher.RequestModeChange(1);
	}
}
