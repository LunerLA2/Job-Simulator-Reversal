using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VR;

public class OVRDebugInfo : MonoBehaviour
{
	private GameObject debugUIManager;

	private GameObject debugUIObject;

	private GameObject riftPresent;

	private GameObject fps;

	private GameObject ipd;

	private GameObject fov;

	private GameObject height;

	private GameObject depth;

	private GameObject resolutionEyeTexture;

	private GameObject latencies;

	private GameObject texts;

	private string strRiftPresent;

	private string strFPS;

	private string strIPD;

	private string strFOV;

	private string strHeight;

	private string strDepth;

	private string strResolutionEyeTexture;

	private string strLatencies;

	private float updateInterval = 0.5f;

	private float accum;

	private int frames;

	private float timeLeft;

	private bool initUIComponent;

	private bool isInited;

	private float offsetY = 55f;

	private float riftPresentTimeout;

	private bool showVRVars;

	private void Awake()
	{
		debugUIManager = new GameObject();
		debugUIManager.name = "DebugUIManager";
		debugUIManager.transform.parent = GameObject.Find("LeftEyeAnchor").transform;
		RectTransform rectTransform = debugUIManager.AddComponent<RectTransform>();
		rectTransform.sizeDelta = new Vector2(100f, 100f);
		rectTransform.localScale = new Vector3(0.001f, 0.001f, 0.001f);
		rectTransform.localPosition = new Vector3(0.01f, 0.17f, 0.53f);
		rectTransform.localEulerAngles = Vector3.zero;
		Canvas canvas = debugUIManager.AddComponent<Canvas>();
		canvas.renderMode = RenderMode.WorldSpace;
		canvas.pixelPerfect = false;
	}

	private void Update()
	{
		if (initUIComponent && !isInited)
		{
			InitUIComponents();
		}
		if (Input.GetKeyDown(KeyCode.Space) && riftPresentTimeout < 0f)
		{
			initUIComponent = true;
			showVRVars ^= true;
		}
		UpdateDeviceDetection();
		if (showVRVars)
		{
			debugUIManager.SetActive(true);
			UpdateVariable();
			UpdateStrings();
		}
		else
		{
			debugUIManager.SetActive(false);
		}
	}

	private void OnDestroy()
	{
		isInited = false;
	}

	private void InitUIComponents()
	{
		float num = 0f;
		int fontSize = 20;
		debugUIObject = new GameObject();
		debugUIObject.name = "DebugInfo";
		debugUIObject.transform.parent = GameObject.Find("DebugUIManager").transform;
		debugUIObject.transform.localPosition = new Vector3(0f, 100f, 0f);
		debugUIObject.transform.localEulerAngles = Vector3.zero;
		debugUIObject.transform.localScale = new Vector3(1f, 1f, 1f);
		if (!string.IsNullOrEmpty(strFPS))
		{
			fps = VariableObjectManager(fps, "FPS", num -= offsetY, strFPS, fontSize);
		}
		if (!string.IsNullOrEmpty(strIPD))
		{
			ipd = VariableObjectManager(ipd, "IPD", num -= offsetY, strIPD, fontSize);
		}
		if (!string.IsNullOrEmpty(strFOV))
		{
			fov = VariableObjectManager(fov, "FOV", num -= offsetY, strFOV, fontSize);
		}
		if (!string.IsNullOrEmpty(strHeight))
		{
			height = VariableObjectManager(height, "Height", num -= offsetY, strHeight, fontSize);
		}
		if (!string.IsNullOrEmpty(strDepth))
		{
			depth = VariableObjectManager(depth, "Depth", num -= offsetY, strDepth, fontSize);
		}
		if (!string.IsNullOrEmpty(strResolutionEyeTexture))
		{
			resolutionEyeTexture = VariableObjectManager(resolutionEyeTexture, "Resolution", num -= offsetY, strResolutionEyeTexture, fontSize);
		}
		if (!string.IsNullOrEmpty(strLatencies))
		{
			latencies = VariableObjectManager(latencies, "Latency", num -= offsetY, strLatencies, 17);
			num = 0f;
		}
		initUIComponent = false;
		isInited = true;
	}

	private void UpdateVariable()
	{
		UpdateIPD();
		UpdateEyeHeightOffset();
		UpdateEyeDepthOffset();
		UpdateFOV();
		UpdateResolutionEyeTexture();
		UpdateLatencyValues();
		UpdateFPS();
	}

	private void UpdateStrings()
	{
		if (!(debugUIObject == null))
		{
			if (!string.IsNullOrEmpty(strFPS))
			{
				fps.GetComponentInChildren<Text>().text = strFPS;
			}
			if (!string.IsNullOrEmpty(strIPD))
			{
				ipd.GetComponentInChildren<Text>().text = strIPD;
			}
			if (!string.IsNullOrEmpty(strFOV))
			{
				fov.GetComponentInChildren<Text>().text = strFOV;
			}
			if (!string.IsNullOrEmpty(strResolutionEyeTexture))
			{
				resolutionEyeTexture.GetComponentInChildren<Text>().text = strResolutionEyeTexture;
			}
			if (!string.IsNullOrEmpty(strLatencies))
			{
				latencies.GetComponentInChildren<Text>().text = strLatencies;
				latencies.GetComponentInChildren<Text>().fontSize = 14;
			}
			if (!string.IsNullOrEmpty(strHeight))
			{
				height.GetComponentInChildren<Text>().text = strHeight;
			}
			if (!string.IsNullOrEmpty(strDepth))
			{
				depth.GetComponentInChildren<Text>().text = strDepth;
			}
		}
	}

	private void RiftPresentGUI(GameObject guiMainOBj)
	{
		riftPresent = ComponentComposition(riftPresent);
		riftPresent.transform.SetParent(guiMainOBj.transform);
		riftPresent.name = "RiftPresent";
		RectTransform component = riftPresent.GetComponent<RectTransform>();
		component.localPosition = new Vector3(0f, 0f, 0f);
		component.localScale = new Vector3(1f, 1f, 1f);
		component.localEulerAngles = Vector3.zero;
		Text componentInChildren = riftPresent.GetComponentInChildren<Text>();
		componentInChildren.text = strRiftPresent;
		componentInChildren.fontSize = 20;
	}

	private void UpdateDeviceDetection()
	{
		if (riftPresentTimeout >= 0f)
		{
			riftPresentTimeout -= Time.deltaTime;
		}
	}

	private GameObject VariableObjectManager(GameObject gameObject, string name, float posY, string str, int fontSize)
	{
		gameObject = ComponentComposition(gameObject);
		gameObject.name = name;
		gameObject.transform.SetParent(debugUIObject.transform);
		RectTransform component = gameObject.GetComponent<RectTransform>();
		component.localPosition = new Vector3(0f, posY -= offsetY, 0f);
		Text componentInChildren = gameObject.GetComponentInChildren<Text>();
		componentInChildren.text = str;
		componentInChildren.fontSize = fontSize;
		gameObject.transform.localEulerAngles = Vector3.zero;
		component.localScale = new Vector3(1f, 1f, 1f);
		return gameObject;
	}

	private GameObject ComponentComposition(GameObject GO)
	{
		GO = new GameObject();
		GO.AddComponent<RectTransform>();
		GO.AddComponent<CanvasRenderer>();
		GO.AddComponent<Image>();
		GO.GetComponent<RectTransform>().sizeDelta = new Vector2(350f, 50f);
		GO.GetComponent<Image>().color = new Color(0.02745098f, 0.1764706f, 0.2784314f, 40f / 51f);
		texts = new GameObject();
		texts.AddComponent<RectTransform>();
		texts.AddComponent<CanvasRenderer>();
		texts.AddComponent<Text>();
		texts.GetComponent<RectTransform>().sizeDelta = new Vector2(350f, 50f);
		texts.GetComponent<Text>().font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
		texts.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
		texts.transform.SetParent(GO.transform);
		texts.name = "TextBox";
		return GO;
	}

	private void UpdateIPD()
	{
		strIPD = string.Format("IPD (mm): {0:F4}", OVRManager.profile.ipd * 1000f);
	}

	private void UpdateEyeHeightOffset()
	{
		float eyeHeight = OVRManager.profile.eyeHeight;
		strHeight = string.Format("Eye Height (m): {0:F3}", eyeHeight);
	}

	private void UpdateEyeDepthOffset()
	{
		float eyeDepth = OVRManager.profile.eyeDepth;
		strDepth = string.Format("Eye Depth (m): {0:F3}", eyeDepth);
	}

	private void UpdateFOV()
	{
		strFOV = string.Format("FOV (deg): {0:F3}", OVRManager.display.GetEyeRenderDesc(UnityEngine.XR.XRNode.LeftEye).fov.y);
	}

	private void UpdateResolutionEyeTexture()
	{
		OVRDisplay.EyeRenderDesc eyeRenderDesc = OVRManager.display.GetEyeRenderDesc(UnityEngine.XR.XRNode.LeftEye);
		OVRDisplay.EyeRenderDesc eyeRenderDesc2 = OVRManager.display.GetEyeRenderDesc(UnityEngine.XR.XRNode.RightEye);
		float renderScale = UnityEngine.XR.XRSettings.eyeTextureResolutionScale;
		float num = (int)(renderScale * (eyeRenderDesc.resolution.x + eyeRenderDesc2.resolution.x));
		float num2 = (int)(renderScale * Mathf.Max(eyeRenderDesc.resolution.y, eyeRenderDesc2.resolution.y));
		strResolutionEyeTexture = string.Format("Resolution : {0} x {1}", num, num2);
	}

	private void UpdateLatencyValues()
	{
		OVRDisplay.LatencyData latency = OVRManager.display.latency;
		if (latency.render < 1E-06f && latency.timeWarp < 1E-06f && latency.postPresent < 1E-06f)
		{
			strLatencies = string.Format("Latency values are not available.");
			return;
		}
		strLatencies = string.Format("Render: {0:F3} TimeWarp: {1:F3} Post-Present: {2:F3}\nRender Error: {3:F3} TimeWarp Error: {4:F3}", latency.render, latency.timeWarp, latency.postPresent, latency.renderError, latency.timeWarpError);
	}

	private void UpdateFPS()
	{
		timeLeft -= Time.unscaledDeltaTime;
		accum += Time.unscaledDeltaTime;
		frames++;
		if ((double)timeLeft <= 0.0)
		{
			float num = (float)frames / accum;
			strFPS = string.Format("FPS: {0:F2}", num);
			timeLeft += updateInterval;
			accum = 0f;
			frames = 0;
		}
	}
}
