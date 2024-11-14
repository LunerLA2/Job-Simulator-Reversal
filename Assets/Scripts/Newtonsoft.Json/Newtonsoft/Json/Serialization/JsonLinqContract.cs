using System;
using Newtonsoft.Json.Shims;

namespace Newtonsoft.Json.Serialization
{
	[Preserve]
	public class JsonLinqContract : JsonContract
	{
		public JsonLinqContract(Type underlyingType)
			: base(underlyingType)
		{
			ContractType = JsonContractType.Linq;
		}
	}
}
