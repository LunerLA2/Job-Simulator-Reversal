using System;

namespace Oculus.Platform.Models
{
	public class MatchmakingAdminSnapshot
	{
		public readonly MatchmakingAdminSnapshotCandidateList Candidates;

		public readonly double MyCurrentThreshold;

		public MatchmakingAdminSnapshot(IntPtr o)
		{
			Candidates = new MatchmakingAdminSnapshotCandidateList(CAPI.ovr_MatchmakingAdminSnapshot_GetCandidates(o));
			MyCurrentThreshold = CAPI.ovr_MatchmakingAdminSnapshot_GetMyCurrentThreshold(o);
		}
	}
}
