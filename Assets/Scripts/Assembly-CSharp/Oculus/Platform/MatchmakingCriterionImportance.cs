using System.ComponentModel;

namespace Oculus.Platform
{
	public enum MatchmakingCriterionImportance : uint
	{
		[Description("REQUIRED")]
		Required = 0u,
		[Description("HIGH")]
		High = 1u,
		[Description("MEDIUM")]
		Medium = 2u,
		[Description("LOW")]
		Low = 3u,
		[Description("UNKNOWN")]
		Unknown = 4u
	}
}
