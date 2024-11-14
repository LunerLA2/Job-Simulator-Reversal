using OwlchemyVR;
using UnityEngine;
using UnityEngine.UI;

public class OrderPartsComputerProgram : ComputerProgram
{
	private enum ScrollingDirection
	{
		None = 0,
		Up = 1,
		Down = 2
	}

	[SerializeField]
	private WorldItemData worldItemData;

	[SerializeField]
	private RectTransform contentPanelTransform;

	[SerializeField]
	private RectTransform scrollBarClickable;

	[SerializeField]
	private float maxScrollBarY = -73f;

	[SerializeField]
	private float minScrollBarY = -90f;

	[SerializeField]
	private float maxContentPanelY = -100f;

	[SerializeField]
	private float minContentPanelY = -847f;

	[Header("Parts")]
	[SerializeField]
	private GameObject partClickablePrefab;

	[SerializeField]
	private RectTransform partListParent;

	[SerializeField]
	private OrderablePart[] orderableParts;

	[SerializeField]
	private Transform spawnLocation;

	private ScrollingDirection scrollDirection;

	[SerializeField]
	private float scrollSpeed = 500f;

	private bool draggingScrollbar;

	private bool cursorPositionSet;

	private float initialMousePos;

	private float initialScrollbarPosition;

	public override ComputerProgramID ProgramID
	{
		get
		{
			return ComputerProgramID.OrderParts;
		}
	}

	private void OnEnable()
	{
		if (worldItemData != null)
		{
			GameEventsManager.Instance.ItemActionOccurred(worldItemData, "OPENED");
		}
	}

	private void Awake()
	{
		for (int i = 0; i < orderableParts.Length; i++)
		{
			GameObject gameObject = Object.Instantiate(partClickablePrefab);
			gameObject.GetComponent<ComputerClickable>().ManualSetHostProgram(this);
			gameObject.transform.SetParent(partListParent, false);
			gameObject.transform.SetToDefaultPosRotScale();
			gameObject.GetComponent<Image>().sprite = orderableParts[i].Sprite;
			gameObject.name = orderableParts[i].ID;
		}
	}

	private void Update()
	{
		float num = contentPanelTransform.localPosition.y;
		if (scrollDirection == ScrollingDirection.Up)
		{
			num -= scrollSpeed * Time.deltaTime;
		}
		else if (scrollDirection == ScrollingDirection.Down)
		{
			num += scrollSpeed * Time.deltaTime;
		}
		if (num != contentPanelTransform.localPosition.y)
		{
			if (num > maxContentPanelY)
			{
				num = maxContentPanelY;
			}
			if (num < minContentPanelY)
			{
				num = minContentPanelY;
			}
			contentPanelTransform.localPosition = new Vector3(contentPanelTransform.localPosition.x, num, contentPanelTransform.localPosition.z);
			float num2 = 1f - (num - minContentPanelY) / (maxContentPanelY - minContentPanelY);
			num2 = num2 * (maxScrollBarY - minScrollBarY) + minScrollBarY;
			scrollBarClickable.localPosition = new Vector3(scrollBarClickable.localPosition.x, num2, scrollBarClickable.localPosition.z);
		}
	}

	protected override bool OnMouseMove(Vector2 cursorPos)
	{
		if (draggingScrollbar)
		{
			if (!cursorPositionSet)
			{
				cursorPositionSet = true;
				initialMousePos = cursorPos.y;
				initialScrollbarPosition = scrollBarClickable.localPosition.y;
				return true;
			}
			float num = initialScrollbarPosition + (cursorPos.y - initialMousePos);
			if (num > maxScrollBarY)
			{
				num = maxScrollBarY;
			}
			if (num < minScrollBarY)
			{
				num = minScrollBarY;
			}
			scrollBarClickable.localPosition = new Vector3(scrollBarClickable.localPosition.x, num, scrollBarClickable.localPosition.z);
			float num2 = 1f - (scrollBarClickable.localPosition.y - minScrollBarY) / (maxScrollBarY - minScrollBarY);
			num2 = num2 * (maxContentPanelY - minContentPanelY) + minContentPanelY;
			contentPanelTransform.localPosition = new Vector3(contentPanelTransform.localPosition.x, num2, contentPanelTransform.localPosition.z);
		}
		return true;
	}

	protected override void OnClickableClicked(ComputerClickable clickable)
	{
		if (!(clickable != null))
		{
			return;
		}
		clickable.PlayClickEffect();
		if (clickable.name == "X")
		{
			Finish();
			return;
		}
		if (clickable.name == "UpButton")
		{
			scrollDirection = ScrollingDirection.Up;
			return;
		}
		if (clickable.name == "DownButton")
		{
			scrollDirection = ScrollingDirection.Down;
			return;
		}
		if (clickable.name == "ScrollBarClickable")
		{
			draggingScrollbar = true;
			cursorPositionSet = false;
			return;
		}
		OrderablePart orderablePart = null;
		for (int i = 0; i < orderableParts.Length; i++)
		{
			if (orderableParts[i].ID == clickable.name)
			{
				orderablePart = orderableParts[i];
				break;
			}
		}
		SpawnNewPart(orderablePart.Prefab);
	}

	protected override void OnClickableStopClick(ComputerClickable clickable)
	{
		if (clickable != null && (clickable.name == "UpButton" || clickable.name == "DownButton" || clickable.name == "ScrollBarClickable"))
		{
			scrollDirection = ScrollingDirection.None;
			draggingScrollbar = false;
			cursorPositionSet = false;
		}
	}

	private GameObject SpawnNewPart(GameObject aPrefab)
	{
		Vector3 position = new Vector3(-1.21f, 1.5f, 0.096f);
		if (spawnLocation != null)
		{
			position = spawnLocation.position;
		}
		return Object.Instantiate(aPrefab, position, Quaternion.identity) as GameObject;
	}
}
