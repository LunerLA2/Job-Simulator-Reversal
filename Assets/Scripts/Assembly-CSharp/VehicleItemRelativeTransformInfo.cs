using UnityEngine;

public class VehicleItemRelativeTransformInfo
{
	private Transform transform;

	private Transform chassisTransform;

	private Vector3 positionRelativeToChassis;

	private Quaternion rotationRelativeToChassis;

	public Transform Transform
	{
		get
		{
			return transform;
		}
	}

	public Transform ChassisTransform
	{
		get
		{
			return chassisTransform;
		}
	}

	public VehicleItemRelativeTransformInfo(Transform t, Transform _chassisTransform)
	{
		transform = t;
		chassisTransform = _chassisTransform;
		positionRelativeToChassis = chassisTransform.InverseTransformPoint(t.position);
		rotationRelativeToChassis = Quaternion.Inverse(chassisTransform.rotation) * t.rotation;
	}

	public override bool Equals(object obj)
	{
		if (obj is VehicleItemRelativeTransformInfo)
		{
			VehicleItemRelativeTransformInfo vehicleItemRelativeTransformInfo = obj as VehicleItemRelativeTransformInfo;
			if (vehicleItemRelativeTransformInfo.Transform == transform && vehicleItemRelativeTransformInfo.ChassisTransform == chassisTransform)
			{
				return true;
			}
		}
		return false;
	}

	public void Update()
	{
		if (!(transform == null))
		{
			transform.position = chassisTransform.TransformPoint(positionRelativeToChassis);
			transform.rotation = chassisTransform.rotation * rotationRelativeToChassis;
		}
	}
}
