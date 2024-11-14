using System;
using System.Collections;
using OwlchemyVR;
using UnityEngine;

public class AudioBallController : MonoBehaviour
{
	[SerializeField]
	private AudioClip primaryClip;

	[SerializeField]
	private AudioClip secondaryClip;

	[SerializeField]
	private bool keepPosition;

	[SerializeField]
	private bool useSecondaryClip;

	private AudioSourceHelper sourceHelper;

	private PickupableItem pickupable;

	private Rigidbody m_rigidbody;

	private Coroutine releaseRoutine;

	private Coroutine playSecondaryClipRoutine;

	private Vector3 initalPosition;

	private void OnEnable()
	{
		sourceHelper = GetComponent<AudioSourceHelper>();
		pickupable = GetComponent<PickupableItem>();
		m_rigidbody = GetComponent<Rigidbody>();
		initalPosition = base.transform.position;
		PickupableItem pickupableItem = pickupable;
		pickupableItem.OnGrabbed = (Action<GrabbableItem>)Delegate.Combine(pickupableItem.OnGrabbed, new Action<GrabbableItem>(Grabbed));
		PickupableItem pickupableItem2 = pickupable;
		pickupableItem2.OnReleasedWasNotSwappedBetweenHands = (Action<GrabbableItem>)Delegate.Combine(pickupableItem2.OnReleasedWasNotSwappedBetweenHands, new Action<GrabbableItem>(Release));
	}

	private void OnDisable()
	{
		PickupableItem pickupableItem = pickupable;
		pickupableItem.OnGrabbed = (Action<GrabbableItem>)Delegate.Remove(pickupableItem.OnGrabbed, new Action<GrabbableItem>(Grabbed));
		PickupableItem pickupableItem2 = pickupable;
		pickupableItem2.OnReleasedWasNotSwappedBetweenHands = (Action<GrabbableItem>)Delegate.Remove(pickupableItem2.OnReleasedWasNotSwappedBetweenHands, new Action<GrabbableItem>(Release));
	}

	private void Grabbed(GrabbableItem item)
	{
		if (releaseRoutine != null)
		{
			StopCoroutine(releaseRoutine);
		}
		if ((bool)sourceHelper)
		{
			if (useSecondaryClip)
			{
				if (playSecondaryClipRoutine != null)
				{
					StopCoroutine(playSecondaryClipRoutine);
				}
				playSecondaryClipRoutine = StartCoroutine(PlaySecondaryClipRoutine());
			}
			else
			{
				AudioManager.Instance.PlayWithAudioSrcHelper(sourceHelper, primaryClip, 1f, 1f, true);
			}
		}
		else if (keepPosition)
		{
			AudioManager.Instance.Play(base.transform.position, primaryClip, 1f, 1f);
		}
		else
		{
			sourceHelper = AudioManager.Instance.PlayLooping(base.transform, primaryClip, 1f, 1f);
		}
	}

	private IEnumerator PlaySecondaryClipRoutine()
	{
		sourceHelper.SetClip(secondaryClip);
		sourceHelper.Play();
		yield return new WaitForSeconds(secondaryClip.length);
		sourceHelper.SetClip(primaryClip);
		sourceHelper.Play();
		sourceHelper.SetLooping(true);
	}

	private void Release(GrabbableItem item)
	{
		if (releaseRoutine != null)
		{
			StopCoroutine(releaseRoutine);
		}
		releaseRoutine = StartCoroutine(ReleaseRoutine());
	}

	private IEnumerator ReleaseRoutine()
	{
		yield return new WaitForSeconds(3f);
		base.transform.position = initalPosition;
		m_rigidbody.isKinematic = true;
		if ((bool)sourceHelper)
		{
			sourceHelper.Stop();
		}
		sourceHelper = null;
		sourceHelper = GetComponent<AudioSourceHelper>();
	}
}
