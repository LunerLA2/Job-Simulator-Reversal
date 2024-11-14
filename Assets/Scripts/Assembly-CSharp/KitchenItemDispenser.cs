using System;
using UnityEngine;

public class KitchenItemDispenser : MonoBehaviour
{
	[SerializeField]
	private KitchenItemDispenserLoad[] groups;

	private void OnEnable()
	{
		JobBoardManager instance = JobBoardManager.instance;
		instance.OnTaskShown = (Action<TaskStatusController>)Delegate.Combine(instance.OnTaskShown, new Action<TaskStatusController>(TaskShown));
	}

	private void OnDisable()
	{
		JobBoardManager instance = JobBoardManager.instance;
		instance.OnTaskShown = (Action<TaskStatusController>)Delegate.Remove(instance.OnTaskShown, new Action<TaskStatusController>(TaskShown));
	}

	private void TaskShown(TaskStatusController taskStatus)
	{
		for (int i = 0; i < groups.Length; i++)
		{
			if (taskStatus.Data == groups[i].waitForTaskShown)
			{
				groups[i].gameobject.SetActive(true);
			}
		}
	}
}
