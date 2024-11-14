using System;

namespace Oculus.Platform.Models
{
	public class LivestreamingStatus
	{
		public readonly bool LivestreamingEnabled;

		public readonly bool MicEnabled;

		public LivestreamingStatus(IntPtr o)
		{
			LivestreamingEnabled = CAPI.ovr_LivestreamingStatus_GetLivestreamingEnabled(o);
			MicEnabled = CAPI.ovr_LivestreamingStatus_GetMicEnabled(o);
		}
	}
}
