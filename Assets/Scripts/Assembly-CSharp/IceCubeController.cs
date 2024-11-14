using System;
using OwlchemyVR;
using UnityEngine;

public class IceCubeController : MonoBehaviour
{
	[SerializeField]
	private Transform scaler;

	[SerializeField]
	private Renderer rendererToTint;

	[SerializeField]
	private WorldItem myWorldItem;

	[SerializeField]
	private TemperatureStateItem temperatureState;

	[SerializeField]
	private GravityDispensingItem meltLiquidDispenser;

	private float frozenPercentage = 1f;

	private bool isTempBelowZero;

	private float meltSpeedMultiplier = 0.02f;

	private void OnEnable()
	{
		TemperatureStateItem temperatureStateItem = temperatureState;
		temperatureStateItem.OnTemperatureChangeWholeUnit = (Action<TemperatureStateItem>)Delegate.Combine(temperatureStateItem.OnTemperatureChangeWholeUnit, new Action<TemperatureStateItem>(TempChanged));
	}

	private void OnDisable()
	{
		TemperatureStateItem temperatureStateItem = temperatureState;
		temperatureStateItem.OnTemperatureChangeWholeUnit = (Action<TemperatureStateItem>)Delegate.Remove(temperatureStateItem.OnTemperatureChangeWholeUnit, new Action<TemperatureStateItem>(TempChanged));
	}

	public void Setup(WorldItemData worldItem)
	{
		meltLiquidDispenser.SetFluidToDispense(worldItem);
		rendererToTint.material.color = worldItem.OverallColor;
	}

	private void TempChanged(TemperatureStateItem temp)
	{
	}

	private void Update()
	{
		isTempBelowZero = temperatureState.TemperatureCelsius <= 0f;
		meltLiquidDispenser.enabled = !isTempBelowZero;
		if (isTempBelowZero)
		{
			frozenPercentage = Mathf.Min(1f, frozenPercentage + Time.deltaTime * meltSpeedMultiplier);
		}
		else
		{
			frozenPercentage = Mathf.Max(0f, frozenPercentage - Time.deltaTime * meltSpeedMultiplier * temperatureState.TemperatureCelsius / 10f);
		}
		scaler.localScale = Vector3.one * Mathf.Lerp(0.25f, 1f, frozenPercentage);
		if (frozenPercentage <= 0f)
		{
			GameEventsManager.Instance.ItemActionOccurred(myWorldItem.Data, "DESTROYED");
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}
}
