using UnityEngine;
using Valve.VR;

public class SteamVR_Controller
{
	public class ButtonMask
	{
		public const ulong System = 1uL;

		public const ulong ApplicationMenu = 2uL;

		public const ulong Grip = 4uL;

		public const ulong Axis0 = 4294967296uL;

		public const ulong Axis1 = 8589934592uL;

		public const ulong Axis2 = 17179869184uL;

		public const ulong Axis3 = 34359738368uL;

		public const ulong Axis4 = 68719476736uL;

		public const ulong Touchpad = 4294967296uL;

		public const ulong Trigger = 8589934592uL;
	}

	public class Device
	{
		private VRControllerState_t state;

		private VRControllerState_t prevState;

		private TrackedDevicePose_t pose;

		private int prevFrameCount = -1;

		public float hairTriggerDelta = 0.1f;

		private float hairTriggerLimit;

		private bool hairTriggerState;

		private bool hairTriggerPrevState;

		public uint index { get; private set; }

		public bool valid { get; private set; }

		public bool connected
		{
			get
			{
				Update();
				return pose.bDeviceIsConnected;
			}
		}

		public bool hasTracking
		{
			get
			{
				Update();
				return pose.bPoseIsValid;
			}
		}

		public bool outOfRange
		{
			get
			{
				Update();
				return pose.eTrackingResult == ETrackingResult.Running_OutOfRange || pose.eTrackingResult == ETrackingResult.Calibrating_OutOfRange;
			}
		}

		public bool calibrating
		{
			get
			{
				Update();
				return pose.eTrackingResult == ETrackingResult.Calibrating_InProgress || pose.eTrackingResult == ETrackingResult.Calibrating_OutOfRange;
			}
		}

		public bool uninitialized
		{
			get
			{
				Update();
				return pose.eTrackingResult == ETrackingResult.Uninitialized;
			}
		}

		public SteamVR_Utils.RigidTransform transform
		{
			get
			{
				Update();
				return new SteamVR_Utils.RigidTransform(pose.mDeviceToAbsoluteTracking);
			}
		}

		public Vector3 velocity
		{
			get
			{
				Update();
				return new Vector3(pose.vVelocity.v0, pose.vVelocity.v1, 0f - pose.vVelocity.v2);
			}
		}

		public Vector3 angularVelocity
		{
			get
			{
				Update();
				return new Vector3(0f - pose.vAngularVelocity.v0, 0f - pose.vAngularVelocity.v1, pose.vAngularVelocity.v2);
			}
		}

		public Device(uint i)
		{
			index = i;
		}

		public VRControllerState_t GetState()
		{
			Update();
			return state;
		}

		public VRControllerState_t GetPrevState()
		{
			Update();
			return prevState;
		}

		public TrackedDevicePose_t GetPose()
		{
			Update();
			return pose;
		}

		public void Update()
		{
			if (Time.frameCount != prevFrameCount)
			{
				prevFrameCount = Time.frameCount;
				prevState = state;
				CVRSystem system = OpenVR.System;
				if (system != null)
				{
					valid = system.GetControllerStateWithPose(SteamVR_Render.instance.trackingSpace, index, ref state, ref pose);
					UpdateHairTrigger();
				}
			}
		}

		public bool GetPress(ulong buttonMask)
		{
			Update();
			return (state.ulButtonPressed & buttonMask) != 0;
		}

		public bool GetPressDown(ulong buttonMask)
		{
			Update();
			return (state.ulButtonPressed & buttonMask) != 0L && (prevState.ulButtonPressed & buttonMask) == 0;
		}

		public bool GetPressUp(ulong buttonMask)
		{
			Update();
			return (state.ulButtonPressed & buttonMask) == 0L && (prevState.ulButtonPressed & buttonMask) != 0;
		}

		public bool GetPress(EVRButtonId buttonId)
		{
			return GetPress((ulong)(1L << (int)buttonId));
		}

		public bool GetPressDown(EVRButtonId buttonId)
		{
			return GetPressDown((ulong)(1L << (int)buttonId));
		}

		public bool GetPressUp(EVRButtonId buttonId)
		{
			return GetPressUp((ulong)(1L << (int)buttonId));
		}

		public bool GetTouch(ulong buttonMask)
		{
			Update();
			return (state.ulButtonTouched & buttonMask) != 0;
		}

		public bool GetTouchDown(ulong buttonMask)
		{
			Update();
			return (state.ulButtonTouched & buttonMask) != 0L && (prevState.ulButtonTouched & buttonMask) == 0;
		}

		public bool GetTouchUp(ulong buttonMask)
		{
			Update();
			return (state.ulButtonTouched & buttonMask) == 0L && (prevState.ulButtonTouched & buttonMask) != 0;
		}

		public bool GetTouch(EVRButtonId buttonId)
		{
			return GetTouch((ulong)(1L << (int)buttonId));
		}

		public bool GetTouchDown(EVRButtonId buttonId)
		{
			return GetTouchDown((ulong)(1L << (int)buttonId));
		}

		public bool GetTouchUp(EVRButtonId buttonId)
		{
			return GetTouchUp((ulong)(1L << (int)buttonId));
		}

		public Vector2 GetAxis(EVRButtonId buttonId = EVRButtonId.k_EButton_Axis0)
		{
			Update();
			switch ((uint)buttonId)
			{
			case 32u:
				return new Vector2(state.rAxis0.x, state.rAxis0.y);
			case 33u:
				return new Vector2(state.rAxis1.x, state.rAxis1.y);
			case 34u:
				return new Vector2(state.rAxis2.x, state.rAxis2.y);
			case 35u:
				return new Vector2(state.rAxis3.x, state.rAxis3.y);
			case 36u:
				return new Vector2(state.rAxis4.x, state.rAxis4.y);
			default:
				return Vector2.zero;
			}
		}

		public void TriggerHapticPulse(ushort durationMicroSec = 500, EVRButtonId buttonId = EVRButtonId.k_EButton_Axis0)
		{
			CVRSystem system = OpenVR.System;
			if (system != null)
			{
				uint unAxisId = (uint)(buttonId - 32);
				system.TriggerHapticPulse(index, unAxisId, (char)durationMicroSec);
			}
		}

		private void UpdateHairTrigger()
		{
			hairTriggerPrevState = hairTriggerState;
			float x = state.rAxis1.x;
			if (hairTriggerState)
			{
				if (x < hairTriggerLimit - hairTriggerDelta || x <= 0f)
				{
					hairTriggerState = false;
				}
			}
			else if (x > hairTriggerLimit + hairTriggerDelta || x >= 1f)
			{
				hairTriggerState = true;
			}
			hairTriggerLimit = ((!hairTriggerState) ? Mathf.Min(hairTriggerLimit, x) : Mathf.Max(hairTriggerLimit, x));
		}

		public bool GetHairTrigger()
		{
			Update();
			return hairTriggerState;
		}

		public bool GetHairTriggerDown()
		{
			Update();
			return hairTriggerState && !hairTriggerPrevState;
		}

		public bool GetHairTriggerUp()
		{
			Update();
			return !hairTriggerState && hairTriggerPrevState;
		}
	}

	public enum DeviceRelation
	{
		First = 0,
		Leftmost = 1,
		Rightmost = 2,
		FarthestLeft = 3,
		FarthestRight = 4
	}

	private static Device[] devices;

	public static Device Input(int deviceIndex)
	{
		if (devices == null)
		{
			devices = new Device[16];
			for (uint num = 0u; num < devices.Length; num++)
			{
				devices[num] = new Device(num);
			}
		}
		return devices[deviceIndex];
	}

	public static void Update()
	{
		for (int i = 0; (long)i < 16L; i++)
		{
			Input(i).Update();
		}
	}

	public static int GetDeviceIndex(DeviceRelation relation, ETrackedDeviceClass deviceClass = ETrackedDeviceClass.Controller, int relativeTo = 0)
	{
		int result = -1;
		SteamVR_Utils.RigidTransform rigidTransform = (((uint)relativeTo >= 16u) ? SteamVR_Utils.RigidTransform.identity : Input(relativeTo).transform.GetInverse());
		CVRSystem system = OpenVR.System;
		if (system == null)
		{
			return result;
		}
		float num = float.MinValue;
		for (int i = 0; (long)i < 16L; i++)
		{
			if (i == relativeTo || system.GetTrackedDeviceClass((uint)i) != deviceClass)
			{
				continue;
			}
			Device device = Input(i);
			if (device.connected)
			{
				if (relation == DeviceRelation.First)
				{
					return i;
				}
				Vector3 vector = rigidTransform * device.transform.pos;
				float num3;
				switch (relation)
				{
				case DeviceRelation.FarthestRight:
					num3 = vector.x;
					break;
				case DeviceRelation.FarthestLeft:
					num3 = 0f - vector.x;
					break;
				default:
				{
					Vector3 normalized = new Vector3(vector.x, 0f, vector.z).normalized;
					float num2 = Vector3.Dot(normalized, Vector3.forward);
					Vector3 vector2 = Vector3.Cross(normalized, Vector3.forward);
					num3 = ((relation != DeviceRelation.Leftmost) ? ((!(vector2.y < 0f)) ? num2 : (2f - num2)) : ((!(vector2.y > 0f)) ? num2 : (2f - num2)));
					break;
				}
				}
				if (num3 > num)
				{
					result = i;
					num = num3;
				}
			}
		}
		return result;
	}
}
