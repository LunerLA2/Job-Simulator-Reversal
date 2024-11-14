using UnityEngine;

public class ApplyForce : MonoBehaviour
{
	public bool onUpdate;

	public Vector3 force;

	private Rigidbody r;

	private void Awake()
	{
		r = GetComponent<Rigidbody>();
		if (!onUpdate)
		{
			r.AddRelativeForce(force);
		}
	}

	private void FixedUpdate()
	{
		if (onUpdate)
		{
			r.AddRelativeForce(force, ForceMode.Force);
		}
	}
}
