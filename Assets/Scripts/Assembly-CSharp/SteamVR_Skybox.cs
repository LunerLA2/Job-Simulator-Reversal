using System;
using UnityEngine;
using Valve.VR;

public class SteamVR_Skybox : MonoBehaviour
{
	public Texture front;

	public Texture back;

	public Texture left;

	public Texture right;

	public Texture top;

	public Texture bottom;

	public void SetTextureByIndex(int i, Texture t)
	{
		switch (i)
		{
		case 0:
			front = t;
			break;
		case 1:
			back = t;
			break;
		case 2:
			left = t;
			break;
		case 3:
			right = t;
			break;
		case 4:
			top = t;
			break;
		case 5:
			bottom = t;
			break;
		}
	}

	public Texture GetTextureByIndex(int i)
	{
		switch (i)
		{
		case 0:
			return front;
		case 1:
			return back;
		case 2:
			return left;
		case 3:
			return right;
		case 4:
			return top;
		case 5:
			return bottom;
		default:
			return null;
		}
	}

	public static void SetOverride(Texture front = null, Texture back = null, Texture left = null, Texture right = null, Texture top = null, Texture bottom = null)
	{
		CVRCompositor compositor = OpenVR.Compositor;
		if (compositor == null)
		{
			return;
		}
		Texture[] array = new Texture[6] { front, back, left, right, top, bottom };
		Texture_t[] array2 = new Texture_t[6];
		for (int i = 0; i < 6; i++)
		{
			array2[i].handle = ((!(array[i] != null)) ? IntPtr.Zero : array[i].GetNativeTexturePtr());
			array2[i].eType = SteamVR.instance.graphicsAPI;
			array2[i].eColorSpace = EColorSpace.Auto;
		}
		EVRCompositorError eVRCompositorError = compositor.SetSkyboxOverride(array2);
		if (eVRCompositorError != 0)
		{
			Debug.LogError("Failed to set skybox override with error: " + eVRCompositorError);
			switch (eVRCompositorError)
			{
			case EVRCompositorError.TextureIsOnWrongDevice:
				Debug.Log("Set your graphics driver to use the same video card as the headset is plugged into for Unity.");
				break;
			case EVRCompositorError.TextureUsesUnsupportedFormat:
				Debug.Log("Ensure skybox textures are not compressed and have no mipmaps.");
				break;
			}
		}
	}

	public static void ClearOverride()
	{
		CVRCompositor compositor = OpenVR.Compositor;
		if (compositor != null)
		{
			compositor.ClearSkyboxOverride();
		}
	}

	private void OnEnable()
	{
		SetOverride(front, back, left, right, top, bottom);
	}

	private void OnDisable()
	{
		ClearOverride();
	}
}
