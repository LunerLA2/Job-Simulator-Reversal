using System;
using UnityEngine;

public class TemperatureTeser : MonoBehaviour
{
	private TemperatureStateItem temperatureStateItem;

	private void Awake()
	{
		temperatureStateItem = GetComponent<TemperatureStateItem>();
	}

	private void OnEnable()
	{
		TemperatureStateItem obj = temperatureStateItem;
		obj.OnTemperatureChangeWholeUnit = (Action<TemperatureStateItem>)Delegate.Combine(obj.OnTemperatureChangeWholeUnit, new Action<TemperatureStateItem>(TemperatureChangeWholeUnit));
	}

	private void OnDisable()
	{
		TemperatureStateItem obj = temperatureStateItem;
		obj.OnTemperatureChangeWholeUnit = (Action<TemperatureStateItem>)Delegate.Remove(obj.OnTemperatureChangeWholeUnit, new Action<TemperatureStateItem>(TemperatureChangeWholeUnit));
	}

	private void TemperatureChangeWholeUnit(TemperatureStateItem stateItem)
	{
		Debug.Log("New temp:" + stateItem.TemperatureCelsius);
	}
}
