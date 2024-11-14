using System;
using System.Collections;
using UnityEngine;

public class AutoDoorController : MonoBehaviour
{
	private const string doorOpenAnim = "OpenDoor";

	private const string doorCloseAnim = "CloseDoor";

	[SerializeField]
	private Animator doorAnimator;

	[SerializeField]
	private AudioClip openClip;

	[SerializeField]
	private AudioClip closeClip;

	[SerializeField]
	private RigidbodyEnterExitTriggerEvents rigidbodyTriggerEvents;

	[Space]
	[SerializeField]
	private AudioLowPassFilter ambientLowPass1;

	[SerializeField]
	private AudioLowPassFilter ambientLowPass2;

	private Coroutine ambientCloseCoroutine;

	private int lowCutoffFreq = 5000;

	private bool isDoorOpen;

	private float closeDelay = 5f;

	private float timeTillClose = 5f;

	private void OnEnable()
	{
		RigidbodyEnterExitTriggerEvents rigidbodyEnterExitTriggerEvents = rigidbodyTriggerEvents;
		rigidbodyEnterExitTriggerEvents.OnRigidbodyEnterTrigger = (Action<Rigidbody>)Delegate.Combine(rigidbodyEnterExitTriggerEvents.OnRigidbodyEnterTrigger, new Action<Rigidbody>(OnRigidbodyTriggerEnter));
	}

	private void OnDisable()
	{
		RigidbodyEnterExitTriggerEvents rigidbodyEnterExitTriggerEvents = rigidbodyTriggerEvents;
		rigidbodyEnterExitTriggerEvents.OnRigidbodyEnterTrigger = (Action<Rigidbody>)Delegate.Remove(rigidbodyEnterExitTriggerEvents.OnRigidbodyEnterTrigger, new Action<Rigidbody>(OnRigidbodyTriggerEnter));
	}

	private void OnRigidbodyTriggerEnter(Rigidbody rb)
	{
		timeTillClose = closeDelay;
		if (!isDoorOpen)
		{
			StartCoroutine(OpenDoorAsync());
		}
	}

	private IEnumerator OpenDoorAsync()
	{
		isDoorOpen = true;
		doorAnimator.SetTrigger("DoorOpen");
		OpenDoorAmbientSound();
		if (openClip != null)
		{
			AudioManager.Instance.Play(base.transform.position, openClip, 1f, 1f);
		}
		float timer = 0f;
		while (timer <= timeTillClose)
		{
			timeTillClose -= Time.deltaTime;
			yield return null;
		}
		doorAnimator.SetTrigger("DoorClose");
		CloseDoorAmbientSound();
		if (closeClip != null)
		{
			AudioManager.Instance.Play(base.transform.position, closeClip, 1f, 1f);
		}
		isDoorOpen = false;
	}

	private void OpenDoorAmbientSound()
	{
		if (ambientCloseCoroutine != null)
		{
			StopCoroutine(ambientCloseCoroutine);
		}
		if (ambientLowPass1 != null)
		{
			ambientLowPass1.cutoffFrequency = 22000f;
		}
		if (ambientLowPass2 != null)
		{
			ambientLowPass2.cutoffFrequency = 22000f;
		}
	}

	private void CloseDoorAmbientSound()
	{
		ambientCloseCoroutine = StartCoroutine(CloseDoorAmbientSoundASync());
	}

	private IEnumerator CloseDoorAmbientSoundASync()
	{
		yield return new WaitForSeconds(0.9f);
		if (ambientLowPass1 != null)
		{
			ambientLowPass1.cutoffFrequency = lowCutoffFreq;
		}
		if (ambientLowPass2 != null)
		{
			ambientLowPass2.cutoffFrequency = lowCutoffFreq;
		}
		ambientCloseCoroutine = null;
	}
}
