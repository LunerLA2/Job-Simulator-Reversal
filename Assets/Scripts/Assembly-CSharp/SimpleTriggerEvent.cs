using System;
using UnityEngine;
using UnityEngine.Events;

public class SimpleTriggerEvent : MonoBehaviour
{
	[SerializeField]
	private RigidbodyEnterExitTriggerEvents triggerEvent;

	[SerializeField]
	private UnityEvent Enter;

	[SerializeField]
	private UnityEvent Exit;

	[SerializeField]
	private AudioClip enterClip;

	[SerializeField]
	private AudioClip exitClip;

	[SerializeField]
	private Transform optionalAudioPosition;

	private void OnEnable()
	{
		RigidbodyEnterExitTriggerEvents rigidbodyEnterExitTriggerEvents = triggerEvent;
		rigidbodyEnterExitTriggerEvents.OnRigidbodyEnterTrigger = (Action<Rigidbody>)Delegate.Combine(rigidbodyEnterExitTriggerEvents.OnRigidbodyEnterTrigger, new Action<Rigidbody>(RigidbodyEnter));
		RigidbodyEnterExitTriggerEvents rigidbodyEnterExitTriggerEvents2 = triggerEvent;
		rigidbodyEnterExitTriggerEvents2.OnRigidbodyExitTrigger = (Action<Rigidbody>)Delegate.Combine(rigidbodyEnterExitTriggerEvents2.OnRigidbodyExitTrigger, new Action<Rigidbody>(RigidbodyExit));
	}

	private void OnDisable()
	{
		RigidbodyEnterExitTriggerEvents rigidbodyEnterExitTriggerEvents = triggerEvent;
		rigidbodyEnterExitTriggerEvents.OnRigidbodyEnterTrigger = (Action<Rigidbody>)Delegate.Remove(rigidbodyEnterExitTriggerEvents.OnRigidbodyEnterTrigger, new Action<Rigidbody>(RigidbodyEnter));
		RigidbodyEnterExitTriggerEvents rigidbodyEnterExitTriggerEvents2 = triggerEvent;
		rigidbodyEnterExitTriggerEvents2.OnRigidbodyExitTrigger = (Action<Rigidbody>)Delegate.Remove(rigidbodyEnterExitTriggerEvents2.OnRigidbodyExitTrigger, new Action<Rigidbody>(RigidbodyExit));
	}

	private void RigidbodyEnter(Rigidbody rb)
	{
		if (Enter != null)
		{
			Enter.Invoke();
		}
		if ((bool)enterClip && optionalAudioPosition == null)
		{
			AudioManager.Instance.Play(base.transform.position, enterClip, 1f, 1f);
		}
		if ((bool)enterClip && (bool)optionalAudioPosition)
		{
			AudioManager.Instance.Play(optionalAudioPosition.position, enterClip, 1f, 1f);
		}
	}

	private void RigidbodyExit(Rigidbody rb)
	{
		if (Exit != null)
		{
			Exit.Invoke();
		}
		if ((bool)exitClip && optionalAudioPosition == null)
		{
			AudioManager.Instance.Play(base.transform.position, exitClip, 1f, 1f);
		}
		if ((bool)exitClip && (bool)optionalAudioPosition)
		{
			AudioManager.Instance.Play(optionalAudioPosition.position, exitClip, 1f, 1f);
		}
	}
}
