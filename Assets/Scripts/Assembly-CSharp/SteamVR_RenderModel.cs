using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using UnityEngine;
using Valve.VR;

[ExecuteInEditMode]
public class SteamVR_RenderModel : MonoBehaviour
{
	public class RenderModel
	{
		public Mesh mesh { get; private set; }

		public Material material { get; private set; }

		public RenderModel(Mesh mesh, Material material)
		{
			this.mesh = mesh;
			this.material = material;
		}
	}

	public sealed class RenderModelInterfaceHolder : IDisposable
	{
		private bool needsShutdown;

		private bool failedLoadInterface;

		private CVRRenderModels _instance;

		public CVRRenderModels instance
		{
			get
			{
				if (_instance == null && !failedLoadInterface)
				{
					if (!SteamVR.active && !SteamVR.usingNativeSupport)
					{
						EVRInitError peError = EVRInitError.None;
						OpenVR.Init(ref peError, EVRApplicationType.VRApplication_Other);
						needsShutdown = true;
					}
					_instance = OpenVR.RenderModels;
					if (_instance == null)
					{
						Debug.LogError("Failed to load IVRRenderModels interface version IVRRenderModels_005");
						failedLoadInterface = true;
					}
				}
				return _instance;
			}
		}

		public void Dispose()
		{
			if (needsShutdown)
			{
				OpenVR.Shutdown();
			}
		}
	}

	public const string k_localTransformName = "attach";

	public SteamVR_TrackedObject.EIndex index = SteamVR_TrackedObject.EIndex.None;

	public string modelOverride;

	public Shader shader;

	public bool verbose;

	public bool createComponents = true;

	public bool updateDynamically = true;

	public static Hashtable models = new Hashtable();

	public static Hashtable materials = new Hashtable();

	public string renderModelName { get; private set; }

	private void OnHideRenderModels(params object[] args)
	{
		bool flag = (bool)args[0];
		MeshRenderer component = GetComponent<MeshRenderer>();
		if (component != null)
		{
			component.enabled = !flag;
		}
		MeshRenderer[] componentsInChildren = base.transform.GetComponentsInChildren<MeshRenderer>();
		foreach (MeshRenderer meshRenderer in componentsInChildren)
		{
			meshRenderer.enabled = !flag;
		}
	}

	private void OnDeviceConnected(params object[] args)
	{
		int num = (int)args[0];
		if (num == (int)index)
		{
			if ((bool)args[1])
			{
				UpdateModel();
			}
			else
			{
				StripMesh(base.gameObject);
			}
		}
	}

	public void UpdateModel()
	{
		CVRSystem system = OpenVR.System;
		if (system == null)
		{
			return;
		}
		ETrackedPropertyError pError = ETrackedPropertyError.TrackedProp_Success;
		uint stringTrackedDeviceProperty = system.GetStringTrackedDeviceProperty((uint)index, ETrackedDeviceProperty.Prop_RenderModelName_String, null, 0u, ref pError);
		if (stringTrackedDeviceProperty <= 1)
		{
			Debug.LogError("Failed to get render model name for tracked object " + index);
			return;
		}
		StringBuilder stringBuilder = new StringBuilder((int)stringTrackedDeviceProperty);
		system.GetStringTrackedDeviceProperty((uint)index, ETrackedDeviceProperty.Prop_RenderModelName_String, stringBuilder, stringTrackedDeviceProperty, ref pError);
		string text = stringBuilder.ToString();
		if (renderModelName != text)
		{
			renderModelName = text;
			StartCoroutine(SetModelAsync(text));
		}
	}

	private IEnumerator SetModelAsync(string renderModelName)
	{
		if (string.IsNullOrEmpty(renderModelName))
		{
			yield break;
		}
		using (RenderModelInterfaceHolder holder = new RenderModelInterfaceHolder())
		{
			CVRRenderModels renderModels = holder.instance;
			if (renderModels == null)
			{
				yield break;
			}
			uint count = renderModels.GetComponentCount(renderModelName);
			string[] renderModelNames;
			if (count != 0)
			{
				renderModelNames = new string[count];
				for (int i = 0; i < count; i++)
				{
					uint capacity2 = renderModels.GetComponentName(renderModelName, (uint)i, null, 0u);
					if (capacity2 == 0)
					{
						continue;
					}
					StringBuilder componentName = new StringBuilder((int)capacity2);
					if (renderModels.GetComponentName(renderModelName, (uint)i, componentName, capacity2) == 0)
					{
						continue;
					}
					capacity2 = renderModels.GetComponentRenderModelName(renderModelName, componentName.ToString(), null, 0u);
					if (capacity2 == 0)
					{
						continue;
					}
					StringBuilder name2 = new StringBuilder((int)capacity2);
					if (renderModels.GetComponentRenderModelName(renderModelName, componentName.ToString(), name2, capacity2) != 0)
					{
						string s = name2.ToString();
						RenderModel model = models[s] as RenderModel;
						if (model == null || model.mesh == null)
						{
							renderModelNames[i] = s;
						}
					}
				}
			}
			else
			{
				RenderModel model2 = models[renderModelName] as RenderModel;
				renderModelNames = ((model2 != null && !(model2.mesh == null)) ? new string[0] : new string[1] { renderModelName });
			}
			while (true)
			{
				bool loading = false;
				string[] array = renderModelNames;
				foreach (string name in array)
				{
					if (string.IsNullOrEmpty(name))
					{
						continue;
					}
					IntPtr pRenderModel = IntPtr.Zero;
					switch (renderModels.LoadRenderModel_Async(name, ref pRenderModel))
					{
					case EVRRenderModelError.Loading:
						loading = true;
						break;
					case EVRRenderModelError.None:
					{
						RenderModel_t renderModel = (RenderModel_t)Marshal.PtrToStructure(pRenderModel, typeof(RenderModel_t));
						Material material = materials[renderModel.diffuseTextureId] as Material;
						if (material == null || material.mainTexture == null)
						{
							IntPtr pDiffuseTexture = IntPtr.Zero;
							EVRRenderModelError error = renderModels.LoadTexture_Async(renderModel.diffuseTextureId, ref pDiffuseTexture);
							if (error == EVRRenderModelError.Loading)
							{
								loading = true;
							}
						}
						break;
					}
					}
				}
				if (loading)
				{
					yield return new WaitForSeconds(0.1f);
					continue;
				}
				break;
			}
		}
		bool success = SetModel(renderModelName);
		SteamVR_Utils.Event.Send("render_model_loaded", this, success);
	}

	private bool SetModel(string renderModelName)
	{
		StripMesh(base.gameObject);
		using (RenderModelInterfaceHolder renderModelInterfaceHolder = new RenderModelInterfaceHolder())
		{
			if (createComponents)
			{
				if (LoadComponents(renderModelInterfaceHolder, renderModelName))
				{
					UpdateComponents();
					return true;
				}
				Debug.Log("[" + base.gameObject.name + "] Render model does not support components, falling back to single mesh.");
			}
			if (!string.IsNullOrEmpty(renderModelName))
			{
				RenderModel renderModel = models[renderModelName] as RenderModel;
				if (renderModel == null || renderModel.mesh == null)
				{
					CVRRenderModels instance = renderModelInterfaceHolder.instance;
					if (instance == null)
					{
						return false;
					}
					if (verbose)
					{
						Debug.Log("Loading render model " + renderModelName);
					}
					renderModel = LoadRenderModel(instance, renderModelName, renderModelName);
					if (renderModel == null)
					{
						return false;
					}
					models[renderModelName] = renderModel;
				}
				base.gameObject.AddComponent<MeshFilter>().mesh = renderModel.mesh;
				base.gameObject.AddComponent<MeshRenderer>().sharedMaterial = renderModel.material;
				return true;
			}
		}
		return false;
	}

	private RenderModel LoadRenderModel(CVRRenderModels renderModels, string renderModelName, string baseName)
	{
		IntPtr ppRenderModel = IntPtr.Zero;
		while (true)
		{
			EVRRenderModelError eVRRenderModelError = renderModels.LoadRenderModel_Async(renderModelName, ref ppRenderModel);
			switch (eVRRenderModelError)
			{
			case EVRRenderModelError.Loading:
				break;
			default:
				Debug.LogError(string.Format("Failed to load render model {0} - {1}", renderModelName, eVRRenderModelError.ToString()));
				return null;
			case EVRRenderModelError.None:
			{
				RenderModel_t renderModel_t = (RenderModel_t)Marshal.PtrToStructure(ppRenderModel, typeof(RenderModel_t));
				Vector3[] array = new Vector3[renderModel_t.unVertexCount];
				Vector3[] array2 = new Vector3[renderModel_t.unVertexCount];
				Vector2[] array3 = new Vector2[renderModel_t.unVertexCount];
				Type typeFromHandle = typeof(RenderModel_Vertex_t);
				for (int i = 0; i < renderModel_t.unVertexCount; i++)
				{
					IntPtr ptr = new IntPtr(renderModel_t.rVertexData.ToInt64() + i * Marshal.SizeOf(typeFromHandle));
					RenderModel_Vertex_t renderModel_Vertex_t = (RenderModel_Vertex_t)Marshal.PtrToStructure(ptr, typeFromHandle);
					array[i] = new Vector3(renderModel_Vertex_t.vPosition.v0, renderModel_Vertex_t.vPosition.v1, 0f - renderModel_Vertex_t.vPosition.v2);
					array2[i] = new Vector3(renderModel_Vertex_t.vNormal.v0, renderModel_Vertex_t.vNormal.v1, 0f - renderModel_Vertex_t.vNormal.v2);
					array3[i] = new Vector2(renderModel_Vertex_t.rfTextureCoord0, renderModel_Vertex_t.rfTextureCoord1);
				}
				int num = (int)(renderModel_t.unTriangleCount * 3);
				short[] array4 = new short[num];
				Marshal.Copy(renderModel_t.rIndexData, array4, 0, array4.Length);
				int[] array5 = new int[num];
				for (int j = 0; j < renderModel_t.unTriangleCount; j++)
				{
					array5[j * 3] = array4[j * 3 + 2];
					array5[j * 3 + 1] = array4[j * 3 + 1];
					array5[j * 3 + 2] = array4[j * 3];
				}
				Mesh mesh = new Mesh();
				mesh.vertices = array;
				mesh.normals = array2;
				mesh.uv = array3;
				mesh.triangles = array5;
				;
				Material material = materials[renderModel_t.diffuseTextureId] as Material;
				if (material == null || material.mainTexture == null)
				{
					IntPtr ppTexture = IntPtr.Zero;
					while (true)
					{
						switch (renderModels.LoadTexture_Async(renderModel_t.diffuseTextureId, ref ppTexture))
						{
						case EVRRenderModelError.Loading:
							goto IL_0287;
						case EVRRenderModelError.None:
						{
							RenderModel_TextureMap_t renderModel_TextureMap_t = (RenderModel_TextureMap_t)Marshal.PtrToStructure(ppTexture, typeof(RenderModel_TextureMap_t));
							Texture2D texture2D = new Texture2D(renderModel_TextureMap_t.unWidth, renderModel_TextureMap_t.unHeight, TextureFormat.ARGB32, false);
							if (SteamVR.instance.graphicsAPI == EGraphicsAPIConvention.API_DirectX)
							{
								texture2D.Apply();
								while (true)
								{
									eVRRenderModelError = renderModels.LoadIntoTextureD3D11_Async(renderModel_t.diffuseTextureId, texture2D.GetNativeTexturePtr());
									if (eVRRenderModelError != EVRRenderModelError.Loading)
									{
										break;
									}
									Thread.Sleep(1);
								}
							}
							else
							{
								byte[] array6 = new byte[renderModel_TextureMap_t.unWidth * renderModel_TextureMap_t.unHeight * 4];
								Marshal.Copy(renderModel_TextureMap_t.rubTextureMapData, array6, 0, array6.Length);
								Color32[] array7 = new Color32[renderModel_TextureMap_t.unWidth * renderModel_TextureMap_t.unHeight];
								int num2 = 0;
								for (int k = 0; k < renderModel_TextureMap_t.unHeight; k++)
								{
									for (int l = 0; l < renderModel_TextureMap_t.unWidth; l++)
									{
										byte r = array6[num2++];
										byte g = array6[num2++];
										byte b = array6[num2++];
										byte a = array6[num2++];
										array7[k * renderModel_TextureMap_t.unWidth + l] = new Color32(r, g, b, a);
									}
								}
								texture2D.SetPixels32(array7);
								texture2D.Apply();
							}
							material = new Material((!(shader != null)) ? Shader.Find("Standard") : shader);
							material.mainTexture = texture2D;
							materials[renderModel_t.diffuseTextureId] = material;
							renderModels.FreeTexture(ppTexture);
							break;
						}
						default:
							Debug.Log("Failed to load render model texture for render model " + renderModelName);
							break;
						}
						break;
						IL_0287:
						Thread.Sleep(1);
					}
				}
				StartCoroutine(FreeRenderModel(ppRenderModel));
				return new RenderModel(mesh, material);
			}
			}
			Thread.Sleep(1);
		}
	}

	private IEnumerator FreeRenderModel(IntPtr pRenderModel)
	{
		yield return new WaitForSeconds(1f);
		using (RenderModelInterfaceHolder holder = new RenderModelInterfaceHolder())
		{
			CVRRenderModels renderModels = holder.instance;
			renderModels.FreeRenderModel(pRenderModel);
		}
	}

	public Transform FindComponent(string componentName)
	{
		Transform transform = base.transform;
		for (int i = 0; i < transform.childCount; i++)
		{
			Transform child = transform.GetChild(i);
			if (child.name == componentName)
			{
				return child;
			}
		}
		return null;
	}

	private void StripMesh(GameObject go)
	{
		MeshRenderer component = go.GetComponent<MeshRenderer>();
		if (component != null)
		{
			UnityEngine.Object.DestroyImmediate(component);
		}
		MeshFilter component2 = go.GetComponent<MeshFilter>();
		if (component2 != null)
		{
			UnityEngine.Object.DestroyImmediate(component2);
		}
	}

	private bool LoadComponents(RenderModelInterfaceHolder holder, string renderModelName)
	{
		Transform transform = base.transform;
		for (int i = 0; i < transform.childCount; i++)
		{
			Transform child = transform.GetChild(i);
			child.gameObject.SetActive(false);
			StripMesh(child.gameObject);
		}
		if (string.IsNullOrEmpty(renderModelName))
		{
			return true;
		}
		CVRRenderModels instance = holder.instance;
		if (instance == null)
		{
			return false;
		}
		uint componentCount = instance.GetComponentCount(renderModelName);
		if (componentCount == 0)
		{
			return false;
		}
		for (int j = 0; j < componentCount; j++)
		{
			uint componentName = instance.GetComponentName(renderModelName, (uint)j, null, 0u);
			if (componentName == 0)
			{
				continue;
			}
			StringBuilder stringBuilder = new StringBuilder((int)componentName);
			if (instance.GetComponentName(renderModelName, (uint)j, stringBuilder, componentName) == 0)
			{
				continue;
			}
			transform = FindComponent(stringBuilder.ToString());
			if (transform != null)
			{
				transform.gameObject.SetActive(true);
			}
			else
			{
				transform = new GameObject(stringBuilder.ToString()).transform;
				transform.parent = base.transform;
				transform.gameObject.layer = base.gameObject.layer;
				Transform transform2 = new GameObject("attach").transform;
				transform2.parent = transform;
				transform2.localPosition = Vector3.zero;
				transform2.localRotation = Quaternion.identity;
				transform2.localScale = Vector3.one;
				transform2.gameObject.layer = base.gameObject.layer;
			}
			transform.localPosition = Vector3.zero;
			transform.localRotation = Quaternion.identity;
			transform.localScale = Vector3.one;
			componentName = instance.GetComponentRenderModelName(renderModelName, stringBuilder.ToString(), null, 0u);
			if (componentName == 0)
			{
				continue;
			}
			StringBuilder stringBuilder2 = new StringBuilder((int)componentName);
			if (instance.GetComponentRenderModelName(renderModelName, stringBuilder.ToString(), stringBuilder2, componentName) == 0)
			{
				continue;
			}
			RenderModel renderModel = models[stringBuilder2] as RenderModel;
			if (renderModel == null || renderModel.mesh == null)
			{
				if (verbose)
				{
					Debug.Log("Loading render model " + stringBuilder2);
				}
				renderModel = LoadRenderModel(instance, stringBuilder2.ToString(), renderModelName);
				if (renderModel == null)
				{
					continue;
				}
				models[stringBuilder2] = renderModel;
			}
			transform.gameObject.AddComponent<MeshFilter>().mesh = renderModel.mesh;
			transform.gameObject.AddComponent<MeshRenderer>().sharedMaterial = renderModel.material;
		}
		return true;
	}

	private void OnEnable()
	{
		if (!string.IsNullOrEmpty(modelOverride))
		{
			Debug.Log("Model override is really only meant to be used in the scene view for lining things up; using it at runtime is discouraged.  Use tracked device index instead to ensure the correct model is displayed for all users.");
			base.enabled = false;
			return;
		}
		CVRSystem system = OpenVR.System;
		if (system != null && system.IsTrackedDeviceConnected((uint)index))
		{
			UpdateModel();
		}
		SteamVR_Utils.Event.Listen("device_connected", OnDeviceConnected);
		SteamVR_Utils.Event.Listen("hide_render_models", OnHideRenderModels);
	}

	private void OnDisable()
	{
		SteamVR_Utils.Event.Remove("device_connected", OnDeviceConnected);
		SteamVR_Utils.Event.Remove("hide_render_models", OnHideRenderModels);
	}

	private void Update()
	{
		if (updateDynamically)
		{
			UpdateComponents();
		}
	}

	public void UpdateComponents()
	{
		Transform transform = base.transform;
		if (transform.childCount == 0)
		{
			return;
		}
		using (RenderModelInterfaceHolder renderModelInterfaceHolder = new RenderModelInterfaceHolder())
		{
			VRControllerState_t pControllerState = ((index == SteamVR_TrackedObject.EIndex.None) ? default(VRControllerState_t) : SteamVR_Controller.Input((int)index).GetState());
			for (int i = 0; i < transform.childCount; i++)
			{
				Transform child = transform.GetChild(i);
				CVRRenderModels instance = renderModelInterfaceHolder.instance;
				if (instance == null)
				{
					break;
				}
				RenderModel_ComponentState_t pComponentState = default(RenderModel_ComponentState_t);
				RenderModel_ControllerMode_State_t pState = default(RenderModel_ControllerMode_State_t);
				if (instance.GetComponentState(renderModelName, child.name, ref pControllerState, ref pState, ref pComponentState))
				{
					SteamVR_Utils.RigidTransform rigidTransform = new SteamVR_Utils.RigidTransform(pComponentState.mTrackingToComponentRenderModel);
					child.localPosition = rigidTransform.pos;
					child.localRotation = rigidTransform.rot;
					Transform transform2 = child.Find("attach");
					if (transform2 != null)
					{
						SteamVR_Utils.RigidTransform rigidTransform2 = new SteamVR_Utils.RigidTransform(pComponentState.mTrackingToComponentLocal);
						transform2.position = transform.TransformPoint(rigidTransform2.pos);
						transform2.rotation = transform.rotation * rigidTransform2.rot;
					}
					bool flag = (pComponentState.uProperties & 2) != 0;
					if (flag != child.gameObject.activeSelf)
					{
						child.gameObject.SetActive(flag);
					}
				}
			}
		}
	}

	public void SetDeviceIndex(int index)
	{
		this.index = (SteamVR_TrackedObject.EIndex)index;
		modelOverride = string.Empty;
		if (base.enabled)
		{
			UpdateModel();
		}
	}
}
