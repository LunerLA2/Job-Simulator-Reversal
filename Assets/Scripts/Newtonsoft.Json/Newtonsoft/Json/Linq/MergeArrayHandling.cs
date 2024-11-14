using Newtonsoft.Json.Shims;

namespace Newtonsoft.Json.Linq
{
	[Preserve]
	public enum MergeArrayHandling
	{
		Concat = 0,
		Union = 1,
		Replace = 2,
		Merge = 3
	}
}
