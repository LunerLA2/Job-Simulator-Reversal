using System.Collections;
using Newtonsoft.Json.Shims;

namespace Newtonsoft.Json.Utilities
{
	[Preserve]
	internal interface IWrappedDictionary : IDictionary, ICollection, IEnumerable
	{
		object UnderlyingDictionary { get; }
	}
}
