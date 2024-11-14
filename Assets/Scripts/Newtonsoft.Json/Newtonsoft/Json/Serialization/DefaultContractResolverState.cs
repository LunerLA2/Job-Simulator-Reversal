using System.Collections.Generic;
using Newtonsoft.Json.Shims;
using Newtonsoft.Json.Utilities;

namespace Newtonsoft.Json.Serialization
{
	[Preserve]
	internal class DefaultContractResolverState
	{
		public Dictionary<ResolverContractKey, JsonContract> ContractCache;

		public PropertyNameTable NameTable = new PropertyNameTable();
	}
}
