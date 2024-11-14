using System;
using System.Runtime.InteropServices;

namespace Valve.VR
{
	public struct IVRTrackedCamera
	{
		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate bool _HasCamera(uint nDeviceIndex);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate bool _GetCameraFirmwareDescription(uint nDeviceIndex, string pBuffer, uint nBufferLen);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate bool _GetCameraFrameDimensions(uint nDeviceIndex, ECameraVideoStreamFormat nVideoStreamFormat, ref uint pWidth, ref uint pHeight);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate bool _SetCameraVideoStreamFormat(uint nDeviceIndex, ECameraVideoStreamFormat nVideoStreamFormat);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate ECameraVideoStreamFormat _GetCameraVideoStreamFormat(uint nDeviceIndex);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate bool _EnableCameraForStreaming(uint nDeviceIndex, bool bEnable);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate bool _StartVideoStream(uint nDeviceIndex);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate bool _StopVideoStream(uint nDeviceIndex);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate bool _IsVideoStreamActive(uint nDeviceIndex, ref bool pbPaused, ref float pflElapsedTime);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate IntPtr _GetVideoStreamFrame(uint nDeviceIndex);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate bool _ReleaseVideoStreamFrame(uint nDeviceIndex, ref CameraVideoStreamFrame_t pFrameImage);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate bool _SetAutoExposure(uint nDeviceIndex, bool bEnable);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate bool _PauseVideoStream(uint nDeviceIndex);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate bool _ResumeVideoStream(uint nDeviceIndex);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate bool _GetCameraDistortion(uint nDeviceIndex, float flInputU, float flInputV, ref float pflOutputU, ref float pflOutputV);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate bool _GetCameraProjection(uint nDeviceIndex, float flWidthPixels, float flHeightPixels, float flZNear, float flZFar, ref HmdMatrix44_t pProjection);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate bool _GetRecommendedCameraUndistortion(uint nDeviceIndex, ref uint pUndistortionWidthPixels, ref uint pUndistortionHeightPixels);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate bool _SetCameraUndistortion(uint nDeviceIndex, uint nUndistortionWidthPixels, uint nUndistortionHeightPixels);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void _RequestVideoServicesForTool();

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void _ReleaseVideoServicesForTool();

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate bool _GetVideoStreamFrameSharedTextureGL(bool bUndistorted, ref uint pglTextureId, IntPtr pglSharedTextureHandle);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate bool _ReleaseVideoStreamFrameSharedTextureGL(uint glTextureId, IntPtr glSharedTextureHandle);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void _LockSharedTextureGL(IntPtr glSharedTextureHandle, ref CameraVideoStreamFrame_t pFrameImage);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void _UnlockSharedTextureGL(IntPtr glSharedTextureHandle);

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal _HasCamera HasCamera;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal _GetCameraFirmwareDescription GetCameraFirmwareDescription;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal _GetCameraFrameDimensions GetCameraFrameDimensions;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal _SetCameraVideoStreamFormat SetCameraVideoStreamFormat;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal _GetCameraVideoStreamFormat GetCameraVideoStreamFormat;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal _EnableCameraForStreaming EnableCameraForStreaming;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal _StartVideoStream StartVideoStream;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal _StopVideoStream StopVideoStream;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal _IsVideoStreamActive IsVideoStreamActive;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal _GetVideoStreamFrame GetVideoStreamFrame;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal _ReleaseVideoStreamFrame ReleaseVideoStreamFrame;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal _SetAutoExposure SetAutoExposure;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal _PauseVideoStream PauseVideoStream;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal _ResumeVideoStream ResumeVideoStream;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal _GetCameraDistortion GetCameraDistortion;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal _GetCameraProjection GetCameraProjection;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal _GetRecommendedCameraUndistortion GetRecommendedCameraUndistortion;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal _SetCameraUndistortion SetCameraUndistortion;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal _RequestVideoServicesForTool RequestVideoServicesForTool;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal _ReleaseVideoServicesForTool ReleaseVideoServicesForTool;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal _GetVideoStreamFrameSharedTextureGL GetVideoStreamFrameSharedTextureGL;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal _ReleaseVideoStreamFrameSharedTextureGL ReleaseVideoStreamFrameSharedTextureGL;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal _LockSharedTextureGL LockSharedTextureGL;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal _UnlockSharedTextureGL UnlockSharedTextureGL;
	}
}
