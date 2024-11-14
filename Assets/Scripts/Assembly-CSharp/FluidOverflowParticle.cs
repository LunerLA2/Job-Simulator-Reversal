using System.Collections;
using UnityEngine;

public class FluidOverflowParticle : MonoBehaviour
{
	[SerializeField]
	private ParticleSystem fluidParticleSystem;

	private float baseVelocity;

	private float baseEmissionRate;

	private float particleFallingAcceleration;

	private void Awake()
	{
		if (GenieManager.IsNoGravityEnabled())
		{
			fluidParticleSystem.gravityModifier = fluidParticleSystem.gravityModifier / 1E-05f * 1f;
		}
		baseVelocity = fluidParticleSystem.startSpeed;
		baseEmissionRate = fluidParticleSystem.emissionRate;
		particleFallingAcceleration = Physics.gravity.y * fluidParticleSystem.gravityModifier;
	}

	public void SetColor(Color c)
	{
		fluidParticleSystem.startColor = c;
	}

	public void SetVelocityMultiplier(float v)
	{
		fluidParticleSystem.startSpeed = baseVelocity * v;
	}

	public void SetEmissionMultiplier(float e, bool instant = true)
	{
		if (instant)
		{
			fluidParticleSystem.emissionRate = baseEmissionRate * e;
		}
		else
		{
			StartCoroutine(WaitAndSetEmissionMultiplier(e));
		}
	}

	private IEnumerator WaitAndSetEmissionMultiplier(float e)
	{
		yield return null;
		fluidParticleSystem.emissionRate = baseEmissionRate * e;
	}

	public float SetLifetimeUsingDistance(float distance)
	{
		distance += 0.03f;
		float timeToParticleImpact = FluidParticleRaycastEmitter.GetTimeToParticleImpact(distance, particleFallingAcceleration, fluidParticleSystem.startSpeed * fluidParticleSystem.transform.forward.y);
		fluidParticleSystem.startLifetime = timeToParticleImpact;
		return timeToParticleImpact;
	}
}
