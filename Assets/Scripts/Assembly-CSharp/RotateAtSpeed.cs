using UnityEngine;

public class RotateAtSpeed : MonoBehaviour
{
	[SerializeField]
	private Vector3 rotationSpeed;

	public Vector3 RotationSpeed
	{
		get
		{
			return rotationSpeed;
		}
	}

	private void Update()
	{
		base.transform.Rotate(rotationSpeed * Time.deltaTime, Space.Self);
	}

	public void SetSpeed(Vector3 rotationSpeed)
	{
		this.rotationSpeed = rotationSpeed;
	}

	public void IncreaseSpeed()
	{
		rotationSpeed *= 2f;
	}

	public void DecreaseSpeed()
	{
		rotationSpeed /= 2f;
	}
}
