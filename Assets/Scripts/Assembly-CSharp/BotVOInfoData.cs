using System.Collections.Generic;
using UnityEngine;

public class BotVOInfoData : ScriptableObject
{
	[SerializeField]
	private AudioClip audioClip;

	[SerializeField]
	private List<BotVOEmoteEvent> events = new List<BotVOEmoteEvent>();

	public AudioClip AudioClip
	{
		get
		{
			return audioClip;
		}
	}

	public List<BotVOEmoteEvent> Events
	{
		get
		{
			return events;
		}
	}

	public void InternalSetAudioClip(AudioClip clip)
	{
		audioClip = clip;
	}
}
