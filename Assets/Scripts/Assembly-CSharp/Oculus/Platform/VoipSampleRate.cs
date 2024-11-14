using System.ComponentModel;

namespace Oculus.Platform
{
	public enum VoipSampleRate : uint
	{
		[Description("UNKNOWN")]
		Unknown = 0u,
		[Description("HZ24000")]
		HZ24000 = 1u,
		[Description("HZ44100")]
		HZ44100 = 2u,
		[Description("HZ48000")]
		HZ48000 = 3u
	}
}
