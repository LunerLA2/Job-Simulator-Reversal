using System;
using System.Collections.Generic;

public class PageStatusController
{
	private bool wasPrevCompleted;

	private bool _isSuccess = true;

	private bool _isSkipped;

	private PageData pageData;

	private List<SubtaskStatusController> subtaskStatusControllerList;

	public Action<PageStatusController> OnComplete;

	public Action<PageStatusController> OnUncomplete;

	public Action<PageStatusController, SubtaskStatusController> OnSubtaskComplete;

	public Action<PageStatusController, SubtaskStatusController> OnSubtaskUncomplete;

	public Action<PageStatusController, SubtaskStatusController, bool> OnSubtaskCounterChange;

	public bool IsSuccess
	{
		get
		{
			return _isSuccess;
		}
	}

	public bool IsCompleted
	{
		get
		{
			return AreAllSubtasksCompleted();
		}
	}

	public bool IsSkipped
	{
		get
		{
			return _isSkipped;
		}
	}

	public PageData Data
	{
		get
		{
			return pageData;
		}
	}

	public List<SubtaskStatusController> SubtaskStatusControllerList
	{
		get
		{
			return subtaskStatusControllerList;
		}
	}

	public PageStatusController(PageData pageData)
	{
		this.pageData = pageData;
		subtaskStatusControllerList = new List<SubtaskStatusController>();
		for (int i = 0; i < pageData.Subtasks.Count; i++)
		{
			SubtaskStatusController subtaskStatusController = new SubtaskStatusController(pageData.Subtasks[i]);
			subtaskStatusControllerList.Add(subtaskStatusController);
			AddSubtaskEvents(subtaskStatusController);
		}
	}

	private bool AreAllSubtasksCompleted()
	{
		for (int i = 0; i < subtaskStatusControllerList.Count; i++)
		{
			if (!subtaskStatusControllerList[i].IsCompleted)
			{
				return false;
			}
		}
		return true;
	}

	private bool CheckSubtaskSuccess()
	{
		for (int i = 0; i < subtaskStatusControllerList.Count; i++)
		{
			if (!subtaskStatusControllerList[i].isSuccess)
			{
				return false;
			}
		}
		return true;
	}

	private bool CheckSubtaskSkipped()
	{
		for (int i = 0; i < subtaskStatusControllerList.Count; i++)
		{
			if (!subtaskStatusControllerList[i].isSkipped)
			{
				return false;
			}
		}
		return true;
	}

	private void AddSubtaskEvents(SubtaskStatusController subtaskStatus)
	{
		subtaskStatus.OnComplete = (Action<SubtaskStatusController>)Delegate.Combine(subtaskStatus.OnComplete, new Action<SubtaskStatusController>(SubtaskComplete));
		subtaskStatus.OnUncomplete = (Action<SubtaskStatusController>)Delegate.Combine(subtaskStatus.OnUncomplete, new Action<SubtaskStatusController>(SubtaskUncomplete));
		subtaskStatus.OnCounterChange = (Action<SubtaskStatusController, bool>)Delegate.Combine(subtaskStatus.OnCounterChange, new Action<SubtaskStatusController, bool>(SubtaskCounterChange));
	}

	private void SubtaskComplete(SubtaskStatusController subtaskStatus)
	{
		if (OnSubtaskComplete != null)
		{
			OnSubtaskComplete(this, subtaskStatus);
		}
		if (AreAllSubtasksCompleted() && !wasPrevCompleted)
		{
			_isSuccess = CheckSubtaskSuccess();
			_isSkipped = CheckSubtaskSkipped();
			wasPrevCompleted = true;
			if (OnComplete != null)
			{
				OnComplete(this);
			}
		}
	}

	private void SubtaskUncomplete(SubtaskStatusController subtaskStatus)
	{
		if (OnSubtaskUncomplete != null)
		{
			OnSubtaskUncomplete(this, subtaskStatus);
		}
		if (wasPrevCompleted && !AreAllSubtasksCompleted())
		{
			wasPrevCompleted = false;
			if (OnUncomplete != null)
			{
				OnUncomplete(this);
			}
		}
	}

	private void SubtaskCounterChange(SubtaskStatusController subtaskStatus, bool isPositive)
	{
		if (OnSubtaskCounterChange != null)
		{
			OnSubtaskCounterChange(this, subtaskStatus, isPositive);
		}
	}

	public SubtaskStatusController GetNextUncompletedSubtask()
	{
		for (int i = 0; i < subtaskStatusControllerList.Count; i++)
		{
			if (!subtaskStatusControllerList[i].IsCompleted)
			{
				return subtaskStatusControllerList[i];
			}
		}
		return null;
	}

	public void ForceCompleteTasks(bool success = true, bool skipped = false)
	{
		for (int i = 0; i < subtaskStatusControllerList.Count; i++)
		{
			subtaskStatusControllerList[i].ForceComplete(success, skipped);
		}
	}
}
