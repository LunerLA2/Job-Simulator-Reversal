using System.ComponentModel;

namespace Oculus.Platform
{
	public enum RoomMembershipLockStatus : uint
	{
		[Description("UNKNOWN")]
		Unknown = 0u,
		[Description("LOCK")]
		Lock = 1u,
		[Description("UNLOCK")]
		Unlock = 2u
	}
}
