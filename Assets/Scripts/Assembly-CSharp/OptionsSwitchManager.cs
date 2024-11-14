using System;
using System.Collections;
using UnityEngine;

public class OptionsSwitchManager : MonoBehaviour
{
	[SerializeField]
	private GrabbableSlider heightSlider;

	[SerializeField]
	private GrabbableSlider spectatorSlider;

	[SerializeField]
	private GrabbableHinge optionsDoor;

	private void OnEnable()
	{
		StartCoroutine(DelayedEnable());
	}

	private IEnumerator DelayedEnable()
	{
		yield return new WaitForSeconds(0.1f);
		EnableLate();
	}

	private void EnableLate()
	{
		if (GlobalStorage.Instance.isSpectatorModeEnabled || GlobalStorage.Instance.isShortModeEnabled)
		{
			optionsDoor.Unlock();
			optionsDoor.LockUpper();
		}
		if (GlobalStorage.Instance.isShortModeEnabled)
		{
			Debug.Log("Short mode enabled on the way into the museum, setting slider to on");
			heightSlider.Unlock();
			heightSlider.LockUpper();
		}
		if (GlobalStorage.Instance.isSpectatorModeEnabled)
		{
			Debug.Log("Spectator mode was already enabled, locking slider to on");
			spectatorSlider.Unlock();
			spectatorSlider.LockUpper();
		}
		heightSlider.OnUpperLocked += ToggleHeightSliderUpperLocked;
		heightSlider.OnLowerLocked += ToggleHeightSliderLowerLocked;
		spectatorSlider.OnUpperLocked += ToggleSpectatorSliderUpperLocked;
		spectatorSlider.OnLowerLocked += ToggleSpectatorSliderLowerLocked;
		if (GlobalStorage.Instance.CompanionUIManager != null)
		{
			CompanionUIManager companionUIManager = GlobalStorage.Instance.CompanionUIManager;
			companionUIManager.OnStreamerModeEnabledChanged = (Action<bool>)Delegate.Combine(companionUIManager.OnStreamerModeEnabledChanged, new Action<bool>(StreamerModeEnabledStateChanged));
		}
	}

	private void OnDisable()
	{
		heightSlider.OnUpperLocked -= ToggleHeightSliderUpperLocked;
		heightSlider.OnLowerLocked -= ToggleHeightSliderLowerLocked;
		spectatorSlider.OnUpperLocked -= ToggleSpectatorSliderUpperLocked;
		spectatorSlider.OnLowerLocked -= ToggleSpectatorSliderLowerLocked;
		if (GlobalStorage.no_instantiate_instance != null && GlobalStorage.no_instantiate_instance.CompanionUIManager != null)
		{
			CompanionUIManager companionUIManager = GlobalStorage.no_instantiate_instance.CompanionUIManager;
			companionUIManager.OnStreamerModeEnabledChanged = (Action<bool>)Delegate.Remove(companionUIManager.OnStreamerModeEnabledChanged, new Action<bool>(StreamerModeEnabledStateChanged));
		}
	}

	private void StreamerModeEnabledStateChanged(bool state)
	{
		GlobalStorage.Instance.isSpectatorModeEnabled = state;
		if (spectatorSlider.Grabbable.IsCurrInHand && spectatorSlider.Grabbable.CurrInteractableHand != null)
		{
			spectatorSlider.Grabbable.CurrInteractableHand.TryRelease();
		}
		spectatorSlider.Unlock(false);
		if (state)
		{
			spectatorSlider.LockUpper();
		}
		else
		{
			spectatorSlider.LockLower();
		}
	}

	private void ToggleHeightSliderUpperLocked(GrabbableSlider slider, bool isInitial)
	{
		if (!isInitial)
		{
			Debug.Log("Height Slider: SHORTMODE");
			GenieManager.SetShortModeEnabled(true);
		}
	}

	private void ToggleHeightSliderLowerLocked(GrabbableSlider slider, bool isInitial)
	{
		if (!isInitial)
		{
			Debug.Log("Height Slider: Normal");
			GenieManager.SetShortModeEnabled(false);
		}
	}

	private void ToggleSpectatorSliderUpperLocked(GrabbableSlider slider, bool isInitial)
	{
		if (!isInitial)
		{
			Debug.Log("Spectator Slider: On");
			SetSpectatorModeEnabled(true);
		}
	}

	private void ToggleSpectatorSliderLowerLocked(GrabbableSlider slider, bool isInitial)
	{
		if (!isInitial)
		{
			Debug.Log("Spectator Slider: Off");
			SetSpectatorModeEnabled(false);
		}
	}

	public void SetSpectatorModeEnabled(bool enabled)
	{
		if (!(GlobalStorage.no_instantiate_instance == null))
		{
			if (GlobalStorage.Instance.CompanionUIManager == null)
			{
				Debug.LogError("The CompanionUIManager does not exist, can't change spectator mode settings");
				return;
			}
			GlobalStorage.Instance.isSpectatorModeEnabled = enabled;
			GlobalStorage.Instance.CompanionUIManager.ForceCompanionEnableAndSwitchToExternalCam(enabled);
		}
	}
}
