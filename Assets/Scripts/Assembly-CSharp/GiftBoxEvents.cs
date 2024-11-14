using System;
using OwlchemyVR;
using UnityEngine;

public class GiftBoxEvents : MonoBehaviour
{
	[SerializeField]
	private RigidbodyEnterExitTriggerEvents trigger;

	[SerializeField]
	private WorldItem myWorldItem;

	[SerializeField]
	private WorldItemData watchingWorldItemData;

	private void OnEnable()
	{
		RigidbodyEnterExitTriggerEvents rigidbodyEnterExitTriggerEvents = trigger;
		rigidbodyEnterExitTriggerEvents.OnRigidbodyExitTrigger = (Action<Rigidbody>)Delegate.Combine(rigidbodyEnterExitTriggerEvents.OnRigidbodyExitTrigger, new Action<Rigidbody>(ItemRemoved));
	}

	private void OnDisable()
	{
		RigidbodyEnterExitTriggerEvents rigidbodyEnterExitTriggerEvents = trigger;
		rigidbodyEnterExitTriggerEvents.OnRigidbodyExitTrigger = (Action<Rigidbody>)Delegate.Remove(rigidbodyEnterExitTriggerEvents.OnRigidbodyExitTrigger, new Action<Rigidbody>(ItemRemoved));
	}

	private void ItemRemoved(Rigidbody rb)
	{
		WorldItem component = rb.GetComponent<WorldItem>();
		if (component != null && component.Data == watchingWorldItemData)
		{
			GameEventsManager.Instance.ItemAppliedToItemActionOccurred(component.Data, myWorldItem.Data, "REMOVED_FROM");
			rb.gameObject.transform.SetParent(GlobalStorage.Instance.ContentRoot, true);
		}
	}
}
