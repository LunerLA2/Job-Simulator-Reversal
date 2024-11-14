using UnityEngine;
using Valve.VR;

namespace OwlchemyVR
{
	public class SteamVR_IndividualController : MonoBehaviour
	{
		public enum SteamControllerButton
		{
			Trigger = 0,
			Grip = 1,
			Trackpad = 2,
			Steam = 3,
			GripOrTrackpad = 4,
			Menu = 5
		}

		private const float TRIGGER_ENGAGED_PERCENTAGE = 0.3f;

		private const int TRIGGER_R_AXIS = 1;

		protected VRControllerState_t currentState = default(VRControllerState_t);

		protected VRControllerState_t previousState = default(VRControllerState_t);

		private bool isControllerReady;

		private uint deviceIndex = uint.MaxValue;

		public VRControllerState_t CurrentState
		{
			get
			{
				return currentState;
			}
		}

		public VRControllerState_t PreviousState
		{
			get
			{
				return previousState;
			}
		}

		public bool IsControllerReady
		{
			get
			{
				return isControllerReady;
			}
		}

		public uint DeviceIndex
		{
			get
			{
				return deviceIndex;
			}
		}

		public bool IsDeviceFound
		{
			get
			{
				return deviceIndex < uint.MaxValue;
			}
		}

		public void Setup(SteamVR_TrackedObject trackedObject)
		{
			deviceIndex = (uint)trackedObject.index;
			isControllerReady = true;
			SteamVR.instance.hmd.GetControllerState(deviceIndex, ref currentState);
		}

		protected virtual void Update()
		{
			if (isControllerReady)
			{
				previousState = currentState;
				SteamVR.instance.hmd.GetControllerState(deviceIndex, ref currentState);
			}
		}

		public virtual void TriggerHapticPulse(float usDurationMicroSec)
		{
			SteamVR.instance.hmd.TriggerHapticPulse(deviceIndex, 0u, (char)usDurationMicroSec);
		}

		public Vector2 GetTrackPadPos()
		{
			VRControllerAxis_t rAxis = currentState.rAxis0;
			return new Vector2(rAxis.x, rAxis.y);
		}

		public bool GetButton(SteamControllerButton button)
		{
			if (button == SteamControllerButton.Trigger)
			{
				if (currentState.rAxis1.x >= 0.3f)
				{
					return true;
				}
				return false;
			}
			return GetButton(ButtonMask(button));
		}

		public virtual float GetGrabAxisValue()
		{
			return currentState.rAxis1.x;
		}

		public bool GetButtonDown(SteamControllerButton button)
		{
			if (button == SteamControllerButton.Trigger)
			{
				if (currentState.rAxis1.x >= 0.3f && previousState.rAxis1.x < 0.3f)
				{
					return true;
				}
				return false;
			}
			return GetButtonDown(ButtonMask(button));
		}

		public bool GetButtonUp(SteamControllerButton button)
		{
			if (button == SteamControllerButton.Trigger)
			{
				if (currentState.rAxis1.x < 0.3f && previousState.rAxis1.x >= 0.3f)
				{
					return true;
				}
				return false;
			}
			return GetButtonUp(ButtonMask(button));
		}

		private bool GetButtonDown(ulong mask)
		{
			bool flag = (currentState.ulButtonPressed & mask) != 0;
			bool flag2 = (previousState.ulButtonPressed & mask) != 0;
			return flag && !flag2;
		}

		protected bool GetButton(ulong mask)
		{
			return (currentState.ulButtonPressed & mask) != 0;
		}

		protected bool GetButtonUp(ulong mask)
		{
			bool flag = (currentState.ulButtonPressed & mask) != 0;
			return (previousState.ulButtonPressed & mask) != 0 && !flag;
		}

		public bool GetButtonTouch(SteamControllerButton button)
		{
			return GetButtonTouch(ButtonMask(button));
		}

		public bool GetButtonTouchDown(SteamControllerButton button)
		{
			return GetButtonTouchDown(ButtonMask(button));
		}

		public bool GetButtonTouchUp(SteamControllerButton button)
		{
			return GetButtonTouchUp(ButtonMask(button));
		}

		protected bool GetButtonTouchDown(ulong mask)
		{
			bool flag = (currentState.ulButtonTouched & mask) != 0;
			bool flag2 = (previousState.ulButtonTouched & mask) != 0;
			return flag && !flag2;
		}

		protected bool GetButtonTouch(ulong mask)
		{
			return (currentState.ulButtonTouched & mask) != 0;
		}

		protected bool GetButtonTouchUp(ulong mask)
		{
			bool flag = (currentState.ulButtonTouched & mask) != 0;
			return (previousState.ulButtonTouched & mask) != 0 && !flag;
		}

		protected ulong ButtonMask(SteamControllerButton button)
		{
			switch (button)
			{
			case SteamControllerButton.Trigger:
				return 8589934592uL;
			case SteamControllerButton.Grip:
				return 4uL;
			case SteamControllerButton.Trackpad:
				return 4294967296uL;
			case SteamControllerButton.Steam:
				return 2uL;
			case SteamControllerButton.GripOrTrackpad:
				return 4294967300uL;
			case SteamControllerButton.Menu:
				return 2uL;
			default:
				return 0uL;
			}
		}
	}
}
