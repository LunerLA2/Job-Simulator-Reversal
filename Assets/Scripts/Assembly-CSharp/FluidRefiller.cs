using OwlchemyVR;
using UnityEngine;

public class FluidRefiller : MonoBehaviour
{
	[SerializeField]
	private ParticleCollectionZone particleCollectionZone;

	[SerializeField]
	private ContainerFluidSystem containerFluidSystem;

	[SerializeField]
	[Tooltip("What fluid to fill the container with")]
	private WorldItemData fluidWorldItemData;

	[Tooltip("Rate the fluid should refill")]
	[SerializeField]
	private float refillParticlesPerSecond;

	[SerializeField]
	private float temperature = 21f;

	[Tooltip("Fluid will stop refilling once it reaches this percentage")]
	[SerializeField]
	private float maxFillPercentage = 0.8f;

	private void Start()
	{
		if (particleCollectionZone == null || containerFluidSystem == null)
		{
			Debug.LogWarning("FluidRefiller doesn't have a ParticleCollectionZone or ContainerFluidSystem to refill. Turning off.");
			base.enabled = false;
		}
	}

	private void Update()
	{
		if (containerFluidSystem.FluidFullPercent < maxFillPercentage)
		{
			particleCollectionZone.ApplyParticleQuantity(fluidWorldItemData, refillParticlesPerSecond * Time.deltaTime, temperature);
		}
	}
}
