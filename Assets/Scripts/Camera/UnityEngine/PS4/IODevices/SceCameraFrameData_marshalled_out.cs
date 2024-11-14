using System;
using System.Runtime.InteropServices;

namespace UnityEngine.PS4.IODevices
{
	[StructLayout(LayoutKind.Sequential)]
	public class SceCameraFrameData_marshalled_out
	{
		public uint sizeThis;

		public uint readMode;

		public IntPtr framePosition;
	}
}
