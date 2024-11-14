using System.Collections.Generic;
using Newtonsoft.Json.Shims;

namespace Newtonsoft.Json.Linq.JsonPath
{
	[Preserve]
	internal class ScanFilter : PathFilter
	{
		public string Name { get; set; }

		public override IEnumerable<JToken> ExecuteFilter(IEnumerable<JToken> current, bool errorWhenNoMatch)
		{
			foreach (JToken root in current)
			{
				if (Name == null)
				{
					yield return root;
				}
				JToken value = root;
				JToken jToken = root;
				while (true)
				{
					if (jToken != null && jToken.HasValues)
					{
						value = jToken.First;
					}
					else
					{
						while (value != null && value != root && value == value.Parent.Last)
						{
							value = value.Parent;
						}
						if (value == null || value == root)
						{
							break;
						}
						value = value.Next;
					}
					JProperty jProperty = value as JProperty;
					if (jProperty != null)
					{
						if (jProperty.Name == Name)
						{
							yield return jProperty.Value;
						}
					}
					else if (Name == null)
					{
						yield return value;
					}
					jToken = value as JContainer;
				}
			}
		}
	}
}
