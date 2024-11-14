using OwlchemyVR;
using UnityEngine;

public class AnimationBasedHapticInfoObject : HapticInfoObject
{
	private const float ANIMATION_GENERAL_SPEED_MULTIPLIER = 100f;

	private AnimationClip animClip;

	private HapticsAnimationData manualHapticsAnimationData;

	private GameObject manualHapticsAnimationDataGO;

	private bool isLooping;

	public AnimationClip AnimClip
	{
		get
		{
			return animClip;
		}
	}

	public bool IsLooping
	{
		get
		{
			return isLooping;
		}
	}

	public AnimationBasedHapticInfoObject(AnimationClip animClip, HapticsAnimationData manualHapticsAnimationData, bool isLooping, bool isAutoSetElapsedUsingDeltaTime = true)
	{
		this.animClip = animClip;
		this.manualHapticsAnimationData = manualHapticsAnimationData;
		manualHapticsAnimationDataGO = manualHapticsAnimationData.gameObject;
		base.isAutoSetElapsedUsingDeltaTime = isAutoSetElapsedUsingDeltaTime;
		length = animClip.length / 100f;
		pulseRateMicroSec = 0f;
		this.isLooping = isLooping;
		if (isLooping)
		{
			doesHaveLength = false;
		}
		else
		{
			doesHaveLength = true;
		}
	}

	public override void RunHapticsUpdate(float deltaTime)
	{
		if (isRunning)
		{
			base.RunHapticsUpdate(deltaTime);
			if (isLooping && elapsed > length)
			{
				elapsed %= length;
			}
			animClip.SampleAnimation(manualHapticsAnimationDataGO, elapsed * 100f);
			pulseRateMicroSec = manualHapticsAnimationData.pulseRateMicroSec;
		}
	}
}
