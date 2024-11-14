using System;
using Oculus.Platform.Models;

namespace Oculus.Platform
{
	public class MessageWithUser : Message<User>
	{
		public MessageWithUser(IntPtr c_message)
			: base(c_message)
		{
		}

		public override User GetUser()
		{
			return base.Data;
		}

		protected override User GetDataFromMessage(IntPtr c_message)
		{
			IntPtr obj = CAPI.ovr_Message_GetNativeMessage(c_message);
			IntPtr o = CAPI.ovr_Message_GetUser(obj);
			return new User(o);
		}
	}
}
