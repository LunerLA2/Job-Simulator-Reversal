using System;
using Newtonsoft.Json.Shims;

namespace Newtonsoft.Json.Converters
{
	[Preserve]
	public abstract class DateTimeConverterBase : JsonConverter
	{
		public override bool CanConvert(Type objectType)
		{
			if (objectType == typeof(DateTime) || objectType == typeof(DateTime?))
			{
				return true;
			}
			if (objectType == typeof(DateTimeOffset) || objectType == typeof(DateTimeOffset?))
			{
				return true;
			}
			return false;
		}
	}
}
