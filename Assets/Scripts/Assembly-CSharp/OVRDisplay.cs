using System;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.VR;

public class OVRDisplay
{
	public struct EyeRenderDesc
	{
		public Vector2 resolution;

		public Vector2 fov;
	}

	public struct LatencyData
	{
		public float render;

		public float timeWarp;

		public float postPresent;

		public float renderError;

		public float timeWarpError;
	}

	private bool needsConfigureTexture;

	private EyeRenderDesc[] eyeDescs = new EyeRenderDesc[2];

	public Vector3 acceleration
	{
		get
		{
			if (!OVRManager.isHmdPresent)
			{
				return Vector3.zero;
			}
			return OVRPlugin.GetEyeAcceleration(OVRPlugin.Eye.None).ToOVRPose().position;
		}
	}

	public Quaternion angularAcceleration
	{
		get
		{
			if (!OVRManager.isHmdPresent)
			{
				return Quaternion.identity;
			}
			return OVRPlugin.GetEyeAcceleration(OVRPlugin.Eye.None).ToOVRPose().orientation;
		}
	}

	public Vector3 velocity
	{
		get
		{
			if (!OVRManager.isHmdPresent)
			{
				return Vector3.zero;
			}
			return OVRPlugin.GetEyeVelocity(OVRPlugin.Eye.None).ToOVRPose().position;
		}
	}

	public Quaternion angularVelocity
	{
		get
		{
			if (!OVRManager.isHmdPresent)
			{
				return Quaternion.identity;
			}
			return OVRPlugin.GetEyeVelocity(OVRPlugin.Eye.None).ToOVRPose().orientation;
		}
	}

	public LatencyData latency
	{
		get
		{
			if (!OVRManager.isHmdPresent)
			{
				return default(LatencyData);
			}
			string input = OVRPlugin.latency;
			Regex regex = new Regex("Render: ([0-9]+[.][0-9]+)ms, TimeWarp: ([0-9]+[.][0-9]+)ms, PostPresent: ([0-9]+[.][0-9]+)ms", RegexOptions.None);
			LatencyData result = default(LatencyData);
			Match match = regex.Match(input);
			if (match.Success)
			{
				result.render = float.Parse(match.Groups[1].Value);
				result.timeWarp = float.Parse(match.Groups[2].Value);
				result.postPresent = float.Parse(match.Groups[3].Value);
			}
			return result;
		}
	}

	public int recommendedMSAALevel
	{
		get
		{
			int num = OVRPlugin.recommendedMSAALevel;
			if (num == 1)
			{
				num = 0;
			}
			return num;
		}
	}

	public event Action RecenteredPose;

	public OVRDisplay()
	{
		UpdateTextures();
	}

	public void Update()
	{
		UpdateTextures();
	}

	public void RecenterPose()
	{
		UnityEngine.XR.InputTracking.Recenter();
		if (this.RecenteredPose != null)
		{
			this.RecenteredPose();
		}
	}

	public EyeRenderDesc GetEyeRenderDesc(UnityEngine.XR.XRNode eye)
	{
		return eyeDescs[(int)eye];
	}

	private void UpdateTextures()
	{
		ConfigureEyeDesc(UnityEngine.XR.XRNode.LeftEye);
		ConfigureEyeDesc(UnityEngine.XR.XRNode.RightEye);
	}

	private void ConfigureEyeDesc(UnityEngine.XR.XRNode eye)
	{
		if (OVRManager.isHmdPresent)
		{
			OVRPlugin.Sizei eyeTextureSize = OVRPlugin.GetEyeTextureSize((OVRPlugin.Eye)eye);
			OVRPlugin.Frustumf eyeFrustum = OVRPlugin.GetEyeFrustum((OVRPlugin.Eye)eye);
			eyeDescs[(int)eye] = new EyeRenderDesc
			{
				resolution = new Vector2(eyeTextureSize.w, eyeTextureSize.h),
				fov = 57.29578f * new Vector2(eyeFrustum.fovX, eyeFrustum.fovY)
			};
		}
	}
}
