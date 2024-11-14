using System.Collections;
using UnityEngine;

public class StovePotModeController : KitchenTool
{
	[SerializeField]
	private TemperatureModifierArea temperatureModifierArea;

	[SerializeField]
	private ParticleSystem[] stoveOnPFX;

	[SerializeField]
	private string stoveUniqueObjName = "Stove";

	private float maxCookTemp;

	public float MaxCookTemp
	{
		get
		{
			return maxCookTemp;
		}
	}

	private void Awake()
	{
		maxCookTemp = temperatureModifierArea.TargetTemperature;
	}

	private IEnumerator Start()
	{
		yield return new WaitForEndOfFrame();
		UniqueObject stove = BotUniqueElementManager.Instance.GetObjectByName(stoveUniqueObjName);
		if (stove != null)
		{
			StoveController sc = stove.GetComponent<StoveController>();
			sc.AttachToPotModeController(this);
			SetPowerState(sc.IsOn);
		}
		else
		{
			SetPowerState(false);
		}
	}

	public void SetPowerState(bool on)
	{
		if (stoveOnPFX != null)
		{
			for (int i = 0; i < stoveOnPFX.Length; i++)
			{
				if (stoveOnPFX[i] != null)
				{
					ParticleSystem.EmissionModule emission = stoveOnPFX[i].emission;
					emission.enabled = on;
				}
			}
		}
		if (on)
		{
			SetCounterTemperature(maxCookTemp);
		}
		else
		{
			SetCounterTemperature(21f);
		}
	}

	public void SetCounterTemperature(float temperature)
	{
		temperatureModifierArea.SetTargetTemperature(temperature);
	}
}
