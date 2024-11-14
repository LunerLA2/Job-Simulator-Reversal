using System;
using OwlchemyVR;
using UnityEngine;

public class VisibleObjectDetector : MonoBehaviour
{
	[SerializeField]
	private WorldItem worldItemOfObserver;

	[SerializeField]
	private RigidbodyEnterExitTriggerEvents triggerEvents;

	private void OnEnable()
	{
		RigidbodyEnterExitTriggerEvents rigidbodyEnterExitTriggerEvents = triggerEvents;
		rigidbodyEnterExitTriggerEvents.OnRigidbodyEnterTrigger = (Action<Rigidbody>)Delegate.Combine(rigidbodyEnterExitTriggerEvents.OnRigidbodyEnterTrigger, new Action<Rigidbody>(SomethingEntered));
	}

	private void OnDisable()
	{
		RigidbodyEnterExitTriggerEvents rigidbodyEnterExitTriggerEvents = triggerEvents;
		rigidbodyEnterExitTriggerEvents.OnRigidbodyEnterTrigger = (Action<Rigidbody>)Delegate.Remove(rigidbodyEnterExitTriggerEvents.OnRigidbodyEnterTrigger, new Action<Rigidbody>(SomethingEntered));
	}

	private void SomethingEntered(Rigidbody r)
	{
		WorldItem component = r.gameObject.GetComponent<WorldItem>();
		if (component != null)
		{
			GameEventsManager.Instance.ItemAppliedToItemActionOccurred(component.Data, worldItemOfObserver.Data, "SEEN_BY");
		}
	}
}
