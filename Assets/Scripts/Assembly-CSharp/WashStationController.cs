using OwlchemyVR2;
using UnityEngine;

public class WashStationController : KitchenTool
{
	[SerializeField]
	private OwlchemyVR2.GrabbableHinge waterHinge;

	[SerializeField]
	private GravityDispensingItem waterDispenser;

	[SerializeField]
	private AudioSourceHelper waterAudio;

	[SerializeField]
	private AudioClip soapDispenseSound;

	[SerializeField]
	private AudioClip[] handleAudio;

	private ElementSequence<AudioClip> handleAudioSequence;

	[SerializeField]
	private AudioClip drainSound;

	[SerializeField]
	private GravityDispensingItem soapDispenser;

	[SerializeField]
	private float minSoapDispenseDuration = 0.25f;

	[SerializeField]
	private bool useFluidContainer = true;

	[SerializeField]
	private ParticleCollectionZone particleCollectionZone;

	[SerializeField]
	private ContainerFluidSystem fluidContainer;

	[SerializeField]
	private Transform fluidDrainSpot;

	[SerializeField]
	private float drainSpeedMLperSecond = 100f;

	private float maxWaterDispenseRate;

	private float soapDispenseTimeLeft;

	private float waterTurnOnThreshold = 0.1f;

	private bool soapReleased = true;

	private void Awake()
	{
		handleAudioSequence = new ElementSequence<AudioClip>(handleAudio);
		maxWaterDispenseRate = waterDispenser.DispenseQuantityMLPerSecond;
		if (!useFluidContainer)
		{
			particleCollectionZone.gameObject.SetActive(false);
			fluidContainer.enabled = false;
		}
	}

	public override void OnDismiss()
	{
		base.OnDismiss();
		waterHinge.LockLower();
	}

	public void DispenseSoap()
	{
		soapDispenseTimeLeft = minSoapDispenseDuration;
		soapDispenser.enabled = true;
		soapReleased = false;
		AudioManager.Instance.Play(soapDispenser.transform.position, soapDispenseSound, 1f, 1f);
	}

	public void StopSoap()
	{
		if (soapDispenseTimeLeft <= 0f)
		{
			soapDispenser.enabled = false;
			AudioManager.Instance.Stop(soapDispenser.transform, soapDispenseSound);
		}
		soapReleased = true;
	}

	private void Update()
	{
		float num = Mathf.Round(waterHinge.NormalizedAxisValue * 10f) / 10f;
		if (waterDispenser.enabled && num <= waterTurnOnThreshold)
		{
			WaterToggle(false);
		}
		else if (!waterDispenser.enabled && num > waterTurnOnThreshold)
		{
			WaterToggle(true);
		}
		waterDispenser.DispenseQuantityMLPerSecond = num * maxWaterDispenseRate;
		waterAudio.SetPitch(Mathf.Min(Mathf.Max(0f, num - waterTurnOnThreshold) + 1f, 1.5f));
		if (soapDispenseTimeLeft > 0f)
		{
			soapDispenseTimeLeft -= Time.deltaTime;
			if (soapDispenseTimeLeft < 0f)
			{
				soapDispenseTimeLeft = 0f;
				if (soapReleased)
				{
					StopSoap();
				}
			}
		}
		if (useFluidContainer && particleCollectionZone.GetTotalQuantity() > 0f)
		{
			fluidContainer.ManualPourLiquid(drainSpeedMLperSecond * Time.deltaTime, fluidDrainSpot.position);
		}
	}

	private void WaterToggle(bool on)
	{
		waterDispenser.enabled = on;
		if (handleAudio.Length > 0)
		{
			AudioManager.Instance.Play(waterHinge.transform.position, handleAudioSequence.GetNext(), 1f, 1f);
		}
		if (!on && drainSound != null)
		{
			AudioManager.Instance.Play(fluidDrainSpot, drainSound, 1f, 1f);
		}
	}
}
