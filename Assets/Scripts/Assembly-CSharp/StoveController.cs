using System;
using System.Globalization;
using OwlchemyVR;
using OwlchemyVR2;
using TMPro;
using UnityEngine;

public class StoveController : MonoBehaviour
{
	[SerializeField]
	private WorldItem myWorldItem;

	[SerializeField]
	private OwlchemyVR2.GrabbableHinge hingeToDriveCookTemp;

	[SerializeField]
	private float hingeRatioToConsiderTurnedOn = 0.1f;

	[SerializeField]
	private GameObject powerLightOn;

	[SerializeField]
	private GameObject powerLightOff;

	[SerializeField]
	private TextMeshPro timeText;

	[SerializeField]
	private AudioClip lightToggleAudio;

	private DateTime currentTime;

	private float timer;

	private bool isOn;

	private StoveGrillController grillController;

	private StovePotModeController potModeController;

	public bool IsOn
	{
		get
		{
			return isOn;
		}
	}

	protected OwlchemyVR2.GrabbableHinge HingeToDriveCookTemp
	{
		get
		{
			return hingeToDriveCookTemp;
		}
	}

	private void Start()
	{
		if (hingeToDriveCookTemp != null)
		{
			SetPowerState(false);
			UpdatePowerStateFromHinge();
		}
		else
		{
			SetPowerState(false);
		}
		if (timeText != null)
		{
			currentTime = new DateTime(2016, 1, 1, 9, 0, 0);
			if (GenieManager.AreAnyJobGenieModesActive() && GenieManager.DoesContainGenieMode(GlobalStorage.Instance.CurrentGenieModes, JobGenieCartridge.GenieModeTypes.EndlessMode))
			{
				currentTime = new DateTime(2016, 1, 1, 0, 0, 0);
			}
			timeText.text = GetClockTime();
		}
	}

	public void AttachToGrillController(StoveGrillController grill)
	{
		grillController = grill;
		UpdatePowerStateFromHinge();
	}

	public void AttachToPotModeController(StovePotModeController pot)
	{
		potModeController = pot;
		UpdatePowerStateFromHinge();
	}

	private void UpdatePowerStateFromHinge()
	{
		bool flag = hingeToDriveCookTemp.NormalizedAxisValue >= hingeRatioToConsiderTurnedOn;
		if (flag != isOn)
		{
			SetPowerState(flag);
		}
		if (isOn)
		{
			if (grillController != null)
			{
				float counterTemperature = Mathf.Lerp(grillController.MaxCookTemp / 2f, grillController.MaxCookTemp, hingeToDriveCookTemp.NormalizedAxisValue);
				grillController.SetCounterTemperature(counterTemperature);
			}
			if (potModeController != null)
			{
				float counterTemperature2 = Mathf.Lerp(potModeController.MaxCookTemp / 2f, potModeController.MaxCookTemp, hingeToDriveCookTemp.NormalizedAxisValue);
				potModeController.SetCounterTemperature(counterTemperature2);
			}
		}
		else
		{
			if (grillController != null)
			{
				grillController.SetCounterTemperature(21f);
			}
			if (potModeController != null)
			{
				potModeController.SetCounterTemperature(21f);
			}
		}
	}

	private void SetPowerState(bool on)
	{
		if (lightToggleAudio != null)
		{
			AudioManager.Instance.Play(powerLightOn.transform.position, lightToggleAudio, 1f, 1f);
		}
		if (powerLightOn != null)
		{
			powerLightOn.SetActive(on);
		}
		if (powerLightOff != null)
		{
			powerLightOff.SetActive(!on);
		}
		if (grillController != null)
		{
			grillController.SetPowerState(on);
		}
		if (potModeController != null)
		{
			potModeController.SetPowerState(on);
		}
		if (on)
		{
			if (myWorldItem != null)
			{
				GameEventsManager.Instance.ItemActionOccurred(myWorldItem.Data, "ACTIVATED");
			}
		}
		else if (myWorldItem != null)
		{
			GameEventsManager.Instance.ItemActionOccurred(myWorldItem.Data, "DEACTIVATED");
		}
		isOn = on;
	}

	private void Update()
	{
		if (hingeToDriveCookTemp != null)
		{
			UpdatePowerStateFromHinge();
		}
		if (timeText != null)
		{
			timer += Time.deltaTime;
			if (timer >= 60f)
			{
				currentTime = currentTime.AddMinutes(1.0);
				timeText.text = GetClockTime();
				timer -= 60f;
			}
		}
	}

	private string GetClockTime()
	{
		return currentTime.ToString("h:mm tt", CultureInfo.InvariantCulture);
	}
}
