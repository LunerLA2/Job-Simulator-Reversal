using UnityEngine;

public class TailPipeController : MonoBehaviour
{
	[SerializeField]
	private ParticleSystem exhaustParticle;

	[SerializeField]
	private ParticleSystem interiorExhaustParticle;

	[SerializeField]
	private AttachablePoint tailPipeAttachablePoint;

	private void OnEnable()
	{
		tailPipeAttachablePoint.OnObjectWasAttached += TailPipePlugged;
		tailPipeAttachablePoint.OnObjectWasDetached += TailPipeUnplugged;
	}

	private void OnDisable()
	{
		tailPipeAttachablePoint.OnObjectWasAttached -= TailPipePlugged;
		tailPipeAttachablePoint.OnObjectWasDetached -= TailPipeUnplugged;
	}

	private void TailPipePlugged(AttachablePoint point, AttachableObject plug)
	{
		ParticleSystem.EmissionModule emission = exhaustParticle.emission;
		emission.enabled = false;
		emission = interiorExhaustParticle.emission;
		emission.enabled = true;
	}

	private void TailPipeUnplugged(AttachablePoint point, AttachableObject plug)
	{
		ParticleSystem.EmissionModule emission = exhaustParticle.emission;
		emission.enabled = true;
		interiorExhaustParticle.startColor = exhaustParticle.startColor;
		emission = interiorExhaustParticle.emission;
		emission.enabled = false;
	}

	private void Start()
	{
		ParticleSystem.EmissionModule emission = interiorExhaustParticle.emission;
		emission.enabled = false;
	}
}
