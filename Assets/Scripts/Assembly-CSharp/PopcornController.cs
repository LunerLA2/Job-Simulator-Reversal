using System;
using UnityEngine;

public class PopcornController : MonoBehaviour
{
	[SerializeField]
	private TemperatureStateItem temperatureState;

	[SerializeField]
	private ParticleSystem particleEffect;

	[SerializeField]
	private float tempToStartPopping = 100f;

	[SerializeField]
	private float tempOfMaxPopping = 200f;

	[SerializeField]
	private float popRateAtMax = 5f;

	private void OnEnable()
	{
		TemperatureStateItem temperatureStateItem = temperatureState;
		temperatureStateItem.OnTemperatureChangeWholeUnit = (Action<TemperatureStateItem>)Delegate.Combine(temperatureStateItem.OnTemperatureChangeWholeUnit, new Action<TemperatureStateItem>(UpdateEffect));
	}

	private void OnDisable()
	{
		TemperatureStateItem temperatureStateItem = temperatureState;
		temperatureStateItem.OnTemperatureChangeWholeUnit = (Action<TemperatureStateItem>)Delegate.Remove(temperatureStateItem.OnTemperatureChangeWholeUnit, new Action<TemperatureStateItem>(UpdateEffect));
	}

	private void UpdateEffect(TemperatureStateItem temp)
	{
		ParticleSystem.EmissionModule emission = particleEffect.emission;
		float num = 0f;
		if (temp.TemperatureCelsius >= tempToStartPopping)
		{
			float t = (temp.TemperatureCelsius - tempToStartPopping) / (tempOfMaxPopping - tempToStartPopping);
			num = Mathf.Lerp(0f, popRateAtMax, t);
		}
		ParticleSystem.MinMaxCurve rate = emission.rate;
		rate.constantMax = num;
		rate.curveMultiplier = num;
		rate.constantMin = num;
		emission.rate = rate;
	}
}
