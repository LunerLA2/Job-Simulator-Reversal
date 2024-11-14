using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(HingeJoint))]
public class OneWayHingeSpring : MonoBehaviour
{
	[SerializeField]
	private SpringResistanceDirection resistanceDirection;

	private HingeJoint hinge;

	public SpringResistanceDirection ResistanceDirection
	{
		get
		{
			return resistanceDirection;
		}
	}

	private void Awake()
	{
		hinge = GetComponent<HingeJoint>();
	}

	private void Update()
	{
		JointSpring spring = hinge.spring;
		float num;
		for (num = hinge.angle; num > 180f; num -= 360f)
		{
		}
		for (; num < -180f; num += 360f)
		{
		}
		if (base.name == "MicrowaveDoorLogic")
		{
			hinge.useSpring = (resistanceDirection == SpringResistanceDirection.GreaterThan && num > spring.targetPosition) || (resistanceDirection == SpringResistanceDirection.LessThan && num < spring.targetPosition);
		}
	}
}
