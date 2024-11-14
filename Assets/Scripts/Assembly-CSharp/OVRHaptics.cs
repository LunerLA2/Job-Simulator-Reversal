using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public static class OVRHaptics
{
	public static class Config
	{
		public static int SampleRateHz { get; private set; }

		public static int SampleSizeInBytes { get; private set; }

		public static int MinimumSafeSamplesQueued { get; private set; }

		public static int MinimumBufferSamplesCount { get; private set; }

		public static int OptimalBufferSamplesCount { get; private set; }

		public static int MaximumBufferSamplesCount { get; private set; }

		static Config()
		{
			Load();
		}

		public static void Load()
		{
			OVRPlugin.HapticsDesc controllerHapticsDesc = OVRPlugin.GetControllerHapticsDesc(2u);
			SampleRateHz = controllerHapticsDesc.SampleRateHz;
			SampleSizeInBytes = controllerHapticsDesc.SampleSizeInBytes;
			MinimumSafeSamplesQueued = controllerHapticsDesc.MinimumSafeSamplesQueued;
			MinimumBufferSamplesCount = controllerHapticsDesc.MinimumBufferSamplesCount;
			OptimalBufferSamplesCount = controllerHapticsDesc.OptimalBufferSamplesCount;
			MaximumBufferSamplesCount = controllerHapticsDesc.MaximumBufferSamplesCount;
		}
	}

	public class OVRHapticsChannel
	{
		private OVRHapticsOutput m_output;

		public OVRHapticsChannel(uint outputIndex)
		{
			m_output = m_outputs[outputIndex];
		}

		public void Preempt(OVRHapticsClip clip)
		{
			m_output.Preempt(clip);
		}

		public void Queue(OVRHapticsClip clip)
		{
			m_output.Queue(clip);
		}

		public void Mix(OVRHapticsClip clip)
		{
			m_output.Mix(clip);
		}

		public void Clear()
		{
			m_output.Clear();
		}
	}

	private class OVRHapticsOutput
	{
		private class NativeBuffer
		{
			private int m_numBytes;

			private IntPtr m_ptr = IntPtr.Zero;

			public NativeBuffer(int numBytes)
			{
				m_ptr = Marshal.AllocHGlobal(numBytes);
				m_numBytes = numBytes;
			}

			public IntPtr GetPointer(int byteOffset = 0)
			{
				if (byteOffset < 0 || byteOffset >= m_numBytes)
				{
					return IntPtr.Zero;
				}
				return (byteOffset != 0) ? new IntPtr(m_ptr.ToInt64() + byteOffset) : m_ptr;
			}

			~NativeBuffer()
			{
				if (m_ptr != IntPtr.Zero)
				{
					Marshal.FreeHGlobal(m_ptr);
					m_ptr = IntPtr.Zero;
					m_numBytes = 0;
				}
			}
		}

		private class ClipPlaybackTracker
		{
			public int ReadCount { get; set; }

			public OVRHapticsClip Clip { get; set; }

			public ClipPlaybackTracker(OVRHapticsClip clip)
			{
				Clip = clip;
			}
		}

		private bool m_lowLatencyMode = true;

		private int m_prevSamplesQueued;

		private float m_prevSamplesQueuedTime;

		private int m_numPredictionHits;

		private int m_numPredictionMisses;

		private int m_numUnderruns;

		private List<ClipPlaybackTracker> m_pendingClips = new List<ClipPlaybackTracker>();

		private uint m_controller;

		private NativeBuffer m_nativeBuffer = new NativeBuffer(Config.MaximumBufferSamplesCount * Config.SampleSizeInBytes);

		private OVRHapticsClip m_paddingClip = new OVRHapticsClip();

		public OVRHapticsOutput(uint controller)
		{
			m_controller = controller;
		}

		public void Process()
		{
			OVRPlugin.HapticsState controllerHapticsState = OVRPlugin.GetControllerHapticsState(m_controller);
			float num = Time.realtimeSinceStartup - m_prevSamplesQueuedTime;
			if (m_prevSamplesQueued > 0)
			{
				int num2 = m_prevSamplesQueued - (int)(num * (float)Config.SampleRateHz + 0.5f);
				if (num2 < 0)
				{
					num2 = 0;
				}
				if (controllerHapticsState.SamplesQueued - num2 == 0)
				{
					m_numPredictionHits++;
				}
				else
				{
					m_numPredictionMisses++;
				}
				if (num2 > 0 && controllerHapticsState.SamplesQueued == 0)
				{
					m_numUnderruns++;
				}
				m_prevSamplesQueued = controllerHapticsState.SamplesQueued;
				m_prevSamplesQueuedTime = Time.realtimeSinceStartup;
			}
			int num3 = Config.OptimalBufferSamplesCount;
			if (m_lowLatencyMode)
			{
				float num4 = 1000f / (float)Config.SampleRateHz;
				float num5 = num * 1000f;
				int num6 = (int)Mathf.Ceil(num5 / num4);
				int num7 = Config.MinimumSafeSamplesQueued + num6;
				if (num7 < num3)
				{
					num3 = num7;
				}
			}
			if (controllerHapticsState.SamplesQueued > num3)
			{
				return;
			}
			if (num3 > Config.MaximumBufferSamplesCount)
			{
				num3 = Config.MaximumBufferSamplesCount;
			}
			if (num3 > controllerHapticsState.SamplesAvailable)
			{
				num3 = controllerHapticsState.SamplesAvailable;
			}
			int num8 = 0;
			int num9 = 0;
			while (num8 < num3 && num9 < m_pendingClips.Count)
			{
				int num10 = num3 - num8;
				int num11 = m_pendingClips[num9].Clip.Count - m_pendingClips[num9].ReadCount;
				if (num10 > num11)
				{
					num10 = num11;
				}
				if (num10 > 0)
				{
					int length = num10 * Config.SampleSizeInBytes;
					int byteOffset = num8 * Config.SampleSizeInBytes;
					int startIndex = m_pendingClips[num9].ReadCount * Config.SampleSizeInBytes;
					Marshal.Copy(m_pendingClips[num9].Clip.Samples, startIndex, m_nativeBuffer.GetPointer(byteOffset), length);
					m_pendingClips[num9].ReadCount += num10;
					num8 += num10;
				}
				num9++;
			}
			int num12 = m_pendingClips.Count - 1;
			while (num12 >= 0 && m_pendingClips.Count > 0)
			{
				if (m_pendingClips[num12].ReadCount >= m_pendingClips[num12].Clip.Count)
				{
					m_pendingClips.RemoveAt(num12);
				}
				num12--;
			}
			int num13 = num3 - (controllerHapticsState.SamplesQueued + num8);
			if (num13 < Config.MinimumBufferSamplesCount - num8)
			{
				num13 = Config.MinimumBufferSamplesCount - num8;
			}
			if (num13 > controllerHapticsState.SamplesAvailable)
			{
				num13 = controllerHapticsState.SamplesAvailable;
			}
			if (num13 > 0)
			{
				int length2 = num13 * Config.SampleSizeInBytes;
				int byteOffset2 = num8 * Config.SampleSizeInBytes;
				int startIndex2 = 0;
				Marshal.Copy(m_paddingClip.Samples, startIndex2, m_nativeBuffer.GetPointer(byteOffset2), length2);
				num8 += num13;
			}
			if (num8 > 0)
			{
				OVRPlugin.HapticsBuffer hapticsBuffer = default(OVRPlugin.HapticsBuffer);
				hapticsBuffer.Samples = m_nativeBuffer.GetPointer();
				hapticsBuffer.SamplesCount = num8;
				OVRPlugin.SetControllerHaptics(m_controller, hapticsBuffer);
				m_prevSamplesQueued = OVRPlugin.GetControllerHapticsState(m_controller).SamplesQueued;
				m_prevSamplesQueuedTime = Time.realtimeSinceStartup;
			}
		}

		public void Preempt(OVRHapticsClip clip)
		{
			m_pendingClips.Clear();
			m_pendingClips.Add(new ClipPlaybackTracker(clip));
		}

		public void Queue(OVRHapticsClip clip)
		{
			m_pendingClips.Add(new ClipPlaybackTracker(clip));
		}

		public void Mix(OVRHapticsClip clip)
		{
			int num = 0;
			int num2 = 0;
			int num3 = clip.Count;
			while (num3 > 0 && num < m_pendingClips.Count)
			{
				int num4 = m_pendingClips[num].Clip.Count - m_pendingClips[num].ReadCount;
				num3 -= num4;
				num2 += num4;
				num++;
			}
			if (num3 > 0)
			{
				num2 += num3;
				num3 = 0;
			}
			if (num > 0)
			{
				OVRHapticsClip oVRHapticsClip = new OVRHapticsClip(num2);
				int i = 0;
				for (int j = 0; j < num; j++)
				{
					OVRHapticsClip clip2 = m_pendingClips[j].Clip;
					for (int k = m_pendingClips[j].ReadCount; k < clip2.Count; k++)
					{
						if (Config.SampleSizeInBytes == 1)
						{
							byte sample = 0;
							if (i < clip.Count && k < clip2.Count)
							{
								sample = (byte)Mathf.Clamp(clip.Samples[i] + clip2.Samples[k], 0, 255);
								i++;
							}
							else if (k < clip2.Count)
							{
								sample = clip2.Samples[k];
							}
							oVRHapticsClip.WriteSample(sample);
						}
					}
				}
				for (; i < clip.Count; i++)
				{
					if (Config.SampleSizeInBytes == 1)
					{
						oVRHapticsClip.WriteSample(clip.Samples[i]);
					}
				}
				m_pendingClips[0] = new ClipPlaybackTracker(oVRHapticsClip);
				for (int l = 1; l < num; l++)
				{
					m_pendingClips.RemoveAt(1);
				}
			}
			else
			{
				m_pendingClips.Add(new ClipPlaybackTracker(clip));
			}
		}

		public void Clear()
		{
			m_pendingClips.Clear();
		}
	}

	public static readonly OVRHapticsChannel[] Channels;

	public static readonly OVRHapticsChannel LeftChannel;

	public static readonly OVRHapticsChannel RightChannel;

	private static readonly OVRHapticsOutput[] m_outputs;

	static OVRHaptics()
	{
		Config.Load();
		m_outputs = new OVRHapticsOutput[2]
		{
			new OVRHapticsOutput(1u),
			new OVRHapticsOutput(2u)
		};
		Channels = new OVRHapticsChannel[2]
		{
			LeftChannel = new OVRHapticsChannel(0u),
			RightChannel = new OVRHapticsChannel(1u)
		};
	}

	public static void Process()
	{
		Config.Load();
		for (int i = 0; i < m_outputs.Length; i++)
		{
			m_outputs[i].Process();
		}
	}
}
