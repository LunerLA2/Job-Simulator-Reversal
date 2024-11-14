using System;

namespace Oculus.Platform.Models
{
	public class LeaderboardEntry
	{
		public readonly byte[] ExtraData;

		public readonly int Rank;

		public readonly long Score;

		public readonly DateTime Timestamp;

		public readonly User User;

		public LeaderboardEntry(IntPtr o)
		{
			ExtraData = CAPI.ovr_LeaderboardEntry_GetExtraData(o);
			Rank = CAPI.ovr_LeaderboardEntry_GetRank(o);
			Score = CAPI.ovr_LeaderboardEntry_GetScore(o);
			Timestamp = CAPI.ovr_LeaderboardEntry_GetTimestamp(o);
			User = new User(CAPI.ovr_LeaderboardEntry_GetUser(o));
		}
	}
}
