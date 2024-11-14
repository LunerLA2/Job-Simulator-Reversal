using System;
using Newtonsoft.Json.Shims;

namespace Newtonsoft.Json.Serialization
{
	[Preserve]
	public interface IContractResolver
	{
		JsonContract ResolveContract(Type type);
	}
}
