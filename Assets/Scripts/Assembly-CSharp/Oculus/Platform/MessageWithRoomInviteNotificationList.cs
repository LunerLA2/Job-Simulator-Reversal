using System;
using Oculus.Platform.Models;

namespace Oculus.Platform
{
	public class MessageWithRoomInviteNotificationList : Message<RoomInviteNotificationList>
	{
		public MessageWithRoomInviteNotificationList(IntPtr c_message)
			: base(c_message)
		{
		}

		public override RoomInviteNotificationList GetRoomInviteNotificationList()
		{
			return base.Data;
		}

		protected override RoomInviteNotificationList GetDataFromMessage(IntPtr c_message)
		{
			IntPtr obj = CAPI.ovr_Message_GetNativeMessage(c_message);
			IntPtr a = CAPI.ovr_Message_GetRoomInviteNotificationArray(obj);
			return new RoomInviteNotificationList(a);
		}
	}
}
