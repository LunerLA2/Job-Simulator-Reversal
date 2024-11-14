using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using OwlchemyVR;
using UnityEngine;

[Serializable]
public class MunchSFXData
{
	public List<AudioClip> MunchClips = new List<AudioClip>();

	public List<WorldItemData> MunchWorldItems = new List<WorldItemData>();

	private ElementSequence<AudioClip> munchClipElementSequence;

	[IgnoreDataMember]
	public ElementSequence<AudioClip> MunchClipElementSequence
	{
		get
		{
			if (munchClipElementSequence == null)
			{
				munchClipElementSequence = new ElementSequence<AudioClip>(MunchClips.ToArray());
			}
			return munchClipElementSequence;
		}
	}
}
