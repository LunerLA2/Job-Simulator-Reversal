using System.Collections.Generic;
using Newtonsoft.Json.Shims;

namespace Newtonsoft.Json.Serialization
{
	[Preserve]
	public delegate IEnumerable<KeyValuePair<object, object>> ExtensionDataGetter(object o);
}
