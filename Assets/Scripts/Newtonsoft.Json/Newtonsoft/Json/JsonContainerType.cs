using Newtonsoft.Json.Shims;

namespace Newtonsoft.Json
{
	[Preserve]
	internal enum JsonContainerType
	{
		None = 0,
		Object = 1,
		Array = 2,
		Constructor = 3
	}
}
