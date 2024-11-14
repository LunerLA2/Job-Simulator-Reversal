using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Newtonsoft.Json.Shims;
using Newtonsoft.Json.Utilities;

namespace Newtonsoft.Json.Linq.JsonPath
{
	[Preserve]
	internal class FieldMultipleFilter : PathFilter
	{
		public List<string> Names { get; set; }

		public override IEnumerable<JToken> ExecuteFilter(IEnumerable<JToken> current, bool errorWhenNoMatch)
		{
			foreach (JToken t in current)
			{
				JObject o = t as JObject;
				if (o != null)
				{
					foreach (string name in Names)
					{
						JToken jToken = o[name];
						if (jToken != null)
						{
							yield return jToken;
						}
						if (errorWhenNoMatch)
						{
							throw new JsonException("Property '{0}' does not exist on JObject.".FormatWith(CultureInfo.InvariantCulture, name));
						}
					}
				}
				else if (errorWhenNoMatch)
				{
					throw new JsonException("Properties {0} not valid on {1}.".FormatWith(CultureInfo.InvariantCulture, string.Join(", ", Names.Select((string n) => "'" + n + "'").ToArray()), t.GetType().Name));
				}
			}
		}
	}
}
