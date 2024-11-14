using System;
using Oculus.Platform.Models;

namespace Oculus.Platform
{
	public class MessageWithUserList : Message<UserList>
	{
		public MessageWithUserList(IntPtr c_message)
			: base(c_message)
		{
		}

		public override UserList GetUserList()
		{
			return base.Data;
		}

		protected override UserList GetDataFromMessage(IntPtr c_message)
		{
			IntPtr obj = CAPI.ovr_Message_GetNativeMessage(c_message);
			IntPtr a = CAPI.ovr_Message_GetUserArray(obj);
			return new UserList(a);
		}
	}
}
