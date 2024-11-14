using Newtonsoft.Json.Shims;

namespace Newtonsoft.Json
{
	[Preserve]
	public enum DateParseHandling
	{
		None = 0,
		DateTime = 1,
		DateTimeOffset = 2
	}
}
