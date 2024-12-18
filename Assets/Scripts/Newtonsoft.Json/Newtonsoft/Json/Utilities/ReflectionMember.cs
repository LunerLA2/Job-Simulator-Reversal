using System;
using Newtonsoft.Json.Shims;

namespace Newtonsoft.Json.Utilities
{
	[Preserve]
	internal class ReflectionMember
	{
		public Type MemberType { get; set; }

		public Func<object, object> Getter { get; set; }

		public Action<object, object> Setter { get; set; }
	}
}
