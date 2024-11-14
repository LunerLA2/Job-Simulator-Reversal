using System;
using OwlchemyVR;
using UnityEngine;

public class SprinklerController : MonoBehaviour
{
	[SerializeField]
	private RigidbodyEnterExitTriggerEvents triggerEvents;

	[SerializeField]
	private WorldItem myWorldItem;

	[SerializeField]
	private AudioClip[] soundOnHit;

	[SerializeField]
	private ElementSequence<AudioClip> soundOnHitSequence;

	[SerializeField]
	private Animation animationOnHit;

	[SerializeField]
	private ParticleSystem[] pfxOnHit;

	private void Awake()
	{
		soundOnHitSequence = new ElementSequence<AudioClip>(soundOnHit);
	}

	private void OnEnable()
	{
		RigidbodyEnterExitTriggerEvents rigidbodyEnterExitTriggerEvents = triggerEvents;
		rigidbodyEnterExitTriggerEvents.OnRigidbodyEnterTrigger = (Action<Rigidbody>)Delegate.Combine(rigidbodyEnterExitTriggerEvents.OnRigidbodyEnterTrigger, new Action<Rigidbody>(ItemEntered));
	}

	private void OnDisable()
	{
		RigidbodyEnterExitTriggerEvents rigidbodyEnterExitTriggerEvents = triggerEvents;
		rigidbodyEnterExitTriggerEvents.OnRigidbodyEnterTrigger = (Action<Rigidbody>)Delegate.Remove(rigidbodyEnterExitTriggerEvents.OnRigidbodyEnterTrigger, new Action<Rigidbody>(ItemEntered));
	}

	private void ItemEntered(Rigidbody rb)
	{
		if (!animationOnHit.isPlaying)
		{
			GameEventsManager.Instance.ItemActionOccurred(myWorldItem.Data, "ACTIVATED");
			AudioManager.Instance.Play(base.transform, soundOnHitSequence.GetNext(), 1f, 1f);
			animationOnHit.Play();
			for (int i = 0; i < pfxOnHit.Length; i++)
			{
				pfxOnHit[i].Play();
			}
		}
	}
}
