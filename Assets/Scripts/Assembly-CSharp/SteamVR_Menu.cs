using System;
using UnityEngine;
using Valve.VR;

public class SteamVR_Menu : MonoBehaviour
{
	public Texture cursor;

	public Texture background;

	public Texture logo;

	public float logoHeight;

	public float menuOffset;

	public Vector2 scaleLimits = new Vector2(0.1f, 5f);

	public float scaleRate = 0.5f;

	private SteamVR_Overlay overlay;

	private Camera overlayCam;

	private Vector4 uvOffset;

	private float distance;

	private string scaleLimitX;

	private string scaleLimitY;

	private string scaleRateText;

	private CursorLockMode savedCursorLockState;

	private bool savedCursorVisible;

	public RenderTexture texture
	{
		get
		{
			return (!overlay) ? null : (overlay.texture as RenderTexture);
		}
	}

	public float scale { get; private set; }

	private void Awake()
	{
		scaleLimitX = string.Format("{0:N1}", scaleLimits.x);
		scaleLimitY = string.Format("{0:N1}", scaleLimits.y);
		scaleRateText = string.Format("{0:N1}", scaleRate);
		SteamVR_Overlay instance = SteamVR_Overlay.instance;
		if (instance != null)
		{
			uvOffset = instance.uvOffset;
			distance = instance.distance;
		}
	}

	private void OnGUI()
	{
		if (overlay == null)
		{
			return;
		}
		RenderTexture renderTexture = overlay.texture as RenderTexture;
		RenderTexture active = RenderTexture.active;
		RenderTexture.active = renderTexture;
		if (Event.current.type == EventType.Repaint)
		{
			GL.Clear(false, true, Color.clear);
		}
		Rect screenRect = new Rect(0f, 0f, renderTexture.width, renderTexture.height);
		if (Screen.width < renderTexture.width)
		{
			screenRect.width = Screen.width;
			overlay.uvOffset.x = (0f - (float)(renderTexture.width - Screen.width)) / (float)(2 * renderTexture.width);
		}
		if (Screen.height < renderTexture.height)
		{
			screenRect.height = Screen.height;
			overlay.uvOffset.y = (float)(renderTexture.height - Screen.height) / (float)(2 * renderTexture.height);
		}
		GUILayout.BeginArea(screenRect);
		if (background != null)
		{
			GUI.DrawTexture(new Rect((screenRect.width - (float)background.width) / 2f, (screenRect.height - (float)background.height) / 2f, background.width, background.height), background);
		}
		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		GUILayout.BeginVertical();
		if (logo != null)
		{
			GUILayout.Space(screenRect.height / 2f - logoHeight);
			GUILayout.Box(logo);
		}
		GUILayout.Space(menuOffset);
		bool flag = GUILayout.Button("[Esc] - Close menu");
		GUILayout.BeginHorizontal();
		GUILayout.Label(string.Format("Scale: {0:N4}", scale));
		float num = GUILayout.HorizontalSlider(scale, scaleLimits.x, scaleLimits.y);
		if (num != scale)
		{
			SetScale(num);
		}
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		GUILayout.Label(string.Format("Scale limits:"));
		string text = GUILayout.TextField(scaleLimitX);
		if (text != scaleLimitX && float.TryParse(text, out scaleLimits.x))
		{
			scaleLimitX = text;
		}
		string text2 = GUILayout.TextField(scaleLimitY);
		if (text2 != scaleLimitY && float.TryParse(text2, out scaleLimits.y))
		{
			scaleLimitY = text2;
		}
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		GUILayout.Label(string.Format("Scale rate:"));
		string text3 = GUILayout.TextField(scaleRateText);
		if (text3 != scaleRateText && float.TryParse(text3, out scaleRate))
		{
			scaleRateText = text3;
		}
		GUILayout.EndHorizontal();
		if (SteamVR.active)
		{
			SteamVR instance = SteamVR.instance;
			GUILayout.BeginHorizontal();
			float sceneResolutionScale = SteamVR_Camera.sceneResolutionScale;
			int num2 = (int)(instance.sceneWidth * sceneResolutionScale);
			int num3 = (int)(instance.sceneHeight * sceneResolutionScale);
			int num4 = (int)(100f * sceneResolutionScale);
			GUILayout.Label(string.Format("Scene quality: {0}x{1} ({2}%)", num2, num3, num4));
			int num5 = Mathf.RoundToInt(GUILayout.HorizontalSlider(num4, 50f, 200f));
			if (num5 != num4)
			{
				SteamVR_Camera.sceneResolutionScale = (float)num5 / 100f;
			}
			GUILayout.EndHorizontal();
		}
		overlay.highquality = GUILayout.Toggle(overlay.highquality, "High quality");
		if (overlay.highquality)
		{
			overlay.curved = GUILayout.Toggle(overlay.curved, "Curved overlay");
			overlay.antialias = GUILayout.Toggle(overlay.antialias, "Overlay RGSS(2x2)");
		}
		else
		{
			overlay.curved = false;
			overlay.antialias = false;
		}
		SteamVR_Camera steamVR_Camera = SteamVR_Render.Top();
		if (steamVR_Camera != null)
		{
			steamVR_Camera.wireframe = GUILayout.Toggle(steamVR_Camera.wireframe, "Wireframe");
			SteamVR_Render instance2 = SteamVR_Render.instance;
			if (instance2.trackingSpace == ETrackingUniverseOrigin.TrackingUniverseSeated)
			{
				if (GUILayout.Button("Switch to Standing"))
				{
					instance2.trackingSpace = ETrackingUniverseOrigin.TrackingUniverseStanding;
				}
				if (GUILayout.Button("Center View"))
				{
					CVRSystem system = OpenVR.System;
					if (system != null)
					{
						system.ResetSeatedZeroPose();
					}
				}
			}
			else if (GUILayout.Button("Switch to Seated"))
			{
				instance2.trackingSpace = ETrackingUniverseOrigin.TrackingUniverseSeated;
			}
		}
		if (GUILayout.Button("Exit"))
		{
			Application.Quit();
		}
		GUILayout.Space(menuOffset);
		string environmentVariable = Environment.GetEnvironmentVariable("VR_OVERRIDE");
		if (environmentVariable != null)
		{
			GUILayout.Label("VR_OVERRIDE=" + environmentVariable);
		}
		GUILayout.Label("Graphics device: " + SystemInfo.graphicsDeviceVersion);
		GUILayout.EndVertical();
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		GUILayout.EndArea();
		if (cursor != null)
		{
			float x = Input.mousePosition.x;
			float y = (float)Screen.height - Input.mousePosition.y;
			float width = cursor.width;
			float height = cursor.height;
			GUI.DrawTexture(new Rect(x, y, width, height), cursor);
		}
		RenderTexture.active = active;
		if (flag)
		{
			HideMenu();
		}
	}

	public void ShowMenu()
	{
		SteamVR_Overlay instance = SteamVR_Overlay.instance;
		if (instance == null)
		{
			return;
		}
		RenderTexture renderTexture = instance.texture as RenderTexture;
		if (renderTexture == null)
		{
			Debug.LogError("Menu requires overlay texture to be a render texture.");
			return;
		}
		SaveCursorState();
		Cursor.visible = true;
		Cursor.lockState = CursorLockMode.None;
		overlay = instance;
		uvOffset = instance.uvOffset;
		distance = instance.distance;
		Camera[] array = UnityEngine.Object.FindObjectsOfType(typeof(Camera)) as Camera[];
		Camera[] array2 = array;
		foreach (Camera camera in array2)
		{
			if (camera.enabled && camera.targetTexture == renderTexture)
			{
				overlayCam = camera;
				overlayCam.enabled = false;
				break;
			}
		}
		SteamVR_Camera steamVR_Camera = SteamVR_Render.Top();
		if (steamVR_Camera != null)
		{
			scale = steamVR_Camera.origin.localScale.x;
		}
	}

	public void HideMenu()
	{
		RestoreCursorState();
		if (overlayCam != null)
		{
			overlayCam.enabled = true;
		}
		if (overlay != null)
		{
			overlay.uvOffset = uvOffset;
			overlay.distance = distance;
			overlay = null;
		}
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Joystick1Button7))
		{
			if (overlay == null)
			{
				ShowMenu();
			}
			else
			{
				HideMenu();
			}
		}
		else if (Input.GetKeyDown(KeyCode.Home))
		{
			SetScale(1f);
		}
		else if (Input.GetKey(KeyCode.PageUp))
		{
			SetScale(Mathf.Clamp(scale + scaleRate * Time.deltaTime, scaleLimits.x, scaleLimits.y));
		}
		else if (Input.GetKey(KeyCode.PageDown))
		{
			SetScale(Mathf.Clamp(scale - scaleRate * Time.deltaTime, scaleLimits.x, scaleLimits.y));
		}
	}

	private void SetScale(float scale)
	{
		this.scale = scale;
		SteamVR_Camera steamVR_Camera = SteamVR_Render.Top();
		if (steamVR_Camera != null)
		{
			steamVR_Camera.origin.localScale = new Vector3(scale, scale, scale);
		}
	}

	private void SaveCursorState()
	{
		savedCursorVisible = Cursor.visible;
		savedCursorLockState = Cursor.lockState;
	}

	private void RestoreCursorState()
	{
		Cursor.visible = savedCursorVisible;
		Cursor.lockState = savedCursorLockState;
	}
}
