using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Newtonsoft.Json.Converters
{
	public class EnumerableVectorConverter<T> : JsonConverter
	{
		private static readonly VectorConverter VectorConverter = new VectorConverter();

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
			}
			IEnumerable<T> obj = value as IEnumerable<T>;
			T[] array = ((obj != null) ? obj.ToArray() : null);
			if (array == null)
			{
				writer.WriteNull();
				return;
			}
			writer.WriteStartArray();
			for (int i = 0; i < array.Length; i++)
			{
				VectorConverter.WriteJson(writer, array[i], serializer);
			}
			writer.WriteEndArray();
		}

		public override bool CanConvert(Type objectType)
		{
			if (!typeof(IEnumerable<Vector2>).IsAssignableFrom(objectType) && !typeof(IEnumerable<Vector3>).IsAssignableFrom(objectType))
			{
				return typeof(IEnumerable<Vector4>).IsAssignableFrom(objectType);
			}
			return true;
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			if (reader.TokenType == JsonToken.Null)
			{
				return null;
			}
			List<T> list = new List<T>();
			JObject jObject = JObject.Load(reader);
			for (int i = 0; i < jObject.Count; i++)
			{
				list.Add(JsonConvert.DeserializeObject<T>(jObject[i].ToString()));
			}
			return list;
		}
	}
}
