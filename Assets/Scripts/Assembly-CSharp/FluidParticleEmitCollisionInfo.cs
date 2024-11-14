public class FluidParticleEmitCollisionInfo
{
	public float distanceToHit;

	public ParticleImpactZone zone;

	public override string ToString()
	{
		return "DistanceToHit:" + distanceToHit + ", zone:" + zone;
	}
}
