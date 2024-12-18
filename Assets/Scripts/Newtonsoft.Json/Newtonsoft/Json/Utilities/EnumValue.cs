using Newtonsoft.Json.Shims;

namespace Newtonsoft.Json.Utilities
{
	[Preserve]
	internal class EnumValue<T> where T : struct
	{
		private readonly string _name;

		private readonly T _value;

		public string Name
		{
			get
			{
				return _name;
			}
		}

		public T Value
		{
			get
			{
				return _value;
			}
		}

		public EnumValue(string name, T value)
		{
			_name = name;
			_value = value;
		}
	}
}
