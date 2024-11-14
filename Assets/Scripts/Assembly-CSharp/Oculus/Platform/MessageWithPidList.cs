using System;
using Oculus.Platform.Models;

namespace Oculus.Platform
{
	public class MessageWithPidList : Message<PidList>
	{
		public MessageWithPidList(IntPtr c_message)
			: base(c_message)
		{
		}

		public override PidList GetPidList()
		{
			return base.Data;
		}

		protected override PidList GetDataFromMessage(IntPtr c_message)
		{
			IntPtr obj = CAPI.ovr_Message_GetNativeMessage(c_message);
			IntPtr a = CAPI.ovr_Message_GetPidArray(obj);
			return new PidList(a);
		}
	}
}
