using System;
using UnityEngine;

public class SignPaintingController : MonoBehaviour
{
	private const int TEXTURE_SIZE_WIDTH = 12;

	private const int TEXTURE_SIZE_HEIGHT = 8;

	private Texture2D paintTexture;

	[SerializeField]
	private MeshRenderer paintRenderer;

	[SerializeField]
	private RigidbodyEnterExitTriggerEvents paintTrigger;

	[SerializeField]
	private BoxCollider sizeBox;

	private SignPaintingBrushController currentBrush;

	private int lastX = -1;

	private int lastY = -1;

	private float cachedBoxWidth;

	private float cachedBoxHeight;

	private float worldUnitsPixelWidth;

	private float worldUnitsPixelHeight;

	private void Awake()
	{
		paintTexture = new Texture2D(12, 8, TextureFormat.ARGB32, false);
		paintTexture.filterMode = FilterMode.Point;
		Color[] array = new Color[96];
		for (int i = 0; i < 96; i++)
		{
			array[i] = Color.white;
		}
		paintTexture.SetPixels(array);
		paintTexture.Apply();
		paintRenderer.material.mainTexture = paintTexture;
		cachedBoxWidth = sizeBox.size.x;
		cachedBoxHeight = sizeBox.size.y;
		worldUnitsPixelWidth = cachedBoxWidth / 12f;
		worldUnitsPixelHeight = cachedBoxHeight / 8f;
	}

	private void OnEnable()
	{
		RigidbodyEnterExitTriggerEvents rigidbodyEnterExitTriggerEvents = paintTrigger;
		rigidbodyEnterExitTriggerEvents.OnRigidbodyEnterTrigger = (Action<Rigidbody>)Delegate.Combine(rigidbodyEnterExitTriggerEvents.OnRigidbodyEnterTrigger, new Action<Rigidbody>(RigidbodyEnter));
		RigidbodyEnterExitTriggerEvents rigidbodyEnterExitTriggerEvents2 = paintTrigger;
		rigidbodyEnterExitTriggerEvents2.OnRigidbodyExitTrigger = (Action<Rigidbody>)Delegate.Combine(rigidbodyEnterExitTriggerEvents2.OnRigidbodyExitTrigger, new Action<Rigidbody>(RigidbodyExit));
	}

	private void OnDisable()
	{
		RigidbodyEnterExitTriggerEvents rigidbodyEnterExitTriggerEvents = paintTrigger;
		rigidbodyEnterExitTriggerEvents.OnRigidbodyEnterTrigger = (Action<Rigidbody>)Delegate.Remove(rigidbodyEnterExitTriggerEvents.OnRigidbodyEnterTrigger, new Action<Rigidbody>(RigidbodyEnter));
		RigidbodyEnterExitTriggerEvents rigidbodyEnterExitTriggerEvents2 = paintTrigger;
		rigidbodyEnterExitTriggerEvents2.OnRigidbodyExitTrigger = (Action<Rigidbody>)Delegate.Remove(rigidbodyEnterExitTriggerEvents2.OnRigidbodyExitTrigger, new Action<Rigidbody>(RigidbodyExit));
	}

	private void RigidbodyEnter(Rigidbody rb)
	{
		SignPaintingBrushController component = rb.GetComponent<SignPaintingBrushController>();
		if (component != null)
		{
			currentBrush = component;
		}
		lastX = -1;
		lastY = -1;
	}

	private void RigidbodyExit(Rigidbody rb)
	{
		SignPaintingBrushController component = rb.GetComponent<SignPaintingBrushController>();
		if (component != null && currentBrush == component)
		{
			currentBrush = null;
		}
		lastX = -1;
		lastY = -1;
	}

	private void Update()
	{
		if (currentBrush != null)
		{
			PaintAt(currentBrush.PaintTip.position, currentBrush.CurrentColor);
		}
	}

	public void PaintAt(Vector3 worldPos, Color col)
	{
		if (currentBrush.HasBeenDippedOnce)
		{
			Vector3 vector = base.transform.InverseTransformPoint(worldPos);
			float num = (vector.x + worldUnitsPixelWidth / 2f + cachedBoxWidth / 2f) / cachedBoxWidth;
			float num2 = (vector.y - worldUnitsPixelHeight / 2f + cachedBoxHeight / 2f) / cachedBoxHeight;
			float x = 12f - num * 12f;
			float y = num2 * 8f;
			int num3 = Mathf.Clamp(12 - Mathf.RoundToInt(num * 12f), 0, 11);
			int num4 = Mathf.Clamp(Mathf.RoundToInt(num2 * 8f), 0, 7);
			float num5 = Vector2.Distance(new Vector2(x, y), new Vector2(num3, num4));
			if (num5 < 0.4f && (num3 != lastX || num4 != lastY))
			{
				paintTexture.SetPixel(num3, num4, currentBrush.CurrentColor);
				paintTexture.Apply();
				lastX = num3;
				lastY = num4;
				currentBrush.UsedEvent();
			}
		}
	}
}
