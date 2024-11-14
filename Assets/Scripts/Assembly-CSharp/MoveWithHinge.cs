using UnityEngine;

public class MoveWithHinge : MonoBehaviour
{
	[SerializeField]
	private GrabbableHinge hingeToFollow;

	[SerializeField]
	private Vector3 movementDelta = new Vector3(0f, -0.2f, 0f);

	private Vector3 initialPosition;

	private void Start()
	{
		initialPosition = base.transform.localPosition;
	}

	private void Update()
	{
		float num = hingeToFollow.UpperLimit - hingeToFollow.LowerLimit;
		if (num == 0f)
		{
			Debug.LogError("Max angle in MoveWithHinge is 0. Disabling.");
			base.enabled = false;
			return;
		}
		float num2 = 0f;
		if (hingeToFollow.Angle - hingeToFollow.LowerLimit != 0f)
		{
			num2 = (hingeToFollow.Angle - hingeToFollow.LowerLimit) / (hingeToFollow.UpperLimit - hingeToFollow.LowerLimit);
		}
		base.transform.localPosition = initialPosition + movementDelta * num2;
	}
}
