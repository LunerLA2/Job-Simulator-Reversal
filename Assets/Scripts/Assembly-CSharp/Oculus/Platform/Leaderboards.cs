using Oculus.Platform.Models;

namespace Oculus.Platform
{
	public static class Leaderboards
	{
		public static Request WriteEntry(string leaderboardName, long score, byte[] extraData = null, bool forceUpdate = false)
		{
			if (Core.IsInitialized())
			{
				return new Request(CAPI.ovr_Leaderboard_WriteEntry(leaderboardName, score, extraData, (extraData != null) ? ((uint)extraData.Length) : 0u, forceUpdate));
			}
			return null;
		}

		public static Request<LeaderboardEntryList> GetEntries(string leaderboardName, int limit, LeaderboardFilterType filter, LeaderboardStartAt startAt)
		{
			if (Core.IsInitialized())
			{
				return new Request<LeaderboardEntryList>(CAPI.ovr_Leaderboard_GetEntries(leaderboardName, limit, filter, startAt));
			}
			return null;
		}

		public static Request<LeaderboardEntryList> GetEntriesAfterRank(string leaderboardName, int limit, ulong afterRank)
		{
			if (Core.IsInitialized())
			{
				return new Request<LeaderboardEntryList>(CAPI.ovr_Leaderboard_GetEntriesAfterRank(leaderboardName, limit, afterRank));
			}
			return null;
		}

		public static Request<LeaderboardEntryList> GetNextEntries(LeaderboardEntryList list)
		{
			if (Core.IsInitialized())
			{
				return new Request<LeaderboardEntryList>(CAPI.ovr_HTTP_GetWithMessageType(list.NextUrl, 1310751961));
			}
			return null;
		}

		public static Request<LeaderboardEntryList> GetPreviousEntries(LeaderboardEntryList list)
		{
			if (Core.IsInitialized())
			{
				return new Request<LeaderboardEntryList>(CAPI.ovr_HTTP_GetWithMessageType(list.PreviousUrl, 1224858304));
			}
			return null;
		}
	}
}
