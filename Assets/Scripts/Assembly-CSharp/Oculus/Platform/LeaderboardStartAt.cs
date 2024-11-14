using System.ComponentModel;

namespace Oculus.Platform
{
	public enum LeaderboardStartAt : uint
	{
		[Description("TOP")]
		Top = 0u,
		[Description("CENTERED_ON_VIEWER")]
		CenteredOnViewer = 1u,
		[Description("UNKNOWN")]
		Unknown = 2u
	}
}
