using System;
using Newtonsoft.Json.Shims;

namespace Newtonsoft.Json.Serialization
{
	[Preserve]
	public class JsonStringContract : JsonPrimitiveContract
	{
		public JsonStringContract(Type underlyingType)
			: base(underlyingType)
		{
			ContractType = JsonContractType.String;
		}
	}
}
