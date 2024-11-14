using System;
using System.Collections;
using System.Collections.Generic;
using OwlchemyVR;
using UnityEngine;

public class ShredderController : MonoBehaviour
{
	private const float BLADES_ROTATION_SPEED = 1800f;

	private const float BLADES_ACCELERATION = 5000f;

	private const float BLADES_DECELERATION = 2500f;

	[SerializeField]
	private WorldItem myWorldItem;

	[SerializeField]
	private RigidbodyEnterExitTriggerEvents shredRegion;

	[SerializeField]
	private PlayerPartDetector handShredRegion;

	[SerializeField]
	private Collider shredRegionCollider;

	[SerializeField]
	private RotateAtSpeed bladesRotator;

	[SerializeField]
	private Transform killAxis;

	[SerializeField]
	private ParticleSystem debris;

	[SerializeField]
	private ParticleSystem psvrDebris;

	[SerializeField]
	private float shredDuration;

	[SerializeField]
	private AudioSourceHelper bladesAudio;

	[SerializeField]
	private AudioClip shredSound;

	[SerializeField]
	private ParticleSystem shredSparks;

	[SerializeField]
	private float handShredHapticsPulseRate;

	[SerializeField]
	private Collider[] pullTabColliders;

	[SerializeField]
	private GrabbableItem pullTabGrabbable;

	[SerializeField]
	private Transform pullTabBarrier;

	[SerializeField]
	private LineRenderer stringRenderer;

	[SerializeField]
	private ParticleSystem pullSparks;

	[SerializeField]
	private AudioClip pullSparkSound;

	[SerializeField]
	private HapticTransformInfo pullDistanceHaptics;

	[SerializeField]
	private float activationPullThreshold = 0.15f;

	private bool isOn;

	private Transform pullTabTransform;

	private SpringJoint pullTabJoint;

	private List<PickupableItem> itemsBeingShredded = new List<PickupableItem>();

	private Color defaultDebrisColor;

	private HapticInfoObject rightHandShredHaptics;

	private HapticInfoObject leftHandShredHaptics;

	private Vector3 initialPullTabLocalPos;

	private int cachedPullTabLayer;

	private float pullDistance;

	private HapticInfoObject pullLoopingHaptics;

	[SerializeField]
	private AudioClip toyJobBotScream;

	[SerializeField]
	private WorldItemData toyJobBotWorldItem;

	[SerializeField]
	private AudioClip[] toyJobBotScreamsNight;

	private void Awake()
	{
		pullTabTransform = pullTabGrabbable.transform;
		pullTabJoint = pullTabGrabbable.GetComponent<SpringJoint>();
		rightHandShredHaptics = new HapticInfoObject(handShredHapticsPulseRate);
		leftHandShredHaptics = new HapticInfoObject(handShredHapticsPulseRate);
		pullLoopingHaptics = new HapticInfoObject(0f);
		pullDistanceHaptics.ManualAwake();
		defaultDebrisColor = debris.startColor;
		initialPullTabLocalPos = pullTabTransform.localPosition;
		cachedPullTabLayer = pullTabColliders[0].gameObject.layer;
	}

	private void OnEnable()
	{
		RigidbodyEnterExitTriggerEvents rigidbodyEnterExitTriggerEvents = shredRegion;
		rigidbodyEnterExitTriggerEvents.OnRigidbodyEnterTrigger = (Action<Rigidbody>)Delegate.Combine(rigidbodyEnterExitTriggerEvents.OnRigidbodyEnterTrigger, new Action<Rigidbody>(ObjectEnteredShredRegion));
		PlayerPartDetector playerPartDetector = handShredRegion;
		playerPartDetector.OnHandEntered = (Action<PlayerPartDetector, InteractionHandController>)Delegate.Combine(playerPartDetector.OnHandEntered, new Action<PlayerPartDetector, InteractionHandController>(HandEnteredShredRegion));
		PlayerPartDetector playerPartDetector2 = handShredRegion;
		playerPartDetector2.OnHandExited = (Action<PlayerPartDetector, InteractionHandController>)Delegate.Combine(playerPartDetector2.OnHandExited, new Action<PlayerPartDetector, InteractionHandController>(HandExitedShredRegion));
		GrabbableItem grabbableItem = pullTabGrabbable;
		grabbableItem.OnGrabbed = (Action<GrabbableItem>)Delegate.Combine(grabbableItem.OnGrabbed, new Action<GrabbableItem>(PullTabGrabbed));
		GrabbableItem grabbableItem2 = pullTabGrabbable;
		grabbableItem2.OnReleased = (Action<GrabbableItem>)Delegate.Combine(grabbableItem2.OnReleased, new Action<GrabbableItem>(PullTabReleased));
	}

	private void OnDisable()
	{
		if (isOn)
		{
			TurnOff();
		}
		RigidbodyEnterExitTriggerEvents rigidbodyEnterExitTriggerEvents = shredRegion;
		rigidbodyEnterExitTriggerEvents.OnRigidbodyEnterTrigger = (Action<Rigidbody>)Delegate.Remove(rigidbodyEnterExitTriggerEvents.OnRigidbodyEnterTrigger, new Action<Rigidbody>(ObjectEnteredShredRegion));
		PlayerPartDetector playerPartDetector = handShredRegion;
		playerPartDetector.OnHandEntered = (Action<PlayerPartDetector, InteractionHandController>)Delegate.Remove(playerPartDetector.OnHandEntered, new Action<PlayerPartDetector, InteractionHandController>(HandEnteredShredRegion));
		PlayerPartDetector playerPartDetector2 = handShredRegion;
		playerPartDetector2.OnHandExited = (Action<PlayerPartDetector, InteractionHandController>)Delegate.Remove(playerPartDetector2.OnHandExited, new Action<PlayerPartDetector, InteractionHandController>(HandExitedShredRegion));
		GrabbableItem grabbableItem = pullTabGrabbable;
		grabbableItem.OnGrabbed = (Action<GrabbableItem>)Delegate.Remove(grabbableItem.OnGrabbed, new Action<GrabbableItem>(PullTabGrabbed));
		GrabbableItem grabbableItem2 = pullTabGrabbable;
		grabbableItem2.OnReleased = (Action<GrabbableItem>)Delegate.Remove(grabbableItem2.OnReleased, new Action<GrabbableItem>(PullTabReleased));
	}

	public void TogglePower()
	{
		if (isOn)
		{
			TurnOff();
		}
		else
		{
			TurnOn();
		}
	}

	public void TurnOn()
	{
		if (!isOn)
		{
			GameEventsManager.Instance.ItemActionOccurred(myWorldItem.Data, "ACTIVATED");
		}
		isOn = true;
		bladesAudio.Play();
		pullSparks.Play();
		AudioManager.Instance.Play(pullSparks.transform.position, pullSparkSound, 1f, 1f);
		shredRegionCollider.isTrigger = true;
	}

	public void TurnOff()
	{
		if (isOn)
		{
			GameEventsManager.Instance.ItemActionOccurred(myWorldItem.Data, "DEACTIVATED");
		}
		isOn = false;
		shredRegionCollider.isTrigger = false;
		HandExitedShredRegion(null, GlobalStorage.Instance.MasterHMDAndInputController.LeftHand);
		HandExitedShredRegion(null, GlobalStorage.Instance.MasterHMDAndInputController.RightHand);
	}

	private void Update()
	{
		UpdateStringLength();
		UpdateHaptics();
		UpdatePullTab();
		UpdateStringRenderer();
		UpdateActivation();
		UpdateBlades();
	}

	private void UpdateStringLength()
	{
		pullDistance = (pullTabTransform.localPosition - initialPullTabLocalPos).magnitude;
	}

	private void UpdateHaptics()
	{
		pullDistanceHaptics.ManualUpdate();
		if (pullTabGrabbable.IsCurrInHand)
		{
			float num = Mathf.Clamp(Vector3.Distance(pullTabTransform.position, base.transform.position) / 1f, 0f, 1f) * 600f;
			if (num < 150f)
			{
				num = 0f;
			}
			pullLoopingHaptics.SetCurrPulseRateMicroSec(num);
		}
	}

	private void UpdatePullTab()
	{
		bool flag = !pullTabGrabbable.IsCurrInHand && pullTabBarrier.InverseTransformPoint(pullTabTransform.position).z > 0f;
		int num = ((!flag) ? 17 : cachedPullTabLayer);
		for (int i = 0; i < pullTabColliders.Length; i++)
		{
			if (pullTabColliders[i].gameObject.layer != num)
			{
				pullTabColliders[i].gameObject.layer = num;
			}
		}
		if (pullTabBarrier.InverseTransformPoint(pullTabTransform.position).z <= 0.1f && !pullTabGrabbable.IsCurrInHand && !flag)
		{
			pullTabGrabbable.transform.Translate(pullTabBarrier.forward * Time.deltaTime);
		}
	}

	private void UpdateStringRenderer()
	{
		stringRenderer.SetPosition(0, stringRenderer.transform.position);
		stringRenderer.SetPosition(1, pullTabTransform.TransformPoint(pullTabJoint.anchor));
	}

	private void UpdateActivation()
	{
		if (pullTabGrabbable.IsCurrInHand && pullDistance >= activationPullThreshold)
		{
			pullTabGrabbable.CurrInteractableHand.TryRelease();
			if (!isOn)
			{
				TurnOn();
			}
		}
	}

	private void UpdateBlades()
	{
		if (isOn)
		{
			if (bladesRotator.RotationSpeed.x < 1800f)
			{
				bladesRotator.SetSpeed(new Vector3(Mathf.Min(1800f, bladesRotator.RotationSpeed.x + Time.deltaTime * 5000f), 0f, 0f));
				bladesAudio.SetPitch(Mathf.Lerp(0.25f, 1f, bladesRotator.RotationSpeed.x / 1800f));
				bladesAudio.SetVolume(bladesRotator.RotationSpeed.x / 1800f);
			}
		}
		else if (bladesRotator.RotationSpeed.x > 0f)
		{
			bladesRotator.SetSpeed(new Vector3(Mathf.Max(0f, bladesRotator.RotationSpeed.x - Time.deltaTime * 2500f), 0f, 0f));
			if (bladesRotator.RotationSpeed.x == 0f)
			{
				bladesAudio.Stop();
				return;
			}
			bladesAudio.SetPitch(Mathf.Lerp(0.25f, 1f, bladesRotator.RotationSpeed.x / 1800f));
			bladesAudio.SetVolume(bladesRotator.RotationSpeed.x / 1800f);
		}
	}

	private void PullTabGrabbed(GrabbableItem item)
	{
		item.CurrInteractableHand.HapticsController.AddNewHaptic(pullLoopingHaptics);
	}

	private void PullTabReleased(GrabbableItem item)
	{
		item.CurrInteractableHand.HapticsController.RemoveHaptic(pullLoopingHaptics);
	}

	private void ObjectEnteredShredRegion(Rigidbody rb)
	{
		if (isOn)
		{
			PickupableItem componentInParent = rb.GetComponentInParent<PickupableItem>();
			if (componentInParent != null && !itemsBeingShredded.Contains(componentInParent))
			{
				StartCoroutine(ShredAsync(componentInParent));
			}
		}
	}

	private void HandEnteredShredRegion(PlayerPartDetector partDetector, InteractionHandController hand)
	{
		if (isOn)
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

	private IEnumerator ShredAsync(PickupableItem pickupable)
	{
		if (!shredSparks.isPlaying)
		{
			shredSparks.Play();
		}
		itemsBeingShredded.Add(pickupable);
		if (pickupable.IsCurrInHand)
		{
			pickupable.CurrInteractableHand.TryRelease();
		}
		pickupable.enabled = false;
		Rigidbody[] componentsInChildren = pickupable.GetComponentsInChildren<Rigidbody>();
		foreach (Rigidbody rb in componentsInChildren)
		{
			rb.isKinematic = true;
		}
		Transform tr = pickupable.transform;
		WorldItemData worldItemData = pickupable.InteractableItem.WorldItemData;
		if (worldItemData == toyJobBotWorldItem)
		{
			AudioClip scream = toyJobBotScream;
			if (JobBoardManager.instance != null && JobBoardManager.instance.EndlessModeStatusController != null && toyJobBotScreamsNight != null && toyJobBotScreamsNight.Length > 0)
			{
				scream = toyJobBotScreamsNight[UnityEngine.Random.Range(0, toyJobBotScreamsNight.Length)];
			}
			AudioManager.Instance.Play(bladesAudio.transform, scream, 1f, 1f);
			AchievementManager.CompleteAchievement(7);
		}
		if (worldItemData != null)
		{
			Color debrisColor = worldItemData.OverallColor;
			if (debrisColor.a == 0f)
			{
				debrisColor = defaultDebrisColor;
			}
			debris.startColor = debrisColor;
		}
		else
		{
			debris.startColor = defaultDebrisColor;
		}
		debris.Play();
		if (shredSound != null)
		{
			AudioManager.Instance.Play(bladesAudio.transform, shredSound, 1f, 1f);
		}
		Vector3 startPosRelToKillAxis = killAxis.InverseTransformPoint(tr.position);
		Vector3 targetPosRelToKillAxis = new Vector3(startPosRelToKillAxis.x, 0f, 0f);
		float shredTime = 0f;
		float timeToNextJitter = 0f;
		while (shredTime < shredDuration)
		{
			if (timeToNextJitter <= 0f)
			{
				float progress = shredTime / shredDuration;
				tr.position = killAxis.TransformPoint(Vector3.Lerp(startPosRelToKillAxis, targetPosRelToKillAxis, progress)) + new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f)) * 0.005f;
				tr.localScale = Vector3.one * Mathf.Lerp(1f, 0.6f, progress);
				tr.Rotate(new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f)) * 10f);
				timeToNextJitter = 0.02f;
			}
			else
			{
				timeToNextJitter -= Time.deltaTime;
			}
			shredTime += Time.deltaTime * ((!isOn) ? 2f : 1f);
			yield return null;
		}
		itemsBeingShredded.Remove(pickupable);
		if (itemsBeingShredded.Count == 0 && shredSparks.isPlaying)
		{
			shredSparks.Stop();
		}
		GameEventsManager.Instance.ItemActionOccurred(myWorldItem.Data, "USED");
		WorldItem worldItem = pickupable.GetComponent<WorldItem>();
		if (worldItem != null)
		{
			GameEventsManager.Instance.ItemAppliedToItemActionOccurred(worldItem.Data, myWorldItem.Data, "ADDED_TO");
			GameEventsManager.Instance.ItemActionOccurred(worldItem.Data, "DESTROYED");
		}
		UnityEngine.Object.Destroy(pickupable.gameObject);
	}
}
