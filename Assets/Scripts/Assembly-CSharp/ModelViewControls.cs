using UnityEngine;

public class ModelViewControls : MonoBehaviour
{
	private int yMinLimit;

	private int yMaxLimit = 80;

	private Quaternion currentRotation;

	private Quaternion desiredRotation;

	private Quaternion rotation;

	private float yDeg = 15f;

	private float xDeg;

	private float currentDistance;

	private float desiredDistance = 3f;

	private float maxDistance = 6f;

	private float minDistance = 9f;

	private Vector3 position;

	public GameObject targetObject;

	public GameObject camObject;

	private float sensitivity = 1.25f;

	private void Start()
	{
		currentDistance = Mathf.Clamp(desiredDistance, minDistance, maxDistance);
	}

	private void Update()
	{
		CameraControlUpdate();
	}

	private void CameraControlUpdate()
	{
		yDeg += Input.GetAxis("Vertical") * sensitivity;
		xDeg -= Input.GetAxis("Horizontal") * sensitivity;
		yDeg = ClampAngle(yDeg, yMinLimit, yMaxLimit);
		desiredRotation = Quaternion.Euler(yDeg, xDeg, 0f);
		rotation = Quaternion.Lerp(targetObject.transform.rotation, desiredRotation, 0.05f);
		targetObject.transform.rotation = desiredRotation;
		desiredDistance = Mathf.Clamp(desiredDistance, minDistance, maxDistance);
		currentDistance = Mathf.Lerp(currentDistance, desiredDistance, 0.05f);
		position = targetObject.transform.position - rotation * Vector3.forward * currentDistance;
		Vector3 vector = Vector3.Lerp(camObject.transform.position, position, 0.05f);
		camObject.transform.position = vector;
	}

	private static float ClampAngle(float angle, float min, float max)
	{
		if (angle < -360f)
		{
			angle += 360f;
		}
		if (angle > 360f)
		{
			angle -= 360f;
		}
		return Mathf.Clamp(angle, min, max);
	}
}
