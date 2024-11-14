using System.Collections;
using OwlchemyVR;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class ComputerClickable : MonoBehaviour
{
	[SerializeField]
	private WorldItemData worldItemData;

	[SerializeField]
	private bool isInteractive = true;

	[SerializeField]
	private bool showHighlight = true;

	[SerializeField]
	private bool fadeNonInteractive = true;

	[SerializeField]
	private Text text;

	[SerializeField]
	private bool canHold;

	[SerializeField]
	private float lowerResGrowthFactor = 1f;

	private bool released;

	private ComputerProgram hostProgram;

	private RectTransform rectTransform;

	private CanvasGroup canvasGroup;

	private bool containsCursor;

	private Vector3 origScale = Vector3.one;

	private bool isBusy;

	private GameObject highlight;

	public ComputerProgram HostProgram
	{
		get
		{
			return hostProgram;
		}
	}

	public bool ContainsCursor
	{
		get
		{
			return containsCursor;
		}
	}

	public bool IsInteractive
	{
		get
		{
			return isInteractive;
		}
	}

	public bool ShowHighlight
	{
		get
		{
			return showHighlight;
		}
	}

	public bool IsHighlighted
	{
		get
		{
			return this == hostProgram.HostComputer.HighlightedClickable;
		}
	}

	public bool IsBusy
	{
		get
		{
			return isBusy;
		}
	}

	public Text Text
	{
		get
		{
			return text;
		}
	}

	public event ComputerClickableEventHandler CursorEntered;

	public event ComputerClickableEventHandler CursorExited;

	public event ComputerClickableEventHandler Highlighted;

	public event ComputerClickableEventHandler Unhighlighted;

	public event ComputerClickableEventHandler Clicked;

	public event ComputerClickableEventHandler ClickedUp;

	public virtual void Awake()
	{
		origScale = base.transform.localScale;
		this.rectTransform = GetComponent<RectTransform>();
		canvasGroup = GetComponent<CanvasGroup>();
		if (text == null)
		{
			for (int i = 0; i < base.transform.childCount; i++)
			{
				text = base.transform.GetChild(i).GetComponent<Text>();
				if (text != null)
				{
					break;
				}
			}
		}
		SetInteractive(isInteractive);
		highlight = new GameObject("highlight");
		RectTransform rectTransform = highlight.AddComponent<RectTransform>();
		rectTransform.SetParent(base.transform, false);
		rectTransform.localScale = Vector3.one;
		rectTransform.anchorMin = Vector2.zero;
		rectTransform.anchorMax = Vector2.one;
		rectTransform.offsetMin = Vector2.zero;
		rectTransform.offsetMax = Vector2.zero;
		Image image = highlight.AddComponent<Image>();
		image.color = new Color(0.4f, 0.5f, 1f, 0.5f);
		highlight.SetActive(false);
		if (lowerResGrowthFactor != 1f && VRPlatform.GetCurrVRPlatformType() == VRPlatformTypes.PSVR)
		{
			Vector3 localScale = this.rectTransform.localScale;
			localScale.x *= lowerResGrowthFactor;
			localScale.y *= lowerResGrowthFactor;
			this.rectTransform.localScale = localScale;
			origScale = localScale;
		}
	}

	protected virtual void OnEnable()
	{
		Transform parent = base.transform;
		do
		{
			if (parent.parent != null)
			{
				parent = parent.parent;
				hostProgram = parent.GetComponent<ComputerProgram>();
			}
		}
		while (hostProgram == null && parent.parent != null);
		if (hostProgram != null)
		{
			hostProgram.AddClickable(this);
		}
	}

	public void ManualSetHostProgram(ComputerProgram program)
	{
		hostProgram = program;
		hostProgram.AddClickable(this);
	}

	protected virtual void OnDisable()
	{
		StopClicking();
		containsCursor = false;
	}

	public void SetInteractive(bool interactive)
	{
		if (fadeNonInteractive && canvasGroup == null)
		{
			canvasGroup = GetComponent<CanvasGroup>();
		}
		isInteractive = interactive;
		if (fadeNonInteractive)
		{
			if (interactive)
			{
				canvasGroup.alpha = 1f;
			}
			else
			{
				canvasGroup.alpha = 0.25f;
			}
		}
	}

	public void SetShowHighlight(bool showHighlight)
	{
		this.showHighlight = showHighlight;
		if (!showHighlight)
		{
			highlight.SetActive(false);
		}
	}

	public void InjectCursorPosition(Vector2 cursorPos)
	{
		bool flag = RectTransformUtility.RectangleContainsScreenPoint(rectTransform, cursorPos, hostProgram.HostComputer.CanvasCamera);
		if (!containsCursor && flag)
		{
			if (this.CursorEntered != null)
			{
				this.CursorEntered(this);
			}
		}
		else if (containsCursor && !flag && this.CursorExited != null)
		{
			this.CursorExited(this);
		}
		containsCursor = flag;
		if (containsCursor)
		{
			hostProgram.HostComputer.SetHighlightedClickableCandidate(this);
		}
	}

	public void PlayClickEffect()
	{
		StopAllCoroutines();
		if (worldItemData != null)
		{
			GameEventsManager.Instance.ItemActionOccurred(worldItemData, "ACTIVATED");
		}
		StartCoroutine(ClickAsync());
	}

	public void Highlight()
	{
		if (showHighlight)
		{
			highlight.SetActive(true);
		}
		if (this.Highlighted != null)
		{
			this.Highlighted(this);
		}
	}

	public void Unhighlight()
	{
		StopClicking();
		highlight.SetActive(false);
		if (this.Unhighlighted != null)
		{
			this.Unhighlighted(this);
		}
	}

	public virtual void Click()
	{
		if (isInteractive && !isBusy)
		{
			released = false;
			StartCoroutine(ClickAsync());
			if (this.Clicked != null)
			{
				this.Clicked(this);
			}
		}
	}

	public virtual void ClickUp()
	{
		if (isInteractive)
		{
			released = true;
			if (this.ClickedUp != null)
			{
				this.ClickedUp(this);
			}
		}
	}

	private IEnumerator ClickAsync()
	{
		isBusy = true;
		origScale = base.transform.localScale;
		float size2;
		for (size2 = 1f; size2 > 0.75f; size2 -= Time.deltaTime / 0.2f)
		{
			base.transform.localScale = origScale * size2;
			yield return null;
		}
		size2 = 0.75f;
		if (canHold)
		{
			while (!released)
			{
				yield return new WaitForEndOfFrame();
			}
		}
		for (; size2 < 1f; size2 += Time.deltaTime / 0.2f)
		{
			base.transform.localScale = origScale * size2;
			yield return null;
		}
		StopClicking();
	}

	private void StopClicking()
	{
		StopAllCoroutines();
		base.transform.localScale = origScale;
		isBusy = false;
	}
}
