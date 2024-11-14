using Newtonsoft.Json.Shims;

namespace Newtonsoft.Json.Linq.JsonPath
{
	[Preserve]
	internal enum QueryOperator
	{
		None = 0,
		Equals = 1,
		NotEquals = 2,
		Exists = 3,
		LessThan = 4,
		LessThanOrEquals = 5,
		GreaterThan = 6,
		GreaterThanOrEquals = 7,
		And = 8,
		Or = 9
	}
}
