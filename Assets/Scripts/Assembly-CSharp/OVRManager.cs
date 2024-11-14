using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.VR;

public class OVRManager : MonoBehaviour
{
	public enum TrackingOrigin
	{
		EyeLevel = 0,
		FloorLevel = 1
	}

	private static OVRProfile _profile;

	private bool _isPaused;

	private IEnumerable<Camera> disabledCameras;

	private float prevTimeScale;

	private static bool _isHmdPresentCached = false;

	private static bool _isHmdPresent = false;

	private static bool _wasHmdPresent = false;

	private static bool _hasVrFocusCached = false;

	private static bool _hasVrFocus = false;

	private static bool _hadVrFocus = false;

	public bool queueAhead = true;

	public bool useRecommendedMSAALevel;

	public bool enableAdaptiveResolution;

	[Range(0.5f, 2f)]
	public float maxRenderScale = 1f;

	[Range(0.5f, 2f)]
	public float minRenderScale = 0.7f;

	[SerializeField]
	private TrackingOrigin _trackingOriginType;

	public bool usePositionTracking = true;

	public bool useIPDInPositionTracking = true;

	public bool resetTrackerOnLoad;

	private static bool _isUserPresentCached = false;

	private static bool _isUserPresent = false;

	private static bool _wasUserPresent = false;

	private static bool prevAudioOutIdIsCached = false;

	private static bool prevAudioInIdIsCached = false;

	private static string prevAudioOutId = string.Empty;

	private static string prevAudioInId = string.Empty;

	private static bool wasPositionTracked = false;

	[HideInInspector]
	[SerializeField]
	internal static bool runInBackground = false;

	private static bool isRecenteringSupported = false;

	public static OVRManager instance { get; private set; }

	public static OVRDisplay display { get; private set; }

	public static OVRTracker tracker { get; private set; }

	public static OVRBoundary boundary { get; private set; }

	public static OVRProfile profile
	{
		get
		{
			if (_profile == null)
			{
				_profile = new OVRProfile();
			}
			return _profile;
		}
	}

	private bool paused
	{
		get
		{
			return _isPaused;
		}
		set
		{
			if (value != _isPaused)
			{
				_isPaused = value;
			}
		}
	}

	public static bool isHmdPresent
	{
		get
		{
			if (!_isHmdPresentCached)
			{
				_isHmdPresentCached = true;
				_isHmdPresent = OVRPlugin.hmdPresent;
			}
			return _isHmdPresent;
		}
		private set
		{
			_isHmdPresentCached = true;
			_isHmdPresent = value;
		}
	}

	public static string audioOutId
	{
		get
		{
			return OVRPlugin.audioOutId;
		}
	}

	public static string audioInId
	{
		get
		{
			return OVRPlugin.audioInId;
		}
	}

	public static bool hasVrFocus
	{
		get
		{
			if (!_hasVrFocusCached)
			{
				_hasVrFocusCached = true;
				_hasVrFocus = OVRPlugin.hasVrFocus;
			}
			return _hasVrFocus;
		}
		private set
		{
			_hasVrFocusCached = true;
			_hasVrFocus = value;
		}
	}

	[Obsolete]
	public static bool isHSWDisplayed
	{
		get
		{
			return false;
		}
	}

	public bool chromatic
	{
		get
		{
			if (!isHmdPresent)
			{
				return false;
			}
			return OVRPlugin.chromatic;
		}
		set
		{
			if (isHmdPresent)
			{
				OVRPlugin.chromatic = value;
			}
		}
	}

	public bool monoscopic
	{
		get
		{
			if (!isHmdPresent)
			{
				return true;
			}
			return OVRPlugin.monoscopic;
		}
		set
		{
			if (isHmdPresent)
			{
				OVRPlugin.monoscopic = value;
			}
		}
	}

	public int vsyncCount
	{
		get
		{
			if (!isHmdPresent)
			{
				return 1;
			}
			return OVRPlugin.vsyncCount;
		}
		set
		{
			if (isHmdPresent)
			{
				OVRPlugin.vsyncCount = value;
			}
		}
	}

	public static float batteryLevel
	{
		get
		{
			if (!isHmdPresent)
			{
				return 1f;
			}
			return OVRPlugin.batteryLevel;
		}
	}

	public static float batteryTemperature
	{
		get
		{
			if (!isHmdPresent)
			{
				return 0f;
			}
			return OVRPlugin.batteryTemperature;
		}
	}

	public static int batteryStatus
	{
		get
		{
			if (!isHmdPresent)
			{
				return -1;
			}
			return (int)OVRPlugin.batteryStatus;
		}
	}

	public static float volumeLevel
	{
		get
		{
			if (!isHmdPresent)
			{
				return 0f;
			}
			return OVRPlugin.systemVolume;
		}
	}

	public static int cpuLevel
	{
		get
		{
			if (!isHmdPresent)
			{
				return 2;
			}
			return OVRPlugin.cpuLevel;
		}
		set
		{
			if (isHmdPresent)
			{
				OVRPlugin.cpuLevel = value;
			}
		}
	}

	public static int gpuLevel
	{
		get
		{
			if (!isHmdPresent)
			{
				return 2;
			}
			return OVRPlugin.gpuLevel;
		}
		set
		{
			if (isHmdPresent)
			{
				OVRPlugin.gpuLevel = value;
			}
		}
	}

	public static bool isPowerSavingActive
	{
		get
		{
			if (!isHmdPresent)
			{
				return false;
			}
			return OVRPlugin.powerSaving;
		}
	}

	public TrackingOrigin trackingOriginType
	{
		get
		{
			if (!isHmdPresent)
			{
				return _trackingOriginType;
			}
			return (TrackingOrigin)OVRPlugin.GetTrackingOriginType();
		}
		set
		{
			if (isHmdPresent && OVRPlugin.SetTrackingOriginType((OVRPlugin.TrackingOrigin)value))
			{
				_trackingOriginType = value;
			}
		}
	}

	public bool isSupportedPlatform { get; private set; }

	public bool isUserPresent
	{
		get
		{
			if (!_isUserPresentCached)
			{
				_isUserPresentCached = true;
				_isUserPresent = OVRPlugin.userPresent;
			}
			return _isUserPresent;
		}
		private set
		{
			_isUserPresentCached = true;
			_isUserPresent = value;
		}
	}

	public static bool IsRecenteringSupported
	{
		get
		{
			return isRecenteringSupported;
		}
		set
		{
			isRecenteringSupported = value;
		}
	}

	public static event Action HMDAcquired;

	public static event Action HMDLost;

	public static event Action HMDMounted;

	public static event Action HMDUnmounted;

	public static event Action VrFocusAcquired;

	public static event Action VrFocusLost;

	public static event Action AudioOutChanged;

	public static event Action AudioInChanged;

	public static event Action TrackingAcquired;

	public static event Action TrackingLost;

	[Obsolete]
	public static event Action HSWDismissed;

	[Obsolete]
	public static void DismissHSWDisplay()
	{
	}

	private void Awake()
	{
		if (instance != null)
		{
			base.enabled = false;
			UnityEngine.Object.DestroyImmediate(this);
			return;
		}
		instance = this;
		Debug.Log("OVR Manager!");
		Debug.Log(string.Concat("Unity v", Application.unityVersion, ", Oculus Utilities v", OVRPlugin.wrapperVersion, ", OVRPlugin v", OVRPlugin.version, ", SDK v", OVRPlugin.nativeSDKVersion, "."));
		if (SystemInfo.graphicsDeviceType != GraphicsDeviceType.Direct3D11)
		{
			Debug.LogWarning("VR rendering requires Direct3D11. Your graphics device: " + SystemInfo.graphicsDeviceType);
		}
		RuntimePlatform platform = Application.platform;
		isSupportedPlatform |= platform == RuntimePlatform.Android;
		isSupportedPlatform |= platform == RuntimePlatform.OSXEditor;
		isSupportedPlatform |= platform == RuntimePlatform.OSXPlayer;
		isSupportedPlatform |= platform == RuntimePlatform.WindowsEditor;
		isSupportedPlatform |= platform == RuntimePlatform.WindowsPlayer;
		if (!isSupportedPlatform)
		{
			Debug.LogWarning("This platform is unsupported");
			return;
		}
		if (display == null)
		{
			display = new OVRDisplay();
		}
		if (tracker == null)
		{
			tracker = new OVRTracker();
		}
		if (boundary == null)
		{
			boundary = new OVRBoundary();
		}
		if (resetTrackerOnLoad)
		{
			display.RecenterPose();
		}
		OVRPlugin.occlusionMesh = false;
		OVRPlugin.ignoreVrFocus = runInBackground;
	}

	private void Update()
	{
		paused = !OVRPlugin.hasVrFocus;
		if (OVRPlugin.shouldQuit)
		{
			Application.Quit();
		}
		if (IsRecenteringSupported && OVRPlugin.shouldRecenter)
		{
			display.RecenterPose();
		}
		if (trackingOriginType != _trackingOriginType)
		{
			trackingOriginType = _trackingOriginType;
		}
		tracker.isEnabled = usePositionTracking;
		OVRPlugin.useIPDInPositionTracking = useIPDInPositionTracking;
		isHmdPresent = OVRPlugin.hmdPresent;
		if (useRecommendedMSAALevel && QualitySettings.antiAliasing != display.recommendedMSAALevel)
		{
			Debug.Log("The current MSAA level is " + QualitySettings.antiAliasing + ", but the recommended MSAA level is " + display.recommendedMSAALevel + ". Switching to the recommended level.");
			QualitySettings.antiAliasing = display.recommendedMSAALevel;
		}
		if (_wasHmdPresent && !isHmdPresent)
		{
			try
			{
				if (OVRManager.HMDLost != null)
				{
					OVRManager.HMDLost();
				}
			}
			catch (Exception ex)
			{
				Debug.LogError("Caught Exception: " + ex);
			}
		}
		if (!_wasHmdPresent && isHmdPresent)
		{
			try
			{
				if (OVRManager.HMDAcquired != null)
				{
					OVRManager.HMDAcquired();
				}
			}
			catch (Exception ex2)
			{
				Debug.LogError("Caught Exception: " + ex2);
			}
		}
		_wasHmdPresent = isHmdPresent;
		isUserPresent = OVRPlugin.userPresent;
		if (_wasUserPresent && !isUserPresent)
		{
			try
			{
				if (OVRManager.HMDUnmounted != null)
				{
					OVRManager.HMDUnmounted();
				}
			}
			catch (Exception ex3)
			{
				Debug.LogError("Caught Exception: " + ex3);
			}
		}
		if (!_wasUserPresent && isUserPresent)
		{
			try
			{
				if (OVRManager.HMDMounted != null)
				{
					OVRManager.HMDMounted();
				}
			}
			catch (Exception ex4)
			{
				Debug.LogError("Caught Exception: " + ex4);
			}
		}
		_wasUserPresent = isUserPresent;
		hasVrFocus = OVRPlugin.hasVrFocus;
		if (_hadVrFocus && !hasVrFocus)
		{
			try
			{
				if (OVRManager.VrFocusLost != null)
				{
					OVRManager.VrFocusLost();
				}
			}
			catch (Exception ex5)
			{
				Debug.LogError("Caught Exception: " + ex5);
			}
		}
		if (!_hadVrFocus && hasVrFocus)
		{
			try
			{
				if (OVRManager.VrFocusAcquired != null)
				{
					OVRManager.VrFocusAcquired();
				}
			}
			catch (Exception ex6)
			{
				Debug.LogError("Caught Exception: " + ex6);
			}
		}
		_hadVrFocus = hasVrFocus;
		if (enableAdaptiveResolution)
		{
			if (UnityEngine.XR.XRSettings.eyeTextureResolutionScale < maxRenderScale)
			{
				UnityEngine.XR.XRSettings.eyeTextureResolutionScale = maxRenderScale;
			}
			else
			{
				maxRenderScale = Mathf.Max(maxRenderScale, UnityEngine.XR.XRSettings.eyeTextureResolutionScale);
			}
			float min = minRenderScale / UnityEngine.XR.XRSettings.eyeTextureResolutionScale;
			float value = OVRPlugin.GetEyeRecommendedResolutionScale() / UnityEngine.XR.XRSettings.eyeTextureResolutionScale;
			value = Mathf.Clamp(value, min, 1f);
			UnityEngine.XR.XRSettings.renderViewportScale = value;
		}
		string text = OVRPlugin.audioOutId;
		if (!prevAudioOutIdIsCached)
		{
			prevAudioOutId = text;
			prevAudioOutIdIsCached = true;
		}
		else if (text != prevAudioOutId)
		{
			try
			{
				if (OVRManager.AudioOutChanged != null)
				{
					OVRManager.AudioOutChanged();
				}
			}
			catch (Exception ex7)
			{
				Debug.LogError("Caught Exception: " + ex7);
			}
			prevAudioOutId = text;
		}
		string text2 = OVRPlugin.audioInId;
		if (!prevAudioInIdIsCached)
		{
			prevAudioInId = text2;
			prevAudioInIdIsCached = true;
		}
		else if (text2 != prevAudioInId)
		{
			try
			{
				if (OVRManager.AudioInChanged != null)
				{
					OVRManager.AudioInChanged();
				}
			}
			catch (Exception ex8)
			{
				Debug.LogError("Caught Exception: " + ex8);
			}
			prevAudioInId = text2;
		}
		if (wasPositionTracked && !tracker.isPositionTracked)
		{
			try
			{
				if (OVRManager.TrackingLost != null)
				{
					OVRManager.TrackingLost();
				}
			}
			catch (Exception ex9)
			{
				Debug.LogError("Caught Exception: " + ex9);
			}
		}
		if (!wasPositionTracked && tracker.isPositionTracked)
		{
			try
			{
				if (OVRManager.TrackingAcquired != null)
				{
					OVRManager.TrackingAcquired();
				}
			}
			catch (Exception ex10)
			{
				Debug.LogError("Caught Exception: " + ex10);
			}
		}
		wasPositionTracked = tracker.isPositionTracked;
		display.Update();
		OVRInput.Update();
	}

	private void LateUpdate()
	{
		OVRHaptics.Process();
	}

	private void FixedUpdate()
	{
		OVRInput.FixedUpdate();
	}

	public void ReturnToLauncher()
	{
		PlatformUIConfirmQuit();
	}

	public static void PlatformUIConfirmQuit()
	{
		if (isHmdPresent)
		{
			OVRPlugin.ShowUI(OVRPlugin.PlatformUI.ConfirmQuit);
		}
	}

	public static void PlatformUIGlobalMenu()
	{
		if (isHmdPresent)
		{
			OVRPlugin.ShowUI(OVRPlugin.PlatformUI.GlobalMenu);
		}
	}
}
