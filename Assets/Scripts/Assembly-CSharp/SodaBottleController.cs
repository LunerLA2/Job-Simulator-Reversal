using System;
using OwlchemyVR;
using UnityEngine;

public class SodaBottleController : MonoBehaviour
{
	[SerializeField]
	private const float capBlastOffSpeed = 3f;

	[SerializeField]
	private GameObject cap;

	[SerializeField]
	private MeshFilter capOutlineMesh;

	[SerializeField]
	private ParticleSystem spray;

	[SerializeField]
	private Transform spoutTransform;

	[SerializeField]
	private MeshRenderer[] visualsRenderers;

	[SerializeField]
	private ParticleSystem[] pfxToTint;

	private PickupableItem item;

	private Vector3 prevPos = Vector3.zero;

	[SerializeField]
	private float minimumVelocityToCharge = 1f;

	private float amountOfCharge;

	private float timeSpentStill;

	[SerializeField]
	private float secondsOfChargingRequired = 3f;

	[SerializeField]
	private float launchForce = 20f;

	private bool isRocketing;

	private bool isHapticInProgress;

	private HapticInfoObject hapticInfoObject;

	[SerializeField]
	private float maxHapticValueBeforeLaunch = 500f;

	[SerializeField]
	private float hapticValueAfterLaunch = 1000f;

	[SerializeField]
	private AudioClip launchingAudioClipLoop;

	[SerializeField]
	private AudioSourceHelper audioSrcHelper;

	[SerializeField]
	private Collider[] collidersToSwapMaterials;

	[SerializeField]
	private PhysicMaterial physicsMatRocket;

	[SerializeField]
	private Transform fluidContainer;

	[SerializeField]
	private float maxFlightTime = 20f;

	private float currFlightTime;

	private bool isEmpty;

	private void Awake()
	{
		item = GetComponent<PickupableItem>();
		hapticInfoObject = new HapticInfoObject(0f);
	}

	private void OnEnable()
	{
		PickupableItem pickupableItem = item;
		pickupableItem.OnGrabbed = (Action<GrabbableItem>)Delegate.Combine(pickupableItem.OnGrabbed, new Action<GrabbableItem>(ItemGrabbed));
		PickupableItem pickupableItem2 = item;
		pickupableItem2.OnReleased = (Action<GrabbableItem>)Delegate.Combine(pickupableItem2.OnReleased, new Action<GrabbableItem>(ItemReleased));
	}

	private void OnDisable()
	{
		PickupableItem pickupableItem = item;
		pickupableItem.OnGrabbed = (Action<GrabbableItem>)Delegate.Remove(pickupableItem.OnGrabbed, new Action<GrabbableItem>(ItemGrabbed));
		PickupableItem pickupableItem2 = item;
		pickupableItem2.OnReleased = (Action<GrabbableItem>)Delegate.Remove(pickupableItem2.OnReleased, new Action<GrabbableItem>(ItemReleased));
	}

	public void Setup(Material mat, Color pfxTint)
	{
		for (int i = 0; i < visualsRenderers.Length; i++)
		{
			visualsRenderers[i].material = mat;
		}
		for (int j = 0; j < pfxToTint.Length; j++)
		{
			pfxToTint[j].startColor = new Color(pfxTint.r, pfxTint.g, pfxTint.b, pfxToTint[j].startColor.a);
		}
	}

	private void ItemGrabbed(GrabbableItem grabbableItem)
	{
		if (amountOfCharge > 0f && !isEmpty)
		{
			UpdateHapticValueBasedOnAmount();
			isHapticInProgress = true;
			item.CurrInteractableHand.HapticsController.AddNewHaptic(hapticInfoObject);
		}
	}

	private void ItemReleased(GrabbableItem grabbableItem)
	{
		if (isHapticInProgress)
		{
			isHapticInProgress = false;
			item.CurrInteractableHand.HapticsController.RemoveHaptic(hapticInfoObject);
		}
	}

	private void UpdateHapticValueBasedOnAmount()
	{
		float num = GetPercentCompleteOnAmount() * maxHapticValueBeforeLaunch;
		if (num < 150f)
		{
			num = 0f;
		}
		if (isRocketing)
		{
			num = hapticValueAfterLaunch;
		}
		hapticInfoObject.SetCurrPulseRateMicroSec(num);
	}

	private float GetPercentCompleteOnAmount()
	{
		return amountOfCharge / secondsOfChargingRequired;
	}

	private void Start()
	{
		prevPos = base.transform.position;
	}

	private void Update()
	{
		if (isEmpty)
		{
			return;
		}
		if (item.IsCurrInHand)
		{
			float num = Mathf.Abs(((base.transform.position - prevPos) / Time.deltaTime).magnitude);
			prevPos = base.transform.position;
			if (num >= minimumVelocityToCharge)
			{
				amountOfCharge += Time.deltaTime;
			}
			if (amountOfCharge >= secondsOfChargingRequired)
			{
				amountOfCharge = secondsOfChargingRequired;
				if (!isRocketing)
				{
					BeginLaunch();
				}
			}
			if (!isHapticInProgress)
			{
				if (amountOfCharge > 0f && !isEmpty)
				{
					isHapticInProgress = true;
					UpdateHapticValueBasedOnAmount();
					item.CurrInteractableHand.HapticsController.AddNewHaptic(hapticInfoObject);
				}
			}
			else
			{
				UpdateHapticValueBasedOnAmount();
			}
		}
		else if (isRocketing)
		{
			currFlightTime += Time.deltaTime;
			if (currFlightTime > maxFlightTime)
			{
				currFlightTime = maxFlightTime;
				isRocketing = false;
				isEmpty = true;
				spray.Stop();
				audioSrcHelper.Stop();
			}
			UpdateSodaQuantity();
		}
		prevPos = base.transform.position;
	}

	private void FixedUpdate()
	{
		if (isRocketing && !isEmpty)
		{
			float num = Vector3.Angle(base.transform.up * -1f, Vector3.up);
			Vector3 current = base.transform.up * -1f;
			if (num < 90f)
			{
			}
			current = Vector3.RotateTowards(current, Vector3.up, 0.08726646f, 0f);
			item.Rigidbody.AddForce(current * launchForce * item.Rigidbody.mass, ForceMode.Force);
		}
	}

	private void BeginLaunch()
	{
		if (item.CurrInteractableHand != null)
		{
			item.CurrInteractableHand.UnIgnoreOtherHandGrabbleCollidersAndClearInOtherHand();
		}
		FireOffCap();
		spray.Play();
		isRocketing = true;
		UpdateHapticValueBasedOnAmount();
		for (int i = 0; i < collidersToSwapMaterials.Length; i++)
		{
			collidersToSwapMaterials[i].material = physicsMatRocket;
		}
		audioSrcHelper.SetClip(launchingAudioClipLoop);
		audioSrcHelper.SetLooping(true);
		audioSrcHelper.Play();
		if (item.CurrInteractableHand != null)
		{
			item.CurrInteractableHand.PhysicsIgnoreGrabbableInTheOtherHand();
		}
	}

	private void FireOffCap()
	{
		cap.transform.parent = GlobalStorage.Instance.ContentRoot;
		Rigidbody rigidbody = cap.AddComponent<Rigidbody>();
		rigidbody.mass = 0.4f;
		cap.AddComponent<PickupableItem>();
		SelectedChangeOutlineController selectedChangeOutlineController = cap.AddComponent<SelectedChangeOutlineController>();
		selectedChangeOutlineController.meshFilters = new MeshFilter[1] { capOutlineMesh };
		selectedChangeOutlineController.Build();
		rigidbody.AddRelativeForce(Vector3.up * 3f, ForceMode.Impulse);
	}

	private void UpdateSodaQuantity()
	{
		Vector3 localScale = fluidContainer.localScale;
		localScale.y = 1f - currFlightTime / maxFlightTime;
		if (localScale.y <= 0f)
		{
			localScale.y = 0f;
			fluidContainer.gameObject.SetActive(false);
		}
		else
		{
			fluidContainer.gameObject.SetActive(true);
		}
		fluidContainer.localScale = localScale;
	}
}
