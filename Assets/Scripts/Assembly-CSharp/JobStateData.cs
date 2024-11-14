using System.Collections.Generic;

public class JobStateData
{
	public JobLevelData JobLevelData { get; private set; }

	public List<TaskStateData> TasksData { get; private set; }

	public string ID
	{
		get
		{
			return JobLevelData.ID;
		}
	}

	public int LongestShift { get; private set; }

	public JobStateData(JobLevelData jobLevelData, List<TaskStateData> taskStateDataList, int highestStreak)
	{
		JobLevelData = jobLevelData;
		TasksData = taskStateDataList;
		LongestShift = highestStreak;
	}

	public void SetNumTasksCompleted(int highestStreak)
	{
		LongestShift = highestStreak;
	}

	public float GetPercentageComplete()
	{
		int num = 0;
		int count = TasksData.Count;
		for (int i = 0; i < count; i++)
		{
			if (TasksData[i].IsCompleted)
			{
				num++;
			}
		}
		return (float)num / (float)count;
	}

	public int GetIndexOfNextUnCompletedTask()
	{
		for (int i = 0; i < TasksData.Count; i++)
		{
			if (!TasksData[i].IsCompleted)
			{
				return i;
			}
		}
		return -1;
	}
}
