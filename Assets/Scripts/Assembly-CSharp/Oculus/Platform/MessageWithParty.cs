using System;
using Oculus.Platform.Models;

namespace Oculus.Platform
{
	public class MessageWithParty : Message<Party>
	{
		public MessageWithParty(IntPtr c_message)
			: base(c_message)
		{
		}

		public override Party GetParty()
		{
			return base.Data;
		}

		protected override Party GetDataFromMessage(IntPtr c_message)
		{
			IntPtr obj = CAPI.ovr_Message_GetNativeMessage(c_message);
			IntPtr o = CAPI.ovr_Message_GetParty(obj);
			return new Party(o);
		}
	}
}
