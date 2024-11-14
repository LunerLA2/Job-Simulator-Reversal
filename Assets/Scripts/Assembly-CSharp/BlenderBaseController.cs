using System;
using OwlchemyVR;
using UnityEngine;

public class BlenderBaseController : KitchenTool
{
	public const float REQUIRED_POWER_RATIO_BEFORE_ON = 0.06f;

	public const float SPIN_SPEED_AT_MIN_POWER = 200f;

	public const float SPIN_SPEED_AT_MAX_POWER = 1000f;

	public const float TAP_MIN_POUR_SPEED = 10f;

	public const float TAP_MAX_POUR_SPEED = 500f;

	[SerializeField]
	private WorldItem myWorldItem;

	[SerializeField]
	private GrabbableItem leverGrab;

	[SerializeField]
	private GrabbableHinge leverHinge;

	[SerializeField]
	private Transform leverTransform;

	[SerializeField]
	private Transform spedometerTransform;

	[SerializeField]
	private GrabbableHinge spoutHinge;

	[SerializeField]
	private ContainerFluidSystem blenderFluidContainer;

	[SerializeField]
	private Transform spoutDispenser;

	[SerializeField]
	private Transform baseSpinnerTransform;

	[SerializeField]
	private Rigidbody[] rigidbodiesToMakeKinematicWhenStowed;

	[SerializeField]
	private GrabbableItem[] grabbablesToDisableWhenStowed;

	private HapticInfoObject hapticInfoObject;

	private bool isHapticInProgress;

	[SerializeField]
	private BlenderBodyController blenderBodyController;

	[SerializeField]
	private AudioSourceHelper dispenserAudioSource;

	private float lastPower;

	public Action<BlenderBaseController> OnTurnedOn;

	public Action<BlenderBaseController> OnTurnedOff;

	public bool IsOn
	{
		get
		{
			return lastPower > 0.06f;
		}
	}

	private void Awake()
	{
		hapticInfoObject = new HapticInfoObject(0f);
		blenderBodyController.ConnectToBase(this);
	}

	private void OnEnable()
	{
		GrabbableItem grabbableItem = leverGrab;
		grabbableItem.OnGrabbed = (Action<GrabbableItem>)Delegate.Combine(grabbableItem.OnGrabbed, new Action<GrabbableItem>(ItemGrabbed));
		GrabbableItem grabbableItem2 = leverGrab;
		grabbableItem2.OnReleased = (Action<GrabbableItem>)Delegate.Combine(grabbableItem2.OnReleased, new Action<GrabbableItem>(ItemReleased));
		BlenderBodyController obj = blenderBodyController;
		obj.OnItemWasBlended = (Action<WorldItemData>)Delegate.Combine(obj.OnItemWasBlended, new Action<WorldItemData>(ItemWasBlended));
	}

	private void OnDisable()
	{
		GrabbableItem grabbableItem = leverGrab;
		grabbableItem.OnGrabbed = (Action<GrabbableItem>)Delegate.Remove(grabbableItem.OnGrabbed, new Action<GrabbableItem>(ItemGrabbed));
		GrabbableItem grabbableItem2 = leverGrab;
		grabbableItem2.OnReleased = (Action<GrabbableItem>)Delegate.Remove(grabbableItem2.OnReleased, new Action<GrabbableItem>(ItemReleased));
		BlenderBodyController obj = blenderBodyController;
		obj.OnItemWasBlended = (Action<WorldItemData>)Delegate.Remove(obj.OnItemWasBlended, new Action<WorldItemData>(ItemWasBlended));
	}

	private void ItemGrabbed(GrabbableItem grabbableItem)
	{
		if (lastPower >= 0.06f)
		{
			UpdateHapticValueBasedOnAmount();
			isHapticInProgress = true;
			leverGrab.CurrInteractableHand.HapticsController.AddNewHaptic(hapticInfoObject);
		}
	}

	private void ItemReleased(GrabbableItem grabbableItem)
	{
		if (isHapticInProgress)
		{
			isHapticInProgress = false;
			leverGrab.CurrInteractableHand.HapticsController.RemoveHaptic(hapticInfoObject);
		}
	}

	private void UpdateHapticValueBasedOnAmount()
	{
		float num = (lastPower - 0.06f) * 1000f;
		if (num < 0.06f)
		{
			num = 0f;
		}
		hapticInfoObject.SetCurrPulseRateMicroSec(num);
	}

	public override void OnDismiss()
	{
		base.OnDismiss();
		SetStowedState(true);
		spoutHinge.transform.localPosition = Vector3.zero;
		spoutHinge.transform.localRotation = Quaternion.identity;
	}

	public override void OnSummon()
	{
		base.OnSummon();
		spoutHinge.transform.localPosition = Vector3.zero;
		spoutHinge.transform.localRotation = Quaternion.identity;
		SetStowedState(false);
	}

	private void SetStowedState(bool state)
	{
		for (int i = 0; i < rigidbodiesToMakeKinematicWhenStowed.Length; i++)
		{
			rigidbodiesToMakeKinematicWhenStowed[i].isKinematic = state;
		}
		for (int j = 0; j < grabbablesToDisableWhenStowed.Length; j++)
		{
			grabbablesToDisableWhenStowed[j].enabled = !state;
		}
	}

	private void Update()
	{
		float num = 0f;
		if (Mathf.Abs(spoutHinge.NormalizedAngle) > 0.25f)
		{
			num = Mathf.Lerp(10f, 500f, Mathf.InverseLerp(0.25f, 1f, Mathf.Abs(spoutHinge.NormalizedAngle)));
		}
		if (num > 0f && !blenderBodyController.IsBlending && blenderFluidContainer.FluidFullPercent > 0f)
		{
			if (!dispenserAudioSource.IsPlaying)
			{
				dispenserAudioSource.Play();
			}
			blenderFluidContainer.ManualPourLiquid(num * Time.deltaTime, spoutDispenser.position);
		}
		else if (dispenserAudioSource.IsPlaying)
		{
			dispenserAudioSource.Stop();
		}
		float normalizedAngle = leverHinge.NormalizedAngle;
		if (lastPower >= 0.06f)
		{
			spedometerTransform.localEulerAngles = new Vector3(spedometerTransform.localEulerAngles.x, spedometerTransform.localEulerAngles.y, Mathf.Clamp(Mathf.InverseLerp(0.06f, 0.94f, lastPower), 0f, 1f) * 180f);
		}
		else
		{
			spedometerTransform.localEulerAngles = new Vector3(spedometerTransform.localEulerAngles.x, spedometerTransform.localEulerAngles.y, 0f);
		}
		SpinGraphics(normalizedAngle);
		if (lastPower < 0.06f && normalizedAngle >= 0.06f)
		{
			isToolBusy = true;
			if (OnTurnedOn != null)
			{
				OnTurnedOn(this);
			}
			GameEventsManager.Instance.ItemActionOccurred(myWorldItem.Data, "ACTIVATED");
		}
		if (lastPower >= 0.06f && normalizedAngle < 0.06f)
		{
			isToolBusy = false;
			if (OnTurnedOff != null)
			{
				OnTurnedOff(this);
			}
			GameEventsManager.Instance.ItemActionOccurred(myWorldItem.Data, "DEACTIVATED");
		}
		if (leverGrab.IsCurrInHand)
		{
			if (!isHapticInProgress)
			{
				if (lastPower >= 0.06f)
				{
					isHapticInProgress = true;
					UpdateHapticValueBasedOnAmount();
					leverGrab.CurrInteractableHand.HapticsController.AddNewHaptic(hapticInfoObject);
				}
			}
			else
			{
				UpdateHapticValueBasedOnAmount();
			}
		}
		lastPower = normalizedAngle;
	}

	private void ItemWasBlended(WorldItemData item)
	{
		GameEventsManager.Instance.ItemActionOccurred(myWorldItem.Data, "USED");
		GameEventsManager.Instance.ItemAppliedToItemActionOccurred(myWorldItem.Data, item, "REMOVED_FROM");
	}

	private void SpinGraphics(float powerRatio)
	{
		float num = Mathf.Lerp(200f, 1000f, powerRatio);
		if (powerRatio <= 0.06f)
		{
			num = 0f;
		}
		if (num > 0f)
		{
			baseSpinnerTransform.Rotate(Vector3.up * num * Time.deltaTime);
			if (blenderBodyController != null)
			{
				blenderBodyController.UpdateSpinSpeed(num, powerRatio);
			}
		}
	}
}
