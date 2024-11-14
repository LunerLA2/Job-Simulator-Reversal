using System;
using System.Collections;
using OwlchemyVR;
using UnityEngine;

public class RomanCandleController : MonoBehaviour
{
	[SerializeField]
	private AttachablePoint fuseAttachPoint;

	[SerializeField]
	private Transform burstLaunchPoint;

	[SerializeField]
	private GameObject burstPrefab;

	[SerializeField]
	private GameObject psvrBurstPrefab;

	[SerializeField]
	private float numberOfBursts = 5f;

	[SerializeField]
	private float timeBetweenBursts = 2f;

	[SerializeField]
	private ParticleSystem fireEffect;

	[SerializeField]
	private ParticleSystem activeEffect;

	private ObjectPool<GameObject> pool;

	[SerializeField]
	private AudioClip lightSfx;

	[SerializeField]
	private AudioClip shootSfx;

	[SerializeField]
	private AudioSourceHelper audioSourceHelper;

	[SerializeField]
	private PickupableItem item;

	private HapticInfoObject hapticObject = new HapticInfoObject(0f);

	private bool useHaptics;

	private bool hapticsAdded;

	private bool inHand;

	private bool usedUp;

	private void OnEnable()
	{
		fuseAttachPoint.OnObjectWasDetached += OnFuseDetach;
		PickupableItem pickupableItem = item;
		pickupableItem.OnGrabbed = (Action<GrabbableItem>)Delegate.Combine(pickupableItem.OnGrabbed, new Action<GrabbableItem>(Grabbed));
		PickupableItem pickupableItem2 = item;
		pickupableItem2.OnReleased = (Action<GrabbableItem>)Delegate.Combine(pickupableItem2.OnReleased, new Action<GrabbableItem>(Released));
	}

	private void OnDisable()
	{
		fuseAttachPoint.OnObjectWasDetached -= OnFuseDetach;
		PickupableItem pickupableItem = item;
		pickupableItem.OnGrabbed = (Action<GrabbableItem>)Delegate.Remove(pickupableItem.OnGrabbed, new Action<GrabbableItem>(Grabbed));
		PickupableItem pickupableItem2 = item;
		pickupableItem2.OnReleased = (Action<GrabbableItem>)Delegate.Remove(pickupableItem2.OnReleased, new Action<GrabbableItem>(Released));
	}

	private void Grabbed(GrabbableItem grab)
	{
		if (!usedUp)
		{
			UpdateHaptics(true);
			inHand = true;
		}
	}

	private void Released(GrabbableItem grab)
	{
		UpdateHaptics(false);
		inHand = false;
	}

	private void UpdateHaptics(bool add)
	{
		if (!inHand)
		{
			return;
		}
		if (add)
		{
			if (!hapticsAdded)
			{
				item.CurrInteractableHand.HapticsController.AddNewHaptic(hapticObject);
				hapticsAdded = true;
			}
		}
		else
		{
			StopHaptics();
		}
	}

	private void OnFuseDetach(AttachablePoint point, AttachableObject obj)
	{
		fuseAttachPoint.enabled = false;
		audioSourceHelper.SetClip(lightSfx);
		audioSourceHelper.Play();
		StartCoroutine(FireworkRoutine());
		useHaptics = true;
		hapticObject.SetCurrPulseRateMicroSec(200f);
		UpdateHaptics(true);
	}

	private IEnumerator FireworkRoutine()
	{
		yield return new WaitForSeconds(timeBetweenBursts);
		for (int i = 0; (float)i < numberOfBursts; i++)
		{
			StartCoroutine(BurstHaptics());
			fireEffect.Play();
			audioSourceHelper.SetClip(shootSfx);
			audioSourceHelper.Play();
			pool.Fetch(burstLaunchPoint.position, burstLaunchPoint.rotation);
			yield return new WaitForSeconds(timeBetweenBursts);
		}
		yield return new WaitForSeconds(timeBetweenBursts);
		activeEffect.Stop();
		StopHaptics();
		usedUp = true;
	}

	private IEnumerator BurstHaptics()
	{
		hapticObject.SetCurrPulseRateMicroSec(1500f);
		yield return new WaitForSeconds(0.25f);
		hapticObject.SetCurrPulseRateMicroSec(200f);
	}

	private void Start()
	{
		pool = new ObjectPool<GameObject>(burstPrefab, 5, true, true, GlobalStorage.Instance.ContentRoot, Vector3.zero);
	}

	private void Update()
	{
		if (!usedUp && useHaptics)
		{
			UpdateHaptics(true);
		}
	}

	public void StopHaptics()
	{
		if (hapticsAdded)
		{
			item.CurrInteractableHand.HapticsController.RemoveHaptic(hapticObject);
			hapticsAdded = false;
		}
	}
}
