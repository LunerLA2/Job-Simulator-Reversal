using System.Collections;
using UnityEngine;

namespace OwlchemyVR
{
	public class SteamVRCompatible_IndividualController : SteamVR_IndividualController
	{
		private const ulong TOUCH_A_OR_X_BUTTON_MASK = 128uL;

		private const ulong TOUCH_B_OR_Y_BUTTON_MASK = 2uL;

		private const ulong TOUCH_JOYSTICK_BUTTON_MASK = 4294967296uL;

		private const ulong TOUCH_FRONT_TRIGGER_MASK = 8589934592uL;

		private const ulong TOUCH_THUMB_REST_TOUCH_MASK = 4uL;

		private const ulong DEFAULT_TOUCHPAD_BUTTON_MASK = 4294967296uL;

		private const ulong DEFAULT_GRIP_BUTTON_MASK = 4uL;

		private const ulong DEFAULT_TRIGGER_BUTTON_MASK = 8589934592uL;

		private const ulong DEFAULT_MENU_BUTTON_MASK = 2uL;

		private const ulong WINDOWS_TOUCHPAD_BUTTON_MASK = 4294967296uL;

		private const ulong WINDOWS_GRIP_BUTTON_MASK = 4uL;

		private const ulong WINDOWS_TRIGGER_BUTTON_MASK = 8589934592uL;

		private const ulong WINDOWS_MENU_BUTTON_MASK = 2uL;

		private void Start()
		{
			if (VRPlatform.GetCurrVRPlatformHardwareType() == VRPlatformHardwareType.SteamVRCompatible)
			{
				Debug.LogWarning("Explicit device hardware definition not found! Input may not be accurate...");
			}
		}

		private IEnumerator HapticTest()
		{
			while (true)
			{
				yield return new WaitForSeconds(0.5f);
				base.TriggerHapticPulse(10000f);
			}
		}

		protected override void Update()
		{
			base.Update();
		}

		public override void TriggerHapticPulse(float usDurationMicroSec)
		{
			if (VRPlatform.GetCurrVRPlatformHardwareType() == VRPlatformHardwareType.OculusRift)
			{
				float num = 650f;
				float num2 = 2500f;
				float num3 = 200f;
				float num4 = 650f;
				float usDurationMicroSec2 = (usDurationMicroSec - num3) * (num2 - num) / (num4 - num3) + num;
				base.TriggerHapticPulse(usDurationMicroSec2);
			}
			else
			{
				base.TriggerHapticPulse(usDurationMicroSec);
			}
		}

		public override float GetGrabAxisValue()
		{
			float result = 0f;
			switch (VRPlatform.GetCurrVRPlatformHardwareType())
			{
			case VRPlatformHardwareType.OculusRift:
				result = currentState.rAxis2.x;
				break;
			case VRPlatformHardwareType.WindowsMR:
				result = currentState.rAxis1.x;
				break;
			case VRPlatformHardwareType.SteamVRCompatible:
				result = currentState.rAxis1.x;
				break;
			}
			return result;
		}

		public float GetGrabAxisValueLastFrame()
		{
			float result = 0f;
			switch (VRPlatform.GetCurrVRPlatformHardwareType())
			{
			case VRPlatformHardwareType.OculusRift:
				result = previousState.rAxis2.x;
				break;
			case VRPlatformHardwareType.WindowsMR:
				result = previousState.rAxis1.x;
				break;
			case VRPlatformHardwareType.SteamVRCompatible:
				result = previousState.rAxis1.x;
				break;
			}
			return result;
		}

		public bool IsControllerTracked()
		{
			return !SteamVR_Controller.Input((int)base.DeviceIndex).outOfRange;
		}

		public float GetPointerFingerAxisValue()
		{
			float result = 0f;
			Debug.Log(currentState.rAxis2.x);
			switch (VRPlatform.GetCurrVRPlatformHardwareType())
			{
			case VRPlatformHardwareType.OculusRift:
				result = currentState.rAxis1.x;
				break;
			case VRPlatformHardwareType.WindowsMR:
				result = currentState.rAxis1.x;
				break;
			case VRPlatformHardwareType.SteamVRCompatible:
				result = currentState.rAxis1.x;
				break;
			}
			return result;
		}

		public float GetPointerFingerAxisValueLastFrame()
		{
			float result = 0f;
			switch (VRPlatform.GetCurrVRPlatformHardwareType())
			{
			case VRPlatformHardwareType.OculusRift:
				result = previousState.rAxis1.x;
				break;
			case VRPlatformHardwareType.WindowsMR:
				result = previousState.rAxis1.x;
				break;
			case VRPlatformHardwareType.SteamVRCompatible:
				result = previousState.rAxis1.x;
				break;
			}
			return result;
		}

		public bool IsThumbTouching()
		{
			ulong mask = 1uL;
			switch (VRPlatform.GetCurrVRPlatformHardwareType())
			{
			case VRPlatformHardwareType.OculusRift:
				mask = 4294967430uL;
				break;
			case VRPlatformHardwareType.WindowsMR:
				mask = 4294967296uL;
				break;
			case VRPlatformHardwareType.SteamVRCompatible:
				mask = 4294967296uL;
				break;
			}
			return GetButtonTouch(mask);
		}

		public bool IsThumbPressed()
		{
			return GetButton(SteamControllerButton.Trackpad);
		}

		public bool IsTriggerPressed()
		{
			return GetButton(SteamControllerButton.Trigger);
		}

		public bool IsTriggerTouching()
		{
			ulong num = 1uL;
			VRPlatformHardwareType currVRPlatformHardwareType = VRPlatform.GetCurrVRPlatformHardwareType();
			if (currVRPlatformHardwareType == VRPlatformHardwareType.OculusRift)
			{
				num = 8589934592uL;
				return GetButtonTouch(num);
			}
			return true;
		}

		public bool IsCompanionCamCycleButtonUp()
		{
			if (VRPlatform.GetCurrVRPlatformHardwareType() == VRPlatformHardwareType.OculusRift)
			{
				return GetButtonUp(128uL);
			}
			return GetButtonUp(SteamControllerButton.Grip);
		}
	}
}
