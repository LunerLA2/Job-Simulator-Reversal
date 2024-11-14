using System.Runtime.Serialization;
using Newtonsoft.Json.Shims;

namespace Newtonsoft.Json.Serialization
{
	[Preserve]
	public delegate void SerializationCallback(object o, StreamingContext context);
}
