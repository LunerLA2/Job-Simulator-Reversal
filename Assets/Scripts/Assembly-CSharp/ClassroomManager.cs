using UnityEngine;

public class ClassroomManager : MonoBehaviour
{
	[SerializeField]
	private JobData jobData;

	private void Start()
	{
		BotManager.Instance.InitializeJob(jobData);
		JobStateData jobStateData = JobBoardManager.instance.InitJob(jobData);
		SetupSceneFromJobStateData(jobStateData);
	}

	private void SetupSceneFromJobStateData(JobStateData jobStateData)
	{
		if (jobStateData == null)
		{
			Debug.LogWarning("Not setting up the scene from jobStateData because " + jobData.name + " is not included in the game.");
			return;
		}
		int currentTaskIndex = JobBoardManager.instance.GetCurrentTaskIndex();
		for (int i = 0; i < jobStateData.JobLevelData.JobData.Tasks.Count; i++)
		{
			if (i < currentTaskIndex)
			{
			}
		}
	}
}
