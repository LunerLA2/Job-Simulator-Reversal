using System.Runtime.InteropServices;

namespace UnityEngine.PS4.IODevices
{
	[StructLayout(LayoutKind.Sequential)]
	public class SceCameraConfig
	{
		public uint sizeThis;

		public SceCameraConfigType configType;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
		public SceCameraConfigExtention[] configExtention;

		public SceCameraConfig()
		{
			sizeThis = 104u;
			configExtention = new SceCameraConfigExtention[2];
		}
	}
}
