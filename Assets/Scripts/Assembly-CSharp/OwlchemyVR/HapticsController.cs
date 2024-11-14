using System.Collections.Generic;
using UnityEngine;

namespace OwlchemyVR
{
	public class HapticsController : MonoBehaviour
	{
		[SerializeField]
		private HapticsAnimationData hapticsAnimationData;

		private HandController handController;

		private List<HapticInfoObject> activeHaptics = new List<HapticInfoObject>();

		private bool isPaused;

		public HapticsAnimationData HapticAnimationData
		{
			get
			{
				return hapticsAnimationData;
			}
		}

		public void SetIsPaused(bool isPaused)
		{
			this.isPaused = isPaused;
		}

		public void Init(HandController handController)
		{
			this.handController = handController;
		}

		public HapticInfoObject AddNewHaptic(HapticInfoObject newHaptic)
		{
			activeHaptics.Add(newHaptic);
			return newHaptic;
		}

		public bool ContainHaptic(HapticInfoObject haptic)
		{
			return activeHaptics.Contains(haptic);
		}

		public void RemoveHaptic(HapticInfoObject haptic)
		{
			activeHaptics.Remove(haptic);
		}

		public HapticInfoObject BeginBasicHapticNoLength(float pulseRateMicroSec)
		{
			return BeginBasicHaptic(pulseRateMicroSec, float.PositiveInfinity);
		}

		public HapticInfoObject BeginBasicHaptic(float pulseRateMicroSec, float length)
		{
			HapticInfoObject hapticInfoObject = new HapticInfoObject(pulseRateMicroSec, length);
			activeHaptics.Add(hapticInfoObject);
			return hapticInfoObject;
		}

		public void HapticsManualUpdate()
		{
			if (isPaused)
			{
				return;
			}
			if (activeHaptics.Count > 0)
			{
				float deltaTime = Time.deltaTime;
				float num = 0f;
				for (int i = 0; i < activeHaptics.Count; i++)
				{
					HapticInfoObject hapticInfoObject = activeHaptics[i];
					if (hapticInfoObject.IsRunning)
					{
						hapticInfoObject.RunHapticsUpdate(deltaTime);
						if (hapticInfoObject.GetCurrPulseRateMicroSec() > num)
						{
							num = hapticInfoObject.GetCurrPulseRateMicroSec();
						}
					}
					else if (!hapticInfoObject.IsPermanent)
					{
						activeHaptics.RemoveAt(i);
						i--;
					}
				}
				if (num > 0f)
				{
					handController.TriggerHapticPulse(num);
				}
				else
				{
					handController.ClearHaptics();
				}
			}
			else
			{
				handController.ClearHaptics();
			}
		}

		public void ManuallyClearHaptics()
		{
			handController.ClearHaptics();
		}
	}
}
