using System;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Newtonsoft.Json.Converters
{
	public class ResolutionConverter : JsonConverter
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
			Resolution resolution = (Resolution)value;
			writer.WriteStartObject();
			writer.WritePropertyName("height");
			writer.WriteValue(resolution.height);
			writer.WritePropertyName("width");
			writer.WriteValue(resolution.width);
			writer.WritePropertyName("refreshRate");
			writer.WriteValue(resolution.refreshRate);
			writer.WriteEndObject();
		}

		public override bool CanConvert(Type objectType)
		{
			return objectType == typeof(Resolution);
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			JObject jObject = JObject.Load(reader);
			Resolution resolution = default(Resolution);
			resolution.height = (int)jObject["height"];
			resolution.width = (int)jObject["width"];
			resolution.refreshRate = (int)jObject["refreshRate"];
			return resolution;
		}
	}
}
