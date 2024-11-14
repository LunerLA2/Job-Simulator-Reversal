using System;
using OwlchemyVR;
using UnityEngine;

public class GasTankController : VehicleHardware
{
	[SerializeField]
	private ParticleImpactZone particleImpactZone;

	private ParticleSystem[] flameExhaustPFX;

	private ParticleSystem[] exhaustPFX;

	[SerializeField]
	private WorldItemData energyDrinkMaxFluid;

	[SerializeField]
	private int smokeEmissionRate = 5;

	[SerializeField]
	private int NOSEmissionRate = 10;

	[SerializeField]
	private MeshFilter meshFilter;

	public ParticleImpactZone GasImpactZone
	{
		get
		{
			return particleImpactZone;
		}
	}

	private void OnEnable()
	{
		ParticleImpactZone obj = particleImpactZone;
		obj.OnSpecificParticleAppliedUpdate = (Action<ParticleImpactZone, WorldItemData, Vector3>)Delegate.Combine(obj.OnSpecificParticleAppliedUpdate, new Action<ParticleImpactZone, WorldItemData, Vector3>(OnFluidAddedToTank));
	}

	private void OnDisable()
	{
		ParticleImpactZone obj = particleImpactZone;
		obj.OnSpecificParticleAppliedUpdate = (Action<ParticleImpactZone, WorldItemData, Vector3>)Delegate.Remove(obj.OnSpecificParticleAppliedUpdate, new Action<ParticleImpactZone, WorldItemData, Vector3>(OnFluidAddedToTank));
	}

	public void Setup(Mesh mesh)
	{
		meshFilter.mesh = mesh;
	}

	public override void AttachToChassis(VehicleChassisController chassis)
	{
		base.AttachToChassis(chassis);
		exhaustPFX = parentChassis.ExhaustPFX;
		flameExhaustPFX = parentChassis.FlameExhaustPFX;
	}

	public void OnFluidAddedToTank(ParticleImpactZone zone, WorldItemData fluidData, Vector3 pos)
	{
		if (fluidData == energyDrinkMaxFluid)
		{
			if (flameExhaustPFX.Length > 0)
			{
				ParticleSystem[] array = flameExhaustPFX;
				foreach (ParticleSystem particleSystem in array)
				{
					particleSystem.gameObject.SetActive(true);
				}
			}
			if (exhaustPFX.Length > 0)
			{
				ParticleSystem[] array2 = exhaustPFX;
				foreach (ParticleSystem particleSystem2 in array2)
				{
					particleSystem2.emissionRate = NOSEmissionRate;
				}
			}
			parentChassis.StartFlameSFX();
		}
		else
		{
			if (flameExhaustPFX.Length > 0)
			{
				ParticleSystem[] array3 = flameExhaustPFX;
				foreach (ParticleSystem particleSystem3 in array3)
				{
					particleSystem3.gameObject.SetActive(false);
				}
			}
			if (exhaustPFX.Length > 0)
			{
				ParticleSystem[] array4 = exhaustPFX;
				foreach (ParticleSystem particleSystem4 in array4)
				{
					particleSystem4.emissionRate = smokeEmissionRate;
				}
			}
			parentChassis.StopFlameSFX();
		}
		if (exhaustPFX.Length > 0)
		{
			ParticleSystem[] array5 = exhaustPFX;
			foreach (ParticleSystem particleSystem5 in array5)
			{
				particleSystem5.startColor = fluidData.OverallColor;
			}
		}
	}
}
