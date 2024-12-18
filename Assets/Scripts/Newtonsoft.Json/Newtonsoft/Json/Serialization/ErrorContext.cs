using System;
using Newtonsoft.Json.Shims;

namespace Newtonsoft.Json.Serialization
{
	[Preserve]
	public class ErrorContext
	{
		internal bool Traced { get; set; }

		public Exception Error { get; private set; }

		public object OriginalObject { get; private set; }

		public object Member { get; private set; }

		public string Path { get; private set; }

		public bool Handled { get; set; }

		internal ErrorContext(object originalObject, object member, string path, Exception error)
		{
			OriginalObject = originalObject;
			Member = member;
			Error = error;
			Path = path;
		}
	}
}
