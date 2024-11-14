using UnityEngine;

public class ExtraPrefs
{
	public const string EXTRA_PREF_NAME = "ExtraPref";

	public static int ExtraProgress
	{
		get
		{
			if (!PlayerPrefs.HasKey("ExtraPref"))
			{
				PlayerPrefs.SetInt("ExtraPref", 0);
			}
			return PlayerPrefs.GetInt("ExtraPref");
		}
		set
		{
			PlayerPrefs.SetInt("ExtraPref", value);
		}
	}
}
