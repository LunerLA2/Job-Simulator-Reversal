using System;
using System.Collections;
using UnityEngine;

public class ConvenienceStoreCarController : MonoBehaviour
{
	[SerializeField]
	private TaskData policeOnTaskComplete;

	[SerializeField]
	private Animation[] policeAnimations;

	private float delayBetweenPoliceAnimations = 2f;

	[SerializeField]
	private AudioClip copSound;

	[SerializeField]
	private AudioClip passBySound1;

	[SerializeField]
	private AudioClip passBySound2;

	private void OnEnable()
	{
		if (JobBoardManager.instance != null)
		{
			JobBoardManager instance = JobBoardManager.instance;
			instance.OnTaskComplete = (Action<TaskStatusController>)Delegate.Combine(instance.OnTaskComplete, new Action<TaskStatusController>(TaskComplete));
		}
		else
		{
			Debug.LogError("This should only appear if you start the PSVR layout from the editor directly. If you enter this scene with another layout or use the PSVR layout in the museum to load in, it will be fine instead.  If this is not the case or something else weird is appearing, it's time to raise a flag -- talk to Mike or Eiche");
		}
	}

	private void OnDisable()
	{
		JobBoardManager instance = JobBoardManager.instance;
		instance.OnTaskComplete = (Action<TaskStatusController>)Delegate.Remove(instance.OnTaskComplete, new Action<TaskStatusController>(TaskComplete));
	}

	private void TaskComplete(TaskStatusController task)
	{
		if (task.Data == policeOnTaskComplete)
		{
			StartCoroutine(InternalDoPolice());
		}
	}

	private IEnumerator InternalDoPolice()
	{
		yield return new WaitForSeconds(5.5f);
		for (int i = 0; i < policeAnimations.Length; i++)
		{
			policeAnimations[i].gameObject.SetActive(true);
			policeAnimations[i].Play();
			AudioManager.Instance.Play(policeAnimations[i].transform, copSound, 1f, 1f);
			yield return new WaitForSeconds(delayBetweenPoliceAnimations);
			policeAnimations[i].gameObject.SetActive(false);
		}
	}
}
