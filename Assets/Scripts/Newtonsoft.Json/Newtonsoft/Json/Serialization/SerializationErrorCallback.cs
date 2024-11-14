using System.Runtime.Serialization;
using Newtonsoft.Json.Shims;

namespace Newtonsoft.Json.Serialization
{
	[Preserve]
	public delegate void SerializationErrorCallback(object o, StreamingContext context, ErrorContext errorContext);
}
