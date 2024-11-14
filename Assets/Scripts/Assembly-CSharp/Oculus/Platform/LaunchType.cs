using System.ComponentModel;

namespace Oculus.Platform
{
	public enum LaunchType : uint
	{
		[Description("UNKNOWN")]
		Unknown = 0u,
		[Description("NORMAL")]
		Normal = 1u,
		[Description("INVITE")]
		Invite = 2u,
		[Description("COORDINATED")]
		Coordinated = 3u
	}
}
