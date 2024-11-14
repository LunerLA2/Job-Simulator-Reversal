using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskStatusController
{
	private bool isCompleted;

	private bool isFailed;

	private bool _isSuccess;

	private bool _isSkipped;

	private TaskData taskData;

	private JobStatusController parentJobStatus;

	private int currPageIndex;

	private List<PageStatusController> pageStatusControllerList;

	public Action<TaskStatusController> OnComplete;

	public Action<TaskStatusController> OnUncomplete;

	public Action<TaskStatusController, bool> OnPageChange;

	public Action<PageStatusController> OnPageComplete;

	public Action<PageStatusController> OnPageUncomplete;

	public Action<SubtaskStatusController> OnSubtaskComplete;

	public Action<SubtaskStatusController> OnSubtaskUncomplete;

	public Action<SubtaskStatusController, bool> OnSubtaskCounterChange;

	public bool IsCompleted
	{
		get
		{
			return isCompleted;
		}
	}

	public bool IsSuccess
	{
		get
		{
			return _isSuccess;
		}
	}

	public bool IsSkipped
	{
		get
		{
			return _isSkipped;
		}
	}

	public TaskData Data
	{
		get
		{
			return taskData;
		}
	}

	public int CurrPageIndex
	{
		get
		{
			return currPageIndex;
		}
	}

	public List<PageStatusController> PageStatusControllerList
	{
		get
		{
			return pageStatusControllerList;
		}
	}

	public string TaskHeader
	{
		get
		{
			return taskData.TaskHeader;
		}
	}

	public TaskStatusController(TaskData taskData, JobStatusController parentJobStatus)
	{
		this.taskData = taskData;
		this.parentJobStatus = parentJobStatus;
		pageStatusControllerList = new List<PageStatusController>();
		List<PageData> listOfPagesForActiveRoomLayout = taskData.GetListOfPagesForActiveRoomLayout();
		for (int i = 0; i < listOfPagesForActiveRoomLayout.Count; i++)
		{
			PageStatusController pageStatusController = new PageStatusController(listOfPagesForActiveRoomLayout[i]);
			pageStatusControllerList.Add(pageStatusController);
			AddPageStatusEvents(pageStatusController);
		}
	}

	public void SetToStartingPage()
	{
		for (int i = 0; i < pageStatusControllerList.Count; i++)
		{
			if (!pageStatusControllerList[i].IsCompleted)
			{
				currPageIndex = i;
				break;
			}
			Debug.Log(pageStatusControllerList[i].Data.name + " was completed before the job started, so it is being skipped.");
		}
	}

	private void AddPageStatusEvents(PageStatusController pageStatus)
	{
		pageStatus.OnComplete = (Action<PageStatusController>)Delegate.Combine(pageStatus.OnComplete, new Action<PageStatusController>(PageComplete));
		pageStatus.OnUncomplete = (Action<PageStatusController>)Delegate.Combine(pageStatus.OnUncomplete, new Action<PageStatusController>(PageUncomplete));
		pageStatus.OnSubtaskComplete = (Action<PageStatusController, SubtaskStatusController>)Delegate.Combine(pageStatus.OnSubtaskComplete, new Action<PageStatusController, SubtaskStatusController>(SubtaskComplete));
		pageStatus.OnSubtaskUncomplete = (Action<PageStatusController, SubtaskStatusController>)Delegate.Combine(pageStatus.OnSubtaskUncomplete, new Action<PageStatusController, SubtaskStatusController>(SubtaskUncomplete));
		pageStatus.OnSubtaskCounterChange = (Action<PageStatusController, SubtaskStatusController, bool>)Delegate.Combine(pageStatus.OnSubtaskCounterChange, new Action<PageStatusController, SubtaskStatusController, bool>(SubtaskCounterChange));
	}

	public PageStatusController GetCurrentPage()
	{
		if (currPageIndex < pageStatusControllerList.Count)
		{
			return pageStatusControllerList[currPageIndex];
		}
		if (currPageIndex >= pageStatusControllerList.Count)
		{
			return pageStatusControllerList[pageStatusControllerList.Count - 1];
		}
		return null;
	}

	private void GoToNextPage()
	{
		currPageIndex++;
		if (currPageIndex >= pageStatusControllerList.Count)
		{
			if (OnPageChange != null)
			{
				OnPageChange(this, true);
			}
			if (JobBoardManager.instance.IsInEndlessMode)
			{
				JobBoardManager.instance.StartCoroutine(MarkTaskStatusAsCompleteAsync());
			}
			else
			{
				MarkTaskStatusAsComplete();
			}
		}
		else if (GetCurrentPage().IsCompleted || _isSkipped)
		{
			GoToNextPage();
		}
		else if (OnPageChange != null)
		{
			OnPageChange(this, false);
		}
	}

	private bool CheckPageSuccess()
	{
		for (int i = 0; i < pageStatusControllerList.Count; i++)
		{
			if (!pageStatusControllerList[i].IsSuccess)
			{
				return false;
			}
		}
		return true;
	}

	private bool CheckPageSkipped()
	{
		for (int i = 0; i < pageStatusControllerList.Count; i++)
		{
			if (pageStatusControllerList[i].IsSkipped)
			{
				return true;
			}
		}
		return false;
	}

	private void PageComplete(PageStatusController pageStatus)
	{
		if (pageStatus == GetCurrentPage())
		{
			if (OnPageComplete != null)
			{
				OnPageComplete(pageStatus);
			}
			GoToNextPage();
		}
	}

	private void PageUncomplete(PageStatusController pageStatus)
	{
		if (OnPageUncomplete != null)
		{
			OnPageUncomplete(pageStatus);
		}
		if (parentJobStatus != null && parentJobStatus.GetCurrentTask() != this)
		{
			MarkTaskStatusAsUncomplete();
			UpdateCurrentPageIndex();
		}
	}

	private void SubtaskComplete(PageStatusController pageStatus, SubtaskStatusController subtaskStatus)
	{
		if (OnSubtaskComplete != null)
		{
			OnSubtaskComplete(subtaskStatus);
		}
	}

	private void SubtaskUncomplete(PageStatusController pageStatus, SubtaskStatusController subtaskStatus)
	{
		if (OnSubtaskUncomplete != null)
		{
			OnSubtaskUncomplete(subtaskStatus);
		}
	}

	private void SubtaskCounterChange(PageStatusController pageStatus, SubtaskStatusController subtaskStatus, bool isPositive)
	{
		if (OnSubtaskCounterChange != null)
		{
			OnSubtaskCounterChange(subtaskStatus, isPositive);
		}
	}

	private void MarkTaskStatusAsComplete()
	{
		if (!isCompleted)
		{
			_isSuccess = CheckPageSuccess();
			_isSkipped = CheckPageSkipped();
			isCompleted = true;
			if (OnComplete != null)
			{
				OnComplete(this);
			}
		}
	}

	private IEnumerator MarkTaskStatusAsCompleteAsync()
	{
		if (!isCompleted)
		{
			_isSuccess = CheckPageSuccess();
			_isSkipped = CheckPageSkipped();
			isCompleted = true;
			if (OnComplete != null)
			{
				yield return null;
				OnComplete(this);
			}
		}
	}

	private void UpdateCurrentPageIndex()
	{
		currPageIndex = 0;
		for (int i = 0; i < pageStatusControllerList.Count && pageStatusControllerList[i].IsCompleted; i++)
		{
			currPageIndex = i + 1;
		}
	}

	private void MarkTaskStatusAsUncomplete()
	{
		if (isCompleted)
		{
			isCompleted = false;
			if (OnUncomplete != null)
			{
				OnUncomplete(this);
			}
		}
	}

	public void ForceComplete(bool success = true, bool skipped = false)
	{
		for (int i = 0; i < pageStatusControllerList.Count; i++)
		{
			pageStatusControllerList[i].ForceCompleteTasks(success, skipped);
		}
	}

	public IEnumerator ForceCompleteRoutine(bool success = true, bool skipped = false)
	{
		for (int i = 0; i < pageStatusControllerList.Count; i++)
		{
			pageStatusControllerList[i].ForceCompleteTasks(success, skipped);
			yield return null;
		}
	}

	public void ForceSkip()
	{
		if (!isCompleted)
		{
			_isSuccess = false;
			_isSkipped = true;
			ForceComplete(false, true);
		}
	}
}
