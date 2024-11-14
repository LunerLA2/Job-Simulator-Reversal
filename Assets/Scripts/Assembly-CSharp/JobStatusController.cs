using System;
using System.Collections;
using System.Collections.Generic;
using OwlchemyVR;
using UnityEngine;

public class JobStatusController
{
	private bool isCompleted;

	private JobData jobData;

	private int currTaskIndex;

	private List<TaskStatusController> taskStatusControllerList;

	public Action<JobStatusController> OnCompleted;

	public Action<TaskStatusController> OnTaskComplete;

	public Action<PageStatusController> OnPageComplete;

	public Action<SubtaskStatusController> OnSubtaskComplete;

	public Action<SubtaskStatusController> OnSubtaskUncomplete;

	public Action<SubtaskStatusController, bool> OnSubtaskCounterChange;

	private JobStateData jobStateData;

	public bool IsCompleted
	{
		get
		{
			return isCompleted;
		}
	}

	public JobData Data
	{
		get
		{
			return jobData;
		}
	}

	public int CurrTaskIndex
	{
		get
		{
			return currTaskIndex;
		}
	}

	public List<TaskStatusController> TaskStatusControllerList
	{
		get
		{
			return taskStatusControllerList;
		}
	}

	public JobStateData JobStateData
	{
		get
		{
			return jobStateData;
		}
	}

	public JobStatusController(JobData jobData)
	{
		this.jobData = jobData;
		jobStateData = GlobalStorage.Instance.GameStateData.GetJobStateDataByJobData(jobData);
		taskStatusControllerList = new List<TaskStatusController>();
		for (int i = 0; i < jobData.Tasks.Count; i++)
		{
			TaskStatusController taskStatusController = new TaskStatusController(jobData.Tasks[i], this);
			taskStatusControllerList.Add(taskStatusController);
			AddTaskEvents(taskStatusController);
		}
	}

	public void SetToStartingTask()
	{
		for (int i = GlobalStorage.Instance.StartingTaskIndex; i < taskStatusControllerList.Count; i++)
		{
			if (!taskStatusControllerList[i].IsCompleted)
			{
				currTaskIndex = i;
				break;
			}
			Debug.Log(taskStatusControllerList[i].Data.name + " was completed before the job started, so it is being skipped.");
		}
		GetCurrentTask().SetToStartingPage();
	}

	public TaskStatusController GetCurrentTask()
	{
		if (currTaskIndex >= 0 && currTaskIndex < taskStatusControllerList.Count)
		{
			return taskStatusControllerList[currTaskIndex];
		}
		return null;
	}

	public PageData GetCurrentPageData()
	{
		TaskStatusController currentTask = GetCurrentTask();
		if (currentTask != null)
		{
			PageStatusController currentPage = currentTask.GetCurrentPage();
			if (currentPage != null)
			{
				return currentPage.Data;
			}
			return null;
		}
		return null;
	}

	private void AddTaskEvents(TaskStatusController taskStatus)
	{
		taskStatus.OnComplete = (Action<TaskStatusController>)Delegate.Combine(taskStatus.OnComplete, new Action<TaskStatusController>(TaskComplete));
		taskStatus.OnPageComplete = (Action<PageStatusController>)Delegate.Combine(taskStatus.OnPageComplete, new Action<PageStatusController>(PageComplete));
		taskStatus.OnSubtaskComplete = (Action<SubtaskStatusController>)Delegate.Combine(taskStatus.OnSubtaskComplete, new Action<SubtaskStatusController>(SubtaskComplete));
		taskStatus.OnSubtaskUncomplete = (Action<SubtaskStatusController>)Delegate.Combine(taskStatus.OnSubtaskUncomplete, new Action<SubtaskStatusController>(SubtaskUncomplete));
		taskStatus.OnSubtaskCounterChange = (Action<SubtaskStatusController, bool>)Delegate.Combine(taskStatus.OnSubtaskCounterChange, new Action<SubtaskStatusController, bool>(SubtaskCounterChange));
	}

	private void SetTaskCompletedState(int index, bool completed)
	{
		if (jobStateData != null)
		{
			jobStateData.TasksData[index].SetIsCompleted(completed);
		}
		else
		{
			Debug.LogWarning("Game state will not be saved because there is no jobStateData for " + jobData.name + " (it is probably not in the master job list)");
		}
	}

	private void TaskComplete(TaskStatusController taskStatus)
	{
		if (taskStatus == GetCurrentTask() && taskStatus != null)
		{
			AnalyticsManager.CustomEvent("Task Complete", "Task", taskStatus.Data.name);
			SetTaskCompletedState(currTaskIndex, true);
			JobBoardManager.instance.StartCoroutine(SaveRoutine(currTaskIndex));
			if (GlobalStorage.Instance.GameStateData != null)
			{
				JobStateData jobStateDataByJobData = GlobalStorage.Instance.GameStateData.GetJobStateDataByJobData(jobData);
				if (jobStateDataByJobData != null && jobStateDataByJobData.GetPercentageComplete() >= 0.99f)
				{
					string sceneName = jobStateData.JobLevelData.SceneName;
					if (sceneName.Equals("Office"))
					{
						AchievementManager.CompleteAchievement(4);
					}
					else if (sceneName.Equals("Kitchen"))
					{
						AchievementManager.CompleteAchievement(1);
					}
					else if (sceneName.Equals("ConvenienceStore"))
					{
						AchievementManager.CompleteAchievement(3);
					}
					else if (sceneName.Equals("AutoMechanic"))
					{
						AchievementManager.CompleteAchievement(2);
					}
				}
			}
			if (OnTaskComplete != null)
			{
				OnTaskComplete(taskStatus);
			}
			for (int i = currTaskIndex + 1; i < taskStatusControllerList.Count; i++)
			{
				if (!taskStatusControllerList[i].IsCompleted)
				{
					currTaskIndex = i;
					break;
				}
				SetTaskCompletedState(i, true);
				Debug.Log(taskStatusControllerList[i].Data.name + " was already completed, so it is being skipped.");
			}
			if (GlobalStorage.Instance.GameStateData.AllJobsComplete() && !GlobalStorage.Instance.GameStateData.HasSeenGameComplete)
			{
				AnalyticsManager.CustomEvent("All Jobs Complete", null);
			}
		}
		else
		{
			Debug.Log(string.Concat(taskStatus.Data, " was completed ahead of time, so it will be skipped over when it comes up."));
		}
		if (isCompleted)
		{
			return;
		}
		bool flag = true;
		for (int j = 0; j < taskStatusControllerList.Count; j++)
		{
			if (!taskStatusControllerList[j].IsCompleted)
			{
				flag = false;
			}
		}
		int num = taskStatusControllerList.IndexOf(taskStatus);
		if (flag || num == taskStatusControllerList.Count - 1)
		{
			isCompleted = true;
			if (OnCompleted != null)
			{
				OnCompleted(this);
			}
		}
	}

	private IEnumerator SaveRoutine(int currentTask)
	{
		yield return null;
		yield return null;
		yield return null;
		yield return null;
		if (VRPlatform.GetCurrVRPlatformType() == VRPlatformTypes.PSVR)
		{
			if (PSVRCalibrationController.CurrentBuildType == PSVRCalibrationController.PSVRBuildType.FullGame)
			{
				SaveStatePlayerPrefsSerializer.SaveTaskComplete(jobStateData, jobStateData.TasksData[currentTask], true);
			}
		}
		else
		{
			GameStateController.SaveState();
		}
	}

	private void PageComplete(PageStatusController pageStatus)
	{
		if (OnPageComplete != null)
		{
			string eventDataValue = ((pageStatus == null) ? "null" : pageStatus.Data.name);
			AnalyticsManager.CustomEvent("Page Complete", "Page", eventDataValue);
			OnPageComplete(pageStatus);
		}
	}

	private void SubtaskComplete(SubtaskStatusController subtaskStatus)
	{
		if (OnSubtaskComplete != null)
		{
			OnSubtaskComplete(subtaskStatus);
			string eventDataValue = ((subtaskStatus == null) ? "null" : subtaskStatus.Data.name);
			AnalyticsManager.CustomEvent("Subtask Complete", "Subtask", eventDataValue);
		}
		int num = 0;
		if (subtaskStatus.Data.SubtasksToAutoComplete.Count <= 0)
		{
			return;
		}
		for (int i = 0; i < taskStatusControllerList.Count; i++)
		{
			TaskStatusController taskStatusController = taskStatusControllerList[i];
			for (int j = 0; j < taskStatusController.PageStatusControllerList.Count; j++)
			{
				PageStatusController pageStatusController = taskStatusController.PageStatusControllerList[j];
				for (int k = 0; k < pageStatusController.SubtaskStatusControllerList.Count; k++)
				{
					SubtaskStatusController subtaskStatusController = pageStatusController.SubtaskStatusControllerList[k];
					if (subtaskStatus.Data.SubtasksToAutoComplete.Contains(subtaskStatusController.Data))
					{
						subtaskStatusController.AutoComplete(subtaskStatus.Data.DisallowUncompleteOnAutoCompletedSubtasks);
						num++;
						if (num == subtaskStatus.Data.SubtasksToAutoComplete.Count)
						{
							return;
						}
					}
				}
			}
		}
	}

	private void SubtaskUncomplete(SubtaskStatusController subtaskStatus)
	{
		if (OnSubtaskUncomplete != null)
		{
			OnSubtaskUncomplete(subtaskStatus);
		}
	}

	private void SubtaskCounterChange(SubtaskStatusController subtaskStatus, bool isPositive)
	{
		if (OnSubtaskCounterChange != null)
		{
			OnSubtaskCounterChange(subtaskStatus, isPositive);
		}
	}
}
