using System;
using System.Runtime.InteropServices;

namespace UnityEngine.PS4.IODevices
{
	[StructLayout(LayoutKind.Sequential)]
	public class SceCameraFrameData_marshalled
	{
		public uint sizeThis;

		public uint readMode;

		public IntPtr _framePosition;

		public SceCameraFrameData_marshalled()
		{
			Console.WriteLine("SceCameraFrameData_marshalled constr\n");
			sizeThis = 488u;
		}
	}
}
