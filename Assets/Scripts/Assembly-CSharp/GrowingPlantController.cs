using System;
using OwlchemyVR;
using UnityEngine;

public class GrowingPlantController : MonoBehaviour
{
	[SerializeField]
	private ParticleCollectionZone fluidCollector;

	[SerializeField]
	private Animation growAnimation;

	[SerializeField]
	private float growthPerFluidUnit = 0.005f;

	private AnimationState growAnimState;

	private void OnEnable()
	{
		ParticleCollectionZone particleCollectionZone = fluidCollector;
		particleCollectionZone.OnParticleQuantityUnitAdded = (Action<ParticleCollectionZone, WorldItemData>)Delegate.Combine(particleCollectionZone.OnParticleQuantityUnitAdded, new Action<ParticleCollectionZone, WorldItemData>(FluidAdded));
		growAnimState = growAnimation[growAnimation.clip.name];
		growAnimState.speed = 0f;
	}

	private void OnDisable()
	{
		ParticleCollectionZone particleCollectionZone = fluidCollector;
		particleCollectionZone.OnParticleQuantityUnitAdded = (Action<ParticleCollectionZone, WorldItemData>)Delegate.Remove(particleCollectionZone.OnParticleQuantityUnitAdded, new Action<ParticleCollectionZone, WorldItemData>(FluidAdded));
	}

	private void FluidAdded(ParticleCollectionZone collectionZone, WorldItemData fluidData)
	{
		growAnimState.normalizedTime += growthPerFluidUnit;
	}
}
