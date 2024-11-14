using System;
using System.Collections;
using OwlchemyVR;
using TMPro;
using UnityEngine;

public class CarInteriorDashboardHardware : VehicleHardware
{
	public enum PedalState
	{
		Idle = 0,
		Rev = 1,
		Drive = 2
	}

	private const float GAS_FILL_SPEED = 0.0025f;

	[SerializeField]
	private GrabbableHinge gearShifterHinge;

	[SerializeField]
	private GrabbableHinge gasPedalHinge;

	[SerializeField]
	private float maxWheelSpinSpeed = 1000f;

	[SerializeField]
	private AudioSourceHelper engineSoundLoop;

	[SerializeField]
	private float engineSoundInitVolume = 0.35f;

	[SerializeField]
	private AudioSource engineSoundAudioSource;

	[SerializeField]
	private float engineMinPitch = 0.5f;

	[SerializeField]
	private float engineMaxPitch = 2f;

	[SerializeField]
	private Transform steeringWheelTrackedTransform;

	[SerializeField]
	private Transform rpmTransform;

	[SerializeField]
	private Transform speedTransform;

	[SerializeField]
	private GameObject batteryIndicator;

	[SerializeField]
	private TextMeshPro odometerLabel;

	[SerializeField]
	private Transform gasLevelTransform;

	[SerializeField]
	private ParticleSystem ACParticleSystem;

	[SerializeField]
	private AudioClip acaudioclip;

	[SerializeField]
	private AudioSourceHelper acaudiosourcehelper;

	private Transform savedEngineAudioSourceParent;

	private Transform savedACAudioSourceParent;

	private Vector3 savedLocalPosEngineAudioSource = Vector3.zero;

	private Vector3 savedLocalPosACAudioSource = Vector3.zero;

	private bool audioSourcesOffloaded;

	private float smoothedSpeedValue;

	private float lastWheelAngle;

	private PedalState pedalState;

	private float distanceDriven;

	private VehicleEngineHardware engine;

	private GasTankController gasTank;

	private VehicleChassisController.Gear currentGear;

	[SerializeField]
	private WorldItemData gearShiftDriveWorldItemData;

	[SerializeField]
	private WorldItemData gearShiftReverseWorldItemData;

	[SerializeField]
	private WorldItemData driveForwardsWorldItemData;

	[SerializeField]
	private WorldItemData driveBackwardsWorldItemData;

	private float distanceChangeSinceAwake;

	public Transform GearShifterTransform
	{
		get
		{
			return gearShifterHinge.transform;
		}
	}

	public PedalState CurrentPedalState
	{
		get
		{
			return pedalState;
		}
		set
		{
			pedalState = value;
		}
	}

	private void Awake()
	{
		SetGasLevel(0.5f);
		currentGear = VehicleChassisController.Gear.Drive;
		DoGameEventForGearChange();
		distanceDriven = UnityEngine.Random.Range(50000f, 500000f);
		distanceChangeSinceAwake = 0f;
		UpdateOdometerLabel();
	}

	private IEnumerator Start()
	{
		while (JobBoardManager.instance == null || JobBoardManager.instance.GetCurrentTaskData() == null)
		{
			yield return null;
		}
		engine = parentChassis.Engine;
		AttachablePoint[] batteryAttachPoints = engine.BatteryAttachPoints;
		foreach (AttachablePoint battery in batteryAttachPoints)
		{
			battery.OnObjectWasAttached += OnBatteryWasAttached;
			battery.OnObjectWasDetached += OnBatteryWasDetached;
			batteryIndicator.SetActive(battery.NumAttachedObjects > 0);
		}
		gasTank = parentChassis.GasTank;
		ParticleImpactZone gasImpactZone = gasTank.GasImpactZone;
		gasImpactZone.OnAnyParticleAppliedUpdate = (Action<ParticleImpactZone, Vector3>)Delegate.Combine(gasImpactZone.OnAnyParticleAppliedUpdate, new Action<ParticleImpactZone, Vector3>(OnFluidAddedToGasTank));
	}

	private void OnFluidAddedToGasTank(ParticleImpactZone particleImpactZone, Vector3 vector3)
	{
		SetGasLevel(gasLevelTransform.localScale.z + 0.0025f);
	}

	private void OnBatteryWasDetached(AttachablePoint attachablePoint, AttachableObject attachableObject)
	{
		batteryIndicator.SetActive(false);
	}

	private void OnBatteryWasAttached(AttachablePoint attachablePoint, AttachableObject attachableObject)
	{
		batteryIndicator.SetActive(true);
	}

	private void OnEnable()
	{
		gearShifterHinge.OnLowerLocked += ShiftedUp;
		gearShifterHinge.OnUpperLocked += ShiftedDown;
	}

	private void OnDisable()
	{
		gearShifterHinge.OnLowerLocked -= ShiftedUp;
		gearShifterHinge.OnUpperLocked -= ShiftedDown;
	}

	public void StartEngineSound()
	{
		if (engineSoundLoop.GetClip() != null)
		{
			engineSoundLoop.SetPitch(engineMinPitch);
			engineSoundLoop.SetVolume(engineSoundInitVolume);
			engineSoundLoop.Play();
		}
	}

	public void StopEngineSound()
	{
		if (engineSoundLoop.GetClip() != null)
		{
			engineSoundLoop.Stop();
		}
	}

	public override void WillBecomeOptimized()
	{
		base.WillBecomeOptimized();
		if (!audioSourcesOffloaded)
		{
			audioSourcesOffloaded = true;
			savedLocalPosEngineAudioSource = engineSoundLoop.transform.localPosition;
			savedLocalPosACAudioSource = acaudiosourcehelper.transform.localPosition;
			savedEngineAudioSourceParent = engineSoundLoop.transform.parent;
			savedACAudioSourceParent = acaudiosourcehelper.transform.parent;
			engineSoundLoop.transform.SetParent(parentChassis.transform, true);
			acaudiosourcehelper.transform.SetParent(parentChassis.transform, true);
		}
	}

	public override void HasBecomeUnoptimized()
	{
		base.HasBecomeUnoptimized();
		if (audioSourcesOffloaded)
		{
			audioSourcesOffloaded = false;
			engineSoundLoop.transform.SetParent(savedEngineAudioSourceParent);
			engineSoundLoop.transform.localPosition = savedLocalPosEngineAudioSource;
			acaudiosourcehelper.transform.SetParent(savedACAudioSourceParent);
			acaudiosourcehelper.transform.localPosition = savedLocalPosACAudioSource;
		}
	}

	private void ShiftedDown(GrabbableHinge hinge, bool isInitial)
	{
		if (parentChassis != null)
		{
			parentChassis.ChangeGear(VehicleChassisController.Gear.Reverse);
		}
		currentGear = VehicleChassisController.Gear.Reverse;
		DoGameEventForGearChange();
	}

	public void SetGasLevel(float level)
	{
		gasLevelTransform.localScale = new Vector3(1f, 1f, Mathf.Clamp01(level));
	}

	public void OnACButton()
	{
		if (!ACParticleSystem.isPlaying)
		{
			ACParticleSystem.Play();
			acaudiosourcehelper.SetClip(acaudioclip);
			acaudiosourcehelper.Play();
		}
	}

	private void ShiftedUp(GrabbableHinge hinge, bool isInitial)
	{
		if (parentChassis != null)
		{
			parentChassis.ChangeGear(VehicleChassisController.Gear.Drive);
		}
		currentGear = VehicleChassisController.Gear.Drive;
		DoGameEventForGearChange();
	}

	private void DoGameEventForGearChange()
	{
		if (currentGear == VehicleChassisController.Gear.Drive)
		{
			GameEventsManager.Instance.ItemActionOccurred(gearShiftDriveWorldItemData, "OPENED");
			GameEventsManager.Instance.ItemActionOccurred(gearShiftReverseWorldItemData, "CLOSED");
		}
		else if (currentGear == VehicleChassisController.Gear.Reverse)
		{
			GameEventsManager.Instance.ItemActionOccurred(gearShiftDriveWorldItemData, "CLOSED");
			GameEventsManager.Instance.ItemActionOccurred(gearShiftReverseWorldItemData, "OPENED");
		}
	}

	private void Update()
	{
		float num;
		for (num = steeringWheelTrackedTransform.localEulerAngles.y; num >= 360f; num -= 360f)
		{
		}
		float num2 = num - lastWheelAngle;
		if (num2 > 180f)
		{
			num2 = 0f - (360f - num2);
		}
		if (parentChassis != null)
		{
			parentChassis.SteerWheelsByAmount(num2 / 10f);
		}
		for (lastWheelAngle = steeringWheelTrackedTransform.localEulerAngles.y; lastWheelAngle >= 360f; lastWheelAngle -= 360f)
		{
		}
		float num3 = 1f - gasPedalHinge.NormalizedAngle;
		switch (pedalState)
		{
		case PedalState.Idle:
		{
			if (num3 >= 0.05f)
			{
				pedalState = PedalState.Rev;
				break;
			}
			float b2 = engineSoundLoop.GetPitch() - Time.deltaTime;
			engineSoundLoop.SetPitch(Mathf.Max(engineMinPitch, b2));
			float b3 = engineSoundAudioSource.volume - Time.deltaTime;
			engineSoundLoop.SetVolume(Mathf.Max(engineSoundInitVolume, b3));
			break;
		}
		case PedalState.Rev:
		{
			int num6 = 0;
			int num7 = -1;
			if (num6 == 0 && parentChassis != null)
			{
				parentChassis.SpinWheelsByAmount(Time.deltaTime * num3 * maxWheelSpinSpeed);
			}
			float num8 = num3 * Time.deltaTime * maxWheelSpinSpeed * 0.08f;
			if (currentGear == VehicleChassisController.Gear.Reverse)
			{
				num8 *= -1f;
			}
			distanceDriven += num8;
			distanceChangeSinceAwake += num8;
			if (num7 == 2 || num7 == -1)
			{
				GameEventsManager.Instance.ItemActionOccurredWithAmount(driveForwardsWorldItemData, "USED_PARTIALLY", distanceChangeSinceAwake / 100f);
			}
			if (num7 == 5 || num7 == -1)
			{
				GameEventsManager.Instance.ItemActionOccurredWithAmount(driveBackwardsWorldItemData, "USED_PARTIALLY", (0f - distanceChangeSinceAwake) / 100f);
			}
			if (num7 == 8 || num7 == -1)
			{
				UpdateOdometerLabel();
				UpdateGasLevel(distanceDriven);
			}
			engineSoundLoop.SetPitch(Mathf.Lerp(engineMinPitch, engineMaxPitch, num3));
			engineSoundLoop.SetVolume(Mathf.Lerp(engineSoundInitVolume, 1f, num3));
			if (num3 < 0.05f)
			{
				pedalState = PedalState.Idle;
			}
			break;
		}
		case PedalState.Drive:
		{
			float num4 = engineMaxPitch - engineMinPitch;
			float num5 = num4 * 0.5f + engineMinPitch;
			float b = engineSoundLoop.GetPitch() + Time.deltaTime;
			float a = engineSoundAudioSource.volume + Time.deltaTime;
			engineSoundLoop.SetPitch(Mathf.Min(num5, b));
			engineSoundLoop.SetVolume(Mathf.Min(a, 1f));
			if (parentChassis != null)
			{
				parentChassis.SpinWheelsByAmount(Time.deltaTime * num5 * maxWheelSpinSpeed);
			}
			break;
		}
		default:
			Debug.LogWarning("Unimplemented pedal state");
			break;
		}
		rpmTransform.localEulerAngles = Vector3.forward * Mathf.Lerp(-86f, 0f, num3 * Mathf.Lerp(Mathf.PerlinNoise(Time.time * 10f, 0f), 1f, 0.9f));
		if (smoothedSpeedValue < num3)
		{
			smoothedSpeedValue = Mathf.Min(num3, smoothedSpeedValue + Time.deltaTime * 3f);
		}
		if (smoothedSpeedValue > num3)
		{
			smoothedSpeedValue = Mathf.Max(num3, smoothedSpeedValue - Time.deltaTime * 3f);
		}
		speedTransform.localEulerAngles = Vector3.forward * Mathf.Lerp(0f, 180f, smoothedSpeedValue);
	}

	private void UpdateOdometerLabel()
	{
		odometerLabel.text = (Mathf.RoundToInt(distanceDriven) / 10).ToString("000000") + ".<color=#991111>" + Mathf.RoundToInt(distanceDriven) % 10 + "</color>";
	}

	private void UpdateGasLevel(float miles)
	{
		if (!(gasLevelTransform.position.z <= 0f))
		{
			SetGasLevel(gasLevelTransform.localScale.z - 0.0025f * Time.deltaTime * 10f);
		}
	}
}
