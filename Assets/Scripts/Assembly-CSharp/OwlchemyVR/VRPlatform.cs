using UnityEngine;
using Valve.VR;

namespace OwlchemyVR
{
	public class VRPlatform
	{
		public const string STEAM_VR_DEFINE = "STEAMVR";

		public const string OCULUS_DEFINE = "OCULUS";

		public const string MORPHEUS_DEFINE = "MORPHEUS";

		public const string DAYDREAM_DEFINE = "DAYDREAM";

		public const string STEAMVR_OCULUSCV1_MODEL_NAME = "Oculus Rift CV1";

		public const string STEAMVR_OCULUSDK2_MODEL_NAME = "Oculus Rift DK2";

		public const string STEAMVR_VIVE_MODEL_NAME = "Vive MV";

		public const string STEAMVR_VIVE_PRE_MODEL_NAME = "Vive DVT";

		public const string STEAMVR_WINDOWSMR_MODEL_NAME = "TrackedProp_UnknownProperty";

		public const string STEAMVR_WINDOWSMR_SERIAL_NUM = "WindowsHolographic";

		private const VRPlatformTypes DEFAULT_VR_PLATFORM = VRPlatformTypes.SteamVR;

		private const VRPlatformHardwareType DEFAULT_VR_HARDWARE = VRPlatformHardwareType.SteamVRCompatible;

		private const int MIN_OCULUS_CAMERAS_FOR_ROOMSCALE = 3;

		private const string PREF_KEY_VR_PLATFORM_TYPE = "VRPlatformType";

		private const string PREF_KEY_VR_PLATFORM_HARDWARE_TYPE = "VRPlatformHardwareType";

		private static VRPlatformTypes currVRPlatformType;

		private static VRPlatformHardwareType currVRPlatformHardwareType;

		public static bool IsRoomScale
		{
			get
			{
				return GetIsRoomScale();
			}
		}

		public static bool IsLowPerformancePlatform
		{
			get
			{
				return currVRPlatformType == VRPlatformTypes.PSVR || currVRPlatformType == VRPlatformTypes.Daydream;
			}
		}

		private static bool GetIsRoomScale()
		{
			bool result = true;
			if (GetCurrVRPlatformHardwareType() == VRPlatformHardwareType.OculusRift)
			{
				if (GetCurrVRPlatformType() == VRPlatformTypes.Oculus)
				{
					result = ((OVRManager.tracker.count >= 3) ? true : false);
				}
				else if (GetCurrVRPlatformType() == VRPlatformTypes.SteamVR)
				{
					int num = 0;
					for (uint num2 = 0u; num2 < 16; num2++)
					{
						if (OpenVR.System.GetTrackedDeviceClass(num2) == ETrackedDeviceClass.TrackingReference)
						{
							num++;
						}
					}
					result = ((num >= 3) ? true : false);
				}
				else if (GetCurrVRPlatformType() == VRPlatformTypes.Daydream)
				{
					result = true;
				}
			}
			else if (GetCurrVRPlatformHardwareType() == VRPlatformHardwareType.PSVR)
			{
				result = false;
			}
			return result;
		}

		public static string GetDefineStringFromVRPlatform(VRPlatformTypes vrPlatformType)
		{
			switch (vrPlatformType)
			{
			case VRPlatformTypes.SteamVR:
				return "STEAMVR";
			case VRPlatformTypes.Oculus:
				return "OCULUS";
			case VRPlatformTypes.PSVR:
				return "MORPHEUS";
			case VRPlatformTypes.Daydream:
				return "DAYDREAM";
			default:
				return string.Empty;
			}
		}

		public static void SetPlatformAndHardware(VRPlatformTypes vrPlatformType, VRPlatformHardwareType hardwareType)
		{
			currVRPlatformType = vrPlatformType;
			currVRPlatformHardwareType = hardwareType;
			PlayerPrefs.SetInt("VRPlatformType", (int)currVRPlatformType);
			PlayerPrefs.SetInt("VRPlatformHardwareType", (int)currVRPlatformHardwareType);
			PlayerPrefs.Save();
		}

		public static VRPlatformTypes GetCurrVRPlatformType()
		{
			if (currVRPlatformType == VRPlatformTypes.None)
			{
				if (Application.isEditor)
				{
					currVRPlatformType = (VRPlatformTypes)PlayerPrefs.GetInt("VRPlatformType", 0);
					if (currVRPlatformType == VRPlatformTypes.None)
					{
						currVRPlatformType = VRPlatformTypes.SteamVR;
					}
				}
				else
				{
					currVRPlatformType = VRPlatformTypes.SteamVR;
					if (currVRPlatformType == VRPlatformTypes.None)
					{
						Debug.LogError("Correct VR Platform was not yet defined!!");
						currVRPlatformType = VRPlatformTypes.SteamVR;
					}
				}
			}
			return currVRPlatformType;
		}

		public static VRPlatformHardwareType GetCurrVRPlatformHardwareType()
		{
			if (currVRPlatformHardwareType == VRPlatformHardwareType.None)
			{
				if (Application.isEditor)
				{
					currVRPlatformHardwareType = (VRPlatformHardwareType)PlayerPrefs.GetInt("VRPlatformHardwareType", 0);
					if (currVRPlatformHardwareType == VRPlatformHardwareType.None)
					{
						currVRPlatformHardwareType = VRPlatformHardwareType.SteamVRCompatible;
					}
				}
				else
				{
					switch (GetCurrVRPlatformType())
					{
					case VRPlatformTypes.SteamVR:
					{
						SteamVR instance = SteamVR.instance;
						if (instance.hmd_SerialNumber == "WindowsHolographic")
						{
							currVRPlatformHardwareType = VRPlatformHardwareType.WindowsMR;
							break;
						}
						if (instance.hmd_ModelNumber.Contains("Oculus Rift ES"))
						{
							currVRPlatformHardwareType = VRPlatformHardwareType.OculusRift;
							break;
						}
						switch (instance.hmd_ModelNumber)
						{
						case "Oculus Rift CV1":
							currVRPlatformHardwareType = VRPlatformHardwareType.OculusRift;
							break;
						case "Vive MV":
							currVRPlatformHardwareType = VRPlatformHardwareType.Vive;
							break;
						case "Vive DVT":
							currVRPlatformHardwareType = VRPlatformHardwareType.Vive;
							break;
						default:
							currVRPlatformHardwareType = VRPlatformHardwareType.SteamVRCompatible;
							break;
						}
						break;
					}
					case VRPlatformTypes.Oculus:
						currVRPlatformHardwareType = VRPlatformHardwareType.OculusRift;
						break;
					case VRPlatformTypes.PSVR:
						currVRPlatformHardwareType = VRPlatformHardwareType.PSVR;
						break;
					case VRPlatformTypes.Daydream:
						currVRPlatformHardwareType = VRPlatformHardwareType.DaydreamView;
						break;
					default:
						currVRPlatformHardwareType = VRPlatformHardwareType.SteamVRCompatible;
						break;
					}
				}
			}
			return currVRPlatformHardwareType;
		}

		public static string GetCoreLoaderNameForCurrConfig()
		{
			string empty = string.Empty;
			if (currVRPlatformType == VRPlatformTypes.PSVR)
			{
				return "Morpheus";
			}
			if (currVRPlatformType == VRPlatformTypes.Oculus && currVRPlatformHardwareType == VRPlatformHardwareType.OculusRift)
			{
				return "Oculus";
			}
			if (currVRPlatformType == VRPlatformTypes.SteamVR)
			{
				return "SteamVR";
			}
			if (currVRPlatformType == VRPlatformTypes.Daydream)
			{
				return "Daydream";
			}
			return "Daydream";
		}
	}
}
