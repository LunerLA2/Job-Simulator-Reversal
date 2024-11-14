using System.ComponentModel;

namespace Oculus.Platform
{
	public enum SystemVoipStatus : uint
	{
		[Description("UNKNOWN")]
		Unknown = 0u,
		[Description("UNAVAILABLE")]
		Unavailable = 1u,
		[Description("SUPPRESSED")]
		Suppressed = 2u,
		[Description("ACTIVE")]
		Active = 3u
	}
}
