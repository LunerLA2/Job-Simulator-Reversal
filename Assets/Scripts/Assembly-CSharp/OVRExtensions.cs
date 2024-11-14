using UnityEngine;
using UnityEngine.VR;

public static class OVRExtensions
{
	public static OVRPose ToTrackingSpacePose(this Transform transform)
	{
		OVRPose oVRPose = default(OVRPose);
		oVRPose.position = UnityEngine.XR.InputTracking.GetLocalPosition(UnityEngine.XR.XRNode.Head);
		oVRPose.orientation = UnityEngine.XR.InputTracking.GetLocalRotation(UnityEngine.XR.XRNode.Head);
		return oVRPose * transform.ToHeadSpacePose();
	}

	public static OVRPose ToHeadSpacePose(this Transform transform)
	{
		return Camera.current.transform.ToOVRPose().Inverse() * transform.ToOVRPose();
	}

	internal static OVRPose ToOVRPose(this Transform t, bool isLocal = false)
	{
		OVRPose result = default(OVRPose);
		result.orientation = ((!isLocal) ? t.rotation : t.localRotation);
		result.position = ((!isLocal) ? t.position : t.localPosition);
		return result;
	}

	internal static void FromOVRPose(this Transform t, OVRPose pose, bool isLocal = false)
	{
		if (isLocal)
		{
			t.localRotation = pose.orientation;
			t.localPosition = pose.position;
		}
		else
		{
			t.rotation = pose.orientation;
			t.position = pose.position;
		}
	}

	internal static OVRPose ToOVRPose(this OVRPlugin.Posef p)
	{
		OVRPose result = default(OVRPose);
		result.position = new Vector3(p.Position.x, p.Position.y, 0f - p.Position.z);
		result.orientation = new Quaternion(0f - p.Orientation.x, 0f - p.Orientation.y, p.Orientation.z, p.Orientation.w);
		return result;
	}

	internal static OVRTracker.Frustum ToFrustum(this OVRPlugin.Frustumf f)
	{
		OVRTracker.Frustum result = default(OVRTracker.Frustum);
		result.nearZ = f.zNear;
		result.farZ = f.zFar;
		result.fov = new Vector2
		{
			x = 57.29578f * f.fovX,
			y = 57.29578f * f.fovY
		};
		return result;
	}

	internal static Color FromColorf(this OVRPlugin.Colorf c)
	{
		Color result = default(Color);
		result.r = c.r;
		result.g = c.g;
		result.b = c.b;
		result.a = c.a;
		return result;
	}

	internal static OVRPlugin.Colorf ToColorf(this Color c)
	{
		OVRPlugin.Colorf result = default(OVRPlugin.Colorf);
		result.r = c.r;
		result.g = c.g;
		result.b = c.b;
		result.a = c.a;
		return result;
	}

	internal static Vector3 FromVector3f(this OVRPlugin.Vector3f v)
	{
		Vector3 result = default(Vector3);
		result.x = v.x;
		result.y = v.y;
		result.z = v.z;
		return result;
	}

	internal static Vector3 FromFlippedZVector3f(this OVRPlugin.Vector3f v)
	{
		Vector3 result = default(Vector3);
		result.x = v.x;
		result.y = v.y;
		result.z = 0f - v.z;
		return result;
	}

	internal static OVRPlugin.Vector3f ToVector3f(this Vector3 v)
	{
		OVRPlugin.Vector3f result = default(OVRPlugin.Vector3f);
		result.x = v.x;
		result.y = v.y;
		result.z = v.z;
		return result;
	}

	internal static OVRPlugin.Vector3f ToFlippedZVector3f(this Vector3 v)
	{
		OVRPlugin.Vector3f result = default(OVRPlugin.Vector3f);
		result.x = v.x;
		result.y = v.y;
		result.z = 0f - v.z;
		return result;
	}

	internal static Quaternion FromQuatf(this OVRPlugin.Quatf q)
	{
		Quaternion result = default(Quaternion);
		result.x = q.x;
		result.y = q.y;
		result.z = q.z;
		result.w = q.w;
		return result;
	}

	internal static OVRPlugin.Quatf ToQuatf(this Quaternion q)
	{
		OVRPlugin.Quatf result = default(OVRPlugin.Quatf);
		result.x = q.x;
		result.y = q.y;
		result.z = q.z;
		result.w = q.w;
		return result;
	}
}
