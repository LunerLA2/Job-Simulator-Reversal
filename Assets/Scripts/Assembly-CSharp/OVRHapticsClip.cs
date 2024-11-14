using UnityEngine;

public class OVRHapticsClip
{
	public int Count { get; private set; }

	public int Capacity { get; private set; }

	public byte[] Samples { get; private set; }

	public OVRHapticsClip()
	{
		Capacity = OVRHaptics.Config.MaximumBufferSamplesCount;
		Samples = new byte[Capacity * OVRHaptics.Config.SampleSizeInBytes];
	}

	public OVRHapticsClip(int capacity)
	{
		Capacity = ((capacity >= 0) ? capacity : 0);
		Samples = new byte[Capacity * OVRHaptics.Config.SampleSizeInBytes];
	}

	public OVRHapticsClip(byte[] samples, int samplesCount)
	{
		Samples = samples;
		Capacity = Samples.Length / OVRHaptics.Config.SampleSizeInBytes;
		Count = ((samplesCount >= 0) ? samplesCount : 0);
	}

	public OVRHapticsClip(OVRHapticsClip a, OVRHapticsClip b)
	{
		int count = a.Count;
		if (b.Count > count)
		{
			count = b.Count;
		}
		Capacity = count;
		Samples = new byte[Capacity * OVRHaptics.Config.SampleSizeInBytes];
		for (int i = 0; i < a.Count || i < b.Count; i++)
		{
			if (OVRHaptics.Config.SampleSizeInBytes == 1)
			{
				byte sample = 0;
				if (i < a.Count && i < b.Count)
				{
					sample = (byte)Mathf.Clamp(a.Samples[i] + b.Samples[i], 0, 255);
				}
				else if (i < a.Count)
				{
					sample = a.Samples[i];
				}
				else if (i < b.Count)
				{
					sample = b.Samples[i];
				}
				WriteSample(sample);
			}
		}
	}

	public OVRHapticsClip(AudioClip audioClip, int channel = 0)
	{
		float[] array = new float[audioClip.samples * audioClip.channels];
		audioClip.GetData(array, 0);
		InitializeFromAudioFloatTrack(array, audioClip.frequency, audioClip.channels, channel);
	}

	public void WriteSample(byte sample)
	{
		if (Count < Capacity)
		{
			if (OVRHaptics.Config.SampleSizeInBytes == 1)
			{
				Samples[Count * OVRHaptics.Config.SampleSizeInBytes] = sample;
			}
			Count++;
		}
	}

	public void Reset()
	{
		Count = 0;
	}

	private void InitializeFromAudioFloatTrack(float[] sourceData, double sourceFrequency, int sourceChannelCount, int sourceChannel)
	{
		double num = sourceFrequency / (double)OVRHaptics.Config.SampleRateHz;
		int num2 = (int)num;
		double num3 = num - (double)num2;
		double num4 = 0.0;
		int num5 = sourceData.Length;
		Count = 0;
		Capacity = num5 / sourceChannelCount / num2 + 1;
		Samples = new byte[Capacity * OVRHaptics.Config.SampleSizeInBytes];
		int num6 = sourceChannel % sourceChannelCount;
		while (num6 < num5)
		{
			if (OVRHaptics.Config.SampleSizeInBytes == 1)
			{
				WriteSample((byte)(Mathf.Clamp01(Mathf.Abs(sourceData[num6])) * 255f));
			}
			num6 += num2 * sourceChannelCount;
			num4 += num3;
			if ((int)num4 > 0)
			{
				num6 += (int)num4 * sourceChannelCount;
				num4 -= (double)(int)num4;
			}
		}
	}
}
