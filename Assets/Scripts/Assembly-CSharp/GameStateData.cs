using System.Collections.Generic;

public class GameStateData
{
	public List<JobStateData> JobsData { get; private set; }

	public bool HasSeenGameComplete { get; private set; }

	public void SetHasSeenGameComplete(bool value)
	{
		HasSeenGameComplete = value;
	}

	public void SetJobStateData(List<JobStateData> jobData)
	{
		JobsData = jobData;
	}

	public JobStateData GetJobStateDataByJobData(JobData jobData)
	{
		for (int i = 0; i < JobsData.Count; i++)
		{
			if (JobsData[i].JobLevelData.JobData == jobData)
			{
				return JobsData[i];
			}
		}
		return null;
	}

	public bool HasSavedData()
	{
		foreach (JobStateData jobsDatum in JobsData)
		{
			if (jobsDatum.JobLevelData.JobData.CountsAsGameProgression && (double)jobsDatum.GetPercentageComplete() >= 0.01)
			{
				return true;
			}
		}
		return false;
	}

	public bool AllJobsComplete()
	{
		for (int i = 0; i < JobsData.Count; i++)
		{
			if (JobsData[i].JobLevelData.JobData.CountsAsGameProgression && JobsData[i].GetPercentageComplete() < 1f)
			{
				return false;
			}
		}
		return true;
	}

	public int NumberOfCompletedEndlessTasks()
	{
		int num = 0;
		for (int i = 0; i < JobsData.Count; i++)
		{
			num += JobsData[i].LongestShift;
		}
		return num;
	}
}
