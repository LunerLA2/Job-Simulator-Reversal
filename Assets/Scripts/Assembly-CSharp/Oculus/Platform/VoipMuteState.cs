using System.ComponentModel;

namespace Oculus.Platform
{
	public enum VoipMuteState : uint
	{
		[Description("UNKNOWN")]
		Unknown = 0u,
		[Description("MUTED")]
		Muted = 1u,
		[Description("UNMUTED")]
		Unmuted = 2u
	}
}
