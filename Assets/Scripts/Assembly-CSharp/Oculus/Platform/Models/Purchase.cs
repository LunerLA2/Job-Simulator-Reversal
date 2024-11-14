using System;

namespace Oculus.Platform.Models
{
	public class Purchase
	{
		public readonly string Sku;

		public readonly DateTime GrantTime;

		public readonly ulong ID;

		public Purchase(IntPtr o)
		{
			Sku = CAPI.ovr_Purchase_GetSKU(o);
			GrantTime = CAPI.ovr_Purchase_GetGrantTime(o);
			ID = CAPI.ovr_Purchase_GetPurchaseID(o);
		}
	}
}
