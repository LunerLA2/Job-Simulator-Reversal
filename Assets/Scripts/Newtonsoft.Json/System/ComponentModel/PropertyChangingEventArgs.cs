using Newtonsoft.Json.Shims;

namespace System.ComponentModel
{
	[Preserve]
	public class PropertyChangingEventArgs : EventArgs
	{
		public virtual string PropertyName { get; set; }

		public PropertyChangingEventArgs(string propertyName)
		{
			PropertyName = propertyName;
		}
	}
}
