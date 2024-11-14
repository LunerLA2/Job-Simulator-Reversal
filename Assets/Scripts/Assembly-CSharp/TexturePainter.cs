using System.Collections;
using System.IO;
using UnityEngine;

public class TexturePainter : MonoBehaviour
{
	public GameObject brushCursor;

	public GameObject brushContainer;

	public Camera sceneCamera;

	public Camera canvasCam;

	public Sprite cursorPaint;

	public Sprite cursorDecal;

	public RenderTexture canvasTexture;

	public Material baseMaterial;

	private Painter_BrushMode mode;

	private float brushSize = 1f;

	private Color brushColor;

	private int brushCounter;

	private int MAX_BRUSH_COUNT = 1000;

	private bool saving;

	private void Update()
	{
		brushColor = ColorSelector.GetColor();
		if (Input.GetMouseButtonDown(0))
		{
			DoAction();
		}
		UpdateBrushCursor();
	}

	private void DoAction()
	{
		if (saving)
		{
			return;
		}
		Vector3 uvWorldPosition = Vector3.zero;
		if (HitTestUVPosition(ref uvWorldPosition))
		{
			GameObject gameObject;
			if (mode == Painter_BrushMode.PAINT)
			{
				gameObject = (GameObject)Object.Instantiate(Resources.Load("TexturePainter-Instances/BrushEntity"));
				gameObject.GetComponent<SpriteRenderer>().color = brushColor;
			}
			else
			{
				gameObject = (GameObject)Object.Instantiate(Resources.Load("TexturePainter-Instances/DecalEntity"));
			}
			brushColor.a = brushSize * 2f;
			gameObject.transform.parent = brushContainer.transform;
			gameObject.transform.localPosition = uvWorldPosition;
			gameObject.transform.localScale = Vector3.one * brushSize;
		}
		brushCounter++;
		if (brushCounter >= MAX_BRUSH_COUNT)
		{
			brushCursor.SetActive(false);
			saving = true;
			Invoke("SaveTexture", 0.1f);
		}
	}

	private void UpdateBrushCursor()
	{
		Vector3 uvWorldPosition = Vector3.zero;
		if (HitTestUVPosition(ref uvWorldPosition) && !saving)
		{
			brushCursor.SetActive(true);
			brushCursor.transform.position = uvWorldPosition + brushContainer.transform.position;
		}
		else
		{
			brushCursor.SetActive(false);
		}
	}

	private bool HitTestUVPosition(ref Vector3 uvWorldPosition)
	{
		Vector3 position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0f);
		Ray ray = sceneCamera.ScreenPointToRay(position);
		RaycastHit hitInfo;
		if (Physics.Raycast(ray, out hitInfo, 200f))
		{
			MeshCollider meshCollider = hitInfo.collider as MeshCollider;
			if (meshCollider == null || meshCollider.sharedMesh == null)
			{
				return false;
			}
			Vector2 vector = new Vector2(hitInfo.textureCoord.x, hitInfo.textureCoord.y);
			uvWorldPosition.x = vector.x - canvasCam.orthographicSize;
			uvWorldPosition.y = vector.y - canvasCam.orthographicSize;
			uvWorldPosition.z = 0f;
			return true;
		}
		return false;
	}

	private void SaveTexture()
	{
		brushCounter = 0;
		RenderTexture.active = canvasTexture;
		Texture2D texture2D = new Texture2D(canvasTexture.width, canvasTexture.height, TextureFormat.RGB24, false);
		texture2D.ReadPixels(new Rect(0f, 0f, canvasTexture.width, canvasTexture.height), 0, 0);
		texture2D.Apply();
		RenderTexture.active = null;
		baseMaterial.mainTexture = texture2D;
		foreach (Transform item in brushContainer.transform)
		{
			Object.Destroy(item.gameObject);
		}
		Invoke("ShowCursor", 0.1f);
	}

	private void ShowCursor()
	{
		saving = false;
	}

	public void SetBrushMode(Painter_BrushMode brushMode)
	{
		mode = brushMode;
		brushCursor.GetComponent<SpriteRenderer>().sprite = ((brushMode != 0) ? cursorDecal : cursorPaint);
	}

	public void SetBrushSize(float newBrushSize)
	{
		brushSize = newBrushSize;
		brushCursor.transform.localScale = Vector3.one * brushSize;
	}

	private IEnumerator SaveTextureToFile(Texture2D savedTexture)
	{
		brushCounter = 0;
		string fullPath = Directory.GetCurrentDirectory() + "\\UserCanvas\\";
		string fileName = "CanvasTexture.png";
		if (!Directory.Exists(fullPath))
		{
			Directory.CreateDirectory(fullPath);
		}
		File.WriteAllBytes(bytes: savedTexture.EncodeToPNG(), path: fullPath + fileName);
		Debug.Log("<color=orange>Saved Successfully!</color>" + fullPath + fileName);
		yield return null;
	}
}
