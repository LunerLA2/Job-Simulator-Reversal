using OwlchemyVR;
using UnityEngine;

public class JoystickController : MonoBehaviour
{
	[SerializeField]
	private GrabbableItem grabbableItem;

	[SerializeField]
	private Transform anchor;

	private Rigidbody rb;

	[SerializeField]
	private float deadZone = 0.15f;

	[SerializeField]
	private Vector2 joystickDirection;

	public Vector2 JoystickDirection
	{
		get
		{
			return joystickDirection;
		}
	}

	private void Awake()
	{
		rb = GetComponent<Rigidbody>();
	}

	private void Update()
	{
		if (!grabbableItem.IsCurrInHand)
		{
			if (Vector3.Distance(base.transform.position, anchor.position) > 0.005f)
			{
				rb.velocity = (anchor.position - base.transform.position).normalized * 0.5f;
			}
			else
			{
				rb.velocity = Vector3.zero;
				base.transform.localPosition = new Vector3(0f, base.transform.localPosition.y, 0f);
			}
		}
		joystickDirection = new Vector2(base.transform.localPosition.z * 10f, (0f - base.transform.localPosition.x) * 10f);
		ApplyDeadZoneAndClamp();
	}

	private void ApplyDeadZoneAndClamp()
	{
		if (joystickDirection.x > 1f)
		{
			joystickDirection.x = 1f;
		}
		if (joystickDirection.x < -1f)
		{
			joystickDirection.x = -1f;
		}
		if (joystickDirection.y > 1f)
		{
			joystickDirection.y = 1f;
		}
		if (joystickDirection.y < -1f)
		{
			joystickDirection.y = -1f;
		}
		if (joystickDirection.x > 0f && joystickDirection.x < deadZone)
		{
			joystickDirection.x = 0f;
		}
		if (joystickDirection.x < 0f && joystickDirection.x > 0f - deadZone)
		{
			joystickDirection.x = 0f;
		}
		if (joystickDirection.y > 0f && joystickDirection.y < deadZone)
		{
			joystickDirection.y = 0f;
		}
		if (joystickDirection.y < 0f && joystickDirection.y > 0f - deadZone)
		{
			joystickDirection.y = 0f;
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.Lerp(Color.red, Color.clear, 0.5f);
		Gizmos.DrawSphere(anchor.position, 0.04f);
	}
}
