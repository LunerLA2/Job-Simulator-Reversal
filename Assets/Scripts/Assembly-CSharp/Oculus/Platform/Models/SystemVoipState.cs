using System;

namespace Oculus.Platform.Models
{
	public class SystemVoipState
	{
		public readonly VoipMuteState MicrophoneMuted;

		public readonly SystemVoipStatus Status;

		public SystemVoipState(IntPtr o)
		{
			MicrophoneMuted = CAPI.ovr_SystemVoipState_GetMicrophoneMuted(o);
			Status = CAPI.ovr_SystemVoipState_GetStatus(o);
		}
	}
}
