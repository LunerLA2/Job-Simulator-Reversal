using System;
using Newtonsoft.Json.Shims;

namespace Newtonsoft.Json
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
	[Preserve]
	public class JsonExtensionDataAttribute : Attribute
	{
		public bool WriteData { get; set; }

		public bool ReadData { get; set; }

		public JsonExtensionDataAttribute()
		{
			WriteData = true;
			ReadData = true;
		}
	}
}
