using Newtonsoft.Json.Shims;

namespace Newtonsoft.Json
{
	[Preserve]
	public enum Required
	{
		Default = 0,
		AllowNull = 1,
		Always = 2,
		DisallowNull = 3
	}
}
