using System;
using Newtonsoft.Json.Shims;

namespace Newtonsoft.Json.Utilities
{
	[Preserve]
	internal class TypeInformation
	{
		public Type Type { get; set; }

		public PrimitiveTypeCode TypeCode { get; set; }
	}
}
