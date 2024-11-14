using System;
using System.Collections;
using OwlchemyVR;
using UnityEngine;
using UnityEngine.UI;

[SelectionBase]
public class JobCartridge : GameCartridge
{
	[SerializeField]
	private Slider progressSlider;

	[SerializeField]
	private Image sliderFillImage;

	[SerializeField]
	private ParticleSystem FxDots;

	[SerializeField]
	private MeshRenderer accentLight;

	[SerializeField]
	private Text percentageText;

	[SerializeField]
	private MeshRenderer cartridgeArt;

	[SerializeField]
	private WorldItem worldItem;

	[SerializeField]
	private BlowableItem blowableItem;

	[SerializeField]
	private ParticleSystem dustParticleSystem;

	[SerializeField]
	private AudioClip blowingInAudioClip;

	private JobStateData stateData;

	private JobStateData stateDataForGamedev;

	[SerializeField]
	private Color sliderJobCompleteFillColor = Color.yellow;

	[SerializeField]
	private Color jobCompleteLightColor = Color.yellow;

	[SerializeField]
	private Color jobUncomlpeteColor = Color.cyan;

	private Material infiniteOvertimeArt;

	private Material classicArt;

	private bool isOvertimeMode;

	private DayNightFader dayNightFader;

	private bool isComplete;

	public JobStateData StateData
	{
		get
		{
			return stateData;
		}
	}

	public JobStateData StateDataForGamedev
	{
		get
		{
			return stateDataForGamedev;
		}
	}

	private void OnEnable()
	{
		BlowableItem obj = blowableItem;
		obj.OnWasBlown = (Action<BlowableItem, float, HeadController>)Delegate.Combine(obj.OnWasBlown, new Action<BlowableItem, float, HeadController>(WasCartridgeBlown));
		StartCoroutine(SubscribeToDayNightEventsOneFrameLater());
	}

	private IEnumerator SubscribeToDayNightEventsOneFrameLater()
	{
		yield return null;
		dayNightFader = DayNightFader.instance;
		if (dayNightFader != null)
		{
			dayNightFader.OvertimeModeSwapped += SwapOvertimeTexture;
			if (stateData != null && stateData.GetPercentageComplete() == 1f)
			{
				SwapOvertimeTexture(dayNightFader.IsOvertimeMode);
			}
		}
		else
		{
			Debug.LogWarning("DayNightFader is null");
		}
	}

	private void OnDisable()
	{
		BlowableItem obj = blowableItem;
		obj.OnWasBlown = (Action<BlowableItem, float, HeadController>)Delegate.Remove(obj.OnWasBlown, new Action<BlowableItem, float, HeadController>(WasCartridgeBlown));
		if (dayNightFader != null)
		{
			dayNightFader.OvertimeModeSwapped -= SwapOvertimeTexture;
		}
	}

	public void SetUpCartridge(JobStateData jobStateData, JobStateData altJobStateForGamedev, Material newArt, WorldItemData worldItemDataOverride, Material overtimeArt)
	{
		percentageText.text = Mathf.Round(jobStateData.GetPercentageComplete() * 100f) + "%";
		progressSlider.value = jobStateData.GetPercentageComplete();
		stateData = jobStateData;
		stateDataForGamedev = altJobStateForGamedev;
		cartridgeArt.material = newArt;
		classicArt = newArt;
		infiniteOvertimeArt = overtimeArt;
		if (jobStateData.GetPercentageComplete() == 1f)
		{
			sliderFillImage.color = Color.Lerp(sliderJobCompleteFillColor, Color.black, 0.35f);
			FxDots.startColor = jobCompleteLightColor;
			accentLight.material.color = jobCompleteLightColor;
			percentageText.text = string.Empty;
		}
		else
		{
			sliderFillImage.color = jobUncomlpeteColor;
			FxDots.startColor = jobUncomlpeteColor;
			accentLight.material.color = jobUncomlpeteColor;
		}
		worldItem.ManualSetData(worldItemDataOverride);
	}

	private void WasCartridgeBlown(BlowableItem item, float amount, HeadController headController)
	{
		if (!dustParticleSystem.isPlaying && blowableItem.InMouth)
		{
			dustParticleSystem.Play();
			if (blowingInAudioClip != null)
			{
				AudioManager.Instance.Play(base.transform, blowingInAudioClip, 1f, 1f);
			}
		}
	}

	public void EnableBlowing()
	{
		BlowableItem obj = blowableItem;
		obj.OnWasBlown = (Action<BlowableItem, float, HeadController>)Delegate.Combine(obj.OnWasBlown, new Action<BlowableItem, float, HeadController>(WasCartridgeBlown));
		HeadController componentInChildren = GlobalStorage.Instance.MasterHMDAndInputController.Head.GetComponentInChildren<HeadController>();
	}

	public void DisableBlowing()
	{
		dustParticleSystem.Stop();
		BlowableItem obj = blowableItem;
		obj.OnWasBlown = (Action<BlowableItem, float, HeadController>)Delegate.Remove(obj.OnWasBlown, new Action<BlowableItem, float, HeadController>(WasCartridgeBlown));
		blowableItem.SetInMouth(false);
		blowableItem.SetNearMouth(false);
		HeadController componentInChildren = GlobalStorage.Instance.MasterHMDAndInputController.Head.GetComponentInChildren<HeadController>();
		componentInChildren.ForceStopAudio();
	}

	public override JobCartridgeWithGenieFlags GetJobCartridgeWithGenieFlags(JobGenieCartridge.GenieModeTypes types = JobGenieCartridge.GenieModeTypes.None)
	{
		return new JobCartridgeWithGenieFlags(this, types);
	}

	public void SwapOvertimeTexture(bool isOvertime)
	{
		isOvertimeMode = isOvertime;
		if (isOvertime)
		{
			cartridgeArt.material = infiniteOvertimeArt;
		}
		else
		{
			cartridgeArt.material = classicArt;
		}
	}
}
