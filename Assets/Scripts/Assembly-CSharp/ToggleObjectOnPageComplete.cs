using System;
using UnityEngine;

public class ToggleObjectOnPageComplete : MonoBehaviour
{
	[SerializeField]
	private bool turnOn;

	[SerializeField]
	private GameObject targetObject;

	[SerializeField]
	private PageData targetPage;

	private void OnEnable()
	{
		JobBoardManager instance = JobBoardManager.instance;
		instance.OnPageComplete = (Action<PageStatusController>)Delegate.Combine(instance.OnPageComplete, new Action<PageStatusController>(PageComplete));
	}

	private void OnDisable()
	{
		JobBoardManager instance = JobBoardManager.instance;
		instance.OnPageComplete = (Action<PageStatusController>)Delegate.Combine(instance.OnPageComplete, new Action<PageStatusController>(PageComplete));
	}

	private void PageComplete(PageStatusController psc)
	{
		if (psc.Data == targetPage)
		{
			targetObject.SetActive(turnOn);
		}
	}
}
