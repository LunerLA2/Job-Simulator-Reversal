using System;
using OwlchemyVR;
using UnityEngine;

public class EnginePourCooldownController : VehicleHardware
{
	private const float ENGINE_COOL_AMOUNT = 175f;

	[SerializeField]
	private ParticleSystem particleSystem;

	[SerializeField]
	private ParticleCollectionZone particleCollectionZone;

	[SerializeField]
	private AudioSourceHelper audioSourceHelper;

	[SerializeField]
	private AudioSourceHelper engineBurnSFX;

	[SerializeField]
	private SubtaskData coolingSubtask;

	[SerializeField]
	private SubtaskData coolingSubtask_Endless;

	private GameObject engineTransform;

	private bool isEngineCool;

	private void OnEnable()
	{
		ParticleCollectionZone obj = particleCollectionZone;
		obj.OnParticleIsCollecting = (Action<ParticleCollectionZone, WorldItemData, float>)Delegate.Combine(obj.OnParticleIsCollecting, new Action<ParticleCollectionZone, WorldItemData, float>(OnParticleIsCollecting));
		JobBoardManager instance = JobBoardManager.instance;
		instance.OnSubtaskComplete = (Action<SubtaskStatusController>)Delegate.Combine(instance.OnSubtaskComplete, new Action<SubtaskStatusController>(SubtaskComplete));
	}

	private void OnDisable()
	{
		ParticleCollectionZone obj = particleCollectionZone;
		obj.OnParticleIsCollecting = (Action<ParticleCollectionZone, WorldItemData, float>)Delegate.Remove(obj.OnParticleIsCollecting, new Action<ParticleCollectionZone, WorldItemData, float>(OnParticleIsCollecting));
		JobBoardManager instance = JobBoardManager.instance;
		instance.OnSubtaskComplete = (Action<SubtaskStatusController>)Delegate.Remove(instance.OnSubtaskComplete, new Action<SubtaskStatusController>(SubtaskComplete));
	}

	private void Update()
	{
		if (engineTransform == null)
		{
			engineTransform = parentChassis.EngineTransform;
			if (engineTransform == null)
			{
				return;
			}
		}
		if (!isEngineCool && engineTransform.activeInHierarchy && !particleSystem.isPlaying)
		{
			particleSystem.Play();
		}
		else if ((isEngineCool || !engineTransform.activeInHierarchy) && particleSystem.isPlaying)
		{
			particleSystem.Stop();
		}
	}

	private void OnParticleIsCollecting(ParticleCollectionZone pcz, WorldItemData worldItemData, float arg3)
	{
		GameEventsManager.Instance.ItemActionOccurredWithAmount(pcz.CollectionZoneWorldItem.Data, "AMOUNT_CHANGE_ANY_FLUID", pcz.GetTotalQuantity());
		if (!isEngineCool && pcz.GetTotalQuantity() >= 175f)
		{
			CoolEngine();
		}
	}

	private void SubtaskComplete(SubtaskStatusController subtask)
	{
		if (!isEngineCool && (subtask.Data == coolingSubtask || subtask.Data == coolingSubtask_Endless))
		{
			CoolEngine();
		}
	}

	private void CoolEngine()
	{
		engineBurnSFX.Stop();
		isEngineCool = true;
		audioSourceHelper.Play();
	}
}
