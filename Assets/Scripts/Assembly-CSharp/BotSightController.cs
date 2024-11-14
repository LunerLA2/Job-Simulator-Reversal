using System;
using System.Collections.Generic;
using OwlchemyVR;
using UnityEngine;

public class BotSightController : MonoBehaviour
{
	[SerializeField]
	private WorldItem myWorldItem;

	[SerializeField]
	private GameObject sightSystemRoot;

	private Transform cachedSightSystemParent;

	[SerializeField]
	private RigidbodyEnterExitTriggerEvents sightTrigger;

	[SerializeField]
	private RigidbodyEnterExitTriggerEvents instantlyNoticeTrigger;

	[SerializeField]
	private SphereCollider sightAdjustableRadius;

	private List<WorldItemData> itemsOfInterest = new List<WorldItemData>();

	private List<WorldItem> trackedWorldItems = new List<WorldItem>();

	private List<PickupableItem> trackedPickupableItems = new List<PickupableItem>();

	private List<float> timers = new List<float>();

	private float min = -360f;

	private float max = 360f;

	private void Awake()
	{
		if (myWorldItem == null)
		{
			myWorldItem = GetComponent<WorldItem>();
		}
		cachedSightSystemParent = sightSystemRoot.transform.parent;
		SetSystemObjectState(false);
	}

	private void OnEnable()
	{
		RigidbodyEnterExitTriggerEvents rigidbodyEnterExitTriggerEvents = sightTrigger;
		rigidbodyEnterExitTriggerEvents.OnRigidbodyEnterTrigger = (Action<Rigidbody>)Delegate.Combine(rigidbodyEnterExitTriggerEvents.OnRigidbodyEnterTrigger, new Action<Rigidbody>(RigidbodyEnter));
		RigidbodyEnterExitTriggerEvents rigidbodyEnterExitTriggerEvents2 = sightTrigger;
		rigidbodyEnterExitTriggerEvents2.OnRigidbodyExitTrigger = (Action<Rigidbody>)Delegate.Combine(rigidbodyEnterExitTriggerEvents2.OnRigidbodyExitTrigger, new Action<Rigidbody>(RigidbodyExit));
		if (instantlyNoticeTrigger != null)
		{
			RigidbodyEnterExitTriggerEvents rigidbodyEnterExitTriggerEvents3 = instantlyNoticeTrigger;
			rigidbodyEnterExitTriggerEvents3.OnRigidbodyEnterTrigger = (Action<Rigidbody>)Delegate.Combine(rigidbodyEnterExitTriggerEvents3.OnRigidbodyEnterTrigger, new Action<Rigidbody>(InstantlyNoticeTriggerEvent));
		}
	}

	private void OnDisable()
	{
		RigidbodyEnterExitTriggerEvents rigidbodyEnterExitTriggerEvents = sightTrigger;
		rigidbodyEnterExitTriggerEvents.OnRigidbodyEnterTrigger = (Action<Rigidbody>)Delegate.Remove(rigidbodyEnterExitTriggerEvents.OnRigidbodyEnterTrigger, new Action<Rigidbody>(RigidbodyEnter));
		RigidbodyEnterExitTriggerEvents rigidbodyEnterExitTriggerEvents2 = sightTrigger;
		rigidbodyEnterExitTriggerEvents2.OnRigidbodyExitTrigger = (Action<Rigidbody>)Delegate.Combine(rigidbodyEnterExitTriggerEvents2.OnRigidbodyExitTrigger, new Action<Rigidbody>(RigidbodyExit));
		if (instantlyNoticeTrigger != null)
		{
			RigidbodyEnterExitTriggerEvents rigidbodyEnterExitTriggerEvents3 = instantlyNoticeTrigger;
			rigidbodyEnterExitTriggerEvents3.OnRigidbodyEnterTrigger = (Action<Rigidbody>)Delegate.Remove(rigidbodyEnterExitTriggerEvents3.OnRigidbodyEnterTrigger, new Action<Rigidbody>(InstantlyNoticeTriggerEvent));
		}
	}

	public void SetRadiusMultiplier(float mult)
	{
		sightAdjustableRadius.radius = mult;
		sightAdjustableRadius.center = Vector3.forward * (mult - 1f);
	}

	public void AddItemOfInterest(WorldItemData item)
	{
		if (!itemsOfInterest.Contains(item))
		{
			itemsOfInterest.Add(item);
		}
		if (!sightSystemRoot.activeSelf)
		{
			SetSystemObjectState(true);
		}
	}

	public void RemoveItemOfInterest(WorldItemData item)
	{
		if (itemsOfInterest.Contains(item))
		{
			itemsOfInterest.Remove(item);
			for (int i = 0; i < trackedWorldItems.Count; i++)
			{
				if (trackedWorldItems[i].Data == item)
				{
					timers.RemoveAt(i);
					trackedPickupableItems.RemoveAt(i);
					trackedWorldItems.RemoveAt(i);
					i--;
				}
			}
		}
		if (itemsOfInterest.Count == 0 && sightSystemRoot.activeSelf)
		{
			SetSystemObjectState(false);
		}
	}

	public void ClearItemsOfInterest()
	{
		itemsOfInterest.Clear();
		trackedWorldItems.Clear();
		trackedPickupableItems.Clear();
		timers.Clear();
		SetSystemObjectState(false);
	}

	private void SetSystemObjectState(bool state)
	{
		sightSystemRoot.transform.SetParent((!state) ? GlobalStorage.Instance.ContentRoot : cachedSightSystemParent, false);
		sightSystemRoot.transform.localPosition = Vector3.zero;
		sightSystemRoot.transform.localRotation = Quaternion.identity;
		sightSystemRoot.transform.localScale = Vector3.one;
		sightSystemRoot.SetActive(state);
	}

	private void RigidbodyEnter(Rigidbody rb)
	{
		WorldItem component = rb.GetComponent<WorldItem>();
		if (component != null && itemsOfInterest.Contains(component.Data))
		{
			PickupableItem component2 = rb.GetComponent<PickupableItem>();
			if (component2 != null)
			{
				trackedWorldItems.Add(component);
				trackedPickupableItems.Add(component2);
				timers.Add(0f);
			}
		}
	}

	private void RigidbodyExit(Rigidbody rb)
	{
		WorldItem component = rb.GetComponent<WorldItem>();
		if (component != null && trackedWorldItems.Contains(component))
		{
			int index = trackedWorldItems.IndexOf(component);
			timers.RemoveAt(index);
			trackedPickupableItems.RemoveAt(index);
			trackedWorldItems.Remove(component);
		}
	}

	private void InstantlyNoticeTriggerEvent(Rigidbody rb)
	{
		WorldItem component = rb.GetComponent<WorldItem>();
		if (component != null && itemsOfInterest.Contains(component.Data))
		{
			GameEventsManager.Instance.ItemAppliedToItemActionOccurredWithAmount(myWorldItem.Data, component.Data, "SEEN_BY", 10f);
		}
	}

	private void Update()
	{
		UpdateTrackedObjects();
	}

	private void UpdateTrackedObjects()
	{
		for (int i = 0; i < trackedWorldItems.Count; i++)
		{
			if (trackedWorldItems[i] == null || trackedPickupableItems[i] == null)
			{
				trackedWorldItems.RemoveAt(i);
				trackedPickupableItems.RemoveAt(i);
				i--;
				continue;
			}
			float num = Mathf.DeltaAngle(trackedWorldItems[i].transform.eulerAngles.y, -90f);
			if (num < max && num > min && trackedPickupableItems[i] != null && trackedPickupableItems[i].IsCurrInHand)
			{
				List<float> list;
				List<float> list2 = (list = timers);
				int index;
				int index2 = (index = i);
				float num2 = list[index];
				list2[index2] = num2 + Time.deltaTime;
				GameEventsManager.Instance.ItemAppliedToItemActionOccurredWithAmount(myWorldItem.Data, trackedWorldItems[i].Data, "SEEN_BY", timers[i]);
			}
		}
	}
}
