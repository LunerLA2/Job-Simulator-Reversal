using System;
using OwlchemyVR;
using UnityEngine;

public class TeabagController : MonoBehaviour
{
	[SerializeField]
	private WorldItem myWorldItem;

	[SerializeField]
	private WorldItemData teaFluid;

	[SerializeField]
	private float mlOfFluidToReplacePerSecond = 50f;

	[SerializeField]
	private PickupableItem pickupableItem;

	[SerializeField]
	private WorldItemData[] zonesToKeepPickupableOn;

	private bool pickupableState;

	[SerializeField]
	private WorldItemData[] containerTypesToNotConvertTeaWhileInside;

	private ParticleCollectionZone currentZone;

	private FluidImpactParticleManager.FluidImpactDetails impactDetails;

	private void Awake()
	{
		impactDetails = new FluidImpactParticleManager.FluidImpactDetails();
	}

	private void Update()
	{
		if (currentZone == null)
		{
			pickupableState = true;
			if (pickupableItem.enabled != pickupableState)
			{
				pickupableItem.enabled = pickupableState;
			}
			return;
		}
		pickupableState = false;
		for (int i = 0; i < zonesToKeepPickupableOn.Length; i++)
		{
			if (currentZone.CollectionZoneWorldItem.Data == zonesToKeepPickupableOn[i])
			{
				pickupableState = true;
			}
		}
		if (pickupableItem.enabled != pickupableState)
		{
			pickupableItem.enabled = pickupableState;
		}
		float num = mlOfFluidToReplacePerSecond * Time.deltaTime;
		float totalQuantity = currentZone.GetTotalQuantity();
		float quantityOfFluidOfTypeInContainer = currentZone.GetQuantityOfFluidOfTypeInContainer(teaFluid);
		if (totalQuantity - quantityOfFluidOfTypeInContainer >= num)
		{
			currentZone.RemoveParticleQuantity(num, ref impactDetails);
			currentZone.ApplyParticleQuantity(teaFluid, num, currentZone.AvgTemperatureCelsius);
		}
	}

	private void OnTriggerEnter(Collider c)
	{
		if (c.isTrigger)
		{
			ParticleImpactZonePointer component = c.GetComponent<ParticleImpactZonePointer>();
			if (component != null && component.ParticleImpactZone != null && component.ParticleImpactZone.ParticleCollectionZone != null && Array.IndexOf(containerTypesToNotConvertTeaWhileInside, component.ParticleImpactZone.ParticleCollectionZone.CollectionZoneWorldItem) <= -1)
			{
				currentZone = component.ParticleImpactZone.ParticleCollectionZone;
				GameEventsManager.Instance.ItemAppliedToItemActionOccurred(myWorldItem.Data, currentZone.CollectionZoneWorldItem.Data, "ADDED_TO");
			}
		}
	}

	private void OnTriggerExit(Collider c)
	{
		if (!c.isTrigger)
		{
			return;
		}
		ParticleImpactZonePointer component = c.GetComponent<ParticleImpactZonePointer>();
		if (component != null && component.ParticleImpactZone != null && component.ParticleImpactZone.ParticleCollectionZone != null && component.ParticleImpactZone.ParticleCollectionZone == currentZone)
		{
			if (Array.IndexOf(containerTypesToNotConvertTeaWhileInside, component.ParticleImpactZone.ParticleCollectionZone.CollectionZoneWorldItem) <= -1)
			{
				GameEventsManager.Instance.ItemAppliedToItemActionOccurred(myWorldItem.Data, currentZone.CollectionZoneWorldItem.Data, "REMOVED_FROM");
			}
			currentZone = null;
		}
	}
}
