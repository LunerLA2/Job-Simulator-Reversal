using System;
using OwlchemyVR;
using UnityEngine;

public class KitchenManager : LevelManager
{
	[SerializeField]
	private PageData cockroachPageData;

	[SerializeField]
	private GameObject cockroachContainer;

	[SerializeField]
	private GameObject cockroachContainerNoFloor;

	[SerializeField]
	private GameObject cockroachContainerLowGrav;

	[SerializeField]
	private GameObject cockroachContainerLowGravNoFloor;

	private JobStateData jobStateData;

	private void OnEnable()
	{
		JobBoardManager instance = JobBoardManager.instance;
		instance.OnPageStarted = (Action<PageStatusController>)Delegate.Combine(instance.OnPageStarted, new Action<PageStatusController>(PageStarted));
	}

	private void OnDisable()
	{
		JobBoardManager instance = JobBoardManager.instance;
		instance.OnPageStarted = (Action<PageStatusController>)Delegate.Remove(instance.OnPageStarted, new Action<PageStatusController>(PageStarted));
	}

	private void Start()
	{
		JobData jobData = base.jobData;
		bool flag = false;
		if (GenieManager.DoesContainGenieMode(GlobalStorage.Instance.CurrentGenieModes, JobGenieCartridge.GenieModeTypes.EndlessMode))
		{
			flag = true;
		}
		if (flag)
		{
			JobStatusController jobStatusController = new JobStatusController(base.jobData);
			BotManager.Instance.InitializeEndlessMode(endlessModeConfigData);
			JobBoardManager.instance.InitEndlessMode(endlessModeConfigData, jobStatusController.JobStateData);
		}
		else
		{
			BotManager.Instance.InitializeJob(jobData);
			JobStateData jobStateData = JobBoardManager.instance.InitJob(jobData);
			SetupSceneFromJobStateData(jobStateData);
		}
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

	private void PageStarted(PageStatusController pageStatus)
	{
		if (!(pageStatus.Data == cockroachPageData))
		{
			return;
		}
		bool flag = false;
		if (GenieManager.AreAnyJobGenieModesActive() && GenieManager.DoesContainGenieMode(GlobalStorage.Instance.CurrentGenieModes, JobGenieCartridge.GenieModeTypes.NoGravityMode))
		{
			flag = true;
		}
		if (flag)
		{
			if (VRPlatform.IsRoomScale)
			{
				cockroachContainerLowGrav.SetActive(true);
			}
			else
			{
				cockroachContainerLowGravNoFloor.SetActive(true);
			}
		}
		else if (VRPlatform.IsRoomScale)
		{
			cockroachContainer.SetActive(true);
		}
		else
		{
			cockroachContainerNoFloor.SetActive(true);
		}
	}
}
