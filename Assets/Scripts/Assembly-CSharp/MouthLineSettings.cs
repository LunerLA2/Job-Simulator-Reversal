using System;
using UnityEngine;

[Serializable]
public class MouthLineSettings
{
	[SerializeField]
	private float amplitude = 1f;

	[SerializeField]
	private float frequency = 1.5f;

	[SerializeField]
	private float slideAutomatically = 1f;

	[SerializeField]
	private float warpRandom;

	[SerializeField]
	private float volumeBasedAmplitudeAdd = 5f;

	[SerializeField]
	private float volumeBasedFrequencyAdd;

	[SerializeField]
	private float volumeBasedSlideAutomaticallyAdd = 100f;

	[SerializeField]
	private float volumeBasedWarpRandomAdd;

	public void ApplyToLinewave(LineWave lineWave, float volume = 0f)
	{
		lineWave.amplitude = amplitude + volumeBasedAmplitudeAdd / 100f * volume;
		lineWave.frequency = frequency + volumeBasedFrequencyAdd / 100f * volume;
		lineWave.slideAutomatically = slideAutomatically + volumeBasedSlideAutomaticallyAdd / 100f * volume;
		lineWave.warpRandom = warpRandom + volumeBasedWarpRandomAdd / 100f * volume;
	}
}
