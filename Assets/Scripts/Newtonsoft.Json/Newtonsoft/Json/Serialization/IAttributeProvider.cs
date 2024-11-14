using System;
using System.Collections.Generic;
using Newtonsoft.Json.Shims;

namespace Newtonsoft.Json.Serialization
{
	[Preserve]
	public interface IAttributeProvider
	{
		IList<Attribute> GetAttributes(bool inherit);

		IList<Attribute> GetAttributes(Type attributeType, bool inherit);
	}
}
