using System;
using System.Collections;
using OwlchemyVR;
using UnityEngine;

public class DastardlyLauncherController : MonoBehaviour
{
	[SerializeField]
	private WorldItem myWorldItem;

	[SerializeField]
	private GrabbableItem trunkGrabbable;

	[SerializeField]
	private Transform aimTransform;

	[SerializeField]
	private Transform cannonTransform;

	[SerializeField]
	private Transform launchPositionTransform;

	[SerializeField]
	private ItemCollectionZone funnelCollectionZone;

	[SerializeField]
	private float launchForce = 750f;

	[SerializeField]
	private ParticleImpactZone fluidImpactZone;

	[SerializeField]
	private ParticleSystem fluidParticle;

	[SerializeField]
	private float launcherDelay = 0.75f;

	private Vector3 currentRotation;

	private float particleEmissionEndTime;

	private float particleDuration = 0.75f;

	private void OnEnable()
	{
		GrabbableItem grabbableItem = trunkGrabbable;
		grabbableItem.OnGrabbed = (Action<GrabbableItem>)Delegate.Combine(grabbableItem.OnGrabbed, new Action<GrabbableItem>(ResetAimTransform));
		ItemCollectionZone itemCollectionZone = funnelCollectionZone;
		itemCollectionZone.OnItemsInCollectionAdded = (Action<ItemCollectionZone, PickupableItem>)Delegate.Combine(itemCollectionZone.OnItemsInCollectionAdded, new Action<ItemCollectionZone, PickupableItem>(LaunchItem));
		ParticleImpactZone particleImpactZone = fluidImpactZone;
		particleImpactZone.OnSpecificParticleAppliedUpdate = (Action<ParticleImpactZone, WorldItemData, Vector3>)Delegate.Combine(particleImpactZone.OnSpecificParticleAppliedUpdate, new Action<ParticleImpactZone, WorldItemData, Vector3>(FluidPouredInFunnel));
	}

	private void OnDisable()
	{
		GrabbableItem grabbableItem = trunkGrabbable;
		grabbableItem.OnGrabbed = (Action<GrabbableItem>)Delegate.Remove(grabbableItem.OnGrabbed, new Action<GrabbableItem>(ResetAimTransform));
		ItemCollectionZone itemCollectionZone = funnelCollectionZone;
		itemCollectionZone.OnItemsInCollectionAdded = (Action<ItemCollectionZone, PickupableItem>)Delegate.Remove(itemCollectionZone.OnItemsInCollectionAdded, new Action<ItemCollectionZone, PickupableItem>(LaunchItem));
		ParticleImpactZone particleImpactZone = fluidImpactZone;
		particleImpactZone.OnSpecificParticleAppliedUpdate = (Action<ParticleImpactZone, WorldItemData, Vector3>)Delegate.Remove(particleImpactZone.OnSpecificParticleAppliedUpdate, new Action<ParticleImpactZone, WorldItemData, Vector3>(FluidPouredInFunnel));
	}

	private void Start()
	{
		ParticleSystem.EmissionModule emission = fluidParticle.emission;
		emission.enabled = false;
	}

	private void Update()
	{
		currentRotation.Set(cannonTransform.localEulerAngles.x, aimTransform.localEulerAngles.y, cannonTransform.localEulerAngles.z);
		cannonTransform.localEulerAngles = currentRotation;
		if (fluidParticle.emission.enabled && particleEmissionEndTime <= Time.time)
		{
			ParticleSystem.EmissionModule emission = fluidParticle.emission;
			emission.enabled = false;
		}
	}

	private void ResetAimTransform(GrabbableItem grabbable)
	{
		aimTransform.localEulerAngles = Vector3.zero;
	}

	private void LaunchItem(ItemCollectionZone collectionZone, PickupableItem item)
	{
		item.gameObject.SetActive(false);
		StartCoroutine(LaunchItemAfterDelay(item));
	}

	private IEnumerator LaunchItemAfterDelay(PickupableItem item)
	{
		yield return new WaitForSeconds(launcherDelay);
		item.gameObject.SetActive(true);
		item.transform.position = launchPositionTransform.position;
		item.Rigidbody.AddForce(launchPositionTransform.forward * launchForce);
		GameEventsManager.Instance.ItemActionOccurred(myWorldItem.Data, "USED");
		WorldItem worldItem = item.GetComponent<WorldItem>();
		if (worldItem != null)
		{
			GameEventsManager.Instance.ItemAppliedToItemActionOccurred(worldItem.Data, myWorldItem.Data, "ADDED_TO");
		}
	}

	private void FluidPouredInFunnel(ParticleImpactZone zone, WorldItemData data, Vector3 position)
	{
		fluidParticle.startColor = data.OverallColor;
		particleEmissionEndTime = Time.time + particleDuration;
		ParticleSystem.EmissionModule emission = fluidParticle.emission;
		emission.enabled = true;
	}
}
