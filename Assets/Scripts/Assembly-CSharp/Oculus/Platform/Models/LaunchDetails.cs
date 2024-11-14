using System;

namespace Oculus.Platform.Models
{
	public class LaunchDetails
	{
		public readonly LaunchType LaunchType;

		public readonly ulong RoomID;

		public readonly UserList Users;

		public LaunchDetails(IntPtr o)
		{
			LaunchType = CAPI.ovr_LaunchDetails_GetLaunchType(o);
			RoomID = CAPI.ovr_LaunchDetails_GetRoomID(o);
			Users = new UserList(CAPI.ovr_LaunchDetails_GetUsers(o));
		}
	}
}
