using UnityEngine;

namespace OwlchemyVR
{
	public class OculusTouch_IndividualController : MonoBehaviour
	{
		private bool isGripTriggerButtonFlip = true;

		private bool isControllerReady;

		private float hapticsTriggerTime;

		private OVRInput.Controller controllerType;

		public bool IsControllerReady
		{
			get
			{
				return isControllerReady;
			}
		}

		public OVRInput.Controller ControllerType
		{
			get
			{
				return controllerType;
			}
		}

		public void Setup(bool isLeft)
		{
			isControllerReady = true;
			if (isLeft)
			{
				controllerType = OVRInput.Controller.LTouch;
			}
			else
			{
				controllerType = OVRInput.Controller.RTouch;
			}
		}

		private void Update()
		{
			if (isControllerReady && Input.GetKeyDown(KeyCode.Alpha9))
			{
				isGripTriggerButtonFlip = !isGripTriggerButtonFlip;
			}
		}

		public void TriggerHapticPulse(float usDurationMicroSec)
		{
			if (usDurationMicroSec > 0f)
			{
				float num = 5f;
				float amplitude = Mathf.Min(usDurationMicroSec / 4000f * num, 1f);
				OVRInput.SetControllerVibration(0.48f, amplitude, controllerType);
			}
		}

		public void ClearHaptics()
		{
			OVRInput.SetControllerVibration(0f, 0f, controllerType);
		}

		public float GetButtonRaw(HandController.HandControllerButton button)
		{
			return GetButton1DAxis(ButtonAxis1DMask(button));
		}

		private float GetButton1DAxis(OVRInput.Axis1D buttonAxis1D)
		{
			return OVRInput.Get(buttonAxis1D, controllerType);
		}

		public bool GetButton(HandController.HandControllerButton button)
		{
			if (button == HandController.HandControllerButton.Menu)
			{
				return GetButton(OVRInput.Button.Start);
			}
			return GetButton(ButtonMask(button));
		}

		public bool GetButtonDown(HandController.HandControllerButton button)
		{
			if (button == HandController.HandControllerButton.Menu)
			{
				return GetButtonDown(OVRInput.Button.Start);
			}
			return GetButtonDown(ButtonMask(button));
		}

		public bool GetButtonUp(HandController.HandControllerButton button)
		{
			if (button == HandController.HandControllerButton.Menu)
			{
				return GetButtonUp(OVRInput.Button.Start);
			}
			return GetButtonUp(ButtonMask(button));
		}

		private bool GetButtonDown(OVRInput.Button button)
		{
			return OVRInput.GetDown(button, controllerType);
		}

		private bool GetButton(OVRInput.Button button)
		{
			return OVRInput.Get(button, controllerType);
		}

		private bool GetButtonUp(OVRInput.Button button)
		{
			return OVRInput.GetUp(button, controllerType);
		}

		public bool GetTouchButton(HandController.HandControllerButton touch)
		{
			return GetButtonTouch(TouchMask(touch));
		}

		public bool GetTouchButtonDown(HandController.HandControllerButton touch)
		{
			return GetButtonTouchDown(TouchMask(touch));
		}

		public bool GetTouchButtonUp(HandController.HandControllerButton touch)
		{
			return GetButtonTouchUp(TouchMask(touch));
		}

		private bool GetButtonTouch(OVRInput.Touch touch)
		{
			return OVRInput.Get(touch, controllerType);
		}

		private bool GetButtonTouchUp(OVRInput.Touch touch)
		{
			return OVRInput.GetUp(touch, controllerType);
		}

		private bool GetButtonTouchDown(OVRInput.Touch touch)
		{
			return OVRInput.GetDown(touch, controllerType);
		}

		private OVRInput.Touch TouchMask(HandController.HandControllerButton touch)
		{
			touch = GetCustomHandControllerButtonToRealButton(touch);
			switch (touch)
			{
			case HandController.HandControllerButton.Trigger:
				return OVRInput.Touch.PrimaryIndexTrigger;
			case HandController.HandControllerButton.Joystick:
				return OVRInput.Touch.PrimaryThumbstick;
			case HandController.HandControllerButton.AButton:
				return OVRInput.Touch.One;
			case HandController.HandControllerButton.BButton:
				return OVRInput.Touch.Two;
			case HandController.HandControllerButton.XButton:
				return OVRInput.Touch.Three;
			case HandController.HandControllerButton.YButton:
				return OVRInput.Touch.Four;
			default:
				return OVRInput.Touch.None;
			}
		}

		public HandController.HandControllerButton GetCustomHandControllerButtonToRealButton(HandController.HandControllerButton button)
		{
			switch (button)
			{
			case HandController.HandControllerButton.GrabCustom:
				if (isGripTriggerButtonFlip)
				{
					return HandController.HandControllerButton.Grip;
				}
				return HandController.HandControllerButton.Trigger;
			case HandController.HandControllerButton.InteractCustom:
				if (isGripTriggerButtonFlip)
				{
					return HandController.HandControllerButton.Trigger;
				}
				return HandController.HandControllerButton.Grip;
			default:
				return button;
			}
		}

		public OVRInput.Button ButtonMask(HandController.HandControllerButton button)
		{
			button = GetCustomHandControllerButtonToRealButton(button);
			switch (button)
			{
			case HandController.HandControllerButton.Trigger:
				return OVRInput.Button.PrimaryIndexTrigger;
			case HandController.HandControllerButton.Grip:
				return OVRInput.Button.PrimaryHandTrigger;
			default:
				return OVRInput.Button.None;
			}
		}

		private OVRInput.Axis1D ButtonAxis1DMask(HandController.HandControllerButton button)
		{
			button = GetCustomHandControllerButtonToRealButton(button);
			switch (button)
			{
			case HandController.HandControllerButton.Trigger:
				return OVRInput.Axis1D.PrimaryIndexTrigger;
			case HandController.HandControllerButton.Grip:
				return OVRInput.Axis1D.PrimaryHandTrigger;
			default:
				return OVRInput.Axis1D.None;
			}
		}

		public bool IsControllerTracked()
		{
			if (OVRInput.GetControllerOrientationTracked(controllerType) && OVRInput.GetControllerPositionTracked(controllerType))
			{
				return true;
			}
			return false;
		}
	}
}
