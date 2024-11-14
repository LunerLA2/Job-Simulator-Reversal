using System;
using UnityEngine;

public class BlowSoundController : MonoBehaviour
{
	[SerializeField]
	private BlowableItem _blowableItem;

	[SerializeField]
	private AudioClip[] blowClips;

	private float timeToNextAudio;

	private void OnEnable()
	{
		BlowableItem blowableItem = _blowableItem;
		blowableItem.OnWasBlown = (Action<BlowableItem, float, HeadController>)Delegate.Combine(blowableItem.OnWasBlown, new Action<BlowableItem, float, HeadController>(OnWasBlown));
	}

	private void OnDisable()
	{
		BlowableItem blowableItem = _blowableItem;
		blowableItem.OnWasBlown = (Action<BlowableItem, float, HeadController>)Delegate.Remove(blowableItem.OnWasBlown, new Action<BlowableItem, float, HeadController>(OnWasBlown));
	}

	private void Update()
	{
		if (timeToNextAudio > 0f)
		{
			timeToNextAudio -= Time.deltaTime;
		}
	}

	private void OnWasBlown(BlowableItem blowableItem, float amount, HeadController headController)
	{
		if (timeToNextAudio <= 0f)
		{
			AudioClip audioClip = blowClips[UnityEngine.Random.Range(0, blowClips.Length)];
			AudioManager.Instance.Play(base.transform.position, audioClip, 1f, 1f);
			timeToNextAudio = audioClip.length;
		}
	}
}
