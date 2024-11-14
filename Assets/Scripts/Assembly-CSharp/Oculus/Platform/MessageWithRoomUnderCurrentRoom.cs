using System;
using Oculus.Platform.Models;

namespace Oculus.Platform
{
	public class MessageWithRoomUnderCurrentRoom : Message<Room>
	{
		public MessageWithRoomUnderCurrentRoom(IntPtr c_message)
			: base(c_message)
		{
		}

		public override Room GetRoom()
		{
			return base.Data;
		}

		protected override Room GetDataFromMessage(IntPtr c_message)
		{
			IntPtr obj = CAPI.ovr_Message_GetNativeMessage(c_message);
			IntPtr o = CAPI.ovr_Message_GetRoom(obj);
			return new Room(o);
		}
	}
}
