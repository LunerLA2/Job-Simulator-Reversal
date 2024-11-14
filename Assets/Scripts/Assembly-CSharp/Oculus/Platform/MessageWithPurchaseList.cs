using System;
using Oculus.Platform.Models;

namespace Oculus.Platform
{
	public class MessageWithPurchaseList : Message<PurchaseList>
	{
		public MessageWithPurchaseList(IntPtr c_message)
			: base(c_message)
		{
		}

		public override PurchaseList GetPurchaseList()
		{
			return base.Data;
		}

		protected override PurchaseList GetDataFromMessage(IntPtr c_message)
		{
			IntPtr obj = CAPI.ovr_Message_GetNativeMessage(c_message);
			IntPtr a = CAPI.ovr_Message_GetPurchaseArray(obj);
			return new PurchaseList(a);
		}
	}
}
