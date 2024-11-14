using System;

namespace UnityEngine.PS4.IODevices
{
	[Flags]
	public enum sceCameraFrameFormat : uint
	{
		SCE_CAMERA_FRAME_FORMAT_LEVEL0 = 1u,
		SCE_CAMERA_FRAME_FORMAT_LEVEL1 = 2u,
		SCE_CAMERA_FRAME_FORMAT_LEVEL2 = 4u,
		SCE_CAMERA_FRAME_FORMAT_LEVEL3 = 8u,
		SCE_CAMERA_FRAME_FORMAT_ALL = 0xFu
	}
}
