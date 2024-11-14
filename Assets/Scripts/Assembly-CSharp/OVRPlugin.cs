using System;
using System.Runtime.InteropServices;

internal static class OVRPlugin
{
	private struct GUID
	{
		public int a;

		public short b;

		public short c;

		public byte d0;

		public byte d1;

		public byte d2;

		public byte d3;

		public byte d4;

		public byte d5;

		public byte d6;

		public byte d7;
	}

	public enum Bool
	{
		False = 0,
		True = 1
	}

	public enum Eye
	{
		None = -1,
		Left = 0,
		Right = 1,
		Count = 2
	}

	public enum Tracker
	{
		None = -1,
		Zero = 0,
		One = 1,
		Two = 2,
		Four = 3,
		Count = 4
	}

	public enum Node
	{
		None = -1,
		EyeLeft = 0,
		EyeRight = 1,
		EyeCenter = 2,
		HandLeft = 3,
		HandRight = 4,
		TrackerZero = 5,
		TrackerOne = 6,
		TrackerTwo = 7,
		TrackerThree = 8,
		Head = 9,
		Count = 10
	}

	public enum Controller
	{
		None = 0,
		LTouch = 1,
		RTouch = 2,
		Touch = 3,
		Remote = 4,
		Gamepad = 8,
		Active = int.MinValue,
		All = -1
	}

	public enum TrackingOrigin
	{
		EyeLevel = 0,
		FloorLevel = 1,
		Count = 2
	}

	public enum RecenterFlags
	{
		Default = 0,
		IgnoreAll = int.MinValue,
		Count = -2147483647
	}

	public enum BatteryStatus
	{
		Charging = 0,
		Discharging = 1,
		Full = 2,
		NotCharging = 3,
		Unknown = 4
	}

	public enum PlatformUI
	{
		None = -1,
		GlobalMenu = 0,
		ConfirmQuit = 1,
		GlobalMenuTutorial = 2
	}

	public enum SystemRegion
	{
		Unspecified = 0,
		Japan = 1,
		China = 2
	}

	public enum OverlayShape
	{
		Quad = 0,
		Cylinder = 1,
		Cubemap = 2
	}

	private enum OverlayFlag
	{
		None = 0,
		OnTop = 1,
		HeadLocked = 2,
		ShapeFlag_Quad = 0,
		ShapeFlag_Cylinder = 16,
		ShapeFlag_Cubemap = 32,
		ShapeFlagRangeMask = 240
	}

	public struct Vector2i
	{
		public int x;

		public int y;
	}

	public struct Vector2f
	{
		public float x;

		public float y;
	}

	public struct Vector3f
	{
		public float x;

		public float y;

		public float z;
	}

	public struct Quatf
	{
		public float x;

		public float y;

		public float z;

		public float w;
	}

	public struct Posef
	{
		public Quatf Orientation;

		public Vector3f Position;
	}

	internal struct InputState
	{
		public uint ConnectedControllers;

		public uint Buttons;

		public uint Touches;

		public uint NearTouches;

		public float LIndexTrigger;

		public float RIndexTrigger;

		public float LHandTrigger;

		public float RHandTrigger;

		public Vector2f LThumbstick;

		public Vector2f RThumbstick;
	}

	public struct ControllerState
	{
		public uint ConnectedControllers;

		public uint Buttons;

		public uint Touches;

		public uint NearTouches;

		public float LIndexTrigger;

		public float RIndexTrigger;

		public float LHandTrigger;

		public float RHandTrigger;

		public Vector2f LThumbstick;

		public Vector2f RThumbstick;

		internal ControllerState(InputState inputState)
		{
			ConnectedControllers = inputState.ConnectedControllers;
			Buttons = inputState.Buttons;
			Touches = inputState.Touches;
			NearTouches = inputState.NearTouches;
			LIndexTrigger = inputState.LIndexTrigger;
			RIndexTrigger = inputState.RIndexTrigger;
			LHandTrigger = inputState.LHandTrigger;
			RHandTrigger = inputState.RHandTrigger;
			LThumbstick = inputState.LThumbstick;
			RThumbstick = inputState.RThumbstick;
		}
	}

	public struct HapticsBuffer
	{
		public IntPtr Samples;

		public int SamplesCount;
	}

	public struct HapticsState
	{
		public int SamplesAvailable;

		public int SamplesQueued;
	}

	public struct HapticsDesc
	{
		public int SampleRateHz;

		public int SampleSizeInBytes;

		public int MinimumSafeSamplesQueued;

		public int MinimumBufferSamplesCount;

		public int OptimalBufferSamplesCount;

		public int MaximumBufferSamplesCount;
	}

	public struct Sizei
	{
		public int w;

		public int h;
	}

	public struct Frustumf
	{
		public float zNear;

		public float zFar;

		public float fovX;

		public float fovY;
	}

	public enum BoundaryType
	{
		OuterBoundary = 1,
		PlayArea = 0x100
	}

	public struct BoundaryTestResult
	{
		public Bool IsTriggering;

		public float ClosestDistance;

		public Vector3f ClosestPoint;

		public Vector3f ClosestPointNormal;
	}

	public struct BoundaryLookAndFeel
	{
		public Colorf Color;
	}

	public struct BoundaryGeometry
	{
		public BoundaryType BoundaryType;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
		public Vector3f[] Points;

		public int PointsCount;
	}

	public struct Colorf
	{
		public float r;

		public float g;

		public float b;

		public float a;
	}

	private static class OVRP_0_1_0
	{
		public static readonly Version version = new Version(0, 1, 0);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern Sizei ovrp_GetEyeTextureSize(Eye eyeId);
	}

	private static class OVRP_0_1_1
	{
		public static readonly Version version = new Version(0, 1, 1);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern Bool ovrp_SetOverlayQuad2(Bool onTop, Bool headLocked, IntPtr texture, IntPtr device, Posef pose, Vector3f scale);
	}

	private static class OVRP_0_1_2
	{
		public static readonly Version version = new Version(0, 1, 2);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern Posef ovrp_GetNodePose(Node nodeId);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern Bool ovrp_SetControllerVibration(uint controllerMask, float frequency, float amplitude);
	}

	private static class OVRP_0_1_3
	{
		public static readonly Version version = new Version(0, 1, 3);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern Posef ovrp_GetNodeVelocity(Node nodeId);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern Posef ovrp_GetNodeAcceleration(Node nodeId);
	}

	private static class OVRP_0_5_0
	{
		public static readonly Version version = new Version(0, 5, 0);
	}

	private static class OVRP_1_0_0
	{
		public static readonly Version version = new Version(1, 0, 0);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern TrackingOrigin ovrp_GetTrackingOriginType();

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern Bool ovrp_SetTrackingOriginType(TrackingOrigin originType);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern Posef ovrp_GetTrackingCalibratedOrigin();

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern Bool ovrp_RecenterTrackingOrigin(uint flags);
	}

	private static class OVRP_1_1_0
	{
		public static readonly Version version = new Version(1, 1, 0);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern Bool ovrp_GetInitialized();

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovrp_GetVersion")]
		private static extern IntPtr _ovrp_GetVersion();

		public static string ovrp_GetVersion()
		{
			return Marshal.PtrToStringAnsi(_ovrp_GetVersion());
		}

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovrp_GetNativeSDKVersion")]
		private static extern IntPtr _ovrp_GetNativeSDKVersion();

		public static string ovrp_GetNativeSDKVersion()
		{
			return Marshal.PtrToStringAnsi(_ovrp_GetNativeSDKVersion());
		}

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr ovrp_GetAudioOutId();

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr ovrp_GetAudioInId();

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern float ovrp_GetEyeTextureScale();

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern Bool ovrp_SetEyeTextureScale(float value);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern Bool ovrp_GetTrackingOrientationSupported();

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern Bool ovrp_GetTrackingOrientationEnabled();

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern Bool ovrp_SetTrackingOrientationEnabled(Bool value);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern Bool ovrp_GetTrackingPositionSupported();

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern Bool ovrp_GetTrackingPositionEnabled();

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern Bool ovrp_SetTrackingPositionEnabled(Bool value);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern Bool ovrp_GetNodePresent(Node nodeId);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern Bool ovrp_GetNodeOrientationTracked(Node nodeId);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern Bool ovrp_GetNodePositionTracked(Node nodeId);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern Frustumf ovrp_GetNodeFrustum(Node nodeId);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern ControllerState ovrp_GetControllerState(uint controllerMask);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern int ovrp_GetSystemCpuLevel();

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern Bool ovrp_SetSystemCpuLevel(int value);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern int ovrp_GetSystemGpuLevel();

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern Bool ovrp_SetSystemGpuLevel(int value);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern Bool ovrp_GetSystemPowerSavingMode();

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern float ovrp_GetSystemDisplayFrequency();

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern int ovrp_GetSystemVSyncCount();

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern float ovrp_GetSystemVolume();

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern BatteryStatus ovrp_GetSystemBatteryStatus();

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern float ovrp_GetSystemBatteryLevel();

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern float ovrp_GetSystemBatteryTemperature();

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovrp_GetSystemProductName")]
		private static extern IntPtr _ovrp_GetSystemProductName();

		public static string ovrp_GetSystemProductName()
		{
			return Marshal.PtrToStringAnsi(_ovrp_GetSystemProductName());
		}

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern Bool ovrp_ShowSystemUI(PlatformUI ui);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern Bool ovrp_GetAppMonoscopic();

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern Bool ovrp_SetAppMonoscopic(Bool value);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern Bool ovrp_GetAppHasVrFocus();

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern Bool ovrp_GetAppShouldQuit();

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern Bool ovrp_GetAppShouldRecenter();

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovrp_GetAppLatencyTimings")]
		private static extern IntPtr _ovrp_GetAppLatencyTimings();

		public static string ovrp_GetAppLatencyTimings()
		{
			return Marshal.PtrToStringAnsi(_ovrp_GetAppLatencyTimings());
		}

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern Bool ovrp_GetUserPresent();

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern float ovrp_GetUserIPD();

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern Bool ovrp_SetUserIPD(float value);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern float ovrp_GetUserEyeDepth();

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern Bool ovrp_SetUserEyeDepth(float value);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern float ovrp_GetUserEyeHeight();

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern Bool ovrp_SetUserEyeHeight(float value);
	}

	private static class OVRP_1_2_0
	{
		public static readonly Version version = new Version(1, 2, 0);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern Bool ovrp_SetSystemVSyncCount(int vsyncCount);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern Bool ovrpi_SetTrackingCalibratedOrigin();
	}

	private static class OVRP_1_2_1
	{
		public static readonly Version version = new Version(1, 2, 1);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern Bool ovrp_SetAppIgnoreVrFocus(Bool value);
	}

	private static class OVRP_1_3_0
	{
		public static readonly Version version = new Version(1, 3, 0);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern Bool ovrp_GetEyeOcclusionMeshEnabled();

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern Bool ovrp_SetEyeOcclusionMeshEnabled(Bool value);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern Bool ovrp_GetSystemHeadphonesPresent();
	}

	private static class OVRP_1_5_0
	{
		public static readonly Version version = new Version(1, 5, 0);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern SystemRegion ovrp_GetSystemRegion();
	}

	private static class OVRP_1_6_0
	{
		public static readonly Version version = new Version(1, 6, 0);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern Bool ovrp_GetTrackingIPDEnabled();

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern Bool ovrp_SetTrackingIPDEnabled(Bool value);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern HapticsDesc ovrp_GetControllerHapticsDesc(uint controllerMask);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern HapticsState ovrp_GetControllerHapticsState(uint controllerMask);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern Bool ovrp_SetControllerHaptics(uint controllerMask, HapticsBuffer hapticsBuffer);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern Bool ovrp_SetOverlayQuad3(uint flags, IntPtr textureLeft, IntPtr textureRight, IntPtr device, Posef pose, Vector3f scale, int layerIndex);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern float ovrp_GetEyeRecommendedResolutionScale();

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern float ovrp_GetAppCpuStartToGpuEndTime();

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern int ovrp_GetSystemRecommendedMSAALevel();
	}

	private static class OVRP_1_7_0
	{
		public static readonly Version version = new Version(1, 7, 0);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern Bool ovrp_GetAppChromaticCorrection();

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern Bool ovrp_SetAppChromaticCorrection(Bool value);
	}

	private static class OVRP_1_8_0
	{
		public static readonly Version version = new Version(1, 8, 0);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern Bool ovrp_GetBoundaryConfigured();

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern BoundaryTestResult ovrp_TestBoundaryNode(Node nodeId, BoundaryType boundaryType);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern BoundaryTestResult ovrp_TestBoundaryPoint(Vector3f point, BoundaryType boundaryType);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern Bool ovrp_SetBoundaryLookAndFeel(BoundaryLookAndFeel lookAndFeel);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern Bool ovrp_ResetBoundaryLookAndFeel();

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern BoundaryGeometry ovrp_GetBoundaryGeometry(BoundaryType boundaryType);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern Vector3f ovrp_GetBoundaryDimensions(BoundaryType boundaryType);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern Bool ovrp_GetBoundaryVisible();

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern Bool ovrp_SetBoundaryVisible(Bool value);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern Bool ovrp_Update2(int stateId, int frameIndex, double predictionSeconds);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern Posef ovrp_GetNodePose2(int stateId, Node nodeId);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern Posef ovrp_GetNodeVelocity2(int stateId, Node nodeId);

		[DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
		public static extern Posef ovrp_GetNodeAcceleration2(int stateId, Node nodeId);
	}

	private const int OverlayShapeFlagShift = 4;

	private const string pluginName = "OVRPlugin";

	public static readonly Version wrapperVersion = OVRP_1_8_0.version;

	private static Version _version;

	private static Version _nativeSDKVersion;

	private static Guid _cachedAudioOutGuid;

	private static string _cachedAudioOutString;

	private static Guid _cachedAudioInGuid;

	private static string _cachedAudioInString;

	public static Version version
	{
		get
		{
			if (_version == null)
			{
				try
				{
					string text = OVRP_1_1_0.ovrp_GetVersion();
					if (text != null)
					{
						text = text.Split('-')[0];
						_version = new Version(text);
					}
					else
					{
						_version = new Version(0, 0, 0);
					}
				}
				catch
				{
					_version = new Version(0, 0, 0);
				}
				if (_version == OVRP_0_5_0.version)
				{
					_version = OVRP_0_1_0.version;
				}
				if (_version < OVRP_1_3_0.version)
				{
					throw new PlatformNotSupportedException(string.Concat("Oculus Utilities version ", wrapperVersion, " is too new for OVRPlugin version ", _version.ToString(), ". Update to the latest version of Unity."));
				}
			}
			return _version;
		}
	}

	public static Version nativeSDKVersion
	{
		get
		{
			if (_nativeSDKVersion == null)
			{
				try
				{
					string empty = string.Empty;
					empty = ((!(version >= OVRP_1_1_0.version)) ? "0.0.0" : OVRP_1_1_0.ovrp_GetNativeSDKVersion());
					if (empty != null)
					{
						empty = empty.Split('-')[0];
						_nativeSDKVersion = new Version(empty);
					}
					else
					{
						_nativeSDKVersion = new Version(0, 0, 0);
					}
				}
				catch
				{
					_nativeSDKVersion = new Version(0, 0, 0);
				}
			}
			return _nativeSDKVersion;
		}
	}

	public static bool initialized
	{
		get
		{
			return OVRP_1_1_0.ovrp_GetInitialized() == Bool.True;
		}
	}

	public static bool chromatic
	{
		get
		{
			if (version >= OVRP_1_7_0.version)
			{
				return OVRP_1_7_0.ovrp_GetAppChromaticCorrection() == Bool.True;
			}
			return true;
		}
		set
		{
			if (version >= OVRP_1_7_0.version)
			{
				OVRP_1_7_0.ovrp_SetAppChromaticCorrection(ToBool(value));
			}
		}
	}

	public static bool monoscopic
	{
		get
		{
			return OVRP_1_1_0.ovrp_GetAppMonoscopic() == Bool.True;
		}
		set
		{
			OVRP_1_1_0.ovrp_SetAppMonoscopic(ToBool(value));
		}
	}

	public static bool rotation
	{
		get
		{
			return OVRP_1_1_0.ovrp_GetTrackingOrientationEnabled() == Bool.True;
		}
		set
		{
			OVRP_1_1_0.ovrp_SetTrackingOrientationEnabled(ToBool(value));
		}
	}

	public static bool position
	{
		get
		{
			return OVRP_1_1_0.ovrp_GetTrackingPositionEnabled() == Bool.True;
		}
		set
		{
			OVRP_1_1_0.ovrp_SetTrackingPositionEnabled(ToBool(value));
		}
	}

	public static bool useIPDInPositionTracking
	{
		get
		{
			if (version >= OVRP_1_6_0.version)
			{
				return OVRP_1_6_0.ovrp_GetTrackingIPDEnabled() == Bool.True;
			}
			return true;
		}
		set
		{
			if (version >= OVRP_1_6_0.version)
			{
				OVRP_1_6_0.ovrp_SetTrackingIPDEnabled(ToBool(value));
			}
		}
	}

	public static bool positionSupported
	{
		get
		{
			return OVRP_1_1_0.ovrp_GetTrackingPositionSupported() == Bool.True;
		}
	}

	public static bool positionTracked
	{
		get
		{
			return OVRP_1_1_0.ovrp_GetNodePositionTracked(Node.EyeCenter) == Bool.True;
		}
	}

	public static bool powerSaving
	{
		get
		{
			return OVRP_1_1_0.ovrp_GetSystemPowerSavingMode() == Bool.True;
		}
	}

	public static bool hmdPresent
	{
		get
		{
			return OVRP_1_1_0.ovrp_GetNodePresent(Node.EyeCenter) == Bool.True;
		}
	}

	public static bool userPresent
	{
		get
		{
			return OVRP_1_1_0.ovrp_GetUserPresent() == Bool.True;
		}
	}

	public static bool headphonesPresent
	{
		get
		{
			return OVRP_1_3_0.ovrp_GetSystemHeadphonesPresent() == Bool.True;
		}
	}

	public static int recommendedMSAALevel
	{
		get
		{
			if (version >= OVRP_1_6_0.version)
			{
				return OVRP_1_6_0.ovrp_GetSystemRecommendedMSAALevel();
			}
			return 2;
		}
	}

	public static SystemRegion systemRegion
	{
		get
		{
			if (version >= OVRP_1_5_0.version)
			{
				return OVRP_1_5_0.ovrp_GetSystemRegion();
			}
			return SystemRegion.Unspecified;
		}
	}

	public static string audioOutId
	{
		get
		{
			try
			{
				IntPtr intPtr = OVRP_1_1_0.ovrp_GetAudioOutId();
				if (intPtr != IntPtr.Zero)
				{
					GUID gUID = (GUID)Marshal.PtrToStructure(intPtr, typeof(GUID));
					Guid guid = new Guid(gUID.a, gUID.b, gUID.c, gUID.d0, gUID.d1, gUID.d2, gUID.d3, gUID.d4, gUID.d5, gUID.d6, gUID.d7);
					if (guid != _cachedAudioOutGuid)
					{
						_cachedAudioOutGuid = guid;
						_cachedAudioOutString = _cachedAudioOutGuid.ToString();
					}
					return _cachedAudioOutString;
				}
			}
			catch
			{
			}
			return string.Empty;
		}
	}

	public static string audioInId
	{
		get
		{
			try
			{
				IntPtr intPtr = OVRP_1_1_0.ovrp_GetAudioInId();
				if (intPtr != IntPtr.Zero)
				{
					GUID gUID = (GUID)Marshal.PtrToStructure(intPtr, typeof(GUID));
					Guid guid = new Guid(gUID.a, gUID.b, gUID.c, gUID.d0, gUID.d1, gUID.d2, gUID.d3, gUID.d4, gUID.d5, gUID.d6, gUID.d7);
					if (guid != _cachedAudioInGuid)
					{
						_cachedAudioInGuid = guid;
						_cachedAudioInString = _cachedAudioInGuid.ToString();
					}
					return _cachedAudioInString;
				}
			}
			catch
			{
			}
			return string.Empty;
		}
	}

	public static bool hasVrFocus
	{
		get
		{
			return OVRP_1_1_0.ovrp_GetAppHasVrFocus() == Bool.True;
		}
	}

	public static bool shouldQuit
	{
		get
		{
			return OVRP_1_1_0.ovrp_GetAppShouldQuit() == Bool.True;
		}
	}

	public static bool shouldRecenter
	{
		get
		{
			return OVRP_1_1_0.ovrp_GetAppShouldRecenter() == Bool.True;
		}
	}

	public static string productName
	{
		get
		{
			return OVRP_1_1_0.ovrp_GetSystemProductName();
		}
	}

	public static string latency
	{
		get
		{
			return OVRP_1_1_0.ovrp_GetAppLatencyTimings();
		}
	}

	public static float eyeDepth
	{
		get
		{
			return OVRP_1_1_0.ovrp_GetUserEyeDepth();
		}
		set
		{
			OVRP_1_1_0.ovrp_SetUserEyeDepth(value);
		}
	}

	public static float eyeHeight
	{
		get
		{
			return OVRP_1_1_0.ovrp_GetUserEyeHeight();
		}
		set
		{
			OVRP_1_1_0.ovrp_SetUserEyeHeight(value);
		}
	}

	public static float batteryLevel
	{
		get
		{
			return OVRP_1_1_0.ovrp_GetSystemBatteryLevel();
		}
	}

	public static float batteryTemperature
	{
		get
		{
			return OVRP_1_1_0.ovrp_GetSystemBatteryTemperature();
		}
	}

	public static int cpuLevel
	{
		get
		{
			return OVRP_1_1_0.ovrp_GetSystemCpuLevel();
		}
		set
		{
			OVRP_1_1_0.ovrp_SetSystemCpuLevel(value);
		}
	}

	public static int gpuLevel
	{
		get
		{
			return OVRP_1_1_0.ovrp_GetSystemGpuLevel();
		}
		set
		{
			OVRP_1_1_0.ovrp_SetSystemGpuLevel(value);
		}
	}

	public static int vsyncCount
	{
		get
		{
			return OVRP_1_1_0.ovrp_GetSystemVSyncCount();
		}
		set
		{
			OVRP_1_2_0.ovrp_SetSystemVSyncCount(value);
		}
	}

	public static float systemVolume
	{
		get
		{
			return OVRP_1_1_0.ovrp_GetSystemVolume();
		}
	}

	public static float ipd
	{
		get
		{
			return OVRP_1_1_0.ovrp_GetUserIPD();
		}
		set
		{
			OVRP_1_1_0.ovrp_SetUserIPD(value);
		}
	}

	public static bool occlusionMesh
	{
		get
		{
			return OVRP_1_3_0.ovrp_GetEyeOcclusionMeshEnabled() == Bool.True;
		}
		set
		{
			OVRP_1_3_0.ovrp_SetEyeOcclusionMeshEnabled(ToBool(value));
		}
	}

	public static BatteryStatus batteryStatus
	{
		get
		{
			return OVRP_1_1_0.ovrp_GetSystemBatteryStatus();
		}
	}

	internal static bool ignoreVrFocus
	{
		set
		{
			OVRP_1_2_1.ovrp_SetAppIgnoreVrFocus(ToBool(value));
		}
	}

	public static Posef GetEyeVelocity(Eye eyeId)
	{
		return GetNodeVelocity((Node)eyeId, false);
	}

	public static Posef GetEyeAcceleration(Eye eyeId)
	{
		return GetNodeAcceleration((Node)eyeId, false);
	}

	public static Frustumf GetEyeFrustum(Eye eyeId)
	{
		return OVRP_1_1_0.ovrp_GetNodeFrustum((Node)eyeId);
	}

	public static Sizei GetEyeTextureSize(Eye eyeId)
	{
		return OVRP_0_1_0.ovrp_GetEyeTextureSize(eyeId);
	}

	public static Posef GetTrackerPose(Tracker trackerId)
	{
		return GetNodePose((Node)(trackerId + 5), false);
	}

	public static Frustumf GetTrackerFrustum(Tracker trackerId)
	{
		return OVRP_1_1_0.ovrp_GetNodeFrustum((Node)(trackerId + 5));
	}

	public static bool ShowUI(PlatformUI ui)
	{
		return OVRP_1_1_0.ovrp_ShowSystemUI(ui) == Bool.True;
	}

	public static bool SetOverlayQuad(bool onTop, bool headLocked, IntPtr leftTexture, IntPtr rightTexture, IntPtr device, Posef pose, Vector3f scale, int layerIndex = 0, OverlayShape shape = OverlayShape.Quad)
	{
		if (version >= OVRP_1_6_0.version)
		{
			uint num = 0u;
			if (onTop)
			{
				num |= 1u;
			}
			if (headLocked)
			{
				num |= 2u;
			}
			if (shape == OverlayShape.Cylinder || shape == OverlayShape.Cubemap)
			{
				return false;
			}
			return OVRP_1_6_0.ovrp_SetOverlayQuad3(num, leftTexture, rightTexture, device, pose, scale, layerIndex) == Bool.True;
		}
		if (layerIndex != 0)
		{
			return false;
		}
		return OVRP_0_1_1.ovrp_SetOverlayQuad2(ToBool(onTop), ToBool(headLocked), leftTexture, device, pose, scale) == Bool.True;
	}

	public static bool UpdateNodePhysicsPoses(int frameIndex, double predictionSeconds)
	{
		if (version >= OVRP_1_8_0.version)
		{
			return OVRP_1_8_0.ovrp_Update2(0, frameIndex, predictionSeconds) == Bool.True;
		}
		return false;
	}

	public static Posef GetNodePose(Node nodeId, bool usePhysicsPose)
	{
		if (version >= OVRP_1_8_0.version && usePhysicsPose)
		{
			return OVRP_1_8_0.ovrp_GetNodePose2(0, nodeId);
		}
		return OVRP_0_1_2.ovrp_GetNodePose(nodeId);
	}

	public static Posef GetNodeVelocity(Node nodeId, bool usePhysicsPose)
	{
		if (version >= OVRP_1_8_0.version && usePhysicsPose)
		{
			return OVRP_1_8_0.ovrp_GetNodeVelocity2(0, nodeId);
		}
		return OVRP_0_1_3.ovrp_GetNodeVelocity(nodeId);
	}

	public static Posef GetNodeAcceleration(Node nodeId, bool usePhysicsPose)
	{
		if (version >= OVRP_1_8_0.version && usePhysicsPose)
		{
			return OVRP_1_8_0.ovrp_GetNodeAcceleration2(0, nodeId);
		}
		return OVRP_0_1_3.ovrp_GetNodeAcceleration(nodeId);
	}

	public static bool GetNodePresent(Node nodeId)
	{
		return OVRP_1_1_0.ovrp_GetNodePresent(nodeId) == Bool.True;
	}

	public static bool GetNodeOrientationTracked(Node nodeId)
	{
		return OVRP_1_1_0.ovrp_GetNodeOrientationTracked(nodeId) == Bool.True;
	}

	public static bool GetNodePositionTracked(Node nodeId)
	{
		return OVRP_1_1_0.ovrp_GetNodePositionTracked(nodeId) == Bool.True;
	}

	public static ControllerState GetControllerState(uint controllerMask)
	{
		return OVRP_1_1_0.ovrp_GetControllerState(controllerMask);
	}

	public static bool SetControllerVibration(uint controllerMask, float frequency, float amplitude)
	{
		return OVRP_0_1_2.ovrp_SetControllerVibration(controllerMask, frequency, amplitude) == Bool.True;
	}

	public static HapticsDesc GetControllerHapticsDesc(uint controllerMask)
	{
		if (version >= OVRP_1_6_0.version)
		{
			return OVRP_1_6_0.ovrp_GetControllerHapticsDesc(controllerMask);
		}
		return default(HapticsDesc);
	}

	public static HapticsState GetControllerHapticsState(uint controllerMask)
	{
		if (version >= OVRP_1_6_0.version)
		{
			return OVRP_1_6_0.ovrp_GetControllerHapticsState(controllerMask);
		}
		return default(HapticsState);
	}

	public static bool SetControllerHaptics(uint controllerMask, HapticsBuffer hapticsBuffer)
	{
		if (version >= OVRP_1_6_0.version)
		{
			return OVRP_1_6_0.ovrp_SetControllerHaptics(controllerMask, hapticsBuffer) == Bool.True;
		}
		return false;
	}

	public static float GetEyeRecommendedResolutionScale()
	{
		if (version >= OVRP_1_6_0.version)
		{
			return OVRP_1_6_0.ovrp_GetEyeRecommendedResolutionScale();
		}
		return 1f;
	}

	public static float GetAppCpuStartToGpuEndTime()
	{
		if (version >= OVRP_1_6_0.version)
		{
			return OVRP_1_6_0.ovrp_GetAppCpuStartToGpuEndTime();
		}
		return 0f;
	}

	public static bool GetBoundaryConfigured()
	{
		if (version >= OVRP_1_8_0.version)
		{
			return OVRP_1_8_0.ovrp_GetBoundaryConfigured() == Bool.True;
		}
		return false;
	}

	public static BoundaryTestResult TestBoundaryNode(Node nodeId, BoundaryType boundaryType)
	{
		if (version >= OVRP_1_8_0.version)
		{
			return OVRP_1_8_0.ovrp_TestBoundaryNode(nodeId, boundaryType);
		}
		return default(BoundaryTestResult);
	}

	public static BoundaryTestResult TestBoundaryPoint(Vector3f point, BoundaryType boundaryType)
	{
		if (version >= OVRP_1_8_0.version)
		{
			return OVRP_1_8_0.ovrp_TestBoundaryPoint(point, boundaryType);
		}
		return default(BoundaryTestResult);
	}

	public static bool SetBoundaryLookAndFeel(BoundaryLookAndFeel lookAndFeel)
	{
		if (version >= OVRP_1_8_0.version)
		{
			return OVRP_1_8_0.ovrp_SetBoundaryLookAndFeel(lookAndFeel) == Bool.True;
		}
		return false;
	}

	public static bool ResetBoundaryLookAndFeel()
	{
		if (version >= OVRP_1_8_0.version)
		{
			return OVRP_1_8_0.ovrp_ResetBoundaryLookAndFeel() == Bool.True;
		}
		return false;
	}

	public static BoundaryGeometry GetBoundaryGeometry(BoundaryType boundaryType)
	{
		if (version >= OVRP_1_8_0.version)
		{
			return OVRP_1_8_0.ovrp_GetBoundaryGeometry(boundaryType);
		}
		return default(BoundaryGeometry);
	}

	public static Vector3f GetBoundaryDimensions(BoundaryType boundaryType)
	{
		if (version >= OVRP_1_8_0.version)
		{
			return OVRP_1_8_0.ovrp_GetBoundaryDimensions(boundaryType);
		}
		return default(Vector3f);
	}

	public static bool GetBoundaryVisible()
	{
		if (version >= OVRP_1_8_0.version)
		{
			return OVRP_1_8_0.ovrp_GetBoundaryVisible() == Bool.True;
		}
		return false;
	}

	public static bool SetBoundaryVisible(bool value)
	{
		if (version >= OVRP_1_8_0.version)
		{
			return OVRP_1_8_0.ovrp_SetBoundaryVisible(ToBool(value)) == Bool.True;
		}
		return false;
	}

	private static Bool ToBool(bool b)
	{
		return b ? Bool.True : Bool.False;
	}

	public static TrackingOrigin GetTrackingOriginType()
	{
		return OVRP_1_0_0.ovrp_GetTrackingOriginType();
	}

	public static bool SetTrackingOriginType(TrackingOrigin originType)
	{
		return OVRP_1_0_0.ovrp_SetTrackingOriginType(originType) == Bool.True;
	}

	public static Posef GetTrackingCalibratedOrigin()
	{
		return OVRP_1_0_0.ovrp_GetTrackingCalibratedOrigin();
	}

	public static bool SetTrackingCalibratedOrigin()
	{
		return OVRP_1_2_0.ovrpi_SetTrackingCalibratedOrigin() == Bool.True;
	}

	public static bool RecenterTrackingOrigin(RecenterFlags flags)
	{
		return OVRP_1_0_0.ovrp_RecenterTrackingOrigin((uint)flags) == Bool.True;
	}
}
