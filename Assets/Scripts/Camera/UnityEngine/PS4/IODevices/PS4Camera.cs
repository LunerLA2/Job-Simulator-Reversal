using System;
using System.Runtime.InteropServices;

namespace UnityEngine.PS4.IODevices
{
	public class PS4Camera
	{
		[DllImport("CameraPlugin")]
		private static extern int PrxCameraIsAttached(int index);

		public static int IsAttached(int index)
		{
			return PrxCameraIsAttached(index);
		}

		[DllImport("CameraPlugin")]
		private static extern void PrxCameraInit();

		public static void Init()
		{
			PrxCameraInit();
		}

		[DllImport("CameraPlugin")]
		private static extern void PrxCameraSetExposureGainMode(int camera0mode, int camera1mode);

		public static void SetExposureGainMode(SceCameraAttributeExposureGainMode camera0mode, SceCameraAttributeExposureGainMode camera1mode)
		{
			PrxCameraSetExposureGainMode((int)camera0mode, (int)camera1mode);
		}

		[DllImport("CameraPlugin")]
		private static extern void PrxCameraShutdown();

		public static void Term()
		{
			PrxCameraShutdown();
		}

		[DllImport("CameraPlugin")]
		private static extern int PrxCameraOpen(int userId, int type, int index, IntPtr pParam);

		public static int Open(int userId, int type, int index, IntPtr pParam)
		{
			return PrxCameraOpen(userId, type, index, pParam);
		}

		[DllImport("CameraPlugin")]
		private static extern int PrxCameraSetConfig(int handle, SceCameraConfig Config);

		[DllImport("CameraPlugin")]
		private static extern int PrxSetCameraFrameCallback(ulong newcallback);

		[DllImport("CameraPlugin")]
		private static extern int PrxCameraStart(int handle, SceCameraStartParameter pStart);

		[DllImport("CameraPlugin")]
		private static extern int PrxCameraGetFrameData(int handle, uint readmode, ref ulong framehandle);

		[DllImport("CameraPlugin")]
		private static extern IntPtr PrxCameraGetFrameIntPtr(ulong framehandle, int device, int level);

		[DllImport("CameraPlugin")]
		private static extern int PrxCameraIsValidFrameData(int handle, IntPtr pFrameData);

		[DllImport("CameraPlugin")]
		private static extern int PrxCameraStop(int handle);

		[DllImport("CameraPlugin")]
		private static extern int PrxCameraClose(int handle);

		public static int SetConfig(int handle, SceCameraConfig config)
		{
			return PrxCameraSetConfig(handle, config);
		}

		public static int Start(int handle, SceCameraStartParameter start)
		{
			return PrxCameraStart(handle, start);
		}

		public static int Stop(int handle)
		{
			return PrxCameraStop(handle);
		}

		public static int GetFrameData(int handle, uint readmode, ref ulong framehandle)
		{
			return PrxCameraGetFrameData(handle, readmode, ref framehandle);
		}

		public static IntPtr getFrameIntPtr(ulong framehandle, int device, int level)
		{
			return PrxCameraGetFrameIntPtr(framehandle, device, level);
		}

		public static int SetCameraFrameCallback(ulong callback)
		{
			return PrxSetCameraFrameCallback(callback);
		}
	}
}
