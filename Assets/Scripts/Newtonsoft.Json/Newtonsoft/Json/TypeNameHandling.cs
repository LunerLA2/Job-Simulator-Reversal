using System;
using Newtonsoft.Json.Shims;

namespace Newtonsoft.Json
{
	[Flags]
	[Preserve]
	public enum TypeNameHandling
	{
		None = 0,
		Objects = 1,
		Arrays = 2,
		All = 3,
		Auto = 4
	}
}
