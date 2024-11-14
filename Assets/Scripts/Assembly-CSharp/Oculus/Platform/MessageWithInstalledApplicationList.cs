using System;
using Oculus.Platform.Models;

namespace Oculus.Platform
{
	public class MessageWithInstalledApplicationList : Message<InstalledApplicationList>
	{
		public MessageWithInstalledApplicationList(IntPtr c_message)
			: base(c_message)
		{
		}

		public override InstalledApplicationList GetInstalledApplicationList()
		{
			return base.Data;
		}

		protected override InstalledApplicationList GetDataFromMessage(IntPtr c_message)
		{
			IntPtr obj = CAPI.ovr_Message_GetNativeMessage(c_message);
			IntPtr a = CAPI.ovr_Message_GetInstalledApplicationArray(obj);
			return new InstalledApplicationList(a);
		}
	}
}
