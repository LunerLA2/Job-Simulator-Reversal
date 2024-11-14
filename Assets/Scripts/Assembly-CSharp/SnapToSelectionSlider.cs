using System;
using OwlchemyVR;
using UnityEngine;
using UnityEngine.Events;

public class SnapToSelectionSlider : MonoBehaviour
{
	[SerializeField]
	private GrabbableSlider slider;

	[SerializeField]
	private GrabbableItem sliderGrabbableItem;

	[SerializeField]
	private int numberOfSelections;

	[SerializeField]
	private UnityEvent[] optionEvents;

	private float snapTo;

	private void Start()
	{
		numberOfSelections--;
	}

	private void OnEnable()
	{
		GrabbableItem grabbableItem = sliderGrabbableItem;
		grabbableItem.OnReleased = (Action<GrabbableItem>)Delegate.Combine(grabbableItem.OnReleased, new Action<GrabbableItem>(SnapToNearestSelection));
	}

	private void OnDisable()
	{
		GrabbableItem grabbableItem = sliderGrabbableItem;
		grabbableItem.OnReleased = (Action<GrabbableItem>)Delegate.Remove(grabbableItem.OnReleased, new Action<GrabbableItem>(SnapToNearestSelection));
	}

	private void SnapToNearestSelection(GrabbableItem grabbable)
	{
		float num = 0f;
		if (slider.SliderAxis == GrabbableSlider.Axis.X)
		{
			num = slider.transform.localPosition.x;
		}
		else if (slider.SliderAxis == GrabbableSlider.Axis.Y)
		{
			num = slider.transform.localPosition.y;
		}
		else if (slider.SliderAxis == GrabbableSlider.Axis.Z)
		{
			num = slider.transform.localPosition.z;
		}
		int num2 = (int)Mathf.Round(num / ((slider.LowerLimit - slider.UpperLimit) / (float)numberOfSelections));
		if (num2 >= 0 && num2 < optionEvents.Length)
		{
			optionEvents[num2].Invoke();
		}
		float num3 = (float)num2 * ((slider.LowerLimit - slider.UpperLimit) / (float)numberOfSelections);
		slider.transform.localPosition = new Vector3(slider.GetAxisVector().x * num3, slider.GetAxisVector().y * num3, slider.GetAxisVector().z * num3);
	}
}
