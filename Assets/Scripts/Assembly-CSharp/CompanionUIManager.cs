using System;
using System.Globalization;
using OwlchemyVR;
using PSC;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR;

public class CompanionUIManager : MonoBehaviour
{
	private const float MAX_TIME_BETWEEN_CLICKS = 0.5f;

	private const int NUM_OF_CLICKS_REQUIRED_TO_GO_TO_RECORD_MODE = 4;

	private const HandController.HandControllerButton START_RECORD_MODE_SHORTCUT_BUTTON = HandController.HandControllerButton.InteractCustom;

	[SerializeField]
	private Text versionInfoText;

	[SerializeField]
	private GameObject companionCamPrefab;

	[SerializeField]
	private Text fovText;

	[SerializeField]
	private RenderTexture rt;

	[SerializeField]
	private Text smoothingText;

	private GameObject companionCamGO;

	private CompanionCam companionCam;

	[SerializeField]
	private Slider smoothingSlider;

	[SerializeField]
	private Slider fovSlider;

	[SerializeField]
	private CompanionModeLogic companionModeLogic;

	[SerializeField]
	private GameObject leftPanelRoot;

	[SerializeField]
	private GameObject leftPanel;

	[SerializeField]
	private GameObject creditsPanel;

	[SerializeField]
	private GameObject leftPanelWhenDeactivated;

	[SerializeField]
	private GameObject fullscreenMessage;

	[SerializeField]
	private Image buttonImage;

	[SerializeField]
	private Sprite showSprite;

	[SerializeField]
	private Sprite hideSprite;

	[SerializeField]
	private SimpleClickableButton showHideClickable;

	[SerializeField]
	private Color buttonOnColor = Color.white;

	[SerializeField]
	private Color buttonOffColor = Color.white;

	[SerializeField]
	private AudioClip poofClip;

	[SerializeField]
	private AudioClip poofClipHead;

	[SerializeField]
	private Text roomSizeText;

	[SerializeField]
	private Text roomscaleNoticeText;

	[SerializeField]
	private Text platformDebugText;

	private float xSize = -1f;

	private float ySize = -1f;

	private MasterHMDAndInputController masterHMDAndInputController;

	private InteractionHandController lastHandThatClickedRecordModeButton;

	private int numOfClicksOfRecordModeButton;

	private float timeOfLastRecordModeButtonPress = float.PositiveInfinity;

	private GameObject childGOTest;

	private bool isHidden;

	private bool isTabOpen;

	public Action<bool> OnStreamerModeEnabledChanged;

	public void EnableStreamerMode(bool enable, bool doEvent = true)
	{
		if (enable)
		{
			SetupCompanionCam();
		}
		if (!enable && (bool)companionCam)
		{
			StopCompanionFeatures();
		}
		companionModeLogic.enabled = enable;
		if (doEvent && OnStreamerModeEnabledChanged != null)
		{
			OnStreamerModeEnabledChanged(enable);
		}
	}

	private void Awake()
	{
		GlobalStorage.Instance.SetCompanionUIManager(this);
	}

	private void Start()
	{
		if (VRPlatform.GetCurrVRPlatformType() == VRPlatformTypes.SteamVR)
		{
			CheckSteamVRChaperone();
		}
		else if (VRPlatform.GetCurrVRPlatformType() == VRPlatformTypes.Oculus)
		{
			CheckOVRGuardian();
		}
		platformDebugText.text = VRPlatform.GetCurrVRPlatformType().ToString() + ": " + VRPlatform.GetCurrVRPlatformHardwareType();
		roomscaleNoticeText.text = ((!VRPlatform.IsRoomScale) ? string.Empty : "Roomscale Enabled");
		childGOTest = base.transform.GetChild(0).gameObject;
		VersionInfoStorage versionInfoStorage = VersionInfoStorage.LoadVersionInfoStorageFromResources();
		if (versionInfoStorage != null)
		{
			versionInfoText.text = "Build #" + versionInfoStorage.GetBuildNumber();
		}
		else
		{
			versionInfoText.text = "INVALID BUILD #";
		}
	}

	private void CheckOVRGuardian()
	{
		Vector3 dimensions = OVRManager.boundary.GetDimensions(OVRBoundary.BoundaryType.PlayArea);
		if (!(dimensions != Vector3.zero))
		{
			return;
		}
		xSize = dimensions.x;
		ySize = dimensions.z;
		string text = "--";
		if (Room.activeRoom != null && Room.activeRoom.configuration != null)
		{
			text = Room.activeRoom.configuration.name;
			if (Room.activeRoom.configuration.name == "Oculus_2.5Meter")
			{
				text = "Medium (2.5m wide)";
			}
			if (Room.activeRoom.configuration.name == "Oculus_2Meter")
			{
				text = "Small (2m wide)";
			}
			if (Room.activeRoom.configuration.name == "SteamVR_3Meter" || Room.activeRoom.configuration.name == "Oculus_3Meter")
			{
				text = "Large (3m wide)";
			}
			if (Room.activeRoom.configuration.name == "SteamVR_2.5Meter" || Room.activeRoom.configuration.name == "Oculus_2.5Meter")
			{
				text = "Medium (2.5m wide)";
			}
			if (Room.activeRoom.configuration.name == "SteamVR_2Meter" || Room.activeRoom.configuration.name == "Oculus_2Meter")
			{
				text = "Small (2m wide)";
			}
		}
		roomSizeText.text = (Math.Truncate(xSize * 100f) / 100.0).ToString(CultureInfo.InvariantCulture) + "m X " + (Math.Truncate(ySize * 100f) / 100.0).ToString(CultureInfo.InvariantCulture) + "m (" + text + ")";
	}

	private void OnEnable()
	{
		SimpleClickableButton simpleClickableButton = showHideClickable;
		simpleClickableButton.OnClicked = (Action<SimpleClickableButton>)Delegate.Combine(simpleClickableButton.OnClicked, new Action<SimpleClickableButton>(ToggleButtonClicked));
	}

	private void OnDisable()
	{
		SimpleClickableButton simpleClickableButton = showHideClickable;
		simpleClickableButton.OnClicked = (Action<SimpleClickableButton>)Delegate.Remove(simpleClickableButton.OnClicked, new Action<SimpleClickableButton>(ToggleButtonClicked));
	}

	private void ToggleButtonClicked(SimpleClickableButton clicked)
	{
		DoToggleCompanion();
	}

	public void DoToggleCompanion()
	{
		isTabOpen = !isTabOpen;
		EnableStreamerMode(isTabOpen);
		leftPanelRoot.SetActive(isTabOpen);
		leftPanel.SetActive(isTabOpen);
		leftPanelWhenDeactivated.SetActive(!isTabOpen);
		if (!isTabOpen)
		{
			creditsPanel.SetActive(false);
		}
		buttonImage.sprite = ((!isTabOpen) ? showSprite : hideSprite);
		buttonImage.color = ((!isTabOpen) ? buttonOffColor : buttonOnColor);
	}

	public void ForceCompanionEnableAndSwitchToExternalCam(bool enabled)
	{
		if (isTabOpen == enabled)
		{
			Debug.Log("Tab already synced!");
			return;
		}
		isTabOpen = enabled;
		EnableStreamerMode(isTabOpen, false);
		leftPanelRoot.SetActive(isTabOpen);
		leftPanel.SetActive(isTabOpen);
		leftPanelWhenDeactivated.SetActive(!isTabOpen);
		if (!isTabOpen)
		{
			creditsPanel.SetActive(false);
		}
		buttonImage.sprite = ((!isTabOpen) ? showSprite : hideSprite);
		buttonImage.color = ((!isTabOpen) ? buttonOffColor : buttonOnColor);
		if (enabled)
		{
			if (companionCam.currentCameraLocation == 0)
			{
				Invoke("GoToNextFollowTarget", 0.05f);
			}
		}
		else
		{
			companionCam.GoToNextFollowTarget(100);
		}
	}

	private void CheckSteamVRChaperone()
	{
		bool flag = !SteamVR.active && !SteamVR.usingNativeSupport;
		if (flag)
		{
			EVRInitError peError = EVRInitError.None;
			OpenVR.Init(ref peError, EVRApplicationType.VRApplication_Other);
		}
		CVRChaperone chaperone = OpenVR.Chaperone;
		if (chaperone != null && chaperone.GetPlayAreaSize(ref xSize, ref ySize))
		{
			xSize = (float)Math.Round(xSize, 1);
			ySize = (float)Math.Round(ySize, 1);
			string text = "--";
			if (Room.activeRoom != null && Room.activeRoom.configuration != null)
			{
				text = Room.activeRoom.configuration.name;
				if (Room.activeRoom.configuration.name == "SteamVR_3Meter" || Room.activeRoom.configuration.name == "Oculus_3Meter")
				{
					text = "Large (3m wide)";
				}
				if (Room.activeRoom.configuration.name == "SteamVR_2.5Meter" || Room.activeRoom.configuration.name == "Oculus_2.5Meter")
				{
					text = "Medium (2.5m wide)";
				}
				if (Room.activeRoom.configuration.name == "SteamVR_2Meter" || Room.activeRoom.configuration.name == "Oculus_2Meter")
				{
					text = "Small (2m wide)";
				}
			}
			roomSizeText.text = xSize + "m X " + ySize + "m (" + text + ")";
		}
		else
		{
			Debug.LogWarning("Failed to get Calibrated Play Area bounds!  Make sure you have tracking first, and that your space is calibrated.");
		}
		if (flag)
		{
			OpenVR.Shutdown();
		}
	}

	public void RecreateSecurityCameraModel()
	{
		companionCam.securityCameraModel = UnityEngine.Object.Instantiate(companionCamPrefab).transform;
		companionCam.securityCameraModel.GetComponent<CompanionSecurityCam>().uiManager = this;
		companionCam.UpdateSecurityCam();
		Debug.Log("Camera destroyed - recreating security camera model and repositioning");
	}

	private void SetupCompanionCam()
	{
		if (companionCamGO != null)
		{
			companionCamGO.SetActive(true);
		}
		else
		{
			companionCamGO = new GameObject("CompanionCam");
			companionCamGO.AddComponent<CompanionCam>();
			companionCam = companionCamGO.GetComponent<CompanionCam>();
			companionCam.securityCameraModel = UnityEngine.Object.Instantiate(companionCamPrefab).transform;
			companionCam.securityCameraModel.GetComponent<CompanionSecurityCam>().uiManager = this;
			companionCam.companionRTCamRenderTexture = rt;
			companionCam.poofClip = poofClip;
			companionCam.poofClipHead = poofClipHead;
			UnityEngine.Object.DontDestroyOnLoad(companionCamGO.transform);
			UnityEngine.Object.DontDestroyOnLoad(companionCam.securityCameraModel);
		}
		companionCamGO.GetComponent<CompanionCam>().isStreamerModeEnabled = true;
		RefreshFOVAndSmoothing();
	}

	private void StopCompanionFeatures()
	{
		companionCam.RemoveStaticCameraFeatures();
		companionCamGO.SetActive(false);
	}

	public void GoToNextFollowTarget()
	{
		companionCam.GoToNextFollowTarget(1);
	}

	public void SetFOV(float fov)
	{
		companionCam.SetFov(fov);
		RefreshFOVAndSmoothing();
	}

	public void SetSmoothing(float smoothing)
	{
		companionCam.SetCamSmoothingAmount(smoothing);
		RefreshFOVAndSmoothing();
	}

	private void RefreshFOVAndSmoothing()
	{
		float fov = companionCam.GetFov();
		fovText.text = "FOV (" + (int)fov + ")";
		float smoothing = companionCam.GetSmoothing();
		smoothingText.text = "SMOOTHING (" + smoothing.ToString("n2") + ")";
	}

	public void ResetSettings()
	{
		companionCam.ResetSettings();
		RefreshFOVAndSmoothing();
		smoothingSlider.value = companionCam.GetSmoothing();
		fovSlider.value = companionCam.GetFov();
	}

	public void UnhideRecordModeTab()
	{
		buttonImage.gameObject.SetActive(true);
		leftPanelRoot.SetActive(true);
		leftPanel.SetActive(true);
		fullscreenMessage.gameObject.SetActive(false);
		companionModeLogic.StopFullscreenMode();
		buttonImage.sprite = hideSprite;
		buttonImage.color = buttonOnColor;
	}

	public void HideRecordModeTab()
	{
		buttonImage.gameObject.SetActive(false);
		leftPanelRoot.SetActive(false);
		leftPanel.SetActive(false);
		fullscreenMessage.gameObject.SetActive(true);
		companionModeLogic.StartFullScreenMode();
		buttonImage.sprite = showSprite;
		buttonImage.color = buttonOffColor;
	}

	private void Update()
	{
		if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) && Input.GetKeyDown(KeyCode.Alpha8))
		{
			companionCamGO.GetComponent<Camera>().enabled = isHidden;
			isHidden = !isHidden;
		}
	}

	private void ToggleRecordMode()
	{
		buttonImage.gameObject.SetActive(true);
		if (companionModeLogic.enabled)
		{
			EnableStreamerMode(false);
			leftPanelRoot.SetActive(false);
			leftPanel.gameObject.SetActive(false);
			fullscreenMessage.gameObject.SetActive(false);
			buttonImage.sprite = showSprite;
			buttonImage.color = buttonOffColor;
		}
		else
		{
			EnableStreamerMode(true);
			leftPanelRoot.SetActive(true);
			leftPanel.gameObject.SetActive(true);
			buttonImage.sprite = hideSprite;
			buttonImage.color = buttonOnColor;
		}
	}
}
