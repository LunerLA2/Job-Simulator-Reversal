using System.ComponentModel;

namespace Oculus.Platform
{
	public enum LeaderboardFilterType : uint
	{
		[Description("NONE")]
		None = 0u,
		[Description("FRIENDS")]
		Friends = 1u,
		[Description("UNKNOWN")]
		Unknown = 2u
	}
}
