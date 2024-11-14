using UnityEngine;

public class Pedestal : MonoBehaviour
{
	[SerializeField]
	private MechanicalPushButtonController button;

	[SerializeField]
	private AudioSourceHelper audioSourceHelper;

	private bool isPlaying;

	private float audioClipEndTime;

	private void Update()
	{
		if (isPlaying && Time.time >= audioClipEndTime)
		{
			isPlaying = false;
		}
	}

	private void OnEnable()
	{
		button.OnButtonPress += PlayAudio;
	}

	private void OnDisable()
	{
		button.OnButtonPress -= PlayAudio;
	}

	private void PlayAudio()
	{
		if (!isPlaying)
		{
			isPlaying = true;
			audioClipEndTime = Time.time + audioSourceHelper.GetClip().length;
			audioSourceHelper.Play();
		}
		else
		{
			audioSourceHelper.Stop();
			isPlaying = false;
		}
	}
}
