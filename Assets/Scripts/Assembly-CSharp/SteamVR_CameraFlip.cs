using UnityEngine;

public class SteamVR_CameraFlip : MonoBehaviour
{
	private static Material blitMaterial;

	private void OnEnable()
	{
		if (blitMaterial == null)
		{
			blitMaterial = new Material(Shader.Find("Custom/SteamVR_BlitFlip"));
		}
	}

	private void OnRenderImage(RenderTexture src, RenderTexture dest)
	{
		Graphics.Blit(src, dest, blitMaterial);
	}
}
