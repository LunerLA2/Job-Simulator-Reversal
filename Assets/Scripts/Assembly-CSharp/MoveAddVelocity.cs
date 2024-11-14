using UnityEngine;

public class MoveAddVelocity : MonoBehaviour
{
	public Vector3 velocity;

	private Rigidbody r;

	private void Awake()
	{
		r = GetComponent<Rigidbody>();
		if (!r.isKinematic)
		{
			r.velocity = velocity;
		}
	}

	private void OnCollisionEnter(Collision collision)
	{
		Vector3 relativeVelocity = collision.relativeVelocity;
		if (collision.rigidbody.isKinematic)
		{
			KinematicRigidbodyVelocityStore component = collision.gameObject.GetComponent<KinematicRigidbodyVelocityStore>();
			if (component != null)
			{
				relativeVelocity += component.CurrVelocity;
			}
		}
		if (r.isKinematic)
		{
			KinematicRigidbodyVelocityStore component2 = base.gameObject.GetComponent<KinematicRigidbodyVelocityStore>();
			if (component2 != null)
			{
				relativeVelocity -= component2.CurrVelocity;
			}
		}
		Debug.Log(string.Concat("GameObject:", base.gameObject.name, ", Collision Force:", relativeVelocity, ", Mag:", relativeVelocity.magnitude));
	}

	private void Update()
	{
		if (r.isKinematic)
		{
			base.transform.position = base.transform.position + velocity * Time.deltaTime;
		}
	}
}
