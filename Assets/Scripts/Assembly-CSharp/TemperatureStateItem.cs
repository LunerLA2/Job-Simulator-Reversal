using System;
using UnityEngine;

public class TemperatureStateItem : MonoBehaviour
{
	public const float ROOM_TEMPERATURE_CELSIUS = 21f;

	private const float RATE_OF_ROOM_TEMPERATURE_RETURN = 0.2f;

	private const float GENERAL_SPEED_TEMPERATURE_CHANGE_MULTIPLIER = 5f;

	[SerializeField]
	[Tooltip("A measurable physical quantity equal to the ratio of the heat added to (or removed from) an object to the resulting temperature change (inv) (lower number longer to change)")]
	private float heatCapacityInv = 1f;

	private float temperatureCelsius = 21f;

	private int temperatureLastRecordedWholeUnit = 21;

	public Action<TemperatureStateItem> OnTemperatureChangeWholeUnit;

	private bool isRegisteredToReturnToRoomTemp;

	private int numOfActiveTemperatureChangeZones;

	public float TemperatureCelsius
	{
		get
		{
			return temperatureCelsius;
		}
	}

	private void Awake()
	{
		if (TemperatureManager.InstanceNoCreate == null)
		{
			TemperatureManager.Instance.Init();
		}
	}

	public void SetManualTemperature(float manualTemperature)
	{
		if (temperatureCelsius != manualTemperature)
		{
			SetTemperature(manualTemperature);
			if (temperatureCelsius != 21f)
			{
				RegisterReturnToRoomTemp();
			}
		}
	}

	public void SetHeatCapacityInv(float value)
	{
		heatCapacityInv = value;
	}

	public void ManualSetRegisteredToReturnToRoomTempToFalse()
	{
		isRegisteredToReturnToRoomTemp = false;
	}

	public void AddTemperatureChangeZone()
	{
		numOfActiveTemperatureChangeZones++;
		DeregisterReturnToRoomTemp();
	}

	public void RemoveTemperatureChangeZone()
	{
		numOfActiveTemperatureChangeZones--;
		if (numOfActiveTemperatureChangeZones == 0 && temperatureCelsius != 21f)
		{
			RegisterReturnToRoomTemp();
		}
	}

	public void ApplyHeat(float targetTemperature, float speedToTargetMultiplier = 1f)
	{
		if (temperatureCelsius != targetTemperature)
		{
			SetTemperature(targetTemperature, speedToTargetMultiplier);
		}
	}

	public bool ReturnToRoomTemperatureUpdate()
	{
		SetTemperature(21f, 0.2f);
		return temperatureCelsius == 21f;
	}

	private void SetTemperature(float targetTemperature, float multiplier)
	{
		if (Mathf.Abs(temperatureCelsius - targetTemperature) < 0.1f)
		{
			SetTemperature(targetTemperature);
		}
		else
		{
			SetTemperature(Mathf.Lerp(temperatureCelsius, targetTemperature, Time.deltaTime * multiplier * 5f * heatCapacityInv));
		}
	}

	private void SetTemperature(float temperature)
	{
		temperatureCelsius = temperature;
		if ((int)temperatureCelsius != temperatureLastRecordedWholeUnit)
		{
			temperatureLastRecordedWholeUnit = (int)temperatureCelsius;
			if (OnTemperatureChangeWholeUnit != null)
			{
				OnTemperatureChangeWholeUnit(this);
			}
		}
	}

	private void RegisterReturnToRoomTemp()
	{
		if (!isRegisteredToReturnToRoomTemp)
		{
			TemperatureManager.InstanceNoCreate.RegisterReturnToRoomTempStateItem(this);
			isRegisteredToReturnToRoomTemp = true;
		}
	}

	private void DeregisterReturnToRoomTemp()
	{
		if (isRegisteredToReturnToRoomTemp)
		{
			TemperatureManager.InstanceNoCreate.DeregisterReturnToRoomTempStateItem(this);
			isRegisteredToReturnToRoomTemp = false;
		}
	}
}
