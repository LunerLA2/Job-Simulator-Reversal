using System;
using System.Collections.Generic;

namespace Oculus.Platform.Models
{
	public class AchievementProgressList : DeserializableList<AchievementProgress>
	{
		public AchievementProgressList(IntPtr a)
		{
			int num = (int)(uint)CAPI.ovr_AchievementProgressArray_GetSize(a);
			_Data = new List<AchievementProgress>(num);
			for (int i = 0; i < num; i++)
			{
				_Data.Add(new AchievementProgress(CAPI.ovr_AchievementProgressArray_GetElement(a, (UIntPtr)(ulong)i)));
			}
			_NextUrl = CAPI.ovr_AchievementProgressArray_GetNextUrl(a);
		}
	}
}
