using System;
using UnityEngine;

public class VOContainer
{
	private bool isSimple = true;

	private BotVoiceController.VOImportance importance;

	private AudioClip clip;

	private BotVOInfoData info;

	public Action OnWasRemotelyCancelled;

	public bool IsSimple
	{
		get
		{
			return isSimple;
		}
	}

	public BotVoiceController.VOImportance Importance
	{
		get
		{
			return importance;
		}
	}

	public BotVOInfoData Info
	{
		get
		{
			return info;
		}
	}

	public AudioClip Clip
	{
		get
		{
			if (isSimple)
			{
				return clip;
			}
			if (info != null)
			{
				return info.AudioClip;
			}
			return null;
		}
	}

	public VOContainer(AudioClip _clip, BotVoiceController.VOImportance _importance)
	{
		isSimple = true;
		importance = _importance;
		clip = _clip;
	}

	public VOContainer(BotVOInfoData _info, BotVoiceController.VOImportance _importance)
	{
		isSimple = false;
		importance = _importance;
		info = _info;
		clip = info.AudioClip;
	}

	public void Cancel()
	{
		if (OnWasRemotelyCancelled != null)
		{
			OnWasRemotelyCancelled();
		}
	}
}
