using System;
using OwlchemyVR;
using UnityEngine;

public class WineBottleSmashDetector : MonoBehaviour
{
	[SerializeField]
	private WorldItemData wineBottleSmashData;

	[SerializeField]
	private RigidbodyEnterExitTriggerEvents triggerEvents;

	private void OnEnable()
	{
		RigidbodyEnterExitTriggerEvents rigidbodyEnterExitTriggerEvents = triggerEvents;
		rigidbodyEnterExitTriggerEvents.OnRigidbodyEnterTrigger = (Action<Rigidbody>)Delegate.Combine(rigidbodyEnterExitTriggerEvents.OnRigidbodyEnterTrigger, new Action<Rigidbody>(TriggerEntered));
		RigidbodyEnterExitTriggerEvents rigidbodyEnterExitTriggerEvents2 = triggerEvents;
		rigidbodyEnterExitTriggerEvents2.OnRigidbodyExitTrigger = (Action<Rigidbody>)Delegate.Combine(rigidbodyEnterExitTriggerEvents2.OnRigidbodyExitTrigger, new Action<Rigidbody>(TriggerExited));
	}

	private void OnDisable()
	{
		RigidbodyEnterExitTriggerEvents rigidbodyEnterExitTriggerEvents = triggerEvents;
		rigidbodyEnterExitTriggerEvents.OnRigidbodyEnterTrigger = (Action<Rigidbody>)Delegate.Remove(rigidbodyEnterExitTriggerEvents.OnRigidbodyEnterTrigger, new Action<Rigidbody>(TriggerEntered));
		RigidbodyEnterExitTriggerEvents rigidbodyEnterExitTriggerEvents2 = triggerEvents;
		rigidbodyEnterExitTriggerEvents2.OnRigidbodyExitTrigger = (Action<Rigidbody>)Delegate.Remove(rigidbodyEnterExitTriggerEvents2.OnRigidbodyExitTrigger, new Action<Rigidbody>(TriggerExited));
	}

	private void TriggerEntered(Rigidbody rb)
	{
		ShatterOnCollision component = rb.GetComponent<ShatterOnCollision>();
		if (component != null)
		{
			component.OnShatter = (Action<ShatterOnCollision>)Delegate.Combine(component.OnShatter, new Action<ShatterOnCollision>(SomethingWasShattered));
		}
	}

	private void TriggerExited(Rigidbody rb)
	{
		ShatterOnCollision component = rb.GetComponent<ShatterOnCollision>();
		if (component != null)
		{
			component.OnShatter = (Action<ShatterOnCollision>)Delegate.Remove(component.OnShatter, new Action<ShatterOnCollision>(SomethingWasShattered));
		}
	}

	private void SomethingWasShattered(ShatterOnCollision shatterOnCollision)
	{
		shatterOnCollision.OnShatter = (Action<ShatterOnCollision>)Delegate.Remove(shatterOnCollision.OnShatter, new Action<ShatterOnCollision>(SomethingWasShattered));
		GameEventsManager.Instance.ItemActionOccurred(wineBottleSmashData, "ACTIVATED");
	}
}
