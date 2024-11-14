using Newtonsoft.Json.Shims;

namespace Newtonsoft.Json.Utilities
{
	[Preserve]
	internal enum ParseResult
	{
		None = 0,
		Success = 1,
		Overflow = 2,
		Invalid = 3
	}
}
