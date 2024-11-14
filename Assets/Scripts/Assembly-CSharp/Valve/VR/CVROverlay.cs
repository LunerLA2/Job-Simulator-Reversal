using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Valve.VR
{
	public class CVROverlay
	{
		private IVROverlay FnTable;

		internal CVROverlay(IntPtr pInterface)
		{
			FnTable = (IVROverlay)Marshal.PtrToStructure(pInterface, typeof(IVROverlay));
		}

		public EVROverlayError FindOverlay(string pchOverlayKey, ref ulong pOverlayHandle)
		{
			pOverlayHandle = 0uL;
			return FnTable.FindOverlay(pchOverlayKey, ref pOverlayHandle);
		}

		public EVROverlayError CreateOverlay(string pchOverlayKey, string pchOverlayFriendlyName, ref ulong pOverlayHandle)
		{
			pOverlayHandle = 0uL;
			return FnTable.CreateOverlay(pchOverlayKey, pchOverlayFriendlyName, ref pOverlayHandle);
		}

		public EVROverlayError DestroyOverlay(ulong ulOverlayHandle)
		{
			return FnTable.DestroyOverlay(ulOverlayHandle);
		}

		public EVROverlayError SetHighQualityOverlay(ulong ulOverlayHandle)
		{
			return FnTable.SetHighQualityOverlay(ulOverlayHandle);
		}

		public ulong GetHighQualityOverlay()
		{
			return FnTable.GetHighQualityOverlay();
		}

		public uint GetOverlayKey(ulong ulOverlayHandle, StringBuilder pchValue, uint unBufferSize, ref EVROverlayError pError)
		{
			return FnTable.GetOverlayKey(ulOverlayHandle, pchValue, unBufferSize, ref pError);
		}

		public uint GetOverlayName(ulong ulOverlayHandle, StringBuilder pchValue, uint unBufferSize, ref EVROverlayError pError)
		{
			return FnTable.GetOverlayName(ulOverlayHandle, pchValue, unBufferSize, ref pError);
		}

		public EVROverlayError GetOverlayImageData(ulong ulOverlayHandle, IntPtr pvBuffer, uint unBufferSize, ref uint punWidth, ref uint punHeight)
		{
			punWidth = 0u;
			punHeight = 0u;
			return FnTable.GetOverlayImageData(ulOverlayHandle, pvBuffer, unBufferSize, ref punWidth, ref punHeight);
		}

		public string GetOverlayErrorNameFromEnum(EVROverlayError error)
		{
			IntPtr ptr = FnTable.GetOverlayErrorNameFromEnum(error);
			return (string)Marshal.PtrToStructure(ptr, typeof(string));
		}

		public EVROverlayError SetOverlayFlag(ulong ulOverlayHandle, VROverlayFlags eOverlayFlag, bool bEnabled)
		{
			return FnTable.SetOverlayFlag(ulOverlayHandle, eOverlayFlag, bEnabled);
		}

		public EVROverlayError GetOverlayFlag(ulong ulOverlayHandle, VROverlayFlags eOverlayFlag, ref bool pbEnabled)
		{
			pbEnabled = false;
			return FnTable.GetOverlayFlag(ulOverlayHandle, eOverlayFlag, ref pbEnabled);
		}

		public EVROverlayError SetOverlayColor(ulong ulOverlayHandle, float fRed, float fGreen, float fBlue)
		{
			return FnTable.SetOverlayColor(ulOverlayHandle, fRed, fGreen, fBlue);
		}

		public EVROverlayError GetOverlayColor(ulong ulOverlayHandle, ref float pfRed, ref float pfGreen, ref float pfBlue)
		{
			pfRed = 0f;
			pfGreen = 0f;
			pfBlue = 0f;
			return FnTable.GetOverlayColor(ulOverlayHandle, ref pfRed, ref pfGreen, ref pfBlue);
		}

		public EVROverlayError SetOverlayAlpha(ulong ulOverlayHandle, float fAlpha)
		{
			return FnTable.SetOverlayAlpha(ulOverlayHandle, fAlpha);
		}

		public EVROverlayError GetOverlayAlpha(ulong ulOverlayHandle, ref float pfAlpha)
		{
			pfAlpha = 0f;
			return FnTable.GetOverlayAlpha(ulOverlayHandle, ref pfAlpha);
		}

		public EVROverlayError SetOverlayWidthInMeters(ulong ulOverlayHandle, float fWidthInMeters)
		{
			return FnTable.SetOverlayWidthInMeters(ulOverlayHandle, fWidthInMeters);
		}

		public EVROverlayError GetOverlayWidthInMeters(ulong ulOverlayHandle, ref float pfWidthInMeters)
		{
			pfWidthInMeters = 0f;
			return FnTable.GetOverlayWidthInMeters(ulOverlayHandle, ref pfWidthInMeters);
		}

		public EVROverlayError SetOverlayAutoCurveDistanceRangeInMeters(ulong ulOverlayHandle, float fMinDistanceInMeters, float fMaxDistanceInMeters)
		{
			return FnTable.SetOverlayAutoCurveDistanceRangeInMeters(ulOverlayHandle, fMinDistanceInMeters, fMaxDistanceInMeters);
		}

		public EVROverlayError GetOverlayAutoCurveDistanceRangeInMeters(ulong ulOverlayHandle, ref float pfMinDistanceInMeters, ref float pfMaxDistanceInMeters)
		{
			pfMinDistanceInMeters = 0f;
			pfMaxDistanceInMeters = 0f;
			return FnTable.GetOverlayAutoCurveDistanceRangeInMeters(ulOverlayHandle, ref pfMinDistanceInMeters, ref pfMaxDistanceInMeters);
		}

		public EVROverlayError SetOverlayTextureColorSpace(ulong ulOverlayHandle, EColorSpace eTextureColorSpace)
		{
			return FnTable.SetOverlayTextureColorSpace(ulOverlayHandle, eTextureColorSpace);
		}

		public EVROverlayError GetOverlayTextureColorSpace(ulong ulOverlayHandle, ref EColorSpace peTextureColorSpace)
		{
			return FnTable.GetOverlayTextureColorSpace(ulOverlayHandle, ref peTextureColorSpace);
		}

		public EVROverlayError SetOverlayTextureBounds(ulong ulOverlayHandle, ref VRTextureBounds_t pOverlayTextureBounds)
		{
			return FnTable.SetOverlayTextureBounds(ulOverlayHandle, ref pOverlayTextureBounds);
		}

		public EVROverlayError GetOverlayTextureBounds(ulong ulOverlayHandle, ref VRTextureBounds_t pOverlayTextureBounds)
		{
			return FnTable.GetOverlayTextureBounds(ulOverlayHandle, ref pOverlayTextureBounds);
		}

		public EVROverlayError GetOverlayTransformType(ulong ulOverlayHandle, ref VROverlayTransformType peTransformType)
		{
			return FnTable.GetOverlayTransformType(ulOverlayHandle, ref peTransformType);
		}

		public EVROverlayError SetOverlayTransformAbsolute(ulong ulOverlayHandle, ETrackingUniverseOrigin eTrackingOrigin, ref HmdMatrix34_t pmatTrackingOriginToOverlayTransform)
		{
			return FnTable.SetOverlayTransformAbsolute(ulOverlayHandle, eTrackingOrigin, ref pmatTrackingOriginToOverlayTransform);
		}

		public EVROverlayError GetOverlayTransformAbsolute(ulong ulOverlayHandle, ref ETrackingUniverseOrigin peTrackingOrigin, ref HmdMatrix34_t pmatTrackingOriginToOverlayTransform)
		{
			return FnTable.GetOverlayTransformAbsolute(ulOverlayHandle, ref peTrackingOrigin, ref pmatTrackingOriginToOverlayTransform);
		}

		public EVROverlayError SetOverlayTransformTrackedDeviceRelative(ulong ulOverlayHandle, uint unTrackedDevice, ref HmdMatrix34_t pmatTrackedDeviceToOverlayTransform)
		{
			return FnTable.SetOverlayTransformTrackedDeviceRelative(ulOverlayHandle, unTrackedDevice, ref pmatTrackedDeviceToOverlayTransform);
		}

		public EVROverlayError GetOverlayTransformTrackedDeviceRelative(ulong ulOverlayHandle, ref uint punTrackedDevice, ref HmdMatrix34_t pmatTrackedDeviceToOverlayTransform)
		{
			punTrackedDevice = 0u;
			return FnTable.GetOverlayTransformTrackedDeviceRelative(ulOverlayHandle, ref punTrackedDevice, ref pmatTrackedDeviceToOverlayTransform);
		}

		public EVROverlayError SetOverlayTransformTrackedDeviceComponent(ulong ulOverlayHandle, uint unDeviceIndex, string pchComponentName)
		{
			return FnTable.SetOverlayTransformTrackedDeviceComponent(ulOverlayHandle, unDeviceIndex, pchComponentName);
		}

		public EVROverlayError GetOverlayTransformTrackedDeviceComponent(ulong ulOverlayHandle, ref uint punDeviceIndex, string pchComponentName, uint unComponentNameSize)
		{
			punDeviceIndex = 0u;
			return FnTable.GetOverlayTransformTrackedDeviceComponent(ulOverlayHandle, ref punDeviceIndex, pchComponentName, unComponentNameSize);
		}

		public EVROverlayError ShowOverlay(ulong ulOverlayHandle)
		{
			return FnTable.ShowOverlay(ulOverlayHandle);
		}

		public EVROverlayError HideOverlay(ulong ulOverlayHandle)
		{
			return FnTable.HideOverlay(ulOverlayHandle);
		}

		public bool IsOverlayVisible(ulong ulOverlayHandle)
		{
			return FnTable.IsOverlayVisible(ulOverlayHandle);
		}

		public EVROverlayError GetTransformForOverlayCoordinates(ulong ulOverlayHandle, ETrackingUniverseOrigin eTrackingOrigin, HmdVector2_t coordinatesInOverlay, ref HmdMatrix34_t pmatTransform)
		{
			return FnTable.GetTransformForOverlayCoordinates(ulOverlayHandle, eTrackingOrigin, coordinatesInOverlay, ref pmatTransform);
		}

		public bool PollNextOverlayEvent(ulong ulOverlayHandle, ref VREvent_t pEvent, uint uncbVREvent)
		{
			return FnTable.PollNextOverlayEvent(ulOverlayHandle, ref pEvent, uncbVREvent);
		}

		public EVROverlayError GetOverlayInputMethod(ulong ulOverlayHandle, ref VROverlayInputMethod peInputMethod)
		{
			return FnTable.GetOverlayInputMethod(ulOverlayHandle, ref peInputMethod);
		}

		public EVROverlayError SetOverlayInputMethod(ulong ulOverlayHandle, VROverlayInputMethod eInputMethod)
		{
			return FnTable.SetOverlayInputMethod(ulOverlayHandle, eInputMethod);
		}

		public EVROverlayError GetOverlayMouseScale(ulong ulOverlayHandle, ref HmdVector2_t pvecMouseScale)
		{
			return FnTable.GetOverlayMouseScale(ulOverlayHandle, ref pvecMouseScale);
		}

		public EVROverlayError SetOverlayMouseScale(ulong ulOverlayHandle, ref HmdVector2_t pvecMouseScale)
		{
			return FnTable.SetOverlayMouseScale(ulOverlayHandle, ref pvecMouseScale);
		}

		public bool ComputeOverlayIntersection(ulong ulOverlayHandle, ref VROverlayIntersectionParams_t pParams, ref VROverlayIntersectionResults_t pResults)
		{
			return FnTable.ComputeOverlayIntersection(ulOverlayHandle, ref pParams, ref pResults);
		}

		public bool HandleControllerOverlayInteractionAsMouse(ulong ulOverlayHandle, uint unControllerDeviceIndex)
		{
			return FnTable.HandleControllerOverlayInteractionAsMouse(ulOverlayHandle, unControllerDeviceIndex);
		}

		public bool IsHoverTargetOverlay(ulong ulOverlayHandle)
		{
			return FnTable.IsHoverTargetOverlay(ulOverlayHandle);
		}

		public ulong GetGamepadFocusOverlay()
		{
			return FnTable.GetGamepadFocusOverlay();
		}

		public EVROverlayError SetGamepadFocusOverlay(ulong ulNewFocusOverlay)
		{
			return FnTable.SetGamepadFocusOverlay(ulNewFocusOverlay);
		}

		public EVROverlayError SetOverlayNeighbor(EOverlayDirection eDirection, ulong ulFrom, ulong ulTo)
		{
			return FnTable.SetOverlayNeighbor(eDirection, ulFrom, ulTo);
		}

		public EVROverlayError MoveGamepadFocusToNeighbor(EOverlayDirection eDirection, ulong ulFrom)
		{
			return FnTable.MoveGamepadFocusToNeighbor(eDirection, ulFrom);
		}

		public EVROverlayError SetOverlayTexture(ulong ulOverlayHandle, ref Texture_t pTexture)
		{
			return FnTable.SetOverlayTexture(ulOverlayHandle, ref pTexture);
		}

		public EVROverlayError ClearOverlayTexture(ulong ulOverlayHandle)
		{
			return FnTable.ClearOverlayTexture(ulOverlayHandle);
		}

		public EVROverlayError SetOverlayRaw(ulong ulOverlayHandle, IntPtr pvBuffer, uint unWidth, uint unHeight, uint unDepth)
		{
			return FnTable.SetOverlayRaw(ulOverlayHandle, pvBuffer, unWidth, unHeight, unDepth);
		}

		public EVROverlayError SetOverlayFromFile(ulong ulOverlayHandle, string pchFilePath)
		{
			return FnTable.SetOverlayFromFile(ulOverlayHandle, pchFilePath);
		}

		public EVROverlayError CreateDashboardOverlay(string pchOverlayKey, string pchOverlayFriendlyName, ref ulong pMainHandle, ref ulong pThumbnailHandle)
		{
			pMainHandle = 0uL;
			pThumbnailHandle = 0uL;
			return FnTable.CreateDashboardOverlay(pchOverlayKey, pchOverlayFriendlyName, ref pMainHandle, ref pThumbnailHandle);
		}

		public bool IsDashboardVisible()
		{
			return FnTable.IsDashboardVisible();
		}

		public bool IsActiveDashboardOverlay(ulong ulOverlayHandle)
		{
			return FnTable.IsActiveDashboardOverlay(ulOverlayHandle);
		}

		public EVROverlayError SetDashboardOverlaySceneProcess(ulong ulOverlayHandle, uint unProcessId)
		{
			return FnTable.SetDashboardOverlaySceneProcess(ulOverlayHandle, unProcessId);
		}

		public EVROverlayError GetDashboardOverlaySceneProcess(ulong ulOverlayHandle, ref uint punProcessId)
		{
			punProcessId = 0u;
			return FnTable.GetDashboardOverlaySceneProcess(ulOverlayHandle, ref punProcessId);
		}

		public void ShowDashboard(string pchOverlayToShow)
		{
			FnTable.ShowDashboard(pchOverlayToShow);
		}

		public uint GetPrimaryDashboardDevice()
		{
			return FnTable.GetPrimaryDashboardDevice();
		}

		public EVROverlayError ShowKeyboard(int eInputMode, int eLineInputMode, string pchDescription, uint unCharMax, string pchExistingText, bool bUseMinimalMode, ulong uUserValue)
		{
			return FnTable.ShowKeyboard(eInputMode, eLineInputMode, pchDescription, unCharMax, pchExistingText, bUseMinimalMode, uUserValue);
		}

		public EVROverlayError ShowKeyboardForOverlay(ulong ulOverlayHandle, int eInputMode, int eLineInputMode, string pchDescription, uint unCharMax, string pchExistingText, bool bUseMinimalMode, ulong uUserValue)
		{
			return FnTable.ShowKeyboardForOverlay(ulOverlayHandle, eInputMode, eLineInputMode, pchDescription, unCharMax, pchExistingText, bUseMinimalMode, uUserValue);
		}

		public uint GetKeyboardText(StringBuilder pchText, uint cchText)
		{
			return FnTable.GetKeyboardText(pchText, cchText);
		}

		public void HideKeyboard()
		{
			FnTable.HideKeyboard();
		}

		public void SetKeyboardTransformAbsolute(ETrackingUniverseOrigin eTrackingOrigin, ref HmdMatrix34_t pmatTrackingOriginToKeyboardTransform)
		{
			FnTable.SetKeyboardTransformAbsolute(eTrackingOrigin, ref pmatTrackingOriginToKeyboardTransform);
		}

		public void SetKeyboardPositionForOverlay(ulong ulOverlayHandle, HmdRect2_t avoidRect)
		{
			FnTable.SetKeyboardPositionForOverlay(ulOverlayHandle, avoidRect);
		}
	}
}
