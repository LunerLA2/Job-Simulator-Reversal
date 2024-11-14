using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using Valve.VR;

public class SteamVR_LoadLevel : MonoBehaviour
{
	private static SteamVR_LoadLevel _active;

	public string levelName;

	public bool loadExternalApp;

	public string externalAppPath;

	public string externalAppArgs;

	public bool loadAdditive;

	public bool loadAsync = true;

	public Texture loadingScreen;

	public Texture progressBarEmpty;

	public Texture progressBarFull;

	public float loadingScreenWidthInMeters = 6f;

	public float progressBarWidthInMeters = 3f;

	public float loadingScreenDistance;

	public Transform loadingScreenTransform;

	public Transform progressBarTransform;

	public Texture front;

	public Texture back;

	public Texture left;

	public Texture right;

	public Texture top;

	public Texture bottom;

	public Color backgroundColor = Color.black;

	public bool showGrid;

	public float fadeOutTime = 0.5f;

	public float fadeInTime = 0.5f;

	public float postLoadSettleTime;

	public float loadingScreenFadeInTime = 1f;

	public float loadingScreenFadeOutTime = 0.25f;

	private float fadeRate = 1f;

	private float alpha;

	private AsyncOperation async;

	private RenderTexture renderTexture;

	private ulong loadingScreenOverlayHandle;

	private ulong progressBarOverlayHandle;

	public bool autoTriggerOnEnable;

	public static bool loading
	{
		get
		{
			return _active != null;
		}
	}

	public static float progress
	{
		get
		{
			return (!(_active != null) || _active.async == null) ? 0f : _active.async.progress;
		}
	}

	public static Texture progressTexture
	{
		get
		{
			return (!(_active != null)) ? null : _active.renderTexture;
		}
	}

	private void OnEnable()
	{
		if (autoTriggerOnEnable)
		{
			Trigger();
		}
	}

	public void Trigger()
	{
		if (!loading && !string.IsNullOrEmpty(levelName))
		{
			StartCoroutine("LoadLevel");
		}
	}

	public static void Begin(string levelName, bool showGrid = false, float fadeOutTime = 0.5f, float r = 0f, float g = 0f, float b = 0f, float a = 1f)
	{
		SteamVR_LoadLevel steamVR_LoadLevel = new GameObject("loader").AddComponent<SteamVR_LoadLevel>();
		steamVR_LoadLevel.levelName = levelName;
		steamVR_LoadLevel.showGrid = showGrid;
		steamVR_LoadLevel.fadeOutTime = fadeOutTime;
		steamVR_LoadLevel.backgroundColor = new Color(r, g, b, a);
		steamVR_LoadLevel.Trigger();
	}

	private void OnGUI()
	{
		if (_active != this || !(progressBarEmpty != null) || !(progressBarFull != null))
		{
			return;
		}
		if (progressBarOverlayHandle == 0L)
		{
			progressBarOverlayHandle = GetOverlayHandle("progressBar", (!(progressBarTransform != null)) ? base.transform : progressBarTransform, progressBarWidthInMeters);
		}
		if (progressBarOverlayHandle != 0L)
		{
			float num = ((async == null) ? 0f : async.progress);
			int width = progressBarFull.width;
			int height = progressBarFull.height;
			if (renderTexture == null)
			{
				renderTexture = new RenderTexture(width, height, 0);
				renderTexture.Create();
			}
			RenderTexture active = RenderTexture.active;
			RenderTexture.active = renderTexture;
			if (Event.current.type == EventType.Repaint)
			{
				GL.Clear(false, true, Color.clear);
			}
			GUILayout.BeginArea(new Rect(0f, 0f, width, height));
			GUI.DrawTexture(new Rect(0f, 0f, width, height), progressBarEmpty);
			GUI.DrawTextureWithTexCoords(new Rect(0f, 0f, num * (float)width, height), progressBarFull, new Rect(0f, 0f, num, 1f));
			GUILayout.EndArea();
			RenderTexture.active = active;
			CVROverlay overlay = OpenVR.Overlay;
			if (overlay != null)
			{
				Texture_t pTexture = default(Texture_t);
				pTexture.handle = renderTexture.GetNativeTexturePtr();
				pTexture.eType = SteamVR.instance.graphicsAPI;
				pTexture.eColorSpace = EColorSpace.Auto;
				overlay.SetOverlayTexture(progressBarOverlayHandle, ref pTexture);
			}
		}
	}

	private void Update()
	{
		if (_active != this)
		{
			return;
		}
		alpha = Mathf.Clamp01(alpha + fadeRate * Time.deltaTime);
		CVROverlay overlay = OpenVR.Overlay;
		if (overlay != null)
		{
			if (loadingScreenOverlayHandle != 0L)
			{
				overlay.SetOverlayAlpha(loadingScreenOverlayHandle, alpha);
			}
			if (progressBarOverlayHandle != 0L)
			{
				overlay.SetOverlayAlpha(progressBarOverlayHandle, alpha);
			}
		}
	}

	private IEnumerator LoadLevel()
	{
		if (loadingScreen != null && loadingScreenDistance > 0f)
		{
			SteamVR_Controller.Device hmd = SteamVR_Controller.Input(0);
			while (!hmd.hasTracking)
			{
				yield return null;
			}
			SteamVR_Utils.RigidTransform tloading = hmd.transform;
			tloading.rot = Quaternion.Euler(0f, tloading.rot.eulerAngles.y, 0f);
			tloading.pos += tloading.rot * new Vector3(0f, 0f, loadingScreenDistance);
			Transform t = ((!(loadingScreenTransform != null)) ? base.transform : loadingScreenTransform);
			t.position = tloading.pos;
			t.rotation = tloading.rot;
		}
		_active = this;
		SteamVR_Utils.Event.Send("loading", true);
		if (loadingScreenFadeInTime > 0f)
		{
			fadeRate = 1f / loadingScreenFadeInTime;
		}
		else
		{
			alpha = 1f;
		}
		CVROverlay overlay = OpenVR.Overlay;
		if (loadingScreen != null && overlay != null)
		{
			loadingScreenOverlayHandle = GetOverlayHandle("loadingScreen", (!(loadingScreenTransform != null)) ? base.transform : loadingScreenTransform, loadingScreenWidthInMeters);
			if (loadingScreenOverlayHandle != 0L)
			{
				Texture_t texture = new Texture_t
				{
					handle = loadingScreen.GetNativeTexturePtr(),
					eType = SteamVR.instance.graphicsAPI,
					eColorSpace = EColorSpace.Auto
				};
				overlay.SetOverlayTexture(loadingScreenOverlayHandle, ref texture);
			}
		}
		bool fadedForeground = false;
		SteamVR_Utils.Event.Send("loading_fade_out", fadeOutTime);
		CVRCompositor compositor = OpenVR.Compositor;
		if (compositor != null)
		{
			if (front != null)
			{
				SteamVR_Skybox.SetOverride(front, back, left, right, top, bottom);
				compositor.FadeGrid(fadeOutTime, true);
				yield return new WaitForSeconds(fadeOutTime);
			}
			else if (backgroundColor != Color.clear)
			{
				if (showGrid)
				{
					compositor.FadeToColor(0f, backgroundColor.r, backgroundColor.g, backgroundColor.b, backgroundColor.a, true);
					compositor.FadeGrid(fadeOutTime, true);
					yield return new WaitForSeconds(fadeOutTime);
				}
				else
				{
					compositor.FadeToColor(fadeOutTime, backgroundColor.r, backgroundColor.g, backgroundColor.b, backgroundColor.a, false);
					yield return new WaitForSeconds(fadeOutTime + 0.1f);
					compositor.FadeGrid(0f, true);
					fadedForeground = true;
				}
			}
		}
		SteamVR_Render.pauseRendering = true;
		while (alpha < 1f)
		{
			yield return null;
		}
		base.transform.parent = null;
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		if (loadExternalApp)
		{
			Debug.Log("Launching external application...");
			CVRApplications applications = OpenVR.Applications;
			if (applications == null)
			{
				Debug.Log("Failed to get OpenVR.Applications interface!");
			}
			else
			{
				string workingDirectory = Directory.GetCurrentDirectory();
				Debug.Log("working directory: " + workingDirectory);
				Debug.Log("path: " + externalAppPath);
				Debug.Log("args: " + externalAppArgs);
				EVRApplicationError error = applications.LaunchInternalProcess(externalAppPath, externalAppArgs, workingDirectory);
				Debug.Log("LaunchInternalProcessError: " + error);
				Application.Quit();
			}
		}
		else
		{
			LoadSceneMode mode = (loadAdditive ? LoadSceneMode.Additive : LoadSceneMode.Single);
			if (loadAsync)
			{
				async = SceneManager.LoadSceneAsync(levelName, mode);
				yield return async;
			}
			else
			{
				SceneManager.LoadScene(levelName, mode);
			}
		}
		GC.Collect();
		yield return new WaitForSeconds(postLoadSettleTime);
		SteamVR_Render.pauseRendering = false;
		if (loadingScreenFadeOutTime > 0f)
		{
			fadeRate = -1f / loadingScreenFadeOutTime;
		}
		else
		{
			alpha = 0f;
		}
		SteamVR_Utils.Event.Send("loading_fade_in", fadeInTime);
		if (compositor != null)
		{
			if (fadedForeground)
			{
				compositor.FadeGrid(0f, false);
				compositor.FadeToColor(fadeInTime, 0f, 0f, 0f, 0f, false);
				yield return new WaitForSeconds(fadeInTime);
			}
			else
			{
				compositor.FadeGrid(fadeInTime, false);
				yield return new WaitForSeconds(fadeInTime);
				if (front != null)
				{
					SteamVR_Skybox.ClearOverride();
				}
			}
		}
		while (alpha > 0f)
		{
			yield return null;
		}
		if (overlay != null)
		{
			if (progressBarOverlayHandle != 0L)
			{
				overlay.HideOverlay(progressBarOverlayHandle);
			}
			if (loadingScreenOverlayHandle != 0L)
			{
				overlay.HideOverlay(loadingScreenOverlayHandle);
			}
		}
		UnityEngine.Object.Destroy(base.gameObject);
		_active = null;
		SteamVR_Utils.Event.Send("loading", false);
	}

	private ulong GetOverlayHandle(string overlayName, Transform transform, float widthInMeters = 1f)
	{
		ulong pOverlayHandle = 0uL;
		CVROverlay overlay = OpenVR.Overlay;
		if (overlay == null)
		{
			return pOverlayHandle;
		}
		string pchOverlayKey = SteamVR_Overlay.key + "." + overlayName;
		EVROverlayError eVROverlayError = overlay.FindOverlay(pchOverlayKey, ref pOverlayHandle);
		if (eVROverlayError != 0)
		{
			eVROverlayError = overlay.CreateOverlay(pchOverlayKey, overlayName, ref pOverlayHandle);
		}
		if (eVROverlayError == EVROverlayError.None)
		{
			overlay.ShowOverlay(pOverlayHandle);
			overlay.SetOverlayAlpha(pOverlayHandle, alpha);
			overlay.SetOverlayWidthInMeters(pOverlayHandle, widthInMeters);
			if (SteamVR.instance.graphicsAPI == EGraphicsAPIConvention.API_DirectX)
			{
				VRTextureBounds_t pOverlayTextureBounds = default(VRTextureBounds_t);
				pOverlayTextureBounds.uMin = 0f;
				pOverlayTextureBounds.vMin = 1f;
				pOverlayTextureBounds.uMax = 1f;
				pOverlayTextureBounds.vMax = 0f;
				overlay.SetOverlayTextureBounds(pOverlayHandle, ref pOverlayTextureBounds);
			}
			SteamVR_Camera steamVR_Camera = ((loadingScreenDistance != 0f) ? null : SteamVR_Render.Top());
			if (steamVR_Camera != null && steamVR_Camera.origin != null)
			{
				SteamVR_Utils.RigidTransform rigidTransform = new SteamVR_Utils.RigidTransform(steamVR_Camera.origin, transform);
				rigidTransform.pos.x /= steamVR_Camera.origin.localScale.x;
				rigidTransform.pos.y /= steamVR_Camera.origin.localScale.y;
				rigidTransform.pos.z /= steamVR_Camera.origin.localScale.z;
				HmdMatrix34_t pmatTrackingOriginToOverlayTransform = rigidTransform.ToHmdMatrix34();
				overlay.SetOverlayTransformAbsolute(pOverlayHandle, SteamVR_Render.instance.trackingSpace, ref pmatTrackingOriginToOverlayTransform);
			}
			else
			{
				HmdMatrix34_t pmatTrackingOriginToOverlayTransform2 = new SteamVR_Utils.RigidTransform(transform).ToHmdMatrix34();
				overlay.SetOverlayTransformAbsolute(pOverlayHandle, SteamVR_Render.instance.trackingSpace, ref pmatTrackingOriginToOverlayTransform2);
			}
		}
		return pOverlayHandle;
	}
}
