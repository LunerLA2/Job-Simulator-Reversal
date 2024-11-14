using System;
using Newtonsoft.Json.Shims;

namespace Newtonsoft.Json
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
	[Preserve]
	public sealed class JsonIgnoreAttribute : Attribute
	{
	}
}
