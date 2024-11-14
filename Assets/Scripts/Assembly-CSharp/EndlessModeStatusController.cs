using System;
using System.Collections;
using System.Collections.Generic;
using OwlchemyVR;
using UnityEngine;

public class EndlessModeStatusController
{
	private const int numberOfTasksForPromotion = 5;

	private EndlessModeData data;

	private bool hasStarted;

	private TaskStatusController currentGoalStatus;

	private int goalsCompleted;

	private List<int> botVOProfileIndices;

	private int currentBotProfile;

	[HideInInspector]
	public bool readyToBeginNextGoal = true;

	public Action<JobStatusController> OnCompleted;

	public Action<TaskStatusController> OnTaskComplete;

	public Action<PageStatusController> OnPageComplete;

	public Action<SubtaskStatusController> OnSubtaskComplete;

	public Action<SubtaskStatusController> OnSubtaskUncomplete;

	public Action<SubtaskStatusController, bool> OnSubtaskCounterChange;

	private List<PageNameItemMatchedPair> currentTaskPageNameItemPairs = new List<PageNameItemMatchedPair>();

	private List<WorldItemData> lastUsedItems = new List<WorldItemData>();

	private List<int> lastPlayedTaskIndices = new List<int>();

	private int numCurrentTasksBeforeReset;

	private int numCurrentTasksBeforeResetWorldItem;

	private string tasksCSVString = "Task Name,Pages,Events,WorldItems\n";

	public EndlessModeData Data
	{
		get
		{
			return data;
		}
	}

	public JobStateData JobStateData { get; private set; }

	public int Score { get; private set; }

	public string RankName { get; private set; }

	public List<PageNameItemMatchedPair> CurrentTaskPageItemNamePairs
	{
		get
		{
			return currentTaskPageNameItemPairs;
		}
	}

	public int NumberOfTasksForPromotion
	{
		get
		{
			return 5;
		}
	}

	public event Action<TaskStatusController, int> OnGoalStart;

	public event Action<TaskStatusController, int> OnGoalComplete;

	public event Action<int> OnScoreUpdate;

	public event Action<EndlessModeStatusController> OnClear;

	public EndlessModeStatusController(EndlessModeData _data, JobStateData jobStateData)
	{
		data = _data;
		JobStateData = jobStateData;
		SetScore(jobStateData.LongestShift);
		botVOProfileIndices = new List<int>(data.BotVOProfiles.Length);
		for (int i = 0; i < data.BotVOProfiles.Length; i++)
		{
			botVOProfileIndices.Add(i);
			data.BotVOProfiles[i].Init();
		}
		ShuffleRandomBotVOProfile();
		for (int j = 0; j < data.PageTypes.Count; j++)
		{
			data.PageTypes[j].Init();
		}
	}

	public void Clear()
	{
		if (currentGoalStatus != null)
		{
			RemoveEventsFromCurrentGoal();
		}
		if (this.OnClear != null)
		{
			this.OnClear(this);
		}
	}

	public TaskStatusController GetCurrentGoal()
	{
		return currentGoalStatus;
	}

	public PageData GetCurrentPageData()
	{
		if (GetCurrentGoal() == null || GetCurrentGoal().GetCurrentPage() == null)
		{
			return null;
		}
		return GetCurrentGoal().GetCurrentPage().Data;
	}

	public string GenerateTaskRandomUnitTest(int numTasks)
	{
		string text = string.Copy(tasksCSVString);
		for (int i = 0; i < numTasks; i++)
		{
			TaskStatusController taskStatusController = BeginNextGoal();
			text += DebugLogTask(taskStatusController.Data, true);
		}
		return text;
	}

	public string DebugLogTask(TaskData task, bool generateCSV = false)
	{
		string text = string.Empty;
		string empty = string.Empty;
		empty += string.Format("<color=lightblue><b>Task: {0}</b></color>\n", task.TaskHeader);
		List<string> list = new List<string>();
		List<string> list2 = new List<string>();
		List<string> list3 = new List<string>();
		for (int i = 0; i < task.Pages.Count; i++)
		{
			PageData pageData = task.Pages[i];
			empty += string.Format("<color=yellow>Page {0}: {1}</color>\n", i, pageData.name);
			for (int j = 0; j < pageData.Subtasks.Count; j++)
			{
				SubtaskData subtaskData = pageData.Subtasks[j];
				empty += string.Format("Subtask {0}: {1}\n", j, subtaskData.name);
				list.Add(pageData.name);
				for (int k = 0; k < subtaskData.ActionEventConditions.Count; k++)
				{
					ActionEventCondition actionEventCondition = subtaskData.ActionEventConditions[k];
					empty += string.Format("Action Event {0}: {1}\n", k, actionEventCondition.ActionEventData.ActionEventName);
					if (!list2.Contains(actionEventCondition.ActionEventData.ActionEventName))
					{
						list2.Add(actionEventCondition.ActionEventData.ActionEventName);
					}
					if (!(actionEventCondition.WorldItemData1 != null) && !(actionEventCondition.WorldItemData2 != null))
					{
						continue;
					}
					empty += "<i>World Items</i>\n";
					if (actionEventCondition.WorldItemData1 != null)
					{
						empty += string.Format("<color=green>{0}</color>\n", actionEventCondition.WorldItemData1.ItemFullName);
						if (!list3.Contains(actionEventCondition.WorldItemData1.ItemFullName))
						{
							list3.Add(actionEventCondition.WorldItemData1.ItemFullName);
						}
					}
					if (actionEventCondition.WorldItemData2 != null)
					{
						empty += string.Format("<color=green>{0}</color>\n", actionEventCondition.WorldItemData2.ItemFullName);
					}
				}
			}
		}
		if (generateCSV)
		{
			text += string.Format("{0},", task.TaskHeader.Replace(",", string.Empty));
			for (int l = 0; l < list.Count; l++)
			{
				text += string.Format("{0} |", list[l].Replace(",", string.Empty));
			}
			text += ",";
			for (int m = 0; m < list2.Count; m++)
			{
				text += string.Format("{0} |", list2[m].Replace(",", string.Empty));
			}
			text += ",";
			for (int n = 0; n < list3.Count; n++)
			{
				text += string.Format("{0} |", list3[n].Replace(",", string.Empty));
			}
			text += "\n";
		}
		Debug.Log(empty);
		return text;
	}

	public TaskStatusController BeginNextGoal()
	{
		TaskData taskData = null;
		if (currentTaskPageNameItemPairs.Count > 0)
		{
			currentTaskPageNameItemPairs.Clear();
		}
		numCurrentTasksBeforeReset++;
		if (lastPlayedTaskIndices.Count >= data.NumTasksTrackedBeforeRepeat)
		{
			lastPlayedTaskIndices.RemoveAt(0);
		}
		numCurrentTasksBeforeResetWorldItem++;
		if (numCurrentTasksBeforeResetWorldItem >= data.NumTasksTrackedBeforeRepeatWorldItems)
		{
			lastUsedItems.Clear();
			numCurrentTasksBeforeResetWorldItem = 0;
		}
		switch (data.RandomizationType)
		{
		case EndlessModeTaskRandomizationType.UseTasksButRandomizeSubtaskConditions:
			taskData = GenerateTaskFromSubtaskConditions();
			break;
		case EndlessModeTaskRandomizationType.GenerateFromPages:
			taskData = data.GetNewInstanceOfBaseTaskData();
			GenerateTaskFromPages(taskData);
			break;
		case EndlessModeTaskRandomizationType.GenerateFromPagesAndRandomizeSubtaskConditions:
			taskData = data.GetNewInstanceOfBaseTaskData();
			GenerateTaskFromPagesAndSubtaskConditions(taskData);
			break;
		case EndlessModeTaskRandomizationType.GenerageFromPagesOrUseTasks:
			taskData = GenerateTaskFromPagesOrUseTasks(null);
			break;
		case EndlessModeTaskRandomizationType.None:
			Debug.LogError("Selected Endless Mode Randomization Type has not been implemented");
			break;
		default:
			Debug.LogError("Error in task random generation");
			break;
		}
		currentGoalStatus = new TaskStatusController(taskData, null);
		TaskStatusController taskStatusController = currentGoalStatus;
		taskStatusController.OnComplete = (Action<TaskStatusController>)Delegate.Combine(taskStatusController.OnComplete, new Action<TaskStatusController>(CurrentGoalCompleted));
		TaskStatusController taskStatusController2 = currentGoalStatus;
		taskStatusController2.OnPageComplete = (Action<PageStatusController>)Delegate.Combine(taskStatusController2.OnPageComplete, new Action<PageStatusController>(PageComplete));
		TaskStatusController taskStatusController3 = currentGoalStatus;
		taskStatusController3.OnSubtaskComplete = (Action<SubtaskStatusController>)Delegate.Combine(taskStatusController3.OnSubtaskComplete, new Action<SubtaskStatusController>(SubtaskComplete));
		TaskStatusController taskStatusController4 = currentGoalStatus;
		taskStatusController4.OnSubtaskUncomplete = (Action<SubtaskStatusController>)Delegate.Combine(taskStatusController4.OnSubtaskUncomplete, new Action<SubtaskStatusController>(SubtaskUncomplete));
		TaskStatusController taskStatusController5 = currentGoalStatus;
		taskStatusController5.OnSubtaskCounterChange = (Action<SubtaskStatusController, bool>)Delegate.Combine(taskStatusController5.OnSubtaskCounterChange, new Action<SubtaskStatusController, bool>(SubtaskCounterChange));
		SetRegistrationStateOfTask(currentGoalStatus, true);
		GameEventsManager.Instance.ActionOccurred("ENDLESS_GOAL_STARTED");
		if (this.OnGoalStart != null)
		{
			this.OnGoalStart(currentGoalStatus, goalsCompleted);
		}
		return currentGoalStatus;
	}

	private List<int> CalculateUnusedIndices(List<int> indices_a)
	{
		List<int> list = new List<int>();
		for (int i = 0; i < data.Goals.Count; i++)
		{
			if (!indices_a.Contains(i))
			{
				list.Add(i);
			}
		}
		return list;
	}

	private TaskData GenerateTaskFromPagesOrUseTasks(TaskData nextGoalData)
	{
		if (!(UnityEngine.Random.value < data.PercentChanceGenerateFromPages))
		{
			nextGoalData = null;
			if (data.Goals.Count == 1)
			{
				nextGoalData = data.GetGoalAtIndex(0);
			}
			else if (data.Goals.Count > 1)
			{
				List<int> list = CalculateUnusedIndices(lastPlayedTaskIndices);
				if (list.Count <= 0)
				{
					lastPlayedTaskIndices.Clear();
					for (int i = 0; i < data.Goals.Count; i++)
					{
						list.Add(i);
					}
					Debug.LogWarning("We are tracking too many tasks in overtime. Need to track fewer previous tasks.");
				}
				while (nextGoalData == null)
				{
					int index = UnityEngine.Random.Range(0, list.Count);
					lastPlayedTaskIndices.Add(list[index]);
					nextGoalData = data.GetGoalAtIndex(list[index]);
				}
			}
			for (int j = 0; j < data.RandomizedTasks.Count; j++)
			{
				if (data.RandomizedTasks[j].AssociatedTask == nextGoalData)
				{
					nextGoalData = data.RandomizedTasks[j].GetRandomizedTaskData(lastUsedItems);
					currentTaskPageNameItemPairs = data.RandomizedTasks[j].CurrentPageNameItemMatchedPairs;
				}
			}
		}
		else
		{
			nextGoalData = data.GetNewInstanceOfBaseTaskData();
			for (int k = 0; k < data.PageTypes.Count; k++)
			{
				int num = UnityEngine.Random.Range(0, 2);
				PageData pageData = null;
				if (num == 1)
				{
					pageData = data.PageTypes[k].GetRandomPage(lastUsedItems);
					if (pageData != null)
					{
						nextGoalData.Pages.Add(pageData);
						PageNameItemMatchedPair currentPageItemNamePair = data.PageTypes[k].CurrentPageItemNamePair;
						if (currentPageItemNamePair != null)
						{
							currentTaskPageNameItemPairs.Add(currentPageItemNamePair);
						}
						else
						{
							Debug.Log("couldn't add page name item name pair for some weird reason");
						}
						data.PageTypes[k].ClearPageItemNamePairs();
					}
					else
					{
						Debug.LogError("Tried to add a null page from : " + data.PageTypes[k].name);
					}
				}
				else if (nextGoalData.Pages.Count < data.MinimumNumberOfRandomPages && k >= data.PageTypes.Count - data.MinimumNumberOfRandomPages)
				{
					pageData = data.PageTypes[k].GetRandomPage(lastUsedItems);
					if (pageData != null)
					{
						nextGoalData.Pages.Add(pageData);
						PageNameItemMatchedPair currentPageItemNamePair2 = data.PageTypes[k].CurrentPageItemNamePair;
						if (currentPageItemNamePair2 != null)
						{
							currentTaskPageNameItemPairs.Add(currentPageItemNamePair2);
						}
						else
						{
							Debug.Log("couldn't add page name item name pair for some weird reason");
						}
						data.PageTypes[k].ClearPageItemNamePairs();
					}
					else
					{
						Debug.LogError("Tried to add a null page from : " + data.PageTypes[k].name);
					}
				}
				else if (num != 0)
				{
					Debug.LogError("Something went horribly wrong");
				}
			}
			for (int l = 0; l < data.RequiredPages.Length; l++)
			{
				nextGoalData.Pages.Add(data.RequiredPages[l]);
			}
		}
		return nextGoalData;
	}

	private void GenerateTaskFromPagesAndSubtaskConditions(TaskData nextGoalData)
	{
		for (int i = 0; i < data.PageTypes.Count; i++)
		{
			int num = UnityEngine.Random.Range(0, 2);
			PageData pageData = null;
			if (num == 1)
			{
				pageData = data.PageTypes[i].GetRandomPage(lastUsedItems);
				if (pageData != null)
				{
					nextGoalData.Pages.Add(pageData);
					PageNameItemMatchedPair currentPageItemNamePair = data.PageTypes[i].CurrentPageItemNamePair;
					if (currentPageItemNamePair != null)
					{
						currentTaskPageNameItemPairs.Add(currentPageItemNamePair);
					}
					data.PageTypes[i].ClearPageItemNamePairs();
				}
			}
			else if (nextGoalData.Pages.Count < data.MinimumNumberOfRandomPages && i >= data.PageTypes.Count - data.MinimumNumberOfRandomPages)
			{
				pageData = data.PageTypes[i].GetRandomPage(lastUsedItems);
				if (pageData != null)
				{
					nextGoalData.Pages.Add(pageData);
					PageNameItemMatchedPair currentPageItemNamePair2 = data.PageTypes[i].CurrentPageItemNamePair;
					if (currentPageItemNamePair2 != null)
					{
						currentTaskPageNameItemPairs.Add(currentPageItemNamePair2);
					}
					data.PageTypes[i].ClearPageItemNamePairs();
				}
			}
			else if (num != 0)
			{
			}
		}
		for (int j = 0; j < data.RequiredPages.Length; j++)
		{
			nextGoalData.Pages.Add(data.RequiredPages[j]);
		}
	}

	private TaskData GenerateTaskFromSubtaskConditions()
	{
		TaskData taskData = null;
		if (data.Goals.Count == 1)
		{
			taskData = data.Goals[0];
		}
		else if (data.Goals.Count > 1)
		{
			List<int> list = CalculateUnusedIndices(lastPlayedTaskIndices);
			if (list.Count <= 0)
			{
				lastPlayedTaskIndices.Clear();
				Debug.LogWarning("We are tracking too many tasks in overtime. Need to track fewer previous tasks.");
			}
			while (taskData == null)
			{
				int index = UnityEngine.Random.Range(0, list.Count);
				lastPlayedTaskIndices.Add(list[index]);
				taskData = data.GetGoalAtIndex(list[index]);
			}
		}
		for (int i = 0; i < data.RandomizedTasks.Count; i++)
		{
			if (data.RandomizedTasks[i].AssociatedTask == taskData)
			{
				taskData = data.RandomizedTasks[i].GetRandomizedTaskData(lastUsedItems);
				currentTaskPageNameItemPairs = data.RandomizedTasks[i].CurrentPageNameItemMatchedPairs;
			}
		}
		return taskData;
	}

	private void GenerateTaskFromPages(TaskData nextGoalData)
	{
		for (int i = 0; i < data.PageTypes.Count; i++)
		{
			int num = UnityEngine.Random.Range(0, 2);
			PageData pageData = null;
			if (num == 1)
			{
				pageData = data.PageTypes[i].GetRandomPage(lastUsedItems);
				if (pageData != null)
				{
					nextGoalData.Pages.Add(pageData);
					PageNameItemMatchedPair currentPageItemNamePair = data.PageTypes[i].CurrentPageItemNamePair;
					if (currentPageItemNamePair != null)
					{
						currentTaskPageNameItemPairs.Add(currentPageItemNamePair);
					}
					data.PageTypes[i].ClearPageItemNamePairs();
				}
			}
			else if (nextGoalData.Pages.Count < data.MinimumNumberOfRandomPages && i >= data.PageTypes.Count - data.MinimumNumberOfRandomPages)
			{
				pageData = data.PageTypes[i].GetRandomPage(lastUsedItems);
				if (pageData != null)
				{
					nextGoalData.Pages.Add(pageData);
					PageNameItemMatchedPair currentPageItemNamePair2 = data.PageTypes[i].CurrentPageItemNamePair;
					if (currentPageItemNamePair2 != null)
					{
						currentTaskPageNameItemPairs.Add(currentPageItemNamePair2);
					}
					data.PageTypes[i].ClearPageItemNamePairs();
				}
			}
			else if (num != 0)
			{
			}
		}
		for (int j = 0; j < data.RequiredPages.Length; j++)
		{
			nextGoalData.Pages.Add(data.RequiredPages[j]);
		}
	}

	private void RemoveEventsFromCurrentGoal()
	{
		TaskStatusController taskStatusController = currentGoalStatus;
		taskStatusController.OnComplete = (Action<TaskStatusController>)Delegate.Remove(taskStatusController.OnComplete, new Action<TaskStatusController>(CurrentGoalCompleted));
		TaskStatusController taskStatusController2 = currentGoalStatus;
		taskStatusController2.OnPageComplete = (Action<PageStatusController>)Delegate.Remove(taskStatusController2.OnPageComplete, new Action<PageStatusController>(PageComplete));
		TaskStatusController taskStatusController3 = currentGoalStatus;
		taskStatusController3.OnSubtaskComplete = (Action<SubtaskStatusController>)Delegate.Remove(taskStatusController3.OnSubtaskComplete, new Action<SubtaskStatusController>(SubtaskComplete));
		TaskStatusController taskStatusController4 = currentGoalStatus;
		taskStatusController4.OnSubtaskUncomplete = (Action<SubtaskStatusController>)Delegate.Remove(taskStatusController4.OnSubtaskUncomplete, new Action<SubtaskStatusController>(SubtaskUncomplete));
		TaskStatusController taskStatusController5 = currentGoalStatus;
		taskStatusController5.OnSubtaskCounterChange = (Action<SubtaskStatusController, bool>)Delegate.Remove(taskStatusController5.OnSubtaskCounterChange, new Action<SubtaskStatusController, bool>(SubtaskCounterChange));
	}

	private void PageComplete(PageStatusController pageStatus)
	{
		if (OnPageComplete != null)
		{
			OnPageComplete(pageStatus);
		}
	}

	private void SubtaskComplete(SubtaskStatusController subtask)
	{
		if (OnSubtaskComplete != null)
		{
			OnSubtaskComplete(subtask);
		}
		int num = 0;
		if (subtask.Data.SubtasksToAutoComplete.Count <= 0)
		{
			return;
		}
		for (int i = 0; i < currentGoalStatus.PageStatusControllerList.Count; i++)
		{
			PageStatusController pageStatusController = currentGoalStatus.PageStatusControllerList[i];
			for (int j = 0; j < pageStatusController.SubtaskStatusControllerList.Count; j++)
			{
				SubtaskStatusController subtaskStatusController = pageStatusController.SubtaskStatusControllerList[j];
				if (subtask.Data.SubtasksToAutoComplete.Contains(subtaskStatusController.Data))
				{
					subtaskStatusController.AutoComplete(subtask.Data.DisallowUncompleteOnAutoCompletedSubtasks);
					num++;
					if (num == subtask.Data.SubtasksToAutoComplete.Count)
					{
						return;
					}
					continue;
				}
				for (int k = 0; k < subtask.Data.SubtasksToAutoComplete.Count; k++)
				{
					if (subtaskStatusController.Data.name == subtask.Data.SubtasksToAutoComplete[k].name)
					{
						subtaskStatusController.AutoComplete(subtask.Data.DisallowUncompleteOnAutoCompletedSubtasks);
						num++;
						if (num == subtask.Data.SubtasksToAutoComplete.Count)
						{
							return;
						}
					}
				}
			}
		}
	}

	private void SubtaskUncomplete(SubtaskStatusController subtask)
	{
		if (OnSubtaskUncomplete != null)
		{
			OnSubtaskUncomplete(subtask);
		}
	}

	private void SubtaskCounterChange(SubtaskStatusController subtask, bool isPositive)
	{
		if (OnSubtaskCounterChange != null)
		{
			OnSubtaskCounterChange(subtask, isPositive);
		}
	}

	private void CurrentGoalCompleted(TaskStatusController goalStatus)
	{
		SetRegistrationStateOfTask(goalStatus, false);
		RemoveEventsFromCurrentGoal();
		if (!goalStatus.IsSkipped)
		{
			goalsCompleted++;
			ModifyScore(1);
			AnalyticsManager.CustomEvent("Overtime Task Complete", new Dictionary<string, object>
			{
				{
					"Task",
					goalStatus.Data.name
				},
				{ "Total Tasks", Score }
			});
		}
		else
		{
			AnalyticsManager.CustomEvent("Overtime Task Skipped", "Task", goalStatus.Data.name);
		}
		GameEventsManager.Instance.ActionOccurred("ENDLESS_GOAL_COMPLETED");
		if (this.OnGoalComplete != null)
		{
			this.OnGoalComplete(goalStatus, goalsCompleted);
		}
		if (OnTaskComplete != null)
		{
			OnTaskComplete(goalStatus);
		}
	}

	private void SetRegistrationStateOfTask(TaskStatusController task, bool shouldBeRegistered)
	{
		for (int i = 0; i < task.PageStatusControllerList.Count; i++)
		{
			PageStatusController pageStatusController = task.PageStatusControllerList[i];
			for (int j = 0; j < pageStatusController.SubtaskStatusControllerList.Count; j++)
			{
				SubtaskStatusController subtaskStatus = pageStatusController.SubtaskStatusControllerList[j];
				if (shouldBeRegistered)
				{
					JobBoardManager.instance.ManuallyRegisterSubtaskCondition(subtaskStatus);
				}
				else
				{
					JobBoardManager.instance.ManuallyUnregisterSubtaskCondition(subtaskStatus);
				}
			}
		}
	}

	private void ModifyScore(int amt)
	{
		JobBoardManager.instance.StartCoroutine(ModifyScoreAsync(amt));
	}

	private IEnumerator ModifyScoreAsync(int amt)
	{
		yield return null;
		yield return null;
		yield return null;
		yield return null;
		yield return null;
		yield return null;
		SetScore(Score + amt);
		JobStateData.SetNumTasksCompleted(Score);
		if (VRPlatform.GetCurrVRPlatformType() == VRPlatformTypes.PSVR)
		{
			if (PSVRCalibrationController.CurrentBuildType == PSVRCalibrationController.PSVRBuildType.FullGame)
			{
				SaveStatePlayerPrefsSerializer.SaveHighestStreak(JobStateData.ID, Score);
			}
		}
		else
		{
			GameStateController.SaveState();
		}
		yield return null;
		RankName = PromotionRankNameGenerator.GetRankName(GlobalStorage.Instance.GameStateData.NumberOfCompletedEndlessTasks());
		if (this.OnScoreUpdate != null)
		{
			this.OnScoreUpdate(Score);
		}
	}

	private void SetScore(int score)
	{
		Score = score;
		if (this.OnScoreUpdate != null)
		{
			this.OnScoreUpdate(Score);
		}
	}

	public void ResetStreak()
	{
		Score = 0;
		JobStateData.SetNumTasksCompleted(0);
		GameStateController.SaveState();
		RankName = PromotionRankNameGenerator.GetRankName(GlobalStorage.Instance.GameStateData.NumberOfCompletedEndlessTasks());
		if (this.OnScoreUpdate != null)
		{
			this.OnScoreUpdate(Score);
		}
	}

	public void ForceJobComplete(bool success, bool skipped)
	{
		if (success)
		{
			JobBoardManager.instance.StartCoroutine(currentGoalStatus.ForceCompleteRoutine());
		}
		else if (skipped)
		{
			currentGoalStatus.ForceSkip();
		}
		else
		{
			currentGoalStatus.ForceComplete(false);
		}
	}

	public BotVOProfileData GetRandomBotVOProfile()
	{
		BotVOProfileData botVOProfileData = null;
		if (data.BotVOProfiles.Length > 0)
		{
			if (currentBotProfile >= botVOProfileIndices.Count)
			{
				ShuffleRandomBotVOProfile();
				currentBotProfile = 0;
			}
			botVOProfileData = data.BotVOProfiles[botVOProfileIndices[currentBotProfile]];
			currentBotProfile++;
			if (botVOProfileData == null)
			{
			}
			return botVOProfileData;
		}
		return null;
	}

	private void ShuffleRandomBotVOProfile()
	{
		botVOProfileIndices.Shuffle();
	}

	public bool ShouldGetPromotion()
	{
		int num = GlobalStorage.Instance.GameStateData.NumberOfCompletedEndlessTasks();
		if (num % 5 == 0 && JobStateData.LongestShift != 0)
		{
			return true;
		}
		return false;
	}
}
