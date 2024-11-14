using Newtonsoft.Json.Shims;

namespace Newtonsoft.Json
{
	[Preserve]
	public enum StringEscapeHandling
	{
		Default = 0,
		EscapeNonAscii = 1,
		EscapeHtml = 2
	}
}
