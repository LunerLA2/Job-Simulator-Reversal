using System;
using Newtonsoft.Json.Shims;

namespace Newtonsoft.Json
{
	[AttributeUsage(AttributeTargets.Constructor, AllowMultiple = false)]
	[Preserve]
	public sealed class JsonConstructorAttribute : Attribute
	{
	}
}
