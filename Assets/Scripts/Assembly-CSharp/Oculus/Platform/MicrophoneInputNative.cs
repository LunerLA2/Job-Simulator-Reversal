using System;
using UnityEngine;

namespace Oculus.Platform
{
	public class MicrophoneInputNative : IMicrophone
	{
		private IntPtr mic;

		private int tempBufferSize = 9600;

		private float[] tempBuffer;

		public MicrophoneInputNative()
		{
			mic = CAPI.ovr_Microphone_Create();
			CAPI.ovr_Microphone_Start(mic);
			tempBuffer = new float[tempBufferSize];
			Debug.Log(mic);
		}

		public float[] Update()
		{
			ulong num = (ulong)CAPI.ovr_Microphone_ReadData(mic, tempBuffer, (UIntPtr)(ulong)tempBufferSize);
			if (num != 0)
			{
				float[] array = new float[num];
				Array.Copy(tempBuffer, array, (int)num);
				return array;
			}
			return null;
		}

		public void Start()
		{
		}

		public void Stop()
		{
			CAPI.ovr_Microphone_Stop(mic);
			CAPI.ovr_Microphone_Destroy(mic);
		}
	}
}
