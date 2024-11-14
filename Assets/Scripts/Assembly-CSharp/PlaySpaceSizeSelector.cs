using System;
using System.Collections;
using OwlchemyVR;
using PSC;
using UnityEngine;
using Valve.VR;

public class PlaySpaceSizeSelector : MonoBehaviour
{
	private const int FRAMES_TO_WAIT_ON_OVR_SDK = 5;

	[SerializeField]
	private LayoutConfiguration[] steamVRLayouts;

	[SerializeField]
	private LayoutConfiguration[] oculusLayouts;

	[SerializeField]
	private LayoutConfiguration oculusLayoutNoGuardian;

	[SerializeField]
	private LayoutConfiguration psvrLayout;

	public static bool isComplete;

	private void Awake()
	{
		if (Room.defaultLayoutToLoad == null)
		{
			Debug.Log("Determine play space size selection");
			if (VRPlatform.GetCurrVRPlatformType() == VRPlatformTypes.Oculus)
			{
				Debug.Log("Oculus");
				StartCoroutine(DetermineOculusPlaySpace());
			}
			else if (VRPlatform.GetCurrVRPlatformType() == VRPlatformTypes.SteamVR)
			{
				Debug.Log("SteamVR");
				StartCoroutine(DetermineSteamVRPlaySpace());
			}
			else if (VRPlatform.GetCurrVRPlatformType() == VRPlatformTypes.PSVR)
			{
				StartCoroutine(DeterminePSVRPlaySpace());
			}
			else
			{
				Debug.LogError("unsupported playspace resizer! Loading default.");
				Room.defaultLayoutToLoad = oculusLayoutNoGuardian;
				isComplete = true;
			}
		}
		else
		{
			Debug.LogError("Room.defaultLayoutToLoad was null");
		}
	}

	private IEnumerator DeterminePSVRPlaySpace()
	{
		yield return null;
		Room.defaultLayoutToLoad = psvrLayout;
		isComplete = true;
	}

	private IEnumerator DetermineOculusPlaySpace()
	{
		Debug.Log("DetermineOculusPlaySpace");
		yield return null;
		OVRBoundary boundary = OVRManager.boundary;
		int i = 5;
		while (i > 0)
		{
			if (OVRManager.boundary == null || !OVRManager.boundary.GetConfigured())
			{
				yield return null;
				i--;
			}
			else
			{
				i = 0;
			}
		}
		boundary = OVRManager.boundary;
		if (boundary == null)
		{
			Debug.Log("Oculus SDK reports guardian bounds as NOT SET UP! Setting playspace to default small size.");
			Room.defaultLayoutToLoad = oculusLayoutNoGuardian;
		}
		else
		{
			Vector3 playArea = boundary.GetDimensions(OVRBoundary.BoundaryType.PlayArea);
			string measuredRoomSizeString2 = "No size detected";
			float xSize = playArea.x;
			float zSize = playArea.z;
			measuredRoomSizeString2 = string.Format("{0}x{1}", xSize, zSize);
			Debug.Log("Oculus SDK reports guardian bounds as: " + measuredRoomSizeString2);
			LayoutConfiguration largestPossiblyLayout2;
			if (VRPlatform.IsRoomScale)
			{
				Debug.Log("doing room scale for oculus!");
				largestPossiblyLayout2 = GetLargestLayoutBasedOnLargestXSize(steamVRLayouts, xSize, zSize);
			}
			else
			{
				largestPossiblyLayout2 = GetLargestLayoutBasedOnLargestXSize(oculusLayouts, xSize, zSize);
			}
			if (largestPossiblyLayout2 != null)
			{
				Debug.Log("Set Layout:" + largestPossiblyLayout2.sizeInMeters);
				Room.defaultLayoutToLoad = largestPossiblyLayout2;
			}
			else
			{
				Debug.LogWarning("Unable to find largest possible layout, attempting to set based on smallest X size");
				largestPossiblyLayout2 = ((!VRPlatform.IsRoomScale) ? GetSmallestLayoutBasedonSmallestXSize(oculusLayouts) : GetSmallestLayoutBasedonSmallestXSize(steamVRLayouts));
				if (largestPossiblyLayout2 == null)
				{
					Debug.LogError("Layout set to dfeault because no layout was available");
				}
				else
				{
					Debug.Log("Set Layout:" + largestPossiblyLayout2.sizeInMeters);
					Room.defaultLayoutToLoad = largestPossiblyLayout2;
				}
			}
			AnalyticsManager.CustomEvent("Measured Room Size", "Size", measuredRoomSizeString2);
			string roomSizeString = string.Format("{0}x{1}", Room.defaultLayoutToLoad.sizeInMeters.x, Room.defaultLayoutToLoad.sizeInMeters.y);
			AnalyticsManager.CustomEvent("Room Size", "Size", roomSizeString);
		}
		isComplete = true;
	}

	private IEnumerator DetermineSteamVRPlaySpace()
	{
		Debug.Log("DetermineSteamVRPlaySpace");
		CVRChaperone chaperone = OpenVR.Chaperone;
		while (chaperone == null)
		{
			chaperone = OpenVR.Chaperone;
			yield return null;
		}
		while (chaperone.GetCalibrationState() != ChaperoneCalibrationState.OK)
		{
			yield return null;
		}
		string measuredRoomSizeString = "No size detected";
		float xSize = -1f;
		float ySize = -1f;
		if (GetBoundsArea(ref xSize, ref ySize))
		{
			measuredRoomSizeString = string.Format("{0}x{1}", xSize, ySize);
			if (VRPlatform.IsRoomScale)
			{
				Debug.Log("Roomscale mode is on!");
			}
			if (VRPlatform.GetCurrVRPlatformHardwareType() == VRPlatformHardwareType.OculusRift)
			{
				Debug.Log("Using Oculus hardware with SteamVR");
			}
			LayoutConfiguration largestPossiblyLayout2 = ((VRPlatform.GetCurrVRPlatformHardwareType() != VRPlatformHardwareType.OculusRift || VRPlatform.IsRoomScale) ? GetLargestLayoutBasedOnLargestXSize(steamVRLayouts, xSize, ySize) : GetLargestLayoutBasedOnLargestXSize(oculusLayouts, xSize, ySize));
			if (largestPossiblyLayout2 != null)
			{
				Debug.Log("Set Layout:" + largestPossiblyLayout2.sizeInMeters);
				Room.defaultLayoutToLoad = largestPossiblyLayout2;
			}
			else
			{
				Debug.LogError("Unable to find largest possible layout");
				largestPossiblyLayout2 = GetSmallestLayoutBasedonSmallestXSize(steamVRLayouts);
				Debug.LogError("Layout set to dfeault because no layout was available");
				if (VRPlatform.GetCurrVRPlatformHardwareType() == VRPlatformHardwareType.OculusRift && !VRPlatform.IsRoomScale)
				{
					Room.defaultLayoutToLoad = GetSmallestLayoutBasedonSmallestXSize(oculusLayouts);
				}
				else
				{
					Room.defaultLayoutToLoad = GetSmallestLayoutBasedonSmallestXSize(steamVRLayouts);
				}
			}
		}
		else
		{
			Debug.LogError("Unable to get calibration bounds of playspace SteamVR, using default: smallest space");
			Room.defaultLayoutToLoad = GetSmallestLayoutBasedonSmallestXSize(steamVRLayouts);
		}
		AnalyticsManager.CustomEvent("Measured Room Size", "Size", measuredRoomSizeString);
		string roomSizeString = string.Format("{0}x{1}", Room.defaultLayoutToLoad.sizeInMeters.x, Room.defaultLayoutToLoad.sizeInMeters.y);
		AnalyticsManager.CustomEvent("Room Size", "Size", roomSizeString);
		isComplete = true;
	}

	private LayoutConfiguration GetSmallestLayoutBasedonSmallestXSize(LayoutConfiguration[] list)
	{
		float num = float.PositiveInfinity;
		float num2 = float.PositiveInfinity;
		LayoutConfiguration result = null;
		if (list != null)
		{
			foreach (LayoutConfiguration layoutConfiguration in list)
			{
				if (layoutConfiguration.sizeInMeters.x < num || (layoutConfiguration.sizeInMeters.x == num && layoutConfiguration.sizeInMeters.y < num2))
				{
					num = layoutConfiguration.sizeInMeters.x;
					num2 = layoutConfiguration.sizeInMeters.y;
					result = layoutConfiguration;
				}
			}
		}
		return result;
	}

	private LayoutConfiguration GetLargestLayoutBasedOnLargestXSize(LayoutConfiguration[] list, float xSize, float ySize)
	{
		float num = -1f;
		float num2 = -1f;
		LayoutConfiguration layoutConfiguration = null;
		LayoutConfiguration layoutConfiguration2 = null;
		if (list != null)
		{
			foreach (LayoutConfiguration layoutConfiguration3 in list)
			{
				if ((layoutConfiguration3.sizeInMeters.x > num || (layoutConfiguration3.sizeInMeters.x == num && layoutConfiguration3.sizeInMeters.y > num2)) && xSize >= layoutConfiguration3.sizeInMeters.x && ySize >= layoutConfiguration3.sizeInMeters.y)
				{
					num = layoutConfiguration3.sizeInMeters.x;
					num2 = layoutConfiguration3.sizeInMeters.y;
					layoutConfiguration2 = layoutConfiguration;
					layoutConfiguration = layoutConfiguration3;
				}
			}
		}
		if (layoutConfiguration == null)
		{
			return null;
		}
		if (VRPlatform.GetCurrVRPlatformHardwareType() == VRPlatformHardwareType.WindowsMR && layoutConfiguration.sizeInMeters.x == 3f)
		{
			layoutConfiguration = layoutConfiguration2;
		}
		return layoutConfiguration;
	}

	public static bool GetBoundsArea(ref float xSize, ref float ySize)
	{
		bool flag = !SteamVR.active && !SteamVR.usingNativeSupport;
		if (flag)
		{
			EVRInitError peError = EVRInitError.None;
			OpenVR.Init(ref peError, EVRApplicationType.VRApplication_Other);
		}
		CVRChaperone chaperone = OpenVR.Chaperone;
		bool flag2 = chaperone != null && chaperone.GetPlayAreaSize(ref xSize, ref ySize);
		if (flag2)
		{
			Debug.Log("Before rounding - XSize: " + xSize + ", YSize: " + ySize);
			xSize = (float)Math.Round(xSize, 1);
			ySize = (float)Math.Round(ySize, 1);
			Debug.Log("After rounding - XSize: " + xSize + ", YSize: " + ySize);
		}
		else
		{
			Debug.LogWarning("Failed to get Calibrated Play Area bounds!  Make sure you have tracking first, and that your space is calibrated.");
		}
		if (flag)
		{
			OpenVR.Shutdown();
		}
		return flag2;
	}
}
