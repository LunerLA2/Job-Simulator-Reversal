using System;
using Newtonsoft.Json.Shims;

namespace Newtonsoft.Json
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = false)]
	[Preserve]
	public sealed class JsonArrayAttribute : JsonContainerAttribute
	{
		private bool _allowNullItems;

		public bool AllowNullItems
		{
			get
			{
				return _allowNullItems;
			}
			set
			{
				_allowNullItems = value;
			}
		}

		public JsonArrayAttribute()
		{
		}

		public JsonArrayAttribute(bool allowNullItems)
		{
			_allowNullItems = allowNullItems;
		}

		public JsonArrayAttribute(string id)
			: base(id)
		{
		}
	}
}
