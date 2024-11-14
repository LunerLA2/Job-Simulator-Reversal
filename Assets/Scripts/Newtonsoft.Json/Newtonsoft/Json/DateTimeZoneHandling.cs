using Newtonsoft.Json.Shims;

namespace Newtonsoft.Json
{
	[Preserve]
	public enum DateTimeZoneHandling
	{
		Local = 0,
		Utc = 1,
		Unspecified = 2,
		RoundtripKind = 3
	}
}
