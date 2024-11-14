using System;
using UnityEngine;

namespace OwlchemyVR
{
	public class Hand : MonoBehaviour
	{
		public Transform ModelContainer;

		public Transform RotationContainer;

		private PlayerController playerController;

		private HandState suggestedState;

		private HandState prevSuggestedState;

		public Handedness Handedness { get; private set; }

		public Hand OtherHand
		{
			get
			{
				return (Handedness != Handedness.Right) ? playerController.RightHand : playerController.LeftHand;
			}
		}

		private void Awake()
		{
		}

		public void Init(PlayerController playerController, Handedness handedness)
		{
			this.playerController = playerController;
			Handedness = handedness;
			base.transform.SetParent(playerController.transform, false);
			base.name = string.Concat(handedness, "Hand");
			if (handedness == Handedness.Right)
			{
				Vector3 localEulerAngles = RotationContainer.localEulerAngles;
				localEulerAngles.z = 180f - localEulerAngles.z;
				RotationContainer.localEulerAngles = localEulerAngles;
				ModelContainer.localScale = new Vector3(-1f, 1f, 1f);
				Vector3 localPosition = ModelContainer.localPosition;
				localPosition.y = 0f - localPosition.y;
				ModelContainer.localPosition = localPosition;
			}
			suggestedState = new HandState
			{
				Position = base.transform.position,
				EulerAngles = base.transform.eulerAngles
			};
			prevSuggestedState = new HandState
			{
				Position = base.transform.position,
				EulerAngles = base.transform.eulerAngles
			};
		}

		private void Start()
		{
			suggestedState.Position = base.transform.position;
			suggestedState.EulerAngles = base.transform.rotation.eulerAngles;
			prevSuggestedState = suggestedState;
		}

		public void SuggestState(HandState suggestedState)
		{
			prevSuggestedState = this.suggestedState;
			if (suggestedState != null)
			{
				this.suggestedState = suggestedState;
				base.transform.position = suggestedState.Position;
				base.transform.rotation = Quaternion.Euler(suggestedState.EulerAngles);
			}
		}

		public bool GetCommonButton(CommonHandButton button)
		{
			return suggestedState.GetCommonButton((int)button);
		}

		public bool GetCommonButtonDown(CommonHandButton button)
		{
			return suggestedState.GetCommonButton((int)button) && !prevSuggestedState.GetCommonButton((int)button);
		}

		public bool GetCommonButtonUp(CommonHandButton button)
		{
			return !suggestedState.GetCommonButton((int)button) && prevSuggestedState.GetCommonButton((int)button);
		}

		public float GetCommonAxis(CommonHandAxis axis)
		{
			return suggestedState.GetCommonAxis((int)axis);
		}

		public Vector2 GetNavVector()
		{
			return GetCommonAxisVector2(CommonHandAxis.NavX, CommonHandAxis.NavY);
		}

		public Vector2 GetCommonAxisVector2(CommonHandAxis xAxis, CommonHandAxis yAxis)
		{
			return new Vector2(GetCommonAxis(xAxis), GetCommonAxis(yAxis));
		}

		public Vector3 GetCommonAxisVector3(CommonHandAxis xAxis, CommonHandAxis yAxis, CommonHandAxis zAxis)
		{
			return new Vector3(GetCommonAxis(xAxis), GetCommonAxis(yAxis), GetCommonAxis(zAxis));
		}

		public bool GetCustomButton(Enum button)
		{
			return suggestedState.GetCustomButton((int)(object)button);
		}

		public bool GetCustomButtonDown(Enum button)
		{
			return suggestedState.GetCustomButton((int)(object)button) && !prevSuggestedState.GetCustomButton((int)(object)button);
		}

		public bool GetCustomButtonUp(Enum button)
		{
			return !suggestedState.GetCustomButton((int)(object)button) && prevSuggestedState.GetCustomButton((int)(object)button);
		}

		public float GetCustomAxis(Enum axis)
		{
			return suggestedState.CustomAxes[(int)(object)axis];
		}

		public Vector2 GetCustomAxisVector2(Enum xAxis, Enum yAxis)
		{
			return new Vector2(GetCustomAxis(xAxis), GetCustomAxis(yAxis));
		}

		public Vector3 GetCustomAxisVector3(Enum xAxis, Enum yAxis, Enum zAxis)
		{
			return new Vector3(GetCustomAxis(xAxis), GetCustomAxis(yAxis), GetCustomAxis(zAxis));
		}

		public void TriggerHapticPulse(float pulseRate)
		{
			playerController.HandDevice.TriggerHapticPulse(this, pulseRate);
		}
	}
}
