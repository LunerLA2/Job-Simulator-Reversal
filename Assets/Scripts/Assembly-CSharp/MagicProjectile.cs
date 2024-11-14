using UnityEngine;

public class MagicProjectile : MonoBehaviour
{
	[SerializeField]
	private Rigidbody rb;

	[SerializeField]
	private float projectileForce;

	[SerializeField]
	private GameObject magicImpactPrefab;

	[SerializeField]
	private AudioClip impactSoundEffect;

	[SerializeField]
	private float impactExplosionForce;

	[SerializeField]
	private GameObject magicTransformPrefab;

	private void Start()
	{
		rb.AddForce(base.transform.forward * projectileForce);
	}

	private void OnCollisionEnter(Collision c)
	{
		if (!(c.collider.GetComponent<MagicWand>() != null))
		{
			AudioManager.Instance.Play(base.transform.position, impactSoundEffect, 0.5f, 1f);
			Object.Instantiate(magicImpactPrefab, c.contacts[0].point, Quaternion.FromToRotation(base.transform.forward, c.contacts[0].normal));
			MagicReceiver component = GetGameObjectFromCollider(c.collider).GetComponent<MagicReceiver>();
			if (component != null)
			{
				Object.Instantiate(magicTransformPrefab, component.transform.position, Quaternion.identity);
				component.HitByMagic();
			}
			else if (c.collider.attachedRigidbody != null)
			{
				c.collider.attachedRigidbody.AddExplosionForce(impactExplosionForce, c.contacts[0].point, 1f);
			}
			Object.Destroy(base.gameObject);
		}
	}

	private GameObject GetGameObjectFromCollider(Collider c)
	{
		if (c.attachedRigidbody != null)
		{
			return c.attachedRigidbody.gameObject;
		}
		return c.gameObject;
	}
}
