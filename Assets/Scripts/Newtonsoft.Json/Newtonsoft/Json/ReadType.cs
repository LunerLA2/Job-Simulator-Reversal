using Newtonsoft.Json.Shims;

namespace Newtonsoft.Json
{
	[Preserve]
	internal enum ReadType
	{
		Read = 0,
		ReadAsInt32 = 1,
		ReadAsBytes = 2,
		ReadAsString = 3,
		ReadAsDecimal = 4,
		ReadAsDateTime = 5,
		ReadAsDateTimeOffset = 6,
		ReadAsDouble = 7,
		ReadAsBoolean = 8
	}
}
