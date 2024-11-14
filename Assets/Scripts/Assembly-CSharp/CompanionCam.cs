using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompanionCam : MonoBehaviour
{
	private const float DEFAULT_CAMERA_SMOOTHING = 0.2f;

	private const float DEFAULT_CAMERA_FOV = 70f;

	private const string CAMERA_SMOOTHING_KEY = "CameraSmoothing";

	private const string CAMERA_FOV_KEY = "CameraFOV";

	[HideInInspector]
	public Transform securityCameraModel;

	[HideInInspector]
	public bool isStreamerModeEnabled;

	[HideInInspector]
	public RenderTexture companionRTCamRenderTexture;

	[HideInInspector]
	public AudioClip poofClip;

	[HideInInspector]
	public AudioClip poofClipHead;

	private Camera hmdCamera;

	private List<Transform> cameraLocations = new List<Transform>();

	public int currentCameraLocation;

	private Camera companionCam;

	private Camera companionRTCam;

	private float cameraSmoothing;

	private float transitionTimeWithNoSmoothing;

	private Renderer genericHMDRenderer;

	private bool isInStaticCamMode;

	private Vector3 positionVelocityRef = Vector3.zero;

	private Vector3 rotationRef = Vector3.zero;

	private GameObject rtCameraGO;

	private void Start()
	{
		Camera component = GlobalStorage.Instance.MasterHMDAndInputController.CamTransform.GetComponent<Camera>();
		hmdCamera = component;
		SetupCamera();
		SetupRTCamera();
		GrabStaticCameraPositions();
		ApplySmoothingAmount();
		ApplyFov();
		GetGenericHMD();
		UpdateSecurityCam();
		StartCoroutine(FixRes());
	}

	private IEnumerator FixRes()
	{
		yield return null;
		Debug.Log("screen res: " + Screen.width + " x " + Screen.height);
		Debug.Log("Screen fullscreen status: " + Screen.fullScreen);
		Debug.Log("SETTING RES TO FIX ASPECT ISSUE");
		Screen.SetResolution(Screen.width, Screen.height, Screen.fullScreen);
	}

	private void Update()
	{
		CheckForStaticMode();
		transitionTimeWithNoSmoothing -= Time.deltaTime;
	}

	private void SetupCamera()
	{
		companionCam = base.gameObject.AddComponent<Camera>();
		companionCam.eventMask = 0;
		base.transform.position = hmdCamera.transform.position;
		base.transform.rotation = hmdCamera.transform.rotation;
		companionCam.backgroundColor = hmdCamera.backgroundColor;
		companionCam.clearFlags = hmdCamera.clearFlags;
		companionCam.farClipPlane = hmdCamera.farClipPlane;
		companionCam.nearClipPlane = hmdCamera.nearClipPlane;
		companionCam.cullingMask = hmdCamera.cullingMask;
		companionCam.cullingMask |= 1073741824;
		companionCam.depth = 10f;
		companionCam.stereoTargetEye = StereoTargetEyeMask.None;
		companionCam.fieldOfView = 70f;
	}

	private void SetupRTCamera()
	{
		rtCameraGO = new GameObject("RTCamGO");
		companionRTCam = rtCameraGO.AddComponent<Camera>();
		companionRTCam.eventMask = 0;
		rtCameraGO.transform.SetParent(companionCam.transform);
		rtCameraGO.transform.localPosition = Vector3.zero;
		rtCameraGO.transform.localRotation = Quaternion.identity;
		companionRTCam.backgroundColor = hmdCamera.backgroundColor;
		companionRTCam.clearFlags = hmdCamera.clearFlags;
		companionRTCam.farClipPlane = hmdCamera.farClipPlane;
		companionRTCam.nearClipPlane = hmdCamera.nearClipPlane;
		companionRTCam.cullingMask = hmdCamera.cullingMask;
		companionRTCam.cullingMask |= 1073741824;
		companionRTCam.depth = 10f;
		companionCam.stereoTargetEye = StereoTargetEyeMask.None;
		companionRTCam.fieldOfView = 70f;
		companionRTCam.targetTexture = companionRTCamRenderTexture;
	}

	private void GrabStaticCameraPositions()
	{
		cameraLocations.Clear();
		StationaryCompanionCamera[] array = Object.FindObjectsOfType<StationaryCompanionCamera>();
		StationaryCompanionCamera[] array2 = array;
		foreach (StationaryCompanionCamera stationaryCompanionCamera in array2)
		{
			cameraLocations.Add(stationaryCompanionCamera.transform);
		}
		cameraLocations.Insert(0, hmdCamera.transform);
	}

	private void CheckForStaticMode()
	{
		if (currentCameraLocation == 0)
		{
			isInStaticCamMode = false;
			FollowTarget(cameraLocations[currentCameraLocation]);
		}
		else
		{
			isInStaticCamMode = true;
			FollowTarget(securityCameraModel);
		}
	}

	private void GetGenericHMD()
	{
		genericHMDRenderer = Object.FindObjectOfType<VRHeadsetModel>().GetComponent<Renderer>();
	}

	public void ApplySmoothingAmount()
	{
		cameraSmoothing = PlayerPrefs.GetFloat("CameraSmoothing", 0.2f);
		Debug.Log("Camera Smoothing applied! Now:" + cameraSmoothing);
	}

	public void SetCamSmoothingAmount(float newSmoothingAmount)
	{
		cameraSmoothing = Mathf.Max(0f, newSmoothingAmount);
		PlayerPrefs.SetFloat("CameraSmoothing", cameraSmoothing);
		Debug.Log("Camera Smoothing set to:" + cameraSmoothing);
	}

	public void ResetSettings()
	{
		SetFov(70f);
		SetCamSmoothingAmount(0.2f);
	}

	public void ApplyFov()
	{
		companionCam.fieldOfView = PlayerPrefs.GetFloat("CameraFOV", 70f);
		companionRTCam.fieldOfView = companionCam.fieldOfView;
		Debug.Log("Camera FOV applied! Now:" + companionCam.fieldOfView);
	}

	public void SetFov(float fov)
	{
		PlayerPrefs.SetFloat("CameraFOV", fov);
		ApplyFov();
	}

	public float GetFov()
	{
		return PlayerPrefs.GetFloat("CameraFOV", 70f);
	}

	public float GetSmoothing()
	{
		return PlayerPrefs.GetFloat("CameraSmoothing", 0.2f);
	}

	public void GoToNextFollowTarget(int numberOfLocationsToIncrement)
	{
		currentCameraLocation += numberOfLocationsToIncrement;
		if (currentCameraLocation >= cameraLocations.Count)
		{
			currentCameraLocation = 0;
		}
		transitionTimeWithNoSmoothing = 0.5f;
		CheckForStaticMode();
		UpdateSecurityCam();
		if (currentCameraLocation == 0)
		{
			AudioManager.Instance.Play(securityCameraModel.position, poofClip, 1f, 1f);
		}
		else
		{
			AudioManager.Instance.Play2D(poofClipHead);
		}
	}

	public void GoToFollowTarget(int numToSkipTo)
	{
		currentCameraLocation = numToSkipTo;
		if (currentCameraLocation >= cameraLocations.Count)
		{
			currentCameraLocation = 0;
		}
		transitionTimeWithNoSmoothing = 0.5f;
		CheckForStaticMode();
		UpdateSecurityCam();
		if (currentCameraLocation == 0)
		{
			AudioManager.Instance.Play(securityCameraModel.position, poofClip, 1f, 1f);
		}
		else
		{
			AudioManager.Instance.Play2D(poofClipHead);
		}
	}

	public void UpdateSecurityCam()
	{
		if (isInStaticCamMode && (bool)securityCameraModel)
		{
			securityCameraModel.gameObject.SetActive(true);
			if (cameraLocations[currentCameraLocation] != null)
			{
				securityCameraModel.position = cameraLocations[currentCameraLocation].position;
				securityCameraModel.rotation = cameraLocations[currentCameraLocation].rotation;
			}
			genericHMDRenderer.enabled = true;
		}
		else
		{
			RemoveStaticCameraFeatures();
		}
	}

	public void RemoveStaticCameraFeatures()
	{
		isStreamerModeEnabled = false;
		securityCameraModel.gameObject.SetActive(false);
		genericHMDRenderer.enabled = false;
	}

	private void FollowTarget(Transform targetT)
	{
		if (!(targetT == null))
		{
			float deltaTime = Time.deltaTime;
			base.transform.position = Vector3.SmoothDamp(base.transform.position, targetT.position, ref positionVelocityRef, cameraSmoothing, float.PositiveInfinity, deltaTime);
			Vector3 eulerAngles = base.transform.eulerAngles;
			Vector3 eulerAngles2 = targetT.eulerAngles;
			eulerAngles.x = Mathf.SmoothDampAngle(eulerAngles.x, eulerAngles2.x, ref rotationRef.x, cameraSmoothing, float.PositiveInfinity, deltaTime);
			eulerAngles.y = Mathf.SmoothDampAngle(eulerAngles.y, eulerAngles2.y, ref rotationRef.y, cameraSmoothing, float.PositiveInfinity, deltaTime);
			eulerAngles.z = Mathf.SmoothDampAngle(eulerAngles.z, eulerAngles2.z, ref rotationRef.z, cameraSmoothing, float.PositiveInfinity, deltaTime);
			base.transform.rotation = Quaternion.Euler(eulerAngles);
			if (transitionTimeWithNoSmoothing > 0f || isInStaticCamMode)
			{
				base.transform.position = targetT.position;
				base.transform.rotation = targetT.rotation;
			}
		}
	}

	private void OnLevelWasLoaded()
	{
		GrabStaticCameraPositions();
		GoToNextFollowTarget(0);
	}
}
