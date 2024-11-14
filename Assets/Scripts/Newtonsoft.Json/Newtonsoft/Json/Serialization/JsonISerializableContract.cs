using System;
using Newtonsoft.Json.Shims;

namespace Newtonsoft.Json.Serialization
{
	[Preserve]
	public class JsonISerializableContract : JsonContainerContract
	{
		public ObjectConstructor<object> ISerializableCreator { get; set; }

		public JsonISerializableContract(Type underlyingType)
			: base(underlyingType)
		{
			ContractType = JsonContractType.Serializable;
		}
	}
}
