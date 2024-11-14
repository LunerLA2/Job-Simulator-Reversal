using Newtonsoft.Json.Shims;

namespace System.ComponentModel
{
	[Preserve]
	public interface INotifyCollectionChanged
	{
		event NotifyCollectionChangedEventHandler CollectionChanged;
	}
}
