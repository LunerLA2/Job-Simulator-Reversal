using UnityEngine;

public class BotPreviewScene : MonoBehaviour
{
	public BotCostumeData costume;

	public AudioClip testVOClip;

	public BotVOInfoData testVOData;

	private Bot bot;

	private void Awake()
	{
		BotManager.Instance.InitializeJob(null);
		bot = BotManager.Instance.GetBot();
		bot.SetupBotNoBrain(costume);
		bot.transform.position = Vector3.zero;
		Invoke("PlayVO", 1f);
	}

	private void Update()
	{
		if (bot != null)
		{
			bot.ForceRefreshFace();
		}
	}

	public void ApplyCostume()
	{
		if (costume != null)
		{
			bot.ChangeCostume(costume);
		}
	}

	public void PlayVO()
	{
		if (testVOData != null)
		{
			bot.PlayVO(testVOData, BotVoiceController.VOImportance.OverrideAllSpeakers);
		}
		else if (testVOClip != null)
		{
			bot.PlayVO(testVOClip, BotVoiceController.VOImportance.OverrideAllSpeakers);
		}
	}

	public void PlayVOClip()
	{
		if (testVOClip != null)
		{
			bot.PlayVO(testVOClip, BotVoiceController.VOImportance.OverrideAllSpeakers);
		}
	}

	public void PlayVOData()
	{
		if (testVOData != null)
		{
			bot.PlayVO(testVOData, BotVoiceController.VOImportance.OverrideAllSpeakers);
		}
	}

	public void SetEmote(BotFaceEmote emote)
	{
		bot.Emote(emote, null);
	}

	public void SetCustomGraphic(Sprite sprite)
	{
		bot.Emote(BotFaceEmote.CustomGraphic, sprite);
	}
}
