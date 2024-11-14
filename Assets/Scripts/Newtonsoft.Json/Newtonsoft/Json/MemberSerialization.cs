using Newtonsoft.Json.Shims;

namespace Newtonsoft.Json
{
	[Preserve]
	public enum MemberSerialization
	{
		OptOut = 0,
		OptIn = 1,
		Fields = 2
	}
}
