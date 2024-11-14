using System;
using System.Collections;
using System.Collections.Generic;
using OwlchemyVR;
using UnityEngine;

public class BlenderBodyController : MonoBehaviour
{
	public const float REQUIRED_POWER_RATIO_BEFORE_SPILL = 0.9f;

	[SerializeField]
	private BlendResult[] blendResults;

	[SerializeField]
	private ParticleCollectionZone fluidParticleCollectionZone;

	[SerializeField]
	private ContainerFluidSystem fluidContainer;

	[SerializeField]
	private Transform emptyFrom;

	[SerializeField]
	private float emptyMLperSec = 1000f;

	[SerializeField]
	private float spillMLperSec = 400f;

	[SerializeField]
	private SubtaskData[] subtasksToAutoEmptyAfter;

	private bool isEmptying;

	private bool isSpilling;

	[SerializeField]
	private Transform spinnerTransform;

	[SerializeField]
	private RigidbodyEnterExitTriggerEvents blendZone;

	[SerializeField]
	private PlayerPartDetector handShredRegion;

	[SerializeField]
	private Transform killAxis;

	[SerializeField]
	private WorldItemData[] doNotBlendItems;

	[SerializeField]
	private float shakeAngleMultipler;

	[SerializeField]
	private ParticleSystem spillPFX;

	[SerializeField]
	private float shakeSpeedMultiplier;

	[SerializeField]
	private float handShredHapticsSpeedMultiplier = 0.6f;

	[SerializeField]
	private AudioSourceHelper audioSource;

	[SerializeField]
	private AudioClip blenderStartSound;

	[SerializeField]
	private AudioClip blenderLoopSound;

	[SerializeField]
	private AudioClip blenderStopSound;

	[SerializeField]
	private AudioClip[] itemDropSounds;

	[SerializeField]
	private AudioClip plopSound;

	private BlenderBaseController connectedBase;

	private bool isBlending;

	private float currentSpeed;

	private List<Rigidbody> blendingItems = new List<Rigidbody>();

	private AnimationCurve shakeSinusoid = new AnimationCurve(new Keyframe(0f, 1f, 0f, 0f), new Keyframe(0.5f, -1f, 0f, 0f), new Keyframe(1f, 1f, 0f, 0f));

	private float shakePhase;

	private HapticInfoObject rightHandShredHaptics;

	private HapticInfoObject leftHandShredHaptics;

	public Action<WorldItemData> OnItemWasBlended;

	public bool IsBlending
	{
		get
		{
			return isBlending;
		}
	}

	private void Awake()
	{
		shakeSinusoid.preWrapMode = WrapMode.Loop;
		shakeSinusoid.postWrapMode = WrapMode.Loop;
		rightHandShredHaptics = new HapticInfoObject(0f);
		leftHandShredHaptics = new HapticInfoObject(0f);
	}

	private void OnEnable()
	{
		RigidbodyEnterExitTriggerEvents rigidbodyEnterExitTriggerEvents = blendZone;
		rigidbodyEnterExitTriggerEvents.OnRigidbodyEnterTrigger = (Action<Rigidbody>)Delegate.Combine(rigidbodyEnterExitTriggerEvents.OnRigidbodyEnterTrigger, new Action<Rigidbody>(ItemDroppedIntoBlender));
		PlayerPartDetector playerPartDetector = handShredRegion;
		playerPartDetector.OnHandEntered = (Action<PlayerPartDetector, InteractionHandController>)Delegate.Combine(playerPartDetector.OnHandEntered, new Action<PlayerPartDetector, InteractionHandController>(HandEnteredShredRegion));
		PlayerPartDetector playerPartDetector2 = handShredRegion;
		playerPartDetector2.OnHandExited = (Action<PlayerPartDetector, InteractionHandController>)Delegate.Combine(playerPartDetector2.OnHandExited, new Action<PlayerPartDetector, InteractionHandController>(HandExitedShredRegion));
		JobBoardManager instance = JobBoardManager.instance;
		instance.OnSubtaskComplete = (Action<SubtaskStatusController>)Delegate.Combine(instance.OnSubtaskComplete, new Action<SubtaskStatusController>(SubtaskComplete));
	}

	private void OnDisable()
	{
		RigidbodyEnterExitTriggerEvents rigidbodyEnterExitTriggerEvents = blendZone;
		rigidbodyEnterExitTriggerEvents.OnRigidbodyEnterTrigger = (Action<Rigidbody>)Delegate.Remove(rigidbodyEnterExitTriggerEvents.OnRigidbodyEnterTrigger, new Action<Rigidbody>(ItemDroppedIntoBlender));
		PlayerPartDetector playerPartDetector = handShredRegion;
		playerPartDetector.OnHandEntered = (Action<PlayerPartDetector, InteractionHandController>)Delegate.Remove(playerPartDetector.OnHandEntered, new Action<PlayerPartDetector, InteractionHandController>(HandEnteredShredRegion));
		PlayerPartDetector playerPartDetector2 = handShredRegion;
		playerPartDetector2.OnHandExited = (Action<PlayerPartDetector, InteractionHandController>)Delegate.Remove(playerPartDetector2.OnHandExited, new Action<PlayerPartDetector, InteractionHandController>(HandExitedShredRegion));
		JobBoardManager instance = JobBoardManager.instance;
		instance.OnSubtaskComplete = (Action<SubtaskStatusController>)Delegate.Remove(instance.OnSubtaskComplete, new Action<SubtaskStatusController>(SubtaskComplete));
	}

	private void SubtaskComplete(SubtaskStatusController subtask)
	{
		if (Array.IndexOf(subtasksToAutoEmptyAfter, subtask.Data) > -1)
		{
			Empty();
		}
	}

	public void Empty()
	{
		if (!isEmptying)
		{
			isEmptying = true;
			StartCoroutine(InternalEmpty());
		}
	}

	private IEnumerator InternalEmpty()
	{
		float mlToEmpty = fluidParticleCollectionZone.GetTotalQuantity();
		while (mlToEmpty > 0f)
		{
			float mlThisFrame = Time.deltaTime * emptyMLperSec;
			if (isSpilling)
			{
				mlThisFrame = Time.deltaTime * spillMLperSec;
			}
			else
			{
				fluidContainer.ManualPourLiquid(mlThisFrame, emptyFrom.position);
			}
			mlToEmpty -= mlThisFrame;
			yield return null;
		}
		yield return null;
		isEmptying = false;
	}

	private IEnumerator InternalBlend(Rigidbody rb)
	{
		if (rb == null || blendingItems.Contains(rb))
		{
			yield break;
		}
		WorldItem wi = rb.GetComponent<WorldItem>();
		if (wi != null)
		{
			for (int l = 0; l < doNotBlendItems.Length; l++)
			{
				if (doNotBlendItems[l] == wi.Data)
				{
					yield break;
				}
			}
			TemperatureStateItem temperatureState = rb.GetComponent<TemperatureStateItem>();
			float temperature = 21f;
			if (temperatureState != null)
			{
				temperature = temperatureState.TemperatureCelsius;
			}
			blendingItems.Add(rb);
			PickupableItem pickupableItem = rb.GetComponent<PickupableItem>();
			if (pickupableItem != null)
			{
				if (pickupableItem.IsCurrInHand)
				{
					pickupableItem.CurrInteractableHand.ManuallyReleaseJoint();
				}
				pickupableItem.enabled = false;
			}
			Rigidbody[] rbs = rb.GetComponentsInChildren<Rigidbody>();
			for (int k = 0; k < rbs.Length; k++)
			{
				rbs[k].isKinematic = true;
			}
			Transform tr = rb.transform;
			Vector3 startPosRelToKillAxis = killAxis.InverseTransformPoint(tr.position);
			Vector3 targetPosRelToKillAxis = new Vector3(startPosRelToKillAxis.x, 0f, 0f);
			Vector3 startScale = tr.localScale;
			float blendTime = 0f;
			float blendDuration = 14f;
			float timeToNextJitter = 0f;
			float progression = 0f;
			float totalMLtoApply = 500f;
			if (wi != null)
			{
				for (int j = 0; j < blendResults.Length; j++)
				{
					if (blendResults[j].BlendItem == wi.Data)
					{
						totalMLtoApply *= blendResults[j].FluidAmountMultiplier;
					}
				}
			}
			ContainedFluidAmount containedFluidAmount = rb.GetComponent<ContainedFluidAmount>();
			if (containedFluidAmount != null)
			{
				totalMLtoApply *= 0.5f;
			}
			while (blendTime < blendDuration)
			{
				float delta = Time.deltaTime * (0.05f + Mathf.Lerp(0f, 50f, Mathf.InverseLerp(200f, 1000f, currentSpeed)));
				if (timeToNextJitter <= 0f)
				{
					float progress = blendTime / blendDuration;
					tr.position = killAxis.TransformPoint(Vector3.Lerp(startPosRelToKillAxis, targetPosRelToKillAxis, progress)) + new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f)) * 0.005f;
					tr.localScale = startScale * Mathf.Lerp(1f, 0.6f, progress);
					tr.Rotate(new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f)) * 10f);
					timeToNextJitter = 0.02f;
				}
				else
				{
					timeToNextJitter -= delta;
				}
				blendTime += delta;
				if (!isBlending)
				{
					blendTime = blendDuration;
				}
				float lastProgression = progression;
				progression = blendTime / blendDuration;
				if (wi != null)
				{
					fluidParticleCollectionZone.ApplyParticleQuantity(wi.Data, totalMLtoApply * progression - totalMLtoApply * lastProgression, temperature);
				}
				if (containedFluidAmount != null)
				{
					for (int i = 0; i < containedFluidAmount.ContainedFluids.Length; i++)
					{
						ContainedFluidAmountInfo info = containedFluidAmount.ContainedFluids[i];
						fluidParticleCollectionZone.ApplyParticleQuantity(info.worldItem, info.amountML * progression - info.amountML * lastProgression, temperature);
					}
				}
				yield return null;
			}
			if (wi != null)
			{
				if (OnItemWasBlended != null)
				{
					OnItemWasBlended(wi.Data);
				}
				GameEventsManager.Instance.ItemActionOccurred(wi.Data, "DESTROYED");
			}
			blendingItems.Remove(rb);
			UnityEngine.Object.Destroy(rb.gameObject);
		}
		else
		{
			Debug.LogWarning("Tried to blend rigidbody without WorldItem: " + rb.gameObject.name + ", aborting.");
		}
	}

	public void DisconnectFromBase(BlenderBaseController baseController)
	{
		if (connectedBase != null)
		{
			BlenderBaseController blenderBaseController = connectedBase;
			blenderBaseController.OnTurnedOn = (Action<BlenderBaseController>)Delegate.Remove(blenderBaseController.OnTurnedOn, new Action<BlenderBaseController>(BaseTurnedOn));
			BlenderBaseController blenderBaseController2 = connectedBase;
			blenderBaseController2.OnTurnedOff = (Action<BlenderBaseController>)Delegate.Remove(blenderBaseController2.OnTurnedOff, new Action<BlenderBaseController>(BaseTurnedOff));
		}
		StopBlending();
		connectedBase = null;
	}

	public void ConnectToBase(BlenderBaseController baseController)
	{
		connectedBase = baseController;
		BlenderBaseController blenderBaseController = connectedBase;
		blenderBaseController.OnTurnedOn = (Action<BlenderBaseController>)Delegate.Combine(blenderBaseController.OnTurnedOn, new Action<BlenderBaseController>(BaseTurnedOn));
		BlenderBaseController blenderBaseController2 = connectedBase;
		blenderBaseController2.OnTurnedOff = (Action<BlenderBaseController>)Delegate.Combine(blenderBaseController2.OnTurnedOff, new Action<BlenderBaseController>(BaseTurnedOff));
		if (baseController.IsOn)
		{
			StartBlending();
		}
	}

	private void BaseTurnedOn(BlenderBaseController b)
	{
		StartBlending();
	}

	private void BaseTurnedOff(BlenderBaseController b)
	{
		StopBlending();
	}

	private void HandEnteredShredRegion(PlayerPartDetector partDetector, InteractionHandController hand)
	{
		if (hand == GlobalStorage.Instance.MasterHMDAndInputController.RightHand)
		{
			hand.HapticsController.AddNewHaptic(rightHandShredHaptics);
		}
		else if (hand == GlobalStorage.Instance.MasterHMDAndInputController.LeftHand)
		{
			hand.HapticsController.AddNewHaptic(leftHandShredHaptics);
		}
	}

	private void HandExitedShredRegion(PlayerPartDetector partDetector, InteractionHandController hand)
	{
		if (hand == GlobalStorage.Instance.MasterHMDAndInputController.RightHand)
		{
			hand.HapticsController.RemoveHaptic(rightHandShredHaptics);
		}
		else if (hand == GlobalStorage.Instance.MasterHMDAndInputController.LeftHand)
		{
			hand.HapticsController.RemoveHaptic(leftHandShredHaptics);
		}
	}

	public void StartBlending()
	{
		isBlending = true;
		if (blenderStartSound != null)
		{
			audioSource.SetLooping(false);
			audioSource.SetClip(blenderStartSound);
			audioSource.Play();
		}
		for (int i = 0; i < blendZone.ActiveRigidbodiesTriggerInfo.Count; i++)
		{
			RigidbodyTriggerInfo rigidbodyTriggerInfo = blendZone.ActiveRigidbodiesTriggerInfo[i];
			StartCoroutine(InternalBlend(rigidbodyTriggerInfo.Rigidbody));
		}
	}

	public void StopBlending()
	{
		isBlending = false;
		UpdateSpinSpeed(0f, 0f);
		if (blenderStopSound != null)
		{
			audioSource.SetPitch(1f);
			audioSource.SetVolume(1f);
			audioSource.SetLooping(false);
			audioSource.SetClip(blenderStopSound);
			audioSource.Play();
		}
	}

	private void ItemDroppedIntoBlender(Rigidbody rb)
	{
		if (isBlending)
		{
			StartCoroutine(InternalBlend(rb));
		}
		else if (rb.GetComponentInParent<InteractionHandController>() == null)
		{
			audioSource.SetLooping(false);
			if (fluidContainer.FluidFullPercent < 0.8f)
			{
				audioSource.SetClip(itemDropSounds[UnityEngine.Random.Range(0, itemDropSounds.Length)]);
			}
			else
			{
				audioSource.SetClip(plopSound);
			}
			audioSource.Play();
		}
	}

	public void UpdateSpinSpeed(float speed, float powerRatio)
	{
		currentSpeed = speed;
		float currPulseRateMicroSec = speed * handShredHapticsSpeedMultiplier;
		rightHandShredHaptics.SetCurrPulseRateMicroSec(currPulseRateMicroSec);
		leftHandShredHaptics.SetCurrPulseRateMicroSec(currPulseRateMicroSec);
		if (powerRatio >= 0.9f)
		{
			spillPFX.startColor = fluidParticleCollectionZone.CalculateCombinedFluidColor();
			spillPFX.emissionRate = Mathf.Lerp(10f, 100f, (powerRatio - 0.9f) / 0.100000024f);
			isSpilling = true;
		}
		else
		{
			spillPFX.emissionRate = 0f;
			isSpilling = false;
		}
	}

	private void Update()
	{
		if (currentSpeed != 0f)
		{
			audioSource.SetVolume(100f * currentSpeed / 1000f * 0.01f);
			audioSource.SetPitch(100f * currentSpeed / 1000f * 0.01f);
			if (!audioSource.IsPlaying && blenderLoopSound != null)
			{
				audioSource.SetLooping(true);
				audioSource.SetClip(blenderLoopSound);
				audioSource.Play();
			}
			spinnerTransform.Rotate(Vector3.up * currentSpeed * Time.deltaTime);
			if (isSpilling)
			{
				fluidContainer.ManualPourLiquid(Time.deltaTime * spillMLperSec, base.transform.position + Vector3.down * 50f);
			}
		}
		shakePhase += Time.deltaTime * currentSpeed / 360f * shakeSpeedMultiplier;
		base.transform.localEulerAngles = new Vector3(shakeSinusoid.Evaluate(shakePhase), 0f, shakeSinusoid.Evaluate(shakePhase - 0.25f)) * currentSpeed * shakeAngleMultipler;
	}
}
