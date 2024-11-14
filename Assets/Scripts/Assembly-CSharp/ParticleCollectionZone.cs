using System;
using System.Collections.Generic;
using OwlchemyVR;
using UnityEngine;

[RequireComponent(typeof(ParticleImpactZone))]
public class ParticleCollectionZone : MonoBehaviour
{
	private const float HEAT_CAPACITY_INV_FOR_FLUIDS = 0.004f;

	public const int MAX_NUM_OF_PARTICLES_COLLECTED = 20;

	private List<CollectedParticleQuantityInfo> particleQuantities = new List<CollectedParticleQuantityInfo>();

	public Action<ParticleCollectionZone, WorldItemData, float> OnParticleIsCollecting;

	public Action<ParticleCollectionZone> OnParticlesBeingRemoved;

	public Action<ParticleCollectionZone, WorldItemData> OnParticleQuantityUnitAdded;

	[SerializeField]
	private Transform surfaceLocationTransform;

	private Plane surfacePlane;

	[SerializeField]
	private WorldItem collectionZoneWorldItem;

	[SerializeField]
	private ItemCollectionZone containsItemCollectionZone;

	private float totalVolumeCollected;

	private WorldItemData majorityFluidData;

	[SerializeField]
	private TemperatureStateItem temperatureStateItem;

	public WorldItem CollectionZoneWorldItem
	{
		get
		{
			return collectionZoneWorldItem;
		}
	}

	public WorldItemData MajorityFluidData
	{
		get
		{
			return majorityFluidData;
		}
	}

	public float AvgTemperatureCelsius
	{
		get
		{
			if (temperatureStateItem != null)
			{
				return temperatureStateItem.TemperatureCelsius;
			}
			return 21f;
		}
	}

	public TemperatureStateItem TemperatureStateItem
	{
		get
		{
			return temperatureStateItem;
		}
	}

	public ItemCollectionZone ContainsItemCollectionZone
	{
		get
		{
			return containsItemCollectionZone;
		}
	}

	private void Awake()
	{
		if (temperatureStateItem != null)
		{
			temperatureStateItem.SetHeatCapacityInv(0.004f);
		}
	}

	public void SetSurfaceLocationTransform(Transform t)
	{
		surfaceLocationTransform = t;
		if (surfaceLocationTransform != null)
		{
			surfacePlane = new Plane(surfaceLocationTransform.up, surfaceLocationTransform.position);
		}
	}

	public float GetRayIntersectWithPlane(Ray ray)
	{
		float enter = -1f;
		if (surfaceLocationTransform != null)
		{
			surfacePlane.SetNormalAndPosition(surfaceLocationTransform.up, surfaceLocationTransform.position);
			surfacePlane.Raycast(ray, out enter);
		}
		return enter;
	}

	public void ApplyParticleQuantity(WorldItemData worldItemFluidData, float quantity, float temperature)
	{
		if (quantity <= 0f)
		{
			return;
		}
		CollectedParticleQuantityInfo collectedParticleQuantityInfo = FindCollection(worldItemFluidData);
		if (collectedParticleQuantityInfo == null)
		{
			collectedParticleQuantityInfo = new CollectedParticleQuantityInfo(worldItemFluidData);
			if (particleQuantities.Count >= 20)
			{
				RemoveSmallestParticleCollectionAndDistributeAmongstTheOthers();
			}
			particleQuantities.Add(collectedParticleQuantityInfo);
			GameEventsManager.Instance.ItemAppliedToItemActionOccurred(collectedParticleQuantityInfo.FluidData, CollectionZoneWorldItem.Data, "FLUID_ADDED_TO");
		}
		int num = Mathf.FloorToInt(collectedParticleQuantityInfo.Quantity);
		collectedParticleQuantityInfo.SetQuantity(collectedParticleQuantityInfo.Quantity + quantity);
		if (Mathf.FloorToInt(collectedParticleQuantityInfo.Quantity) > num && OnParticleQuantityUnitAdded != null)
		{
			OnParticleQuantityUnitAdded(this, collectedParticleQuantityInfo.FluidData);
		}
		if (OnParticleIsCollecting != null)
		{
			OnParticleIsCollecting(this, collectedParticleQuantityInfo.FluidData, collectedParticleQuantityInfo.Quantity);
		}
		totalVolumeCollected += quantity;
		float num2 = quantity / totalVolumeCollected;
		if (temperatureStateItem != null)
		{
			temperatureStateItem.SetManualTemperature(temperatureStateItem.TemperatureCelsius * (1f - num2) + temperature * num2);
		}
		if (majorityFluidData != worldItemFluidData && collectedParticleQuantityInfo.Quantity > totalVolumeCollected * 0.5f)
		{
			majorityFluidData = worldItemFluidData;
		}
		GameEventsManager.Instance.ItemAppliedToItemActionOccurredWithAmount(worldItemFluidData, CollectionZoneWorldItem.Data, "AMOUNT_CHANGE", collectedParticleQuantityInfo.Quantity);
		GameEventsManager.Instance.ItemActionOccurredWithAmount(CollectionZoneWorldItem.Data, "AMOUNT_CHANGE_ANY_FLUID", GetTotalQuantity());
	}

	private void RemoveSmallestParticleCollectionAndDistributeAmongstTheOthers()
	{
		if (particleQuantities.Count <= 1)
		{
			return;
		}
		float num = float.PositiveInfinity;
		int num2 = -1;
		float num3 = 0f;
		for (int i = 0; i < particleQuantities.Count; i++)
		{
			if (particleQuantities[i].Quantity < num)
			{
				num2 = i;
				num = particleQuantities[i].Quantity;
			}
		}
		if (num2 >= 0)
		{
			CollectedParticleQuantityInfo collectedParticleQuantityInfo = particleQuantities[num2];
			collectedParticleQuantityInfo.SetQuantity(0f);
			particleQuantities.RemoveAt(num2);
			GameEventsManager.Instance.ItemAppliedToItemActionOccurredWithAmount(collectedParticleQuantityInfo.FluidData, CollectionZoneWorldItem.Data, "AMOUNT_CHANGE", collectedParticleQuantityInfo.Quantity);
			GameEventsManager.Instance.ItemAppliedToItemActionOccurred(collectedParticleQuantityInfo.FluidData, CollectionZoneWorldItem.Data, "FLUID_REMOVED_FROM");
			num3 = num / (float)particleQuantities.Count;
			for (int j = 0; j < particleQuantities.Count; j++)
			{
				collectedParticleQuantityInfo = particleQuantities[j];
				collectedParticleQuantityInfo.SetQuantity(collectedParticleQuantityInfo.Quantity + num3);
				GameEventsManager.Instance.ItemAppliedToItemActionOccurredWithAmount(collectedParticleQuantityInfo.FluidData, CollectionZoneWorldItem.Data, "AMOUNT_CHANGE", collectedParticleQuantityInfo.Quantity);
			}
			GameEventsManager.Instance.ItemActionOccurredWithAmount(CollectionZoneWorldItem.Data, "AMOUNT_CHANGE_ANY_FLUID", GetTotalQuantity());
		}
		else
		{
			Debug.LogWarning("Unable to remove smallest particle from collection, should not be possible");
		}
	}

	public float GetQuantityOfFluidOfTypeInContainer(WorldItemData worldItemFluidData)
	{
		CollectedParticleQuantityInfo collectedParticleQuantityInfo = FindCollection(worldItemFluidData);
		if (collectedParticleQuantityInfo != null)
		{
			return collectedParticleQuantityInfo.Quantity;
		}
		return 0f;
	}

	private CollectedParticleQuantityInfo FindCollection(WorldItemData worldItemFluidData)
	{
		if (particleQuantities.Count > 0)
		{
			for (int i = 0; i < particleQuantities.Count; i++)
			{
				if (particleQuantities[i].FluidData == worldItemFluidData)
				{
					return particleQuantities[i];
				}
			}
		}
		return null;
	}

	public void RemoveParticleQuantity(float quantity, ref FluidImpactParticleManager.FluidImpactDetails fluidImpactDetails)
	{
		fluidImpactDetails.Clear();
		if (quantity <= 0f || !(totalVolumeCollected > 0f))
		{
			return;
		}
		fluidImpactDetails.avgColor = CalculateCombinedFluidColor();
		fluidImpactDetails.temperatureCelsius = AvgTemperatureCelsius;
		float num = quantity / totalVolumeCollected;
		if (num > 1f)
		{
			num = 1f;
		}
		if (fluidImpactDetails.numOfFluids > 0)
		{
			Debug.LogWarning("FluidImpactDetails Num Of Fluids starts greater than zero:" + fluidImpactDetails.numOfFluids);
		}
		for (int i = 0; i < particleQuantities.Count; i++)
		{
			CollectedParticleQuantityInfo collectedParticleQuantityInfo = particleQuantities[i];
			float num2 = collectedParticleQuantityInfo.Quantity * num;
			collectedParticleQuantityInfo.SetQuantity(collectedParticleQuantityInfo.Quantity - num2);
			fluidImpactDetails.fluidQuantities[fluidImpactDetails.numOfFluids].SetFluidDataAndQuatity(collectedParticleQuantityInfo.FluidData, num2);
			fluidImpactDetails.numOfFluids++;
			fluidImpactDetails.totalAmountOfFluid += num2;
			if (collectedParticleQuantityInfo.Quantity < 0.01f)
			{
				collectedParticleQuantityInfo.SetQuantity(0f);
				particleQuantities.RemoveAt(i);
				i--;
				GameEventsManager.Instance.ItemAppliedToItemActionOccurred(collectedParticleQuantityInfo.FluidData, CollectionZoneWorldItem.Data, "FLUID_REMOVED_FROM");
			}
			GameEventsManager.Instance.ItemAppliedToItemActionOccurredWithAmount(collectedParticleQuantityInfo.FluidData, CollectionZoneWorldItem.Data, "AMOUNT_CHANGE", collectedParticleQuantityInfo.Quantity);
			GameEventsManager.Instance.ItemActionOccurredWithAmount(CollectionZoneWorldItem.Data, "AMOUNT_CHANGE_ANY_FLUID", GetTotalQuantity());
		}
		if (particleQuantities.Count == 0)
		{
			majorityFluidData = null;
		}
		totalVolumeCollected -= quantity;
		if (totalVolumeCollected <= 0f)
		{
		}
		if (OnParticlesBeingRemoved != null)
		{
			OnParticlesBeingRemoved(this);
		}
	}

	public float GetTotalQuantity()
	{
		return totalVolumeCollected;
	}

	public Color CalculateCombinedFluidColor()
	{
		float num = 0f;
		float num2 = 0f;
		float num3 = 0f;
		float num4 = 0f;
		float num5 = 0f;
		for (int i = 0; i < particleQuantities.Count; i++)
		{
			Color overallColor = particleQuantities[i].FluidData.OverallColor;
			float quantity = particleQuantities[i].Quantity;
			float num6 = quantity / totalVolumeCollected;
			float num7 = 1f - overallColor.r;
			float num8 = 1f - overallColor.g;
			float num9 = 1f - overallColor.b;
			float num10 = Mathf.Min(num7, Mathf.Min(num8, num9));
			if ((double)num10 == 1.0)
			{
				num7 = 0f;
				num8 = 0f;
				num9 = 0f;
			}
			else
			{
				num7 = (num7 - num10) / (1f - num10);
				num8 = (num8 - num10) / (1f - num10);
				num9 = (num9 - num10) / (1f - num10);
			}
			num += num7 * num6;
			num2 += num8 * num6;
			num3 += num9 * num6;
			num4 += num10 * num6;
			num5 += overallColor.a * num6;
		}
		return new Color((1f - num) * (1f - num4), (1f - num2) * (1f - num4), (1f - num3) * (1f - num4), num5);
	}

	public void Clear()
	{
		if (particleQuantities.Count > 0)
		{
			for (int i = 0; i < particleQuantities.Count; i++)
			{
				CollectedParticleQuantityInfo collectedParticleQuantityInfo = particleQuantities[i];
				if (collectedParticleQuantityInfo.Quantity > 0f)
				{
					GameEventsManager.Instance.ItemAppliedToItemActionOccurredWithAmount(collectedParticleQuantityInfo.FluidData, CollectionZoneWorldItem.Data, "AMOUNT_CHANGE", 0f);
					GameEventsManager.Instance.ItemAppliedToItemActionOccurred(collectedParticleQuantityInfo.FluidData, CollectionZoneWorldItem.Data, "FLUID_REMOVED_FROM");
					GameEventsManager.Instance.ItemActionOccurredWithAmount(CollectionZoneWorldItem.Data, "AMOUNT_CHANGE_ANY_FLUID", 0f);
				}
			}
		}
		particleQuantities.Clear();
		majorityFluidData = null;
		if (totalVolumeCollected > 0f)
		{
			totalVolumeCollected = 0f;
			if (OnParticlesBeingRemoved != null)
			{
				OnParticlesBeingRemoved(this);
			}
		}
		majorityFluidData = null;
	}
}
