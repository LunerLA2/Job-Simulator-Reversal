using System;
using UnityEngine;

public class OVRProfile : UnityEngine.Object
{
	[Obsolete]
	public enum State
	{
		NOT_TRIGGERED = 0,
		LOADING = 1,
		READY = 2,
		ERROR = 3
	}

	[Obsolete]
	public string id
	{
		get
		{
			return "000abc123def";
		}
	}

	[Obsolete]
	public string userName
	{
		get
		{
			return "Oculus User";
		}
	}

	[Obsolete]
	public string locale
	{
		get
		{
			return "en_US";
		}
	}

	public float ipd
	{
		get
		{
			return Vector3.Distance(OVRPlugin.GetNodePose(OVRPlugin.Node.EyeLeft, false).ToOVRPose().position, OVRPlugin.GetNodePose(OVRPlugin.Node.EyeRight, false).ToOVRPose().position);
		}
	}

	public float eyeHeight
	{
		get
		{
			return OVRPlugin.eyeHeight;
		}
	}

	public float eyeDepth
	{
		get
		{
			return OVRPlugin.eyeDepth;
		}
	}

	public float neckHeight
	{
		get
		{
			return eyeHeight - 0.075f;
		}
	}

	[Obsolete]
	public State state
	{
		get
		{
			return State.READY;
		}
	}
}
