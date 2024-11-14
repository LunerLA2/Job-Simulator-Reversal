using System.Collections.Generic;
using UnityEngine;

public class TemperatureManager : MonoBehaviour
{
	private List<TemperatureStateItem> registeredReturnToRoomTempList = new List<TemperatureStateItem>();

	private bool doesRegisteredReturnToRoomTempListContainItems;

	private static TemperatureManager _instance;

	public static TemperatureManager InstanceNoCreate
	{
		get
		{
			return _instance;
		}
	}

	public static TemperatureManager Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = Object.FindObjectOfType<TemperatureManager>();
				if (_instance == null)
				{
					_instance = new GameObject("_TemperatureManager").AddComponent<TemperatureManager>();
				}
			}
			return _instance;
		}
	}

	public void RegisterReturnToRoomTempStateItem(TemperatureStateItem temperatureStateItem)
	{
		registeredReturnToRoomTempList.Add(temperatureStateItem);
		doesRegisteredReturnToRoomTempListContainItems = true;
	}

	public void DeregisterReturnToRoomTempStateItem(TemperatureStateItem temperatureStateItem)
	{
		registeredReturnToRoomTempList.Remove(temperatureStateItem);
		if (registeredReturnToRoomTempList.Count == 0)
		{
			doesRegisteredReturnToRoomTempListContainItems = false;
		}
	}

	private void LateUpdate()
	{
		if (!doesRegisteredReturnToRoomTempListContainItems)
		{
			return;
		}
		for (int i = 0; i < registeredReturnToRoomTempList.Count; i++)
		{
			if (registeredReturnToRoomTempList[i] != null)
			{
				if (registeredReturnToRoomTempList[i].ReturnToRoomTemperatureUpdate())
				{
					registeredReturnToRoomTempList[i].ManualSetRegisteredToReturnToRoomTempToFalse();
					registeredReturnToRoomTempList.RemoveAt(i);
					i--;
				}
			}
			else
			{
				registeredReturnToRoomTempList.RemoveAt(i);
				i--;
			}
		}
	}

	public void Init()
	{
	}

	private void Awake()
	{
		if (_instance == null)
		{
			_instance = this;
		}
		else if (_instance != this)
		{
			Object.Destroy(base.gameObject);
		}
	}
}
