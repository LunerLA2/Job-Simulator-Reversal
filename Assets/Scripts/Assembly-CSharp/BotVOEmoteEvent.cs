using System;
using UnityEngine;

[Serializable]
public class BotVOEmoteEvent
{
	[SerializeField]
	private float time;

	[SerializeField]
	private BotFaceEmote emote;

	[SerializeField]
	private Sprite customGraphic;

	public float Time
	{
		get
		{
			return time;
		}
	}

	public BotFaceEmote Emote
	{
		get
		{
			return emote;
		}
	}

	public Sprite CustomGraphic
	{
		get
		{
			return customGraphic;
		}
	}

	public void InternalSetTime(float t)
	{
		time = t;
	}

	public void InternalSetEmote(BotFaceEmote e)
	{
		emote = e;
	}

	public void InternalSetCustomGraphic(Sprite s)
	{
		customGraphic = s;
	}
}
