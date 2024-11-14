using System;
using System.Collections;
using System.Collections.Generic;
using OwlchemyVR;
using UnityEngine;

public class MixedRealityRecordingManager : MonoBehaviour
{
	private enum HandControllerModeForHandCameraTypes
	{
		Normal = 0,
		NoHandOrController = 1,
		ControllerOnly = 2
	}

	private const string HAND_CAMERA_FOV_KEY = "HandCameraFOV";

	private const string POV_CAMERA_FOV_KEY = "POVCameraFOV";

	private const string HAND_CAMERA_POSITION_OFFSET_X_KEY = "HandCameraPositionOffsetX";

	private const string HAND_CAMERA_POSITION_OFFSET_Y_KEY = "HandCameraPositionOffsetY";

	private const string HAND_CAMERA_POSITION_OFFSET_Z_KEY = "HandCameraPositionOffsetZ";

	private const string HAND_CAMERA_ROTATION_OFFSET_X_KEY = "HandCameraRotationOffsetX";

	private const string HAND_CAMERA_ROTATION_OFFSET_Y_KEY = "HandCameraRotationOffsetY";

	private const string HAND_CAMERA_ROTATION_OFFSET_Z_KEY = "HandCameraRotationOffsetZ";

	private const string POV_CAMERA_SMOOTHING_KEY = "POVCameraSmoothing";

	private const float DEFAULT_CAMERA_SMOOTHING = 0.4f;

	[SerializeField]
	private GameObject recordingCameraContainer;

	[SerializeField]
	private GameObject recordingCameraOffset;

	[SerializeField]
	private Camera recordingCameraForeground;

	private Camera recordingCameraBackground;

	private Camera recordingCameraFull;

	private Camera emptyCamera;

	[SerializeField]
	private Vector3 cameraToControllerPositionOffset;

	[SerializeField]
	private Vector3 cameraToControllerRotationOffset;

	[SerializeField]
	private Transform bodyPositionGO;

	[SerializeField]
	private Renderer bodyPositionRenderer;

	private bool isRecording;

	private CameraEvents cameraEventsForeground;

	private CameraEvents cameraEventsBackground;

	private Renderer[] allRenderers;

	private bool[] isForegroundList;

	private bool[] wasRendererOn;

	private int[] prevLayers;

	[SerializeField]
	private bool isClipping = true;

	private float foregroundOverlapClippingExtra = 0.08f;

	private bool isWaitingForHMD;

	private Camera povCamera;

	private bool isUsing4CameraView = true;

	private bool isDoingHackBackwardsMove;

	private Quaternion hackBackwardsMoveRotationLock;

	private Vector3 hackBackwardsMoveOrigin;

	private float hackBackwardsMoveProgression;

	private float hackBackwardsSpeed = 1f;

	private Vector3 hackBackwardsMoveDirection;

	private bool isDoingHackPivotMove;

	private Transform hackPivotTransform;

	private Transform hackPivotOriginalParent;

	private Quaternion hackPivotOriginalRotation;

	private float hackPivotSpeed = 1f;

	private float hackPivotProgression;

	private Vector3 cameraOffsetPosition;

	private Vector3 cameraOffsetRotationEuler;

	private Vector3 moveVel = Vector3.zero;

	private Vector3 rotVel = Vector3.zero;

	private float povCameraSmoothing;

	private Vector3 defaultRecordingCameraLocalPosition;

	private Vector3 defaultRecordingCameraLocalRotation;

	private float defaultRecordingHandCameraFOV;

	private float defaultPOVCameraFOV = -1f;

	private HandControllerModeForHandCameraTypes currentHandControllerMode;

	private void Awake()
	{
		Debug.Log("MixedRealityRecordingManager Start");
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		defaultRecordingCameraLocalPosition = new Vector3(0f, -1f, -1f);
		defaultRecordingCameraLocalRotation = new Vector3(-8.5f, 1f, 1f);
		defaultRecordingHandCameraFOV = 45.1f;
		recordingCameraOffset.transform.localPosition = cameraToControllerPositionOffset;
		recordingCameraOffset.transform.localEulerAngles = cameraToControllerRotationOffset;
		recordingCameraForeground.name = "RecordingCameraForeground";
		recordingCameraBackground = UnityEngine.Object.Instantiate(recordingCameraForeground);
		recordingCameraBackground.transform.SetParent(recordingCameraForeground.transform.parent);
		recordingCameraBackground.transform.localPosition = recordingCameraForeground.transform.localPosition;
		recordingCameraBackground.transform.localRotation = recordingCameraForeground.transform.localRotation;
		recordingCameraBackground.name = "RecordingCameraBackground";
		recordingCameraFull = UnityEngine.Object.Instantiate(recordingCameraForeground);
		recordingCameraFull.transform.SetParent(recordingCameraForeground.transform.parent);
		recordingCameraFull.transform.localPosition = recordingCameraForeground.transform.localPosition;
		recordingCameraFull.transform.localRotation = recordingCameraForeground.transform.localRotation;
		recordingCameraFull.name = "RecordingCameraFull";
		recordingCameraFull.depth -= 1f;
		recordingCameraBackground.depth += 1f;
		emptyCamera = UnityEngine.Object.Instantiate(recordingCameraForeground);
		emptyCamera.transform.SetParent(recordingCameraForeground.transform.parent);
		emptyCamera.transform.localPosition = recordingCameraForeground.transform.localPosition;
		emptyCamera.transform.localRotation = recordingCameraForeground.transform.localRotation;
		emptyCamera.depth -= 2f;
		emptyCamera.name = "RecordingBackgroundCamera";
		emptyCamera.cullingMask = 0;
		emptyCamera.rect = new Rect(0f, 0f, 1f, 1f);
		recordingCameraForeground.rect = new Rect(0.5f, 0.5f, 0.5f, 0.5f);
		recordingCameraBackground.rect = new Rect(0f, 0.5f, 0.5f, 0.5f);
		recordingCameraFull.rect = new Rect(new Rect(0f, 0f, 0.5f, 0.5f));
		if (!isClipping)
		{
			recordingCameraForeground.cullingMask = LayerMaskHelper.OnlyIncluding(27);
			recordingCameraBackground.cullingMask = LayerMaskHelper.OnlyIncluding(28);
		}
		float @float = PlayerPrefs.GetFloat("HandCameraFOV", recordingCameraForeground.fieldOfView);
		SetHandCameraFOV(@float);
		SetExistingHandPositionAndRotation();
		SetPovSmoothing();
	}

	private void ToggleHackBackwardsMovement()
	{
		if (povCamera == null)
		{
			Debug.LogError("povCamera doesn't exist yet!");
		}
		else if (!isDoingHackBackwardsMove)
		{
			if (isDoingHackPivotMove)
			{
				isDoingHackPivotMove = false;
				povCamera.transform.SetParent(hackPivotOriginalParent, true);
			}
			isDoingHackBackwardsMove = true;
			hackBackwardsMoveProgression = 0f;
			hackBackwardsMoveRotationLock = povCamera.transform.rotation;
			hackBackwardsMoveOrigin = povCamera.transform.position;
			hackBackwardsMoveDirection = povCamera.transform.forward;
			hackBackwardsSpeed = 5f;
		}
		else if (hackBackwardsSpeed < 12f)
		{
			hackBackwardsSpeed += 4f;
		}
		else
		{
			isDoingHackBackwardsMove = false;
		}
	}

	private void ToggleHackPivotMovement()
	{
		if (povCamera == null)
		{
			Debug.LogError("povCamera doesn't exist yet!");
		}
		else if (!isDoingHackPivotMove)
		{
			hackPivotProgression = 0f;
			hackPivotTransform = new GameObject().transform;
			hackPivotTransform.position = povCamera.transform.position;
			hackPivotTransform.rotation = povCamera.transform.rotation;
			hackPivotTransform.Translate(Vector3.forward * 2f);
			hackPivotTransform.eulerAngles = new Vector3(hackPivotTransform.eulerAngles.x, hackPivotTransform.eulerAngles.y, 0f);
			hackPivotOriginalRotation = hackPivotTransform.rotation;
			hackPivotOriginalParent = povCamera.transform.parent;
			povCamera.transform.SetParent(hackPivotTransform, true);
			isDoingHackPivotMove = true;
			hackPivotSpeed = 0.2f;
			isDoingHackBackwardsMove = false;
		}
		else if (hackPivotSpeed < 1f)
		{
			hackPivotSpeed += 0.3f;
		}
		else
		{
			isDoingHackPivotMove = false;
			povCamera.transform.SetParent(hackPivotOriginalParent, true);
		}
	}

	private void Toggle4CameraView()
	{
		if (povCamera == null)
		{
			Debug.LogError("povCamera doesn't exist yet!");
			return;
		}
		isUsing4CameraView = !isUsing4CameraView;
		if (isUsing4CameraView)
		{
			recordingCameraForeground.gameObject.SetActive(true);
			recordingCameraBackground.gameObject.SetActive(true);
			recordingCameraFull.gameObject.SetActive(true);
			povCamera.rect = new Rect(0.5f, 0f, 0.5f, 0.5f);
		}
		else
		{
			recordingCameraForeground.gameObject.SetActive(false);
			recordingCameraBackground.gameObject.SetActive(false);
			recordingCameraFull.gameObject.SetActive(false);
			povCamera.rect = new Rect(0f, 0f, 1f, 1f);
		}
	}

	private void OnEnable()
	{
		if (GlobalStorage.Instance.MasterHMDAndInputController == null)
		{
			isWaitingForHMD = true;
			return;
		}
		AddControllerEvent();
		AddPOVCamera();
	}

	private void OnDisable()
	{
		if (GlobalStorage.no_instantiate_instance != null && GlobalStorage.Instance.MasterHMDAndInputController != null && !isWaitingForHMD)
		{
			MasterHMDAndInputController masterHMDAndInputController = GlobalStorage.Instance.MasterHMDAndInputController;
			masterHMDAndInputController.OnAdditionalControllersAddedIndex = (Action<int>)Delegate.Remove(masterHMDAndInputController.OnAdditionalControllersAddedIndex, new Action<int>(AdditionalControllerAdded));
		}
	}

	private void AddControllerEvent()
	{
		MasterHMDAndInputController masterHMDAndInputController = GlobalStorage.Instance.MasterHMDAndInputController;
		masterHMDAndInputController.OnAdditionalControllersAddedIndex = (Action<int>)Delegate.Combine(masterHMDAndInputController.OnAdditionalControllersAddedIndex, new Action<int>(AdditionalControllerAdded));
		isWaitingForHMD = false;
	}

	private void AddPOVCamera()
	{
		if (povCamera == null)
		{
			povCamera = new GameObject().AddComponent<Camera>();
			Camera component = GlobalStorage.Instance.MasterHMDAndInputController.CamTransform.GetComponent<Camera>();
			povCamera.fieldOfView = component.fieldOfView;
			povCamera.backgroundColor = component.backgroundColor;
			povCamera.clearFlags = component.clearFlags;
			povCamera.farClipPlane = component.farClipPlane;
			povCamera.nearClipPlane = component.nearClipPlane;
			povCamera.transform.SetParent(base.transform);
			povCamera.name = "SecondScreenPOVCamera";
			povCamera.rect = new Rect(0.5f, 0f, 0.5f, 0.5f);
			povCamera.depth = 10f;
			defaultPOVCameraFOV = 60f;
			float @float = PlayerPrefs.GetFloat("POVCameraFOV", povCamera.fieldOfView);
			SetPOVCameraFOV(@float);
		}
	}

	private void AdditionalControllerAdded(int index)
	{
		Debug.Log("Additional Controller Added:" + index);
		SteamVR_TrackedObject steamVR_TrackedObject = recordingCameraContainer.AddComponent<SteamVR_TrackedObject>();
		steamVR_TrackedObject.index = (SteamVR_TrackedObject.EIndex)index;
		isRecording = true;
	}

	private void Update()
	{
		if (isWaitingForHMD && GlobalStorage.Instance.MasterHMDAndInputController != null)
		{
			AddControllerEvent();
			AddPOVCamera();
		}
		if (isRecording)
		{
			bodyPositionGO.transform.position = GlobalStorage.Instance.MasterHMDAndInputController.Head.transform.position;
			bodyPositionGO.transform.localEulerAngles = new Vector3(0f, GlobalStorage.Instance.MasterHMDAndInputController.Head.transform.eulerAngles.y, 0f);
		}
		if (povCamera != null)
		{
			float deltaTime = Time.deltaTime;
			if (isDoingHackPivotMove)
			{
				hackPivotProgression += Time.deltaTime * 0.25f * hackPivotSpeed;
				if (hackPivotProgression > 1f)
				{
					hackPivotProgression -= 1f;
				}
				hackPivotTransform.rotation = hackPivotOriginalRotation * Quaternion.Euler(Vector3.up * (180f * hackPivotProgression - 90f));
			}
			else if (isDoingHackBackwardsMove)
			{
				hackBackwardsMoveProgression += Time.deltaTime * 0.25f * hackBackwardsSpeed;
				if (hackBackwardsMoveProgression > 3f)
				{
					hackBackwardsMoveProgression -= 3f;
				}
				povCamera.transform.rotation = hackBackwardsMoveRotationLock;
				povCamera.transform.position = Vector3.Lerp(hackBackwardsMoveOrigin, hackBackwardsMoveOrigin - hackBackwardsMoveDirection * 2f, Mathf.SmoothStep(0f, 1f, hackBackwardsMoveProgression));
			}
			else
			{
				povCamera.transform.position = Vector3.SmoothDamp(povCamera.transform.position, GlobalStorage.Instance.MasterHMDAndInputController.CamTransform.position, ref moveVel, povCameraSmoothing, float.PositiveInfinity, deltaTime);
				Vector3 eulerAngles = povCamera.transform.eulerAngles;
				Vector3 eulerAngles2 = GlobalStorage.Instance.MasterHMDAndInputController.CamTransform.eulerAngles;
				eulerAngles.x = Mathf.SmoothDampAngle(eulerAngles.x, eulerAngles2.x, ref rotVel.x, povCameraSmoothing, float.PositiveInfinity, deltaTime);
				eulerAngles.y = Mathf.SmoothDampAngle(eulerAngles.y, eulerAngles2.y, ref rotVel.y, povCameraSmoothing, float.PositiveInfinity, deltaTime);
				eulerAngles.z = Mathf.SmoothDampAngle(eulerAngles.z, eulerAngles2.z, ref rotVel.z, povCameraSmoothing, float.PositiveInfinity, deltaTime);
				povCamera.transform.rotation = Quaternion.Euler(eulerAngles);
			}
		}
		float num = 0.025f;
		float num2 = 0.001f;
		float num3 = 0.025f;
		float num4 = 0.01f;
		if (Input.GetKey(KeyCode.Alpha1))
		{
			if (Input.GetKey(KeyCode.RightShift))
			{
				ChangeFOVOfHandCamera(0f - num);
			}
			else
			{
				ChangeFOVOfHandCamera(num);
			}
		}
		if (Input.GetKey(KeyCode.LeftBracket))
		{
			if (Input.GetKey(KeyCode.RightShift))
			{
				ChangeFOVOfPOVCamera(0f - num);
			}
			else
			{
				ChangeFOVOfPOVCamera(num);
			}
		}
		if (Input.GetKey(KeyCode.Alpha2))
		{
			if (Input.GetKey(KeyCode.RightShift))
			{
				cameraOffsetPosition.x -= num2;
			}
			else
			{
				cameraOffsetPosition.x += num2;
			}
			SetHandCameraPosition(cameraOffsetPosition);
		}
		if (Input.GetKey(KeyCode.Alpha3))
		{
			if (Input.GetKey(KeyCode.RightShift))
			{
				cameraOffsetPosition.y -= num2;
			}
			else
			{
				cameraOffsetPosition.y += num2;
			}
			SetHandCameraPosition(cameraOffsetPosition);
		}
		if (Input.GetKey(KeyCode.Alpha4))
		{
			if (Input.GetKey(KeyCode.RightShift))
			{
				cameraOffsetPosition.z -= num2;
			}
			else
			{
				cameraOffsetPosition.z += num2;
			}
			SetHandCameraPosition(cameraOffsetPosition);
		}
		if (Input.GetKey(KeyCode.Alpha5))
		{
			if (Input.GetKey(KeyCode.RightShift))
			{
				cameraOffsetRotationEuler.x -= num3;
			}
			else
			{
				cameraOffsetRotationEuler.x += num3;
			}
			SetHandCameraRotation(cameraOffsetRotationEuler);
		}
		if (Input.GetKey(KeyCode.Alpha6))
		{
			if (Input.GetKey(KeyCode.RightShift))
			{
				cameraOffsetRotationEuler.y -= num3;
			}
			else
			{
				cameraOffsetRotationEuler.y += num3;
			}
			SetHandCameraRotation(cameraOffsetRotationEuler);
		}
		if (Input.GetKey(KeyCode.Alpha7))
		{
			if (Input.GetKey(KeyCode.RightShift))
			{
				cameraOffsetRotationEuler.z -= num3;
			}
			else
			{
				cameraOffsetRotationEuler.z += num3;
			}
			SetHandCameraRotation(cameraOffsetRotationEuler);
		}
		if (Input.GetKeyDown(KeyCode.Alpha8))
		{
			if (Input.GetKey(KeyCode.RightShift))
			{
				SetPovSmoothingAmount(povCameraSmoothing - num4);
			}
			else
			{
				SetPovSmoothingAmount(povCameraSmoothing + num4);
			}
		}
		if (Input.GetKeyDown(KeyCode.Alpha9))
		{
			if (currentHandControllerMode == HandControllerModeForHandCameraTypes.ControllerOnly)
			{
				currentHandControllerMode = HandControllerModeForHandCameraTypes.Normal;
			}
			else
			{
				currentHandControllerMode++;
			}
			SetCurrentHandControllerMode(currentHandControllerMode);
		}
		if (Input.GetKeyDown(KeyCode.Alpha0))
		{
			Debug.Log("Hand Camera Position:" + cameraOffsetPosition.x + "," + cameraOffsetPosition.y + "," + cameraOffsetPosition.z);
			Debug.Log("Hand Camera Rotation:" + cameraOffsetRotationEuler.x + "," + cameraOffsetRotationEuler.y + "," + cameraOffsetRotationEuler.z);
			Debug.Log("POVCameraSmoothing:" + povCameraSmoothing);
			Debug.Log("HandCameraFOV:" + recordingCameraForeground.fieldOfView);
			if (povCamera != null)
			{
				Debug.Log("POVCameraFOV:" + povCamera.fieldOfView);
			}
			else
			{
				Debug.Log("POVCameraFOV:Camera does not yet exist");
			}
		}
		if (Input.GetKeyDown(KeyCode.F5))
		{
			ResetHandCameraPositionAndRotation();
			ResetFOVHandCamera();
			ResetFOVPOVCamera();
			SetPovSmoothingAmount(0.4f);
		}
		if (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt) || Input.GetKey(KeyCode.LeftCommand) || Input.GetKey(KeyCode.RightCommand))
		{
			if (Input.GetKeyDown(KeyCode.M))
			{
				JobBoardManager.instance.TestingForceCurrentSubtaskComplete();
			}
			if (Input.GetKeyDown(KeyCode.N))
			{
				JobBoardManager.instance.TestingForceCurrentTaskComplete();
			}
			if (Input.GetKeyDown(KeyCode.V))
			{
				Toggle4CameraView();
			}
		}
		if (Input.GetKeyDown(KeyCode.Quote))
		{
			ToggleHackBackwardsMovement();
		}
		if (Input.GetKeyDown(KeyCode.Semicolon))
		{
			ToggleHackPivotMovement();
		}
	}

	private void ChangeFOVOfHandCamera(float change)
	{
		float fieldOfView = recordingCameraForeground.fieldOfView;
		fieldOfView += change;
		SetHandCameraFOV(fieldOfView);
	}

	private void ChangeFOVOfPOVCamera(float change)
	{
		float fieldOfView = povCamera.fieldOfView;
		fieldOfView += change;
		SetPOVCameraFOV(fieldOfView);
	}

	private void SetHandCameraPosition(Vector3 position)
	{
		cameraOffsetPosition = position;
		recordingCameraOffset.transform.localPosition = cameraOffsetPosition;
		PlayerPrefs.SetFloat("HandCameraPositionOffsetX", cameraOffsetPosition.x);
		PlayerPrefs.SetFloat("HandCameraPositionOffsetY", cameraOffsetPosition.y);
		PlayerPrefs.SetFloat("HandCameraPositionOffsetZ", cameraOffsetPosition.z);
		Debug.Log("Hand Camera Position:" + cameraOffsetPosition.x + "," + cameraOffsetPosition.y + "," + cameraOffsetPosition.z);
	}

	private void ResetHandCameraPositionAndRotation()
	{
		PlayerPrefs.DeleteKey("HandCameraPositionOffsetX");
		PlayerPrefs.DeleteKey("HandCameraPositionOffsetY");
		PlayerPrefs.DeleteKey("HandCameraPositionOffsetZ");
		PlayerPrefs.DeleteKey("HandCameraRotationOffsetX");
		PlayerPrefs.DeleteKey("HandCameraRotationOffsetY");
		PlayerPrefs.DeleteKey("HandCameraRotationOffsetZ");
		PlayerPrefs.Save();
		SetExistingHandPositionAndRotation();
	}

	private void SetExistingHandPositionAndRotation()
	{
		Vector3 handCameraPosition = default(Vector3);
		handCameraPosition.x = PlayerPrefs.GetFloat("HandCameraPositionOffsetX", defaultRecordingCameraLocalPosition.x);
		handCameraPosition.y = PlayerPrefs.GetFloat("HandCameraPositionOffsetY", defaultRecordingCameraLocalPosition.y);
		handCameraPosition.z = PlayerPrefs.GetFloat("HandCameraPositionOffsetZ", defaultRecordingCameraLocalPosition.z);
		SetHandCameraPosition(handCameraPosition);
		Vector3 handCameraRotation = default(Vector3);
		handCameraRotation.x = PlayerPrefs.GetFloat("HandCameraRotationOffsetX", defaultRecordingCameraLocalRotation.x);
		handCameraRotation.y = PlayerPrefs.GetFloat("HandCameraRotationOffsetY", defaultRecordingCameraLocalRotation.y);
		handCameraRotation.z = PlayerPrefs.GetFloat("HandCameraRotationOffsetZ", defaultRecordingCameraLocalRotation.z);
		SetHandCameraRotation(handCameraRotation);
	}

	private void SetPovSmoothing()
	{
		povCameraSmoothing = PlayerPrefs.GetFloat("POVCameraSmoothing", 0.4f);
		Debug.Log("POVCameraSmoothing:" + povCameraSmoothing);
	}

	private void SetPovSmoothingAmount(float newPOVSmoothingAmount)
	{
		povCameraSmoothing = Mathf.Max(0f, newPOVSmoothingAmount);
		PlayerPrefs.SetFloat("POVCameraSmoothing", povCameraSmoothing);
		Debug.Log("POVCameraSmoothing:" + povCameraSmoothing);
	}

	private void SetHandCameraRotation(Vector3 rotation)
	{
		cameraOffsetRotationEuler = rotation;
		recordingCameraOffset.transform.localEulerAngles = cameraOffsetRotationEuler;
		PlayerPrefs.SetFloat("HandCameraRotationOffsetX", cameraOffsetRotationEuler.x);
		PlayerPrefs.SetFloat("HandCameraRotationOffsetY", cameraOffsetRotationEuler.y);
		PlayerPrefs.SetFloat("HandCameraRotationOffsetZ", cameraOffsetRotationEuler.z);
		Debug.Log("Hand Camera Rotation:" + cameraOffsetRotationEuler.x + "," + cameraOffsetRotationEuler.y + "," + cameraOffsetRotationEuler.z);
	}

	private void SetHandCameraFOV(float fov)
	{
		recordingCameraForeground.fieldOfView = fov;
		recordingCameraBackground.fieldOfView = fov;
		recordingCameraFull.fieldOfView = fov;
		PlayerPrefs.SetFloat("HandCameraFOV", fov);
		Debug.Log("HandCameraFOV:" + fov);
	}

	private void SetPOVCameraFOV(float fov)
	{
		povCamera.fieldOfView = fov;
		PlayerPrefs.SetFloat("POVCameraFOV", fov);
		Debug.Log("POVCameraFOV:" + fov);
	}

	private void SetCurrentHandControllerMode(HandControllerModeForHandCameraTypes newMode)
	{
		currentHandControllerMode = newMode;
		if (currentHandControllerMode == HandControllerModeForHandCameraTypes.Normal)
		{
			Debug.Log("Normal Hand View");
			if (!IsInLayerMask(recordingCameraForeground.cullingMask, 10))
			{
				recordingCameraForeground.cullingMask |= 1024;
				recordingCameraBackground.cullingMask |= 1024;
				recordingCameraFull.cullingMask |= 1024;
			}
			SetControllerVisible(false);
		}
		else if (currentHandControllerMode == HandControllerModeForHandCameraTypes.NoHandOrController)
		{
			Debug.Log("No Hands or Controller View");
			if (IsInLayerMask(recordingCameraForeground.cullingMask, 10))
			{
				recordingCameraForeground.cullingMask &= -1025;
				recordingCameraBackground.cullingMask &= -1025;
				recordingCameraFull.cullingMask &= -1025;
			}
			SetControllerVisible(false);
		}
		else if (currentHandControllerMode == HandControllerModeForHandCameraTypes.ControllerOnly)
		{
			Debug.Log("Controller Only View");
			if (IsInLayerMask(recordingCameraForeground.cullingMask, 10))
			{
				recordingCameraForeground.cullingMask &= -1025;
				recordingCameraBackground.cullingMask &= -1025;
				recordingCameraFull.cullingMask &= -1025;
			}
			SetControllerVisible(true);
		}
	}

	private void SetControllerVisible(bool isVisible)
	{
		if (!(GlobalStorage.Instance.MasterHMDAndInputController != null))
		{
			return;
		}
		List<InteractionHandController> interactionHandControllers = GlobalStorage.Instance.MasterHMDAndInputController.InteractionHandControllers;
		for (int i = 0; i < interactionHandControllers.Count; i++)
		{
			if (interactionHandControllers[i] != null)
			{
				interactionHandControllers[i].SetControllerVisual(isVisible);
			}
		}
	}

	public bool IsInLayerMask(int mask, int layer)
	{
		return (mask & (1 << layer)) > 0;
	}

	private void ResetFOVHandCamera()
	{
		PlayerPrefs.DeleteKey("HandCameraFOV");
		SetHandCameraFOV(defaultRecordingHandCameraFOV);
	}

	private void ResetFOVPOVCamera()
	{
		PlayerPrefs.DeleteKey("POVCameraFOV");
		if (defaultPOVCameraFOV >= 0f)
		{
			SetPOVCameraFOV(defaultPOVCameraFOV);
		}
	}

	private void LateUpdate()
	{
		if (isRecording)
		{
			if (isClipping)
			{
				ClippingTechnique();
			}
			else
			{
				LayerSwapTechnique();
			}
		}
	}

	private void LayerSwapTechnique()
	{
		Vector3 position = recordingCameraOffset.transform.position;
		float num = Vector3.Distance(GlobalStorage.Instance.MasterHMDAndInputController.Head.transform.position, position);
		allRenderers = UnityEngine.Object.FindObjectsOfType<Renderer>();
		prevLayers = new int[allRenderers.Length];
		wasRendererOn = new bool[allRenderers.Length];
		isForegroundList = new bool[allRenderers.Length];
		for (int i = 0; i < allRenderers.Length; i++)
		{
			Renderer renderer = allRenderers[i];
			prevLayers[i] = renderer.gameObject.layer;
			wasRendererOn[i] = renderer.enabled && base.gameObject.activeInHierarchy;
			if (Vector3.Distance(renderer.bounds.center, position) < num)
			{
				renderer.gameObject.layer = 27;
				isForegroundList[i] = true;
			}
			else
			{
				isForegroundList[i] = false;
				renderer.gameObject.layer = 28;
			}
		}
		StartCoroutine(WaitUntilEndOfFrame());
	}

	private IEnumerator WaitUntilEndOfFrame()
	{
		yield return new WaitForEndOfFrame();
		for (int i = 0; i < allRenderers.Length; i++)
		{
			allRenderers[i].gameObject.layer = prevLayers[i];
		}
	}

	private void ClippingTechnique()
	{
		float num = Vector3.Distance(GlobalStorage.Instance.MasterHMDAndInputController.Head.transform.position, recordingCameraOffset.transform.position);
		recordingCameraForeground.farClipPlane = num + foregroundOverlapClippingExtra;
		recordingCameraBackground.nearClipPlane = num;
	}
}
