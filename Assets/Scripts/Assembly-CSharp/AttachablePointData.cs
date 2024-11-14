using System;
using OwlchemyVR;
using UnityEngine;

[Serializable]
public class AttachablePointData : WorldItemData
{
	public const float DEFAULT_ATTACHABLE_RANGE = 0.1f;

	[SerializeField]
	private bool requiresRigidbodyRemover;

	[SerializeField]
	private AudioClip[] soundsWhenSomethingAttached;

	private ElementSequence<AudioClip> soundsWhenAttachedSequence;

	[SerializeField]
	private AudioClip[] soundsWhenSomethingDetached;

	private ElementSequence<AudioClip> soundsWhenDetachedSequence;

	[SerializeField]
	private AudioClip[] soundsWhenSomethingRefilled;

	private ElementSequence<AudioClip> soundsWhenRefilledSequence;

	public bool RequiresRigidbodyRemover
	{
		get
		{
			return requiresRigidbodyRemover;
		}
	}

	public AudioClip SoundWhenSomethingAttached
	{
		get
		{
			if (soundsWhenSomethingAttached.Length == 0)
			{
				return null;
			}
			if (soundsWhenAttachedSequence == null)
			{
				soundsWhenAttachedSequence = new ElementSequence<AudioClip>(soundsWhenSomethingAttached);
			}
			return soundsWhenAttachedSequence.GetNext();
		}
	}

	public AudioClip SoundWhenSomethingDetached
	{
		get
		{
			if (soundsWhenSomethingDetached.Length == 0)
			{
				return null;
			}
			if (soundsWhenDetachedSequence == null)
			{
				soundsWhenDetachedSequence = new ElementSequence<AudioClip>(soundsWhenSomethingDetached);
			}
			return soundsWhenDetachedSequence.GetNext();
		}
	}

	public AudioClip SoundWhenSomethingRefilled
	{
		get
		{
			if (soundsWhenSomethingRefilled == null)
			{
				return null;
			}
			if (soundsWhenSomethingRefilled.Length == 0)
			{
				return null;
			}
			if (soundsWhenRefilledSequence == null)
			{
				soundsWhenRefilledSequence = new ElementSequence<AudioClip>(soundsWhenSomethingRefilled);
			}
			return soundsWhenRefilledSequence.GetNext();
		}
	}
}
