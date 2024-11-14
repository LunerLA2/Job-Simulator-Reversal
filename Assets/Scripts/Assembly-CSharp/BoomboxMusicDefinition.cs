using System;
using OwlchemyVR;
using UnityEngine;

[Serializable]
public class BoomboxMusicDefinition
{
	[SerializeField]
	private WorldItemData discWorldItem;

	[SerializeField]
	private AudioClip musicClip;

	public WorldItemData DiscWorldItem
	{
		get
		{
			return discWorldItem;
		}
	}

	public AudioClip MusicClip
	{
		get
		{
			return musicClip;
		}
	}
}
