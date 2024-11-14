using System;
using System.Collections.Generic;

namespace Oculus.Platform.Models
{
	public class RoomInviteNotificationList : DeserializableList<RoomInviteNotification>
	{
		public RoomInviteNotificationList(IntPtr a)
		{
			int num = (int)(uint)CAPI.ovr_RoomInviteNotificationArray_GetSize(a);
			_Data = new List<RoomInviteNotification>(num);
			for (int i = 0; i < num; i++)
			{
				_Data.Add(new RoomInviteNotification(CAPI.ovr_RoomInviteNotificationArray_GetElement(a, (UIntPtr)(ulong)i)));
			}
			_NextUrl = CAPI.ovr_RoomInviteNotificationArray_GetNextUrl(a);
		}
	}
}
