using UnityEngine;

public class ParticleImpactZonePointer : MonoBehaviour
{
	[SerializeField]
	private ParticleImpactZone particleImpactZone;

	public ParticleImpactZone ParticleImpactZone
	{
		get
		{
			return particleImpactZone;
		}
	}
}
