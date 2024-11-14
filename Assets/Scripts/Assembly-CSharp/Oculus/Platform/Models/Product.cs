using System;

namespace Oculus.Platform.Models
{
	public class Product
	{
		public readonly string Sku;

		public readonly string Description;

		public readonly string FormattedPrice;

		public readonly string Name;

		public Product(IntPtr o)
		{
			Sku = CAPI.ovr_Product_GetSKU(o);
			Description = CAPI.ovr_Product_GetDescription(o);
			FormattedPrice = CAPI.ovr_Product_GetFormattedPrice(o);
			Name = CAPI.ovr_Product_GetName(o);
		}
	}
}
