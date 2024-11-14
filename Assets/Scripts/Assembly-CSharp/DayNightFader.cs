using System;
using System.Collections;
using System.Collections.Generic;
using OwlchemyVR2;
using UnityEngine;

public class DayNightFader : MonoBehaviour
{
	[SerializeField]
	private float fadespeed = 0.2f;

	[SerializeField]
	private List<MeshRenderer> ShadowFaders;

	private Material[] ShadowMats;

	[SerializeField]
	private MeshRenderer[] AlphaFaders;

	private Color[] StartAlphas;

	[SerializeField]
	private ParticleSystem[] ParticleToggles;

	[SerializeField]
	private Animator[] AnimatorToggles;

	[SerializeField]
	private Animation[] AnimationToggles;

	[SerializeField]
	private Animator LogoFlip;

	private bool fading;

	private float fade = 1f;

	private float targetfade = 1f;

	private Shader ShadowShader;

	private int ShadowScaleInt;

	private int TintColorInt;

	[SerializeField]
	private OwlchemyVR2.GrabbableHinge dayNightSwitch;

	private bool hasCompletedInitialMuseumTask;

	[SerializeField]
	private AudioClip giantSwitchClip;

	[SerializeField]
	private AudioClip smallSwitchClip;

	[SerializeField]
	private AudioClip errorClip;

	[SerializeField]
	private SubtaskData associatedSubtask;

	private bool isOvertimeMode;

	private static DayNightFader _instance;

	public bool IsOvertimeMode
	{
		get
		{
			return isOvertimeMode;
		}
	}

	public bool HasCompletedInitialMuseumTask
	{
		get
		{
			return hasCompletedInitialMuseumTask;
		}
		set
		{
			hasCompletedInitialMuseumTask = value;
		}
	}

	public static DayNightFader instance
	{
		get
		{
			return _instance;
		}
	}

	public event Action<bool> OvertimeModeSwapped;

	private void OnEnable()
	{
		dayNightSwitch.OnHingeActivated += SwitchActivated;
		dayNightSwitch.OnHingeReset += SwitchReset;
		hasCompletedInitialMuseumTask = GlobalStorage.Instance.GameStateData.HasSavedData();
		SetSwitchSounds();
		JobBoardManager jobBoardManager = JobBoardManager.instance;
		jobBoardManager.OnSubtaskComplete = (Action<SubtaskStatusController>)Delegate.Combine(jobBoardManager.OnSubtaskComplete, new Action<SubtaskStatusController>(SubtaskCompleted));
	}

	private void OnDisable()
	{
		dayNightSwitch.OnHingeActivated -= SwitchActivated;
		dayNightSwitch.OnHingeReset -= SwitchReset;
		JobBoardManager jobBoardManager = JobBoardManager.instance;
		jobBoardManager.OnSubtaskComplete = (Action<SubtaskStatusController>)Delegate.Remove(jobBoardManager.OnSubtaskComplete, new Action<SubtaskStatusController>(SubtaskCompleted));
	}

	private void SwitchReset(OwlchemyVR2.GrabbableHinge arg1)
	{
		if (hasCompletedInitialMuseumTask)
		{
			LightsOn();
			GameEventsManager.Instance.ItemActionOccurred(arg1.Grabbable.InteractableItem.WorldItemData, "DEACTIVATED");
		}
	}

	private void SwitchActivated(OwlchemyVR2.GrabbableHinge arg1)
	{
		if (hasCompletedInitialMuseumTask)
		{
			LightsOff();
			GameEventsManager.Instance.ItemActionOccurred(arg1.Grabbable.InteractableItem.WorldItemData, "ACTIVATED");
		}
	}

	private void Start()
	{
		if (_instance != this)
		{
			if (_instance != null)
			{
				UnityEngine.Object.Destroy(_instance);
			}
			_instance = this;
		}
		StartCoroutine(Setup());
	}

	private IEnumerator Setup()
	{
		yield return new WaitForSeconds(1f);
		MeshRenderer[] BotMeshes = GameObject.Find("BotManager").transform.GetComponentsInChildren<MeshRenderer>();
		for (int k = 1; k < BotMeshes.Length; k++)
		{
			ShadowFaders.Add(BotMeshes[k]);
		}
		ShadowMats = new Material[ShadowFaders.Count];
		for (int j = 0; j < ShadowMats.Length; j++)
		{
			ShadowMats[j] = ShadowFaders[j].material;
		}
		StartAlphas = new Color[AlphaFaders.Length];
		for (int i = 0; i < StartAlphas.Length; i++)
		{
			StartAlphas[i] = AlphaFaders[i].material.GetColor("_TintColor");
		}
		ShadowScaleInt = Shader.PropertyToID("_ShadowScale");
		TintColorInt = Shader.PropertyToID("_TintColor");
		MonoBehaviour.print("Finished Setup");
		UpdateMats();
		yield return null;
	}

	private void Update()
	{
		if (fade != targetfade)
		{
			fade = Mathf.MoveTowards(fade, targetfade, fadespeed * Time.deltaTime);
			UpdateMats();
		}
	}

	public void LightsOff()
	{
		for (int i = 0; i < ParticleToggles.Length; i++)
		{
			ParticleToggles[i].Stop();
		}
		for (int j = 0; j < AnimatorToggles.Length; j++)
		{
			AnimatorToggles[j].speed = 0f;
		}
		for (int k = 0; k < AnimationToggles.Length; k++)
		{
			AnimationToggles[k].Stop();
		}
		LogoFlip.SetBool("Endless", true);
		targetfade = 0f;
		isOvertimeMode = true;
		if (this.OvertimeModeSwapped != null)
		{
			this.OvertimeModeSwapped(IsOvertimeMode);
		}
	}

	public void LightsOn()
	{
		for (int i = 0; i < ParticleToggles.Length; i++)
		{
			ParticleToggles[i].Play();
		}
		for (int j = 0; j < AnimatorToggles.Length; j++)
		{
			AnimatorToggles[j].speed = 1f;
		}
		for (int k = 0; k < AnimationToggles.Length; k++)
		{
			AnimationToggles[k].Play();
		}
		LogoFlip.SetBool("Endless", false);
		targetfade = 1f;
		isOvertimeMode = false;
		if (this.OvertimeModeSwapped != null)
		{
			this.OvertimeModeSwapped(IsOvertimeMode);
		}
	}

	private void UpdateMats()
	{
		for (int i = 0; i < ShadowMats.Length; i++)
		{
			ShadowMats[i].SetFloat(ShadowScaleInt, 1f - fade);
		}
		for (int j = 0; j < AlphaFaders.Length; j++)
		{
			AlphaFaders[j].material.SetColor(TintColorInt, new Color(StartAlphas[j].r, StartAlphas[j].g, StartAlphas[j].b, fade * StartAlphas[j].a));
		}
	}

	private void SetSwitchSounds()
	{
		if (!hasCompletedInitialMuseumTask)
		{
			dayNightSwitch.SetActivateSound(errorClip);
			dayNightSwitch.SetResetSound(smallSwitchClip);
		}
		else
		{
			dayNightSwitch.SetActivateSound(giantSwitchClip);
			dayNightSwitch.SetResetSound(giantSwitchClip);
		}
	}

	public void ToggleCanUseSwitch(bool canUseSwitch)
	{
		hasCompletedInitialMuseumTask = canUseSwitch;
		SetSwitchSounds();
	}

	private void SubtaskCompleted(SubtaskStatusController subtask)
	{
		if (subtask.Data == associatedSubtask)
		{
			hasCompletedInitialMuseumTask = true;
			SetSwitchSounds();
		}
	}
}
