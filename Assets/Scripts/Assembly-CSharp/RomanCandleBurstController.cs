using System;
using UnityEngine;

public class RomanCandleBurstController : MonoBehaviour
{
	[SerializeField]
	private CollisionEvents collisionEvents;

	[SerializeField]
	private ParticleSystem flyingParticle;

	[SerializeField]
	private ParticleSystem collisionParticle;

	[SerializeField]
	private Rigidbody rigidBody;

	[SerializeField]
	private float force = 15f;

	[SerializeField]
	private AudioClip explodeSFX;

	[SerializeField]
	private AudioSourceHelper audioSourceHelper;

	private bool isExploding;

	private void Start()
	{
		rigidBody.AddForce(base.transform.up * force, ForceMode.Impulse);
		audioSourceHelper.SetClip(explodeSFX);
	}

	private void OnEnable()
	{
		CollisionEvents obj = collisionEvents;
		obj.OnEnterCollision = (Action<Collision>)Delegate.Combine(obj.OnEnterCollision, new Action<Collision>(OnCollision));
	}

	private void OnDisable()
	{
		CollisionEvents obj = collisionEvents;
		obj.OnEnterCollision = (Action<Collision>)Delegate.Combine(obj.OnEnterCollision, new Action<Collision>(OnCollision));
	}

	private void OnCollision(Collision col)
	{
		if (!isExploding)
		{
			rigidBody.isKinematic = true;
			flyingParticle.Stop();
			collisionParticle.Play();
			GetComponent<Collider>().enabled = false;
			UnityEngine.Object.Destroy(base.gameObject, 5f);
			audioSourceHelper.Play();
			isExploding = true;
		}
	}
}
