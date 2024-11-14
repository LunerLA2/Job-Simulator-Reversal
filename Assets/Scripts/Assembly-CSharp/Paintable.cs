using System;
using OwlchemyVR;
using UnityEngine;

public class Paintable : MonoBehaviour
{
	[SerializeField]
	private ParticleImpactZone particleImpactZone;

	[SerializeField]
	private MeshRenderer meshRenderer;

	private void OnEnable()
	{
		ParticleImpactZone obj = particleImpactZone;
		obj.OnSpecificParticleAppliedUpdate = (Action<ParticleImpactZone, WorldItemData, Vector3>)Delegate.Combine(obj.OnSpecificParticleAppliedUpdate, new Action<ParticleImpactZone, WorldItemData, Vector3>(SpecificFluidPouredOnAtPosition));
	}

	private void OnDisable()
	{
		ParticleImpactZone obj = particleImpactZone;
		obj.OnSpecificParticleAppliedUpdate = (Action<ParticleImpactZone, WorldItemData, Vector3>)Delegate.Remove(obj.OnSpecificParticleAppliedUpdate, new Action<ParticleImpactZone, WorldItemData, Vector3>(SpecificFluidPouredOnAtPosition));
	}

	private void SpecificFluidPouredOnAtPosition(ParticleImpactZone zone, WorldItemData data, Vector3 position)
	{
		meshRenderer.material.color = data.OverallColor;
		meshRenderer.enabled = true;
	}
}
