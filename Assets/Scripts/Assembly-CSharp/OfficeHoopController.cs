using System;
using OwlchemyVR;
using UnityEngine;

public class OfficeHoopController : MonoBehaviour
{
	[SerializeField]
	private RigidbodyEnterExitTriggerEvents hoopZone;

	[SerializeField]
	private ParticleSystem particleToPlay;

	[SerializeField]
	private WorldItemData worldItemData;

	private void OnEnable()
	{
		RigidbodyEnterExitTriggerEvents rigidbodyEnterExitTriggerEvents = hoopZone;
		rigidbodyEnterExitTriggerEvents.OnRigidbodyEnterTrigger = (Action<Rigidbody>)Delegate.Combine(rigidbodyEnterExitTriggerEvents.OnRigidbodyEnterTrigger, new Action<Rigidbody>(PlayParticle));
	}

	private void OnDisable()
	{
		RigidbodyEnterExitTriggerEvents rigidbodyEnterExitTriggerEvents = hoopZone;
		rigidbodyEnterExitTriggerEvents.OnRigidbodyEnterTrigger = (Action<Rigidbody>)Delegate.Remove(rigidbodyEnterExitTriggerEvents.OnRigidbodyEnterTrigger, new Action<Rigidbody>(PlayParticle));
	}

	private void PlayParticle(Rigidbody r)
	{
		if (worldItemData != null)
		{
			GameEventsManager.Instance.ItemActionOccurred(worldItemData, "ACTIVATED");
		}
		particleToPlay.Play();
	}
}
