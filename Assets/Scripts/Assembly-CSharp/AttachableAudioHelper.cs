using System;
using UnityEngine;

public class AttachableAudioHelper : MonoBehaviour
{
	[SerializeField]
	[Tooltip("If empty, will try to find an attachable object on this game object.")]
	[Header("Observes (optional, will search this object)")]
	private AttachableObject attachableObjectToObserve;

	[SerializeField]
	[Header("Audio")]
	private AudioClip[] playOnAttached;

	[SerializeField]
	private AudioClip[] playOnDetached;

	private ElementSequence<AudioClip> playOnAttachedSequence;

	private ElementSequence<AudioClip> playonDetachedSequence;

	private void Awake()
	{
		playOnAttachedSequence = new ElementSequence<AudioClip>(playOnAttached);
		playonDetachedSequence = new ElementSequence<AudioClip>(playOnDetached);
		if (attachableObjectToObserve == null)
		{
			attachableObjectToObserve = GetComponent<AttachableObject>();
			if (attachableObjectToObserve == null)
			{
				Debug.LogWarning("AttachableAudioHelper is missing an AttachableObject in its GameObject, or by inspector reference. " + this);
				return;
			}
		}
		if (playOnAttached.Length == 0 && playOnDetached.Length == 0)
		{
			Debug.LogWarning("AttachableAudioHelper is missing audio clips for both attached and detached events. Maybe remove the component if uneccesary. " + this);
			return;
		}
		AttachableObject attachableObject = attachableObjectToObserve;
		attachableObject.OnAttach = (Action<AttachableObject, AttachablePoint>)Delegate.Combine(attachableObject.OnAttach, new Action<AttachableObject, AttachablePoint>(OnAttach));
		AttachableObject attachableObject2 = attachableObjectToObserve;
		attachableObject2.OnDetach = (Action<AttachableObject, AttachablePoint>)Delegate.Combine(attachableObject2.OnDetach, new Action<AttachableObject, AttachablePoint>(OnDetach));
	}

	private void OnDestroy()
	{
		AttachableObject attachableObject = attachableObjectToObserve;
		attachableObject.OnAttach = (Action<AttachableObject, AttachablePoint>)Delegate.Remove(attachableObject.OnAttach, new Action<AttachableObject, AttachablePoint>(OnAttach));
		AttachableObject attachableObject2 = attachableObjectToObserve;
		attachableObject2.OnDetach = (Action<AttachableObject, AttachablePoint>)Delegate.Remove(attachableObject2.OnDetach, new Action<AttachableObject, AttachablePoint>(OnDetach));
	}

	private void OnAttach(AttachableObject attachable, AttachablePoint point)
	{
		if (playOnAttached.Length > 0)
		{
			AudioManager.Instance.Play(point.GetPoint(), playOnAttachedSequence.GetNext(), 1f, 1f);
		}
	}

	private void OnDetach(AttachableObject attachable, AttachablePoint point)
	{
		if (playOnDetached.Length > 0)
		{
			AudioManager.Instance.Play(point.GetPoint(), playonDetachedSequence.GetNext(), 1f, 1f);
		}
	}
}
