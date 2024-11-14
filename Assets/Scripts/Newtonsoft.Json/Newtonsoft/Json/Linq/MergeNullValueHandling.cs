using System;
using Newtonsoft.Json.Shims;

namespace Newtonsoft.Json.Linq
{
	[Flags]
	[Preserve]
	public enum MergeNullValueHandling
	{
		Ignore = 0,
		Merge = 1
	}
}
