using Newtonsoft.Json.Shims;

namespace Newtonsoft.Json.Serialization
{
	[Preserve]
	internal enum JsonContractType
	{
		None = 0,
		Object = 1,
		Array = 2,
		Primitive = 3,
		String = 4,
		Dictionary = 5,
		Dynamic = 6,
		Serializable = 7,
		Linq = 8
	}
}
