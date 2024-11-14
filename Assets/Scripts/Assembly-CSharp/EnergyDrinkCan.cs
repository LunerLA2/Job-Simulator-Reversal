using System;
using OwlchemyVR;
using UnityEngine;

[RequireComponent(typeof(ShakeController))]
public class EnergyDrinkCan : GravityDispensingItem
{
	[SerializeField]
	[Header("References")]
	private Renderer canMeshToSwapMaterial;

	[SerializeField]
	private Material shakenMaterial;

	[SerializeField]
	private WorldItemData maxEnergyFluid;

	[SerializeField]
	private ParticleSystem fxSpray;

	[SerializeField]
	private AttachablePoint capAttachPoint;

	[SerializeField]
	private PickupableItem pickupableItem;

	[SerializeField]
	[Header("Audio")]
	private AudioSourceHelper audioSourceHelper;

	[SerializeField]
	private AudioSourceHelper audioHum;

	[SerializeField]
	private AudioClip[] shakeClip;

	[SerializeField]
	private AudioClip humClip;

	[SerializeField]
	private AudioClip pourClip;

	private ShakeController shaker;

	private ContainedFluidAmount containedFluidAmount;

	private Renderer tabMeshToSwapMaterial;

	private bool shakeComplete;

	private float lastProgress;

	private void Awake()
	{
		shaker = GetComponent<ShakeController>();
		containedFluidAmount = GetComponent<ContainedFluidAmount>();
		audioSourceHelper.enabled = false;
		audioHum.enabled = false;
		StopDispensingLogic();
		GravityDispensingItemAwake();
	}

	private void OnEnable()
	{
		if (!shakeComplete)
		{
			shaker.onShakeComplete.AddListener(ShakeCompleted);
			shaker.onShakeProgress.AddListener(OnShakeProgress);
		}
		PickupableItem obj = pickupableItem;
		obj.OnGrabbedUpdate = (Action<GrabbableItem>)Delegate.Combine(obj.OnGrabbedUpdate, new Action<GrabbableItem>(OnGrabbedUpdate));
		PickupableItem obj2 = pickupableItem;
		obj2.OnReleased = (Action<GrabbableItem>)Delegate.Combine(obj2.OnReleased, new Action<GrabbableItem>(OnDroppedItem));
		capAttachPoint.OnObjectWasAttached += CapAttached;
		capAttachPoint.OnObjectWasDetached += CapDetached;
		GravityDispensingItemOnEnable();
	}

	private void OnDisable()
	{
		if (!shakeComplete)
		{
			shaker.onShakeComplete.RemoveListener(ShakeCompleted);
			shaker.onShakeProgress.RemoveListener(OnShakeProgress);
		}
		PickupableItem obj = pickupableItem;
		obj.OnGrabbedUpdate = (Action<GrabbableItem>)Delegate.Remove(obj.OnGrabbedUpdate, new Action<GrabbableItem>(OnGrabbedUpdate));
		PickupableItem obj2 = pickupableItem;
		obj2.OnReleased = (Action<GrabbableItem>)Delegate.Remove(obj2.OnReleased, new Action<GrabbableItem>(OnDroppedItem));
		capAttachPoint.OnObjectWasAttached -= CapAttached;
		capAttachPoint.OnObjectWasDetached -= CapDetached;
		GravityDispensingItemOnDisable();
	}

	private void OnShakeProgress(ShakeController shakeController, float progress)
	{
		float num = progress - lastProgress;
		if (!audioSourceHelper.IsPlaying && num > 0.01f)
		{
			audioSourceHelper.enabled = true;
			audioSourceHelper.SetClip(shakeClip[UnityEngine.Random.Range(0, shakeClip.Length)]);
			audioSourceHelper.Play();
		}
		else if (audioSourceHelper.IsPlaying && num < -0.01f)
		{
			audioSourceHelper.Stop();
			audioSourceHelper.enabled = false;
		}
		lastProgress = progress;
	}

	private void OnGrabbedUpdate(GrabbableItem grabbableItem)
	{
		if (shakeComplete && !audioHum.IsPlaying && (bool)humClip)
		{
			audioHum.SetClip(humClip);
			audioHum.enabled = true;
			audioHum.Play();
		}
	}

	private void OnDroppedItem(GrabbableItem grabbableItem)
	{
		if (audioSourceHelper.IsPlaying)
		{
			audioSourceHelper.Stop();
			audioSourceHelper.enabled = false;
		}
		if (audioHum.IsPlaying)
		{
			audioHum.Stop();
			audioHum.enabled = false;
		}
	}

	private void CapAttached(AttachablePoint point, AttachableObject o)
	{
		capAttachPoint.enabled = false;
		EnableShake();
	}

	private void CapDetached(AttachablePoint point, AttachableObject o)
	{
		if (pickupableItem.CurrInteractableHand != null)
		{
			pickupableItem.CurrInteractableHand.UnIgnoreOtherHandGrabbleCollidersAndClearInOtherHand();
		}
		o.gameObject.AddComponent<CapDespawner>();
		capAttachPoint.enabled = false;
		if (pickupableItem.CurrInteractableHand != null)
		{
			pickupableItem.CurrInteractableHand.PhysicsIgnoreGrabbableInTheOtherHand();
		}
		DisableShake();
	}

	private void DisableShake()
	{
		if (shaker.shakeProgress < 1f)
		{
			shaker.StopHaptics();
		}
		shaker.enabled = false;
	}

	private void EnableShake()
	{
		shaker.enabled = true;
		capAttachPoint.enabled = true;
	}

	private void ShakeCompleted(ShakeController shaker)
	{
		shakeComplete = true;
		audioSourceHelper.Stop();
		if ((bool)humClip)
		{
			audioHum.SetClip(humClip);
			audioHum.Play();
		}
		canMeshToSwapMaterial.material = shakenMaterial;
		if (capAttachPoint.GetAttachedObject(0) != null)
		{
			tabMeshToSwapMaterial = capAttachPoint.GetAttachedObject(0).GetComponentInChildren<Renderer>();
			tabMeshToSwapMaterial.material = shakenMaterial;
		}
		SetFluidToDispense(maxEnergyFluid);
		containedFluidAmount.ContainedFluids[0].worldItem = maxEnergyFluid;
		fxSpray.startColor = Color.red;
	}

	protected override void DoDispensing()
	{
		DoDispensingLogic();
		if (shakeComplete)
		{
			pouringAudioSrc.enabled = true;
			if (pouringAudioSrc.GetClip() == humClip)
			{
				pouringAudioSrc.SetClip(pourClip);
				pouringAudioSrc.Play();
			}
		}
		else if (!pouringAudioSrc.IsPlaying)
		{
			PlayAudio();
		}
	}

	protected override void StopDispensing()
	{
		StopDispensingLogic();
		if (shakeComplete)
		{
			pouringAudioSrc.enabled = true;
			if (pouringAudioSrc.GetClip() == pourClip)
			{
				pouringAudioSrc.SetClip(humClip);
				pouringAudioSrc.Play();
			}
		}
		else if (pouringAudioSrc.IsPlaying)
		{
			StopAudio();
		}
	}
}
