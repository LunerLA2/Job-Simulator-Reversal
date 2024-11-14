using System.Collections.Generic;
using UnityEngine;

public class PSVRSaveTest : MonoBehaviour
{
	private void Start()
	{
		GameStateData protoGameState = BuildGameState();
		SaveStateData saveStateData = SaveLoad.LoadFromPlayerPrefs(protoGameState);
		Log("HasSeenGameComplete = " + saveStateData.HasSeenGameComplete);
		foreach (JobSaveData jobsDatum in saveStateData.JobsData)
		{
			Log("JobID = " + jobsDatum.ID);
			foreach (TaskSaveData task in jobsDatum.Tasks)
			{
				Log("    TaskID = " + task.ID);
				Log("    WasCompleted = " + task.WasCompleted);
				task.SetWasCompleted(true);
			}
		}
		saveStateData.SetHasSeenGameComplete(true);
		SaveLoad.SaveToPlayerPrefs(saveStateData);
	}

	private void Update()
	{
	}

	private void Log(string message)
	{
		Debug.Log("PSVRSaveStateTest: " + message);
	}

	private static GameStateData BuildGameState()
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
}
