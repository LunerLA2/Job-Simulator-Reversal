using System;
using System.Runtime.InteropServices;

namespace Valve.VR
{
	public class CVRCompositor
	{
		private IVRCompositor FnTable;

		internal CVRCompositor(IntPtr pInterface)
		{
			FnTable = (IVRCompositor)Marshal.PtrToStructure(pInterface, typeof(IVRCompositor));
		}

		public void SetTrackingSpace(ETrackingUniverseOrigin eOrigin)
		{
			FnTable.SetTrackingSpace(eOrigin);
		}

		public ETrackingUniverseOrigin GetTrackingSpace()
		{
			return FnTable.GetTrackingSpace();
		}

		public EVRCompositorError WaitGetPoses(TrackedDevicePose_t[] pRenderPoseArray, TrackedDevicePose_t[] pGamePoseArray)
		{
			return FnTable.WaitGetPoses(pRenderPoseArray, (uint)pRenderPoseArray.Length, pGamePoseArray, (uint)pGamePoseArray.Length);
		}

		public EVRCompositorError GetLastPoses(TrackedDevicePose_t[] pRenderPoseArray, TrackedDevicePose_t[] pGamePoseArray)
		{
			return FnTable.GetLastPoses(pRenderPoseArray, (uint)pRenderPoseArray.Length, pGamePoseArray, (uint)pGamePoseArray.Length);
		}

		public EVRCompositorError GetLastPoseForTrackedDeviceIndex(uint unDeviceIndex, ref TrackedDevicePose_t pOutputPose, ref TrackedDevicePose_t pOutputGamePose)
		{
			return FnTable.GetLastPoseForTrackedDeviceIndex(unDeviceIndex, ref pOutputPose, ref pOutputGamePose);
		}

		public EVRCompositorError Submit(EVREye eEye, ref Texture_t pTexture, ref VRTextureBounds_t pBounds, EVRSubmitFlags nSubmitFlags)
		{
			return FnTable.Submit(eEye, ref pTexture, ref pBounds, nSubmitFlags);
		}

		public void ClearLastSubmittedFrame()
		{
			FnTable.ClearLastSubmittedFrame();
		}

		public void PostPresentHandoff()
		{
			FnTable.PostPresentHandoff();
		}

		public bool GetFrameTiming(ref Compositor_FrameTiming pTiming, uint unFramesAgo)
		{
			return FnTable.GetFrameTiming(ref pTiming, unFramesAgo);
		}

		public float GetFrameTimeRemaining()
		{
			return FnTable.GetFrameTimeRemaining();
		}

		public void FadeToColor(float fSeconds, float fRed, float fGreen, float fBlue, float fAlpha, bool bBackground)
		{
			FnTable.FadeToColor(fSeconds, fRed, fGreen, fBlue, fAlpha, bBackground);
		}

		public void FadeGrid(float fSeconds, bool bFadeIn)
		{
			FnTable.FadeGrid(fSeconds, bFadeIn);
		}

		public EVRCompositorError SetSkyboxOverride(Texture_t[] pTextures)
		{
			return FnTable.SetSkyboxOverride(pTextures, (uint)pTextures.Length);
		}

		public void ClearSkyboxOverride()
		{
			FnTable.ClearSkyboxOverride();
		}

		public void CompositorBringToFront()
		{
			FnTable.CompositorBringToFront();
		}

		public void CompositorGoToBack()
		{
			FnTable.CompositorGoToBack();
		}

		public void CompositorQuit()
		{
			FnTable.CompositorQuit();
		}

		public bool IsFullscreen()
		{
			return FnTable.IsFullscreen();
		}

		public uint GetCurrentSceneFocusProcess()
		{
			return FnTable.GetCurrentSceneFocusProcess();
		}

		public uint GetLastFrameRenderer()
		{
			return FnTable.GetLastFrameRenderer();
		}

		public bool CanRenderScene()
		{
			return FnTable.CanRenderScene();
		}

		public void ShowMirrorWindow()
		{
			FnTable.ShowMirrorWindow();
		}

		public void HideMirrorWindow()
		{
			FnTable.HideMirrorWindow();
		}

		public bool IsMirrorWindowVisible()
		{
			return FnTable.IsMirrorWindowVisible();
		}

		public void CompositorDumpImages()
		{
			FnTable.CompositorDumpImages();
		}

		public bool ShouldAppRenderWithLowResources()
		{
			return FnTable.ShouldAppRenderWithLowResources();
		}

		public void ForceInterleavedReprojectionOn(bool bOverride)
		{
			FnTable.ForceInterleavedReprojectionOn(bOverride);
		}
	}
}
