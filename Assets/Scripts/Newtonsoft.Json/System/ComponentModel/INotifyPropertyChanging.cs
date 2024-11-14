using Newtonsoft.Json.Shims;

namespace System.ComponentModel
{
	[Preserve]
	public interface INotifyPropertyChanging
	{
		event PropertyChangingEventHandler PropertyChanging;
	}
}
