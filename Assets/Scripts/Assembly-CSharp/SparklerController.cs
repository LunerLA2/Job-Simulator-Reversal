using System;
using OwlchemyVR;
using UnityEngine;

public class SparklerController : MonoBehaviour
{
	[SerializeField]
	private PickupableItem pickupableItem;

	[SerializeField]
	private Animation sparklingAnimation;

	[SerializeField]
	private Animation psvrSparklingAnimation;

	private bool isSparkling;

	[SerializeField]
	private AudioSourceHelper audioSourceHelper;

	[SerializeField]
	private AudioClip sparklerLoopSFX;

	[SerializeField]
	private AudioClip sparklerStartSFX;

	private HapticInfoObject hapticInfo = new HapticInfoObject(0f);

	private bool hapticsAdded;

	private void Awake()
	{
		sparklingAnimation.gameObject.SetActive(true);
	}

	private void OnEnable()
	{
		audioSourceHelper.SetClip(sparklerStartSFX);
		PickupableItem obj = pickupableItem;
		obj.OnGrabbed = (Action<GrabbableItem>)Delegate.Combine(obj.OnGrabbed, new Action<GrabbableItem>(BeginSparkling));
		PickupableItem obj2 = pickupableItem;
		obj2.OnReleased = (Action<GrabbableItem>)Delegate.Combine(obj2.OnReleased, new Action<GrabbableItem>(Release));
	}

	private void OnDisable()
	{
		PickupableItem obj = pickupableItem;
		obj.OnGrabbed = (Action<GrabbableItem>)Delegate.Remove(obj.OnGrabbed, new Action<GrabbableItem>(BeginSparkling));
	}

	private void Release(GrabbableItem item)
	{
		UpdateHaptics(false);
	}

	private void BeginSparkling(GrabbableItem item)
	{
		StopHaptics();
		if (!isSparkling)
		{
			audioSourceHelper.Play();
			audioSourceHelper.SetClip(sparklerLoopSFX);
			audioSourceHelper.SetLooping(true);
			sparklingAnimation.Play();
			isSparkling = true;
			audioSourceHelper.Play();
			hapticInfo.SetCurrPulseRateMicroSec(200f);
			UpdateHaptics(true);
			Invoke("StopHaptics", sparklingAnimation.clip.length);
		}
	}

	private void Update()
	{
		if (isSparkling)
		{
			isSparkling = sparklingAnimation.isPlaying;
			if (pickupableItem.IsCurrInHand)
			{
				UpdateHaptics(true);
			}
		}
		else
		{
			audioSourceHelper.Stop();
			UpdateHaptics(false);
		}
	}

	private void UpdateHaptics(bool add)
	{
		if (add)
		{
			if (!hapticsAdded)
			{
				pickupableItem.CurrInteractableHand.HapticsController.AddNewHaptic(hapticInfo);
				hapticsAdded = true;
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
			pickupableItem.CurrInteractableHand.HapticsController.RemoveHaptic(hapticInfo);
			hapticsAdded = false;
		}
	}
}
