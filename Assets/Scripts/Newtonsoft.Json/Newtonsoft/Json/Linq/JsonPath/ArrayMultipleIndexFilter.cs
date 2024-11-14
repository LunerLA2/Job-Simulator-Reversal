using System.Collections.Generic;
using Newtonsoft.Json.Shims;

namespace Newtonsoft.Json.Linq.JsonPath
{
	[Preserve]
	internal class ArrayMultipleIndexFilter : PathFilter
	{
		public List<int> Indexes { get; set; }

		public override IEnumerable<JToken> ExecuteFilter(IEnumerable<JToken> current, bool errorWhenNoMatch)
		{
			foreach (JToken t in current)
			{
				foreach (int index in Indexes)
				{
					JToken tokenIndex = PathFilter.GetTokenIndex(t, errorWhenNoMatch, index);
					if (tokenIndex != null)
					{
						yield return tokenIndex;
					}
				}
			}
		}
	}
}
