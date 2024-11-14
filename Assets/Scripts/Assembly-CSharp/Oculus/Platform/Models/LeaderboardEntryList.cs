using System;
using System.Collections.Generic;

namespace Oculus.Platform.Models
{
	public class LeaderboardEntryList : DeserializableList<LeaderboardEntry>
	{
		public readonly ulong TotalCount;

		public LeaderboardEntryList(IntPtr a)
		{
			int num = (int)(uint)CAPI.ovr_LeaderboardEntryArray_GetSize(a);
			_Data = new List<LeaderboardEntry>(num);
			for (int i = 0; i < num; i++)
			{
				_Data.Add(new LeaderboardEntry(CAPI.ovr_LeaderboardEntryArray_GetElement(a, (UIntPtr)(ulong)i)));
			}
			TotalCount = CAPI.ovr_LeaderboardEntryArray_GetTotalCount(a);
			_PreviousUrl = CAPI.ovr_LeaderboardEntryArray_GetPreviousUrl(a);
			_NextUrl = CAPI.ovr_LeaderboardEntryArray_GetNextUrl(a);
		}
	}
}
