using System;
using OwlchemyVR;
using UnityEngine;

[CreateAssetMenu(fileName = "WorldItemWithIcon", menuName = "World Item with Icon")]
public class WorldItemWithIconData : ScriptableObject
{
	[Serializable]
	public class ItemNameAudio
	{
		[SerializeField]
		private BotVoiceType voiceType;

		[SerializeField]
		private AudioClip itemNameAudioClip;

		public BotVoiceType VoiceType
		{
			get
			{
				return voiceType;
			}
		}

		public AudioClip ItemNameAudioClip
		{
			get
			{
				return itemNameAudioClip;
			}
		}
	}

	[SerializeField]
	private WorldItemData worldItemData;

	[SerializeField]
	private Sprite icon;

	[SerializeField]
	private ItemNameAudio[] voicedItemNames;

	[SerializeField]
	private GameObject prefab;

	[SerializeField]
	private bool overrideTaskIcon = true;

	public WorldItemData WorldItemData
	{
		get
		{
			return worldItemData;
		}
	}

	public Sprite Icon
	{
		get
		{
			if (overrideTaskIcon)
			{
				return icon;
			}
			return null;
		}
	}

	public GameObject Prefab
	{
		get
		{
			return prefab;
		}
	}

	public AudioClip GetItemNameAudioClip(BotVoiceType voiceType)
	{
		for (int i = 0; i < voicedItemNames.Length; i++)
		{
			if (voicedItemNames[i].VoiceType == voiceType)
			{
				return voicedItemNames[i].ItemNameAudioClip;
			}
		}
		return null;
	}
}
