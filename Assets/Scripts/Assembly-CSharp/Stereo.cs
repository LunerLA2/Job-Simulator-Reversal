using UnityEngine;

public class Stereo : MonoBehaviour
{
	[SerializeField]
	private AudioClip buttonClickAudio;

	[SerializeField]
	private AudioSourceHelper[] speakerAudioSources;

	private bool state;

	public void TogglePower()
	{
		state = !state;
		for (int i = 0; i < speakerAudioSources.Length; i++)
		{
			if (state)
			{
				speakerAudioSources[i].UnPause();
			}
			else
			{
				speakerAudioSources[i].Pause();
			}
		}
		AudioManager.Instance.Play(base.transform.position, buttonClickAudio, 1f, 1f);
	}
}
