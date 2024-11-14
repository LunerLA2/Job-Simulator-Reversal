using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MunchSFX : ScriptableObject
{
	public AudioClip defaultMunchClip;

	[SerializeField]
	private List<MunchSFXData> _munchSfxData = new List<MunchSFXData>();

	public List<MunchSFXData> MunchSfxData
	{
		get
		{
			return _munchSfxData;
		}
	}
}
