using System;

namespace UnityEngine.PS4.IODevices
{
	public struct SceCameraConfigExtention
	{
		public SceCameraFormat format;

		public SceCameraResolution resolution;

		public SceCameraFramerate framerate;

		public uint width;

		public uint height;

		public uint reserved1;

		public IntPtr pBaseOption;
	}
}
