using System;
using System.Globalization;
using Newtonsoft.Json.Bson;
using Newtonsoft.Json.Shims;
using Newtonsoft.Json.Utilities;

namespace Newtonsoft.Json.Converters
{
	[Preserve]
	public class BsonObjectIdConverter : JsonConverter
	{
		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			BsonObjectId bsonObjectId = (BsonObjectId)value;
			BsonWriter bsonWriter = writer as BsonWriter;
			if (bsonWriter != null)
			{
				bsonWriter.WriteObjectId(bsonObjectId.Value);
			}
			else
			{
				writer.WriteValue(bsonObjectId.Value);
			}
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			if (reader.TokenType != JsonToken.Bytes)
			{
				throw new JsonSerializationException("Expected Bytes but got {0}.".FormatWith(CultureInfo.InvariantCulture, reader.TokenType));
			}
			return new BsonObjectId((byte[])reader.Value);
		}

		public override bool CanConvert(Type objectType)
		{
			return objectType == typeof(BsonObjectId);
		}
	}
}
