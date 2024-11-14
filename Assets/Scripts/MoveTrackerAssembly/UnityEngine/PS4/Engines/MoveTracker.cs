using System;
using System.Runtime.InteropServices;

namespace UnityEngine.PS4.Engines
{
	public class MoveTracker
	{
		[DllImport("MoveTrackerPlugin")]
		private static extern int PrxMoveTrackerInitialise();

		[DllImport("MoveTrackerPlugin")]
		private static extern int PrxMoveTrackerShutdown();

		[DllImport("MoveTrackerPlugin")]
		private static extern int PrxMoveTrackerCalibrate();

		[DllImport("MoveTrackerPlugin")]
		private static extern int PrxMoveTrackerUpdate(ulong cameraFrameHandle, int[] controllerHandles, IntPtr controllerInputs);

		[DllImport("MoveTrackerPlugin")]
		private static extern int PrxMoveTrackerQueueUpdate(ulong cameraFrameHandle, int[] controllerHandles, IntPtr controllerInputs);

		[DllImport("MoveTrackerPlugin")]
		private static extern int PrxMoveTrackerReadState(int handle, out SceMoveTrackerState data);

		[DllImport("MoveTrackerPlugin")]
		private static extern int PrxMoveTrackerReadState(int handle, IntPtr ptr);

		public static int Init()
		{
			return PrxMoveTrackerInitialise();
		}

		public static int Term()
		{
			return PrxMoveTrackerShutdown();
		}

		public static int Calibrate()
		{
			return PrxMoveTrackerCalibrate();
		}

		public static int QueueUpdate(ulong cameraFrameHandle, int[] controllerHandles, IntPtr controllerInputs)
		{
			return PrxMoveTrackerQueueUpdate(cameraFrameHandle, controllerHandles, controllerInputs);
		}

		public static int Update(ulong cameraFrameHandle, int[] controllerHandles, IntPtr controllerInputs)
		{
			return PrxMoveTrackerUpdate(cameraFrameHandle, controllerHandles, controllerInputs);
		}

		public static int ReadState(int controllerhandle, out SceMoveTrackerState data)
		{
			return PrxMoveTrackerReadState(controllerhandle, out data);
		}
	}
}
