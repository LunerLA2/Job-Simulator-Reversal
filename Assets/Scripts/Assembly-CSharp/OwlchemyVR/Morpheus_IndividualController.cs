using UnityEngine;

namespace OwlchemyVR
{
	public class Morpheus_IndividualController : MonoBehaviour
	{
		public enum MoveControllerButton
		{
			None = 1,
			Trigger = 2,
			Center = 4,
			Start = 8,
			Triangle = 0x10,
			Circle = 0x20,
			Cross = 0x40,
			Square = 0x80
		}

		private struct ControllerState
		{
			public int trackedControllerIndex;

			public int moveControllerIndex;

			public int buttonBits;

			private bool triggerAxis;

			public void UpdateBits()
			{
			}

			public bool GetButton(HandController.HandControllerButton button)
			{
				return false;
			}

			public bool GetButton(MoveControllerButton button)
			{
				return false;
			}

			private MoveControllerButton ButtonMask(HandController.HandControllerButton handControllerButton)
			{
				switch (handControllerButton)
				{
				case HandController.HandControllerButton.AButton:
					return MoveControllerButton.Cross;
				case HandController.HandControllerButton.BButton:
					return MoveControllerButton.Circle;
				case HandController.HandControllerButton.Menu:
					return MoveControllerButton.Start;
				case HandController.HandControllerButton.XButton:
					return MoveControllerButton.Square;
				case HandController.HandControllerButton.YButton:
					return MoveControllerButton.Triangle;
				case HandController.HandControllerButton.GrabCustom:
					return MoveControllerButton.Center;
				default:
					return MoveControllerButton.None;
				}
			}
		}

		private const float TRIGGER_ENGAGE_PERCENTAGE = 0.35f;

		private ControllerState previousState = default(ControllerState);

		private ControllerState currentState = default(ControllerState);

		private bool isControllerReady;

		public bool IsControllerReady
		{
			get
			{
				return isControllerReady;
			}
		}

		public void Setup(int trackedControllerIndex, int moveControllerIndex)
		{
			isControllerReady = true;
			currentState.trackedControllerIndex = trackedControllerIndex;
			currentState.moveControllerIndex = moveControllerIndex;
			currentState.UpdateBits();
		}

		private void Update()
		{
			if (isControllerReady)
			{
				previousState = currentState;
				currentState.UpdateBits();
			}
		}

		public bool GetButtonDown(HandController.HandControllerButton button)
		{
			return currentState.GetButton(button) && !previousState.GetButton(button);
		}

		public bool GetButtonUp(HandController.HandControllerButton button)
		{
			return !currentState.GetButton(button) && previousState.GetButton(button);
		}

		public bool GetButton(HandController.HandControllerButton button)
		{
			return currentState.GetButton(button);
		}

		public bool GetButtonDown(MoveControllerButton button)
		{
			return currentState.GetButton(button) && !previousState.GetButton(button);
		}

		public bool GetButtonUp(MoveControllerButton button)
		{
			return !currentState.GetButton(button) && previousState.GetButton(button);
		}

		public bool GetButton(MoveControllerButton button)
		{
			return currentState.GetButton(button);
		}

		public void TriggerHapticsPulse(float usDurationMicroSec)
		{
			int a = 62 + (int)(usDurationMicroSec / 3f);
			a = Mathf.Min(a, 85);
			SetActualHapticsValue(a);
		}

		public void ClearHaptics()
		{
			SetActualHapticsValue(0);
		}

		private void SetActualHapticsValue(int realHapticValue)
		{
		}
	}
}
