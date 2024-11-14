using System.Collections;
using System.Collections.Generic;
using OwlchemyVR;
using OwlchemyVR2;
using UnityEngine;
using UnityEngine.UI;

public class TerminalUIController : MonoBehaviour
{
	private enum DisplayState
	{
		SelectTaskPage = 0,
		LoadingPage = 1,
		Off = 2,
		GetReady = 3,
		Disabled = 4,
		NeedGoldCartridgeError = 5,
		GetReadyOvertime = 6
	}

	private const int MAX_TASK_CELLS = 20;

	private const string OVERTIME_SCORE_PREFIX = "\nTasks Completed: ";

	private const string OFFICE_WORKER = "Office Worker";

	private const string GOURMET_CHEF = "Gourmet Chef";

	private const string STORE_CLERK = "Store Clerk";

	private const string AUTO_MECHANIC = "Auto Mechanic";

	private const string DEFAULT_JOB_NAME = "Infinite Overtime";

	private const float loadingBarAnimationTime = 0.6f;

	private const float pickLatestTaskAnimationTime = 0.5f;

	[SerializeField]
	private Transform sliderAnimateTransform;

	[SerializeField]
	private GrabbableItem grabbableSliderItem;

	private Vector3 collapsedSliderSize = new Vector3(0.3f, 1f, 0.3f);

	private Vector3 collapsedSliderPosition = new Vector3(0f, -0.1f, 0f);

	[SerializeField]
	private Transform sliderBlockerTransform;

	private Vector3 slotBlockerOpenPosition = new Vector3(0f, -0.0465f, -0.047f);

	private Vector3 slotBlockerClosedPosition = new Vector3(0f, -0.0465f, 0f);

	private bool sliderHidden;

	[SerializeField]
	private OwlchemyVR2.GrabbableSlider taskSelectionSlider;

	[Header("Page: Insert Cartridge")]
	[SerializeField]
	private CanvasGroup insertCartridgePage;

	[SerializeField]
	[Header("Page: Loading")]
	private CanvasGroup loadingPage;

	[SerializeField]
	private Text jobName;

	[SerializeField]
	private Slider loadingSlider;

	[SerializeField]
	private Text loadingPercentage;

	[SerializeField]
	[Header("Page: SelectTask")]
	private CanvasGroup selectTaskPage;

	[SerializeField]
	private Scrollbar tasksSlider;

	[SerializeField]
	private Text taskName;

	[SerializeField]
	private Transform tasksContainer;

	[Header("Page: Get Ready")]
	[SerializeField]
	private CanvasGroup getReadyPage;

	[SerializeField]
	private MuseumTerminalTaskCell cellPrefab;

	[Header("Infinite Overtime")]
	[SerializeField]
	private CanvasGroup needGoldCartridgeErrorText;

	[SerializeField]
	private CanvasGroup getReadyOvertime;

	[SerializeField]
	private Text jobDisplayText;

	[SerializeField]
	private Text scoreText;

	private List<MuseumTerminalTaskCell> spawnedCells = new List<MuseumTerminalTaskCell>();

	private int currentTask;

	private int totalTasks;

	private int targetTask;

	private int defaultTaskIDOfMostRecentlyLoadedJob;

	private DisplayState currentDisplayState;

	private JobCartridge currentCartridge;

	private bool isCurrentCartridgeGamedev;

	[Header("Custom Icon For Sandbox")]
	[SerializeField]
	private Sprite sandboxModeIcon;

	[SerializeField]
	private AudioSourceHelper audioSource;

	[SerializeField]
	private AudioClip uiNavigationSound;

	private bool sandboxMode;

	private bool isAnimating;

	private bool isHidingSlider;

	private void Awake()
	{
		SetDisplayState(DisplayState.Disabled);
		for (int i = 0; i < 20; i++)
		{
			MuseumTerminalTaskCell museumTerminalTaskCell = Object.Instantiate(cellPrefab);
			museumTerminalTaskCell.transform.SetParent(tasksContainer, false);
			spawnedCells.Add(museumTerminalTaskCell);
			museumTerminalTaskCell.transform.localPosition = new Vector3(museumTerminalTaskCell.transform.localPosition.x, museumTerminalTaskCell.transform.localPosition.y, 50f);
		}
	}

	private void OnEnable()
	{
		FinishHidingSlider();
		if (currentDisplayState != DisplayState.Disabled)
		{
			SetDisplayState(DisplayState.Disabled);
		}
	}

	private void OnDisable()
	{
		if (isHidingSlider)
		{
			FinishHidingSlider();
		}
	}

	private IEnumerator HidePhysicalSlider(bool instant = false)
	{
		if (!sliderHidden)
		{
			isHidingSlider = true;
			if (grabbableSliderItem.IsCurrInHand && grabbableSliderItem.CurrInteractableHand != null)
			{
				grabbableSliderItem.CurrInteractableHand.TryRelease();
			}
			grabbableSliderItem.enabled = false;
			sliderHidden = true;
			Go.killAllTweensWithTarget(sliderAnimateTransform);
			Go.killAllTweensWithTarget(sliderBlockerTransform);
			if (instant)
			{
				sliderAnimateTransform.localScale = collapsedSliderSize;
				sliderAnimateTransform.localPosition = collapsedSliderPosition;
				sliderBlockerTransform.localPosition = slotBlockerClosedPosition;
			}
			else
			{
				Go.to(sliderAnimateTransform, 0.3f, new GoTweenConfig().scale(collapsedSliderSize).setEaseType(GoEaseType.QuadIn));
				Go.to(sliderAnimateTransform, 0.3f, new GoTweenConfig().localPosition(collapsedSliderPosition).setEaseType(GoEaseType.QuadInOut).setDelay(0.3f));
				Go.to(sliderBlockerTransform, 0.1f, new GoTweenConfig().localPosition(slotBlockerClosedPosition).setEaseType(GoEaseType.QuadIn).setDelay(0.6f));
				yield return new WaitForSeconds(0.6f);
			}
			FinishHidingSlider();
		}
	}

	private void FinishHidingSlider()
	{
		grabbableSliderItem.enabled = false;
		sliderHidden = true;
		Go.killAllTweensWithTarget(sliderAnimateTransform);
		Go.killAllTweensWithTarget(sliderBlockerTransform);
		sliderAnimateTransform.localScale = collapsedSliderSize;
		sliderAnimateTransform.localPosition = collapsedSliderPosition;
		sliderBlockerTransform.localPosition = slotBlockerClosedPosition;
		isHidingSlider = false;
	}

	public void LoadJob(JobCartridgeWithGenieFlags cartridgeToLoadWithGenies)
	{
		JobCartridge baseJobCartridge = cartridgeToLoadWithGenies.BaseJobCartridge;
		bool flag = GenieManager.DoesContainGenieMode(cartridgeToLoadWithGenies.GenieFlags, JobGenieCartridge.GenieModeTypes.OfficeModMode);
		isCurrentCartridgeGamedev = flag && baseJobCartridge.StateDataForGamedev != null;
		JobStateData jobStateData = ((!isCurrentCartridgeGamedev) ? baseJobCartridge.StateData : baseJobCartridge.StateDataForGamedev);
		if (GetComponent<TerminalManager>().LoadOvertimeMode)
		{
			DoEndlessModeUI(cartridgeToLoadWithGenies.BaseJobCartridge);
			return;
		}
		currentCartridge = baseJobCartridge;
		currentTask = jobStateData.GetIndexOfNextUnCompletedTask();
		totalTasks = jobStateData.TasksData.Count;
		sandboxMode = currentTask == -1;
		defaultTaskIDOfMostRecentlyLoadedJob = currentTask;
		jobName.text = jobStateData.JobLevelData.FullName;
		for (int i = 0; i < 20; i++)
		{
			if (i <= totalTasks)
			{
				if (i == totalTasks)
				{
					spawnedCells[i].SetValues(jobStateData.JobLevelData.SceneName, -1, true);
					spawnedCells[i].cellImage.sprite = sandboxModeIcon;
					spawnedCells[i].cellText.text = string.Empty;
				}
				else
				{
					spawnedCells[i].cellImage.sprite = null;
					spawnedCells[i].SetValues(jobStateData.JobLevelData.SceneName, i, jobStateData.TasksData[i].IsCompleted);
				}
				spawnedCells[i].transform.localPosition = new Vector3(spawnedCells[i].transform.localPosition.x, spawnedCells[i].transform.localPosition.y, 0f);
			}
			else
			{
				spawnedCells[i].transform.localPosition = new Vector3(spawnedCells[i].transform.localPosition.x, spawnedCells[i].transform.localPosition.y, 50f);
			}
		}
		SetDisplayState(DisplayState.LoadingPage);
		StartCoroutine(LoadingSequence());
	}

	private void DoEndlessModeUI(JobCartridge cartridge)
	{
		if (cartridge.StateData.GetPercentageComplete() == 1f)
		{
			switch (cartridge.StateData.JobLevelData.SceneName)
			{
			case "Office":
				scoreText.text = "Office Worker";
				break;
			case "Kitchen":
				scoreText.text = "Gourmet Chef";
				break;
			case "ConvenienceStore":
				scoreText.text = "Store Clerk";
				break;
			case "AutoMechanic":
				scoreText.text = "Auto Mechanic";
				break;
			default:
				scoreText.text = "Infinite Overtime";
				break;
			}
			scoreText.text += string.Format("\n<size=45%>JOB Title: {0}</size>", PromotionRankNameGenerator.GetRankName(GlobalStorage.Instance.GameStateData.NumberOfCompletedEndlessTasks()));
			SetDisplayState(DisplayState.GetReadyOvertime);
		}
		else
		{
			SetDisplayState(DisplayState.NeedGoldCartridgeError);
		}
	}

	private IEnumerator LoadingSequence()
	{
		targetTask = currentTask;
		isAnimating = true;
		loadingSlider.value = 0f;
		loadingPercentage.text = "0%";
		float t2 = 0f - Time.deltaTime;
		while (t2 < 1f)
		{
			if (currentDisplayState == DisplayState.Off || currentDisplayState == DisplayState.GetReadyOvertime || currentDisplayState == DisplayState.NeedGoldCartridgeError)
			{
				yield break;
			}
			t2 = Mathf.Min(t2 + Time.deltaTime / 0.6f, 1f);
			loadingSlider.value = t2;
			loadingPercentage.text = Mathf.FloorToInt(t2 * 100f) + "%";
			yield return null;
		}
		if (sliderHidden && currentTask != 0)
		{
			Debug.Log("Slider revealing!");
			sliderHidden = false;
			Go.killAllTweensWithTarget(sliderAnimateTransform);
			Go.killAllTweensWithTarget(sliderBlockerTransform);
			Go.to(sliderBlockerTransform, 0.1f, new GoTweenConfig().localPosition(slotBlockerOpenPosition).setEaseType(GoEaseType.QuadInOut));
			Go.to(sliderAnimateTransform, 0.15f, new GoTweenConfig().localPosition(Vector3.zero).setEaseType(GoEaseType.QuadInOut).setDelay(0.05f));
			Go.to(sliderAnimateTransform, 0.3f, new GoTweenConfig().scale(Vector3.one).setEaseType(GoEaseType.ElasticOut).setDelay(0.2f)
				.onComplete(TweenComplete));
		}
		yield return new WaitForSeconds(0.5f);
		if (currentDisplayState == DisplayState.Off || currentDisplayState == DisplayState.GetReadyOvertime || currentDisplayState == DisplayState.NeedGoldCartridgeError)
		{
			StartCoroutine(HidePhysicalSlider());
			yield break;
		}
		SetDisplayState(DisplayState.SelectTaskPage);
		if (currentDisplayState == DisplayState.SelectTaskPage)
		{
			float sliderAnimateToAmount = (float)currentTask / (float)totalTasks;
			targetTask = currentTask;
			if (sandboxMode)
			{
				sliderAnimateToAmount = 0f;
				targetTask = 0;
			}
			float initialTaskSliderValue = taskSelectionSlider.NormalizedAxisValue;
			UpdateSelectedTask(taskSelectionSlider.NormalizedAxisValue, true);
			t2 = 0f - Time.deltaTime;
			while (t2 < 1f)
			{
				if (currentDisplayState == DisplayState.Off || currentDisplayState == DisplayState.GetReadyOvertime || currentDisplayState == DisplayState.NeedGoldCartridgeError)
				{
					yield break;
				}
				t2 = Mathf.Min(t2 + Time.deltaTime / 0.5f, 1f);
				taskSelectionSlider.SetNormalizedAmount(Mathf.Lerp(initialTaskSliderValue, sliderAnimateToAmount, t2));
				yield return null;
			}
			grabbableSliderItem.enabled = true;
		}
		else
		{
			targetTask = 0;
		}
		isAnimating = false;
	}

	private void TweenComplete(AbstractGoTween tween)
	{
	}

	public void PowerOffTerminal()
	{
		SetDisplayState(DisplayState.Off);
	}

	private void SetDisplayState(DisplayState state)
	{
		loadingPage.alpha = 0f;
		selectTaskPage.alpha = 0f;
		insertCartridgePage.alpha = 0f;
		getReadyPage.alpha = 0f;
		needGoldCartridgeErrorText.alpha = 0f;
		getReadyOvertime.alpha = 0f;
		switch (state)
		{
		case DisplayState.LoadingPage:
			loadingPage.alpha = 1f;
			break;
		case DisplayState.SelectTaskPage:
			if (currentTask == 0)
			{
				state = DisplayState.GetReady;
				getReadyPage.alpha = 1f;
			}
			else
			{
				selectTaskPage.alpha = 1f;
			}
			break;
		case DisplayState.Off:
			insertCartridgePage.alpha = 1f;
			break;
		case DisplayState.NeedGoldCartridgeError:
			needGoldCartridgeErrorText.alpha = 1f;
			break;
		case DisplayState.GetReadyOvertime:
			getReadyOvertime.alpha = 1f;
			break;
		}
		if (state != 0)
		{
			if (base.gameObject.activeSelf)
			{
				StartCoroutine(HidePhysicalSlider(state == DisplayState.Disabled));
			}
			else
			{
				Debug.Log(base.gameObject.name + " is not active, so it won't bother hiding its slider. This is probably ok, because " + base.gameObject.name + " is likely not used in the layout you selected.", base.gameObject);
			}
		}
		currentDisplayState = state;
	}

	public void OnPhysicalSliderLocalPositionChanged(Vector3 newPos)
	{
		float num = 0f;
		if (currentDisplayState == DisplayState.SelectTaskPage || currentDisplayState == DisplayState.LoadingPage)
		{
			num = taskSelectionSlider.NormalizedAxisValue;
		}
		tasksSlider.value = Mathf.Lerp(0f, (float)totalTasks / 20f, num);
		if (currentDisplayState == DisplayState.SelectTaskPage)
		{
			UpdateSelectedTask(num);
		}
	}

	private void UpdateSelectedTask(float value, bool forceUpdate = false)
	{
		int num = Mathf.RoundToInt(value * (float)totalTasks);
		if (currentTask == -1)
		{
			currentTask = 0;
		}
		if (num == currentTask && !forceUpdate)
		{
			return;
		}
		spawnedCells[currentTask].SetSelected(false);
		spawnedCells[num].SetSelected(true);
		currentTask = num;
		if (currentTask == totalTasks)
		{
			taskName.text = "Free Play Mode";
		}
		else
		{
			JobStateData jobStateData = ((!isCurrentCartridgeGamedev || currentCartridge.StateDataForGamedev == null) ? currentCartridge.StateData : currentCartridge.StateDataForGamedev);
			if (currentTask >= 0 && currentTask < jobStateData.JobLevelData.JobData.Tasks.Count)
			{
				taskName.text = jobStateData.JobLevelData.JobData.Tasks[currentTask].TaskHeader;
			}
			else
			{
				taskName.text = string.Empty;
			}
		}
		audioSource.SetClip(uiNavigationSound);
		audioSource.Play();
	}

	public int GetDesiredTaskToLoad()
	{
		if (isAnimating)
		{
			if (defaultTaskIDOfMostRecentlyLoadedJob == -1)
			{
				return 0;
			}
			return defaultTaskIDOfMostRecentlyLoadedJob;
		}
		if (currentTask == totalTasks)
		{
			return -1;
		}
		return currentTask;
	}
}
