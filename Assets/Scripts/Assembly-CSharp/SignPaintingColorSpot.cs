using System;
using UnityEngine;

public class SignPaintingColorSpot : MonoBehaviour
{
	[SerializeField]
	private Color color;

	[SerializeField]
	private RigidbodyEnterExitTriggerEvents triggerEvents;

	private void OnEnable()
	{
		RigidbodyEnterExitTriggerEvents rigidbodyEnterExitTriggerEvents = triggerEvents;
		rigidbodyEnterExitTriggerEvents.OnRigidbodyEnterTrigger = (Action<Rigidbody>)Delegate.Combine(rigidbodyEnterExitTriggerEvents.OnRigidbodyEnterTrigger, new Action<Rigidbody>(SomethingEnteredTrigger));
	}

	private void OnDisable()
	{
		RigidbodyEnterExitTriggerEvents rigidbodyEnterExitTriggerEvents = triggerEvents;
		rigidbodyEnterExitTriggerEvents.OnRigidbodyEnterTrigger = (Action<Rigidbody>)Delegate.Remove(rigidbodyEnterExitTriggerEvents.OnRigidbodyEnterTrigger, new Action<Rigidbody>(SomethingEnteredTrigger));
	}

	private void SomethingEnteredTrigger(Rigidbody rb)
	{
		SignPaintingBrushController component = rb.GetComponent<SignPaintingBrushController>();
		if (component != null)
		{
			component.ChangeBrushColor(color);
		}
	}
}
