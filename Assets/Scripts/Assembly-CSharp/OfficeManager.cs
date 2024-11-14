using System;
using System.Collections;
using OwlchemyVR;
using UnityEngine;

public class OfficeManager : LevelManager
{
	private const float SECS_BEFORE_WORK_CLEARS = 256f;

	private const float SECS_BEFORE_WORK_ENDS = 260f;

	private const float SECS_BEFORE_BOSS_COMES = 286.5f;

	private const float SECS_BEFORE_END_OF_DEMO = 300f;

	private const float CIRCLE_HOLD_TIME = 3f;

	public const int maxAdjacentWorkerBots = 3;

	private const float SECS_LENGTH_OF_PARTY = 20f;

	private const float SECS_LENGTH_OF_LOOK_BUSY = 25f;

	[SerializeField]
	private bool ALWAYS_LOAD_GAMEDEV_JOB;

	[SerializeField]
	private JobData gamedevJobData;

	[SerializeField]
	private bool doTimedDemo;

	[SerializeField]
	private JobData demoJobData;

	[SerializeField]
	private TaskData demoTimingWaitTask;

	[SerializeField]
	private WorldItemData demoBossComesIn;

	[SerializeField]
	private ItemRecyclerManager itemRecyclerManager;

	private bool hasWorkCleared;

	private bool hasWorkEnded;

	private bool hasBossCome;

	private bool hasDemoCompleted;

	private bool throwingPlanes;

	private float nextPlaneThrow;

	private float timeSpent;

	private bool forcedComputerOn;

	public int currentAdjacentWorkerBots;

	private static OfficeManager instance;

	[SerializeField]
	private TaskData computerPluggedInAfterTask;

	[SerializeField]
	private AttachableObject[] plugAttachables;

	[SerializeField]
	private AttachablePoint[] plugAttachpoints;

	[SerializeField]
	private PoweredComputerHardwareController[] poweredHardwareToTurnOn;

	[SerializeField]
	private OfficeDesktopComputerProgram desktopProgram;

	[SerializeField]
	private SubtaskData confettiTrigger;

	[SerializeField]
	private TaskData partyTrigger;

	[SerializeField]
	private PageData pageToStartLookBusyTimer;

	[SerializeField]
	private WorldItemData lookBusyTimerWorldItem;

	[SerializeField]
	private MarqueeController marqueeController;

	[SerializeField]
	private GameObject marqueeControllerPSVR;

	[SerializeField]
	private Light mainLight;

	private float normalAmbientIntensity;

	private Color normalAmbientLight;

	private float normalLightIntensity;

	[SerializeField]
	private Transform projectorParent;

	[SerializeField]
	private Transform projectorHideObject;

	[SerializeField]
	private PageData projectorPage;

	[SerializeField]
	private MeshRenderer[] ceilingLights;

	[SerializeField]
	private Material ceilingLightOnMaterial;

	[SerializeField]
	private Material ceilingLightOffMaterial;

	[SerializeField]
	private AudioClip projectorMoveSound;

	[SerializeField]
	private Transform ProjectionScreenLocation;

	[SerializeField]
	private WorldItemData specialCaseWorldItemData;

	[SerializeField]
	private MailCartController[] mailCarts;

	[SerializeField]
	private ParticleSystem confettiLowEnd;

	[SerializeField]
	private ParticleSystem confettiHighEnd;

	[SerializeField]
	private ComputerController computerController;

	[SerializeField]
	private Transform airhornPosition;

	[SerializeField]
	private AudioClip airhornSound;

	[SerializeField]
	private AudioSourceHelper musicSource;

	[SerializeField]
	private AudioClip musicSound;

	[SerializeField]
	private Sprite internetPicsSprite;

	[SerializeField]
	private PageData pageForInternetPicsAlert;

	[SerializeField]
	private SubtaskData jamVendingMachineAfterSubtask;

	[SerializeField]
	private SubtaskData[] subtasksThatHelpStockValue;

	[SerializeField]
	private AudioClip bossbotIsComingVO;

	[SerializeField]
	private float paperPlaneThrowSpeed;

	[SerializeField]
	private float paperPlaneThrowSpeedNoise;

	[SerializeField]
	private float paperPlaneThrowHeadingNoise;

	[SerializeField]
	private AudioClip paperPlaneFightStartVO;

	[SerializeField]
	private AudioClip[] paperPlaneFightVO;

	[SerializeField]
	private GameObjectPrefabSpawner[] paperPlaneSpawners;

	[SerializeField]
	private Transform paperPlaneTarget;

	private ElementSequence<AudioClip> paperPlaneFightVOSequence;

	private MailCartController currentMailCart;

	private SlideshowPresentationController slideshowPresentation;

	private bool isPartyRunning;

	private int stockValue = -96;

	private PageData currentlyShownPage;

	private bool sentInFinalMailCart;

	private bool isInEndlessMode;

	[SerializeField]
	private HandshakeSimulator handshakeSimulator;

	[SerializeField]
	private PageData[] pagesThatRequireHandshakeSimulator;

	[SerializeField]
	private float handShakeSimulatorEnterDelay = 1f;

	[SerializeField]
	private float handShakeSimulatorExitDelay = 3f;

	[HideInInspector]
	public WorkerBotController currentJanitor;

	private MasterHMDAndInputController masterController;

	private Morpheus_IndividualController psvrLeftHand;

	private Morpheus_IndividualController psvrRightHand;

	private float circleTimer;

	private bool isExitingConferenceDemo;

	public bool AlwaysLoadGameDevJob
	{
		get
		{
			return ALWAYS_LOAD_GAMEDEV_JOB;
		}
	}

	public static OfficeManager Instance
	{
		get
		{
			return instance;
		}
	}

	public SlideshowPresentationController SlideshowPresentation
	{
		get
		{
			if (slideshowPresentation == null)
			{
				slideshowPresentation = projectorParent.GetComponentInChildren<SlideshowPresentationController>();
			}
			return slideshowPresentation;
		}
	}

	public override void Awake()
	{
		paperPlaneFightVOSequence = new ElementSequence<AudioClip>(paperPlaneFightVO);
		instance = this;
		UnityEngine.Object.Destroy(marqueeControllerPSVR);
		base.Awake();
	}

	private IEnumerator Start()
	{
		doTimedDemo = false;
		if (TempBuildSettingsHolder.CurrentBuildType == TempBuildSettingsHolder.DemoType.Office5minDemo)
		{
			doTimedDemo = true;
		}
		else
		{
			doTimedDemo = false;
		}
		JobData jobToUse = ((!doTimedDemo) ? jobData : demoJobData);
		isInEndlessMode = false;
		if (GenieManager.DoesContainGenieMode(GlobalStorage.Instance.CurrentGenieModes, JobGenieCartridge.GenieModeTypes.EndlessMode))
		{
			isInEndlessMode = true;
		}
		normalAmbientIntensity = RenderSettings.ambientIntensity;
		normalAmbientLight = RenderSettings.ambientLight;
		normalLightIntensity = mainLight.intensity;
		if (jobToUse == gamedevJobData)
		{
			Debug.LogWarning("GD job temporarily force disabled");
			jobToUse = ((!doTimedDemo) ? jobData : demoJobData);
		}
		if (jobToUse != gamedevJobData)
		{
			projectorParent.localPosition = Vector3.up * 5f;
			projectorHideObject.gameObject.SetActive(false);
			RefreshDefaultStockMessage(0);
		}
		if (isInEndlessMode)
		{
			ToggleFog();
			JobStatusController jobStatusController = new JobStatusController(jobData);
			while (endlessModeConfigData == null)
			{
				yield return null;
			}
			BotManager.Instance.InitializeEndlessMode(endlessModeConfigData);
			JobBoardManager.instance.InitEndlessMode(endlessModeConfigData, jobStatusController.JobStateData);
		}
		else
		{
			BotManager.Instance.InitializeJob(jobToUse);
			JobStateData jobStateData = JobBoardManager.instance.InitJob(jobToUse);
			if (jobStateData == null)
			{
				Debug.LogWarning("Not setting up the scene from jobStateData because " + jobToUse.name + " is not included in the game.");
			}
			else
			{
				SetupSceneFromJobStateData(jobStateData);
			}
		}
		for (int i = 0; i < mailCarts.Length; i++)
		{
			mailCarts[i].gameObject.SetActive(false);
		}
		JobBoardManager jobBoardManager = JobBoardManager.instance;
		jobBoardManager.OnTaskStarted = (Action<TaskStatusController>)Delegate.Combine(jobBoardManager.OnTaskStarted, new Action<TaskStatusController>(TaskWasStarted));
		JobBoardManager jobBoardManager2 = JobBoardManager.instance;
		jobBoardManager2.OnTaskComplete = (Action<TaskStatusController>)Delegate.Combine(jobBoardManager2.OnTaskComplete, new Action<TaskStatusController>(TaskComplete));
		JobBoardManager jobBoardManager3 = JobBoardManager.instance;
		jobBoardManager3.OnPageStarted = (Action<PageStatusController>)Delegate.Combine(jobBoardManager3.OnPageStarted, new Action<PageStatusController>(PageStarted));
		JobBoardManager jobBoardManager4 = JobBoardManager.instance;
		jobBoardManager4.OnPageShown = (Action<PageStatusController>)Delegate.Combine(jobBoardManager4.OnPageShown, new Action<PageStatusController>(PageShown));
		JobBoardManager jobBoardManager5 = JobBoardManager.instance;
		jobBoardManager5.OnPageComplete = (Action<PageStatusController>)Delegate.Combine(jobBoardManager5.OnPageComplete, new Action<PageStatusController>(PageComplete));
		JobBoardManager jobBoardManager6 = JobBoardManager.instance;
		jobBoardManager6.OnSubtaskComplete = (Action<SubtaskStatusController>)Delegate.Combine(jobBoardManager6.OnSubtaskComplete, new Action<SubtaskStatusController>(SubtaskComplete));
		JobBoardManager jobBoardManager7 = JobBoardManager.instance;
		jobBoardManager7.OnJobComplete = (Action<JobStatusController>)Delegate.Combine(jobBoardManager7.OnJobComplete, new Action<JobStatusController>(JobComplete));
		JobBoardManager jobBoardManager8 = JobBoardManager.instance;
		jobBoardManager8.OnSandboxPhaseStarted = (Action)Delegate.Combine(jobBoardManager8.OnSandboxPhaseStarted, new Action(SandboxStart));
		if (jobToUse == gamedevJobData || isInEndlessMode)
		{
			PlugInComputer();
		}
		timeSpent = 0f;
		handshakeSimulator.gameObject.SetActive(false);
	}

	private void SandboxStart()
	{
		if (!doTimedDemo && !sentInFinalMailCart)
		{
			sentInFinalMailCart = true;
			Debug.Log("office job complete, sending in final mailcart");
			StartCoroutine(SendInFinalMailCart(jobData, 1f));
		}
	}

	private void JobComplete(JobStatusController job)
	{
		if (!doTimedDemo && !sentInFinalMailCart)
		{
			sentInFinalMailCart = true;
			Debug.Log("office job complete, sending in final mailcart");
			StartCoroutine(SendInFinalMailCart(job.Data, 32f));
		}
	}

	private IEnumerator SendInFinalMailCart(JobData job, float t)
	{
		yield return new WaitForSeconds(t);
		MailCartController previousCart = currentMailCart;
		MailCartController incomingCart = GetFreeMailCart();
		if (incomingCart != null)
		{
			if (previousCart != null)
			{
				previousCart.SendCartAway();
			}
			currentMailCart = incomingCart;
			incomingCart.BringInCart(job.Tasks[0]);
		}
		else
		{
			Debug.LogError("Couldn't find a free mail cart!");
		}
	}

	private void SetupSceneFromJobStateData(JobStateData jobStateData)
	{
		if (jobStateData == null)
		{
			return;
		}
		int currentTaskIndex = JobBoardManager.instance.GetCurrentTaskIndex();
		for (int i = 0; i < jobStateData.JobLevelData.JobData.Tasks.Count; i++)
		{
			if (i < currentTaskIndex)
			{
				TaskData taskData = jobStateData.JobLevelData.JobData.Tasks[i];
				if (taskData == computerPluggedInAfterTask)
				{
					PlugInComputer();
				}
			}
		}
	}

	private void PlugInComputer()
	{
		if (!forcedComputerOn)
		{
			forcedComputerOn = true;
			StartCoroutine(InternalPlugInComputer());
		}
	}

	private IEnumerator InternalPlugInComputer()
	{
		yield return new WaitForSeconds(0.1f);
		for (int j = 0; j < plugAttachables.Length; j++)
		{
			plugAttachables[j].AttachTo(plugAttachpoints[j]);
		}
		yield return null;
		for (int i = 0; i < poweredHardwareToTurnOn.Length; i++)
		{
			poweredHardwareToTurnOn[i].PowerButtonPressed();
		}
		yield return null;
		computerController.ForceBoot();
		yield return null;
		desktopProgram.ForceLogin();
	}

	private void RefreshDefaultStockMessage(int changeStockValue)
	{
		stockValue += changeStockValue;
		string text = "SRBSN <color=green>▲ " + stockValue + "</color>";
		if (stockValue < 0)
		{
			text = "SRBSN <color=red>▼ " + Mathf.Abs(stockValue) + "</color>";
		}
		if (marqueeController != null)
		{
			if (GenieManager.AreAnyJobGenieModesActive() && GenieManager.DoesContainGenieMode(GlobalStorage.Instance.CurrentGenieModes, JobGenieCartridge.GenieModeTypes.EndlessMode))
			{
				marqueeController.SetMessages("MARKETS ARE <color=red>CLOSED</color> FOR THE DAY.  MARKETS REOPEN <color=green>TOMORROW MORNING</color> !  HAVE A <color=yellow>RESTFUL</color> NIGHT : )  DON'T WORK TOO HARD !");
				return;
			}
			marqueeController.SetMessages("SRS BUSINESS INC.", text, "LAME <color=red>▼ 643</color>", "INIT <color=green>▲ 333</color>", "HERP <color=green>▲ 202</color>", "DERP <color=red>▼ 91</color>", "INET <color=green>▲ 144</color>");
		}
	}

	private void Update()
	{
		if (throwingPlanes)
		{
			nextPlaneThrow -= Time.deltaTime;
			if (nextPlaneThrow <= 0f)
			{
				ThrowPaperPlane(UnityEngine.Random.Range(0, paperPlaneSpawners.Length));
				nextPlaneThrow = UnityEngine.Random.Range(1.5f, 3f);
			}
		}
		if (doTimedDemo)
		{
			timeSpent += Time.deltaTime;
			if (Input.GetKeyDown(KeyCode.T))
			{
				timeSpent = 253f;
			}
			if (Input.GetKeyDown(KeyCode.Escape))
			{
				DemoCompleted();
			}
			DemoEndingUpdate();
		}
	}

	private void DemoEndingUpdate()
	{
		if (timeSpent > 256f && !hasWorkCleared)
		{
			hasWorkCleared = true;
			ClearWork();
		}
		if (timeSpent > 260f && !hasWorkEnded)
		{
			hasWorkEnded = true;
			EndOfWork();
		}
		if (timeSpent > 286.5f && !hasBossCome)
		{
			hasBossCome = true;
			BossComesIn();
		}
		if (timeSpent > 300f && !hasDemoCompleted)
		{
			DemoCompleted();
		}
	}

	private void BossComesIn()
	{
		musicSource.Stop();
		throwingPlanes = false;
		if (VRPlatform.IsLowPerformancePlatform)
		{
			confettiLowEnd.Stop();
		}
		else
		{
			confettiHighEnd.Stop();
		}
		GameEventsManager.Instance.ItemActionOccurred(demoBossComesIn, "ACTIVATED");
		AudioManager.Instance.Play(paperPlaneSpawners[0].transform, bossbotIsComingVO, 1f, 1f);
	}

	private void ClearWork()
	{
		if (itemRecyclerManager != null)
		{
			itemRecyclerManager.gameObject.SetActive(false);
		}
		if (currentMailCart != null)
		{
			currentMailCart.SendCartAway();
		}
		BotVoiceController[] array = UnityEngine.Object.FindObjectsOfType<BotVoiceController>();
		for (int i = 0; i < array.Length; i++)
		{
			array[i].CancelMyCurrentVO();
		}
		WorkerBotController[] array2 = UnityEngine.Object.FindObjectsOfType<WorkerBotController>();
		for (int j = 0; j < array2.Length; j++)
		{
			array2[j].SetVOMode(false);
		}
	}

	private void EndOfWork()
	{
		JobBoardManager.instance.HackForceJobComplete();
		GameEventsManager.Instance.ItemActionOccurred(specialCaseWorldItemData, "ACTIVATED");
		AudioManager.Instance.Play(airhornPosition, airhornSound, 1f, 1f);
		musicSource.SetClip(musicSound);
		musicSource.Play();
		isPartyRunning = true;
		StartCoroutine(DoPaperPlaneFight());
		if (VRPlatform.IsLowPerformancePlatform)
		{
			confettiLowEnd.Play();
		}
		else
		{
			confettiHighEnd.Play();
		}
		Debug.Log("EndOfWork!");
	}

	private void DemoCompleted()
	{
		if (!hasDemoCompleted)
		{
			hasDemoCompleted = true;
			StartCoroutine(FadeAllAudio());
			StartCoroutine(QuitAsync());
		}
	}

	private IEnumerator QuitAsync()
	{
		ScreenFader.Instance.FadeOut(2f);
		yield return new WaitForSeconds(1.95f);
		Application.Quit();
	}

	private IEnumerator FadeAllAudio()
	{
		float currentVolume = AudioListener.volume;
		float startTime = Time.time;
		float fadeTime = 1.25f;
		while (currentVolume > 0f)
		{
			currentVolume = AudioListener.volume;
			AudioListener.volume = Mathf.Clamp(fadeTime - (Time.time - startTime), 0f, 1f);
			yield return null;
		}
	}

	private void ConferenceDemoStart()
	{
		masterController = GlobalStorage.Instance.MasterHMDAndInputController;
		psvrLeftHand = masterController.LeftHand.GetComponent<Morpheus_IndividualController>();
		psvrRightHand = masterController.RightHand.GetComponent<Morpheus_IndividualController>();
	}

	private void ConferenceDemoUpdate()
	{
		if (psvrLeftHand.GetButton(Morpheus_IndividualController.MoveControllerButton.Circle) || psvrRightHand.GetButton(Morpheus_IndividualController.MoveControllerButton.Circle))
		{
			circleTimer += Time.deltaTime;
			if (circleTimer > 3f)
			{
				ExitConferenceDemo();
			}
		}
		else if (circleTimer != 0f)
		{
			circleTimer = 0f;
		}
	}

	private void ExitConferenceDemo()
	{
		if (!isExitingConferenceDemo)
		{
			isExitingConferenceDemo = true;
			LevelLoader.Instance.LoadSceneManual("Loading_PSVR", 2.25f, 1.5f);
		}
	}

	private IEnumerator DoParty()
	{
		GameEventsManager.Instance.ItemActionOccurred(specialCaseWorldItemData, "ACTIVATED");
		if (marqueeController != null)
		{
			marqueeController.SetMessages("5PM", "SEE YOU TOMORROW...");
		}
		AudioManager.Instance.Play(airhornPosition, airhornSound, 1f, 1f);
		yield return null;
		musicSource.SetClip(musicSound);
		musicSource.Play();
		throwingPlanes = true;
		if (VRPlatform.IsLowPerformancePlatform)
		{
			confettiLowEnd.Play();
		}
		else
		{
			confettiHighEnd.Play();
		}
		yield return new WaitForSeconds(20f);
		throwingPlanes = false;
		musicSource.Stop();
		isPartyRunning = false;
		if (VRPlatform.IsLowPerformancePlatform)
		{
			confettiLowEnd.Stop();
		}
		else
		{
			confettiHighEnd.Stop();
		}
		if (marqueeController != null)
		{
			marqueeController.SetMessages(string.Empty);
		}
	}

	private IEnumerator DoPaperPlaneFight()
	{
		yield return new WaitForSeconds(4.5f);
		AudioManager.Instance.Play(paperPlaneSpawners[0].transform, paperPlaneFightStartVO, 1f, 1f);
		yield return new WaitForSeconds(paperPlaneFightStartVO.length);
		if (VRPlatform.IsLowPerformancePlatform)
		{
			ThrowPaperPlane(UnityEngine.Random.Range(0, paperPlaneSpawners.Length));
		}
		else
		{
			for (int j = 0; j < paperPlaneSpawners.Length; j++)
			{
				ThrowPaperPlane(j);
			}
			nextPlaneThrow = 3f;
		}
		for (int i = 0; i < paperPlaneFightVO.Length; i++)
		{
			yield return new WaitForSeconds(UnityEngine.Random.Range(4.5f, 6f));
			int k = UnityEngine.Random.Range(0, paperPlaneSpawners.Length);
			ThrowPaperPlane(k);
			AudioManager.Instance.Play(paperPlaneSpawners[k].transform, paperPlaneFightVOSequence.GetNext(), 1f, 1f);
		}
	}

	private void ThrowPaperPlane(int index)
	{
		GameObjectPrefabSpawner gameObjectPrefabSpawner = paperPlaneSpawners[index];
		GameObject gameObject = gameObjectPrefabSpawner.SpawnPrefab();
		Rigidbody component = gameObject.GetComponent<Rigidbody>();
		Vector3 vector = Quaternion.AngleAxis(UnityEngine.Random.Range(-1f, 1f) * paperPlaneThrowHeadingNoise, Vector3.up) * (paperPlaneTarget.position - gameObject.transform.position).normalized;
		float num = paperPlaneThrowSpeed + UnityEngine.Random.Range(-1f, 1f) * paperPlaneThrowSpeedNoise;
		component.velocity = vector * num;
	}

	private void TaskWasStarted(TaskStatusController taskStatus)
	{
		if (isPartyRunning || hasWorkCleared || sentInFinalMailCart)
		{
			return;
		}
		if (currentMailCart != null && currentMailCart.NeedsToTakeTaskAwayOnTaskStart(taskStatus.Data))
		{
			Debug.Log(currentMailCart.CurrentTask.name + " requires clearing on task " + taskStatus.Data.name);
			currentMailCart.SendCartAway();
		}
		MailCartController mailCartController = currentMailCart;
		MailCartController freeMailCart = GetFreeMailCart();
		if (freeMailCart != null)
		{
			if (freeMailCart.HasLoadForTask(taskStatus.Data))
			{
				if (mailCartController != null)
				{
					mailCartController.SendCartAway();
				}
				currentMailCart = freeMailCart;
				freeMailCart.BringInCart(taskStatus.Data);
			}
		}
		else
		{
			Debug.LogError("Couldn't find a free mail cart!");
		}
	}

	private void TaskComplete(TaskStatusController taskStatus)
	{
		if (!isPartyRunning && !hasWorkCleared)
		{
			if (taskStatus.Data == partyTrigger)
			{
				StopAllCoroutines();
				StartCoroutine(DoParty());
			}
			else if (currentMailCart != null && taskStatus.Data == currentMailCart.CurrentTask && currentMailCart.NeedsToTakeTaskAway(taskStatus.Data))
			{
				currentMailCart.SendCartAway();
			}
			if (isInEndlessMode && handshakeSimulator.gameObject.activeSelf && !handshakeSimulator.IsCurrentlyExiting)
			{
				handshakeSimulator.Exit();
			}
		}
	}

	private void PageStarted(PageStatusController pageStatus)
	{
		if (pageStatus.Data == projectorPage)
		{
			RenderSettings.ambientIntensity = normalAmbientIntensity / 2f;
			RenderSettings.ambientLight = normalAmbientLight / 2f;
			mainLight.intensity = normalLightIntensity / 2f;
			projectorParent.localPosition = Vector3.up * 5f;
			Go.to(projectorParent, 5f, new GoTweenConfig().localPosition(Vector3.zero).setEaseType(GoEaseType.QuadOut));
			AudioManager.Instance.Play(ProjectionScreenLocation.transform, projectorMoveSound, 1f, 1f);
			if (ceilingLights.Length == 0)
			{
				Debug.Log("Manager needs a reference to the ceiling lights");
				return;
			}
			for (int i = 0; i < ceilingLights.Length; i++)
			{
				ceilingLights[i].material = ceilingLightOffMaterial;
			}
		}
		if (!isInEndlessMode)
		{
			return;
		}
		for (int j = 0; j < pagesThatRequireHandshakeSimulator.Length; j++)
		{
			if (pageStatus.Data == pagesThatRequireHandshakeSimulator[j])
			{
				handshakeSimulator.Enter(handShakeSimulatorEnterDelay);
			}
		}
	}

	private void PageShown(PageStatusController pageStatus)
	{
		if (pageStatus.Data == pageForInternetPicsAlert)
		{
			computerController.SetNextAlert(internetPicsSprite, "Coworker 543971 sent you a link!", "GO", delegate
			{
				computerController.StartProgram(ComputerProgramID.FunnyPicViewer);
			});
		}
		else if (pageStatus.Data == pageToStartLookBusyTimer)
		{
			StartCoroutine(LookBusyTimer());
		}
		currentlyShownPage = pageStatus.Data;
	}

	private void PageComplete(PageStatusController pageStatus)
	{
		if (pageStatus.Data == projectorPage)
		{
			RenderSettings.ambientIntensity = normalAmbientIntensity;
			RenderSettings.ambientLight = normalAmbientLight;
			mainLight.intensity = normalLightIntensity;
			Go.to(projectorParent, 5f, new GoTweenConfig().localPosition(Vector3.up * 5f).setEaseType(GoEaseType.QuadIn));
			TimeManager.Invoke(HideProjector, 5.1f);
			AudioManager.Instance.Play(ProjectionScreenLocation.transform, projectorMoveSound, 1f, 1f);
			if (ceilingLights.Length == 0)
			{
				Debug.Log("Manager needs a reference to the ceiling lights");
				return;
			}
			for (int i = 0; i < ceilingLights.Length; i++)
			{
				ceilingLights[i].material = ceilingLightOnMaterial;
			}
		}
		if (pageStatus.Data == currentlyShownPage && currentMailCart != null && currentMailCart.NeedsToTakeTaskAwayAfterPage(pageStatus.Data))
		{
			Debug.Log(currentMailCart.CurrentTask.name + " requires clearing after page " + pageStatus.Data.name);
			currentMailCart.SendCartAway();
		}
		if (!isInEndlessMode)
		{
			return;
		}
		for (int j = 0; j < pagesThatRequireHandshakeSimulator.Length; j++)
		{
			if (pageStatus.Data == pagesThatRequireHandshakeSimulator[j])
			{
				handshakeSimulator.Exit(handShakeSimulatorExitDelay);
			}
		}
	}

	public void ShowProjector()
	{
		projectorHideObject.gameObject.SetActive(true);
	}

	private void HideProjector()
	{
		projectorHideObject.gameObject.SetActive(false);
	}

	private IEnumerator LookBusyTimer()
	{
		yield return new WaitForSeconds(25f);
		GameEventsManager.Instance.ItemActionOccurred(lookBusyTimerWorldItem, "ACTIVATED");
	}

	private void SubtaskComplete(SubtaskStatusController subtaskStatus)
	{
		if (subtaskStatus.Data == jamVendingMachineAfterSubtask)
		{
			VendingMachineController vendingMachineController = UnityEngine.Object.FindObjectOfType<VendingMachineController>();
			if (vendingMachineController != null)
			{
				vendingMachineController.SetNextItemWillGetStuck(true);
			}
			return;
		}
		if (subtaskStatus.Data == confettiTrigger)
		{
			StartCoroutine(ShortConfetti());
			return;
		}
		for (int i = 0; i < subtasksThatHelpStockValue.Length; i++)
		{
			if (subtasksThatHelpStockValue[i] == subtaskStatus.Data)
			{
				RefreshDefaultStockMessage(UnityEngine.Random.Range(50, 100));
			}
		}
	}

	private IEnumerator ShortConfetti()
	{
		if (VRPlatform.IsLowPerformancePlatform)
		{
			confettiLowEnd.Play();
		}
		else
		{
			confettiHighEnd.Play();
		}
		yield return new WaitForSeconds(1f);
		if (VRPlatform.IsLowPerformancePlatform)
		{
			confettiLowEnd.Stop();
		}
		else
		{
			confettiHighEnd.Stop();
		}
	}

	private MailCartController GetFreeMailCart()
	{
		for (int i = 0; i < mailCarts.Length; i++)
		{
			if (mailCarts[i] != currentMailCart)
			{
				return mailCarts[i];
			}
		}
		return null;
	}

	private void ToggleFog()
	{
		RenderSettings.fog = true;
		RenderSettings.fogColor = new Color32(0, 4, 16, byte.MaxValue);
		RenderSettings.fogMode = FogMode.Exponential;
		RenderSettings.fogDensity = 0.1f;
		RenderSettings.ambientLight = new Color(0.462f, 0.439f, 0.435f);
	}
}
