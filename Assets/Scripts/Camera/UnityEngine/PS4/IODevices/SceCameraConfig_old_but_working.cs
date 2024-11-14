using System;
using System.Runtime.InteropServices;

namespace UnityEngine.PS4.IODevices
{
	[StructLayout(LayoutKind.Sequential)]
	public class SceCameraConfig_old_but_working
	{
		public uint sizeThis;

		public SceCameraConfigType configType;

		public SceCameraFormat configExtention0_format;

		public SceCameraResolution configExtention0_resolution;

		public SceCameraFramerate configExtention0_framerate;

		public uint configExtention0_width;

		public uint configExtention0_height;

		public uint configExtention0_reserved1;

		public IntPtr configExtention0_pBaseOption;

		public SceCameraFormat configExtention1_format;

		public SceCameraResolution configExtention1_resolution;

		public SceCameraFramerate configExtention1_framerate;

		public uint configExtention1_width;

		public uint configExtention1_height;

		public uint configExtention1_reserved1;

		public IntPtr configExtention1_pBaseOption;
	}
}
