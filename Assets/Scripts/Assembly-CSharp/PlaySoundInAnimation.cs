using UnityEngine;

public class PlaySoundInAnimation : MonoBehaviour
{
	[SerializeField]
	private AudioSourceHelper audioSourceHelper;

	public void PlayHelper(bool loop = false)
	{
		audioSourceHelper.Play();
	}

	public void PlayAudioClipInHelper(AudioClip audioClip, bool loop = false)
	{
		audioSourceHelper.SetClip(audioClip);
		audioSourceHelper.Play();
	}
}
