using System;
using OwlchemyVR;
using UnityEngine;

public class CubicleTossZone : MonoBehaviour
{
	[SerializeField]
	private RigidbodyEnterExitTriggerEvents triggerEvents;

	[SerializeField]
	private AudioClip[] clipsToPlay;

	[SerializeField]
	private ParticleSystem[] particlesToPlay;

	private float startTime;

	private void OnEnable()
	{
		RigidbodyEnterExitTriggerEvents rigidbodyEnterExitTriggerEvents = triggerEvents;
		rigidbodyEnterExitTriggerEvents.OnRigidbodyEnterTrigger = (Action<Rigidbody>)Delegate.Combine(rigidbodyEnterExitTriggerEvents.OnRigidbodyEnterTrigger, new Action<Rigidbody>(Trigger));
	}

	private void OnDisable()
	{
		RigidbodyEnterExitTriggerEvents rigidbodyEnterExitTriggerEvents = triggerEvents;
		rigidbodyEnterExitTriggerEvents.OnRigidbodyEnterTrigger = (Action<Rigidbody>)Delegate.Remove(rigidbodyEnterExitTriggerEvents.OnRigidbodyEnterTrigger, new Action<Rigidbody>(Trigger));
	}

	private void Awake()
	{
		startTime = Time.realtimeSinceStartup;
	}

	private void Trigger(Rigidbody r)
	{
		if (!(Time.realtimeSinceStartup - startTime < 1f) && r.GetComponent<PickupableItem>() != null)
		{
			for (int i = 0; i < clipsToPlay.Length; i++)
			{
				AudioManager.Instance.Play(base.transform.position, clipsToPlay[i], 0.6f, 1f);
			}
			for (int j = 0; j < particlesToPlay.Length; j++)
			{
				particlesToPlay[j].Play();
			}
		}
	}
}
