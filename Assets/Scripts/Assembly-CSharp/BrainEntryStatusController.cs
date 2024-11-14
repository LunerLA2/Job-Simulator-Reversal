using System;
using System.Collections.Generic;
using UnityEngine;

public class BrainEntryStatusController
{
	private BrainEntry entryData;

	private List<BrainCauseStatusController> causeStatusControllerList;

	private List<PendingEffectInfo> pendingEffects = new List<PendingEffectInfo>();

	public Action<BrainEntryStatusController> OnEntryRequestedAllOtherEffectsToCancel;

	public Action<BrainEntryStatusController, BrainEffect.EffectTypes> OnEntryRequestedOtherEffectsOfTypeToCancel;

	public Action<BrainEffect> OnEffectTriggered;

	private bool hasEverBeenCompleted;

	public BrainEntry EntryData
	{
		get
		{
			return entryData;
		}
	}

	public List<BrainCauseStatusController> CauseStatusControllerList
	{
		get
		{
			return causeStatusControllerList;
		}
	}

	public BrainEntryStatusController(BrainEntry data)
	{
		entryData = data;
		hasEverBeenCompleted = false;
		causeStatusControllerList = new List<BrainCauseStatusController>();
		for (int i = 0; i < entryData.Causes.Count; i++)
		{
			BrainCauseStatusController brainCauseStatusController = new BrainCauseStatusController(entryData.Causes[i]);
			causeStatusControllerList.Add(brainCauseStatusController);
			brainCauseStatusController.OnComplete = (Action<BrainCauseStatusController>)Delegate.Combine(brainCauseStatusController.OnComplete, new Action<BrainCauseStatusController>(CauseCompleted));
			brainCauseStatusController.OnUncomplete = (Action<BrainCauseStatusController>)Delegate.Combine(brainCauseStatusController.OnUncomplete, new Action<BrainCauseStatusController>(CauseUncompleted));
		}
	}

	private void CauseCompleted(BrainCauseStatusController causeStatus)
	{
		if (CheckIfCauseLogicCompleted())
		{
			hasEverBeenCompleted = true;
			PerformEffects();
		}
	}

	private void CauseUncompleted(BrainCauseStatusController causeStatus)
	{
	}

	private bool CheckIfCauseLogicCompleted()
	{
		if (entryData.LogicType == BrainEntry.CauseLogicTypes.AllTrue)
		{
			for (int i = 0; i < causeStatusControllerList.Count; i++)
			{
				if (!causeStatusControllerList[i].IsCompleted)
				{
					return false;
				}
			}
			return true;
		}
		if (entryData.LogicType == BrainEntry.CauseLogicTypes.AnyTrue)
		{
			for (int j = 0; j < causeStatusControllerList.Count; j++)
			{
				if (causeStatusControllerList[j].IsCompleted)
				{
					return true;
				}
			}
			return false;
		}
		if (entryData.LogicType == BrainEntry.CauseLogicTypes.OnceAllTrue)
		{
			if (hasEverBeenCompleted)
			{
				return false;
			}
			for (int k = 0; k < causeStatusControllerList.Count; k++)
			{
				if (!causeStatusControllerList[k].IsCompleted)
				{
					return false;
				}
			}
			return true;
		}
		if (entryData.LogicType == BrainEntry.CauseLogicTypes.OnceAnyTrue)
		{
			if (hasEverBeenCompleted)
			{
				return false;
			}
			for (int l = 0; l < causeStatusControllerList.Count; l++)
			{
				if (causeStatusControllerList[l].IsCompleted)
				{
					return true;
				}
			}
			return false;
		}
		Debug.LogError("Invalid causeLogicType");
		return false;
	}

	public void CancelPendingEffectsOfType(BrainEffect.EffectTypes type)
	{
		for (int i = 0; i < pendingEffects.Count; i++)
		{
			if (pendingEffects[i].Effect.EffectType == type)
			{
				TimeManager.CancelInvoke(EffectFired, pendingEffects[i].Index);
				pendingEffects.RemoveAt(i);
				i--;
			}
		}
	}

	public void CancelAllPendingEffects()
	{
		for (int i = 0; i < pendingEffects.Count; i++)
		{
			TimeManager.CancelInvoke(EffectFired, pendingEffects[i].Index);
		}
		pendingEffects.Clear();
	}

	private void PerformEffects()
	{
		if (entryData.EffectOverrideType == BrainEntry.EffectOverrideTypes.CancelSameTypes)
		{
			List<BrainEffect.EffectTypes> list = new List<BrainEffect.EffectTypes>();
			for (int i = 0; i < entryData.Effects.Count; i++)
			{
				if (!list.Contains(entryData.Effects[i].EffectType))
				{
					if (OnEntryRequestedOtherEffectsOfTypeToCancel != null)
					{
						OnEntryRequestedOtherEffectsOfTypeToCancel(this, entryData.Effects[i].EffectType);
					}
					list.Add(entryData.Effects[i].EffectType);
				}
			}
		}
		else if (entryData.EffectOverrideType == BrainEntry.EffectOverrideTypes.DropEverything && OnEntryRequestedAllOtherEffectsToCancel != null)
		{
			OnEntryRequestedAllOtherEffectsToCancel(this);
		}
		float num = 0f;
		float num2 = 0f;
		for (int j = 0; j < entryData.Effects.Count; j++)
		{
			BrainEffect brainEffect = entryData.Effects[j];
			float num3 = brainEffect.Delay;
			if (brainEffect.TimingType == BrainEffect.TimingTypes.WithPrevious)
			{
				num3 += num;
			}
			else if (brainEffect.TimingType == BrainEffect.TimingTypes.AfterPrevious)
			{
				num3 += num2;
			}
			if (num3 <= 0f)
			{
				EffectFired(j);
			}
			else
			{
				pendingEffects.Add(new PendingEffectInfo(entryData.Effects[j], j));
				TimeManager.Invoke(EffectFired, j, num3);
			}
			num = num3;
			num2 = num3 + brainEffect.GetDuration();
		}
	}

	private void EffectFired(int index)
	{
		BrainEffect obj = entryData.Effects[index];
		for (int i = 0; i < pendingEffects.Count; i++)
		{
			if (pendingEffects[i].Index == index)
			{
				pendingEffects.RemoveAt(i);
				break;
			}
		}
		if (OnEffectTriggered != null)
		{
			OnEffectTriggered(obj);
		}
	}
}
