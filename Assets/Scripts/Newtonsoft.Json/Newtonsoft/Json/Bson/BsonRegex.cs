using Newtonsoft.Json.Shims;

namespace Newtonsoft.Json.Bson
{
	[Preserve]
	internal class BsonRegex : BsonToken
	{
		public BsonString Pattern { get; set; }

		public BsonString Options { get; set; }

		public override BsonType Type
		{
			get
			{
				return BsonType.Regex;
			}
		}

		public BsonRegex(string pattern, string options)
		{
			Pattern = new BsonString(pattern, false);
			Options = new BsonString(options, false);
		}
	}
}
