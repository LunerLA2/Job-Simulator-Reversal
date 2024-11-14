using UnityEngine;

public class SwingingDoorGreaser : MonoBehaviour
{
	private enum Axis
	{
		X = 0,
		Y = 1,
		Z = 2
	}

	[SerializeField]
	private Axis doorAxis;

	[SerializeField]
	private float restingAngle;

	[SerializeField]
	private float angleThreshold = 0.05f;

	private Rigidbody rb;

	private void Awake()
	{
		rb = GetComponent<Rigidbody>();
	}

	private void FixedUpdate()
	{
		if (rb.IsSleeping())
		{
			float num = 0f;
			if (doorAxis == Axis.X)
			{
				num = base.transform.localEulerAngles.x - restingAngle;
			}
			else if (doorAxis == Axis.Y)
			{
				num = base.transform.localEulerAngles.y - restingAngle;
			}
			else if (doorAxis == Axis.Z)
			{
				num = base.transform.localEulerAngles.z - restingAngle;
			}
			while (num > 180f)
			{
				num -= 360f;
			}
			for (; num < -180f; num += 360f)
			{
			}
			if (Mathf.Abs(num) > angleThreshold)
			{
				rb.WakeUp();
			}
			else if (doorAxis == Axis.X)
			{
				base.transform.localEulerAngles = new Vector3(restingAngle, 0f, 0f);
			}
			else if (doorAxis == Axis.Y)
			{
				base.transform.localEulerAngles = new Vector3(0f, restingAngle, 0f);
			}
			else if (doorAxis == Axis.Z)
			{
				base.transform.localEulerAngles = new Vector3(0f, 0f, restingAngle);
			}
		}
	}
}
