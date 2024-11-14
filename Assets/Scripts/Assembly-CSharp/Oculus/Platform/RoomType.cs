using System.ComponentModel;

namespace Oculus.Platform
{
	public enum RoomType : uint
	{
		[Description("UNKNOWN")]
		Unknown = 0u,
		[Description("MATCHMAKING")]
		Matchmaking = 1u,
		[Description("MODERATED")]
		Moderated = 2u,
		[Description("PRIVATE")]
		Private = 3u,
		[Description("SOLO")]
		Solo = 4u
	}
}
