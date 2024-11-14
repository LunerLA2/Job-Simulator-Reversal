using System;
using System.Runtime.InteropServices;

namespace UnityEngine.PS4.Engines
{
	public class Depth
	{
		[DllImport("DepthPlugin")]
		private static extern int PrxDepthInitialise(bool HeadTracking, bool HandTracking);

		[DllImport("DepthPlugin")]
		private static extern int PrxDepthShutdown();

		[DllImport("DepthPlugin")]
		private static extern int PrxDepthUpdate(ulong cameraFrameHandle, bool PerformHeadTracking, bool PerformHandTracking);

		[DllImport("DepthPlugin")]
		private static extern int PrxDepthQueueUpdate(ulong cameraFrameHandle, bool PerformHeadTracking, bool PerformHandTracking);

		[DllImport("DepthPlugin")]
		private static extern int PrxDepthSetRoi(float sx, float sy, float sz, float sw);

		[DllImport("DepthPlugin")]
		private static extern int PrxDepthHeadCandidateTrackerGetResult(out IntPtr results, int numResults);

		[DllImport("DepthPlugin")]
		private static extern int PrxDepthHandCandidateTrackerGetResult(out IntPtr results, int numResults);

		[DllImport("DepthPlugin")]
		private static extern int PrxDepthHeadCandidateTrackerSetValidationInformation(IntPtr results, int numResults);

		[DllImport("DepthPlugin")]
		private static extern IntPtr PrxDepthGetFrameIntPtr();

		[DllImport("DepthPlugin")]
		private static extern int PrxDepthGetValidatedHeadResults(ref IntPtr results, int numResults);

		public static int Init(bool enableHeadTracking, bool enableHandTracking)
		{
			return PrxDepthInitialise(enableHeadTracking, enableHandTracking);
		}

		public static int Term()
		{
			return PrxDepthShutdown();
		}

		public static int QueueUpdate(ulong cameraFrameHandle, bool HeadTracking, bool HandTracking)
		{
			return PrxDepthQueueUpdate(cameraFrameHandle, HeadTracking, HandTracking);
		}

		public static int Update(ulong cameraFrameHandle, bool HeadTracking, bool HandTracking)
		{
			return PrxDepthUpdate(cameraFrameHandle, HeadTracking, HandTracking);
		}

		public static int SetRoi(float sx, float sy, float sz, float sw)
		{
			return PrxDepthSetRoi(sx, sy, sz, sw);
		}

		public static IntPtr getFrameIntPtr()
		{
			return PrxDepthGetFrameIntPtr();
		}

		public static int HeadCandidateTrackerGetResult(out SceDepthHeadCandidateTrackingResult[] data, int maxnumresults)
		{
			int num = 0;
			int num2 = 7;
			IntPtr results = Marshal.AllocHGlobal(num2 * 4 * maxnumresults);
			num = PrxDepthHeadCandidateTrackerGetResult(out results, maxnumresults);
			if (num > 0)
			{
				data = new SceDepthHeadCandidateTrackingResult[num];
				for (int i = 0; i < num; i++)
				{
					IntPtr ptr = new IntPtr(results.ToInt64() + 28 * i);
					data[i] = (SceDepthHeadCandidateTrackingResult)Marshal.PtrToStructure(ptr, typeof(SceDepthHeadCandidateTrackingResult));
				}
			}
			else
			{
				data = new SceDepthHeadCandidateTrackingResult[0];
			}
			Marshal.FreeHGlobal(results);
			return num;
		}

		public static int GetValidatedHeadResults(out SceDepthHeadCandidateTrackingResult[] data, int maxnumresults)
		{
			int num = 0;
			int num2 = 7;
			IntPtr results = Marshal.AllocHGlobal(num2 * 4 * maxnumresults);
			num = PrxDepthGetValidatedHeadResults(ref results, maxnumresults);
			if (num > 0)
			{
				data = new SceDepthHeadCandidateTrackingResult[num];
				for (int i = 0; i < num; i++)
				{
					IntPtr ptr = new IntPtr(results.ToInt64() + num2 * 4 * i);
					data[i] = (SceDepthHeadCandidateTrackingResult)Marshal.PtrToStructure(ptr, typeof(SceDepthHeadCandidateTrackingResult));
				}
			}
			else
			{
				data = new SceDepthHeadCandidateTrackingResult[0];
			}
			Marshal.FreeHGlobal(results);
			return num;
		}

		public static int HandCandidateTrackerGetResult(out SceDepthHandCandidateTrackingResult[] data, int maxnumresults)
		{
			int num = 0;
			int num2 = 5;
			IntPtr results = Marshal.AllocHGlobal(num2 * 4 * maxnumresults);
			num = PrxDepthHandCandidateTrackerGetResult(out results, maxnumresults);
			if (num > 0)
			{
				data = new SceDepthHandCandidateTrackingResult[num];
				for (int i = 0; i < num; i++)
				{
					IntPtr ptr = new IntPtr(results.ToInt64() + num2 * 4 * i);
					data[i] = (SceDepthHandCandidateTrackingResult)Marshal.PtrToStructure(ptr, typeof(SceDepthHandCandidateTrackingResult));
				}
			}
			else
			{
				data = new SceDepthHandCandidateTrackingResult[0];
			}
			Marshal.FreeHGlobal(results);
			return num;
		}

		public static int HeadCandidateTrackerSetValidationInformation(SceDepthTrackingResultValidationInformation[] data)
		{
			int num = data.Length;
			int num2 = 2;
			IntPtr intPtr = Marshal.AllocHGlobal(num2 * 4 * num);
			Marshal.StructureToPtr(data, intPtr, false);
			int result = PrxDepthHeadCandidateTrackerSetValidationInformation(intPtr, num);
			Marshal.FreeHGlobal(intPtr);
			return result;
		}
	}
}
