using UnityEngine;
using UnityEngine.Rendering;
using Valve.VR;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class SteamVR_CameraMask : MonoBehaviour
{
	private static Material material;

	private static Mesh[] hiddenAreaMeshes = new Mesh[2];

	private MeshFilter meshFilter;

	private void Awake()
	{
		meshFilter = GetComponent<MeshFilter>();
		if (material == null)
		{
			material = new Material(Shader.Find("Custom/SteamVR_HiddenArea"));
		}
		MeshRenderer component = GetComponent<MeshRenderer>();
		component.material = material;
		component.shadowCastingMode = ShadowCastingMode.Off;
		component.receiveShadows = false;
		component.lightProbeUsage = LightProbeUsage.Off;
		component.reflectionProbeUsage = ReflectionProbeUsage.Off;
	}

	public void Set(SteamVR vr, EVREye eye)
	{
		if (hiddenAreaMeshes[(int)eye] == null)
		{
			hiddenAreaMeshes[(int)eye] = SteamVR_Utils.CreateHiddenAreaMesh(vr.hmd.GetHiddenAreaMesh(eye), vr.textureBounds[(int)eye]);
		}
		meshFilter.mesh = hiddenAreaMeshes[(int)eye];
	}

	public void Clear()
	{
		meshFilter.mesh = null;
	}
}
