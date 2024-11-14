using System;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Newtonsoft.Json.Converters
{
	public class ColorConverter : JsonConverter
	{
		public override bool CanRead
		{
			get
			{
				return true;
			}
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			if (value == null)
			{
				writer.WriteNull();
				return;
			}
			Color color = (Color)value;
			writer.WriteStartObject();
			writer.WritePropertyName("a");
			writer.WriteValue(color.a);
			writer.WritePropertyName("r");
			writer.WriteValue(color.r);
			writer.WritePropertyName("g");
			writer.WriteValue(color.g);
			writer.WritePropertyName("b");
			writer.WriteValue(color.b);
			writer.WriteEndObject();
		}

		public override bool CanConvert(Type objectType)
		{
			if (objectType != typeof(Color))
			{
				return objectType == typeof(Color32);
			}
			return true;
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			if (reader.TokenType == JsonToken.Null)
			{
				return default(Color);
			}
			JObject jObject = JObject.Load(reader);
			if (objectType == typeof(Color32))
			{
				return new Color32((byte)jObject["r"], (byte)jObject["g"], (byte)jObject["b"], (byte)jObject["a"]);
			}
			return new Color((float)jObject["r"], (float)jObject["g"], (float)jObject["b"], (float)jObject["a"]);
		}
	}
}
