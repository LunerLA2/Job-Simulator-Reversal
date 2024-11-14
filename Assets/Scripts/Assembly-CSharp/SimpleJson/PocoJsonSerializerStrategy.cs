using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;
using SimpleJson.Reflection;

namespace SimpleJson
{
	[GeneratedCode("simple-json", "1.0.0")]
	public class PocoJsonSerializerStrategy : IJsonSerializerStrategy
	{
		internal IDictionary<Type, global::SimpleJson.Reflection.ReflectionUtils.ConstructorDelegate> ConstructorCache;

		internal IDictionary<Type, IDictionary<string, global::SimpleJson.Reflection.ReflectionUtils.GetDelegate>> GetCache;

		internal IDictionary<Type, IDictionary<string, KeyValuePair<Type, global::SimpleJson.Reflection.ReflectionUtils.SetDelegate>>> SetCache;

		internal static readonly Type[] EmptyTypes = new Type[0];

		internal static readonly Type[] ArrayConstructorParameterTypes = new Type[1] { typeof(int) };

		private static readonly string[] Iso8601Format = new string[3] { "yyyy-MM-dd\\THH:mm:ss.FFFFFFF\\Z", "yyyy-MM-dd\\THH:mm:ss\\Z", "yyyy-MM-dd\\THH:mm:ssK" };

		public PocoJsonSerializerStrategy()
		{
			ConstructorCache = new global::SimpleJson.Reflection.ReflectionUtils.ThreadSafeDictionary<Type, global::SimpleJson.Reflection.ReflectionUtils.ConstructorDelegate>(ContructorDelegateFactory);
			GetCache = new global::SimpleJson.Reflection.ReflectionUtils.ThreadSafeDictionary<Type, IDictionary<string, global::SimpleJson.Reflection.ReflectionUtils.GetDelegate>>(GetterValueFactory);
			SetCache = new global::SimpleJson.Reflection.ReflectionUtils.ThreadSafeDictionary<Type, IDictionary<string, KeyValuePair<Type, global::SimpleJson.Reflection.ReflectionUtils.SetDelegate>>>(SetterValueFactory);
		}

		protected virtual string MapClrMemberNameToJsonFieldName(string clrPropertyName)
		{
			return clrPropertyName;
		}

		internal virtual global::SimpleJson.Reflection.ReflectionUtils.ConstructorDelegate ContructorDelegateFactory(Type key)
		{
			return global::SimpleJson.Reflection.ReflectionUtils.GetContructor(key, (!key.IsArray) ? EmptyTypes : ArrayConstructorParameterTypes);
		}

		internal virtual IDictionary<string, global::SimpleJson.Reflection.ReflectionUtils.GetDelegate> GetterValueFactory(Type type)
		{
			IDictionary<string, global::SimpleJson.Reflection.ReflectionUtils.GetDelegate> dictionary = new Dictionary<string, global::SimpleJson.Reflection.ReflectionUtils.GetDelegate>();
			foreach (PropertyInfo property in global::SimpleJson.Reflection.ReflectionUtils.GetProperties(type))
			{
				if (property.CanRead)
				{
					MethodInfo getterMethodInfo = global::SimpleJson.Reflection.ReflectionUtils.GetGetterMethodInfo(property);
					if (!getterMethodInfo.IsStatic && getterMethodInfo.IsPublic)
					{
						dictionary[MapClrMemberNameToJsonFieldName(property.Name)] = global::SimpleJson.Reflection.ReflectionUtils.GetGetMethod(property);
					}
				}
			}
			foreach (FieldInfo field in global::SimpleJson.Reflection.ReflectionUtils.GetFields(type))
			{
				if (!field.IsStatic && field.IsPublic)
				{
					dictionary[MapClrMemberNameToJsonFieldName(field.Name)] = global::SimpleJson.Reflection.ReflectionUtils.GetGetMethod(field);
				}
			}
			return dictionary;
		}

		internal virtual IDictionary<string, KeyValuePair<Type, global::SimpleJson.Reflection.ReflectionUtils.SetDelegate>> SetterValueFactory(Type type)
		{
			IDictionary<string, KeyValuePair<Type, global::SimpleJson.Reflection.ReflectionUtils.SetDelegate>> dictionary = new Dictionary<string, KeyValuePair<Type, global::SimpleJson.Reflection.ReflectionUtils.SetDelegate>>();
			foreach (PropertyInfo property in global::SimpleJson.Reflection.ReflectionUtils.GetProperties(type))
			{
				if (property.CanWrite)
				{
					MethodInfo setterMethodInfo = global::SimpleJson.Reflection.ReflectionUtils.GetSetterMethodInfo(property);
					if (!setterMethodInfo.IsStatic && setterMethodInfo.IsPublic)
					{
						dictionary[MapClrMemberNameToJsonFieldName(property.Name)] = new KeyValuePair<Type, global::SimpleJson.Reflection.ReflectionUtils.SetDelegate>(property.PropertyType, global::SimpleJson.Reflection.ReflectionUtils.GetSetMethod(property));
					}
				}
			}
			foreach (FieldInfo field in global::SimpleJson.Reflection.ReflectionUtils.GetFields(type))
			{
				if (!field.IsInitOnly && !field.IsStatic && field.IsPublic)
				{
					dictionary[MapClrMemberNameToJsonFieldName(field.Name)] = new KeyValuePair<Type, global::SimpleJson.Reflection.ReflectionUtils.SetDelegate>(field.FieldType, global::SimpleJson.Reflection.ReflectionUtils.GetSetMethod(field));
				}
			}
			return dictionary;
		}

		public virtual bool TrySerializeNonPrimitiveObject(object input, out object output)
		{
			return TrySerializeKnownTypes(input, out output) || TrySerializeUnknownTypes(input, out output);
		}

		[SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
		public virtual object DeserializeObject(object value, Type type)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			string text = value as string;
			if (type == typeof(Guid) && string.IsNullOrEmpty(text))
			{
				return default(Guid);
			}
			if (value == null)
			{
				return null;
			}
			object obj = null;
			if (text != null)
			{
				if (text.Length != 0)
				{
					if (type == typeof(DateTime) || (global::SimpleJson.Reflection.ReflectionUtils.IsNullableType(type) && Nullable.GetUnderlyingType(type) == typeof(DateTime)))
					{
						return DateTime.ParseExact(text, Iso8601Format, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal);
					}
					if (type == typeof(DateTimeOffset) || (global::SimpleJson.Reflection.ReflectionUtils.IsNullableType(type) && Nullable.GetUnderlyingType(type) == typeof(DateTimeOffset)))
					{
						return DateTimeOffset.ParseExact(text, Iso8601Format, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal);
					}
					if (type == typeof(Guid) || (global::SimpleJson.Reflection.ReflectionUtils.IsNullableType(type) && Nullable.GetUnderlyingType(type) == typeof(Guid)))
					{
						return new Guid(text);
					}
					if (type == typeof(Uri))
					{
						Uri result;
						if (Uri.IsWellFormedUriString(text, UriKind.RelativeOrAbsolute) && Uri.TryCreate(text, UriKind.RelativeOrAbsolute, out result))
						{
							return result;
						}
						return null;
					}
					if (type == typeof(string))
					{
						return text;
					}
					return Convert.ChangeType(text, type, CultureInfo.InvariantCulture);
				}
				obj = ((type == typeof(Guid)) ? ((object)default(Guid)) : ((!global::SimpleJson.Reflection.ReflectionUtils.IsNullableType(type) || Nullable.GetUnderlyingType(type) != typeof(Guid)) ? text : null));
				if (!global::SimpleJson.Reflection.ReflectionUtils.IsNullableType(type) && Nullable.GetUnderlyingType(type) == typeof(Guid))
				{
					return text;
				}
			}
			else if (value is bool)
			{
				return value;
			}
			bool flag = value is long;
			bool flag2 = value is double;
			if ((flag && type == typeof(long)) || (flag2 && type == typeof(double)))
			{
				return value;
			}
			if ((flag2 && type != typeof(double)) || (flag && type != typeof(long)))
			{
				obj = ((type != typeof(int) && type != typeof(long) && type != typeof(double) && type != typeof(float) && type != typeof(bool) && type != typeof(decimal) && type != typeof(byte) && type != typeof(short)) ? value : Convert.ChangeType(value, type, CultureInfo.InvariantCulture));
				if (global::SimpleJson.Reflection.ReflectionUtils.IsNullableType(type))
				{
					return global::SimpleJson.Reflection.ReflectionUtils.ToNullableType(obj, type);
				}
				return obj;
			}
			IDictionary<string, object> dictionary = value as IDictionary<string, object>;
			if (dictionary != null)
			{
				IDictionary<string, object> dictionary2 = dictionary;
				if (global::SimpleJson.Reflection.ReflectionUtils.IsTypeDictionary(type))
				{
					Type[] genericTypeArguments = global::SimpleJson.Reflection.ReflectionUtils.GetGenericTypeArguments(type);
					Type type2 = genericTypeArguments[0];
					Type type3 = genericTypeArguments[1];
					Type key = typeof(Dictionary<, >).MakeGenericType(type2, type3);
					IDictionary dictionary3 = (IDictionary)ConstructorCache[key]();
					foreach (KeyValuePair<string, object> item in dictionary2)
					{
						dictionary3.Add(item.Key, DeserializeObject(item.Value, type3));
					}
					obj = dictionary3;
				}
				else if (type == typeof(object))
				{
					obj = value;
				}
				else
				{
					obj = ConstructorCache[type]();
					foreach (KeyValuePair<string, KeyValuePair<Type, global::SimpleJson.Reflection.ReflectionUtils.SetDelegate>> item2 in SetCache[type])
					{
						object value2;
						if (dictionary2.TryGetValue(item2.Key, out value2))
						{
							value2 = DeserializeObject(value2, item2.Value.Key);
							item2.Value.Value(obj, value2);
						}
					}
				}
			}
			else
			{
				IList<object> list = value as IList<object>;
				if (list != null)
				{
					IList<object> list2 = list;
					IList list3 = null;
					if (type.IsArray)
					{
						list3 = (IList)ConstructorCache[type](list2.Count);
						int num = 0;
						foreach (object item3 in list2)
						{
							list3[num++] = DeserializeObject(item3, type.GetElementType());
						}
					}
					else if (global::SimpleJson.Reflection.ReflectionUtils.IsTypeGenericeCollectionInterface(type) || global::SimpleJson.Reflection.ReflectionUtils.IsAssignableFrom(typeof(IList), type))
					{
						Type genericListElementType = global::SimpleJson.Reflection.ReflectionUtils.GetGenericListElementType(type);
						list3 = (IList)(ConstructorCache[type] ?? ConstructorCache[typeof(List<>).MakeGenericType(genericListElementType)])(list2.Count);
						foreach (object item4 in list2)
						{
							list3.Add(DeserializeObject(item4, genericListElementType));
						}
					}
					obj = list3;
				}
			}
			return obj;
		}

		protected virtual object SerializeEnum(Enum p)
		{
			return Convert.ToDouble(p, CultureInfo.InvariantCulture);
		}

		[SuppressMessage("Microsoft.Design", "CA1007:UseGenericsWhereAppropriate", Justification = "Need to support .NET 2")]
		protected virtual bool TrySerializeKnownTypes(object input, out object output)
		{
			bool result = true;
			if (input is DateTime)
			{
				output = ((DateTime)input).ToUniversalTime().ToString(Iso8601Format[0], CultureInfo.InvariantCulture);
			}
			else if (input is DateTimeOffset)
			{
				output = ((DateTimeOffset)input).ToUniversalTime().ToString(Iso8601Format[0], CultureInfo.InvariantCulture);
			}
			else if (input is Guid)
			{
				output = ((Guid)input).ToString("D");
			}
			else if (input is Uri)
			{
				output = input.ToString();
			}
			else
			{
				Enum @enum = input as Enum;
				if (@enum != null)
				{
					output = SerializeEnum(@enum);
				}
				else
				{
					result = false;
					output = null;
				}
			}
			return result;
		}

		[SuppressMessage("Microsoft.Design", "CA1007:UseGenericsWhereAppropriate", Justification = "Need to support .NET 2")]
		protected virtual bool TrySerializeUnknownTypes(object input, out object output)
		{
			if (input == null)
			{
				throw new ArgumentNullException("input");
			}
			output = null;
			Type type = input.GetType();
			if (type.FullName == null)
			{
				return false;
			}
			IDictionary<string, object> dictionary = new JsonObject();
			IDictionary<string, global::SimpleJson.Reflection.ReflectionUtils.GetDelegate> dictionary2 = GetCache[type];
			foreach (KeyValuePair<string, global::SimpleJson.Reflection.ReflectionUtils.GetDelegate> item in dictionary2)
			{
				if (item.Value != null)
				{
					dictionary.Add(MapClrMemberNameToJsonFieldName(item.Key), item.Value(input));
				}
			}
			output = dictionary;
			return true;
		}
	}
}
