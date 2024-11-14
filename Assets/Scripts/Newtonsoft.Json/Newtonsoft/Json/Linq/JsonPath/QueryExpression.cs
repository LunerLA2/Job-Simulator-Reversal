using Newtonsoft.Json.Shims;

namespace Newtonsoft.Json.Linq.JsonPath
{
	[Preserve]
	internal abstract class QueryExpression
	{
		public QueryOperator Operator { get; set; }

		public abstract bool IsMatch(JToken t);
	}
}
