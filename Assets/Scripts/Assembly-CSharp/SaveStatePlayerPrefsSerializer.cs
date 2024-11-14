using UnityEngine;

public static class SaveStatePlayerPrefsSerializer
{
	private const string SEEN_GAME_COMPLETE_KEY = "HasSeenGameComplete";

	private const string TASK_COMPLETE_KEY = "TaskComplete";

	private const string HIGHEST_STREAK_KEY = "HighestStreak";

	public static void Save(SaveStateData saveState)
	{
		SetBool(saveState.HasSeenGameComplete, "HasSeenGameComplete");
		int count = saveState.JobsData.Count;
		for (int i = 0; i < count; i++)
		{
			JobSaveData jobSaveData = saveState.JobsData[i];
			PlayerPrefs.SetInt(GetPlayerPrefsKey("HighestStreak", jobSaveData.ID), jobSaveData.HighestStreak);
			int count2 = jobSaveData.Tasks.Count;
			for (int j = 0; j < count2; j++)
			{
				TaskSaveData taskSaveData = jobSaveData.Tasks[j];
				SetBool(taskSaveData.WasCompleted, "TaskComplete", jobSaveData.ID, taskSaveData.ID);
			}
		}
		PlayerPrefs.Save();
	}

	public static void SaveTaskComplete(JobStateData job, TaskStateData task, bool taskComplete)
	{
		SetBool(taskComplete, "TaskComplete", job.ID, task.ID);
		PlayerPrefs.Save();
	}

	public static void SaveHasSeenGameComplete(bool hasSeenGameComplete)
	{
		SetBool(hasSeenGameComplete, "HasSeenGameComplete");
		PlayerPrefs.Save();
	}

	public static void SaveHighestStreak(string jobID, int highestStreak)
	{
		PlayerPrefs.SetInt(GetPlayerPrefsKey("HighestStreak", jobID), highestStreak);
		PlayerPrefs.Save();
	}

	public static SaveStateData Load(GameStateData protoGameState)
	{
		SaveStateData saveStateData = new SaveStateData();
		saveStateData.SetHasSeenGameComplete(GetBool("HasSeenGameComplete"));
		int count = protoGameState.JobsData.Count;
		for (int i = 0; i < count; i++)
		{
			JobStateData jobStateData = protoGameState.JobsData[i];
			JobSaveData jobSaveData = new JobSaveData();
			jobSaveData.SetID(jobStateData.ID);
			int highestStreak = 0;
			if (PlayerPrefs.HasKey(GetPlayerPrefsKey("HighestStreak", jobStateData.ID)))
			{
				highestStreak = PlayerPrefs.GetInt(GetPlayerPrefsKey("HighestStreak", jobStateData.ID));
			}
			jobSaveData.SetHighestStreak(highestStreak);
			int count2 = jobStateData.TasksData.Count;
			for (int j = 0; j < count2; j++)
			{
				TaskStateData taskStateData = jobStateData.TasksData[j];
				TaskSaveData taskSaveData = new TaskSaveData();
				taskSaveData.SetID(taskStateData.ID);
				taskSaveData.SetWasCompleted(GetBool("TaskComplete", jobStateData.ID, taskStateData.ID));
				taskSaveData.SetCustomData(new CustomActionSaveData());
				jobSaveData.Tasks.Add(taskSaveData);
			}
			saveStateData.JobsData.Add(jobSaveData);
		}
		return saveStateData;
	}

	public static void ClearPlayerPrefs()
	{
		GameStateData gameStateData = GameStateController.BuildBlankGameState();
		PlayerPrefs.DeleteKey(GetPlayerPrefsKey("HasSeenGameComplete"));
		int count = gameStateData.JobsData.Count;
		for (int i = 0; i < count; i++)
		{
			JobStateData jobStateData = gameStateData.JobsData[i];
			PlayerPrefs.DeleteKey(GetPlayerPrefsKey("HighestStreak", jobStateData.ID));
			int count2 = jobStateData.TasksData.Count;
			for (int j = 0; j < count2; j++)
			{
				TaskStateData taskStateData = jobStateData.TasksData[j];
				PlayerPrefs.DeleteKey(GetPlayerPrefsKey("TaskComplete", jobStateData.ID, taskStateData.ID));
			}
		}
	}

	private static void SetBool(bool value, string key, string jobID = null, string taskID = null)
	{
		string playerPrefsKey = GetPlayerPrefsKey(key, jobID, taskID);
		PlayerPrefs.SetInt(playerPrefsKey, value ? 1 : 0);
	}

	private static bool GetBool(string key, string jobID = null, string taskID = null)
	{
		string playerPrefsKey = GetPlayerPrefsKey(key, jobID, taskID);
		return PlayerPrefs.HasKey(playerPrefsKey) && PlayerPrefs.GetInt(playerPrefsKey) == 1;
	}

	private static string GetPlayerPrefsKey(string key, string jobID = null, string taskID = null)
	{
		string text = "JobSimSave-";
		if (jobID != null)
		{
			text = text + jobID + "-";
		}
		if (taskID != null)
		{
			text = text + taskID + "-";
		}
		return text + key;
	}
}
