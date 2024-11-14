using System;
using System.Collections.Generic;

namespace Oculus.Platform.Models
{
	public class MatchmakingAdminSnapshotCandidateList : DeserializableList<MatchmakingAdminSnapshotCandidate>
	{
		public MatchmakingAdminSnapshotCandidateList(IntPtr a)
		{
			int num = (int)(uint)CAPI.ovr_MatchmakingAdminSnapshotCandidateArray_GetSize(a);
			_Data = new List<MatchmakingAdminSnapshotCandidate>(num);
			for (int i = 0; i < num; i++)
			{
				_Data.Add(new MatchmakingAdminSnapshotCandidate(CAPI.ovr_MatchmakingAdminSnapshotCandidateArray_GetElement(a, (UIntPtr)(ulong)i)));
			}
		}
	}
}
