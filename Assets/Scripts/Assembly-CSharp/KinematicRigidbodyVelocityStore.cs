using UnityEngine;

public class KinematicRigidbodyVelocityStore : MonoBehaviour
{
	private Vector3 prevPos;

	private Vector3 currVelocity = Vector3.zero;

	public Vector3 CurrVelocity
	{
		get
		{
			return currVelocity;
		}
	}

	private void Start()
	{
		prevPos = base.transform.position;
	}

	private void LateUpdate()
	{
		currVelocity = (base.transform.position - prevPos) / Time.deltaTime;
		prevPos = base.transform.position;
	}
}
