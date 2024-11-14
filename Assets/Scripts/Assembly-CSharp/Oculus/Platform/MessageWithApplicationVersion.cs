using System;
using Oculus.Platform.Models;

namespace Oculus.Platform
{
	public class MessageWithApplicationVersion : Message<ApplicationVersion>
	{
		public MessageWithApplicationVersion(IntPtr c_message)
			: base(c_message)
		{
		}

		public override ApplicationVersion GetApplicationVersion()
		{
			return base.Data;
		}

		protected override ApplicationVersion GetDataFromMessage(IntPtr c_message)
		{
			IntPtr obj = CAPI.ovr_Message_GetNativeMessage(c_message);
			IntPtr o = CAPI.ovr_Message_GetApplicationVersion(obj);
			return new ApplicationVersion(o);
		}
	}
}
