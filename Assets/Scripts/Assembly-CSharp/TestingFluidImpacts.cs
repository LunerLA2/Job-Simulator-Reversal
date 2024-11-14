using System;
using UnityEngine;

public class TestingFluidImpacts : MonoBehaviour
{
	[SerializeField]
	private ParticleImpactZone particleImpactZone;

	private void OnEnable()
	{
		ParticleImpactZone obj = particleImpactZone;
		obj.OnAnyParticleAppliedUpdate = (Action<ParticleImpactZone, Vector3>)Delegate.Combine(obj.OnAnyParticleAppliedUpdate, new Action<ParticleImpactZone, Vector3>(AnyParticleImpactUpdate));
	}

	private void OnDisable()
	{
		ParticleImpactZone obj = particleImpactZone;
		obj.OnAnyParticleAppliedUpdate = (Action<ParticleImpactZone, Vector3>)Delegate.Remove(obj.OnAnyParticleAppliedUpdate, new Action<ParticleImpactZone, Vector3>(AnyParticleImpactUpdate));
	}

	private void AnyParticleImpactUpdate(ParticleImpactZone zone, Vector3 locationOfParticleApplication)
	{
		Debug.Log("Hit" + Time.frameCount + ", Loc:" + locationOfParticleApplication);
	}
}
