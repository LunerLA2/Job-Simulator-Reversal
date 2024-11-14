using UnityEngine;

public class HingeBasedAnimationScrub : MonoBehaviour
{
	[SerializeField]
	private GrabbableHinge hinge;

	[SerializeField]
	private Animation scrubAnimation;

	[SerializeField]
	private float maxScrubSpeed = 3f;

	private float targetScrubAmount;

	private bool isScrubbing;

	private float currentScrubAmount;

	private void Start()
	{
	}

	private void Update()
	{
		ScrubUpdate(Mathf.InverseLerp(hinge.lowerLimit, hinge.upperLimit, hinge.Angle));
	}

	private void ScrubUpdate(float perc)
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
}
