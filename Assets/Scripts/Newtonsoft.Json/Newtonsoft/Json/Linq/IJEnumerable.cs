using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Shims;

namespace Newtonsoft.Json.Linq
{
	[Preserve]
	public interface IJEnumerable<T> : IEnumerable<T>, IEnumerable where T : JToken
	{
		IJEnumerable<JToken> this[object key] { get; }
	}
}
