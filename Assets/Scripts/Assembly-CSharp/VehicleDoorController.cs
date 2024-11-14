using System.Collections;
using UnityEngine;

public class VehicleDoorController : MonoBehaviour
{
	[SerializeField]
	private GrabbableHinge hinge;

	[SerializeField]
	private AudioSourceHelper audioSourceHelper;

	[SerializeField]
	private AudioClip[] doorOpenClip;

	private ElementSequence<AudioClip> doorOpenClipSequence;

	[SerializeField]
	private AudioClip[] doorCloseClip;

	private ElementSequence<AudioClip> doorCloseClipSequence;

	private void Awake()
	{
		doorOpenClipSequence = new ElementSequence<AudioClip>(doorOpenClip);
		doorCloseClipSequence = new ElementSequence<AudioClip>(doorCloseClip);
	}

	private void Start()
	{
		audioSourceHelper.enabled = false;
	}

	private void OnEnable()
	{
		hinge.OnLowerLocked += OnDoorClosed;
		hinge.OnLowerUnlocked += OnDoorOpened;
	}

	private void OnDisable()
	{
		hinge.OnLowerLocked -= OnDoorClosed;
		hinge.OnLowerUnlocked -= OnDoorOpened;
	}

	private void OnDoorClosed(GrabbableHinge hinge, bool isInitial)
	{
		PlayDoorClosedAudio();
	}

	public void PlayDoorClosedAudio()
	{
		audioSourceHelper.enabled = true;
		audioSourceHelper.SetClip(doorCloseClipSequence.GetNext());
		audioSourceHelper.Play();
		StartCoroutine(DisableAudioSource(audioSourceHelper.GetClip().length));
	}

	private void OnDoorOpened(GrabbableHinge hinge)
	{
		PlayDoorOpenAudio();
	}

	public void PlayDoorOpenAudio()
	{
		audioSourceHelper.enabled = true;
		audioSourceHelper.SetClip(doorOpenClipSequence.GetNext());
		audioSourceHelper.Play();
		StartCoroutine(DisableAudioSource(audioSourceHelper.GetClip().length));
	}

	private IEnumerator DisableAudioSource(float time)
	{
		while (audioSourceHelper.IsPlaying)
		{
			yield return null;
		}
		audioSourceHelper.enabled = false;
	}
}
