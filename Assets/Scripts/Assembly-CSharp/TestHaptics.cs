using OwlchemyVR;
using UnityEngine;

public class TestHaptics : MonoBehaviour
{
	public enum HapticTypes
	{
		Default = 0,
		Animation = 1
	}

	[SerializeField]
	private HapticTypes currHapticType;

	[SerializeField]
	[Range(0f, 3999f)]
	private float pulseRateMicroSec;

	[SerializeField]
	private float deltaTimeSpeedMultipler = 1f;

	[SerializeField]
	private float pulseRateMultiplier = 1f;

	[SerializeField]
	private AnimationClip hapticAnimationClip;

	[SerializeField]
	private bool isAnimationLooping;

	private bool hasBeenInit;

	private InteractionHandController[] handControllers;

	private InteractionHandController currHandController;

	private HapticsController currHapticController;

	private HapticInfoObject currHapticInfo;

	private void Awake()
	{
		handControllers = Object.FindObjectsOfType<InteractionHandController>();
	}

	private void Update()
	{
		if (!hasBeenInit)
		{
			for (int i = 0; i < handControllers.Length; i++)
			{
				if (handControllers[i].IsGrabInputButtonDown())
				{
					currHandController = handControllers[i];
					currHapticController = currHandController.HapticsController;
					hasBeenInit = true;
					break;
				}
			}
			return;
		}
		if (currHandController.IsGrabInputButtonDown())
		{
			RemoveCurrentHaptic();
		}
		if (currHapticType == HapticTypes.Animation)
		{
			if (currHapticInfo is AnimationBasedHapticInfoObject)
			{
				if (((AnimationBasedHapticInfoObject)currHapticInfo).AnimClip != hapticAnimationClip)
				{
					RemoveCurrentHaptic();
				}
				if (((AnimationBasedHapticInfoObject)currHapticInfo).IsLooping != isAnimationLooping)
				{
					RemoveCurrentHaptic();
				}
			}
			else if (hapticAnimationClip != null)
			{
				RemoveCurrentHaptic();
				currHapticInfo = currHapticController.AddNewHaptic(new AnimationBasedHapticInfoObject(hapticAnimationClip, currHapticController.HapticAnimationData, isAnimationLooping));
			}
		}
		else if (currHapticType == HapticTypes.Default)
		{
			if (currHapticInfo is AnimationBasedHapticInfoObject)
			{
				RemoveCurrentHaptic();
			}
			if (currHapticInfo == null)
			{
				currHapticInfo = currHapticController.AddNewHaptic(new HapticInfoObject(pulseRateMicroSec));
			}
			currHapticInfo.SetCurrPulseRateMicroSec(pulseRateMicroSec);
		}
		if (currHapticInfo != null)
		{
			currHapticInfo.DeltaTimeSpeedMultiplier = deltaTimeSpeedMultipler;
			currHapticInfo.PulseRateMultiplier = pulseRateMultiplier;
		}
	}

	private void RemoveCurrentHaptic()
	{
		if (currHapticInfo != null)
		{
			currHapticController.RemoveHaptic(currHapticInfo);
			currHapticInfo = null;
		}
	}
}
