using System.ComponentModel;

namespace Oculus.Platform
{
	public enum KeyValuePairType : uint
	{
		[Description("STRING")]
		String = 0u,
		[Description("INTEGER")]
		Int = 1u,
		[Description("DOUBLE")]
		Double = 2u,
		[Description("UNKNOWN")]
		Unknown = 3u
	}
}
