using System;
using Oculus.Platform.Models;

namespace Oculus.Platform
{
	public class MessageWithRoomList : Message<RoomList>
	{
		public MessageWithRoomList(IntPtr c_message)
			: base(c_message)
		{
		}

		public override RoomList GetRoomList()
		{
			return base.Data;
		}

		protected override RoomList GetDataFromMessage(IntPtr c_message)
		{
			IntPtr obj = CAPI.ovr_Message_GetNativeMessage(c_message);
			IntPtr a = CAPI.ovr_Message_GetRoomArray(obj);
			return new RoomList(a);
		}
	}
}
