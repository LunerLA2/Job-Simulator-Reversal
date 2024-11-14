using System;
using UnityEngine;

[ExecuteInEditMode]
public class SteamVR_SphericalProjection : MonoBehaviour
{
	private static Material material;

	public void Set(Vector3 N, float phi0, float phi1, float theta0, float theta1, Vector3 uAxis, Vector3 uOrigin, float uScale, Vector3 vAxis, Vector3 vOrigin, float vScale)
	{
		if (material == null)
		{
			material = new Material(Shader.Find("Custom/SteamVR_SphericalProjection"));
		}
		material.SetVector("_N", new Vector4(N.x, N.y, N.z));
		material.SetFloat("_Phi0", phi0 * ((float)Math.PI / 180f));
		material.SetFloat("_Phi1", phi1 * ((float)Math.PI / 180f));
		material.SetFloat("_Theta0", theta0 * ((float)Math.PI / 180f) + (float)Math.PI / 2f);
		material.SetFloat("_Theta1", theta1 * ((float)Math.PI / 180f) + (float)Math.PI / 2f);
		material.SetVector("_UAxis", uAxis);
		material.SetVector("_VAxis", vAxis);
		material.SetVector("_UOrigin", uOrigin);
		material.SetVector("_VOrigin", vOrigin);
		material.SetFloat("_UScale", uScale);
		material.SetFloat("_VScale", vScale);
	}

	private void OnRenderImage(RenderTexture src, RenderTexture dest)
	{
		Graphics.Blit(src, dest, material);
	}
}
