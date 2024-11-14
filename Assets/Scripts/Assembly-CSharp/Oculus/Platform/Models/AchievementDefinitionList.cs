using System;
using System.Collections.Generic;

namespace Oculus.Platform.Models
{
	public class AchievementDefinitionList : DeserializableList<AchievementDefinition>
	{
		public AchievementDefinitionList(IntPtr a)
		{
			int num = (int)(uint)CAPI.ovr_AchievementDefinitionArray_GetSize(a);
			_Data = new List<AchievementDefinition>(num);
			for (int i = 0; i < num; i++)
			{
				_Data.Add(new AchievementDefinition(CAPI.ovr_AchievementDefinitionArray_GetElement(a, (UIntPtr)(ulong)i)));
			}
			_NextUrl = CAPI.ovr_AchievementDefinitionArray_GetNextUrl(a);
		}
	}
}
