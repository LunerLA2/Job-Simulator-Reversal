using UnityEngine;

namespace OwlchemyVR
{
	public class HandController : MonoBehaviour
	{
		public enum HandControllerButton
		{
			None = 0,
			Trigger = 1,
			Grip = 2,
			Trackpad = 3,
			Joystick = 4,
			AButton = 5,
			BButton = 6,
			XButton = 7,
			YButton = 8,
			GrabCustom = 9,
			InteractCustom = 10,
			Menu = 11
		}

		public enum HandControllerType
		{
			None = 0,
			ViveController = 1,
			OculusController = 2,
			SteamVRCompatibleController = 3,
			MorpheusController = 4
		}

		private enum ButtonStates
		{
			Current = 0,
			Up = 1,
			Down = 2
		}

		private bool isControllerReady;

		private HandControllerType controllerType;

		private SteamVR_IndividualController viveController;

		private SteamVRCompatible_IndividualController steamVRCompatibleHardwareController;

		private OculusTouch_IndividualController oculusTouchController;

		private Morpheus_IndividualController morpheusController;

		private SteamVR_TrackedController trackedObject;

		public HandControllerType ControllerType
		{
			get
			{
				return controllerType;
			}
		}

		public SteamVRCompatible_IndividualController SteamVRCompatibleController
		{
			get
			{
				return steamVRCompatibleHardwareController;
			}
		}

		public OculusTouch_IndividualController OculusTouchController
		{
			get
			{
				return oculusTouchController;
			}
		}

		private void Awake()
		{
			viveController = GetComponent<SteamVR_IndividualController>();
			oculusTouchController = GetComponent<OculusTouch_IndividualController>();
			morpheusController = GetComponent<Morpheus_IndividualController>();
			steamVRCompatibleHardwareController = GetComponent<SteamVRCompatible_IndividualController>();
		}

		public bool IsControllerReady()
		{
			if (isControllerReady)
			{
				return true;
			}
			if (steamVRCompatibleHardwareController != null && steamVRCompatibleHardwareController.IsControllerReady)
			{
				isControllerReady = true;
				controllerType = HandControllerType.SteamVRCompatibleController;
				return true;
			}
			if (viveController != null && viveController.IsControllerReady)
			{
				isControllerReady = true;
				controllerType = HandControllerType.ViveController;
				return true;
			}
			if (oculusTouchController != null && oculusTouchController.IsControllerReady)
			{
				isControllerReady = true;
				controllerType = HandControllerType.OculusController;
				return true;
			}
			if (morpheusController != null && morpheusController.IsControllerReady)
			{
				isControllerReady = true;
				controllerType = HandControllerType.MorpheusController;
				return true;
			}
			return false;
		}

		private void Update()
		{
			if (IsControllerReady())
			{
			}
		}

		private void LateUpdate()
		{
			if (IsControllerReady())
			{
			}
		}

		public Vector2 GetTrackPadPos()
		{
			if (IsControllerReady())
			{
				if (controllerType == HandControllerType.ViveController)
				{
					return GetTrackPadPosSteamController();
				}
				Debug.LogError("ControllerType is not set");
				return Vector2.zero;
			}
			return Vector2.zero;
		}

		public Vector2 GetTrackPadPosPercentage()
		{
			Vector2 trackPadPos = GetTrackPadPos();
			trackPadPos.x /= 32767f;
			trackPadPos.y /= 32767f;
			return trackPadPos;
		}

		public float GetTrackPadAngleFromCenter()
		{
			Vector2 trackPadPos = GetTrackPadPos();
			return (57.29578f * Mathf.Atan2(trackPadPos.y, trackPadPos.x) + 360f) % 360f;
		}

		public void TriggerHapticPulse(float usDurationMicroSec)
		{
			if (IsControllerReady())
			{
				if (controllerType == HandControllerType.ViveController)
				{
					viveController.TriggerHapticPulse(usDurationMicroSec);
				}
				else if (controllerType == HandControllerType.OculusController)
				{
					oculusTouchController.TriggerHapticPulse(usDurationMicroSec);
				}
				else if (controllerType == HandControllerType.MorpheusController)
				{
					morpheusController.TriggerHapticsPulse(usDurationMicroSec);
				}
				else if (controllerType == HandControllerType.SteamVRCompatibleController)
				{
					steamVRCompatibleHardwareController.TriggerHapticPulse(usDurationMicroSec);
				}
			}
		}

		public void ClearHaptics()
		{
			if (IsControllerReady() && controllerType != HandControllerType.ViveController)
			{
				if (controllerType == HandControllerType.OculusController)
				{
					oculusTouchController.ClearHaptics();
				}
				else if (controllerType == HandControllerType.MorpheusController)
				{
					morpheusController.ClearHaptics();
				}
			}
		}

		public float GetButtonRaw(HandControllerButton handControllerButton)
		{
			return GetGrabAxisValue(handControllerButton);
		}

		public bool GetButtonUp(HandControllerButton handControllerButton)
		{
			return GetButtonDetailed(handControllerButton, ButtonStates.Up, false);
		}

		public bool GetButtonDown(HandControllerButton handControllerButton)
		{
			return GetButtonDetailed(handControllerButton, ButtonStates.Down, false);
		}

		public bool GetButton(HandControllerButton handControllerButton)
		{
			return GetButtonDetailed(handControllerButton, ButtonStates.Current, false);
		}

		public bool GetButtonTouchUp(HandControllerButton handControllerButton)
		{
			return GetButtonDetailed(handControllerButton, ButtonStates.Up, true);
		}

		public bool GetButtonTouchDown(HandControllerButton handControllerButton)
		{
			return GetButtonDetailed(handControllerButton, ButtonStates.Down, true);
		}

		public bool GetButtonTouch(HandControllerButton handControllerButton)
		{
			return GetButtonDetailed(handControllerButton, ButtonStates.Current, true);
		}

		private bool GetButtonDetailed(HandControllerButton handControllerButton, ButtonStates state, bool isTouch)
		{
			if (IsControllerReady())
			{
				if (VRPlatform.GetCurrVRPlatformType() == VRPlatformTypes.SteamVR)
				{
					return GetSteamButtonDetailed(handControllerButton, state, controllerType);
				}
				if (VRPlatform.GetCurrVRPlatformType() == VRPlatformTypes.Oculus)
				{
					return GetOculusButtonDetailed(handControllerButton, state, isTouch);
				}
				if (VRPlatform.GetCurrVRPlatformType() == VRPlatformTypes.PSVR)
				{
					return GetMorpheusButtonDetailed(handControllerButton, state, isTouch);
				}
				Debug.LogError("ControllerType is not set or unrecognized platform detected");
				return false;
			}
			return false;
		}

		private float GetGrabAxisValue(HandControllerButton handControllerButton)
		{
			if (IsControllerReady())
			{
				if (VRPlatform.GetCurrVRPlatformType() == VRPlatformTypes.SteamVR)
				{
					return steamVRCompatibleHardwareController.GetGrabAxisValue();
				}
				if (VRPlatform.GetCurrVRPlatformType() == VRPlatformTypes.Oculus)
				{
					return GetOculusButtonDetailedRaw(handControllerButton);
				}
			}
			return 0f;
		}

		private bool GetMorpheusButtonDetailed(HandControllerButton handControllerButton, ButtonStates state, bool isTouch)
		{
			if (!isTouch)
			{
				switch (state)
				{
				case ButtonStates.Up:
					return morpheusController.GetButtonUp(handControllerButton);
				case ButtonStates.Down:
					return morpheusController.GetButtonDown(handControllerButton);
				case ButtonStates.Current:
					return morpheusController.GetButton(handControllerButton);
				default:
					Debug.LogWarning("Unknown button state type:" + state);
					return false;
				}
			}
			return false;
		}

		public HandControllerButton GetCustomHandControllerButtonToRealButton(HandControllerButton button)
		{
			if (IsControllerReady())
			{
				if (VRPlatform.GetCurrVRPlatformType() == VRPlatformTypes.SteamVR)
				{
					return HandControllerButton.None;
				}
				if (VRPlatform.GetCurrVRPlatformType() == VRPlatformTypes.Oculus && controllerType == HandControllerType.OculusController)
				{
					return oculusTouchController.GetCustomHandControllerButtonToRealButton(button);
				}
			}
			return HandControllerButton.None;
		}

		private float GetOculusButtonDetailedRaw(HandControllerButton handControllerButton)
		{
			return oculusTouchController.GetButtonRaw(handControllerButton);
		}

		private bool GetOculusButtonDetailed(HandControllerButton handControllerButton, ButtonStates state, bool isTouch)
		{
			if (!isTouch)
			{
				switch (state)
				{
				case ButtonStates.Up:
					return oculusTouchController.GetButtonUp(handControllerButton);
				case ButtonStates.Down:
					return oculusTouchController.GetButtonDown(handControllerButton);
				case ButtonStates.Current:
					return oculusTouchController.GetButton(handControllerButton);
				default:
					Debug.LogWarning("Unknown button state type:" + state);
					return false;
				}
			}
			switch (state)
			{
			case ButtonStates.Up:
				return oculusTouchController.GetTouchButtonUp(handControllerButton);
			case ButtonStates.Down:
				return oculusTouchController.GetTouchButtonDown(handControllerButton);
			case ButtonStates.Current:
				return oculusTouchController.GetTouchButton(handControllerButton);
			default:
				Debug.LogWarning("Unknown button state type:" + state);
				return false;
			}
		}

		private float TestTrigger()
		{
			if (Input.GetKey(KeyCode.Space))
			{
				return 0f;
			}
			return 1f;
		}

		private bool GetSteamButtonDetailed(HandControllerButton handControllerButton, ButtonStates state, HandControllerType controllerType)
		{
			SteamVR_IndividualController.SteamControllerButton button = SteamControllerButtonFromHandControllerButton(handControllerButton);
			switch (controllerType)
			{
			case HandControllerType.ViveController:
				switch (state)
				{
				case ButtonStates.Up:
					return viveController.GetButtonUp(button);
				case ButtonStates.Down:
					return viveController.GetButtonDown(button);
				case ButtonStates.Current:
					return viveController.GetButton(button);
				default:
					Debug.LogWarning("Unknown button state type:" + state);
					return false;
				}
			case HandControllerType.SteamVRCompatibleController:
				switch (state)
				{
				case ButtonStates.Up:
					return steamVRCompatibleHardwareController.GetButtonUp(button);
				case ButtonStates.Down:
					return steamVRCompatibleHardwareController.GetButtonDown(button);
				case ButtonStates.Current:
					return steamVRCompatibleHardwareController.GetButton(button);
				default:
					Debug.LogWarning("Unknown button state type:" + state);
					return false;
				}
			case HandControllerType.OculusController:
				switch (state)
				{
				case ButtonStates.Up:
					return steamVRCompatibleHardwareController.GetButtonTouchUp(button);
				case ButtonStates.Down:
					return steamVRCompatibleHardwareController.GetButtonTouchDown(button);
				case ButtonStates.Current:
					return steamVRCompatibleHardwareController.GetButtonTouch(button);
				default:
					Debug.LogWarning("Unknown button state type:" + state);
					return false;
				}
			default:
				Debug.LogError("Can't find logic for the controller hardware you're using!");
				return false;
			}
		}

		private Vector2 GetTrackPadPosSteamController()
		{
			return viveController.GetTrackPadPos();
		}

		private bool GetButtonUpSteamController(HandControllerButton handControllerButton)
		{
			return viveController.GetButtonUp(SteamControllerButtonFromHandControllerButton(handControllerButton));
		}

		private bool GetButtonDownSteamController(HandControllerButton handControllerButton)
		{
			return viveController.GetButtonDown(SteamControllerButtonFromHandControllerButton(handControllerButton));
		}

		private bool GetButtonSteamController(HandControllerButton handControllerButton)
		{
			return viveController.GetButton(SteamControllerButtonFromHandControllerButton(handControllerButton));
		}

		private bool GetButtonTouchUpSteamController(HandControllerButton handControllerButton)
		{
			return viveController.GetButtonTouchUp(SteamControllerButtonFromHandControllerButton(handControllerButton));
		}

		private bool GetButtonTouchDownSteamController(HandControllerButton handControllerButton)
		{
			return viveController.GetButtonTouchDown(SteamControllerButtonFromHandControllerButton(handControllerButton));
		}

		private bool GetButtonTouchSteamController(HandControllerButton handControllerButton)
		{
			return viveController.GetButtonTouch(SteamControllerButtonFromHandControllerButton(handControllerButton));
		}

		private SteamVR_IndividualController.SteamControllerButton SteamControllerButtonFromHandControllerButton(HandControllerButton handControllerButton)
		{
			switch (handControllerButton)
			{
			case HandControllerButton.Trigger:
				return SteamVR_IndividualController.SteamControllerButton.Trigger;
			case HandControllerButton.Grip:
				return SteamVR_IndividualController.SteamControllerButton.Grip;
			case HandControllerButton.Trackpad:
				return SteamVR_IndividualController.SteamControllerButton.Trackpad;
			case HandControllerButton.GrabCustom:
				return SteamVR_IndividualController.SteamControllerButton.Trigger;
			case HandControllerButton.InteractCustom:
				return SteamVR_IndividualController.SteamControllerButton.GripOrTrackpad;
			case HandControllerButton.Menu:
				return SteamVR_IndividualController.SteamControllerButton.Menu;
			default:
				Debug.LogError(string.Concat("Unmappable steam controller button, defaulting to trigger (", handControllerButton, ")"));
				return SteamVR_IndividualController.SteamControllerButton.Trigger;
			}
		}
	}
}
