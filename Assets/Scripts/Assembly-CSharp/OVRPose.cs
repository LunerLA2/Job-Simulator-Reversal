using System;
using UnityEngine;

[Serializable]
public struct OVRPose
{
	public Vector3 position;

	public Quaternion orientation;

	public static OVRPose identity
	{
		get
		{
			OVRPose result = default(OVRPose);
			result.position = Vector3.zero;
			result.orientation = Quaternion.identity;
			return result;
		}
	}

	public override bool Equals(object obj)
	{
		return obj is OVRPose && this == (OVRPose)obj;
	}

	public override int GetHashCode()
	{
		return position.GetHashCode() ^ orientation.GetHashCode();
	}

	public OVRPose Inverse()
	{
		OVRPose result = default(OVRPose);
		result.orientation = Quaternion.Inverse(orientation);
		result.position = result.orientation * -position;
		return result;
	}

	internal OVRPose flipZ()
	{
		OVRPose result = this;
		result.position.z = 0f - result.position.z;
		result.orientation.z = 0f - result.orientation.z;
		result.orientation.w = 0f - result.orientation.w;
		return result;
	}

	internal OVRPlugin.Posef ToPosef()
	{
		OVRPlugin.Posef result = default(OVRPlugin.Posef);
		result.Position = position.ToVector3f();
		result.Orientation = orientation.ToQuatf();
		return result;
	}

	public static bool operator ==(OVRPose x, OVRPose y)
	{
		return x.position == y.position && x.orientation == y.orientation;
	}

	public static bool operator !=(OVRPose x, OVRPose y)
	{
		return !(x == y);
	}

	public static OVRPose operator *(OVRPose lhs, OVRPose rhs)
	{
		OVRPose result = default(OVRPose);
		result.position = lhs.position + lhs.orientation * rhs.position;
		result.orientation = lhs.orientation * rhs.orientation;
		return result;
	}
}
