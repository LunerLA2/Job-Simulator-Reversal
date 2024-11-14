using System;
using OwlchemyVR;
using UnityEngine;

[Serializable]
public class ParticleImpactEffectDefinition
{
	[SerializeField]
	private WorldItemData fluidData;

	[SerializeField]
	private ParticleSystem effectPrefab;

	public WorldItemData FluidData
	{
		get
		{
			return fluidData;
		}
	}

	public ParticleSystem EffectPrefab
	{
		get
		{
			return effectPrefab;
		}
	}
}
