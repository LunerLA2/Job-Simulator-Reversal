using System;
using System.Runtime.InteropServices;

namespace UnityEngine.PS4.Engines
{
	[Serializable]
	public struct ScePadTrackerData
	{
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 8, ArraySubType = UnmanagedType.SysUInt)]
		public ScePadTrackerImageCoordinates[] imageCoordinates;
	}
}
