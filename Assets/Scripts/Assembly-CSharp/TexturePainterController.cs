using System;
using UnityEngine;

public class TexturePainterController : MonoBehaviour
{
	[SerializeField]
	private Transform container;

	[SerializeField]
	private Camera targetCamera;

	[SerializeField]
	private MeshRenderer baseTextureRenderer;

	[SerializeField]
	private MeshRenderer detailTextureRenderer;

	private bool isDirty;

	private Color currentTintColor = Color.white;

	private static TexturePainterController instance;

	public Transform Container
	{
		get
		{
			return container;
		}
	}

	public Camera TargetCamera
	{
		get
		{
			return targetCamera;
		}
	}

	public Color CurrentTintColor
	{
		get
		{
			return currentTintColor;
		}
		set
		{
			SetTintColor(value);
		}
	}

	public static TexturePainterController Instance
	{
		get
		{
			if (!instance)
			{
				Debug.LogError("TexturerPainter Instance was null!");
			}
			return instance;
		}
	}

	private void Awake()
	{
		targetCamera.eventMask = 0;
		targetCamera.enabled = false;
		targetCamera.gameObject.SetActive(false);
		if (!instance)
		{
			instance = this;
		}
		if (instance != this)
		{
			UnityEngine.Object.Destroy(this);
		}
	}

	public void SaveTexture()
	{
		throw new NotImplementedException();
	}

	public void Refresh()
	{
		isDirty = true;
	}

	private void Update()
	{
		if (isDirty && Time.frameCount % 3 == 0)
		{
			isDirty = false;
			targetCamera.Render();
		}
	}

	public void SetupTextures(Texture baseTexture, Texture detailTexture)
	{
		baseTextureRenderer.material.SetTexture("_MainTex", baseTexture);
		baseTextureRenderer.material.SetTexture("_EmissionMap", baseTexture);
		detailTextureRenderer.material.SetTexture("_MainTex", detailTexture);
		detailTextureRenderer.enabled = true;
	}

	public void HideDetailTexture()
	{
		detailTextureRenderer.enabled = false;
	}

	public void SwapDetailTexture(Texture newTexture)
	{
		detailTextureRenderer.material.mainTexture = newTexture;
	}

	public void SetTintColor(Color c)
	{
		baseTextureRenderer.material.color = c;
		baseTextureRenderer.material.SetColor("_Color", c);
		baseTextureRenderer.material.SetColor("_EmissionColor", c);
		currentTintColor = c;
	}

	public void ClearDecals()
	{
		foreach (Transform item in container.transform)
		{
			UnityEngine.Object.Destroy(item.gameObject);
		}
	}
}
