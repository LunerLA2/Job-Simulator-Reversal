using System;
using OwlchemyVR;
using UnityEngine;

public class ParticleImpactZone : MonoBehaviour
{
	[SerializeField]
	private ParticleCollectionZone particleCollectionZone;

	public Action<ParticleImpactZone, Vector3> OnAnyParticleAppliedUpdate;

	public Action<ParticleImpactZone, WorldItemData, Vector3> OnSpecificParticleAppliedUpdate;

	public ParticleCollectionZone ParticleCollectionZone
	{
		get
		{
			return particleCollectionZone;
		}
	}

	public void ApplyFuildImpact(FluidImpactParticleManager.FluidImpactDetails fluidImpactDetails)
	{
		if (particleCollectionZone != null)
		{
			for (int i = 0; i < fluidImpactDetails.numOfFluids; i++)
			{
				particleCollectionZone.ApplyParticleQuantity(fluidImpactDetails.fluidQuantities[i].FluidData, fluidImpactDetails.fluidQuantities[i].Quantity, fluidImpactDetails.temperatureCelsius);
			}
		}
		if (OnSpecificParticleAppliedUpdate != null)
		{
			for (int j = 0; j < fluidImpactDetails.numOfFluids; j++)
			{
				OnSpecificParticleAppliedUpdate(this, fluidImpactDetails.fluidQuantities[j].FluidData, fluidImpactDetails.pos);
			}
		}
		if (OnAnyParticleAppliedUpdate != null)
		{
			OnAnyParticleAppliedUpdate(this, fluidImpactDetails.pos);
		}
	}
}
