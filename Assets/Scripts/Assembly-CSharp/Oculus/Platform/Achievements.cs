using Oculus.Platform.Models;
using UnityEngine;

namespace Oculus.Platform
{
	public static class Achievements
	{
		public static Request<AchievementDefinitionList> GetAllDefinitions()
		{
			if (Core.IsInitialized())
			{
				return new Request<AchievementDefinitionList>(CAPI.ovr_Achievements_GetAllDefinitions());
			}
			return null;
		}

		public static Request<AchievementDefinitionList> GetDefinitionsByName(string[] names)
		{
			if (Core.IsInitialized())
			{
				return new Request<AchievementDefinitionList>(CAPI.ovr_Achievements_GetDefinitionsByName(names, names.Length));
			}
			return null;
		}

		public static Request<AchievementProgressList> GetAllProgress()
		{
			if (Core.IsInitialized())
			{
				return new Request<AchievementProgressList>(CAPI.ovr_Achievements_GetAllProgress());
			}
			return null;
		}

		public static Request<AchievementProgressList> GetProgressByName(string[] names)
		{
			if (Core.IsInitialized())
			{
				return new Request<AchievementProgressList>(CAPI.ovr_Achievements_GetProgressByName(names, names.Length));
			}
			return null;
		}

		public static Request Unlock(string name)
		{
			if (Core.IsInitialized())
			{
				return new Request(CAPI.ovr_Achievements_Unlock(name));
			}
			return null;
		}

		public static Request AddCount(string name, ulong count)
		{
			if (Core.IsInitialized())
			{
				return new Request(CAPI.ovr_Achievements_AddCount(name, count));
			}
			return null;
		}

		public static Request AddFields(string name, string fields)
		{
			if (Core.IsInitialized())
			{
				return new Request(CAPI.ovr_Achievements_AddFields(name, fields));
			}
			return null;
		}

		public static Request<AchievementDefinitionList> GetNextAchievementDefinitionListPage(AchievementDefinitionList list)
		{
			if (!list.HasNextPage)
			{
				Debug.LogWarning("Oculus.Platform.GetNextAchievementDefinitionListPage: List has no next page");
				return null;
			}
			if (Core.IsInitialized())
			{
				return new Request<AchievementDefinitionList>(CAPI.ovr_HTTP_GetWithMessageType(list.NextUrl, 712888917));
			}
			return null;
		}

		public static Request<AchievementProgressList> GetNextAchievementProgressListPage(AchievementProgressList list)
		{
			if (!list.HasNextPage)
			{
				Debug.LogWarning("Oculus.Platform.GetNextAchievementProgressListPage: List has no next page");
				return null;
			}
			if (Core.IsInitialized())
			{
				return new Request<AchievementProgressList>(CAPI.ovr_HTTP_GetWithMessageType(list.NextUrl, 792913703));
			}
			return null;
		}
	}
}
