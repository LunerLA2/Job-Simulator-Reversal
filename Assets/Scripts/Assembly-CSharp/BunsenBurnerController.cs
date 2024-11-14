using OwlchemyVR;
using UnityEngine;

public class BunsenBurnerController : MonoBehaviour
{
	private const float MIN_FLAME_HEIGHT = 0.05f;

	private const float MAX_FLAME_HEIGHT = 0.2f;

	[SerializeField]
	private WorldItem myWorldItem;

	[SerializeField]
	private GrabbableHinge hingeToDriveTemp;

	[SerializeField]
	private float hingeRatioToConsiderTurnedOn = 0.1f;

	[SerializeField]
	private TemperatureModifierArea temperatureZone;

	[SerializeField]
	private ParticleSystem flame;

	private bool isOn;

	private void Start()
	{
		temperatureZone.enabled = false;
		if (hingeToDriveTemp != null)
		{
			SetPowerState(false);
			UpdatePowerStateFromHinge();
		}
		else
		{
			SetPowerState(false);
		}
	}

	private void Update()
	{
		if (hingeToDriveTemp != null)
		{
			UpdatePowerStateFromHinge();
		}
	}

	private void UpdatePowerStateFromHinge()
	{
		bool flag = hingeToDriveTemp.NormalizedAngle >= hingeRatioToConsiderTurnedOn;
		if (flag != isOn)
		{
			SetPowerState(flag);
		}
		if (isOn)
		{
			float startLifetime = Mathf.Lerp(0.05f, 0.2f, hingeToDriveTemp.NormalizedAngle);
			flame.startLifetime = startLifetime;
		}
	}

	private void SetPowerState(bool on)
	{
		if (on)
		{
			if (flame != null)
			{
				flame.Play();
				temperatureZone.enabled = true;
			}
			if (myWorldItem != null)
			{
				GameEventsManager.Instance.ItemActionOccurred(myWorldItem.Data, "ACTIVATED");
			}
		}
		else
		{
			if (flame != null)
			{
				flame.Stop();
				temperatureZone.enabled = false;
			}
			if (myWorldItem != null)
			{
				GameEventsManager.Instance.ItemActionOccurred(myWorldItem.Data, "DEACTIVATED");
			}
		}
		isOn = on;
	}
}
