using UnityEngine;

public class PlayAudioInAnimatorLoop : StateMachineBehaviour
{
	[SerializeField]
	private AudioClip[] clip;

	[Range(0f, 1f)]
	[SerializeField]
	private float pointOfOccurance = 0.85f;

	private int loopCount;

	private bool playOnce;

	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (stateInfo.normalizedTime > (float)loopCount)
		{
			loopCount++;
		}
		if ((float)loopCount - stateInfo.normalizedTime < pointOfOccurance && !playOnce)
		{
			int num = Random.Range(0, clip.Length);
			AudioManager.Instance.Play(animator.transform, clip[num], 1f, 1f);
			playOnce = true;
		}
		if ((float)loopCount - stateInfo.normalizedTime > 0.99f)
		{
			playOnce = false;
		}
	}
}
