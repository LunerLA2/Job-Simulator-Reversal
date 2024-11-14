using System.ComponentModel;

namespace Oculus.Platform
{
	public enum SendPolicy : uint
	{
		[Description("UNRELIABLE")]
		Unreliable = 0u,
		[Description("RELIABLE")]
		Reliable = 1u,
		[Description("UNKNOWN")]
		Unknown = 2u
	}
}
