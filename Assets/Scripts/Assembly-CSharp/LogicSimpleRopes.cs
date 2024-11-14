using OwlchemyVR;
using UnityEngine;

public class LogicSimpleRopes : MonoBehaviour
{
	private enum StretchLevel
	{
		High = 0,
		Mid = 1,
		Low = 2
	}

	[SerializeField]
	private Transform handle;

	[SerializeField]
	private Transform topPart;

	[SerializeField]
	private Transform nodeContainer;

	[SerializeField]
	private bool attachLastNodeToHandle;

	[SerializeField]
	private PickupableItem pickupableRopeEnd;

	private float timeSinceRelease;

	private bool isHeld;

	private bool isFrozen;

	private UltimateRope rope;

	[SerializeField]
	private float highLinkJointMaxForceValue;

	[SerializeField]
	private float midLinkJointMaxForceValue;

	[SerializeField]
	private float highLinkJointSpringValue;

	[SerializeField]
	private float midLinkJointSpringValue;

	[SerializeField]
	private float highMass;

	[SerializeField]
	private float midMass;

	private float percentageOfLengthHandBreak = 0.99f;

	private float lowLinkJointSpringValue;

	private float lowLinkJointMaxForceValue;

	private float lowMass;

	private float highStretchLevelDuration;

	private StretchLevel stretchLevel = StretchLevel.Low;

	private void Awake()
	{
	}

	private void Start()
	{
		rope = nodeContainer.GetComponent<UltimateRope>();
		if (attachLastNodeToHandle && nodeContainer != null && nodeContainer.childCount > 1)
		{
			nodeContainer.GetChild(nodeContainer.childCount - 1).SetParent(handle);
		}
		lowLinkJointMaxForceValue = rope.LinkJointMaxForceValue;
		lowLinkJointSpringValue = rope.LinkJointSpringValue;
		lowMass = rope.LinkMass;
		for (int i = 0; i < rope.RopeNodes.Count; i++)
		{
			for (int j = 0; j < rope.RopeNodes[i].linkJoints.Length; j++)
			{
				if (rope.RopeNodes[i] == null || rope.RopeNodes[i].linkJoints == null || !(rope.RopeNodes[i].linkJoints[j] != null) || rope.RopeNodes[i].linkJoints[j].connectedBody != null)
				{
				}
			}
		}
	}

	private void Update()
	{
		if (pickupableRopeEnd.CurrInteractableHand != null)
		{
			if (isFrozen)
			{
				isFrozen = false;
				SetRopeKinematic(false);
			}
			isHeld = true;
		}
		else if (isHeld)
		{
			isHeld = false;
			timeSinceRelease = 0f;
		}
		if (!isHeld)
		{
			timeSinceRelease += Time.deltaTime;
			if (timeSinceRelease >= 2f && !isFrozen)
			{
				isFrozen = true;
				SetRopeKinematic(true);
			}
		}
		float num = rope.RopeNodes[0].fLength + rope.m_fCurrentExtension;
		float num2 = num * 0.955f;
		float num3 = num * 0.94f;
		float num4 = num * percentageOfLengthHandBreak;
		float num5 = 0f;
		for (int i = 0; i < rope.RopeNodes[0].linkJoints.Length; i++)
		{
			bool flag = i > 0;
			ConfigurableJoint configurableJoint = rope.RopeNodes[0].linkJoints[i];
			if (flag)
			{
				float num6 = 0f;
				ConfigurableJoint configurableJoint2 = rope.RopeNodes[0].linkJoints[i - 1];
				num6 = Vector3.Distance(configurableJoint.transform.position, configurableJoint2.transform.position);
				num5 += num6;
			}
		}
		StretchLevel stretchLevel = this.stretchLevel;
		if (num5 > num4 && pickupableRopeEnd != null && pickupableRopeEnd.IsCurrInHand)
		{
			pickupableRopeEnd.CurrInteractableHand.TryRelease();
		}
		if (num5 > num2)
		{
			this.stretchLevel = StretchLevel.High;
			rope.LinkJointMaxForceValue = highLinkJointMaxForceValue;
			rope.LinkJointSpringValue = highLinkJointSpringValue;
			rope.LinkMass = highMass;
		}
		else if (num5 < num2 && num5 > num3)
		{
			this.stretchLevel = StretchLevel.Mid;
			rope.LinkJointMaxForceValue = midLinkJointMaxForceValue;
			rope.LinkJointSpringValue = midLinkJointSpringValue;
			rope.LinkMass = midMass;
		}
		else
		{
			this.stretchLevel = StretchLevel.Low;
			rope.LinkJointMaxForceValue = lowLinkJointMaxForceValue;
			rope.LinkJointSpringValue = lowLinkJointSpringValue;
			rope.LinkMass = lowMass;
		}
		if (this.stretchLevel != stretchLevel)
		{
			RefreshJointData();
		}
		if (this.stretchLevel == StretchLevel.High)
		{
			for (int j = 0; j < rope.RopeNodes.Count; j++)
			{
				for (int k = 0; k < rope.RopeNodes[j].linkJoints.Length; k++)
				{
					if (rope.RopeNodes[j] != null && rope.RopeNodes[j].linkJoints != null && rope.RopeNodes[j].linkJoints[k] != null && rope.RopeNodes[j].linkJoints[k].connectedBody != null)
					{
						rope.RopeNodes[j].linkJoints[k].connectedBody.velocity = Vector3.zero;
						rope.RopeNodes[j].linkJoints[k].connectedBody.angularVelocity = Vector3.zero;
						rope.RopeNodes[j].linkJoints[k].connectedBody.Sleep();
					}
				}
			}
			highStretchLevelDuration += Time.deltaTime;
			if (highStretchLevelDuration >= 1f)
			{
				highStretchLevelDuration = 0f;
				ResetRope();
			}
		}
		else
		{
			highStretchLevelDuration = 0f;
		}
	}

	private void RefreshJointData()
	{
		ConfigurableJoint[] linkJoints = rope.RopeNodes[0].linkJoints;
		foreach (ConfigurableJoint configurableJoint in linkJoints)
		{
			if ((bool)configurableJoint)
			{
				JointDrive jointDrive = default(JointDrive);
				jointDrive.positionSpring = rope.LinkJointSpringValue;
				jointDrive.positionDamper = rope.LinkJointDamperValue;
				jointDrive.maximumForce = rope.LinkJointMaxForceValue;
				configurableJoint.angularXDrive = jointDrive;
				configurableJoint.angularYZDrive = jointDrive;
				configurableJoint.GetComponent<Rigidbody>().mass = rope.LinkMass;
			}
		}
	}

	private void SetRopeKinematic(bool state)
	{
		for (int i = 0; i < rope.RopeNodes.Count; i++)
		{
			for (int j = 0; j < rope.RopeNodes[i].linkJoints.Length; j++)
			{
				if (rope.RopeNodes[i] != null && rope.RopeNodes[i].linkJoints != null && rope.RopeNodes[i].linkJoints[j] != null && rope.RopeNodes[i].linkJoints[j].connectedBody != null && rope.RopeNodes[i].linkJoints[j].connectedBody.gameObject != nodeContainer.gameObject && j > 0)
				{
					rope.RopeNodes[i].linkJoints[j].connectedBody.velocity = Vector3.zero;
					rope.RopeNodes[i].linkJoints[j].connectedBody.angularVelocity = Vector3.zero;
					if (!state)
					{
						rope.RopeNodes[i].linkJoints[j].connectedBody.Sleep();
					}
					rope.RopeNodes[i].linkJoints[j].connectedBody.isKinematic = state;
				}
			}
		}
	}

	private void ResetRope()
	{
		Debug.LogWarning("Rope Reset");
		rope.Regenerate();
	}
}
