using UnityEngine;

namespace Oculus.Platform
{
	public class MicrophoneInput : IMicrophone
	{
		private AudioClip microphoneClip;

		private int lastMicrophoneSample;

		private int micBufferSizeSamples;

		public MicrophoneInput()
		{
			int num = 1;
			int num2 = 48000;
			microphoneClip = Microphone.Start(null, true, num, num2);
			micBufferSizeSamples = num * num2;
		}

		public void Start()
		{
		}

		public void Stop()
		{
		}

		public float[] Update()
		{
			int position = Microphone.GetPosition(null);
			if (position == 0 && lastMicrophoneSample == 0)
			{
				Debug.LogWarning("Possible microphone error");
			}
			int num = 0;
			if (position < lastMicrophoneSample)
			{
				int num2 = micBufferSizeSamples - lastMicrophoneSample;
				num = num2 + position;
			}
			else
			{
				num = position - lastMicrophoneSample;
			}
			if (num == 0)
			{
				return null;
			}
			float[] array = new float[num];
			microphoneClip.GetData(array, lastMicrophoneSample);
			lastMicrophoneSample = position;
			return array;
		}
	}
}
