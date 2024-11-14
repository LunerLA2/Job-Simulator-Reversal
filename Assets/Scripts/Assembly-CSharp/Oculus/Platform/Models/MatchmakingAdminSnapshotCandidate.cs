using System;

namespace Oculus.Platform.Models
{
	public class MatchmakingAdminSnapshotCandidate
	{
		public readonly bool CanMatch;

		public readonly double MyTotalScore;

		public readonly double TheirCurrentThreshold;

		public readonly double TheirTotalScore;

		public readonly string TraceId;

		public MatchmakingAdminSnapshotCandidate(IntPtr o)
		{
			CanMatch = CAPI.ovr_MatchmakingAdminSnapshotCandidate_GetCanMatch(o);
			MyTotalScore = CAPI.ovr_MatchmakingAdminSnapshotCandidate_GetMyTotalScore(o);
			TheirCurrentThreshold = CAPI.ovr_MatchmakingAdminSnapshotCandidate_GetTheirCurrentThreshold(o);
			TheirTotalScore = CAPI.ovr_MatchmakingAdminSnapshotCandidate_GetTheirTotalScore(o);
			TraceId = CAPI.ovr_MatchmakingAdminSnapshotCandidate_GetTraceId(o);
		}
	}
}
