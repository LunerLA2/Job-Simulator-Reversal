using System.Collections;
using System.Reflection;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class SteamVR_Camera : MonoBehaviour
{
	private const string eyeSuffix = " (eye)";

	private const string earsSuffix = " (ears)";

	private const string headSuffix = " (head)";

	private const string originSuffix = " (origin)";

	[SerializeField]
	private Transform _head;

	[SerializeField]
	private Transform _ears;

	public bool wireframe;

	[SerializeField]
	private SteamVR_CameraFlip flip;

	public static Material blitMaterial;

	public static float sceneResolutionScale = 1f;

	private static RenderTexture _sceneTexture;

	private static Hashtable values;

	public Transform head
	{
		get
		{
			return _head;
		}
	}

	public Transform offset
	{
		get
		{
			return _head;
		}
	}

	public Transform origin
	{
		get
		{
			return _head.parent;
		}
	}

	public Transform ears
	{
		get
		{
			return _ears;
		}
	}

	public string baseName
	{
		get
		{
			return (!base.name.EndsWith(" (eye)")) ? base.name : base.name.Substring(0, base.name.Length - " (eye)".Length);
		}
	}

	public Ray GetRay()
	{
		return new Ray(_head.position, _head.forward);
	}

	public static RenderTexture GetSceneTexture(bool hdr)
	{
		SteamVR instance = SteamVR.instance;
		if (instance == null)
		{
			return null;
		}
		int num = (int)(instance.sceneWidth * sceneResolutionScale);
		int num2 = (int)(instance.sceneHeight * sceneResolutionScale);
		int num3 = ((QualitySettings.antiAliasing == 0) ? 1 : QualitySettings.antiAliasing);
		RenderTextureFormat renderTextureFormat = (hdr ? RenderTextureFormat.ARGBHalf : RenderTextureFormat.ARGB32);
		if (_sceneTexture != null && (_sceneTexture.width != num || _sceneTexture.height != num2 || _sceneTexture.antiAliasing != num3 || _sceneTexture.format != renderTextureFormat))
		{
			Debug.Log(string.Format("Recreating scene texture.. Old: {0}x{1} MSAA={2} [{3}] New: {4}x{5} MSAA={6} [{7}]", _sceneTexture.width, _sceneTexture.height, _sceneTexture.antiAliasing, _sceneTexture.format, num, num2, num3, renderTextureFormat));
			Object.Destroy(_sceneTexture);
			_sceneTexture = null;
		}
		if (_sceneTexture == null)
		{
			_sceneTexture = new RenderTexture(num, num2, 0, renderTextureFormat);
			_sceneTexture.antiAliasing = num3;
		}
		return _sceneTexture;
	}

	private void OnDisable()
	{
		SteamVR_Render.Remove(this);
	}

	private void OnEnable()
	{
		Transform transform = base.transform;
		if (head != transform)
		{
			Expand();
			transform.parent = origin;
			while (head.childCount > 0)
			{
				head.GetChild(0).parent = transform;
			}
			Object.DestroyImmediate(head.gameObject);
			_head = transform;
		}
		if (flip != null)
		{
			Object.DestroyImmediate(flip);
			flip = null;
		}
		Camera component = GetComponent<Camera>();
		component.eventMask = 0;
		if (component.gameObject.GetComponent<GUILayer>() == null)
		{
			GUILayer gUILayer = component.gameObject.AddComponent<GUILayer>();
			gUILayer.enabled = false;
		}
		if (!SteamVR.usingNativeSupport)
		{
			base.enabled = false;
			return;
		}
		ears.GetComponent<SteamVR_Ears>().vrcam = this;
		SteamVR_Render.Add(this);
	}

	private void Awake()
	{
		ForceLast();
	}

	public void ForceLast()
	{
		if (values != null)
		{
			foreach (DictionaryEntry value in values)
			{
				FieldInfo fieldInfo = value.Key as FieldInfo;
				fieldInfo.SetValue(this, value.Value);
			}
			values = null;
			return;
		}
		Component[] components = GetComponents<Component>();
		for (int i = 0; i < components.Length; i++)
		{
			SteamVR_Camera steamVR_Camera = components[i] as SteamVR_Camera;
			if (steamVR_Camera != null && steamVR_Camera != this)
			{
				if (steamVR_Camera.flip != null)
				{
					Object.DestroyImmediate(steamVR_Camera.flip);
				}
				Object.DestroyImmediate(steamVR_Camera);
			}
		}
		components = GetComponents<Component>();
		if (!(this != components[components.Length - 1]))
		{
			return;
		}
		values = new Hashtable();
		FieldInfo[] fields = GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		FieldInfo[] array = fields;
		foreach (FieldInfo fieldInfo2 in array)
		{
			if (fieldInfo2.IsPublic || fieldInfo2.IsDefined(typeof(SerializeField), true))
			{
				values[fieldInfo2] = fieldInfo2.GetValue(this);
			}
		}
		GameObject gameObject = base.gameObject;
		Object.DestroyImmediate(this);
		gameObject.AddComponent<SteamVR_Camera>().ForceLast();
	}

	public void Expand()
	{
		Transform parent = base.transform.parent;
		if (parent == null)
		{
			parent = new GameObject(base.name + " (origin)").transform;
			parent.localPosition = base.transform.localPosition;
			parent.localRotation = base.transform.localRotation;
			parent.localScale = base.transform.localScale;
		}
		if (head == null)
		{
			_head = new GameObject(base.name + " (head)", typeof(SteamVR_GameView), typeof(SteamVR_TrackedObject)).transform;
			head.parent = parent;
			head.position = base.transform.position;
			head.rotation = base.transform.rotation;
			head.localScale = Vector3.one;
			head.tag = base.tag;
			Camera component = head.GetComponent<Camera>();
			component.clearFlags = CameraClearFlags.Nothing;
			component.cullingMask = 0;
			component.eventMask = 0;
			component.orthographic = true;
			component.orthographicSize = 1f;
			component.nearClipPlane = 0f;
			component.farClipPlane = 1f;
			component.useOcclusionCulling = false;
		}
		if (base.transform.parent != head)
		{
			base.transform.parent = head;
			base.transform.localPosition = Vector3.zero;
			base.transform.localRotation = Quaternion.identity;
			base.transform.localScale = Vector3.one;
			while (base.transform.childCount > 0)
			{
				base.transform.GetChild(0).parent = head;
			}
			GUILayer component2 = GetComponent<GUILayer>();
			if (component2 != null)
			{
				Object.DestroyImmediate(component2);
				head.gameObject.AddComponent<GUILayer>();
			}
			AudioListener component3 = GetComponent<AudioListener>();
			if (component3 != null)
			{
				Object.DestroyImmediate(component3);
				_ears = new GameObject(base.name + " (ears)", typeof(SteamVR_Ears)).transform;
				ears.parent = _head;
				ears.localPosition = Vector3.zero;
				ears.localRotation = Quaternion.identity;
				ears.localScale = Vector3.one;
			}
		}
		if (!base.name.EndsWith(" (eye)"))
		{
			base.name += " (eye)";
		}
	}

	public void Collapse()
	{
		base.transform.parent = null;
		while (head.childCount > 0)
		{
			head.GetChild(0).parent = base.transform;
		}
		GUILayer component = head.GetComponent<GUILayer>();
		if (component != null)
		{
			Object.DestroyImmediate(component);
			base.gameObject.AddComponent<GUILayer>();
		}
		if (ears != null)
		{
			while (ears.childCount > 0)
			{
				ears.GetChild(0).parent = base.transform;
			}
			Object.DestroyImmediate(ears.gameObject);
			_ears = null;
			base.gameObject.AddComponent(typeof(AudioListener));
		}
		if (origin != null)
		{
			if (origin.name.EndsWith(" (origin)"))
			{
				Transform transform = origin;
				while (transform.childCount > 0)
				{
					transform.GetChild(0).parent = transform.parent;
				}
				Object.DestroyImmediate(transform.gameObject);
			}
			else
			{
				base.transform.parent = origin;
			}
		}
		Object.DestroyImmediate(head.gameObject);
		_head = null;
		if (base.name.EndsWith(" (eye)"))
		{
			base.name = base.name.Substring(0, base.name.Length - " (eye)".Length);
		}
	}
}
