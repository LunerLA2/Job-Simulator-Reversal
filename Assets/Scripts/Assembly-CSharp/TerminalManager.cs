using System;
using System.Collections;
using OwlchemyVR;
using OwlchemyVR2;
using UnityEngine;
using UnityEngine.Events;

public class TerminalManager : MonoBehaviour
{
	private const string linesOn = "TerminalLineActivated";

	private const string linesOff = "TerminalLineDeactivated";

	private const string checksum = "234121432";

	[SerializeField]
	private AttachablePoint cartridgeSocket;

	private JobCartridgeWithGenieFlags currentlyAttachedJobWithGenieFlags;

	[SerializeField]
	private HologramController hologram;

	[SerializeField]
	private TerminalUIController terminalUI;

	[SerializeField]
	private WorldItemData menuButton;

	[SerializeField]
	private WorldItem terminalItem;

	private bool readyToRunRoutine;

	private bool routineRan;

	private bool isLoadingIntoJob;

	[SerializeField]
	private Animator linesAnimator;

	[SerializeField]
	private Transform[] toggleOffOnAttach;

	[SerializeField]
	private GameObject cartridgeLights;

	[SerializeField]
	private GameObject jobTransitionEffects;

	[SerializeField]
	private Transform audioPosition;

	[SerializeField]
	private AudioClip errorSound;

	[SerializeField]
	private AudioClip successSound;

	[SerializeField]
	private AudioClip loadJobSound;

	[SerializeField]
	private float jobLoadDelayTime;

	[SerializeField]
	private GameObject recalibrateButtonPedestal;

	private string levelToLoad;

	private int taskToLoad;

	private string currentChk = string.Empty;

	private float delayForJobBotTutorial = 5f;

	private bool didFakeLevelTransition;

	[SerializeField]
	private OwlchemyVR2.GrabbableHinge overtimeSwitch;

	private bool loadOvertimeMode;

	public bool IsLoadingIntoJob
	{
		get
		{
			return isLoadingIntoJob;
		}
	}

	public bool LoadOvertimeMode
	{
		get
		{
			return loadOvertimeMode;
		}
	}

	private void OnEnable()
	{
		cartridgeSocket.OnObjectWasAttached += CartridgeAttached;
		cartridgeSocket.OnObjectWasDetached += CartridgeDetached;
		overtimeSwitch.OnHingeReset += OvertimeSwitchReset;
		overtimeSwitch.OnHingeActivated += OvertimeSwitchActivated;
	}

	private void OnDisable()
	{
		cartridgeSocket.OnObjectWasAttached -= CartridgeAttached;
		cartridgeSocket.OnObjectWasDetached -= CartridgeDetached;
		overtimeSwitch.OnHingeReset -= OvertimeSwitchReset;
		overtimeSwitch.OnHingeActivated -= OvertimeSwitchActivated;
	}

	public void EnterJobButton()
	{
		if (isLoadingIntoJob)
		{
			return;
		}
		if (currentlyAttachedJobWithGenieFlags == null)
		{
			AudioManager.Instance.Play(audioPosition, errorSound, 1f, 1f);
			return;
		}
		if (currentlyAttachedJobWithGenieFlags != null && currentlyAttachedJobWithGenieFlags.BaseJobCartridge == null)
		{
			ReCheckCartridge(null, cartridgeSocket.GetAttachedObject(0).GetComponent<AttachableObject>());
		}
		if (loadOvertimeMode && currentlyAttachedJobWithGenieFlags.BaseJobCartridge.StateData.GetPercentageComplete() != 1f)
		{
			AudioManager.Instance.Play(audioPosition, errorSound, 1f, 1f);
		}
		else if (currentlyAttachedJobWithGenieFlags != null && currentlyAttachedJobWithGenieFlags.BaseJobCartridge != null)
		{
			PickupableItem[] componentsInChildren = cartridgeSocket.GetAttachedObject(0).GetComponentsInChildren<PickupableItem>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].enabled = false;
			}
			levelToLoad = currentlyAttachedJobWithGenieFlags.BaseJobCartridge.StateData.JobLevelData.SceneName;
			taskToLoad = terminalUI.GetDesiredTaskToLoad();
			if (!GlobalStorage.Instance.GameStateData.HasSavedData())
			{
				if (!didFakeLevelTransition)
				{
					AudioManager.Instance.Play(audioPosition, successSound, 1f, 1f);
					didFakeLevelTransition = true;
					ScreenFader instance = ScreenFader.Instance;
					instance.OnFadeOutComplete = (UnityAction)Delegate.Combine(instance.OnFadeOutComplete, new UnityAction(FakeLevelTransitionComplete));
					ScreenFader.Instance.FadeOut(1.5f);
				}
				else
				{
					AudioManager.Instance.Play(audioPosition, errorSound, 1f, 1f);
				}
			}
			else
			{
				AudioManager.Instance.Play(audioPosition, successSound, 1f, 1f);
				LoadIntoJob();
			}
		}
		else
		{
			AudioManager.Instance.Play(audioPosition, errorSound, 1f, 1f);
		}
	}

	private void FakeLevelTransitionComplete()
	{
		ScreenFader instance = ScreenFader.Instance;
		instance.OnFadeOutComplete = (UnityAction)Delegate.Remove(instance.OnFadeOutComplete, new UnityAction(FakeLevelTransitionComplete));
		HideRecalibrateButton();
		ScreenFader.Instance.FadeIn(1f);
		GameEventsManager.Instance.ItemActionOccurred(GetComponent<WorldItem>().Data, "ACTIVATED");
		Invoke("WaitForJobBotToFinishMenuTutorial", delayForJobBotTutorial);
	}

	private void HideRecalibrateButton()
	{
		if (recalibrateButtonPedestal != null)
		{
			recalibrateButtonPedestal.SetActive(false);
		}
	}

	private void WaitForJobBotToFinishMenuTutorial()
	{
		if (!readyToRunRoutine)
		{
			readyToRunRoutine = true;
		}
	}

	private void Update()
	{
		if (readyToRunRoutine)
		{
			bool flag = false;
			if (GlobalStorage.Instance.MasterHMDAndInputController.GetButtonDownEitherHand(HandController.HandControllerButton.Menu))
			{
				flag = true;
			}
			if (flag && !routineRan)
			{
				routineRan = true;
				StartCoroutine(MenuButtonPressed());
			}
		}
	}

	private IEnumerator MenuButtonPressed()
	{
		GameEventsManager.Instance.ItemActionOccurred(menuButton, "ACTIVATED");
		yield return null;
		LoadIntoJob();
	}

	private void LoadIntoJob(bool isLongLoad = true)
	{
		if (!isLoadingIntoJob)
		{
			overtimeSwitch.Grabbable.enabled = false;
			isLoadingIntoJob = true;
			AudioManager.Instance.Play(audioPosition, loadJobSound, 1f, 1f);
			jobTransitionEffects.SetActive(true);
			if (isLongLoad)
			{
				StartCoroutine(LoadJobDelay(jobLoadDelayTime));
			}
			else
			{
				StartCoroutine(LoadJobDelay(0.001f));
			}
		}
	}

	private IEnumerator LoadJobDelay(float delay)
	{
		yield return new WaitForSeconds(jobLoadDelayTime);
		JobGenieCartridge.GenieModeTypes flags = JobGenieCartridge.GenieModeTypes.None;
		if (currentlyAttachedJobWithGenieFlags != null)
		{
			flags = currentlyAttachedJobWithGenieFlags.GenieFlags;
		}
		if (flags != 0 && flags != JobGenieCartridge.GenieModeTypes.OfficeModMode)
		{
			AchievementManager.CompleteAchievement(5);
		}
		if (loadOvertimeMode)
		{
			flags |= JobGenieCartridge.GenieModeTypes.EndlessMode;
		}
		LevelLoader.Instance.LoadJob(levelToLoad, taskToLoad, flags);
	}

	private void CheckSum(string ID)
	{
		switch (ID)
		{
		case "Office":
			currentChk += "1";
			break;
		case "Kitchen":
			currentChk += "2";
			break;
		case "ConvenienceStore":
			currentChk += "3";
			break;
		case "AutoMechanic":
			currentChk += "4";
			break;
		default:
			currentChk += " ";
			break;
		}
		if (currentChk.Length > "234121432".Length)
		{
			currentChk = currentChk.Substring(currentChk.Length - "234121432".Length);
		}
		if (currentChk == "234121432" && ExtraPrefs.ExtraProgress == 4)
		{
			GameEventsManager.Instance.ItemActionOccurred(terminalItem.Data, "USED");
			ExtraPrefs.ExtraProgress = 5;
		}
	}

	public void ReCheckCartridge(AttachablePoint point, AttachableObject obj)
	{
		if (cartridgeSocket.NumAttachedObjects > 0)
		{
			GameCartridge component = cartridgeSocket.GetAttachedObject(0).gameObject.GetComponent<GameCartridge>();
			CheckCartridge(component);
		}
	}

	private void CheckCartridge(GameCartridge attachedCartridge)
	{
		if (attachedCartridge == null)
		{
			Debug.LogWarning("There is no cartridge attached");
			return;
		}
		attachedCartridge.SetTerminalReference(this);
		currentlyAttachedJobWithGenieFlags = attachedCartridge.GetJobCartridgeWithGenieFlags();
		if (currentlyAttachedJobWithGenieFlags.BaseJobCartridge == null)
		{
			return;
		}
		CheckSum(currentlyAttachedJobWithGenieFlags.BaseJobCartridge.StateData.JobLevelData.SceneName);
		if (currentlyAttachedJobWithGenieFlags != null && currentlyAttachedJobWithGenieFlags.BaseJobCartridge != null)
		{
			hologram.SetHologram(currentlyAttachedJobWithGenieFlags, loadOvertimeMode);
			terminalUI.LoadJob(currentlyAttachedJobWithGenieFlags);
			if (linesAnimator.gameObject.activeInHierarchy)
			{
				linesAnimator.Play("TerminalLineActivated");
			}
		}
	}

	private void CartridgeAttached(AttachablePoint point, AttachableObject ao)
	{
		if ((bool)cartridgeLights)
		{
			cartridgeLights.SetActive(true);
		}
		GameCartridge component = ao.GetComponent<GameCartridge>();
		CheckCartridge(component);
		for (int i = 0; i < toggleOffOnAttach.Length; i++)
		{
			toggleOffOnAttach[i].gameObject.SetActive(false);
		}
	}

	private void CartridgeDetached(AttachablePoint point, AttachableObject ao)
	{
		GameCartridge component = ao.GetComponent<GameCartridge>();
		if (component != null)
		{
			component.SetTerminalReference(null);
		}
		if ((bool)cartridgeLights)
		{
			cartridgeLights.SetActive(false);
		}
		currentlyAttachedJobWithGenieFlags = null;
		terminalUI.PowerOffTerminal();
		hologram.DeactivateAll();
		if (linesAnimator.gameObject.activeInHierarchy)
		{
			linesAnimator.Play("TerminalLineDeactivated");
		}
		for (int i = 0; i < toggleOffOnAttach.Length; i++)
		{
			toggleOffOnAttach[i].gameObject.SetActive(true);
		}
	}

	private void OvertimeSwitchActivated(OwlchemyVR2.GrabbableHinge hinge)
	{
		AnalyticsManager.CustomEvent("Overtime Switch Activated", null);
		if (!loadOvertimeMode)
		{
			loadOvertimeMode = true;
			if (currentlyAttachedJobWithGenieFlags != null)
			{
				ReCheckCartridge(null, null);
			}
		}
	}

	private void OvertimeSwitchReset(OwlchemyVR2.GrabbableHinge hinge)
	{
		if (loadOvertimeMode)
		{
			loadOvertimeMode = false;
			if (currentlyAttachedJobWithGenieFlags != null)
			{
				ReCheckCartridge(null, null);
			}
		}
	}
}
