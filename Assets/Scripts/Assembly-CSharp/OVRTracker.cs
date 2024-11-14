using UnityEngine;

public class OVRTracker
{
	public struct Frustum
	{
		public float nearZ;

		public float farZ;

		public Vector2 fov;
	}

	public bool isPresent
	{
		get
		{
			if (!OVRManager.isHmdPresent)
			{
				return false;
			}
			return OVRPlugin.positionSupported;
		}
	}

	public bool isPositionTracked
	{
		get
		{
			return OVRPlugin.positionTracked;
		}
	}

	public bool isEnabled
	{
		get
		{
			if (!OVRManager.isHmdPresent)
			{
				return false;
			}
			return OVRPlugin.position;
		}
		set
		{
			if (OVRManager.isHmdPresent)
			{
				OVRPlugin.position = value;
			}
		}
	}

	public int count
	{
		get
		{
			int num = 0;
			for (int i = 0; i < 4; i++)
			{
				if (GetPresent(i))
				{
					num++;
				}
			}
			return num;
		}
	}

	public Frustum GetFrustum(int tracker = 0)
	{
		if (!OVRManager.isHmdPresent)
		{
			return default(Frustum);
		}
		return OVRPlugin.GetTrackerFrustum((OVRPlugin.Tracker)tracker).ToFrustum();
	}

	public OVRPose GetPose(int tracker = 0)
	{
		if (!OVRManager.isHmdPresent)
		{
			return OVRPose.identity;
		}
		OVRPose oVRPose;
		switch (tracker)
		{
		case 0:
			oVRPose = OVRPlugin.GetNodePose(OVRPlugin.Node.TrackerZero, false).ToOVRPose();
			break;
		case 1:
			oVRPose = OVRPlugin.GetNodePose(OVRPlugin.Node.TrackerOne, false).ToOVRPose();
			break;
		case 2:
			oVRPose = OVRPlugin.GetNodePose(OVRPlugin.Node.TrackerTwo, false).ToOVRPose();
			break;
		case 3:
			oVRPose = OVRPlugin.GetNodePose(OVRPlugin.Node.TrackerThree, false).ToOVRPose();
			break;
		default:
			return OVRPose.identity;
		}
		OVRPose result = default(OVRPose);
		result.position = oVRPose.position;
		result.orientation = oVRPose.orientation * Quaternion.Euler(0f, 180f, 0f);
		return result;
	}

	public bool GetPoseValid(int tracker = 0)
	{
		if (!OVRManager.isHmdPresent)
		{
			return false;
		}
		switch (tracker)
		{
		case 0:
			return OVRPlugin.GetNodePositionTracked(OVRPlugin.Node.TrackerZero);
		case 1:
			return OVRPlugin.GetNodePositionTracked(OVRPlugin.Node.TrackerOne);
		case 2:
			return OVRPlugin.GetNodePositionTracked(OVRPlugin.Node.TrackerTwo);
		case 3:
			return OVRPlugin.GetNodePositionTracked(OVRPlugin.Node.TrackerThree);
		default:
			return false;
		}
	}

	public bool GetPresent(int tracker = 0)
	{
		if (!OVRManager.isHmdPresent)
		{
			return false;
		}
		switch (tracker)
		{
		case 0:
			return OVRPlugin.GetNodePresent(OVRPlugin.Node.TrackerZero);
		case 1:
			return OVRPlugin.GetNodePresent(OVRPlugin.Node.TrackerOne);
		case 2:
			return OVRPlugin.GetNodePresent(OVRPlugin.Node.TrackerTwo);
		case 3:
			return OVRPlugin.GetNodePresent(OVRPlugin.Node.TrackerThree);
		default:
			return false;
		}
	}
}
