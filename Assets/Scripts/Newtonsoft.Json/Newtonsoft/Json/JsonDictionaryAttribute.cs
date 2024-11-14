using System;
using Newtonsoft.Json.Shims;

namespace Newtonsoft.Json
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = false)]
	[Preserve]
	public sealed class JsonDictionaryAttribute : JsonContainerAttribute
	{
		public JsonDictionaryAttribute()
		{
		}

		public JsonDictionaryAttribute(string id)
			: base(id)
		{
		}
	}
}
