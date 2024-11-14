using UnityEngine;

public class BotMouthData : ScriptableObject
{
	[SerializeField]
	private MouthLineSettings mouthNormalSettings;

	[SerializeField]
	private MouthLineSettings mouthHappySettings;

	[SerializeField]
	private MouthLineSettings mouthSadSettings;

	[SerializeField]
	private MouthLineSettings mouthAngrySettings;

	[SerializeField]
	private MouthLineSettings mouthShiftySettings;

	[SerializeField]
	private MouthLineSettings mouthTechnoSettings;

	[SerializeField]
	private MouthLineSettings mouthTiredSettings;

	[SerializeField]
	private MouthLineSettings mouthTired2Settings;

	public MouthLineSettings GetMouthLineSettingsForEmote(BotFaceEmote emote)
	{
		switch (emote)
		{
		case BotFaceEmote.Idle:
			return mouthNormalSettings;
		case BotFaceEmote.Angry:
			return mouthAngrySettings;
		case BotFaceEmote.Happy:
			return mouthHappySettings;
		case BotFaceEmote.Sad:
			return mouthSadSettings;
		case BotFaceEmote.Shifty:
			return mouthShiftySettings;
		case BotFaceEmote.Techno:
			return mouthTechnoSettings;
		case BotFaceEmote.Tired:
			return mouthTiredSettings;
		case BotFaceEmote.Tired2:
			return mouthTired2Settings;
		default:
			return null;
		}
	}
}
