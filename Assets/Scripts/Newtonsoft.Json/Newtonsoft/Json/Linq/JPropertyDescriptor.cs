using System;
using System.ComponentModel;
using Newtonsoft.Json.Shims;

namespace Newtonsoft.Json.Linq
{
	[Preserve]
	public class JPropertyDescriptor : PropertyDescriptor
	{
		public override Type ComponentType
		{
			get
			{
				return typeof(JObject);
			}
		}

		public override bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		public override Type PropertyType
		{
			get
			{
				return typeof(object);
			}
		}

		protected override int NameHashCode
		{
			get
			{
				return base.NameHashCode;
			}
		}

		public JPropertyDescriptor(string name)
			: base(name, null)
		{
		}

		private static JObject CastInstance(object instance)
		{
			return (JObject)instance;
		}

		public override bool CanResetValue(object component)
		{
			return false;
		}

		public override object GetValue(object component)
		{
			return CastInstance(component)[Name];
		}

		public override void ResetValue(object component)
		{
		}

		public override void SetValue(object component, object value)
		{
			JToken value2 = ((value is JToken) ? ((JToken)value) : new JValue(value));
			CastInstance(component)[Name] = value2;
		}

		public override bool ShouldSerializeValue(object component)
		{
			return false;
		}
	}
}
