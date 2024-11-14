using System;
using OwlchemyVR;
using UnityEngine;

public class ExtinguishableItem : MonoBehaviour
{
	[SerializeField]
	private float timeNeededToExtinguish = 1f;

	[SerializeField]
	private bool canBeExtinguishedByFluids;

	[SerializeField]
	private ParticleImpactZone particleImpactZone;

	[SerializeField]
	private bool destroySelfWhenExtinguished;

	[SerializeField]
	private WorldItem myWorldItem;

	private float timeSpent;

	private void OnEnable()
	{
		if (canBeExtinguishedByFluids)
		{
			ParticleImpactZone obj = particleImpactZone;
			obj.OnAnyParticleAppliedUpdate = (Action<ParticleImpactZone, Vector3>)Delegate.Combine(obj.OnAnyParticleAppliedUpdate, new Action<ParticleImpactZone, Vector3>(ParticleHit));
		}
	}

	private void OnDisable()
	{
		if (canBeExtinguishedByFluids)
		{
			ParticleImpactZone obj = particleImpactZone;
			obj.OnAnyParticleAppliedUpdate = (Action<ParticleImpactZone, Vector3>)Delegate.Remove(obj.OnAnyParticleAppliedUpdate, new Action<ParticleImpactZone, Vector3>(ParticleHit));
		}
	}

	private void ParticleHit(ParticleImpactZone zone, Vector3 pos)
	{
		ExtinguishProgress(Time.deltaTime);
	}

	public void ExtinguishProgress(float delta)
	{
		timeSpent += delta;
		if (timeSpent >= timeNeededToExtinguish && destroySelfWhenExtinguished)
		{
			if (myWorldItem != null)
			{
				GameEventsManager.Instance.ItemActionOccurred(myWorldItem.Data, "DESTROYED");
			}
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}
}
