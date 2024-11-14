using OwlchemyVR;
using UnityEngine;

public class CompanionModeLogic : MonoBehaviour
{
	private const float LENGTH_OF_TIME_FOR_MESSAGE = 2.3f;

	private const float MAX_TIME_BETWEEN_CLICKS = 0.5f;

	[SerializeField]
	private CompanionUIManager companionUIManager;

	[SerializeField]
	private GameObject returnFromFullscreenMessage;

	private float timeOfStart;

	private bool isFullscreenRecordMode;

	private MasterHMDAndInputController masterHMDAndInputController;

	private HandController.HandControllerButton toggleCameraButton = HandController.HandControllerButton.InteractCustom;

	private OVRInput.Button toggleCameraButtonOculus = OVRInput.Button.Two | OVRInput.Button.Four;

	private InteractionHandController lastHandThatClickedToggleCameraButton;

	private float timeOfLastToggleCameraButtonPress = float.PositiveInfinity;

	private void OnDisable()
	{
		returnFromFullscreenMessage.SetActive(false);
	}

	public void StartFullScreenMode()
	{
		isFullscreenRecordMode = true;
		timeOfStart = Time.time;
	}

	public void StopFullscreenMode()
	{
		isFullscreenRecordMode = false;
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.F11))
		{
			if (isFullscreenRecordMode)
			{
				companionUIManager.UnhideRecordModeTab();
			}
			else
			{
				companionUIManager.HideRecordModeTab();
			}
		}
		if (isFullscreenRecordMode && Time.time > timeOfStart + 2.3f && returnFromFullscreenMessage.activeSelf)
		{
			returnFromFullscreenMessage.SetActive(false);
		}
		if (masterHMDAndInputController == null)
		{
			masterHMDAndInputController = GlobalStorage.Instance.MasterHMDAndInputController;
			return;
		}
		VRPlatformHardwareType currVRPlatformHardwareType = VRPlatform.GetCurrVRPlatformHardwareType();
		if (currVRPlatformHardwareType == VRPlatformHardwareType.Vive || currVRPlatformHardwareType == VRPlatformHardwareType.SteamVRCompatible || currVRPlatformHardwareType == VRPlatformHardwareType.WindowsMR)
		{
			if (!masterHMDAndInputController.GetButtonDownEitherHand(toggleCameraButton))
			{
				return;
			}
			for (int i = 0; i < masterHMDAndInputController.InteractionHandControllers.Count; i++)
			{
				if (masterHMDAndInputController.InteractionHandControllers[i].HandController.GetButtonDown(toggleCameraButton))
				{
					if (!(lastHandThatClickedToggleCameraButton == masterHMDAndInputController.InteractionHandControllers[i]))
					{
						lastHandThatClickedToggleCameraButton = masterHMDAndInputController.InteractionHandControllers[i];
						timeOfLastToggleCameraButtonPress = Time.time;
						break;
					}
					if (Time.time - timeOfLastToggleCameraButtonPress < 0.5f)
					{
						ToggleCamera();
						lastHandThatClickedToggleCameraButton = null;
					}
					else
					{
						timeOfLastToggleCameraButtonPress = Time.time;
					}
				}
			}
		}
		else if (VRPlatform.GetCurrVRPlatformType() == VRPlatformTypes.Oculus)
		{
			if (OVRInput.GetUp(toggleCameraButtonOculus))
			{
				if (Time.time - timeOfLastToggleCameraButtonPress < 0.5f)
				{
					ToggleCamera();
				}
				else
				{
					timeOfLastToggleCameraButtonPress = Time.time;
				}
			}
		}
		else if (VRPlatform.GetCurrVRPlatformType() == VRPlatformTypes.SteamVR && VRPlatform.GetCurrVRPlatformHardwareType() == VRPlatformHardwareType.OculusRift && HandsAreNotNull() && (masterHMDAndInputController.LeftHand.HandController.SteamVRCompatibleController.IsCompanionCamCycleButtonUp() || masterHMDAndInputController.RightHand.HandController.SteamVRCompatibleController.IsCompanionCamCycleButtonUp()))
		{
			if (Time.time - timeOfLastToggleCameraButtonPress < 0.5f)
			{
				ToggleCamera();
			}
			else
			{
				timeOfLastToggleCameraButtonPress = Time.time;
			}
		}
	}

	private bool HandsAreNotNull()
	{
		if (masterHMDAndInputController != null && masterHMDAndInputController.LeftHand != null && masterHMDAndInputController.RightHand != null && masterHMDAndInputController.LeftHand.HandController != null && masterHMDAndInputController.RightHand.HandController != null && masterHMDAndInputController.LeftHand.HandController.SteamVRCompatibleController != null && masterHMDAndInputController.RightHand.HandController.SteamVRCompatibleController != null)
		{
			return true;
		}
		return false;
	}

	private void ToggleCamera()
	{
		companionUIManager.GoToNextFollowTarget();
	}
}
