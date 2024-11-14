using System;
using System.Collections;
using OwlchemyVR;
using UnityEngine;

public class CounterMessController : MonoBehaviour
{
	[SerializeField]
	private RigidbodyEnterExitTriggerEvents triggerEvents;

	[SerializeField]
	private WorldItem worldItem;

	[SerializeField]
	private WorldItemData[] itemsToCount;

	private int initialNumberOfObjects;

	private void Start()
	{
		initialNumberOfObjects = 0;
		StartCoroutine(WaitAndCountInitialState());
	}

	private IEnumerator WaitAndCountInitialState()
	{
		yield return null;
		yield return null;
		yield return null;
		for (int i = 0; i < triggerEvents.ActiveRigidbodiesTriggerInfo.Count; i++)
		{
			RigidbodyTriggerInfo info = triggerEvents.ActiveRigidbodiesTriggerInfo[i];
			if (info != null && info.Rigidbody != null)
			{
				WorldItem wi = info.Rigidbody.GetComponent<WorldItem>();
				if (wi != null && Array.IndexOf(itemsToCount, wi.Data) > -1)
				{
					initialNumberOfObjects++;
				}
			}
		}
	}

	private void OnEnable()
	{
		RigidbodyEnterExitTriggerEvents rigidbodyEnterExitTriggerEvents = triggerEvents;
		rigidbodyEnterExitTriggerEvents.OnRigidbodyEnterTrigger = (Action<Rigidbody>)Delegate.Combine(rigidbodyEnterExitTriggerEvents.OnRigidbodyEnterTrigger, new Action<Rigidbody>(RigidbodyEntered));
		RigidbodyEnterExitTriggerEvents rigidbodyEnterExitTriggerEvents2 = triggerEvents;
		rigidbodyEnterExitTriggerEvents2.OnRigidbodyExitTrigger = (Action<Rigidbody>)Delegate.Combine(rigidbodyEnterExitTriggerEvents2.OnRigidbodyExitTrigger, new Action<Rigidbody>(RigidbodyRemoved));
	}

	private void OnDisable()
	{
		RigidbodyEnterExitTriggerEvents rigidbodyEnterExitTriggerEvents = triggerEvents;
		rigidbodyEnterExitTriggerEvents.OnRigidbodyEnterTrigger = (Action<Rigidbody>)Delegate.Remove(rigidbodyEnterExitTriggerEvents.OnRigidbodyEnterTrigger, new Action<Rigidbody>(RigidbodyEntered));
		RigidbodyEnterExitTriggerEvents rigidbodyEnterExitTriggerEvents2 = triggerEvents;
		rigidbodyEnterExitTriggerEvents2.OnRigidbodyExitTrigger = (Action<Rigidbody>)Delegate.Remove(rigidbodyEnterExitTriggerEvents2.OnRigidbodyExitTrigger, new Action<Rigidbody>(RigidbodyRemoved));
	}

	private void RigidbodyEntered(Rigidbody r)
	{
		WorldItem component = r.GetComponent<WorldItem>();
		if (component == null)
		{
			return;
		}
		bool flag = false;
		for (int i = 0; i < itemsToCount.Length; i++)
		{
			if (itemsToCount[i] == component.Data)
			{
				flag = true;
				break;
			}
		}
		if (flag)
		{
			GameEventsManager.Instance.ItemAppliedToItemActionOccurred(component.Data, worldItem.Data, "ADDED_TO");
			GameEventsManager.Instance.ItemActionOccurred(worldItem.Data, "DEACTIVATED");
		}
	}

	private void RigidbodyRemoved(Rigidbody r)
	{
		WorldItem component = r.GetComponent<WorldItem>();
		if (component == null)
		{
			return;
		}
		bool flag = false;
		for (int i = 0; i < itemsToCount.Length; i++)
		{
			if (itemsToCount[i] == component.Data)
			{
				flag = true;
				break;
			}
		}
		if (flag)
		{
			GameEventsManager.Instance.ItemAppliedToItemActionOccurred(component.Data, worldItem.Data, "REMOVED_FROM");
			GameEventsManager.Instance.ItemActionOccurred(worldItem.Data, "ACTIVATED");
		}
	}
}
