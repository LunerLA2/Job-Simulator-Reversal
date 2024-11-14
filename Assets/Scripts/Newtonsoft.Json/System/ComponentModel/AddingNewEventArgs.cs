using Newtonsoft.Json.Shims;

namespace System.ComponentModel
{
	[Preserve]
	public class AddingNewEventArgs
	{
		public object NewObject { get; set; }

		public AddingNewEventArgs()
		{
		}

		public AddingNewEventArgs(object newObject)
		{
			NewObject = newObject;
		}
	}
}
