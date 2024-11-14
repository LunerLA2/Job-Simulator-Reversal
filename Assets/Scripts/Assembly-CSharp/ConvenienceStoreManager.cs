using UnityEngine;

public class ConvenienceStoreManager : LevelManager
{
	private void Start()
	{
		bool flag = false;
		if (GenieManager.DoesContainGenieMode(GlobalStorage.Instance.CurrentGenieModes, JobGenieCartridge.GenieModeTypes.EndlessMode))
		{
			flag = true;
		}
		if (flag)
		{
			JobStatusController jobStatusController = new JobStatusController(jobData);
			BotManager.Instance.InitializeEndlessMode(endlessModeConfigData);
			JobBoardManager.instance.InitEndlessMode(endlessModeConfigData, jobStatusController.JobStateData);
			return;
		}
		BotManager.Instance.InitializeJob(jobData);
		JobStateData jobStateData = JobBoardManager.instance.InitJob(jobData);
		if (jobStateData == null)
		{
			Debug.LogWarning("Not setting up the scene from jobStateData because " + jobData.name + " is not included in the game.");
		}
		else
		{
			SetupSceneFromJobStateData(jobStateData);
		}
	}

	private void SetupSceneFromJobStateData(JobStateData jobStateData)
	{
	}
}
