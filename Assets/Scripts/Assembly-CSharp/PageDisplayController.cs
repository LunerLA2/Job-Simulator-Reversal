using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PageDisplayController : MonoBehaviour
{
	private const int MAX_NUM_OF_SUBTASK_DISPLAYS = 10;

	private const int MAX_NUM_OF_SUBTASK_DISPLAYS_ENDLESS_MODE = 4;

	[SerializeField]
	private RectTransform subtaskHolderAutoGrid;

	[SerializeField]
	private Color partiallyCompletedColor;

	[SerializeField]
	private Color fullyCompletedColor;

	[SerializeField]
	private RectTransform subtaskHolderManyToOne;

	[SerializeField]
	private GameObject manyToOneQuantityBox;

	[SerializeField]
	private Text manyToOneQuantityLabel;

	[SerializeField]
	private Image manyToOneTargetIcon;

	[SerializeField]
	private Image manyToOneTargetIcon_fill;

	[SerializeField]
	private RectTransform subtaskHolderManyToOneWithFinalStep;

	[SerializeField]
	private RectTransform manyToOneWithFinalStepFinalSubtaskHolder;

	[SerializeField]
	private Image manyToOneWithFinalStepTargetIcon;

	[SerializeField]
	private Image manyToOneWithFinalStepTargetIcon_fill;

	[SerializeField]
	private RectTransform subtaskHolderManyToFinalStep;

	[SerializeField]
	private RectTransform manyToFinalStepFinalSubtaskHolder;

	[SerializeField]
	private SubtaskDisplayController subtaskDisplayPrefab;

	[SerializeField]
	private Animation newTaskFirstPageAnimation;

	private List<SubtaskDisplayController> subtaskDisplayControllerList;

	[SerializeField]
	private Transform pageLayoutAutoGridContainer;

	[SerializeField]
	private Transform pageLayoutManyToOneContainer;

	[SerializeField]
	private Transform pageLayoutManyToOneWithFinalStepContainer;

	[SerializeField]
	private Transform pageLayoutManyToFinalStepContainer;

	private PageStatusController currPageStatus;

	public PageStatusController CurrPageStatus
	{
		get
		{
			return currPageStatus;
		}
	}

	public void Initialize()
	{
		int num = 10;
		if (JobBoardManager.instance.IsInEndlessMode)
		{
			num = 4;
		}
		subtaskDisplayControllerList = new List<SubtaskDisplayController>(num);
		for (int i = 0; i < num; i++)
		{
			SubtaskDisplayController subtaskDisplayController = UnityEngine.Object.Instantiate(subtaskDisplayPrefab);
			subtaskDisplayController.transform.SetParent(subtaskHolderAutoGrid, false);
			subtaskDisplayController.Initialize();
			if (!JobBoardManager.USE_GAMEOBJECT_ACTIVATION_FOR_CONTENTS && i >= num / 2)
			{
				subtaskDisplayController.gameObject.SetActive(false);
			}
			subtaskDisplayController.name = "Subtask:" + (i + 1);
			subtaskDisplayControllerList.Add(subtaskDisplayController);
		}
	}

	public bool IsPageCurrentlyDisplayed(PageStatusController pageStatus)
	{
		return currPageStatus == pageStatus;
	}

	public bool IsSubtaskCurrentlyDisplayed(SubtaskStatusController subtaskStatus)
	{
		for (int i = 0; i < subtaskDisplayControllerList.Count; i++)
		{
			if (subtaskDisplayControllerList[i].IsSubtaskCurrentlyDisplayed(subtaskStatus))
			{
				return true;
			}
		}
		return false;
	}

	public void SetNewPage(PageStatusController pageStatus, bool animatePageIn)
	{
		if (currPageStatus != null)
		{
			RemoveEventsFromPage(currPageStatus);
		}
		currPageStatus = pageStatus;
		int num = 0;
		int num2 = 0;
		if (currPageStatus != null)
		{
			AddEventsToPage(currPageStatus);
			num = pageStatus.SubtaskStatusControllerList.Count;
			num2 = num;
			for (int i = 0; i < pageStatus.SubtaskStatusControllerList.Count; i++)
			{
				if (pageStatus.SubtaskStatusControllerList[i].Data.SubtaskIconLayoutType == SubtaskData.SubtaskIconLayoutTypes.COMPLETELYHIDDEN)
				{
					num2--;
				}
			}
		}
		else
		{
			Debug.LogError("currPageStatus was set to null - send graeme the call stack for this please");
		}
		RectTransform rectTransform = subtaskHolderAutoGrid;
		if (pageStatus != null && pageStatus.Data != null)
		{
			if (JobBoardManager.USE_GAMEOBJECT_ACTIVATION_FOR_CONTENTS)
			{
				pageLayoutAutoGridContainer.gameObject.SetActive(pageStatus.Data.PageLayoutType == PageData.PageLayoutTypes.AUTO_GRID);
				pageLayoutManyToOneContainer.gameObject.SetActive(pageStatus.Data.PageLayoutType == PageData.PageLayoutTypes.MANY_TO_ONE);
				pageLayoutManyToOneWithFinalStepContainer.gameObject.SetActive(pageStatus.Data.PageLayoutType == PageData.PageLayoutTypes.MANY_TO_ONE_WITH_FINAL_STEP);
				pageLayoutManyToFinalStepContainer.gameObject.SetActive(pageStatus.Data.PageLayoutType == PageData.PageLayoutTypes.MANY_TO_FINAL_STEP);
			}
			else
			{
				pageLayoutAutoGridContainer.transform.SetLocalPositionZOnly((pageStatus.Data.PageLayoutType != PageData.PageLayoutTypes.AUTO_GRID) ? (-10f) : 0f);
				pageLayoutManyToOneContainer.transform.SetLocalPositionZOnly((pageStatus.Data.PageLayoutType != PageData.PageLayoutTypes.MANY_TO_ONE) ? (-10f) : 0f);
				pageLayoutManyToOneWithFinalStepContainer.transform.SetLocalPositionZOnly((pageStatus.Data.PageLayoutType != PageData.PageLayoutTypes.MANY_TO_ONE_WITH_FINAL_STEP) ? (-10f) : 0f);
				pageLayoutManyToFinalStepContainer.transform.SetLocalPositionZOnly((pageStatus.Data.PageLayoutType != PageData.PageLayoutTypes.MANY_TO_FINAL_STEP) ? (-10f) : 0f);
			}
			if (pageStatus.Data.PageLayoutType == PageData.PageLayoutTypes.AUTO_GRID)
			{
				rectTransform = subtaskHolderAutoGrid;
			}
			else if (pageStatus.Data.PageLayoutType == PageData.PageLayoutTypes.MANY_TO_ONE)
			{
				rectTransform = subtaskHolderManyToOne;
				manyToOneQuantityBox.SetActive(false);
				manyToOneTargetIcon.sprite = pageStatus.Data.LayoutDependantSpriteOne;
			}
			else if (pageStatus.Data.PageLayoutType == PageData.PageLayoutTypes.MANY_TO_ONE_WITH_FINAL_STEP)
			{
				rectTransform = subtaskHolderManyToOneWithFinalStep;
				manyToOneWithFinalStepTargetIcon.sprite = pageStatus.Data.LayoutDependantSpriteOne;
			}
			else if (pageStatus.Data.PageLayoutType == PageData.PageLayoutTypes.MANY_TO_FINAL_STEP)
			{
				rectTransform = subtaskHolderManyToFinalStep;
			}
		}
		else if (JobBoardManager.USE_GAMEOBJECT_ACTIVATION_FOR_CONTENTS)
		{
			pageLayoutAutoGridContainer.gameObject.SetActive(true);
			pageLayoutManyToOneContainer.gameObject.SetActive(false);
		}
		else
		{
			pageLayoutAutoGridContainer.transform.SetLocalPositionZOnly(0f);
			pageLayoutManyToOneContainer.transform.SetLocalPositionZOnly(-10f);
		}
		GridLayoutGroup component = rectTransform.GetComponent<GridLayoutGroup>();
		if (component != null)
		{
			int num3 = pageStatus.SubtaskStatusControllerList.Count;
			if (pageStatus.Data.PageLayoutType == PageData.PageLayoutTypes.MANY_TO_ONE_WITH_FINAL_STEP || pageStatus.Data.PageLayoutType == PageData.PageLayoutTypes.MANY_TO_FINAL_STEP)
			{
				num3--;
			}
			if (num3 > 4)
			{
				rectTransform.localScale = Vector3.one * 0.66f;
				component.constraintCount = 3;
			}
			else
			{
				rectTransform.localScale = Vector3.one;
				component.constraintCount = 2;
			}
		}
		else
		{
			rectTransform.localScale = Vector3.one;
		}
		if (pageStatus.SubtaskStatusControllerList.Count > subtaskDisplayControllerList.Count)
		{
			if (JobBoardManager.instance.IsInEndlessMode)
			{
				Debug.LogError("Trying to display a page with more than " + 4 + " subtasks, need to increase MAX_NUM_OF_SUBTASK_DISPLAYS_ENDLESS_MODE");
			}
			else
			{
				Debug.LogError("Trying to display a page with more than " + 10 + " subtasks, need to increase MAX_NUM_OF_SUBTASK_DISPLAYS");
			}
		}
		for (int j = 0; j < subtaskDisplayControllerList.Count; j++)
		{
			bool flag = false;
			if (j == pageStatus.SubtaskStatusControllerList.Count - 1)
			{
				if (pageStatus.Data.PageLayoutType == PageData.PageLayoutTypes.MANY_TO_ONE_WITH_FINAL_STEP)
				{
					rectTransform = manyToOneWithFinalStepFinalSubtaskHolder;
				}
				else if (pageStatus.Data.PageLayoutType == PageData.PageLayoutTypes.MANY_TO_FINAL_STEP)
				{
					rectTransform = manyToFinalStepFinalSubtaskHolder;
					flag = true;
				}
			}
			SubtaskDisplayController subtaskDisplayController = subtaskDisplayControllerList[j];
			if (j < num)
			{
				if (pageStatus.SubtaskStatusControllerList[j].Data.SubtaskIconLayoutType != SubtaskData.SubtaskIconLayoutTypes.COMPLETELYHIDDEN)
				{
					if (num2 == 1)
					{
						flag = true;
					}
					else if (j != pageStatus.SubtaskStatusControllerList.Count - 1 && num2 == 2 && (pageStatus.Data.PageLayoutType == PageData.PageLayoutTypes.MANY_TO_FINAL_STEP || pageStatus.Data.PageLayoutType == PageData.PageLayoutTypes.MANY_TO_ONE_WITH_FINAL_STEP))
					{
						flag = true;
					}
					subtaskDisplayController.transform.localScale = ((!flag) ? Vector3.one : (Vector3.one * 1.5f));
					if (subtaskDisplayController.transform.parent != rectTransform)
					{
						subtaskDisplayController.transform.SetParent(rectTransform, false);
					}
					subtaskDisplayController.gameObject.SetActive(true);
					subtaskDisplayController.SetNewSubtask(pageStatus.SubtaskStatusControllerList[j]);
				}
				else
				{
					subtaskDisplayController.gameObject.SetActive(false);
				}
			}
			else
			{
				subtaskDisplayController.gameObject.SetActive(false);
			}
		}
		if (animatePageIn)
		{
			base.transform.localPosition = Vector3.zero;
			newTaskFirstPageAnimation.Stop();
			newTaskFirstPageAnimation.Play();
		}
	}

	public float GetTimeToAnimateIn()
	{
		return newTaskFirstPageAnimation[newTaskFirstPageAnimation.clip.name].length;
	}

	private void AddEventsToPage(PageStatusController pageStatus)
	{
		pageStatus.OnComplete = (Action<PageStatusController>)Delegate.Combine(pageStatus.OnComplete, new Action<PageStatusController>(PageComplete));
		pageStatus.OnSubtaskComplete = (Action<PageStatusController, SubtaskStatusController>)Delegate.Combine(pageStatus.OnSubtaskComplete, new Action<PageStatusController, SubtaskStatusController>(SubtaskRefresh));
		pageStatus.OnSubtaskUncomplete = (Action<PageStatusController, SubtaskStatusController>)Delegate.Combine(pageStatus.OnSubtaskUncomplete, new Action<PageStatusController, SubtaskStatusController>(SubtaskRefresh));
		pageStatus.OnSubtaskCounterChange = (Action<PageStatusController, SubtaskStatusController, bool>)Delegate.Combine(pageStatus.OnSubtaskCounterChange, new Action<PageStatusController, SubtaskStatusController, bool>(SubtaskCounterChange));
	}

	private void RemoveEventsFromPage(PageStatusController pageStatus)
	{
		pageStatus.OnComplete = (Action<PageStatusController>)Delegate.Remove(pageStatus.OnComplete, new Action<PageStatusController>(PageComplete));
		pageStatus.OnSubtaskComplete = (Action<PageStatusController, SubtaskStatusController>)Delegate.Remove(pageStatus.OnSubtaskComplete, new Action<PageStatusController, SubtaskStatusController>(SubtaskRefresh));
		pageStatus.OnSubtaskUncomplete = (Action<PageStatusController, SubtaskStatusController>)Delegate.Remove(pageStatus.OnSubtaskUncomplete, new Action<PageStatusController, SubtaskStatusController>(SubtaskRefresh));
		pageStatus.OnSubtaskCounterChange = (Action<PageStatusController, SubtaskStatusController, bool>)Delegate.Remove(pageStatus.OnSubtaskCounterChange, new Action<PageStatusController, SubtaskStatusController, bool>(SubtaskCounterChange));
	}

	private void PageComplete(PageStatusController pageStatus)
	{
		if (pageStatus.Data.PageLayoutType == PageData.PageLayoutTypes.MANY_TO_ONE)
		{
			manyToOneTargetIcon_fill.color = fullyCompletedColor;
			manyToOneTargetIcon_fill.fillAmount = 1f;
		}
	}

	private void SubtaskCounterChange(PageStatusController pageStatus, SubtaskStatusController subtaskStatus, bool isPositive)
	{
		SubtaskRefresh(pageStatus, subtaskStatus);
	}

	private void SubtaskRefresh(PageStatusController pageStatus, SubtaskStatusController subtaskStatus)
	{
		if (pageStatus.Data.PageLayoutType == PageData.PageLayoutTypes.MANY_TO_ONE)
		{
			int num = 0;
			int num2 = 0;
			for (int i = 0; i < pageStatus.SubtaskStatusControllerList.Count; i++)
			{
				int counterSize = pageStatus.SubtaskStatusControllerList[i].Data.CounterSize;
				if (counterSize <= 1)
				{
					num2++;
					if (pageStatus.SubtaskStatusControllerList[i].IsCompleted)
					{
						num++;
					}
				}
				else
				{
					num2 += counterSize;
					num += pageStatus.SubtaskStatusControllerList[i].CurrentCount;
				}
			}
			float num3 = (float)num / (float)num2;
			manyToOneTargetIcon_fill.color = ((num3 != 1f) ? partiallyCompletedColor : fullyCompletedColor);
			manyToOneTargetIcon_fill.fillAmount = num3;
		}
		else
		{
			if (pageStatus.Data.PageLayoutType != PageData.PageLayoutTypes.MANY_TO_ONE_WITH_FINAL_STEP)
			{
				return;
			}
			int num4 = 0;
			int num5 = 0;
			for (int j = 0; j < pageStatus.SubtaskStatusControllerList.Count - 1; j++)
			{
				int counterSize2 = pageStatus.SubtaskStatusControllerList[j].Data.CounterSize;
				if (counterSize2 <= 1)
				{
					num5++;
					if (pageStatus.SubtaskStatusControllerList[j].IsCompleted)
					{
						num4++;
					}
				}
				else
				{
					num5 += counterSize2;
					num4 += pageStatus.SubtaskStatusControllerList[j].CurrentCount;
				}
			}
			float num6 = (float)num4 / (float)num5;
			manyToOneWithFinalStepTargetIcon_fill.color = ((num6 != 1f) ? partiallyCompletedColor : fullyCompletedColor);
			manyToOneWithFinalStepTargetIcon_fill.fillAmount = num6;
		}
	}

	public void ResetUIComponents()
	{
		for (int i = 0; i < subtaskDisplayControllerList.Count; i++)
		{
			subtaskDisplayControllerList[i].ResetUIComponents();
		}
		manyToOneTargetIcon_fill.fillAmount = 0f;
		manyToOneWithFinalStepTargetIcon_fill.fillAmount = 0f;
	}
}
