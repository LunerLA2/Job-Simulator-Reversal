using Steamworks;
using UnityEngine;

public static class AchievementManager
{
	public static string PSVR_ACHIEVEMENT_PLAYERPREF_KEY = "PSVR_TROPHY_COMPLETED_ID_";

	public static string OCULUS_PREFIX = "Oculus";

	public static string STEAM_PREFIX = "Steam";

	public static void CompleteAchievement(int achievementNum)
	{
		Debug.Log("Completed Achievement: " + achievementNum);
		SteamUserStats.SetAchievement(STEAM_PREFIX + achievementNum);
	}

	public static void ResubmitCompletedTrophies()
	{
	}

	public static void ClearCompletedTrophies()
	{
	}

	private static void CheckAndDeleteTrophy(int num)
	{
	}

	private static void CheckAndResubmitTrophy(int num)
	{
	}
}
