using System;
using UnityEngine;

public class QuickRigidbodyDebug : MonoBehaviour
{
	[SerializeField]
	private RigidbodyEnterExitTriggerEvents trigger;

	[SerializeField]
	private RigidbodyEnterExitCollisionEvents collision;

	private void OnEnable()
	{
		if ((bool)trigger)
		{
			RigidbodyEnterExitTriggerEvents rigidbodyEnterExitTriggerEvents = trigger;
			rigidbodyEnterExitTriggerEvents.OnRigidbodyEnterTrigger = (Action<Rigidbody>)Delegate.Combine(rigidbodyEnterExitTriggerEvents.OnRigidbodyEnterTrigger, new Action<Rigidbody>(CubicalTriggerEnter));
		}
		if ((bool)collision)
		{
			RigidbodyEnterExitCollisionEvents rigidbodyEnterExitCollisionEvents = collision;
			rigidbodyEnterExitCollisionEvents.OnRigidbodyEnterCollision = (Action<RigidbodyCollisionInfo>)Delegate.Combine(rigidbodyEnterExitCollisionEvents.OnRigidbodyEnterCollision, new Action<RigidbodyCollisionInfo>(BotCollisionEnter));
		}
	}

	private void OnDisable()
	{
		if ((bool)trigger)
		{
			RigidbodyEnterExitTriggerEvents rigidbodyEnterExitTriggerEvents = trigger;
			rigidbodyEnterExitTriggerEvents.OnRigidbodyEnterTrigger = (Action<Rigidbody>)Delegate.Remove(rigidbodyEnterExitTriggerEvents.OnRigidbodyEnterTrigger, new Action<Rigidbody>(CubicalTriggerEnter));
		}
		if ((bool)collision)
		{
			RigidbodyEnterExitCollisionEvents rigidbodyEnterExitCollisionEvents = collision;
			rigidbodyEnterExitCollisionEvents.OnRigidbodyEnterCollision = (Action<RigidbodyCollisionInfo>)Delegate.Remove(rigidbodyEnterExitCollisionEvents.OnRigidbodyEnterCollision, new Action<RigidbodyCollisionInfo>(BotCollisionEnter));
		}
	}

	private void CubicalTriggerEnter(Rigidbody rb)
	{
		Debug.Log("HIT TRIGGER");
	}

	private void BotCollisionEnter(RigidbodyCollisionInfo rb)
	{
		Debug.Log("HIT COLLISION");
	}
}
