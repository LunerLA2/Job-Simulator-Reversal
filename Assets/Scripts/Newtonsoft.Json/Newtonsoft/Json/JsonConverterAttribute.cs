using System;
using Newtonsoft.Json.Shims;

namespace Newtonsoft.Json
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Interface | AttributeTargets.Parameter, AllowMultiple = false)]
	[Preserve]
	public sealed class JsonConverterAttribute : Attribute
	{
		private readonly Type _converterType;

		public Type ConverterType
		{
			get
			{
				return _converterType;
			}
		}

		public object[] ConverterParameters { get; private set; }

		public JsonConverterAttribute(Type converterType)
		{
			if (converterType == null)
			{
				throw new ArgumentNullException("converterType");
			}
			_converterType = converterType;
		}

		public JsonConverterAttribute(Type converterType, params object[] converterParameters)
			: this(converterType)
		{
			ConverterParameters = converterParameters;
		}
	}
}
