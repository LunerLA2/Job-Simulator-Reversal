using System;
using Oculus.Platform.Models;

namespace Oculus.Platform
{
	public class MessageWithPurchase : Message<Purchase>
	{
		public MessageWithPurchase(IntPtr c_message)
			: base(c_message)
		{
		}

		public override Purchase GetPurchase()
		{
			return base.Data;
		}

		protected override Purchase GetDataFromMessage(IntPtr c_message)
		{
			IntPtr obj = CAPI.ovr_Message_GetNativeMessage(c_message);
			IntPtr o = CAPI.ovr_Message_GetPurchase(obj);
			return new Purchase(o);
		}
	}
}
