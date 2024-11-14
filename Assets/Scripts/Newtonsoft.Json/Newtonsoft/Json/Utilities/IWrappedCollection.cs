using System.Collections;
using Newtonsoft.Json.Shims;

namespace Newtonsoft.Json.Utilities
{
	[Preserve]
	internal interface IWrappedCollection : IList, ICollection, IEnumerable
	{
		object UnderlyingCollection { get; }
	}
}
