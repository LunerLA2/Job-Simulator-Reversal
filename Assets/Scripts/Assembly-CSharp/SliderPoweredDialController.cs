using System;
using OwlchemyVR;
using UnityEngine;

public class SliderPoweredDialController : MonoBehaviour
{
	[SerializeField]
	private GrabbableItem sliderGrabbable;

	[SerializeField]
	private GrabbableSlider sliderControl;

	[SerializeField]
	private Transform dialTransform;

	[SerializeField]
	private float spinSpeed = 180f;

	[SerializeField]
	private Vector3 spinAxis = Vector3.right;

	[SerializeField]
	private float numberOfSides = 10f;

	[SerializeField]
	private bool autoSnap = true;

	[SerializeField]
	private bool useSpinInertia;

	[SerializeField]
	private float inertiaDecayTime = 1f;

	private bool isSliding;

	private bool isSnapped = true;

	private float lastSliderPosition;

	private float storedInertia;

	private float releasedTime;

	private int currentSelectedNumber;

	public Action OnSelectedNumberChanged;

	public int CurrentSelectedNumber
	{
		get
		{
			return currentSelectedNumber;
		}
	}

	private void OnEnable()
	{
		GrabbableItem grabbableItem = sliderGrabbable;
		grabbableItem.OnGrabbed = (Action<GrabbableItem>)Delegate.Combine(grabbableItem.OnGrabbed, new Action<GrabbableItem>(SliderGrabbed));
		GrabbableItem grabbableItem2 = sliderGrabbable;
		grabbableItem2.OnReleased = (Action<GrabbableItem>)Delegate.Combine(grabbableItem2.OnReleased, new Action<GrabbableItem>(SliderReleased));
	}

	private void OnDisable()
	{
		GrabbableItem grabbableItem = sliderGrabbable;
		grabbableItem.OnGrabbed = (Action<GrabbableItem>)Delegate.Remove(grabbableItem.OnGrabbed, new Action<GrabbableItem>(SliderGrabbed));
		GrabbableItem grabbableItem2 = sliderGrabbable;
		grabbableItem2.OnReleased = (Action<GrabbableItem>)Delegate.Remove(grabbableItem2.OnReleased, new Action<GrabbableItem>(SliderReleased));
	}

	private void SliderGrabbed(GrabbableItem item)
	{
		isSnapped = false;
		isSliding = true;
		lastSliderPosition = sliderControl.NormalizedOffset;
	}

	private void Update()
	{
		if (isSliding)
		{
			float angle = (storedInertia = (sliderControl.NormalizedOffset - lastSliderPosition) * spinSpeed);
			dialTransform.Rotate(spinAxis, angle);
			lastSliderPosition = sliderControl.NormalizedOffset;
			if ((lastSliderPosition > 1f || lastSliderPosition < 0f) && sliderGrabbable.CurrInteractableHand != null && sliderGrabbable.IsCurrInHand)
			{
				sliderGrabbable.CurrInteractableHand.ManuallyReleaseJoint();
			}
			return;
		}
		if (useSpinInertia && Mathf.Abs(storedInertia) > 0.01f)
		{
			storedInertia = Mathf.Lerp(storedInertia, 0f, (Time.realtimeSinceStartup - releasedTime) / inertiaDecayTime);
			dialTransform.Rotate(spinAxis, storedInertia);
		}
		if (autoSnap && !isSnapped && Mathf.Abs(storedInertia) <= 0.01f)
		{
			SnapToNearestSide();
		}
	}

	private void SliderReleased(GrabbableItem item)
	{
		releasedTime = Time.realtimeSinceStartup;
		ResetSlider();
	}

	private void ResetSlider()
	{
		isSliding = false;
		lastSliderPosition = sliderControl.NormalizedOffset;
		sliderControl.transform.localPosition = Vector3.zero;
	}

	private void SnapToNearestSide()
	{
		Vector3 localEulerAngles = dialTransform.localEulerAngles;
		int num = (int)Mathf.Round(localEulerAngles.x / (360f / numberOfSides));
		float x = (float)num * (360f / numberOfSides);
		dialTransform.localEulerAngles = new Vector3(x, localEulerAngles.y, localEulerAngles.z);
		isSnapped = true;
		currentSelectedNumber = CalculateSelectedNumber(dialTransform.localEulerAngles);
		if (OnSelectedNumberChanged != null)
		{
			OnSelectedNumberChanged();
		}
	}

	private int CalculateSelectedNumber(Vector3 eulerAngles)
	{
		int num = (int)(eulerAngles.x / (360f / numberOfSides));
		if (eulerAngles.y > 100f)
		{
			int num2 = num - 5;
			if (num2 < 0)
			{
				num2 = num + 5;
			}
			return num2;
		}
		if (num > 9 || num == 0)
		{
			return 0;
		}
		return 10 - num;
	}
}
