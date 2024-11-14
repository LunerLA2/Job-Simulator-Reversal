using System;
using OwlchemyVR;
using UnityEngine;

[RequireComponent(typeof(PickupableItem))]
public class ShakeController : MonoBehaviour
{
	[Header("Control settings")]
	[Tooltip("Should the shake controller be locked to fully shaken on complete? If true, will stop sending events. Otherwise the shake amount will decrease over time and things will restart like usual")]
	[SerializeField]
	private bool lockOnCompletedShake;

	[Tooltip("If the player isn't actively shaking this object, the shake progress eventually gets back to 0")]
	[SerializeField]
	private bool shakeDiminishesOverTime;

	[Header("Haptics settings")]
	[SerializeField]
	private bool hapticsEnabled = true;

	[SerializeField]
	private float hapticsWhileShaking = 500f;

	[SerializeField]
	private float hapticsWhenFullyShaken = 1200f;

	[Header("Shake settings")]
	[SerializeField]
	private float minimumVelocityToCharge = 1.5f;

	[SerializeField]
	[Tooltip("Total time required while shaking to complete. In seconds.")]
	private float shakeDurationRequired = 1f;

	[Header("Events")]
	public ShakeProgressEvent onShakeProgress;

	public ShakeCompleteEvent onShakeComplete;

	private float _shakeProgress;

	private PickupableItem item;

	private HapticInfoObject hapticInfoObject;

	private bool fullyShaken;

	private bool hapticsAdded;

	private Vector3 prevPos;

	public float shakeProgress
	{
		get
		{
			return _shakeProgress;
		}
		protected set
		{
			_shakeProgress = value;
		}
	}

	private void Awake()
	{
		item = GetComponent<PickupableItem>();
		hapticInfoObject = new HapticInfoObject(0f);
		if (item == null)
		{
			Debug.LogError(string.Concat(this, " is missing pickup item!"), this);
		}
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

	private void ItemGrabbed(GrabbableItem item)
	{
		if (!fullyShaken)
		{
			UpdateHaptics(true);
		}
	}

	private void ItemReleased(GrabbableItem item)
	{
		UpdateHaptics(false);
	}

	private void UpdateHaptics(bool add)
	{
		if (!hapticsEnabled)
		{
			return;
		}
		if (add)
		{
			if (fullyShaken || shakeProgress > 0f)
			{
				float num = ((!fullyShaken) ? (hapticsWhileShaking * shakeProgress) : hapticsWhenFullyShaken);
				if (num < 150f)
				{
					num = 0f;
				}
				hapticInfoObject.SetCurrPulseRateMicroSec(num);
				if (!hapticsAdded)
				{
					item.CurrInteractableHand.HapticsController.AddNewHaptic(hapticInfoObject);
					hapticsAdded = true;
				}
			}
		}
		else
		{
			StopHaptics();
		}
	}

	public void StopHaptics()
	{
		if (hapticsAdded)
		{
			item.CurrInteractableHand.HapticsController.RemoveHaptic(hapticInfoObject);
			hapticsAdded = false;
		}
	}

	private void Start()
	{
		prevPos = base.transform.position;
	}

	private void Update()
	{
		bool flag = false;
		if (item.IsCurrInHand && !fullyShaken)
		{
			float num = Mathf.Abs(((base.transform.position - prevPos) / Time.deltaTime).magnitude);
			if (num >= minimumVelocityToCharge)
			{
				shakeProgress = Mathf.Clamp(shakeProgress + Time.deltaTime / shakeDurationRequired, 0f, 1f);
				flag = true;
			}
			if (shakeProgress > 0f)
			{
				if (shakeProgress >= 1f)
				{
					Shaken();
				}
				else
				{
					onShakeProgress.Invoke(this, shakeProgress);
				}
				UpdateHaptics(true);
			}
		}
		else if (item.IsCurrInHand && fullyShaken)
		{
			UpdateHaptics(false);
		}
		if (!flag && shakeDiminishesOverTime && shakeProgress > 0f)
		{
			if (fullyShaken && !lockOnCompletedShake)
			{
				fullyShaken = false;
			}
			shakeProgress = Mathf.Clamp(shakeProgress - Time.deltaTime / shakeDurationRequired * 0.1f, 0f, 1f);
		}
		prevPos = base.transform.position;
	}

	private void Shaken()
	{
		if (!fullyShaken)
		{
			fullyShaken = true;
			shakeProgress = 1f;
			if (item != null)
			{
				GameEventsManager.Instance.ItemActionOccurred(item.InteractableItem.WorldItemData, "ACTIVATED");
			}
			UpdateHaptics(false);
			onShakeComplete.Invoke(this);
		}
	}

	public void PrintOnShakeProgress(ShakeController shaker, float progress)
	{
		Debug.Log("Shake controller progress: " + progress);
	}

	public void PrintOnShakeCompleted(ShakeController shaker)
	{
		Debug.Log("Shake controller completed");
	}
}
