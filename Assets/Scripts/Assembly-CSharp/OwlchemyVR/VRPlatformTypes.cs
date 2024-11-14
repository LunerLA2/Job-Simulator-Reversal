using System;

namespace OwlchemyVR
{
	[Flags]
	public enum VRPlatformTypes
	{
		None = 0,
		SteamVR = 1,
		Oculus = 2,
		PSVR = 3,
		Daydream = 4
	}
}
