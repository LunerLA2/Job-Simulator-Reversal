using OwlchemyVR;
using UnityEngine;
using UnityEngine.UI;

public class PaintComputerProgram : ComputerProgram
{
	[SerializeField]
	private ComputerClickable printButton;

	[SerializeField]
	private ComputerClickable clearButton;

	[SerializeField]
	private ComputerClickable undoButton;

	[SerializeField]
	private ComputerClickable redoButton;

	[SerializeField]
	private ComputerClickable quitButton;

	[SerializeField]
	private ComputerToggleGroup palette;

	[SerializeField]
	private Color[] paletteColors;

	[SerializeField]
	private PaintPaletteNode paletteNodePrefab;

	[SerializeField]
	private RawImage pictureImage;

	[SerializeField]
	private Image drawingCursor;

	[SerializeField]
	private int pictureWidth;

	[SerializeField]
	private int pictureHeight;

	[SerializeField]
	private WorldItemData worldItemData;

	[SerializeField]
	private GameObject printPrefab;

	[SerializeField]
	private GameObject printPrefabEndless;

	[SerializeField]
	private WorldItemData printedWorldItemData;

	private PaintPaletteNode[] paletteNodes;

	private Texture2D pictureTex;

	private Rect pictureRect;

	private Color selectedColor;

	private Color[] undoPixels;

	private Color[] currentPixels;

	private Color[] redoPixels;

	private bool isDrawing;

	public override ComputerProgramID ProgramID
	{
		get
		{
			return ComputerProgramID.Paint;
		}
	}

	private void Awake()
	{
		paletteNodes = new PaintPaletteNode[paletteColors.Length];
		for (int i = 0; i < paletteColors.Length; i++)
		{
			PaintPaletteNode paintPaletteNode = Object.Instantiate(paletteNodePrefab);
			paintPaletteNode.name = paletteNodePrefab.name + "_" + (i + 1);
			paintPaletteNode.transform.SetParent(palette.transform, false);
			paintPaletteNode.transform.localScale = Vector3.one;
			paintPaletteNode.SetColor(paletteColors[i]);
			paintPaletteNode.gameObject.SetActive(true);
			paletteNodes[i] = paintPaletteNode;
		}
		palette.RefreshChoices();
		palette.SelectionChanged += OnPaletteSelectionChanged;
		paletteNodes[0].GetComponent<ComputerTogglable>().Select();
		pictureTex = new Texture2D(pictureWidth, pictureHeight);
		pictureTex.filterMode = FilterMode.Point;
		pictureTex.wrapMode = TextureWrapMode.Clamp;
		pictureImage.texture = pictureTex;
		ClearAllPixels(Color.white);
		pictureRect = pictureImage.rectTransform.rect;
		pictureRect.position = hostComputer.Canvas.transform.InverseTransformPoint(pictureImage.transform.position);
		pictureRect.position += hostComputer.Canvas.GetComponent<RectTransform>().rect.size / 2f - pictureRect.size / 2f;
		drawingCursor.rectTransform.sizeDelta = new Vector2(pictureRect.width / (float)pictureWidth, pictureRect.height / (float)pictureHeight);
	}

	private void OnDestroy()
	{
		if (pictureTex != null)
		{
			Object.Destroy(pictureTex);
		}
	}

	private void OnEnable()
	{
		GameEventsManager.Instance.ItemActionOccurred(worldItemData, "OPENED");
		if (GenieManager.DoesContainGenieMode(GlobalStorage.Instance.CurrentGenieModes, JobGenieCartridge.GenieModeTypes.EndlessMode))
		{
			ClearAllPixels(Color.white);
			undoButton.SetInteractive(false);
			redoButton.SetInteractive(false);
		}
	}

	private void OnDisable()
	{
		if (isDrawing)
		{
			EndDrawingOperation();
			isDrawing = false;
		}
		GameEventsManager.Instance.ItemActionOccurred(worldItemData, "CLOSED");
	}

	private void Update()
	{
		if (printButton.IsInteractive && hostComputer.IsPrinterBusy)
		{
			printButton.SetInteractive(false);
			printButton.Text.text = "Printing...";
		}
		else if (!printButton.IsInteractive && !hostComputer.IsPrinterBusy)
		{
			printButton.SetInteractive(true);
			printButton.Text.text = "Print";
		}
		Vector2 relativeDrawPosition = GetRelativeDrawPosition();
		if (relativeDrawPosition.x >= 0f && relativeDrawPosition.x < 1f && relativeDrawPosition.y >= 0f && relativeDrawPosition.y < 1f)
		{
			Vector2 anchoredPosition = new Vector2((float)(int)(relativeDrawPosition.x * (float)pictureWidth) * pictureRect.width / (float)pictureWidth, (float)(int)(relativeDrawPosition.y * (float)pictureHeight) * pictureRect.height / (float)pictureHeight);
			drawingCursor.enabled = true;
			drawingCursor.rectTransform.anchoredPosition = anchoredPosition;
			if (isDrawing)
			{
				SetPixel(relativeDrawPosition.x, relativeDrawPosition.y, selectedColor);
			}
		}
		else
		{
			drawingCursor.enabled = false;
		}
	}

	protected override void OnClickableClicked(ComputerClickable clickable)
	{
		if (clickable != null)
		{
			if (clickable == printButton)
			{
				Print();
			}
			else if (clickable == clearButton)
			{
				BeginDrawingOperation();
				ClearAllPixels(Color.white);
				EndDrawingOperation();
			}
			else if (clickable == undoButton)
			{
				Undo();
			}
			else if (clickable == redoButton)
			{
				Redo();
			}
			else if (clickable == quitButton)
			{
				Finish();
			}
		}
	}

	protected override bool OnMouseClick(Vector2 cursorPos)
	{
		Vector2 relativeDrawPosition = GetRelativeDrawPosition();
		if (relativeDrawPosition.x >= 0f && relativeDrawPosition.x < 1f && relativeDrawPosition.y >= 0f && relativeDrawPosition.y < 1f)
		{
			BeginDrawingOperation();
			isDrawing = true;
		}
		return true;
	}

	protected override bool OnMouseClickUp(Vector2 cursorPos)
	{
		if (isDrawing)
		{
			EndDrawingOperation();
			isDrawing = false;
		}
		return true;
	}

	private Vector2 GetRelativeDrawPosition()
	{
		Vector2 result = hostComputer.CursorPosition - pictureRect.min;
		result.x /= pictureRect.width;
		result.y /= pictureRect.height;
		return result;
	}

	private void ClearAllPixels(Color color)
	{
		Color[] array = new Color[pictureWidth * pictureHeight];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = color;
		}
		pictureTex.SetPixels(array);
		pictureTex.Apply();
		currentPixels = array;
	}

	private void SetPixel(float relX, float relY, Color color)
	{
		Color[] pixels = pictureTex.GetPixels();
		int num = Mathf.Clamp((int)(relX * (float)pictureWidth), 0, pictureWidth - 1);
		int num2 = Mathf.Clamp((int)(relY * (float)pictureHeight), 0, pictureHeight - 1);
		if (pixels[num2 * pictureWidth + num] != selectedColor)
		{
			GameEventsManager.Instance.ItemActionOccurred(worldItemData, "USED");
			pixels[num2 * pictureWidth + num] = selectedColor;
			pictureTex.SetPixels(pixels);
			pictureTex.Apply();
			currentPixels = pixels;
		}
	}

	private void BeginDrawingOperation()
	{
		undoPixels = currentPixels;
	}

	private void EndDrawingOperation()
	{
		redoPixels = null;
		undoButton.SetInteractive(true);
		redoButton.SetInteractive(false);
	}

	private void Undo()
	{
		redoPixels = currentPixels;
		currentPixels = undoPixels;
		pictureTex.SetPixels(currentPixels);
		pictureTex.Apply();
		undoPixels = null;
		undoButton.SetInteractive(false);
		redoButton.SetInteractive(true);
	}

	private void Redo()
	{
		undoPixels = currentPixels;
		currentPixels = redoPixels;
		pictureTex.SetPixels(currentPixels);
		pictureTex.Apply();
		redoPixels = null;
		undoButton.SetInteractive(true);
		redoButton.SetInteractive(false);
	}

	private void OnPaletteSelectionChanged(ComputerToggleGroup toggleGroup, ComputerTogglable selection, ComputerTogglable prevSelection)
	{
		selectedColor = paletteColors[toggleGroup.SelectionIndex];
	}

	private void Print()
	{
		Texture2D textureToPrint = hostComputer.TakeScreenshot(pictureImage.rectTransform);
		if (GenieManager.DoesContainGenieMode(GlobalStorage.Instance.CurrentGenieModes, JobGenieCartridge.GenieModeTypes.EndlessMode))
		{
			hostComputer.PrintObject(printPrefabEndless, null, delegate(GameObject go)
			{
				go.GetComponent<FramedPhoto>().SetupCustomPicture(textureToPrint, printedWorldItemData);
			});
		}
		else
		{
			hostComputer.PrintObject(printPrefab, null, delegate(GameObject go)
			{
				go.GetComponent<FramedPhoto>().SetupCustomPicture(textureToPrint, printedWorldItemData);
			});
		}
	}
}
