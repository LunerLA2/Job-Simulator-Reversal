using System.ComponentModel;

namespace Oculus.Platform
{
	public enum UserPresenceStatus : uint
	{
		[Description("UNKNOWN")]
		Unknown = 0u,
		[Description("ONLINE")]
		Online = 1u,
		[Description("OFFLINE")]
		Offline = 2u
	}
}
