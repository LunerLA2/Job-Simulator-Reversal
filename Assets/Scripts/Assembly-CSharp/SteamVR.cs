using System;
using System.Text;
using UnityEngine;
using UnityEngine.VR;
using Valve.VR;

public class SteamVR : IDisposable
{
	private static bool _enabled = true;

	private static SteamVR _instance;

	public static bool[] connected = new bool[16];

	public EGraphicsAPIConvention graphicsAPI;

	public static bool active
	{
		get
		{
			return _instance != null;
		}
	}

	public static bool enabled
	{
		get
		{
			return _enabled;
		}
		set
		{
			_enabled = value;
			if (!_enabled)
			{
				SafeDispose();
			}
		}
	}

	public static SteamVR instance
	{
		get
		{
			if (!enabled)
			{
				return null;
			}
			if (_instance == null)
			{
				_instance = CreateInstance();
				if (_instance == null)
				{
					_enabled = false;
				}
			}
			return _instance;
		}
	}

	public static bool usingNativeSupport
	{
		get
		{
			return UnityEngine.XR.XRDevice.GetNativePtr() != IntPtr.Zero;
		}
	}

	public CVRSystem hmd { get; private set; }

	public CVRCompositor compositor { get; private set; }

	public CVROverlay overlay { get; private set; }

	public static bool initializing { get; private set; }

	public static bool calibrating { get; private set; }

	public static bool outOfRange { get; private set; }

	public float sceneWidth { get; private set; }

	public float sceneHeight { get; private set; }

	public float aspect { get; private set; }

	public float fieldOfView { get; private set; }

	public Vector2 tanHalfFov { get; private set; }

	public VRTextureBounds_t[] textureBounds { get; private set; }

	public SteamVR_Utils.RigidTransform[] eyes { get; private set; }

	public string hmd_TrackingSystemName
	{
		get
		{
			return GetStringProperty(ETrackedDeviceProperty.Prop_TrackingSystemName_String);
		}
	}

	public string hmd_ModelNumber
	{
		get
		{
			return GetStringProperty(ETrackedDeviceProperty.Prop_ModelNumber_String);
		}
	}

	public string hmd_SerialNumber
	{
		get
		{
			return GetStringProperty(ETrackedDeviceProperty.Prop_SerialNumber_String);
		}
	}

	public float hmd_SecondsFromVsyncToPhotons
	{
		get
		{
			return GetFloatProperty(ETrackedDeviceProperty.Prop_SecondsFromVsyncToPhotons_Float);
		}
	}

	public float hmd_DisplayFrequency
	{
		get
		{
			return GetFloatProperty(ETrackedDeviceProperty.Prop_DisplayFrequency_Float);
		}
	}

	private SteamVR()
	{
		hmd = OpenVR.System;
		Debug.Log("Connected to " + hmd_TrackingSystemName + ":" + hmd_SerialNumber);
		compositor = OpenVR.Compositor;
		overlay = OpenVR.Overlay;
		uint pnWidth = 0u;
		uint pnHeight = 0u;
		hmd.GetRecommendedRenderTargetSize(ref pnWidth, ref pnHeight);
		sceneWidth = pnWidth;
		sceneHeight = pnHeight;
		float pfLeft = 0f;
		float pfRight = 0f;
		float pfTop = 0f;
		float pfBottom = 0f;
		hmd.GetProjectionRaw(EVREye.Eye_Left, ref pfLeft, ref pfRight, ref pfTop, ref pfBottom);
		float pfLeft2 = 0f;
		float pfRight2 = 0f;
		float pfTop2 = 0f;
		float pfBottom2 = 0f;
		hmd.GetProjectionRaw(EVREye.Eye_Right, ref pfLeft2, ref pfRight2, ref pfTop2, ref pfBottom2);
		tanHalfFov = new Vector2(Mathf.Max(0f - pfLeft, pfRight, 0f - pfLeft2, pfRight2), Mathf.Max(0f - pfTop, pfBottom, 0f - pfTop2, pfBottom2));
		textureBounds = new VRTextureBounds_t[2];
		textureBounds[0].uMin = 0.5f + 0.5f * pfLeft / tanHalfFov.x;
		textureBounds[0].uMax = 0.5f + 0.5f * pfRight / tanHalfFov.x;
		textureBounds[0].vMin = 0.5f - 0.5f * pfBottom / tanHalfFov.y;
		textureBounds[0].vMax = 0.5f - 0.5f * pfTop / tanHalfFov.y;
		textureBounds[1].uMin = 0.5f + 0.5f * pfLeft2 / tanHalfFov.x;
		textureBounds[1].uMax = 0.5f + 0.5f * pfRight2 / tanHalfFov.x;
		textureBounds[1].vMin = 0.5f - 0.5f * pfBottom2 / tanHalfFov.y;
		textureBounds[1].vMax = 0.5f - 0.5f * pfTop2 / tanHalfFov.y;
		sceneWidth /= Mathf.Max(textureBounds[0].uMax - textureBounds[0].uMin, textureBounds[1].uMax - textureBounds[1].uMin);
		sceneHeight /= Mathf.Max(textureBounds[0].vMax - textureBounds[0].vMin, textureBounds[1].vMax - textureBounds[1].vMin);
		aspect = tanHalfFov.x / tanHalfFov.y;
		fieldOfView = 2f * Mathf.Atan(tanHalfFov.y) * 57.29578f;
		eyes = new SteamVR_Utils.RigidTransform[2]
		{
			new SteamVR_Utils.RigidTransform(hmd.GetEyeToHeadTransform(EVREye.Eye_Left)),
			new SteamVR_Utils.RigidTransform(hmd.GetEyeToHeadTransform(EVREye.Eye_Right))
		};
		if (SystemInfo.graphicsDeviceVersion.StartsWith("OpenGL"))
		{
			graphicsAPI = EGraphicsAPIConvention.API_OpenGL;
		}
		else
		{
			graphicsAPI = EGraphicsAPIConvention.API_DirectX;
		}
		SteamVR_Utils.Event.Listen("initializing", OnInitializing);
		SteamVR_Utils.Event.Listen("calibrating", OnCalibrating);
		SteamVR_Utils.Event.Listen("out_of_range", OnOutOfRange);
		SteamVR_Utils.Event.Listen("device_connected", OnDeviceConnected);
		SteamVR_Utils.Event.Listen("new_poses", OnNewPoses);
	}

	private static SteamVR CreateInstance()
	{
		try
		{
			EVRInitError peError = EVRInitError.None;
			if (!usingNativeSupport)
			{
				Debug.Log("OpenVR initialization failed.  Ensure 'Virtual Reality Supported' is checked in Player Settings, and OpenVR is added to the list of Virtual Reality SDKs.");
				return null;
			}
			OpenVR.GetGenericInterface("IVRCompositor_013", ref peError);
			if (peError != 0)
			{
				ReportError(peError);
				ShutdownSystems();
				return null;
			}
			OpenVR.GetGenericInterface("IVROverlay_010", ref peError);
			if (peError != 0)
			{
				ReportError(peError);
				ShutdownSystems();
				return null;
			}
		}
		catch (Exception message)
		{
			Debug.LogError(message);
			return null;
		}
		return new SteamVR();
	}

	private static void ReportError(EVRInitError error)
	{
		switch (error)
		{
		case EVRInitError.None:
			break;
		case EVRInitError.VendorSpecific_UnableToConnectToOculusRuntime:
			Debug.Log("SteamVR Initialization Failed!  Make sure device is on, Oculus runtime is installed, and OVRService_*.exe is running.");
			break;
		case EVRInitError.Init_VRClientDLLNotFound:
			Debug.Log("SteamVR drivers not found!  They can be installed via Steam under Library > Tools.  Visit http://steampowered.com to install Steam.");
			break;
		case EVRInitError.Driver_RuntimeOutOfDate:
			Debug.Log("SteamVR Initialization Failed!  Make sure device's runtime is up to date.");
			break;
		default:
			Debug.Log(OpenVR.GetStringForHmdError(error));
			break;
		}
	}

	public string GetTrackedDeviceString(uint deviceId)
	{
		ETrackedPropertyError pError = ETrackedPropertyError.TrackedProp_Success;
		uint stringTrackedDeviceProperty = hmd.GetStringTrackedDeviceProperty(deviceId, ETrackedDeviceProperty.Prop_AttachedDeviceId_String, null, 0u, ref pError);
		if (stringTrackedDeviceProperty > 1)
		{
			StringBuilder stringBuilder = new StringBuilder((int)stringTrackedDeviceProperty);
			hmd.GetStringTrackedDeviceProperty(deviceId, ETrackedDeviceProperty.Prop_AttachedDeviceId_String, stringBuilder, stringTrackedDeviceProperty, ref pError);
			return stringBuilder.ToString();
		}
		return null;
	}

	private string GetStringProperty(ETrackedDeviceProperty prop)
	{
		ETrackedPropertyError pError = ETrackedPropertyError.TrackedProp_Success;
		uint stringTrackedDeviceProperty = hmd.GetStringTrackedDeviceProperty(0u, prop, null, 0u, ref pError);
		if (stringTrackedDeviceProperty > 1)
		{
			StringBuilder stringBuilder = new StringBuilder((int)stringTrackedDeviceProperty);
			hmd.GetStringTrackedDeviceProperty(0u, prop, stringBuilder, stringTrackedDeviceProperty, ref pError);
			return stringBuilder.ToString();
		}
		return (pError == ETrackedPropertyError.TrackedProp_Success) ? "<unknown>" : pError.ToString();
	}

	private float GetFloatProperty(ETrackedDeviceProperty prop)
	{
		ETrackedPropertyError pError = ETrackedPropertyError.TrackedProp_Success;
		return hmd.GetFloatTrackedDeviceProperty(0u, prop, ref pError);
	}

	private void OnInitializing(params object[] args)
	{
		initializing = (bool)args[0];
	}

	private void OnCalibrating(params object[] args)
	{
		calibrating = (bool)args[0];
	}

	private void OnOutOfRange(params object[] args)
	{
		outOfRange = (bool)args[0];
	}

	private void OnDeviceConnected(params object[] args)
	{
		int num = (int)args[0];
		connected[num] = (bool)args[1];
	}

	private void OnNewPoses(params object[] args)
	{
		TrackedDevicePose_t[] array = (TrackedDevicePose_t[])args[0];
		eyes[0] = new SteamVR_Utils.RigidTransform(hmd.GetEyeToHeadTransform(EVREye.Eye_Left));
		eyes[1] = new SteamVR_Utils.RigidTransform(hmd.GetEyeToHeadTransform(EVREye.Eye_Right));
		for (int i = 0; i < array.Length; i++)
		{
			bool bDeviceIsConnected = array[i].bDeviceIsConnected;
			if (bDeviceIsConnected != connected[i])
			{
				SteamVR_Utils.Event.Send("device_connected", i, bDeviceIsConnected);
			}
		}
		if ((long)array.Length > 0L)
		{
			ETrackingResult eTrackingResult = array[0].eTrackingResult;
			bool flag = eTrackingResult == ETrackingResult.Uninitialized;
			if (flag != initializing)
			{
				SteamVR_Utils.Event.Send("initializing", flag);
			}
			bool flag2 = eTrackingResult == ETrackingResult.Calibrating_InProgress || eTrackingResult == ETrackingResult.Calibrating_OutOfRange;
			if (flag2 != calibrating)
			{
				SteamVR_Utils.Event.Send("calibrating", flag2);
			}
			bool flag3 = eTrackingResult == ETrackingResult.Running_OutOfRange || eTrackingResult == ETrackingResult.Calibrating_OutOfRange;
			if (flag3 != outOfRange)
			{
				SteamVR_Utils.Event.Send("out_of_range", flag3);
			}
		}
	}

	~SteamVR()
	{
		Dispose(false);
	}

	public void Dispose()
	{
		Dispose(true);
		GC.SuppressFinalize(this);
	}

	private void Dispose(bool disposing)
	{
		SteamVR_Utils.Event.Remove("initializing", OnInitializing);
		SteamVR_Utils.Event.Remove("calibrating", OnCalibrating);
		SteamVR_Utils.Event.Remove("out_of_range", OnOutOfRange);
		SteamVR_Utils.Event.Remove("device_connected", OnDeviceConnected);
		SteamVR_Utils.Event.Remove("new_poses", OnNewPoses);
		ShutdownSystems();
		_instance = null;
	}

	private static void ShutdownSystems()
	{
	}

	public static void SafeDispose()
	{
		if (_instance != null)
		{
			_instance.Dispose();
		}
	}
}
