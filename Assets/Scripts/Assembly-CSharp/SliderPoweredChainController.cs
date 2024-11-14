using System;
using OwlchemyVR;
using UnityEngine;

public class SliderPoweredChainController : MonoBehaviour
{
	[SerializeField]
	private GrabbableItem sliderGrabbable;

	[SerializeField]
	private GrabbableSlider sliderControl;

	[SerializeField]
	private Transform grabbableChainTransform;

	[SerializeField]
	private Transform optionalReverseChainTransform;

	[SerializeField]
	private bool useSpinInertia;

	[SerializeField]
	private float inertiaDecayTime = 1f;

	[SerializeField]
	private float speedMultiplier = 2f;

	[SerializeField]
	private WorldItem myWorldItem;

	[SerializeField]
	private Animation scrubAnimation;

	[SerializeField]
	private float animationMovementRatio = 0.3f;

	[SerializeField]
	private float maxScrubSpeed = 1f;

	private float previousHeight;

	private float currentHeight;

	private float percentDoorOpen;

	private float targetScrubAmount;

	private bool isScrubbing;

	private float currentScrubAmount;

	private bool isSliding;

	private float lastSliderPosition;

	private float storedInertia;

	private float releasedTime;

	private bool isActivated;

	private void Start()
	{
		previousHeight = base.transform.position.y;
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
		isSliding = true;
		lastSliderPosition = sliderControl.NormalizedOffset;
	}

	private void SliderReleased(GrabbableItem item)
	{
		releasedTime = Time.realtimeSinceStartup;
		ResetSlider();
	}

	private void Update()
	{
		currentHeight = grabbableChainTransform.transform.position.y;
		if (isSliding)
		{
			SliderUpdate();
		}
		else if (useSpinInertia && Mathf.Abs(storedInertia) > 0.01f)
		{
			storedInertia = Mathf.Lerp(storedInertia, 0f, (Time.realtimeSinceStartup - releasedTime) / inertiaDecayTime);
			grabbableChainTransform.Translate(-Vector3.up * storedInertia);
		}
		if (scrubAnimation != null)
		{
			ScrubUpdate();
		}
		previousHeight = currentHeight;
	}

	private void SliderUpdate()
	{
		float num = (storedInertia = sliderControl.NormalizedOffset - lastSliderPosition);
		grabbableChainTransform.Translate(-Vector3.up * num * speedMultiplier);
		if (optionalReverseChainTransform != null)
		{
			optionalReverseChainTransform.Translate(Vector3.up * num * speedMultiplier);
		}
		lastSliderPosition = sliderControl.NormalizedOffset;
		if ((lastSliderPosition > 1f || lastSliderPosition < 0f) && sliderGrabbable.CurrInteractableHand != null && sliderGrabbable.IsCurrInHand)
		{
			sliderGrabbable.CurrInteractableHand.ManuallyReleaseJoint();
		}
	}

	private void ScrubUpdate()
	{
		percentDoorOpen = Mathf.Clamp(percentDoorOpen + (previousHeight - currentHeight) * animationMovementRatio, 0f, 1f);
		ScrubAnimation(percentDoorOpen);
		if (!isActivated && percentDoorOpen == 1f)
		{
			if (myWorldItem != null)
			{
				GameEventsManager.Instance.ItemActionOccurred(myWorldItem.Data, "ACTIVATED");
			}
			if (sliderGrabbable.CurrInteractableHand != null && sliderGrabbable.IsCurrInHand)
			{
				sliderGrabbable.CurrInteractableHand.ManuallyReleaseJoint();
			}
			sliderGrabbable.enabled = false;
			isActivated = true;
		}
		else if (isActivated && percentDoorOpen == 0f)
		{
			if (myWorldItem != null)
			{
				GameEventsManager.Instance.ItemActionOccurred(myWorldItem.Data, "DEACTIVATED");
			}
			isActivated = false;
		}
	}

	public void CloseDoor()
	{
	}

	private void ScrubAnimation(float perc)
	{
		targetScrubAmount = perc;
		bool flag = isScrubbing;
		if (currentScrubAmount < targetScrubAmount)
		{
			currentScrubAmount = Mathf.Min(currentScrubAmount + Time.deltaTime * maxScrubSpeed, targetScrubAmount);
			isScrubbing = true;
		}
		else if (currentScrubAmount > targetScrubAmount)
		{
			currentScrubAmount = Mathf.Max(currentScrubAmount - Time.deltaTime * maxScrubSpeed, targetScrubAmount);
			isScrubbing = true;
		}
		else
		{
			isScrubbing = false;
		}
		if (flag)
		{
			float time = currentScrubAmount * scrubAnimation.clip.length;
			scrubAnimation[scrubAnimation.clip.name].enabled = true;
			scrubAnimation[scrubAnimation.clip.name].weight = 1f;
			scrubAnimation[scrubAnimation.clip.name].time = time;
			scrubAnimation.Sample();
			scrubAnimation[scrubAnimation.clip.name].enabled = false;
		}
	}

	private void ResetSlider()
	{
		isSliding = false;
		lastSliderPosition = sliderControl.NormalizedOffset;
		sliderControl.transform.localPosition = Vector3.zero;
	}
}
