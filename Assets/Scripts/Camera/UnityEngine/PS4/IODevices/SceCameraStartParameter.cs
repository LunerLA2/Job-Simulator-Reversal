using System;
using System.Runtime.InteropServices;

namespace UnityEngine.PS4.IODevices
{
	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class SceCameraStartParameter
	{
		public uint sizeThis;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
		public sceCameraFrameFormat[] formatLevel;

		public IntPtr pStartOption;

		public SceCameraStartParameter()
		{
			sizeThis = 24u;
			formatLevel = new sceCameraFrameFormat[2];
		}
	}
}
