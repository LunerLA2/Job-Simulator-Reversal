using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using OwlchemyVR;
using UnityEngine;

public class ComputerController : MonoBehaviour
{
	private const float desiredScreenFPS = 45f;

	private const float BOOT_DURATION = 3f;

	private static bool USE_LOW_FPS_RENDER = true;

	private float lastRenderTime;

	[SerializeField]
	private ComputerTowerController tower;

	[SerializeField]
	private PoweredComputerHardwareController[] monitors;

	[SerializeField]
	private ComputerCursorController cursor;

	[SerializeField]
	private PrinterController printer;

	[SerializeField]
	private Camera canvasCamera;

	[SerializeField]
	private VisibilityEvents cameraVisibilityEvents;

	private bool isVisible;

	[SerializeField]
	private Canvas canvas;

	[SerializeField]
	private GameObject blind;

	[SerializeField]
	private GameObject screensaver;

	[SerializeField]
	private GameObject bootScreen;

	[SerializeField]
	private GameObject noSignalScreen;

	[SerializeField]
	private Transform programsRoot;

	[SerializeField]
	private float screensaverIdleTime = 15f;

	[SerializeField]
	private AudioSourceHelper idleSoundSource;

	[SerializeField]
	private AudioSourceHelper programSoundSource;

	[SerializeField]
	private bool isDummy;

	private ComputerState state;

	private bool monitorIsOn;

	private bool towerIsOn;

	private float bootTime;

	private Dictionary<ComputerProgramID, ComputerProgram> programs;

	private ComputerProgram[] activePrograms;

	private bool isActiveProgramFromCD;

	private float timeUntilScreensaver;

	private bool isShowingScreensaver;

	private ComputerClickable highlightedClickable;

	private ComputerClickable newHighlightedClickable;

	private ComputerClickable clickedClickable;

	private ComputerAlertProgram alertProgram;

	private ComputerProgram desktopProgram;

	private Vector2 cursorPos;

	private Coroutine screenshotCoroutine;

	private ComputerProgram programToRestore;

	private float guaranteedRenderTime;

	public ComputerState State
	{
		get
		{
			return state;
		}
	}

	public Camera CanvasCamera
	{
		get
		{
			return canvasCamera;
		}
	}

	public Canvas Canvas
	{
		get
		{
			return canvas;
		}
	}

	public ComputerProgram DesktopProgram
	{
		get
		{
			return desktopProgram;
		}
	}

	public ComputerAlertProgram AlertProgram
	{
		get
		{
			return alertProgram;
		}
	}

	public ComputerProgram[] Programs
	{
		get
		{
			return programs.Values.ToArray();
		}
	}

	public ComputerClickable HighlightedClickable
	{
		get
		{
			return highlightedClickable;
		}
	}

	public Vector2 CursorPosition
	{
		get
		{
			return cursorPos;
		}
	}

	public bool IsMouseButtonDown
	{
		get
		{
			return cursor.IsMouseButtonDown;
		}
	}

	public bool IsCursorVisible
	{
		get
		{
			return cursor.IsVisible;
		}
	}

	public bool IsPrinterBusy
	{
		get
		{
			return printer.IsBusy;
		}
	}

	private void Awake()
	{
		if (USE_LOW_FPS_RENDER)
		{
			canvasCamera.enabled = false;
		}
		programs = new Dictionary<ComputerProgramID, ComputerProgram>();
		for (int i = 0; i < programsRoot.childCount; i++)
		{
			ComputerProgram component = programsRoot.GetChild(i).GetComponent<ComputerProgram>();
			if (component != null)
			{
				component.SetHostComputer(this);
				programs.Add(component.ProgramID, component);
			}
		}
		desktopProgram = GetProgram(ComputerProgramID.Desktop);
		alertProgram = GetProgram(ComputerProgramID.Alert) as ComputerAlertProgram;
		activePrograms = new ComputerProgram[Enum.GetValues(typeof(ComputerProgramPriority)).Length];
		blind.SetActive(true);
		if (isDummy)
		{
			TowerTurnedOn();
			MonitorTurnedOn();
		}
	}

	private void OnEnable()
	{
		if (tower != null)
		{
			ComputerTowerController computerTowerController = tower;
			computerTowerController.OnWasTurnedOn = (Action)Delegate.Combine(computerTowerController.OnWasTurnedOn, new Action(TowerTurnedOn));
			ComputerTowerController computerTowerController2 = tower;
			computerTowerController2.OnWasTurnedOff = (Action)Delegate.Combine(computerTowerController2.OnWasTurnedOff, new Action(TowerTurnedOff));
			ComputerTowerController computerTowerController3 = tower;
			computerTowerController3.OnCDWasInserted = (Action<ComputerCD>)Delegate.Combine(computerTowerController3.OnCDWasInserted, new Action<ComputerCD>(CDInserted));
			ComputerTowerController computerTowerController4 = tower;
			computerTowerController4.OnCDWasEjected = (Action<ComputerCD>)Delegate.Combine(computerTowerController4.OnCDWasEjected, new Action<ComputerCD>(CDEjected));
		}
		if (monitors != null)
		{
			for (int i = 0; i < monitors.Length; i++)
			{
				PoweredComputerHardwareController obj = monitors[i];
				obj.OnWasTurnedOn = (Action)Delegate.Combine(obj.OnWasTurnedOn, new Action(MonitorTurnedOn));
				PoweredComputerHardwareController obj2 = monitors[i];
				obj2.OnWasTurnedOff = (Action)Delegate.Combine(obj2.OnWasTurnedOff, new Action(MonitorTurnedOff));
			}
		}
		ComputerCursorController computerCursorController = cursor;
		computerCursorController.OnClicked = (Action)Delegate.Combine(computerCursorController.OnClicked, new Action(CursorClicked));
		ComputerCursorController computerCursorController2 = cursor;
		computerCursorController2.OnClickUp = (Action)Delegate.Combine(computerCursorController2.OnClickUp, new Action(CursorClickUp));
		ComputerCursorController computerCursorController3 = cursor;
		computerCursorController3.OnMoved = (Action)Delegate.Combine(computerCursorController3.OnMoved, new Action(CursorMoved));
		cursorPos = RectTransformUtility.WorldToScreenPoint(canvasCamera, cursor.transform.position);
		if (cameraVisibilityEvents != null)
		{
			cameraVisibilityEvents.OnObjectBecameVisible += BecameVisible;
			cameraVisibilityEvents.OnObjectBecameInvisible += BecameInvisible;
		}
		else
		{
			Debug.LogError("ComputerController does not have a VisibilityEvents assigned, it will always be rendering!", base.gameObject);
		}
	}

	private void OnDisable()
	{
		if (tower != null)
		{
			ComputerTowerController computerTowerController = tower;
			computerTowerController.OnWasTurnedOn = (Action)Delegate.Remove(computerTowerController.OnWasTurnedOn, new Action(TowerTurnedOn));
			ComputerTowerController computerTowerController2 = tower;
			computerTowerController2.OnWasTurnedOff = (Action)Delegate.Remove(computerTowerController2.OnWasTurnedOff, new Action(TowerTurnedOff));
			ComputerTowerController computerTowerController3 = tower;
			computerTowerController3.OnCDWasInserted = (Action<ComputerCD>)Delegate.Remove(computerTowerController3.OnCDWasInserted, new Action<ComputerCD>(CDInserted));
			ComputerTowerController computerTowerController4 = tower;
			computerTowerController4.OnCDWasEjected = (Action<ComputerCD>)Delegate.Remove(computerTowerController4.OnCDWasEjected, new Action<ComputerCD>(CDEjected));
		}
		if (monitors != null)
		{
			for (int i = 0; i < monitors.Length; i++)
			{
				PoweredComputerHardwareController obj = monitors[i];
				obj.OnWasTurnedOn = (Action)Delegate.Remove(obj.OnWasTurnedOn, new Action(MonitorTurnedOn));
				PoweredComputerHardwareController obj2 = monitors[i];
				obj2.OnWasTurnedOff = (Action)Delegate.Remove(obj2.OnWasTurnedOff, new Action(MonitorTurnedOff));
			}
		}
		ComputerCursorController computerCursorController = cursor;
		computerCursorController.OnClicked = (Action)Delegate.Remove(computerCursorController.OnClicked, new Action(CursorClicked));
		ComputerCursorController computerCursorController2 = cursor;
		computerCursorController2.OnClickUp = (Action)Delegate.Remove(computerCursorController2.OnClickUp, new Action(CursorClickUp));
		ComputerCursorController computerCursorController3 = cursor;
		computerCursorController3.OnMoved = (Action)Delegate.Remove(computerCursorController3.OnMoved, new Action(CursorMoved));
		if (cameraVisibilityEvents != null)
		{
			cameraVisibilityEvents.OnObjectBecameVisible -= BecameVisible;
			cameraVisibilityEvents.OnObjectBecameInvisible -= BecameInvisible;
		}
	}

	public void EnsureRenderingForTime(float t)
	{
		if (t > guaranteedRenderTime)
		{
			guaranteedRenderTime = t;
		}
	}

	private void BecameVisible(VisibilityEvents e)
	{
		isVisible = true;
	}

	private void BecameInvisible(VisibilityEvents e)
	{
		isVisible = false;
	}

	public void ShowCursor()
	{
		cursor.Show();
	}

	public void HideCursor()
	{
		cursor.Hide();
	}

	public void SetHighlightedClickableCandidate(ComputerClickable clickable)
	{
		if (newHighlightedClickable == null && IsCursorVisible)
		{
			newHighlightedClickable = clickable;
		}
	}

	public void PlaySound(AudioClip clip, float volume = 1f, float pitch = 1f, bool loop = false)
	{
		if (!monitorIsOn || !(clip != null))
		{
			return;
		}
		if (programSoundSource != null)
		{
			if (programSoundSource.IsPlaying)
			{
				programSoundSource.Stop();
			}
			programSoundSource.SetClip(clip);
			programSoundSource.SetVolume(volume);
			programSoundSource.SetPitch(pitch);
			programSoundSource.SetLooping(loop);
			programSoundSource.Play();
		}
		else if (monitors != null)
		{
			if (monitors.Length >= 1)
			{
				AudioManager.Instance.Play(monitors[0].transform.position, clip, volume, pitch);
			}
		}
		else
		{
			AudioManager.Instance.Play(base.transform.position, clip, volume, pitch);
		}
	}

	public void SetSoundVolume(float volume)
	{
		if (programSoundSource != null && programSoundSource.IsPlaying)
		{
			programSoundSource.SetVolume(volume);
		}
	}

	public void SetSoundPitch(float pitch)
	{
		if (programSoundSource != null && programSoundSource.IsPlaying)
		{
			programSoundSource.SetPitch(pitch);
		}
	}

	public void StopSound(AudioClip clip = null)
	{
		if (programSoundSource != null && programSoundSource.IsPlaying && (clip == null || clip == programSoundSource.GetClip()))
		{
			programSoundSource.Stop();
		}
	}

	public void SetNextAlert(Sprite graphic, string message, string buttonCaption, Action buttonAction)
	{
		if (alertProgram != null)
		{
			alertProgram.SetNextAlert(graphic, message, buttonCaption, buttonAction);
		}
		else
		{
			Debug.LogError("Alert controller missing on computer.");
		}
	}

	public Texture2D TakeScreenshot()
	{
		return TakeScreenshot(canvas.pixelRect);
	}

	public Texture2D TakeScreenshot(RectTransform captureTransform)
	{
		Rect rect = captureTransform.rect;
		Vector2 vector = canvas.transform.InverseTransformPoint(captureTransform.position);
		vector.x += canvasCamera.pixelWidth / 2;
		vector.y = 0f - vector.y + (float)(canvasCamera.pixelHeight / 2);
		rect.position += vector;
		return TakeScreenshot(rect);
	}

	public Texture2D TakeScreenshot(Rect captureRect)
	{
		Texture2D texture2D = new Texture2D((int)captureRect.width, (int)captureRect.height, TextureFormat.ARGB32, false);
		if (screenshotCoroutine != null)
		{
			StopCoroutine(screenshotCoroutine);
		}
		screenshotCoroutine = StartCoroutine(TakeScreenshotAsync(texture2D, captureRect));
		return texture2D;
	}

	private IEnumerator TakeScreenshotAsync(Texture2D screenshot, Rect captureRect)
	{
		bool wasCursorVisible = IsCursorVisible;
		HideCursor();
		yield return new WaitForEndOfFrame();
		RenderTexture cachedRT = RenderTexture.active;
		RenderTexture.active = canvasCamera.targetTexture;
		canvasCamera.Render();
		screenshot.ReadPixels(captureRect, 0, 0);
		screenshot.Apply();
		if (wasCursorVisible)
		{
			ShowCursor();
		}
		RenderTexture.active = cachedRT;
		screenshotCoroutine = null;
	}

	public void PrintObject(GameObject obj, WorldItemData widOverride = null, Action<GameObject> customizer = null)
	{
		printer.PrintObject(obj, widOverride, customizer);
	}

	public void ForceBoot()
	{
		bootTime = 3f;
	}

	private void LateUpdate()
	{
		if (!USE_LOW_FPS_RENDER)
		{
			return;
		}
		if (guaranteedRenderTime > 0f)
		{
			guaranteedRenderTime -= Time.deltaTime;
		}
		if ((monitorIsOn || guaranteedRenderTime > 0f) && (cameraVisibilityEvents == null || (cameraVisibilityEvents != null && isVisible)))
		{
			float num = lastRenderTime + ((!monitorIsOn) ? (1f / 90f) : (1f / 45f));
			if (Time.time >= num)
			{
				canvasCamera.Render();
				lastRenderTime = Time.time;
			}
		}
	}

	private void Update()
	{
		if (towerIsOn)
		{
			if (state == ComputerState.Booting)
			{
				if (bootTime == 0f && !bootScreen.activeSelf)
				{
					bootScreen.SetActive(true);
				}
				float num = bootTime + Time.deltaTime;
				if (isDummy || num >= 3f)
				{
					bootTime = 0f;
					if (bootScreen.activeSelf)
					{
						bootScreen.SetActive(false);
					}
					state = ComputerState.On;
					StartProgram(desktopProgram, ComputerProgramPriority.Desktop);
					if (programToRestore != null)
					{
						StartProgram(programToRestore);
					}
				}
				else
				{
					bootTime = num;
				}
			}
			else
			{
				UpdateProgramsCursor();
				UpdateScreensaver();
				UpdateAlerts();
			}
		}
		else if (monitorIsOn && !noSignalScreen.activeSelf)
		{
			noSignalScreen.SetActive(true);
		}
	}

	private void ForceScreenRender()
	{
		if (USE_LOW_FPS_RENDER)
		{
			canvasCamera.Render();
			lastRenderTime = Time.time;
		}
	}

	private void UpdateProgramsCursor()
	{
		newHighlightedClickable = null;
		for (int i = 0; i < activePrograms.Length; i++)
		{
			if (activePrograms[i] != null)
			{
				activePrograms[i].InjectCursorPosition(cursorPos);
				break;
			}
		}
		if (highlightedClickable != newHighlightedClickable && (highlightedClickable == null || !highlightedClickable.IsBusy))
		{
			if (highlightedClickable != null)
			{
				highlightedClickable.Unhighlight();
			}
			highlightedClickable = newHighlightedClickable;
			if (newHighlightedClickable != null)
			{
				newHighlightedClickable.Highlight();
				cursor.SetCursorType(ComputerCursorType.Hand);
			}
			else
			{
				cursor.SetCursorType(ComputerCursorType.Arrow);
			}
		}
	}

	private void UpdateScreensaver()
	{
		if (isShowingScreensaver)
		{
			return;
		}
		bool flag = false;
		for (int i = 0; i < activePrograms.Length; i++)
		{
			if (activePrograms[i] != null && activePrograms[i].BlockScreensaver)
			{
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			timeUntilScreensaver -= Time.deltaTime;
			if (timeUntilScreensaver <= 0f)
			{
				timeUntilScreensaver = 0f;
				StartScreensaver();
			}
		}
		else
		{
			timeUntilScreensaver = screensaverIdleTime;
		}
	}

	private void UpdateAlerts()
	{
		if (!(alertProgram != null) || !alertProgram.HasAlertWaiting || alertProgram.IsShowingAlert)
		{
			return;
		}
		bool flag = false;
		for (int i = 0; i < activePrograms.Length; i++)
		{
			if (activePrograms[i] != null && activePrograms[i].BlockAlerts)
			{
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			StartProgram(ComputerProgramID.Alert, ComputerProgramPriority.Alert);
		}
	}

	private void TowerTurnedOn()
	{
		towerIsOn = true;
		state = ComputerState.Booting;
		if (idleSoundSource != null && !idleSoundSource.IsPlaying)
		{
			idleSoundSource.Play();
		}
		bootTime = 0f;
		noSignalScreen.SetActive(false);
		if (monitorIsOn)
		{
			MonitorTurnedOn();
		}
		timeUntilScreensaver = screensaverIdleTime;
		ForceScreenRender();
	}

	private void TowerTurnedOff()
	{
		towerIsOn = false;
		bootTime = 0f;
		for (int i = 0; i < activePrograms.Length; i++)
		{
		}
		StopScreensaver();
		StopAllPrograms(true);
		state = ComputerState.Off;
		if (idleSoundSource != null)
		{
			idleSoundSource.Stop();
		}
		blind.SetActive(true);
		ForceScreenRender();
	}

	private void MonitorTurnedOn()
	{
		monitorIsOn = true;
		if (towerIsOn)
		{
			blind.SetActive(false);
		}
		ForceScreenRender();
	}

	private void MonitorTurnedOff()
	{
		monitorIsOn = false;
		blind.SetActive(true);
		noSignalScreen.SetActive(false);
		StopScreensaver();
		ForceScreenRender();
	}

	public void KeyboardAnyKeyPressed(string code)
	{
		if (towerIsOn && state != ComputerState.Booting && !StopScreensaver())
		{
			for (int i = 0; i < activePrograms.Length && (!(activePrograms[i] != null) || !activePrograms[i].InjectKeyPress(code)); i++)
			{
			}
		}
	}

	private void CursorClicked()
	{
		if (towerIsOn && state != ComputerState.Booting && !StopScreensaver())
		{
			for (int i = 0; i < activePrograms.Length && (!(activePrograms[i] != null) || !activePrograms[i].InjectMouseClick(cursorPos)); i++)
			{
			}
			if (highlightedClickable != null)
			{
				highlightedClickable.Click();
				clickedClickable = highlightedClickable;
			}
		}
	}

	private void CursorClickUp()
	{
		if (towerIsOn && state != ComputerState.Booting && !StopScreensaver())
		{
			for (int i = 0; i < activePrograms.Length && (!(activePrograms[i] != null) || !activePrograms[i].InjectMouseClickUp(cursorPos)); i++)
			{
			}
			if (clickedClickable != null)
			{
				clickedClickable.ClickUp();
				clickedClickable = null;
			}
		}
	}

	private void CursorMoved()
	{
		if (!towerIsOn || state == ComputerState.Booting)
		{
			return;
		}
		cursorPos = RectTransformUtility.WorldToScreenPoint(canvasCamera, cursor.transform.position);
		if (!StopScreensaver())
		{
			for (int i = 0; i < activePrograms.Length && (!(activePrograms[i] != null) || !activePrograms[i].InjectMouseMove(cursorPos)); i++)
			{
			}
		}
	}

	private void CDInserted(ComputerCD cd)
	{
		if ((alertProgram != null && alertProgram.IsShowingAlert) || programToRestore != null)
		{
			tower.OpenTray();
		}
		else
		{
			StartProgram(cd.ProgramID, ComputerProgramPriority.Programs, true);
		}
	}

	private void CDEjected(ComputerCD cd)
	{
		ComputerProgram activeProgram = GetActiveProgram();
		if (activeProgram != null && activeProgram.ProgramID == cd.ProgramID)
		{
			StopProgram(activeProgram);
		}
	}

	public ComputerProgram GetProgram(ComputerProgramID programID)
	{
		ComputerProgram value = null;
		programs.TryGetValue(programID, out value);
		return value;
	}

	public ComputerProgram GetActiveProgram(ComputerProgramPriority priority = ComputerProgramPriority.Programs)
	{
		return activePrograms[(int)priority];
	}

	public void StartProgram(ComputerProgramID programID, ComputerProgramPriority priority = ComputerProgramPriority.Programs, bool isOnCD = false)
	{
		ComputerProgram value = null;
		if (programs.TryGetValue(programID, out value))
		{
			StopScreensaver();
			StartProgram(value, priority, isOnCD);
		}
	}

	public void StartProgram(ComputerProgram program, ComputerProgramPriority priority = ComputerProgramPriority.Programs, bool isOnCD = false)
	{
		ComputerProgram activeProgram = GetActiveProgram(priority);
		if (activeProgram != null)
		{
			StopProgram(activeProgram);
		}
		activePrograms[(int)priority] = program;
		program.SetPriority(priority);
		program.OnFinish += ProgramFinished;
		if (program.RestoreIfInterrupted)
		{
			programToRestore = program;
		}
		if (priority == ComputerProgramPriority.Programs)
		{
			isActiveProgramFromCD = isOnCD;
		}
		program.gameObject.SetActive(true);
	}

	public void StopProgram(ComputerProgramPriority priority = ComputerProgramPriority.Programs, bool eject = false)
	{
		ComputerProgram activeProgram = GetActiveProgram(priority);
		StopProgram(activeProgram);
	}

	public void StopProgram(ComputerProgram program, bool eject = false)
	{
		if (!(program == null))
		{
			program.OnFinish -= ProgramFinished;
			program.gameObject.SetActive(false);
			activePrograms[(int)program.Priority] = null;
			if (eject && isActiveProgramFromCD && tower.IsCDInserted)
			{
				tower.OpenTray();
			}
		}
	}

	public void StopAllPrograms(bool eject = false)
	{
		for (int i = 0; i < activePrograms.Length; i++)
		{
			StopProgram(activePrograms[i], eject);
		}
	}

	private void ProgramFinished(ComputerProgram program)
	{
		StopProgram(program, true);
		if (programToRestore == program)
		{
			programToRestore = null;
		}
	}

	public void StartScreensaver()
	{
		isShowingScreensaver = true;
		screensaver.SetActive(true);
	}

	public bool StopScreensaver()
	{
		if (isShowingScreensaver)
		{
			isShowingScreensaver = false;
			screensaver.SetActive(false);
			timeUntilScreensaver = screensaverIdleTime;
			return true;
		}
		timeUntilScreensaver = screensaverIdleTime;
		return false;
	}
}
