using UnityEngine;

public class ConfigurableJointSaveState
{
	public Rigidbody connectedBody;

	public Vector3 anchor;

	public Vector3 connectedAnchor;

	public Vector3 axis;

	public Vector3 secondaryAxis;

	public ConfigurableJointMotion xMotion;

	public ConfigurableJointMotion yMotion;

	public ConfigurableJointMotion zMotion;

	public ConfigurableJointMotion angularXMotion;

	public ConfigurableJointMotion angularYMotion;

	public ConfigurableJointMotion angularZMotion;

	public SoftJointLimit linearLimit;

	public SoftJointLimit lowAngularXLimit;

	public SoftJointLimit highAngularXLimit;

	public SoftJointLimit angularYLimit;

	public SoftJointLimit angularZLimit;

	public SoftJointLimitSpring linearLimitSpring;

	public SoftJointLimitSpring angularXLimitSpring;

	public SoftJointLimitSpring angularYZLimitSpring;

	public bool enableCollision;

	public static ConfigurableJointSaveState CreateFromJoint(ConfigurableJoint joint)
	{
		ConfigurableJointSaveState configurableJointSaveState = new ConfigurableJointSaveState();
		configurableJointSaveState.connectedBody = joint.connectedBody;
		configurableJointSaveState.anchor = joint.anchor;
		configurableJointSaveState.connectedAnchor = joint.connectedAnchor;
		configurableJointSaveState.axis = joint.axis;
		configurableJointSaveState.secondaryAxis = joint.secondaryAxis;
		configurableJointSaveState.xMotion = joint.xMotion;
		configurableJointSaveState.yMotion = joint.yMotion;
		configurableJointSaveState.zMotion = joint.zMotion;
		configurableJointSaveState.angularXMotion = joint.angularXMotion;
		configurableJointSaveState.angularYMotion = joint.angularYMotion;
		configurableJointSaveState.angularZMotion = joint.angularZMotion;
		configurableJointSaveState.linearLimit = joint.linearLimit;
		configurableJointSaveState.lowAngularXLimit = joint.lowAngularXLimit;
		configurableJointSaveState.highAngularXLimit = joint.highAngularXLimit;
		configurableJointSaveState.angularYLimit = joint.angularYLimit;
		configurableJointSaveState.angularZLimit = joint.angularZLimit;
		configurableJointSaveState.linearLimitSpring = joint.linearLimitSpring;
		configurableJointSaveState.angularXLimitSpring = joint.angularXLimitSpring;
		configurableJointSaveState.angularYZLimitSpring = joint.angularYZLimitSpring;
		return configurableJointSaveState;
	}

	public void RestoreToJoint(ConfigurableJoint joint)
	{
		joint.connectedBody = connectedBody;
		joint.anchor = anchor;
		joint.connectedAnchor = connectedAnchor;
		joint.axis = axis;
		joint.secondaryAxis = secondaryAxis;
		joint.xMotion = xMotion;
		joint.yMotion = yMotion;
		joint.zMotion = zMotion;
		joint.angularXMotion = angularXMotion;
		joint.angularYMotion = angularYMotion;
		joint.angularZMotion = angularZMotion;
		joint.linearLimit = linearLimit;
		joint.lowAngularXLimit = lowAngularXLimit;
		joint.highAngularXLimit = highAngularXLimit;
		joint.angularYLimit = angularYLimit;
		joint.angularZLimit = angularZLimit;
		joint.linearLimitSpring = linearLimitSpring;
		joint.angularXLimitSpring = angularXLimitSpring;
		joint.angularYZLimitSpring = angularYZLimitSpring;
	}

	public ConfigurableJoint GenerateJointOnGameObject(GameObject go)
	{
		ConfigurableJoint configurableJoint = go.AddComponent<ConfigurableJoint>();
		RestoreToJoint(configurableJoint);
		return configurableJoint;
	}
}
