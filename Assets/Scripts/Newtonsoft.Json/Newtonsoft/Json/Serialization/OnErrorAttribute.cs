using System;
using Newtonsoft.Json.Shims;

namespace Newtonsoft.Json.Serialization
{
	[AttributeUsage(AttributeTargets.Method, Inherited = false)]
	[Preserve]
	public sealed class OnErrorAttribute : Attribute
	{
	}
}
