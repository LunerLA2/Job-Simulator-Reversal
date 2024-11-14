using UnityEngine;

public class PromotionRankNameGenerator
{
	private const int RANK_UP_AMOUT = 5;

	private static string[] PriamryRanks = new string[11]
	{
		"Intern", "Employee", "Manager", "Executive", "President", "Bot Intern", "Bot Employee", "Bot Manager", "Bot Executive", "Bot President",
		"Singularity"
	};

	private static string[] SecondaryRanks = new string[12]
	{
		"Intern to the {0}", "Assistant to the {0}", "Associate {0}", "Junior {0}", "Senior {0}", "Principal {0}", "Manager {0}", "Director of {0}s", "Managing Director of {0}s", "Executive Director of {0}s",
		"VP of {0}s", "Chief {0} Officer"
	};

	private static string FinalRank = "The Singularity";

	private static string PrestigeRank = "BUFFER_OVERFLOW_{0:X3}";

	public static int GetSecondaryRank(int score)
	{
		int num = Mathf.FloorToInt(score / 5);
		return num % SecondaryRanks.Length;
	}

	public static string GetRankName(int score)
	{
		int num = Mathf.FloorToInt(score / 5);
		int num2 = Mathf.FloorToInt(num / SecondaryRanks.Length);
		int secondaryRank = GetSecondaryRank(score);
		int num3 = PriamryRanks.Length * SecondaryRanks.Length;
		if (num == num3)
		{
			return FinalRank;
		}
		if (num > num3)
		{
			return string.Format(PrestigeRank, num - num3);
		}
		return string.Format(SecondaryRanks[secondaryRank], PriamryRanks[num2]).Replace("ys", "ies");
	}
}
