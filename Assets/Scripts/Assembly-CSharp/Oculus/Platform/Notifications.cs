using Oculus.Platform.Models;
using UnityEngine;

namespace Oculus.Platform
{
	public static class Notifications
	{
		public static Request MarkAsRead(ulong notificationID)
		{
			if (Core.IsInitialized())
			{
				return new Request(CAPI.ovr_Notification_MarkAsRead(notificationID));
			}
			return null;
		}

		public static Request GetRoomInviteNotifications()
		{
			if (Core.IsInitialized())
			{
				return new Request(CAPI.ovr_Notification_GetRoomInvites());
			}
			return null;
		}

		public static Request<RoomInviteNotificationList> GetNextRoomInviteNotificationListPage(RoomInviteNotificationList list)
		{
			if (!list.HasNextPage)
			{
				Debug.LogWarning("Oculus.Platform.GetNextRoomInviteNotificationListPage: List has no next page");
				return null;
			}
			if (Core.IsInitialized())
			{
				return new Request<RoomInviteNotificationList>(CAPI.ovr_HTTP_GetWithMessageType(list.NextUrl, 102890359));
			}
			return null;
		}
	}
}
