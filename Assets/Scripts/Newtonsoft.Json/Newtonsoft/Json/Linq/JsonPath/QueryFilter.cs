using System.Collections.Generic;
using Newtonsoft.Json.Shims;

namespace Newtonsoft.Json.Linq.JsonPath
{
	[Preserve]
	internal class QueryFilter : PathFilter
	{
		public QueryExpression Expression { get; set; }

		public override IEnumerable<JToken> ExecuteFilter(IEnumerable<JToken> current, bool errorWhenNoMatch)
		{
			foreach (JToken item in current)
			{
				foreach (JToken item2 in (IEnumerable<JToken>)item)
				{
					if (Expression.IsMatch(item2))
					{
						yield return item2;
					}
				}
			}
		}
	}
}
