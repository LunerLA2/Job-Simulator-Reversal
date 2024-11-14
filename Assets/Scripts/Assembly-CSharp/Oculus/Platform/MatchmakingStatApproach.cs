using System.ComponentModel;

namespace Oculus.Platform
{
	public enum MatchmakingStatApproach : uint
	{
		[Description("UNKNOWN")]
		Unknown = 0u,
		[Description("TRAILING")]
		Trailing = 1u,
		[Description("SWINGY")]
		Swingy = 2u
	}
}
