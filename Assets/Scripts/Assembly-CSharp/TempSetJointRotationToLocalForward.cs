using UnityEngine;

public class TempSetJointRotationToLocalForward : MonoBehaviour
{
	public ConfigurableJoint joint;

	public Vector3 orientTo;

	private void Start()
	{
		Quaternion localRotation = base.transform.localRotation;
		joint.SetTargetRotationLocal(Quaternion.Euler(orientTo), localRotation);
	}
}
