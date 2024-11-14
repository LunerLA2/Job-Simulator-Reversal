using System.Collections.Generic;
using UnityEngine;

public class GameStateController
{
	private static SaveStateData lastSaveState;

	public static void SaveState()
	{
		if (!GlobalStorage.SupportSaving)
		{
			return;
		}
		if (lastSaveState != null)
		{
			GameStateData gameStateData = GlobalStorage.Instance.GameStateData;
			for (int i = 0; i < gameStateData.JobsData.Count; i++)
			{
				JobStateData jobStateData = gameStateData.JobsData[i];
				if (lastSaveState.JobsData.Count > i)
				{
					JobSaveData jobSaveData = lastSaveState.JobsData[i];
					if (jobSaveData.ID == jobStateData.ID)
					{
						jobSaveData.SetHighestStreak(jobStateData.LongestShift);
						for (int j = 0; j < jobStateData.TasksData.Count; j++)
						{
							TaskStateData taskStateData = jobStateData.TasksData[j];
							if (jobSaveData.Tasks.Count > j)
							{
								TaskSaveData taskSaveData = jobSaveData.Tasks[j];
								if (taskSaveData.ID == taskStateData.ID)
								{
									taskSaveData.SetWasCompleted(taskStateData.IsCompleted);
									if (!string.IsNullOrEmpty(taskStateData.CustomAction) || !string.IsNullOrEmpty(taskStateData.CustomActionParam))
									{
										if (taskSaveData.CustomAction == null)
										{
											taskSaveData.SetCustomData(new CustomActionSaveData());
										}
										taskSaveData.CustomAction.SetCustomActionData(taskStateData.CustomAction, taskStateData.CustomActionParam);
									}
									continue;
								}
								Debug.LogWarning("Corrupted save state:" + taskSaveData.ID + "vs" + taskStateData.ID);
								return;
							}
							Debug.LogWarning("Corrupted save state");
							return;
						}
						continue;
					}
					Debug.LogWarning("Corrupted save state");
					return;
				}
				Debug.LogWarning("Corrupted save state");
				return;
			}
			lastSaveState.SetHasSeenGameComplete(gameStateData.HasSeenGameComplete);
			SaveLoad.Save(lastSaveState);
		}
		else
		{
			Debug.LogWarning("Save state was not created so save could be done");
		}
	}

	public static void LoadState()
	{
		GameStateData gameStateData = BuildBlankGameState();
		SaveStateData saveStateData = SaveLoad.Load();
		if (saveStateData != null)
		{
			gameStateData = ApplySaveStateToGameState(gameStateData, saveStateData);
		}
		GlobalStorage.Instance.SetGameStateData(gameStateData);
		BuildFullSaveState();
	}

	private static void BuildFullSaveState()
	{
		lastSaveState = new SaveStateData();
		GameStateData gameStateData = GlobalStorage.Instance.GameStateData;
		List<JobSaveData> list = new List<JobSaveData>();
		for (int i = 0; i < gameStateData.JobsData.Count; i++)
		{
			JobStateData jobStateData = gameStateData.JobsData[i];
			JobSaveData jobSaveData = new JobSaveData();
			jobSaveData.SetID(jobStateData.ID);
			List<TaskSaveData> list2 = new List<TaskSaveData>();
			for (int j = 0; j < jobStateData.TasksData.Count; j++)
			{
				TaskStateData taskStateData = jobStateData.TasksData[j];
				TaskSaveData taskSaveData = new TaskSaveData();
				taskSaveData.SetID(taskStateData.ID);
				list2.Add(taskSaveData);
			}
			jobSaveData.SetTasks(list2);
			jobSaveData.SetHighestStreak(0);
			list.Add(jobSaveData);
		}
		lastSaveState.SetJobsData(list);
		lastSaveState.SetHasSeenGameComplete(gameStateData.HasSeenGameComplete);
	}

	public static GameStateData BuildBlankGameState()
	{
		GameStateData gameStateData = new GameStateData();
		List<JobStateData> list = new List<JobStateData>();
		JobLevelsListData jobLevelsListData = Resources.Load<JobLevelsListData>("Data/JobLevelsList/JobLevelsListData");
		for (int i = 0; i < jobLevelsListData.ActiveJobs.Count; i++)
		{
			if (jobLevelsListData.ActiveJobs[i] != null)
			{
				JobLevelData jobLevelData = jobLevelsListData.ActiveJobs[i];
				List<TaskStateData> list2 = new List<TaskStateData>();
				for (int j = 0; j < jobLevelData.JobData.Tasks.Count; j++)
				{
					TaskStateData item = new TaskStateData(jobLevelData.JobData.Tasks[j].ID);
					list2.Add(item);
				}
				JobStateData item2 = new JobStateData(jobLevelData, list2, 0);
				list.Add(item2);
			}
		}
		gameStateData.SetJobStateData(list);
		gameStateData.SetHasSeenGameComplete(false);
		return gameStateData;
	}

	private static GameStateData ApplySaveStateToGameState(GameStateData gameStateData, SaveStateData saveStateData)
	{
		bool flag = false;
		bool flag2 = false;
		for (int i = 0; i < saveStateData.JobsData.Count; i++)
		{
			JobSaveData jobSaveData = saveStateData.JobsData[i];
			if (jobSaveData != null)
			{
				flag = false;
				for (int j = 0; j < gameStateData.JobsData.Count; j++)
				{
					JobStateData jobStateData = gameStateData.JobsData[j];
					if (!(jobStateData.ID == jobSaveData.ID))
					{
						continue;
					}
					flag = true;
					jobStateData.SetNumTasksCompleted(jobSaveData.HighestStreak);
					for (int k = 0; k < jobSaveData.Tasks.Count; k++)
					{
						flag2 = false;
						TaskSaveData taskSaveData = jobSaveData.Tasks[k];
						for (int l = 0; l < jobStateData.TasksData.Count; l++)
						{
							TaskStateData taskStateData = jobStateData.TasksData[l];
							if (taskStateData.ID == taskSaveData.ID)
							{
								flag2 = true;
								taskStateData.SetIsCompleted(taskSaveData.WasCompleted);
								if (taskSaveData.CustomAction != null)
								{
									taskStateData.SetCustomData(taskSaveData.CustomAction.ActionID, taskSaveData.CustomAction.ActionParam);
								}
								break;
							}
						}
						if (!flag2)
						{
							Debug.LogWarning("Could not find task id used in save file: Job:" + jobSaveData.ID + ", Task:" + taskSaveData.ID);
						}
					}
					break;
				}
			}
			if (!flag)
			{
				Debug.LogWarning("Could not find job id used in save file:" + jobSaveData.ID);
			}
		}
		gameStateData.SetHasSeenGameComplete(saveStateData.HasSeenGameComplete);
		return gameStateData;
	}
}
