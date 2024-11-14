using System;
using System.Diagnostics;
using Newtonsoft.Json.Shims;

namespace Newtonsoft.Json.Serialization
{
	[Preserve]
	public interface ITraceWriter
	{
		TraceLevel LevelFilter { get; }

		void Trace(TraceLevel level, string message, Exception ex);
	}
}
