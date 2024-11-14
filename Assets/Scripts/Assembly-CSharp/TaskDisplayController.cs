using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TaskDisplayController : MonoBehaviour
{
	private const int MAX_NUM_OF_PAGES = 12;

	private const float OFFSCREEN_OFFSET_DISTANCE = 400f;

	private const float ANIMATION_START_DELAY = 0.5f;

	[SerializeField]
	private Text header;

	[SerializeField]
	private Animation taskAnimation;

	[SerializeField]
	private AnimationClip taskAnimateInClip;

	[SerializeField]
	private AnimationClip taskAnimateOutClip;

	[SerializeField]
	private AnimationClip taskCompleteInClip;

	[SerializeField]
	private AnimationClip taskCompleteOutClip;

	[SerializeField]
	private Transform pageProgressHolder;

	[SerializeField]
	private PageDisplayController pageDisplayPrefab;

	[SerializeField]
	private Transform pageDisplayParent;

	[SerializeField]
	private GameObject taskCompleteDisplay;

	[SerializeField]
	private Text taskCompleteTextMesh;

	[SerializeField]
	private Image taskCompleteIcon;

	[SerializeField]
	private string taskCompleteSuccessText;

	[SerializeField]
	private string taskCompleteFailureText;

	[SerializeField]
	private string taskCompleteSkipText;

	[SerializeField]
	private Color taskCompleteSuccessColor;

	[SerializeField]
	private Color taskCompleteFailureColor;

	[SerializeField]
	private Color taskCompleteSkipColor;

	[SerializeField]
	private Toggle completionTogglePrefab;

	[SerializeField]
	private Sprite completionToggleEmptySprite;

	[SerializeField]
	private Sprite completionToggleHalfwaySprite;

	[SerializeField]
	private Sprite completionCheckSprite;

	[SerializeField]
	private Sprite completionXSprite;

	[SerializeField]
	private Sprite completionSkipSprite;

	private int MAX_NUM_OF_VISIBLE_PAGES = 2;

	private PageDisplayController[] pageDisplayControllerList;

	private TaskStatusController currTaskStatus;

	private List<Toggle> pageRegistrationToggleList;

	private bool isWaitingForPageTransitionToComplete;

	private Vector3 pageCenterOfBoardLocalPos;

	private bool jobIsCompleted;

	public Action<PageStatusController> OnPageStarted;

	public Action<PageStatusController> OnPageShown;

	public Action<PageStatusController> OnPageEnded;

	public Transform PageDisplayParent
	{
		get
		{
			return pageDisplayParent;
		}
	}

	public void Initialize()
	{
		jobIsCompleted = false;
		pageDisplayControllerList = new PageDisplayController[MAX_NUM_OF_VISIBLE_PAGES];
		for (int i = 0; i < pageDisplayControllerList.Length; i++)
		{
			PageDisplayController pageDisplayController = UnityEngine.Object.Instantiate(pageDisplayPrefab);
			pageDisplayController.gameObject.name = "PageDisplay" + (i + 1);
			pageDisplayController.transform.SetParent(pageDisplayParent, false);
			pageCenterOfBoardLocalPos = pageDisplayController.transform.localPosition;
			pageDisplayController.Initialize();
			pageDisplayControllerList[i] = pageDisplayController;
		}
		pageRegistrationToggleList = new List<Toggle>(12);
		for (int j = 0; j < 12; j++)
		{
			Toggle toggle = UnityEngine.Object.Instantiate(completionTogglePrefab);
			toggle.transform.SetParent(pageProgressHolder, false);
			if (!JobBoardManager.USE_GAMEOBJECT_ACTIVATION_FOR_CONTENTS && j >= 6)
			{
				toggle.gameObject.SetActive(false);
			}
			toggle.name = "CompletionToggle" + (j + 1);
			pageRegistrationToggleList.Add(toggle);
		}
	}

	public float AppearAndGetLength()
	{
		if (JobBoardManager.USE_GAMEOBJECT_ACTIVATION_FOR_CONTENTS)
		{
			taskCompleteDisplay.gameObject.SetActive(false);
		}
		else
		{
			taskCompleteDisplay.transform.SetLocalPositionZOnly(-10f);
		}
		taskAnimation.clip = taskAnimateInClip;
		taskAnimation.Play();
		return taskAnimateInClip.length;
	}

	public float DisappearAndGetLength()
	{
		if (JobBoardManager.USE_GAMEOBJECT_ACTIVATION_FOR_CONTENTS)
		{
			taskCompleteDisplay.gameObject.SetActive(false);
		}
		else
		{
			taskCompleteDisplay.transform.SetLocalPositionZOnly(-10f);
		}
		taskAnimation.clip = taskAnimateOutClip;
		taskAnimation.Play();
		return taskAnimateOutClip.length;
	}

	public bool IsTaskCurrentlyDisplayed(TaskStatusController taskStatus)
	{
		return currTaskStatus == taskStatus;
	}

	public bool IsPageCurrentlyDisplayed(PageStatusController pageStatus)
	{
		return pageDisplayControllerList[0].IsPageCurrentlyDisplayed(pageStatus);
	}

	public bool IsSubtaskCurrentlyDisplayed(SubtaskStatusController subtaskStatus)
	{
		return pageDisplayControllerList[0].IsSubtaskCurrentlyDisplayed(subtaskStatus);
	}

	public void SetJobCompleted()
	{
		jobIsCompleted = true;
	}

	public float OpenTaskCompleteGraphicAndGetLength()
	{
		for (int i = 0; i < pageDisplayControllerList.Length; i++)
		{
			if (JobBoardManager.USE_GAMEOBJECT_ACTIVATION_FOR_CONTENTS)
			{
				pageDisplayControllerList[i].gameObject.SetActive(false);
			}
			else
			{
				pageDisplayControllerList[i].transform.SetLocalPositionZOnly(-10f);
			}
		}
		if (JobBoardManager.USE_GAMEOBJECT_ACTIVATION_FOR_CONTENTS)
		{
			taskCompleteDisplay.gameObject.SetActive(true);
		}
		else
		{
			taskCompleteDisplay.transform.SetLocalPositionZOnly(0f);
		}
		if (GenieManager.AreAnyJobGenieModesActive() && GenieManager.DoesContainGenieMode(GlobalStorage.Instance.CurrentGenieModes, JobGenieCartridge.GenieModeTypes.EndlessMode))
		{
			if (!JobBoardManager.instance.EndlessModeStatusController.GetCurrentGoal().IsSkipped)
			{
				taskCompleteIcon.sprite = completionCheckSprite;
				taskCompleteTextMesh.text = taskCompleteSuccessText;
				taskCompleteTextMesh.color = taskCompleteSuccessColor;
			}
			if (JobBoardManager.instance.EndlessModeStatusController.GetCurrentGoal().IsSkipped)
			{
				taskCompleteIcon.sprite = completionSkipSprite;
				taskCompleteTextMesh.text = taskCompleteSkipText;
				taskCompleteTextMesh.color = taskCompleteSkipColor;
			}
		}
		taskAnimation.clip = taskCompleteInClip;
		taskAnimation.Play();
		return taskCompleteInClip.length;
	}

	public float CloseTaskCompleteGraphicAndGetLength()
	{
		if (JobBoardManager.USE_GAMEOBJECT_ACTIVATION_FOR_CONTENTS)
		{
			taskCompleteDisplay.gameObject.SetActive(true);
		}
		else
		{
			taskCompleteDisplay.transform.SetLocalPositionZOnly(0f);
		}
		taskAnimation.clip = taskCompleteOutClip;
		taskAnimation.Play();
		return taskCompleteOutClip.length;
	}

	public void SetTask(TaskStatusController taskStatus, bool first = false)
	{
		if (jobIsCompleted)
		{
			return;
		}
		if (JobBoardManager.USE_GAMEOBJECT_ACTIVATION_FOR_CONTENTS)
		{
			taskCompleteDisplay.gameObject.SetActive(false);
		}
		else
		{
			taskCompleteDisplay.transform.SetLocalPositionZOnly(-10f);
		}
		if (currTaskStatus != null)
		{
			RemoveEventsFromTask(currTaskStatus);
		}
		isWaitingForPageTransitionToComplete = false;
		currTaskStatus = taskStatus;
		AddEventsToTask(currTaskStatus);
		StartCoroutine(RevealHeaderText(currTaskStatus.TaskHeader));
		int count = currTaskStatus.PageStatusControllerList.Count;
		int currPageIndex = currTaskStatus.CurrPageIndex;
		for (int i = 0; i < pageRegistrationToggleList.Count; i++)
		{
			Toggle toggle = pageRegistrationToggleList[i];
			toggle.image.sprite = ((i != currPageIndex) ? completionToggleEmptySprite : completionToggleHalfwaySprite);
			toggle.isOn = i < currPageIndex;
			toggle.gameObject.SetActive(count > i);
		}
		if (JobBoardManager.USE_GAMEOBJECT_ACTIVATION_FOR_CONTENTS)
		{
			pageDisplayControllerList[0].gameObject.SetActive(true);
			pageDisplayControllerList[1].gameObject.SetActive(false);
		}
		else
		{
			pageDisplayControllerList[0].transform.SetLocalPositionZOnly(0f);
			pageDisplayControllerList[1].transform.SetLocalPositionZOnly(-10f);
		}
		PageStatusController currentPage = currTaskStatus.GetCurrentPage();
		pageDisplayControllerList[0].SetNewPage(currentPage, true);
		if (currentPage != null)
		{
			if (OnPageStarted != null)
			{
				OnPageStarted(currentPage);
			}
			StartCoroutine(WaitAndFirePageShown(currentPage, pageDisplayControllerList[0].GetTimeToAnimateIn() + currentPage.Data.SecsOfBlankBeforeAnimatingIn));
		}
		else
		{
			Debug.LogError("page was null");
		}
	}

	public IEnumerator SetTaskAsync(TaskStatusController taskStatus, bool first = false)
	{
		if (jobIsCompleted)
		{
			yield break;
		}
		if (JobBoardManager.USE_GAMEOBJECT_ACTIVATION_FOR_CONTENTS)
		{
			taskCompleteDisplay.gameObject.SetActive(false);
		}
		else
		{
			taskCompleteDisplay.transform.SetLocalPositionZOnly(-10f);
		}
		if (currTaskStatus != null)
		{
			RemoveEventsFromTask(currTaskStatus);
		}
		isWaitingForPageTransitionToComplete = false;
		currTaskStatus = taskStatus;
		AddEventsToTask(currTaskStatus);
		StartCoroutine(RevealHeaderText(currTaskStatus.TaskHeader));
		int numOfPages = currTaskStatus.PageStatusControllerList.Count;
		int currPage = currTaskStatus.CurrPageIndex;
		for (int i = 0; i < pageRegistrationToggleList.Count; i++)
		{
			Toggle toggle = pageRegistrationToggleList[i];
			toggle.image.sprite = ((i != currPage) ? completionToggleEmptySprite : completionToggleHalfwaySprite);
			toggle.isOn = i < currPage;
			toggle.gameObject.SetActive(numOfPages > i);
		}
		if (JobBoardManager.USE_GAMEOBJECT_ACTIVATION_FOR_CONTENTS)
		{
			pageDisplayControllerList[0].gameObject.SetActive(true);
			pageDisplayControllerList[1].gameObject.SetActive(false);
		}
		else
		{
			pageDisplayControllerList[0].transform.SetLocalPositionZOnly(0f);
			pageDisplayControllerList[1].transform.SetLocalPositionZOnly(-10f);
		}
		PageStatusController page = currTaskStatus.GetCurrentPage();
		yield return null;
		pageDisplayControllerList[0].SetNewPage(page, true);
		if (page != null)
		{
			if (OnPageStarted != null)
			{
				OnPageStarted(page);
			}
			StartCoroutine(WaitAndFirePageShown(page, pageDisplayControllerList[0].GetTimeToAnimateIn() + page.Data.SecsOfBlankBeforeAnimatingIn));
		}
		else
		{
			Debug.LogError("page was null");
		}
	}

	private IEnumerator RevealHeaderText(string t)
	{
		header.text = string.Empty;
		yield return new WaitForSeconds(taskAnimateInClip.length + 0.25f);
		for (int i = 0; i <= t.Length; i++)
		{
			header.text = t.Substring(0, i);
			JobBoardManager.instance.MarkAsDirty();
			yield return new WaitForSeconds(0.025f);
		}
	}

	private void AddEventsToTask(TaskStatusController taskStatus)
	{
		taskStatus.OnComplete = (Action<TaskStatusController>)Delegate.Combine(taskStatus.OnComplete, new Action<TaskStatusController>(TaskCompleted));
		taskStatus.OnPageChange = (Action<TaskStatusController, bool>)Delegate.Combine(taskStatus.OnPageChange, new Action<TaskStatusController, bool>(PageChange));
	}

	private void RemoveEventsFromTask(TaskStatusController taskStatus)
	{
		taskStatus.OnComplete = (Action<TaskStatusController>)Delegate.Remove(taskStatus.OnComplete, new Action<TaskStatusController>(TaskCompleted));
		taskStatus.OnPageChange = (Action<TaskStatusController, bool>)Delegate.Remove(taskStatus.OnPageChange, new Action<TaskStatusController, bool>(PageChange));
	}

	private void TaskCompleted(TaskStatusController taskStatus)
	{
		StartCoroutine(WaitAndUpdatePageTicks(0.1f));
	}

	private void PageChange(TaskStatusController taskStatusController, bool isLastPage)
	{
		if (isWaitingForPageTransitionToComplete)
		{
			return;
		}
		isWaitingForPageTransitionToComplete = true;
		if (!isLastPage)
		{
			if (JobBoardManager.USE_GAMEOBJECT_ACTIVATION_FOR_CONTENTS)
			{
				pageDisplayControllerList[1].gameObject.SetActive(true);
			}
			else
			{
				pageDisplayControllerList[1].transform.SetLocalPositionZOnly(0f);
			}
			pageDisplayControllerList[1].SetNewPage(taskStatusController.GetCurrentPage(), false);
		}
		AnimateToNextPage(!isLastPage);
	}

	private void AnimateToNextPage(bool animateNextPageIn)
	{
		PageDisplayController pageDisplayController = pageDisplayControllerList[0];
		Vector3 endValue = pageCenterOfBoardLocalPos;
		endValue.x -= 400f;
		float num = 1f;
		float num2 = 0f;
		float num3 = 0f;
		if (pageDisplayController != null)
		{
			if (pageDisplayController.CurrPageStatus == null)
			{
				Debug.LogError("pageDisplayOutgoing.CurrPageStatus is null");
			}
			num2 = pageDisplayController.CurrPageStatus.Data.SecsToLingerOnCompletedSubtasks;
			num3 = pageDisplayController.CurrPageStatus.Data.SecsOfBlankAfterAnimatingOut;
			StartCoroutine(WaitAndRegisterDirtyCase(0.5f + num2));
			Go.to(pageDisplayController.transform, num, new GoTweenConfig().localPosition(endValue).onComplete(AnimatingPageOutComplete).setDelay(0.5f + num2)
				.setEaseType(GoEaseType.QuadInOut));
			if (JobBoardManager.USE_GAMEOBJECT_ACTIVATION_FOR_CONTENTS)
			{
				StartCoroutine(WaitAndHideGameObject(pageDisplayController.gameObject, 0.5f + num2 + num + 0.1f));
			}
		}
		else
		{
			Debug.LogError("pageDisplayOutgoing is null");
		}
		if (animateNextPageIn)
		{
			PageDisplayController pageDisplayController2 = pageDisplayControllerList[1];
			Vector3 localPosition = pageCenterOfBoardLocalPos;
			localPosition.x += 400f;
			float num4 = 0f;
			if (pageDisplayController2.CurrPageStatus != null)
			{
				if (pageDisplayController2.CurrPageStatus.Data != null)
				{
					num4 = pageDisplayController2.CurrPageStatus.Data.SecsOfBlankBeforeAnimatingIn;
				}
				StartCoroutine(WaitAndFirePageStarted(pageDisplayController2.CurrPageStatus, 0.5f + num2 + num3 + num + 0.05f));
			}
			pageDisplayController2.transform.localPosition = localPosition;
			if (JobBoardManager.USE_GAMEOBJECT_ACTIVATION_FOR_CONTENTS)
			{
				pageDisplayController2.gameObject.SetActive(true);
			}
			else
			{
				pageDisplayController2.transform.SetLocalPositionZOnly(0f);
			}
			float num5 = 0.5f + num2 + num3 + num4 + 0.25f * num;
			StartCoroutine(WaitAndRegisterDirtyCase(num5));
			Go.to(pageDisplayController2.transform, num, new GoTweenConfig().localPosition(pageCenterOfBoardLocalPos).setDelay(num5).onComplete(AnimatingPageInComplete)
				.setEaseType(GoEaseType.QuadInOut));
		}
		StartCoroutine(WaitAndUpdatePageTicks(0.5f + num2));
	}

	private IEnumerator WaitAndRegisterDirtyCase(float t)
	{
		yield return new WaitForSeconds(t);
		JobBoardManager.instance.AddSpecialDirtyCase();
	}

	private IEnumerator WaitAndHideGameObject(GameObject go, float waitTime)
	{
		yield return new WaitForSeconds(waitTime);
		go.SetActive(false);
	}

	private IEnumerator WaitAndFirePageStarted(PageStatusController pageStatus, float waitTime)
	{
		yield return new WaitForSeconds(waitTime);
		if (OnPageStarted != null)
		{
			OnPageStarted(pageStatus);
		}
	}

	private IEnumerator WaitAndFirePageShown(PageStatusController pageStatus, float waitTime)
	{
		yield return new WaitForSeconds(waitTime);
		if (OnPageShown != null)
		{
			OnPageShown(pageStatus);
		}
	}

	private IEnumerator WaitAndUpdatePageTicks(float waitTime)
	{
		if (currTaskStatus.IsSkipped)
		{
			yield break;
		}
		yield return new WaitForSeconds(waitTime);
		if (pageRegistrationToggleList.Count >= currTaskStatus.PageStatusControllerList.Count)
		{
			for (int i = 0; i < currTaskStatus.PageStatusControllerList.Count; i++)
			{
				bool isThisCompleted = currTaskStatus.PageStatusControllerList[i].IsCompleted || i < currTaskStatus.CurrPageIndex;
				pageRegistrationToggleList[i].image.sprite = ((i != currTaskStatus.CurrPageIndex) ? completionToggleEmptySprite : completionToggleHalfwaySprite);
				pageRegistrationToggleList[i].isOn = isThisCompleted;
			}
		}
		else
		{
			Debug.LogError("Need to increase MAX_NUM_OF_PAGES in TaskDisplayController to support this task!");
		}
	}

	public float HackForceCompleteAnimationAndGetLength()
	{
		return CloseTaskCompleteGraphicAndGetLength();
	}

	private void AnimatingPageOutComplete(AbstractGoTween tween)
	{
		if (!jobIsCompleted)
		{
			pageDisplayControllerList[0].ResetUIComponents();
		}
		if (pageDisplayControllerList[0].CurrPageStatus != null && OnPageEnded != null)
		{
			OnPageEnded(pageDisplayControllerList[0].CurrPageStatus);
		}
		JobBoardManager.instance.RemoveSpecialDirtyCase();
	}

	private void AnimatingPageInComplete(AbstractGoTween tween)
	{
		PageDisplayController pageDisplayController = pageDisplayControllerList[0];
		pageDisplayControllerList[0] = pageDisplayControllerList[1];
		pageDisplayControllerList[1] = pageDisplayController;
		isWaitingForPageTransitionToComplete = false;
		if (pageDisplayControllerList[0].CurrPageStatus != null && OnPageShown != null)
		{
			OnPageShown(pageDisplayControllerList[0].CurrPageStatus);
		}
		if (pageDisplayControllerList[0].CurrPageStatus != currTaskStatus.GetCurrentPage() && currTaskStatus.GetCurrentPage() != null)
		{
			PageChange(currTaskStatus, false);
		}
		JobBoardManager.instance.RemoveSpecialDirtyCase();
	}
}
