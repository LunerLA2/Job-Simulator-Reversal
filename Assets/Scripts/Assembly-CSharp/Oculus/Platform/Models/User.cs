using System;

namespace Oculus.Platform.Models
{
	public class User
	{
		public readonly ulong ID;

		public readonly string ImageURL;

		public readonly string InviteToken;

		public readonly string OculusID;

		public readonly string Presence;

		public readonly UserPresenceStatus PresenceStatus;

		public readonly string SmallImageUrl;

		public User(IntPtr o)
		{
			ID = CAPI.ovr_User_GetID(o);
			ImageURL = CAPI.ovr_User_GetImageUrl(o);
			InviteToken = CAPI.ovr_User_GetInviteToken(o);
			OculusID = CAPI.ovr_User_GetOculusID(o);
			Presence = CAPI.ovr_User_GetPresence(o);
			PresenceStatus = CAPI.ovr_User_GetPresenceStatus(o);
			SmallImageUrl = CAPI.ovr_User_GetSmallImageUrl(o);
		}
	}
}
