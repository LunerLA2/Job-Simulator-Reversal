using System;
using OwlchemyVR;
using UnityEngine;

public class ActivateBasedOnRigidbodyTrigger : MonoBehaviour
{
	[SerializeField]
	private RigidbodyEnterExitTriggerEvents triggerEvents;

	[SerializeField]
	private WorldItemData itemForEvents;

	[SerializeField]
	private WorldItemData[] onlyInteractWithTheseItems;

	[SerializeField]
	private bool activateOnEnter = true;

	[SerializeField]
	private bool deactivateOnExit;

	private void OnEnable()
	{
		RigidbodyEnterExitTriggerEvents rigidbodyEnterExitTriggerEvents = triggerEvents;
		rigidbodyEnterExitTriggerEvents.OnRigidbodyEnterTrigger = (Action<Rigidbody>)Delegate.Combine(rigidbodyEnterExitTriggerEvents.OnRigidbodyEnterTrigger, new Action<Rigidbody>(Entered));
		RigidbodyEnterExitTriggerEvents rigidbodyEnterExitTriggerEvents2 = triggerEvents;
		rigidbodyEnterExitTriggerEvents2.OnRigidbodyExitTrigger = (Action<Rigidbody>)Delegate.Combine(rigidbodyEnterExitTriggerEvents2.OnRigidbodyExitTrigger, new Action<Rigidbody>(Exited));
	}

	private void OnDisable()
	{
		RigidbodyEnterExitTriggerEvents rigidbodyEnterExitTriggerEvents = triggerEvents;
		rigidbodyEnterExitTriggerEvents.OnRigidbodyEnterTrigger = (Action<Rigidbody>)Delegate.Remove(rigidbodyEnterExitTriggerEvents.OnRigidbodyEnterTrigger, new Action<Rigidbody>(Entered));
		RigidbodyEnterExitTriggerEvents rigidbodyEnterExitTriggerEvents2 = triggerEvents;
		rigidbodyEnterExitTriggerEvents2.OnRigidbodyExitTrigger = (Action<Rigidbody>)Delegate.Remove(rigidbodyEnterExitTriggerEvents2.OnRigidbodyExitTrigger, new Action<Rigidbody>(Exited));
	}

	private void Entered(Rigidbody r)
	{
		if (activateOnEnter && RigidbodyIsValid(r))
		{
			GameEventsManager.Instance.ItemActionOccurred(itemForEvents, "ACTIVATED");
		}
	}

	private void Exited(Rigidbody r)
	{
		if (deactivateOnExit && RigidbodyIsValid(r))
		{
			GameEventsManager.Instance.ItemActionOccurred(itemForEvents, "DEACTIVATED");
		}
	}

	private bool RigidbodyIsValid(Rigidbody r)
	{
		if (onlyInteractWithTheseItems == null)
		{
			return true;
		}
		WorldItem component = r.GetComponent<WorldItem>();
		if (component != null)
		{
			return Array.IndexOf(onlyInteractWithTheseItems, component.Data) >= 0;
		}
		return false;
	}
}
