using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.Rendering;
using Valve.VR;

public class SteamVR_ExternalCamera : MonoBehaviour
{
	public struct Config
	{
		public float x;

		public float y;

		public float z;

		public float rx;

		public float ry;

		public float rz;

		public float fov;

		public float near;

		public float far;

		public float sceneResolutionScale;

		public float frameSkip;

		public float nearOffset;

		public float farOffset;

		public float hmdOffset;

		public bool disableStandardAssets;
	}

	public Config config;

	public string configPath;

	private Camera cam;

	private Transform target;

	private GameObject clipQuad;

	private Material clipMaterial;

	private Material colorMat;

	private Material alphaMat;

	private Camera[] cameras;

	private Rect[] cameraRects;

	private float sceneResolutionScale;

	public void ReadConfig()
	{
		try
		{
			HmdMatrix34_t pose = default(HmdMatrix34_t);
			bool flag = false;
			object obj = config;
			string[] array = File.ReadAllLines(configPath);
			string[] array2 = array;
			foreach (string text in array2)
			{
				string[] array3 = text.Split('=');
				if (array3.Length != 2)
				{
					continue;
				}
				string text2 = array3[0];
				if (text2 == "m")
				{
					string[] array4 = array3[1].Split(',');
					if (array4.Length == 12)
					{
						pose.m0 = float.Parse(array4[0]);
						pose.m1 = float.Parse(array4[1]);
						pose.m2 = float.Parse(array4[2]);
						pose.m3 = float.Parse(array4[3]);
						pose.m4 = float.Parse(array4[4]);
						pose.m5 = float.Parse(array4[5]);
						pose.m6 = float.Parse(array4[6]);
						pose.m7 = float.Parse(array4[7]);
						pose.m8 = float.Parse(array4[8]);
						pose.m9 = float.Parse(array4[9]);
						pose.m10 = float.Parse(array4[10]);
						pose.m11 = float.Parse(array4[11]);
						flag = true;
					}
				}
				else if (text2 == "disableStandardAssets")
				{
					FieldInfo field = obj.GetType().GetField(text2);
					if (field != null)
					{
						field.SetValue(obj, bool.Parse(array3[1]));
					}
				}
				else
				{
					FieldInfo field2 = obj.GetType().GetField(text2);
					if (field2 != null)
					{
						field2.SetValue(obj, float.Parse(array3[1]));
					}
				}
			}
			config = (Config)obj;
			if (flag)
			{
				SteamVR_Utils.RigidTransform rigidTransform = new SteamVR_Utils.RigidTransform(pose);
				config.x = rigidTransform.pos.x;
				config.y = rigidTransform.pos.y;
				config.z = rigidTransform.pos.z;
				Vector3 eulerAngles = rigidTransform.rot.eulerAngles;
				config.rx = eulerAngles.x;
				config.ry = eulerAngles.y;
				config.rz = eulerAngles.z;
			}
		}
		catch
		{
		}
	}

	public void AttachToCamera(SteamVR_Camera vrcam)
	{
		if (!(target == vrcam.head))
		{
			target = vrcam.head;
			Transform parent = base.transform.parent;
			Transform parent2 = vrcam.head.parent;
			parent.parent = parent2;
			parent.localPosition = Vector3.zero;
			parent.localRotation = Quaternion.identity;
			parent.localScale = Vector3.one;
			vrcam.enabled = false;
			GameObject gameObject = Object.Instantiate(vrcam.gameObject);
			vrcam.enabled = true;
			gameObject.name = "camera";
			Object.DestroyImmediate(gameObject.GetComponent<SteamVR_Camera>());
			Object.DestroyImmediate(gameObject.GetComponent<SteamVR_CameraFlip>());
			cam = gameObject.GetComponent<Camera>();
			cam.fieldOfView = config.fov;
			cam.useOcclusionCulling = false;
			cam.enabled = false;
			colorMat = new Material(Shader.Find("Custom/SteamVR_ColorOut"));
			alphaMat = new Material(Shader.Find("Custom/SteamVR_AlphaOut"));
			clipMaterial = new Material(Shader.Find("Custom/SteamVR_ClearAll"));
			Transform transform = gameObject.transform;
			transform.parent = base.transform;
			transform.localPosition = new Vector3(config.x, config.y, config.z);
			transform.localRotation = Quaternion.Euler(config.rx, config.ry, config.rz);
			transform.localScale = Vector3.one;
			while (transform.childCount > 0)
			{
				Object.DestroyImmediate(transform.GetChild(0).gameObject);
			}
			clipQuad = GameObject.CreatePrimitive(PrimitiveType.Quad);
			clipQuad.name = "ClipQuad";
			Object.DestroyImmediate(clipQuad.GetComponent<MeshCollider>());
			MeshRenderer component = clipQuad.GetComponent<MeshRenderer>();
			component.material = clipMaterial;
			component.shadowCastingMode = ShadowCastingMode.Off;
			component.receiveShadows = false;
			component.lightProbeUsage = LightProbeUsage.Off;
			component.reflectionProbeUsage = ReflectionProbeUsage.Off;
			Transform transform2 = clipQuad.transform;
			transform2.parent = transform;
			transform2.localScale = new Vector3(1000f, 1000f, 1f);
			transform2.localRotation = Quaternion.identity;
			clipQuad.SetActive(false);
		}
	}

	public float GetTargetDistance()
	{
		if (target == null)
		{
			return config.near + 0.01f;
		}
		Transform transform = cam.transform;
		Vector3 vector = new Vector3(transform.forward.x, 0f, transform.forward.z);
		Vector3 normalized = vector.normalized;
		Vector3 position = target.position;
		Vector3 vector2 = new Vector3(target.forward.x, 0f, target.forward.z);
		Vector3 inPoint = position + vector2.normalized * config.hmdOffset;
		float value = 0f - new Plane(normalized, inPoint).GetDistanceToPoint(transform.position);
		return Mathf.Clamp(value, config.near + 0.01f, config.far - 0.01f);
	}

	public void RenderNear()
	{
		int num = Screen.width / 2;
		int num2 = Screen.height / 2;
		if (cam.targetTexture == null || cam.targetTexture.width != num || cam.targetTexture.height != num2)
		{
			cam.targetTexture = new RenderTexture(num, num2, 24, RenderTextureFormat.ARGB32);
			cam.targetTexture.antiAliasing = ((QualitySettings.antiAliasing == 0) ? 1 : QualitySettings.antiAliasing);
		}
		cam.nearClipPlane = config.near;
		cam.farClipPlane = config.far;
		CameraClearFlags clearFlags = cam.clearFlags;
		Color backgroundColor = cam.backgroundColor;
		cam.clearFlags = CameraClearFlags.Color;
		cam.backgroundColor = Color.clear;
		float num3 = Mathf.Clamp(GetTargetDistance() + config.nearOffset, config.near, config.far);
		Transform parent = clipQuad.transform.parent;
		clipQuad.transform.position = parent.position + parent.forward * num3;
		MonoBehaviour[] array = null;
		bool[] array2 = null;
		if (config.disableStandardAssets)
		{
			array = cam.gameObject.GetComponents<MonoBehaviour>();
			array2 = new bool[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				MonoBehaviour monoBehaviour = array[i];
				if (monoBehaviour.enabled && monoBehaviour.GetType().ToString().StartsWith("UnityStandardAssets."))
				{
					monoBehaviour.enabled = false;
					array2[i] = true;
				}
			}
		}
		clipQuad.SetActive(true);
		cam.Render();
		clipQuad.SetActive(false);
		if (array != null)
		{
			for (int j = 0; j < array.Length; j++)
			{
				if (array2[j])
				{
					array[j].enabled = true;
				}
			}
		}
		cam.clearFlags = clearFlags;
		cam.backgroundColor = backgroundColor;
		Graphics.DrawTexture(new Rect(0f, 0f, num, num2), cam.targetTexture, colorMat);
		Graphics.DrawTexture(new Rect(num, 0f, num, num2), cam.targetTexture, alphaMat);
	}

	public void RenderFar()
	{
		cam.nearClipPlane = config.near;
		cam.farClipPlane = config.far;
		cam.Render();
		int num = Screen.width / 2;
		int num2 = Screen.height / 2;
		Graphics.DrawTexture(new Rect(0f, num2, num, num2), cam.targetTexture, colorMat);
	}

	private void OnEnable()
	{
		cameras = Object.FindObjectsOfType<Camera>();
		if (cameras != null)
		{
			int num = cameras.Length;
			cameraRects = new Rect[num];
			for (int i = 0; i < num; i++)
			{
				Camera camera = cameras[i];
				cameraRects[i] = camera.rect;
				if (!(camera == cam) && !(camera.targetTexture != null) && !(camera.GetComponent<SteamVR_Camera>() != null))
				{
					camera.rect = new Rect(0.5f, 0f, 0.5f, 0.5f);
				}
			}
		}
		if (config.sceneResolutionScale > 0f)
		{
			sceneResolutionScale = SteamVR_Camera.sceneResolutionScale;
			SteamVR_Camera.sceneResolutionScale = config.sceneResolutionScale;
		}
	}

	private void OnDisable()
	{
		if (cameras != null)
		{
			int num = cameras.Length;
			for (int i = 0; i < num; i++)
			{
				Camera camera = cameras[i];
				if (camera != null)
				{
					camera.rect = cameraRects[i];
				}
			}
			cameras = null;
			cameraRects = null;
		}
		if (config.sceneResolutionScale > 0f)
		{
			SteamVR_Camera.sceneResolutionScale = sceneResolutionScale;
		}
	}

	private void OnGUI()
	{
	}
}
