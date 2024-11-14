using System;

namespace Oculus.Platform.Models
{
	public class AchievementProgress
	{
		public readonly string Bitfield;

		public readonly ulong Count;

		public readonly bool IsUnlocked;

		public readonly string Name;

		public readonly DateTime UnlockTime;

		public AchievementProgress(IntPtr o)
		{
			Bitfield = CAPI.ovr_AchievementProgress_GetBitfield(o);
			Count = CAPI.ovr_AchievementProgress_GetCount(o);
			IsUnlocked = CAPI.ovr_AchievementProgress_GetIsUnlocked(o);
			Name = CAPI.ovr_AchievementProgress_GetName(o);
			UnlockTime = CAPI.ovr_AchievementProgress_GetUnlockTime(o);
		}
	}
}
