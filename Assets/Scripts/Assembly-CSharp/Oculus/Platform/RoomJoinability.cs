using System.ComponentModel;

namespace Oculus.Platform
{
	public enum RoomJoinability : uint
	{
		[Description("UNKNOWN")]
		Unknown = 0u,
		[Description("ARE_IN")]
		AreIn = 1u,
		[Description("ARE_KICKED")]
		AreKicked = 2u,
		[Description("CAN_JOIN")]
		CanJoin = 3u,
		[Description("IS_FULL")]
		IsFull = 4u,
		[Description("NO_VIEWER")]
		NoViewer = 5u,
		[Description("POLICY_PREVENTS")]
		PolicyPrevents = 6u
	}
}
