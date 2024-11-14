using System.ComponentModel;

namespace Oculus.Platform
{
	public enum RoomJoinPolicy : uint
	{
		[Description("NONE")]
		None = 0u,
		[Description("EVERYONE")]
		Everyone = 1u,
		[Description("FRIENDS_OF_MEMBERS")]
		FriendsOfMembers = 2u,
		[Description("FRIENDS_OF_OWNER")]
		FriendsOfOwner = 3u,
		[Description("INVITED_USERS")]
		InvitedUsers = 4u,
		[Description("UNKNOWN")]
		Unknown = 5u
	}
}
