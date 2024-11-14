using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using OwlchemyVR;
using UnityEngine;
using UnityEngine.UI;

public class JobBoardManager : MonoBehaviour
{
	private const string KEY_SEPERATOR = ":";

	private static bool USE_LOW_FPS_RENDER = true;

	public static bool USE_GAMEOBJECT_ACTIVATION_FOR_CONTENTS = true;

	private static JobBoardManager _instance;

	[SerializeField]
	private Camera renderTextureCamera;

	[SerializeField]
	private float desiredRenderTextureCameraFPS = 90f;

	private float lastRenderTextureCameraRender;

	private bool isDirty;

	private bool isConstantlyDirty;

	private int specialDirtyCases;

	private int tempPromotionSound;

	[SerializeField]
	private TaskDisplayController taskDisplayPrefab;

	[SerializeField]
	private Transform jobBoardCanvas;

	[SerializeField]
	private Text continueLabel;

	[SerializeField]
	private SubtaskDisplayController subtaskDisplayControllerPrefab;

	[SerializeField]
	private Transform continueSubtaskParent;

	[SerializeField]
	private GameObject psvrAdjustFloorScreen;

	[SerializeField]
	private GameObject psvrFloorDetailsScreen;

	private SubtaskDisplayController betweenTasksSubtaskDisplayController;

	private SubtaskStatusController betweenTasksSubtaskStatusController;

	private bool isUsingConfirmationSystem;

	private bool onlyUseConfirmationOnce;

	private bool isWaitingForConfirmation;

	[SerializeField]
	private Animation introScreenAnimation;

	[SerializeField]
	private Animation outroScreenAnimation;

	[SerializeField]
	private Animation continueScreenAnimation;

	[SerializeField]
	private AnimationClip screenInAnimationClip;

	[SerializeField]
	private AnimationClip screenOutAnimationClip;

	private bool isInEndlessMode;

	private TaskDisplayController taskDisplayController;

	private AudioSourceHelper audioSrcHelper;

	[SerializeField]
	private AudioSourceHelper audioSrcHelper2d;

	[SerializeField]
	private AudioClip subtaskSuccessAudioClip;

	[SerializeField]
	private AudioClip taskCompleteAudioClip;

	[SerializeField]
	private AudioClip taskCompleteFailedAudioClip;

	[SerializeField]
	private AudioClip subtaskCounterProgressAudioClip;

	[SerializeField]
	private AudioClip pageAdvanceAudioClip;

	[SerializeField]
	private AudioClip continueCompleteAudioClip;

	[SerializeField]
	private AudioClip[] promotionAudioClips;

	private Dictionary<string, SubTaskActionEventContainerListObject> quickLookupForActionEventsOccurring = new Dictionary<string, SubTaskActionEventContainerListObject>();

	private StringBuilder sb = new StringBuilder();

	public Action OnBeganWaitingForConfirmation;

	public Action OnBeganWaitingForSkipAction;

	private bool isVisible;

	private VisibilityEvents visibilityEvents;

	private bool visibilityEventsAttached;

	private bool hackDisableRendering;

	public Action<JobStatusController> OnJobComplete;

	public Action OnSandboxPhaseStarted;

	public Action OnStartedFromFirstTask;

	public Action OnDidntStartFromFirstTask;

	public Action<TaskStatusController> OnTaskComplete;

	public Action<PageStatusController> OnPageComplete;

	public Action<SubtaskStatusController> OnSubtaskComplete;

	public Action<SubtaskStatusController> OnSubtaskUncomplete;

	public Action<SubtaskStatusController, bool> OnSubtaskCounterChange;

	public Action<JobStatusController> OnJobStarted;

	public Action<TaskStatusController> OnTaskStarted;

	public Action<TaskStatusController> OnTaskShown;

	public Action<PageStatusController> OnPageStarted;

	public Action<PageStatusController> OnPageShown;

	public Action<PageStatusController> OnPageEnded;

	public Action<TaskStatusController> OnTaskEnded;

	public static JobBoardManager instance
	{
		get
		{
			return _instance;
		}
	}

	public JobStatusController JobStatusController { get; private set; }

	public EndlessModeStatusController EndlessModeStatusController { get; private set; }

	public bool IsInEndlessMode
	{
		get
		{
			return isInEndlessMode;
		}
	}

	private void Awake()
	{
		specialDirtyCases = 0;
		renderTextureCamera.eventMask = 0;
		if (_instance != this)
		{
			if (_instance != null)
			{
				Debug.LogWarning("There were 2 instances of the JobBoard in your scene!");
				UnityEngine.Object.Destroy(_instance);
			}
			_instance = this;
		}
		if (USE_LOW_FPS_RENDER && desiredRenderTextureCameraFPS < 90f)
		{
			renderTextureCamera.gameObject.SetActive(false);
		}
		SetPSVRAdjustFloorScreenState(false);
		SetPSVRFloorDetailsScreenState(false);
	}

	public void SetVisibilityEvents(VisibilityEvents e)
	{
		DetachVisibilityEvents(visibilityEvents);
		visibilityEvents = e;
		AttachVisibilityEvents(e);
	}

	private void OnEnable()
	{
		AttachVisibilityEvents(visibilityEvents);
	}

	private void AttachVisibilityEvents(VisibilityEvents vis)
	{
		if (vis != null && !visibilityEventsAttached)
		{
			vis.OnObjectBecameVisible += BoardBecameVisible;
			vis.OnObjectBecameInvisible += BoardBecameInvisible;
			visibilityEventsAttached = true;
		}
	}

	private void OnDisable()
	{
		DetachVisibilityEvents(visibilityEvents);
	}

	private void DetachVisibilityEvents(VisibilityEvents vis)
	{
		if (vis != null && visibilityEventsAttached)
		{
			vis.OnObjectBecameVisible -= BoardBecameVisible;
			vis.OnObjectBecameInvisible -= BoardBecameInvisible;
			visibilityEventsAttached = false;
		}
	}

	public void HACK_TweenParentToX(float x)
	{
		Go.to(taskDisplayController.PageDisplayParent, 0.5f, new GoTweenConfig().localPosition(new Vector3(x, taskDisplayController.PageDisplayParent.localPosition.y, taskDisplayController.PageDisplayParent.localPosition.z)).setEaseType(GoEaseType.QuadInOut));
	}

	private void BoardBecameVisible(VisibilityEvents e)
	{
		isVisible = true;
		MarkAsDirty();
	}

	private void BoardBecameInvisible(VisibilityEvents e)
	{
		isVisible = false;
	}

	public void GenerateTestTasksEndless()
	{
		if (EndlessModeStatusController != null)
		{
			string contents = EndlessModeStatusController.GenerateTaskRandomUnitTest(100);
			File.WriteAllText(Application.dataPath + "\\endlesstasks.csv", contents);
		}
	}

	private void LateUpdate()
	{
		if (!USE_LOW_FPS_RENDER || !isVisible || hackDisableRendering || !(desiredRenderTextureCameraFPS < 90f))
		{
			return;
		}
		float num = lastRenderTextureCameraRender + 1f / desiredRenderTextureCameraFPS;
		if (Time.time >= num)
		{
			if (isDirty || isConstantlyDirty || specialDirtyCases > 0)
			{
				renderTextureCamera.Render();
				isDirty = false;
			}
			lastRenderTextureCameraRender = Time.time;
		}
	}

	public void AddSpecialDirtyCase()
	{
		specialDirtyCases++;
	}

	public void RemoveSpecialDirtyCase()
	{
		specialDirtyCases--;
		if (specialDirtyCases < 0)
		{
			Debug.LogError("JobBoardManager.specialDirtyCases went below zero somehow");
			specialDirtyCases = 0;
		}
	}

	public void MarkAsDirty()
	{
		isDirty = true;
	}

	public void SetIsConstantlyDirty(bool val)
	{
		isConstantlyDirty = val;
		if (!val)
		{
			isDirty = true;
		}
	}

	private IEnumerator WaitAndSetIsConstantlyDirty(bool val, float t)
	{
		yield return new WaitForSeconds(t);
		SetIsConstantlyDirty(val);
	}

	public void SetPSVRAdjustFloorScreenState(bool state)
	{
		if (psvrAdjustFloorScreen != null)
		{
			psvrAdjustFloorScreen.SetActive(state);
			MarkAsDirty();
		}
	}

	public void SetPSVRFloorDetailsScreenState(bool state)
	{
		if (psvrFloorDetailsScreen != null)
		{
			psvrFloorDetailsScreen.SetActive(state);
			MarkAsDirty();
		}
	}

	public void SetMainRenderTexture(RenderTexture rt)
	{
		renderTextureCamera.targetTexture = rt;
		MarkAsDirty();
	}

	public void SetAudioSrcHelper(AudioSourceHelper src)
	{
		audioSrcHelper = src;
	}

	public int GetCurrentTaskIndex()
	{
		if (JobStatusController != null)
		{
			return JobStatusController.CurrTaskIndex;
		}
		Debug.LogError("Can't get current task index when JobStatusController is null");
		return -1;
	}

	public TaskData GetCurrentTaskData()
	{
		if (JobStatusController != null)
		{
			if (JobStatusController.IsCompleted)
			{
				return null;
			}
			return JobStatusController.GetCurrentTask().Data;
		}
		return null;
	}

	public PageData GetCurrentPageData()
	{
		if (JobStatusController != null)
		{
			if (JobStatusController.IsCompleted)
			{
				return null;
			}
			return JobStatusController.GetCurrentPageData();
		}
		if (EndlessModeStatusController != null)
		{
			if (EndlessModeStatusController.GetCurrentGoal() == null)
			{
				return null;
			}
			if (EndlessModeStatusController.GetCurrentGoal() != null && EndlessModeStatusController.GetCurrentGoal().IsCompleted)
			{
				return null;
			}
			if (EndlessModeStatusController.GetCurrentGoal().GetCurrentPage().IsCompleted)
			{
				return null;
			}
			return EndlessModeStatusController.GetCurrentGoal().GetCurrentPage().Data;
		}
		return null;
	}

	public JobStateData InitJob(JobData jobData)
	{
		isInEndlessMode = false;
		USE_GAMEOBJECT_ACTIVATION_FOR_CONTENTS = true;
		if (GlobalStorage.Instance.StartingTaskIndex == 0)
		{
			if (OnStartedFromFirstTask != null)
			{
				OnStartedFromFirstTask();
			}
		}
		else if (OnDidntStartFromFirstTask != null)
		{
			OnDidntStartFromFirstTask();
		}
		SetIsConstantlyDirty(true);
		introScreenAnimation.clip = screenInAnimationClip;
		introScreenAnimation.Play();
		if (USE_GAMEOBJECT_ACTIVATION_FOR_CONTENTS)
		{
			outroScreenAnimation.gameObject.SetActive(false);
			continueScreenAnimation.gameObject.SetActive(false);
		}
		else
		{
			outroScreenAnimation.transform.SetLocalPositionZOnly(-10f);
			continueScreenAnimation.transform.SetLocalPositionZOnly(-10f);
		}
		taskDisplayController = UnityEngine.Object.Instantiate(taskDisplayPrefab);
		taskDisplayController.name = "TaskDisplay";
		taskDisplayController.transform.SetParent(jobBoardCanvas, false);
		taskDisplayController.GetComponent<TaskDisplayController>().Initialize();
		if (USE_GAMEOBJECT_ACTIVATION_FOR_CONTENTS)
		{
			taskDisplayController.gameObject.SetActive(false);
		}
		else
		{
			taskDisplayController.transform.SetLocalPositionZOnly(-10f);
		}
		AddEventsToTaskDisplay(taskDisplayController);
		if (jobData.PromptTextBetweenTasks != string.Empty && jobData.SubtaskToAdvanceBetweenTasks != null)
		{
			isUsingConfirmationSystem = true;
			continueLabel.text = jobData.PromptTextBetweenTasks;
			betweenTasksSubtaskStatusController = new SubtaskStatusController(jobData.SubtaskToAdvanceBetweenTasks);
			betweenTasksSubtaskDisplayController = UnityEngine.Object.Instantiate(subtaskDisplayControllerPrefab);
			betweenTasksSubtaskDisplayController.name = "BetweenTasksSubtaskDisplay";
			betweenTasksSubtaskDisplayController.transform.SetParent(continueSubtaskParent);
			betweenTasksSubtaskDisplayController.transform.localPosition = Vector3.zero;
			betweenTasksSubtaskDisplayController.transform.localRotation = Quaternion.identity;
			betweenTasksSubtaskDisplayController.transform.localScale = Vector3.one;
			SubtaskStatusController subtaskStatusController = betweenTasksSubtaskStatusController;
			subtaskStatusController.OnComplete = (Action<SubtaskStatusController>)Delegate.Combine(subtaskStatusController.OnComplete, new Action<SubtaskStatusController>(BetweenTasksSubtaskComplete));
		}
		else
		{
			isUsingConfirmationSystem = false;
			Debug.LogWarning(jobData.name + " is not set up to use the Prompt Between Tasks feature, so it won't be used.");
		}
		JobStatusController = new JobStatusController(jobData);
		AddEventsToJobStatusController(JobStatusController);
		BuildQuickLookupEventDictionary();
		StartCoroutine(InternalInitJob(jobData));
		return JobStatusController.JobStateData;
	}

	private IEnumerator InternalInitJob(JobData jobData)
	{
		SetIsConstantlyDirty(true);
		if (USE_GAMEOBJECT_ACTIVATION_FOR_CONTENTS)
		{
			taskDisplayController.gameObject.SetActive(true);
		}
		else
		{
			taskDisplayController.transform.SetLocalPositionZOnly(0f);
		}
		taskDisplayController.transform.localScale = new Vector3(1f, 0f, 1f);
		if (jobData.SecsOfBlankBeforeStartingJob >= screenInAnimationClip.length)
		{
			yield return new WaitForSeconds(screenInAnimationClip.length);
			SetIsConstantlyDirty(false);
			yield return new WaitForSeconds(jobData.SecsOfBlankBeforeStartingJob - screenInAnimationClip.length);
		}
		else
		{
			yield return new WaitForSeconds(jobData.SecsOfBlankBeforeStartingJob);
		}
		SetIsConstantlyDirty(true);
		introScreenAnimation.clip = screenOutAnimationClip;
		introScreenAnimation.Play();
		yield return new WaitForSeconds(screenOutAnimationClip.length);
		if (USE_GAMEOBJECT_ACTIVATION_FOR_CONTENTS)
		{
			introScreenAnimation.gameObject.SetActive(false);
		}
		else
		{
			introScreenAnimation.transform.SetLocalPositionZOnly(-10f);
		}
		if (OnJobStarted != null)
		{
			OnJobStarted(JobStatusController);
		}
		SetIsConstantlyDirty(false);
		BeginJob();
	}

	public EndlessModeStatusController InitEndlessMode(EndlessModeData endlessModeData, JobStateData jobStateData)
	{
		isInEndlessMode = true;
		USE_GAMEOBJECT_ACTIVATION_FOR_CONTENTS = false;
		if (OnStartedFromFirstTask != null)
		{
			OnStartedFromFirstTask();
		}
		SetIsConstantlyDirty(true);
		introScreenAnimation.clip = screenInAnimationClip;
		introScreenAnimation.Play();
		if (USE_GAMEOBJECT_ACTIVATION_FOR_CONTENTS)
		{
			outroScreenAnimation.gameObject.SetActive(false);
			continueScreenAnimation.gameObject.SetActive(false);
		}
		else
		{
			outroScreenAnimation.transform.SetLocalPositionZOnly(-10f);
			continueScreenAnimation.transform.SetLocalPositionZOnly(-10f);
		}
		taskDisplayController = UnityEngine.Object.Instantiate(taskDisplayPrefab);
		taskDisplayController.name = "TaskDisplay";
		taskDisplayController.transform.SetParent(jobBoardCanvas, false);
		taskDisplayController.GetComponent<TaskDisplayController>().Initialize();
		if (USE_GAMEOBJECT_ACTIVATION_FOR_CONTENTS)
		{
			taskDisplayController.gameObject.SetActive(false);
		}
		else
		{
			taskDisplayController.transform.SetLocalPositionZOnly(-10f);
		}
		AddEventsToTaskDisplay(taskDisplayController);
		if (endlessModeData.PromptTextBetweenTasks != string.Empty && endlessModeData.SubtaskToAdvanceBetweenTasks != null)
		{
			isUsingConfirmationSystem = true;
			continueLabel.text = endlessModeData.PromptTextBetweenTasks;
			betweenTasksSubtaskStatusController = new SubtaskStatusController(endlessModeData.SubtaskToAdvanceBetweenTasks);
			betweenTasksSubtaskDisplayController = UnityEngine.Object.Instantiate(subtaskDisplayControllerPrefab);
			betweenTasksSubtaskDisplayController.name = "BetweenTasksSubtaskDisplay";
			betweenTasksSubtaskDisplayController.transform.SetParent(continueSubtaskParent);
			betweenTasksSubtaskDisplayController.transform.localPosition = Vector3.zero;
			betweenTasksSubtaskDisplayController.transform.localRotation = Quaternion.identity;
			betweenTasksSubtaskDisplayController.transform.localScale = Vector3.one;
			SubtaskStatusController subtaskStatusController = betweenTasksSubtaskStatusController;
			subtaskStatusController.OnComplete = (Action<SubtaskStatusController>)Delegate.Combine(subtaskStatusController.OnComplete, new Action<SubtaskStatusController>(BetweenTasksSubtaskComplete));
		}
		else
		{
			isUsingConfirmationSystem = false;
			Debug.LogWarning("endless mode is not set up to use the Prompt Between Tasks feature, so it won't be used.");
		}
		EndlessModeStatusController = new EndlessModeStatusController(endlessModeData, jobStateData);
		AddEventsToEndlessModeStatusController(EndlessModeStatusController);
		BuildQuickLookupEventDictionary(false);
		StartCoroutine(InternalInitEndlessMode(EndlessModeStatusController));
		return EndlessModeStatusController;
	}

	private IEnumerator InternalInitEndlessMode(EndlessModeStatusController endlessModeStatusController)
	{
		SetIsConstantlyDirty(true);
		if (USE_GAMEOBJECT_ACTIVATION_FOR_CONTENTS)
		{
			taskDisplayController.gameObject.SetActive(true);
		}
		else
		{
			taskDisplayController.transform.SetLocalPositionZOnly(0f);
		}
		taskDisplayController.transform.localScale = new Vector3(1f, 0f, 1f);
		if (endlessModeStatusController.Data.SecsOfBlankBeforeStarting >= screenInAnimationClip.length)
		{
			yield return new WaitForSeconds(screenInAnimationClip.length);
			SetIsConstantlyDirty(false);
			yield return new WaitForSeconds(endlessModeStatusController.Data.SecsOfBlankBeforeStarting - screenInAnimationClip.length);
		}
		else
		{
			yield return new WaitForSeconds(endlessModeStatusController.Data.SecsOfBlankBeforeStarting);
		}
		SetIsConstantlyDirty(true);
		introScreenAnimation.clip = screenOutAnimationClip;
		introScreenAnimation.Play();
		yield return new WaitForSeconds(screenOutAnimationClip.length);
		if (USE_GAMEOBJECT_ACTIVATION_FOR_CONTENTS)
		{
			introScreenAnimation.gameObject.SetActive(false);
		}
		else
		{
			introScreenAnimation.transform.SetLocalPositionZOnly(-10f);
		}
		SetIsConstantlyDirty(false);
		StartWaitingForContinue();
		MarkAsDirty();
	}

	private void AddEventsToTaskDisplay(TaskDisplayController taskDisplay)
	{
		taskDisplay.OnPageStarted = (Action<PageStatusController>)Delegate.Combine(taskDisplay.OnPageStarted, new Action<PageStatusController>(PageStarted));
		taskDisplay.OnPageShown = (Action<PageStatusController>)Delegate.Combine(taskDisplay.OnPageShown, new Action<PageStatusController>(PageShown));
		taskDisplay.OnPageEnded = (Action<PageStatusController>)Delegate.Combine(taskDisplay.OnPageEnded, new Action<PageStatusController>(PageEnded));
	}

	private void AddEventsToJobStatusController(JobStatusController jobStatus)
	{
		jobStatus.OnCompleted = (Action<JobStatusController>)Delegate.Combine(jobStatus.OnCompleted, new Action<JobStatusController>(JobComplete));
		jobStatus.OnTaskComplete = (Action<TaskStatusController>)Delegate.Combine(jobStatus.OnTaskComplete, new Action<TaskStatusController>(TaskComplete));
		jobStatus.OnPageComplete = (Action<PageStatusController>)Delegate.Combine(jobStatus.OnPageComplete, new Action<PageStatusController>(PageComplete));
		jobStatus.OnSubtaskComplete = (Action<SubtaskStatusController>)Delegate.Combine(jobStatus.OnSubtaskComplete, new Action<SubtaskStatusController>(SubtaskComplete));
		jobStatus.OnSubtaskUncomplete = (Action<SubtaskStatusController>)Delegate.Combine(jobStatus.OnSubtaskUncomplete, new Action<SubtaskStatusController>(SubtaskUncomplete));
		jobStatus.OnSubtaskCounterChange = (Action<SubtaskStatusController, bool>)Delegate.Combine(jobStatus.OnSubtaskCounterChange, new Action<SubtaskStatusController, bool>(SubtaskCounterChange));
	}

	private void AddEventsToEndlessModeStatusController(EndlessModeStatusController endlessStatus)
	{
		endlessStatus.OnCompleted = (Action<JobStatusController>)Delegate.Combine(endlessStatus.OnCompleted, new Action<JobStatusController>(JobComplete));
		endlessStatus.OnTaskComplete = (Action<TaskStatusController>)Delegate.Combine(endlessStatus.OnTaskComplete, new Action<TaskStatusController>(TaskComplete));
		endlessStatus.OnPageComplete = (Action<PageStatusController>)Delegate.Combine(endlessStatus.OnPageComplete, new Action<PageStatusController>(PageComplete));
		endlessStatus.OnSubtaskComplete = (Action<SubtaskStatusController>)Delegate.Combine(endlessStatus.OnSubtaskComplete, new Action<SubtaskStatusController>(SubtaskComplete));
		endlessStatus.OnSubtaskUncomplete = (Action<SubtaskStatusController>)Delegate.Combine(endlessStatus.OnSubtaskUncomplete, new Action<SubtaskStatusController>(SubtaskUncomplete));
		endlessStatus.OnSubtaskCounterChange = (Action<SubtaskStatusController, bool>)Delegate.Combine(endlessStatus.OnSubtaskCounterChange, new Action<SubtaskStatusController, bool>(SubtaskCounterChange));
	}

	private void BuildQuickLookupEventDictionary(bool includingJobData = true)
	{
		quickLookupForActionEventsOccurring.Clear();
		SubtaskStatusController subtaskStatusController;
		if (includingJobData)
		{
			for (int i = 0; i < JobStatusController.TaskStatusControllerList.Count; i++)
			{
				TaskStatusController taskStatusController = JobStatusController.TaskStatusControllerList[i];
				for (int j = 0; j < taskStatusController.PageStatusControllerList.Count; j++)
				{
					PageStatusController pageStatusController = taskStatusController.PageStatusControllerList[j];
					for (int k = 0; k < pageStatusController.SubtaskStatusControllerList.Count; k++)
					{
						subtaskStatusController = pageStatusController.SubtaskStatusControllerList[k];
						if (subtaskStatusController.Data.ActionEventConditions == null)
						{
							continue;
						}
						for (int l = 0; l < subtaskStatusController.Data.ActionEventConditions.Count; l++)
						{
							if (subtaskStatusController.Data.ActionEventConditions[l].IsSetUpCorrectly())
							{
								AddSubtaskConditionToQuickLookupDictionary(subtaskStatusController, subtaskStatusController.Data.ActionEventConditions[l], pageStatusController.Data);
								continue;
							}
							Debug.LogError(taskStatusController.Data.name + "; " + subtaskStatusController.Data.name + "; Condition " + l + " not set up correctly.", taskStatusController.Data);
						}
					}
				}
			}
		}
		if (!isUsingConfirmationSystem)
		{
			return;
		}
		subtaskStatusController = betweenTasksSubtaskStatusController;
		if (subtaskStatusController.Data.ActionEventConditions == null)
		{
			return;
		}
		for (int m = 0; m < subtaskStatusController.Data.ActionEventConditions.Count; m++)
		{
			if (subtaskStatusController.Data.ActionEventConditions[m].IsSetUpCorrectly())
			{
				AddSubtaskConditionToQuickLookupDictionary(subtaskStatusController, subtaskStatusController.Data.ActionEventConditions[m], null);
				continue;
			}
			Debug.LogError("JOB BETWEEN TASKS SUBTASK; " + subtaskStatusController.Data.name + "; Condition " + m + " not set up correctly.", subtaskStatusController.Data);
		}
	}

	public void ManuallyRegisterSubtaskCondition(SubtaskStatusController subtaskStatus)
	{
		if (subtaskStatus.Data.ActionEventConditions == null)
		{
			return;
		}
		for (int i = 0; i < subtaskStatus.Data.ActionEventConditions.Count; i++)
		{
			if (subtaskStatus.Data.ActionEventConditions[i].IsSetUpCorrectly())
			{
				AddSubtaskConditionToQuickLookupDictionary(subtaskStatus, subtaskStatus.Data.ActionEventConditions[i], null);
			}
			else
			{
				Debug.LogError(subtaskStatus.Data.name + "; Condition " + i + " not set up correctly.", subtaskStatus.Data);
			}
		}
	}

	public void ManuallyUnregisterSubtaskCondition(SubtaskStatusController subtaskStatus)
	{
		if (subtaskStatus.Data.ActionEventConditions == null)
		{
			return;
		}
		for (int i = 0; i < subtaskStatus.Data.ActionEventConditions.Count; i++)
		{
			if (!subtaskStatus.Data.ActionEventConditions[i].IsSetUpCorrectly())
			{
				continue;
			}
			ActionEventCondition actionEventCondition = subtaskStatus.Data.ActionEventConditions[i];
			sb.Length = 0;
			sb.Append(actionEventCondition.ActionEventData.ActionEventName);
			sb.Append(":");
			sb.Append(actionEventCondition.WorldItemData1.ItemName);
			if (actionEventCondition.ActionEventData.FormatType == ActionEventData.FormatTypes.WorldItemDataAppliedToWorldItemData)
			{
				sb.Append(":");
				sb.Append(actionEventCondition.WorldItemData2.ItemName);
			}
			SubTaskActionEventContainerListObject value;
			if (quickLookupForActionEventsOccurring.TryGetValue(sb.ToString(), out value))
			{
				for (int j = 0; j < value.ContainerList.Count; j++)
				{
					if (value.ContainerList[j].SubtaskStatusController == subtaskStatus)
					{
						value.ContainerList.RemoveAt(j);
						j--;
					}
				}
			}
			AddSubtaskConditionToQuickLookupDictionary(subtaskStatus, subtaskStatus.Data.ActionEventConditions[i], null);
		}
	}

	private void AddSubtaskConditionToQuickLookupDictionary(SubtaskStatusController subtaskStatus, ActionEventCondition condition, PageData parentPage)
	{
		SubTaskActionEventConditionsContainer item = new SubTaskActionEventConditionsContainer(subtaskStatus, condition, parentPage);
		sb.Length = 0;
		sb.Append(condition.ActionEventData.ActionEventName);
		sb.Append(":");
		sb.Append(condition.WorldItemData1.ItemName);
		if (condition.ActionEventData.FormatType == ActionEventData.FormatTypes.WorldItemDataAppliedToWorldItemData)
		{
			sb.Append(":");
			sb.Append(condition.WorldItemData2.ItemName);
		}
		SubTaskActionEventContainerListObject value;
		if (quickLookupForActionEventsOccurring.TryGetValue(sb.ToString(), out value))
		{
			value.ContainerList.Add(item);
			return;
		}
		value = new SubTaskActionEventContainerListObject();
		value.ContainerList = new List<SubTaskActionEventConditionsContainer>();
		value.ContainerList.Add(item);
		quickLookupForActionEventsOccurring.Add(sb.ToString(), value);
	}

	private void BeginJob()
	{
		if (GlobalStorage.Instance.StartingTaskIndex == -1)
		{
			Debug.Log("You are in free-play mode, no tasks will run!");
			if (OnSandboxPhaseStarted != null)
			{
				OnSandboxPhaseStarted();
			}
			taskDisplayController.SetJobCompleted();
			if (USE_GAMEOBJECT_ACTIVATION_FOR_CONTENTS)
			{
				outroScreenAnimation.gameObject.SetActive(true);
			}
			else
			{
				outroScreenAnimation.transform.SetLocalPositionZOnly(0f);
			}
			StartCoroutine(PlayAnimationForFreeplayMode());
		}
		else
		{
			JobStatusController.SetToStartingTask();
			StartWaitingForContinue();
			MarkAsDirty();
		}
	}

	private IEnumerator PlayAnimationForFreeplayMode()
	{
		SetIsConstantlyDirty(true);
		outroScreenAnimation.clip = screenInAnimationClip;
		outroScreenAnimation.Play();
		yield return new WaitForSeconds(screenInAnimationClip.length + 0.1f);
		SetIsConstantlyDirty(false);
	}

	private IEnumerator CurrentTaskIncoming()
	{
		TaskStatusController taskStatus = ((!isInEndlessMode) ? JobStatusController.GetCurrentTask() : EndlessModeStatusController.BeginNextGoal());
		if (OnTaskStarted != null)
		{
			OnTaskStarted(taskStatus);
		}
		MarkAsDirty();
		yield return new WaitForSeconds(taskStatus.Data.SecsOfBlankBeforeShowing);
		SetTaskDisplayToCurrentTask(taskStatus);
		MarkAsDirty();
		if (isInEndlessMode && OnBeganWaitingForSkipAction != null)
		{
			yield return new WaitForSeconds(EndlessModeStatusController.Data.SecsBeforeSkipItemAvailable);
			OnBeganWaitingForSkipAction();
		}
	}

	private IEnumerator CurrentTaskOutgoing()
	{
		SetIsConstantlyDirty(true);
		TaskStatusController taskStatus = ((!isInEndlessMode) ? JobStatusController.GetCurrentTask() : EndlessModeStatusController.GetCurrentGoal());
		TaskStatusController endingTask = taskStatus;
		yield return new WaitForSeconds(1.2f);
		float inAnimLength = taskDisplayController.OpenTaskCompleteGraphicAndGetLength();
		yield return new WaitForSeconds(inAnimLength);
		yield return new WaitForSeconds(taskStatus.Data.SecsOfTaskCompleteScreenAtEnd + TaskData.MINIMUM_SECS_OF_TASK_COMPLETE_GRAPHIC);
		float outAnimLength = taskDisplayController.CloseTaskCompleteGraphicAndGetLength();
		yield return new WaitForSeconds(outAnimLength);
		float hideScreenLength = taskDisplayController.DisappearAndGetLength();
		yield return new WaitForSeconds(hideScreenLength);
		SetIsConstantlyDirty(false);
		yield return new WaitForSeconds(taskStatus.Data.SecsOfBlankAfterCompleting);
		if (isInEndlessMode)
		{
			while (!EndlessModeStatusController.readyToBeginNextGoal)
			{
				yield return null;
			}
		}
		if (isInEndlessMode && EndlessModeStatusController.ShouldGetPromotion() && !taskStatus.IsSkipped)
		{
			GameEventsManager.Instance.ScriptedCauseOccurred("PlayerPromotionEndless");
			yield return new WaitForSeconds(EndlessModeStatusController.Data.SecsOfBlankWhenPromoting);
		}
		if (OnTaskEnded != null)
		{
			OnTaskEnded(endingTask);
		}
		if (isInEndlessMode || !JobStatusController.IsCompleted)
		{
			bool wasUsingConfirmationSystem = isUsingConfirmationSystem;
			if (endingTask.IsSkipped)
			{
				isUsingConfirmationSystem = false;
			}
			StartWaitingForContinue();
			isUsingConfirmationSystem = wasUsingConfirmationSystem;
			yield break;
		}
		yield return new WaitForSeconds(JobStatusController.Data.SecsOfBlankAfterCompletingJob);
		SetIsConstantlyDirty(true);
		if (USE_GAMEOBJECT_ACTIVATION_FOR_CONTENTS)
		{
			outroScreenAnimation.gameObject.SetActive(true);
		}
		else
		{
			outroScreenAnimation.transform.SetLocalPositionZOnly(0f);
		}
		outroScreenAnimation.clip = screenInAnimationClip;
		outroScreenAnimation.Play();
		yield return new WaitForSeconds(screenInAnimationClip.length);
		if (OnSandboxPhaseStarted != null)
		{
			OnSandboxPhaseStarted();
		}
		SetIsConstantlyDirty(false);
	}

	private void StartWaitingForContinue()
	{
		if (isUsingConfirmationSystem)
		{
			SetIsConstantlyDirty(true);
			if (USE_GAMEOBJECT_ACTIVATION_FOR_CONTENTS)
			{
				continueScreenAnimation.gameObject.SetActive(true);
			}
			else
			{
				continueScreenAnimation.transform.SetLocalPositionZOnly(0f);
			}
			continueScreenAnimation.clip = screenInAnimationClip;
			continueScreenAnimation.Play();
			StartCoroutine(WaitAndSetIsConstantlyDirty(false, screenInAnimationClip.length + 0.1f));
			betweenTasksSubtaskStatusController.ForceUncomplete();
			betweenTasksSubtaskDisplayController.SetNewSubtask(betweenTasksSubtaskStatusController);
			isWaitingForConfirmation = true;
			if (OnBeganWaitingForConfirmation != null)
			{
				OnBeganWaitingForConfirmation();
			}
			if (onlyUseConfirmationOnce)
			{
				isUsingConfirmationSystem = false;
			}
		}
		else
		{
			StartCoroutine(CurrentTaskIncoming());
		}
		MarkAsDirty();
	}

	private void BetweenTasksSubtaskComplete(SubtaskStatusController status)
	{
		SendConfirmation();
	}

	public void SendConfirmation()
	{
		if (isWaitingForConfirmation)
		{
			isWaitingForConfirmation = false;
			StartCoroutine(ContinueToNextTask());
		}
	}

	private IEnumerator ContinueToNextTask()
	{
		audioSrcHelper.SetClip(continueCompleteAudioClip);
		audioSrcHelper.Play();
		yield return new WaitForSeconds(0.25f);
		SetIsConstantlyDirty(true);
		if (USE_GAMEOBJECT_ACTIVATION_FOR_CONTENTS)
		{
			continueScreenAnimation.gameObject.SetActive(true);
		}
		else
		{
			continueScreenAnimation.transform.SetLocalPositionZOnly(0f);
		}
		continueScreenAnimation.clip = screenOutAnimationClip;
		continueScreenAnimation.Play();
		yield return new WaitForSeconds(screenOutAnimationClip.length);
		if (USE_GAMEOBJECT_ACTIVATION_FOR_CONTENTS)
		{
			outroScreenAnimation.gameObject.SetActive(false);
		}
		else
		{
			outroScreenAnimation.transform.SetLocalPositionZOnly(-10f);
		}
		SetIsConstantlyDirty(false);
		yield return new WaitForSeconds(0.5f);
		StartCoroutine(CurrentTaskIncoming());
	}

	private void SetTaskDisplayToCurrentTask(TaskStatusController taskThatHadStarted)
	{
		TaskStatusController taskStatusController = ((!isInEndlessMode) ? JobStatusController.GetCurrentTask() : EndlessModeStatusController.GetCurrentGoal());
		if (taskStatusController.Data == taskThatHadStarted.Data)
		{
			SetIsConstantlyDirty(true);
			float t = taskDisplayController.AppearAndGetLength();
			StartCoroutine(WaitAndSetIsConstantlyDirty(false, t));
			if (taskStatusController != null && (isInEndlessMode || !JobStatusController.IsCompleted))
			{
				if (OnTaskShown != null)
				{
					OnTaskShown(taskStatusController);
				}
				if (isInEndlessMode)
				{
					StartCoroutine(taskDisplayController.SetTaskAsync(taskStatusController));
				}
				else
				{
					taskDisplayController.SetTask(taskStatusController);
				}
			}
			else
			{
				Debug.Log("No new task to set - the job is completed.");
			}
		}
		else
		{
			Debug.Log("You completed a task (" + taskThatHadStarted.Data.name + ") during it's animation in time, starting the next one (" + taskStatusController.Data.name + ") now");
			StartCoroutine(CurrentTaskIncoming());
		}
		MarkAsDirty();
	}

	public void HackForceJobComplete()
	{
		if (isInEndlessMode)
		{
			Debug.LogError("Can't hack complete while in endless mode!");
			return;
		}
		StopAllCoroutines();
		JobComplete(JobStatusController);
		taskDisplayController.HackForceCompleteAnimationAndGetLength();
		SetIsConstantlyDirty(true);
		if (USE_GAMEOBJECT_ACTIVATION_FOR_CONTENTS)
		{
			taskDisplayController.gameObject.SetActive(false);
			outroScreenAnimation.gameObject.SetActive(true);
		}
		else
		{
			taskDisplayController.transform.SetLocalPositionZOnly(-10f);
			outroScreenAnimation.transform.SetLocalPositionZOnly(0f);
		}
		outroScreenAnimation.clip = screenInAnimationClip;
		outroScreenAnimation.Play();
		StartCoroutine(HackWaitAndStopRenderingForever(outroScreenAnimation.clip.length + 0.05f));
	}

	private IEnumerator HackWaitAndStopRenderingForever(float t)
	{
		yield return new WaitForSeconds(t);
		yield return null;
		renderTextureCamera.Render();
		hackDisableRendering = true;
		SetIsConstantlyDirty(false);
	}

	private void JobComplete(JobStatusController jobStatus)
	{
		if (OnJobComplete != null)
		{
			OnJobComplete(jobStatus);
		}
		taskDisplayController.SetJobCompleted();
		MarkAsDirty();
	}

	private void TaskComplete(TaskStatusController taskStatus)
	{
		if (isInEndlessMode)
		{
			StartCoroutine(EndlessTaskCompleteRoutine(taskStatus));
			return;
		}
		if (OnTaskComplete != null)
		{
			OnTaskComplete(taskStatus);
		}
		if (taskDisplayController.IsTaskCurrentlyDisplayed(taskStatus))
		{
			StartCoroutine(WaitAndPlayTaskCompleteAudio(0.65f));
		}
		if (taskDisplayController.IsTaskCurrentlyDisplayed(taskStatus))
		{
			StartCoroutine(CurrentTaskOutgoing());
		}
		MarkAsDirty();
	}

	private IEnumerator EndlessTaskCompleteRoutine(TaskStatusController taskStatus)
	{
		yield return null;
		if (OnTaskComplete != null)
		{
			OnTaskComplete(taskStatus);
		}
		yield return null;
		if (taskDisplayController.IsTaskCurrentlyDisplayed(taskStatus))
		{
			StartCoroutine(WaitAndPlayTaskCompleteAudio(0.65f));
		}
		if (taskDisplayController.IsTaskCurrentlyDisplayed(taskStatus))
		{
			StartCoroutine(CurrentTaskOutgoing());
		}
		MarkAsDirty();
	}

	private void PageComplete(PageStatusController pageStatus)
	{
		if (OnPageComplete != null)
		{
			OnPageComplete(pageStatus);
		}
		if (taskDisplayController.IsPageCurrentlyDisplayed(pageStatus))
		{
			StartCoroutine(WaitAndPlayPageSwipeAudio(0.4f));
		}
		MarkAsDirty();
	}

	private IEnumerator WaitAndPlayPageSwipeAudio(float waitTime)
	{
		yield return new WaitForSeconds(waitTime);
		audioSrcHelper.SetClip(pageAdvanceAudioClip);
		audioSrcHelper.Play();
	}

	private IEnumerator WaitAndPlayTaskCompleteAudio(float waitTime)
	{
		yield return new WaitForSeconds(waitTime);
		if (GenieManager.AreAnyJobGenieModesActive() && GenieManager.DoesContainGenieMode(GlobalStorage.Instance.CurrentGenieModes, JobGenieCartridge.GenieModeTypes.EndlessMode))
		{
			if (!instance.EndlessModeStatusController.GetCurrentGoal().IsSkipped)
			{
				audioSrcHelper.SetClip(taskCompleteAudioClip);
				if (GlobalStorage.Instance.GameStateData.NumberOfCompletedEndlessTasks() % EndlessModeStatusController.NumberOfTasksForPromotion != 0)
				{
					audioSrcHelper.Play();
				}
			}
			else
			{
				audioSrcHelper.SetClip(taskCompleteFailedAudioClip);
				audioSrcHelper.Play();
			}
		}
		else
		{
			audioSrcHelper.SetClip(taskCompleteAudioClip);
			audioSrcHelper.Play();
		}
	}

	private void SubtaskCounterChange(SubtaskStatusController subtaskStatus, bool isPositive)
	{
		if (OnSubtaskCounterChange != null)
		{
			OnSubtaskCounterChange(subtaskStatus, isPositive);
		}
		if (isPositive && taskDisplayController.IsSubtaskCurrentlyDisplayed(subtaskStatus) && !subtaskStatus.Data.HideCounterOnJobBoard)
		{
			audioSrcHelper.SetClip(subtaskCounterProgressAudioClip);
			audioSrcHelper.Play();
		}
		MarkAsDirty();
	}

	private void SubtaskComplete(SubtaskStatusController subtaskStatus)
	{
		if (OnSubtaskComplete != null)
		{
			OnSubtaskComplete(subtaskStatus);
		}
		if (taskDisplayController.IsSubtaskCurrentlyDisplayed(subtaskStatus))
		{
			audioSrcHelper.SetClip(subtaskSuccessAudioClip);
			audioSrcHelper.Play();
		}
		MarkAsDirty();
	}

	private void SubtaskUncomplete(SubtaskStatusController subtaskStatus)
	{
		if (OnSubtaskUncomplete != null)
		{
			OnSubtaskUncomplete(subtaskStatus);
		}
		MarkAsDirty();
	}

	public void ItemActionOccurred(ActionEventData actionEventData, WorldItemData worldItemData)
	{
		sb.Length = 0;
		sb.Append(actionEventData.ActionEventName);
		sb.Append(":");
		sb.Append(worldItemData.ItemName);
		if (quickLookupForActionEventsOccurring.ContainsKey(sb.ToString()))
		{
			SubTaskActionEventContainerListObject subTaskActionEventContainerListObject = quickLookupForActionEventsOccurring[sb.ToString()];
			if (isInEndlessMode)
			{
				subTaskActionEventContainerListObject.ActionOccurred(EndlessModeStatusController.GetCurrentPageData());
			}
			else
			{
				subTaskActionEventContainerListObject.ActionOccurred(JobStatusController.GetCurrentPageData());
			}
		}
	}

	public void ItemActionOccurredWithAmount(ActionEventData actionEventData, WorldItemData worldItemData, float amount)
	{
		sb.Length = 0;
		sb.Append(actionEventData.ActionEventName);
		sb.Append(":");
		sb.Append(worldItemData.ItemName);
		if (quickLookupForActionEventsOccurring.ContainsKey(sb.ToString()))
		{
			SubTaskActionEventContainerListObject subTaskActionEventContainerListObject = quickLookupForActionEventsOccurring[sb.ToString()];
			if (isInEndlessMode)
			{
				subTaskActionEventContainerListObject.ActionOccurredWithAmount(amount, EndlessModeStatusController.GetCurrentPageData());
			}
			else
			{
				subTaskActionEventContainerListObject.ActionOccurredWithAmount(amount, JobStatusController.GetCurrentPageData());
			}
		}
	}

	public void ItemAppliedToItemActionOccurred(ActionEventData actionEventData, WorldItemData worldItemData, WorldItemData appliedToWorldItemData)
	{
		sb.Length = 0;
		sb.Append(actionEventData.ActionEventName);
		sb.Append(":");
		sb.Append(worldItemData.ItemName);
		sb.Append(":");
		sb.Append(appliedToWorldItemData.ItemName);
		if (quickLookupForActionEventsOccurring.ContainsKey(sb.ToString()))
		{
			SubTaskActionEventContainerListObject subTaskActionEventContainerListObject = quickLookupForActionEventsOccurring[sb.ToString()];
			if (isInEndlessMode)
			{
				subTaskActionEventContainerListObject.ActionOccurred(EndlessModeStatusController.GetCurrentPageData());
			}
			else
			{
				subTaskActionEventContainerListObject.ActionOccurred(JobStatusController.GetCurrentPageData());
			}
		}
	}

	public void ItemAppliedToItemActionOccurredWithAmount(ActionEventData actionEventData, WorldItemData worldItemData, WorldItemData appliedToWorldItemData, float amount)
	{
		sb.Length = 0;
		sb.Append(actionEventData.ActionEventName);
		sb.Append(":");
		sb.Append(worldItemData.ItemName);
		sb.Append(":");
		sb.Append(appliedToWorldItemData.ItemName);
		if (quickLookupForActionEventsOccurring.ContainsKey(sb.ToString()))
		{
			SubTaskActionEventContainerListObject subTaskActionEventContainerListObject = quickLookupForActionEventsOccurring[sb.ToString()];
			if (isInEndlessMode)
			{
				subTaskActionEventContainerListObject.ActionOccurredWithAmount(amount, EndlessModeStatusController.GetCurrentPageData());
			}
			else
			{
				subTaskActionEventContainerListObject.ActionOccurredWithAmount(amount, JobStatusController.GetCurrentPageData());
			}
		}
	}

	private void PageShown(PageStatusController pageStatus)
	{
		if (OnPageShown != null)
		{
			OnPageShown(pageStatus);
		}
	}

	private void PageStarted(PageStatusController pageStatus)
	{
		if (OnPageStarted != null)
		{
			OnPageStarted(pageStatus);
		}
	}

	private void PageEnded(PageStatusController pageStatus)
	{
		if (OnPageEnded != null)
		{
			OnPageEnded(pageStatus);
		}
	}

	public void TestingForceCurrentTaskComplete()
	{
		if (isWaitingForConfirmation && isUsingConfirmationSystem)
		{
			SendConfirmation();
		}
		if (isInEndlessMode)
		{
			EndlessModeStatusController.GetCurrentGoal().ForceComplete();
		}
		else
		{
			JobStatusController.GetCurrentTask().ForceComplete();
		}
		MarkAsDirty();
	}

	public void TestingForceCurrentSubtaskComplete()
	{
		if (isWaitingForConfirmation && isUsingConfirmationSystem)
		{
			SendConfirmation();
		}
		else if ((JobStatusController != null && !JobStatusController.IsCompleted) || isInEndlessMode)
		{
			TaskStatusController taskStatusController = ((!isInEndlessMode) ? JobStatusController.GetCurrentTask() : EndlessModeStatusController.GetCurrentGoal());
			if (taskStatusController != null)
			{
				SubtaskStatusController nextUncompletedSubtask = taskStatusController.GetCurrentPage().GetNextUncompletedSubtask();
				if (nextUncompletedSubtask != null)
				{
					nextUncompletedSubtask.ForceComplete();
				}
			}
			else
			{
				Debug.Log("Can't jump to next subtask because you're done!");
			}
		}
		else
		{
			Debug.Log("Job already completed!");
		}
		MarkAsDirty();
	}

	private void PlayPromotionSound()
	{
		if (promotionAudioClips.Length > 0)
		{
			int score = instance.EndlessModeStatusController.Score;
			int secondaryRank = PromotionRankNameGenerator.GetSecondaryRank(score);
			secondaryRank %= promotionAudioClips.Length;
			audioSrcHelper2d.SetClip(promotionAudioClips[secondaryRank]);
			audioSrcHelper2d.Play();
		}
	}
}
