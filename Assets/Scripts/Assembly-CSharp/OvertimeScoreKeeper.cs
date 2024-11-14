using System;
using UnityEngine;

public class OvertimeScoreKeeper : MonoBehaviour
{
	private int streak;

	private void OnEnable()
	{
		if (JobBoardManager.instance != null)
		{
			JobBoardManager instance = JobBoardManager.instance;
			instance.OnTaskComplete = (Action<TaskStatusController>)Delegate.Combine(instance.OnTaskComplete, new Action<TaskStatusController>(OnTaskComplete));
		}
	}

	private void OnDisable()
	{
		if (JobBoardManager.instance != null)
		{
			JobBoardManager instance = JobBoardManager.instance;
			instance.OnTaskComplete = (Action<TaskStatusController>)Delegate.Remove(instance.OnTaskComplete, new Action<TaskStatusController>(OnTaskComplete));
		}
	}

	private void OnTaskComplete(TaskStatusController taskStatusController)
	{
		throw new NotImplementedException();
	}
}
