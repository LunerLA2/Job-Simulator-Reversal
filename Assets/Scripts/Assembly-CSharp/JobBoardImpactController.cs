using UnityEngine;

public class JobBoardImpactController : MonoBehaviour
{
	[SerializeField]
	private Animation impactAnimation;

	[SerializeField]
	private ParticleSystem impactParticle;

	[SerializeField]
	private AudioClip impactZapAudio;

	private void OnCollisionEnter(Collision collision)
	{
		if (collision.collider.attachedRigidbody != null && !impactAnimation.isPlaying)
		{
			AudioManager.Instance.Play(base.transform.position, impactZapAudio, 0.6f, 1f);
			impactAnimation.Play();
			impactParticle.Play();
		}
	}
}
