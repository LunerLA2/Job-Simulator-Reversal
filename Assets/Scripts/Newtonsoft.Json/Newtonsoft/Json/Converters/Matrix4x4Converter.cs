using System;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Newtonsoft.Json.Converters
{
	public class Matrix4x4Converter : JsonConverter
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
			Matrix4x4 matrix4x = (Matrix4x4)value;
			writer.WriteStartObject();
			writer.WritePropertyName("m00");
			writer.WriteValue(matrix4x.m00);
			writer.WritePropertyName("m01");
			writer.WriteValue(matrix4x.m01);
			writer.WritePropertyName("m02");
			writer.WriteValue(matrix4x.m02);
			writer.WritePropertyName("m03");
			writer.WriteValue(matrix4x.m03);
			writer.WritePropertyName("m10");
			writer.WriteValue(matrix4x.m10);
			writer.WritePropertyName("m11");
			writer.WriteValue(matrix4x.m11);
			writer.WritePropertyName("m12");
			writer.WriteValue(matrix4x.m12);
			writer.WritePropertyName("m13");
			writer.WriteValue(matrix4x.m13);
			writer.WritePropertyName("m20");
			writer.WriteValue(matrix4x.m20);
			writer.WritePropertyName("m21");
			writer.WriteValue(matrix4x.m21);
			writer.WritePropertyName("m22");
			writer.WriteValue(matrix4x.m22);
			writer.WritePropertyName("m23");
			writer.WriteValue(matrix4x.m23);
			writer.WritePropertyName("m30");
			writer.WriteValue(matrix4x.m30);
			writer.WritePropertyName("m31");
			writer.WriteValue(matrix4x.m31);
			writer.WritePropertyName("m32");
			writer.WriteValue(matrix4x.m32);
			writer.WritePropertyName("m33");
			writer.WriteValue(matrix4x.m33);
			writer.WriteEnd();
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			if (reader.TokenType == JsonToken.Null)
			{
				return default(Matrix4x4);
			}
			JObject jObject = JObject.Load(reader);
			Matrix4x4 matrix4x = default(Matrix4x4);
			matrix4x.m00 = (float)jObject["m00"];
			matrix4x.m01 = (float)jObject["m01"];
			matrix4x.m02 = (float)jObject["m02"];
			matrix4x.m03 = (float)jObject["m03"];
			matrix4x.m20 = (float)jObject["m20"];
			matrix4x.m21 = (float)jObject["m21"];
			matrix4x.m22 = (float)jObject["m22"];
			matrix4x.m23 = (float)jObject["m23"];
			matrix4x.m30 = (float)jObject["m30"];
			matrix4x.m31 = (float)jObject["m31"];
			matrix4x.m32 = (float)jObject["m32"];
			matrix4x.m33 = (float)jObject["m33"];
			return matrix4x;
		}

		public override bool CanConvert(Type objectType)
		{
			return objectType == typeof(Matrix4x4);
		}
	}
}
