using UnityEngine;

public class LookAtTransform : MonoBehaviour
{
	[SerializeField]
	private Transform transformToLookAt;

	[SerializeField]
	private float lookAtSpeed = 1f;

	private void LateUpdate()
	{
		if (transformToLookAt != null)
		{
			Quaternion b = Quaternion.LookRotation(transformToLookAt.position - base.transform.position, base.transform.up);
			base.transform.rotation = Quaternion.Lerp(base.transform.rotation, b, lookAtSpeed * Time.deltaTime);
		}
	}

	public void SetTransformToLookAt(Transform t)
	{
		transformToLookAt = t;
	}
}
