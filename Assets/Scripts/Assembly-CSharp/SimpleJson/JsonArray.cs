using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;

namespace SimpleJson
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	[GeneratedCode("simple-json", "1.0.0")]
	public class JsonArray : List<object>
	{
		public JsonArray()
		{
		}

		public JsonArray(int capacity)
			: base(capacity)
		{
		}

		public override string ToString()
		{
			return SimpleJson.SerializeObject(this) ?? string.Empty;
		}
	}
}
