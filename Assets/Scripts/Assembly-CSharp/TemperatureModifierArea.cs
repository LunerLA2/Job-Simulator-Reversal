using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TemperatureModifierArea : MonoBehaviour
{
	[SerializeField]
	private RigidbodyEnterExitTriggerEvents triggerEvents;

	[SerializeField]
	private float targetTemperature;

	[SerializeField]
	private float speedToTempMultiplier = 1f;

	private List<TemperatureStateItem> temperatureStateItemsInArea = new List<TemperatureStateItem>();

	private bool areAnyItemsInTemperatureArea;

	public float TargetTemperature
	{
		get
		{
			return targetTemperature;
		}
	}

	public void SetTargetTemperature(float degreesCelsuis)
	{
		targetTemperature = degreesCelsuis;
	}

	private void Start()
	{
		StartCoroutine(WaitAFrameThenSnapTemperatures());
	}

	private IEnumerator WaitAFrameThenSnapTemperatures()
	{
		yield return null;
		for (int i = 0; i < temperatureStateItemsInArea.Count; i++)
		{
			temperatureStateItemsInArea[i].SetManualTemperature(targetTemperature);
		}
	}

	private void OnEnable()
	{
		RigidbodyEnterExitTriggerEvents rigidbodyEnterExitTriggerEvents = triggerEvents;
		rigidbodyEnterExitTriggerEvents.OnRigidbodyEnterTrigger = (Action<Rigidbody>)Delegate.Combine(rigidbodyEnterExitTriggerEvents.OnRigidbodyEnterTrigger, new Action<Rigidbody>(RigidbodyEntered));
		RigidbodyEnterExitTriggerEvents rigidbodyEnterExitTriggerEvents2 = triggerEvents;
		rigidbodyEnterExitTriggerEvents2.OnRigidbodyExitTrigger = (Action<Rigidbody>)Delegate.Combine(rigidbodyEnterExitTriggerEvents2.OnRigidbodyExitTrigger, new Action<Rigidbody>(RigidbodyExited));
		if (triggerEvents.ActiveRigidbodiesTriggerInfo.Count > 0)
		{
			for (int i = 0; i < triggerEvents.ActiveRigidbodiesTriggerInfo.Count; i++)
			{
				RigidbodyEntered(triggerEvents.ActiveRigidbodiesTriggerInfo[i].Rigidbody);
			}
		}
	}

	private void OnDisable()
	{
		RigidbodyEnterExitTriggerEvents rigidbodyEnterExitTriggerEvents = triggerEvents;
		rigidbodyEnterExitTriggerEvents.OnRigidbodyEnterTrigger = (Action<Rigidbody>)Delegate.Remove(rigidbodyEnterExitTriggerEvents.OnRigidbodyEnterTrigger, new Action<Rigidbody>(RigidbodyEntered));
		RigidbodyEnterExitTriggerEvents rigidbodyEnterExitTriggerEvents2 = triggerEvents;
		rigidbodyEnterExitTriggerEvents2.OnRigidbodyExitTrigger = (Action<Rigidbody>)Delegate.Remove(rigidbodyEnterExitTriggerEvents2.OnRigidbodyExitTrigger, new Action<Rigidbody>(RigidbodyExited));
		if (temperatureStateItemsInArea.Count <= 0)
		{
			return;
		}
		for (int i = 0; i < temperatureStateItemsInArea.Count; i++)
		{
			if (temperatureStateItemsInArea[i] != null)
			{
				temperatureStateItemsInArea[i].RemoveTemperatureChangeZone();
			}
		}
		temperatureStateItemsInArea.Clear();
		areAnyItemsInTemperatureArea = false;
	}

	private void Update()
	{
		if (!areAnyItemsInTemperatureArea)
		{
			return;
		}
		for (int i = 0; i < temperatureStateItemsInArea.Count; i++)
		{
			if (temperatureStateItemsInArea[i] != null)
			{
				temperatureStateItemsInArea[i].ApplyHeat(targetTemperature, speedToTempMultiplier);
				continue;
			}
			temperatureStateItemsInArea.RemoveAt(i);
			i--;
		}
	}

	private void RigidbodyEntered(Rigidbody rb)
	{
		TemperatureStateItem component = rb.GetComponent<TemperatureStateItem>();
		if (component != null && !temperatureStateItemsInArea.Contains(component))
		{
			component.AddTemperatureChangeZone();
			temperatureStateItemsInArea.Add(component);
			if (temperatureStateItemsInArea.Count == 1)
			{
				areAnyItemsInTemperatureArea = true;
			}
		}
	}

	private void RigidbodyExited(Rigidbody rb)
	{
		TemperatureStateItem component = rb.GetComponent<TemperatureStateItem>();
		if (component != null && temperatureStateItemsInArea.Contains(component))
		{
			component.RemoveTemperatureChangeZone();
			temperatureStateItemsInArea.Remove(component);
			if (temperatureStateItemsInArea.Count == 0)
			{
				areAnyItemsInTemperatureArea = false;
			}
		}
	}
}
