using System;
using OwlchemyVR;
using UnityEngine;

public class ComputerCursorController : MonoBehaviour
{
	[SerializeField]
	private GrabbableItem grabbableItem;

	[SerializeField]
	private MouseController mouseController;

	[SerializeField]
	private Animation clickAnimation;

	[SerializeField]
	private Transform matchTransform;

	[SerializeField]
	private Rect thisRect;

	[SerializeField]
	private Rect matchRect;

	[SerializeField]
	private GameObject imageRoot;

	[SerializeField]
	private GameObject arrow;

	[SerializeField]
	private GameObject hand;

	public Action OnClicked;

	public Action OnClickUp;

	public Action OnMoved;

	private RectTransform rectTransform;

	private bool mouseInHand;

	private bool isMouseButtonDown;

	private ComputerCursorType cursorType;

	public ComputerCursorType CursorType
	{
		get
		{
			return cursorType;
		}
	}

	public bool IsMouseButtonDown
	{
		get
		{
			return isMouseButtonDown;
		}
	}

	public bool IsVisible
	{
		get
		{
			return imageRoot.activeSelf;
		}
	}

	private void Awake()
	{
		rectTransform = GetComponent<RectTransform>();
	}

	private void OnEnable()
	{
		if (grabbableItem != null)
		{
			GrabbableItem obj = grabbableItem;
			obj.OnGrabbed = (Action<GrabbableItem>)Delegate.Combine(obj.OnGrabbed, new Action<GrabbableItem>(Grabbed));
			GrabbableItem obj2 = grabbableItem;
			obj2.OnReleased = (Action<GrabbableItem>)Delegate.Combine(obj2.OnReleased, new Action<GrabbableItem>(Released));
			MouseController obj3 = mouseController;
			obj3.OnClicked = (Action)Delegate.Combine(obj3.OnClicked, new Action(StartedUsing));
			MouseController obj4 = mouseController;
			obj4.OnClickUp = (Action)Delegate.Combine(obj4.OnClickUp, new Action(StoppedUsing));
		}
	}

	private void OnDisable()
	{
		if (grabbableItem != null)
		{
			GrabbableItem obj = grabbableItem;
			obj.OnGrabbed = (Action<GrabbableItem>)Delegate.Remove(obj.OnGrabbed, new Action<GrabbableItem>(Grabbed));
			GrabbableItem obj2 = grabbableItem;
			obj2.OnReleased = (Action<GrabbableItem>)Delegate.Remove(obj2.OnReleased, new Action<GrabbableItem>(Released));
			MouseController obj3 = mouseController;
			obj3.OnClicked = (Action)Delegate.Remove(obj3.OnClicked, new Action(StartedUsing));
			MouseController obj4 = mouseController;
			obj4.OnClickUp = (Action)Delegate.Remove(obj4.OnClickUp, new Action(StoppedUsing));
		}
	}

	public void SetCursorType(ComputerCursorType cursorType)
	{
		this.cursorType = cursorType;
		switch (cursorType)
		{
		case ComputerCursorType.Arrow:
			arrow.SetActive(true);
			hand.SetActive(false);
			break;
		case ComputerCursorType.Hand:
			arrow.SetActive(false);
			hand.SetActive(true);
			break;
		}
	}

	public void Show()
	{
		imageRoot.SetActive(true);
	}

	public void Hide()
	{
		imageRoot.SetActive(false);
	}

	private void Update()
	{
		if (mouseInHand)
		{
			float t = Mathf.InverseLerp(matchRect.xMin, matchRect.xMax, matchTransform.localPosition.x);
			float t2 = Mathf.InverseLerp(matchRect.yMin, matchRect.yMax, matchTransform.localPosition.z);
			Vector2 vector = new Vector2(Mathf.Lerp(thisRect.xMin, thisRect.xMax, t), Mathf.Lerp(thisRect.yMin, thisRect.yMax, t2));
			if (rectTransform.anchoredPosition != vector && OnMoved != null)
			{
				OnMoved();
			}
			rectTransform.anchoredPosition = vector;
		}
	}

	private void Grabbed(GrabbableItem item)
	{
		mouseInHand = true;
		isMouseButtonDown = false;
	}

	private void Released(GrabbableItem item)
	{
		mouseInHand = false;
		isMouseButtonDown = false;
	}

	private void StartedUsing()
	{
		isMouseButtonDown = true;
		MouseClick();
	}

	private void StoppedUsing()
	{
		isMouseButtonDown = false;
		MouseClickUp();
	}

	private void MouseClick()
	{
		clickAnimation.Play();
		if (OnClicked != null)
		{
			OnClicked();
		}
	}

	private void MouseClickUp()
	{
		if (OnClickUp != null)
		{
			OnClickUp();
		}
	}
}
