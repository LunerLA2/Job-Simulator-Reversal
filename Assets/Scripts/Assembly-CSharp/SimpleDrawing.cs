using UnityEngine;

public class SimpleDrawing : MonoBehaviour
{
	public enum SurfaceNormalDirection
	{
		XY = 0,
		XZ = 1,
		YZ = 2
	}

	public enum BlendMode
	{
		Normal = 0,
		Additive = 1
	}

	public Transform targetObject;

	public float scaleOfBoundsToDrawIn;

	public bool useTriggerToDraw;

	[HideInInspector]
	public Vector3 targetNormal;

	[SerializeField]
	private SurfaceNormalDirection normalDirection;

	public Texture2D targetTexture;

	[SerializeField]
	private Texture2D brushTexture;

	public Color brushColor;

	private Color[] brushPixels;

	private int brushSize;

	[SerializeField]
	private BlendMode blendMode;

	[HideInInspector]
	public Ray ray;

	[HideInInspector]
	public RaycastHit hit;

	private void Start()
	{
		SetBrushSize();
	}

	private void SetBrushSize()
	{
		brushPixels = brushTexture.GetPixels();
		if (brushTexture.width != brushTexture.height)
		{
			Debug.LogError("Brush Texture Needs To Be 1:1 Aspect Ratio");
		}
		brushSize = brushTexture.width;
	}

	private void Update()
	{
		ray.origin = base.transform.position;
		ray.direction = Physics.gravity;
	}

	public void AttemptToDraw()
	{
		if (!useTriggerToDraw)
		{
			Vector2 a = Vector2.zero;
			Vector2 b = Vector2.zero;
			switch (normalDirection)
			{
			case SurfaceNormalDirection.XY:
				a = new Vector2(base.transform.position.x, base.transform.position.y);
				b = new Vector2(targetObject.position.x, targetObject.position.y);
				break;
			case SurfaceNormalDirection.YZ:
				a = new Vector2(base.transform.position.y, base.transform.position.z);
				b = new Vector2(targetObject.position.y, targetObject.position.z);
				break;
			case SurfaceNormalDirection.XZ:
				a = new Vector2(base.transform.position.x, base.transform.position.z);
				b = new Vector2(targetObject.position.x, targetObject.position.z);
				break;
			}
			if (Vector2.Distance(a, b) < scaleOfBoundsToDrawIn)
			{
				DrawToTexture();
			}
		}
	}

	private void OnTriggerStay(Collider col)
	{
		if (useTriggerToDraw && col.transform == targetObject)
		{
			DrawToTexture();
		}
	}

	private void DrawToTexture()
	{
		if (!Physics.Raycast(ray, out hit, float.PositiveInfinity) || hit.transform != targetObject)
		{
			return;
		}
		Vector2 textureCoord = hit.textureCoord;
		textureCoord.x *= targetTexture.width;
		textureCoord.y *= targetTexture.height;
		textureCoord.x -= brushSize / 2;
		textureCoord.y -= brushSize / 2;
		int x = Mathf.FloorToInt(textureCoord.x);
		int y = Mathf.FloorToInt(textureCoord.y);
		Debug.Log("U:" + hit.textureCoord.x + " V:" + hit.textureCoord.y);
		Debug.Log("Brush Size: " + brushSize);
		Color[] pixels = targetTexture.GetPixels(x, y, brushSize, brushSize);
		Color[] pixels2 = brushTexture.GetPixels();
		for (int i = 0; i < pixels.Length; i++)
		{
			if (brushPixels[i] != Color.clear)
			{
				float a = pixels[i].a;
				pixels2[i] = brushColor;
				pixels2[i].a = brushPixels[i].a;
				if (blendMode == BlendMode.Additive)
				{
					pixels[i] = Color.Lerp(pixels[i], pixels[i] + pixels2[i], pixels2[i].a);
				}
				else
				{
					pixels[i] = Color.Lerp(pixels[i], pixels2[i], pixels2[i].a);
				}
				pixels[i].a = a + pixels2[i].a;
			}
		}
		targetTexture.SetPixels(x, y, brushSize, brushSize, pixels);
		targetTexture.Apply();
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawRay(base.transform.position, Physics.gravity * 3f);
		Gizmos.color = Color.green;
		targetNormal = Vector3.zero;
		switch (normalDirection)
		{
		case SurfaceNormalDirection.XY:
			targetNormal = Vector3.forward;
			break;
		case SurfaceNormalDirection.XZ:
			targetNormal = Vector3.up;
			break;
		case SurfaceNormalDirection.YZ:
			targetNormal = Vector3.left;
			break;
		}
		targetNormal *= scaleOfBoundsToDrawIn;
		Gizmos.DrawRay(targetObject.position, targetNormal);
		Gizmos.DrawRay(targetObject.position, -targetNormal);
	}
}
