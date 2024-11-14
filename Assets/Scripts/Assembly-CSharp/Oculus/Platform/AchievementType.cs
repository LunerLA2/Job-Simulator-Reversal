using System.ComponentModel;

namespace Oculus.Platform
{
	public enum AchievementType : uint
	{
		[Description("UNKNOWN")]
		Unknown = 0u,
		[Description("SIMPLE")]
		Simple = 1u,
		[Description("BITFIELD")]
		Bitfield = 2u,
		[Description("COUNT")]
		Count = 3u
	}
}
