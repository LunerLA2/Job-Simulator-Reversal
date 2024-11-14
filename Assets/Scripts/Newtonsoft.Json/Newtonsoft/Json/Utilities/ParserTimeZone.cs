using Newtonsoft.Json.Shims;

namespace Newtonsoft.Json.Utilities
{
	[Preserve]
	internal enum ParserTimeZone
	{
		Unspecified = 0,
		Utc = 1,
		LocalWestOfUtc = 2,
		LocalEastOfUtc = 3
	}
}
