using System;
using System.Collections.Generic;
using OwlchemyVR;
using UnityEngine;

public class BrainStatusController
{
	private BrainData brain;

	private List<BrainEntryStatusController> entryStatusControllersList;

	private BrainControlledObject controlled;

	private Dictionary<string, int> localCustomCounters;

	public Action<BrainData> OnBrainControlledObjectAppeared;

	public Action<BrainData> OnBrainControlledObjectDisappeared;

	public Action<string> OnGlobalCustomMessage;

	public Action<string> OnGlobalCustomMessageClear;

	public Action<string, BrainData> OnLocalCustomMessage;

	public Action<string, BrainData> OnLocalCustomMessageClear;

	public Action<string, int, BrainData> OnLocalCustomCounterChangedSelf;

	public Action<string, int, BrainData> OnLocalCustomCounterNeedsToChangeByAmount;

	public Action<string, int, BrainData> OnLocalCustomCounterNeedsToSetToAmount;

	public Action<string, int> OnGlobalCustomCountersChangeByAmount;

	public Action<string, int> OnGlobalCustomCountersSetToAmount;

	private int lastPlayedRandomIndex = -1;

	public BrainData Brain
	{
		get
		{
			return brain;
		}
	}

	public List<BrainEntryStatusController> EntryStatusControllersList
	{
		get
		{
			return entryStatusControllersList;
		}
	}

	public BrainControlledObject Controlled
	{
		get
		{
			return controlled;
		}
	}

	public BrainStatusController(BrainData data)
	{
		brain = data;
		localCustomCounters = new Dictionary<string, int>();
		entryStatusControllersList = new List<BrainEntryStatusController>();
		BrainEntry[] allEntries = brain.GetAllEntries();
		for (int i = 0; i < allEntries.Length; i++)
		{
			BrainEntryStatusController brainEntryStatusController = new BrainEntryStatusController(allEntries[i]);
			EntryStatusControllersList.Add(brainEntryStatusController);
			brainEntryStatusController.OnEffectTriggered = (Action<BrainEffect>)Delegate.Combine(brainEntryStatusController.OnEffectTriggered, new Action<BrainEffect>(EffectTriggered));
			brainEntryStatusController.OnEntryRequestedAllOtherEffectsToCancel = (Action<BrainEntryStatusController>)Delegate.Combine(brainEntryStatusController.OnEntryRequestedAllOtherEffectsToCancel, new Action<BrainEntryStatusController>(EntryRequestedAllOtherEffectsToCancel));
			brainEntryStatusController.OnEntryRequestedOtherEffectsOfTypeToCancel = (Action<BrainEntryStatusController, BrainEffect.EffectTypes>)Delegate.Combine(brainEntryStatusController.OnEntryRequestedOtherEffectsOfTypeToCancel, new Action<BrainEntryStatusController, BrainEffect.EffectTypes>(EntryRequestedOtherEffectsOfTypeToCancel));
		}
		if (brain.BrainType == BrainData.BrainTypes.CustomSceneObject)
		{
			if (brain.PresentInSceneByDefault)
			{
				AppearInScene(BrainEffect.AppearTypes.Default, string.Empty);
				return;
			}
			BrainControlledObject controlledSceneObject = GetControlledSceneObject();
			controlledSceneObject.Disappear();
		}
	}

	private BrainControlledObject GetControlledSceneObject()
	{
		UniqueObject objectByName = BotUniqueElementManager.Instance.GetObjectByName(brain.UniqueObjectIDOfControlledObject);
		if (objectByName != null)
		{
			BrainControlledObject component = objectByName.GetComponent<BrainControlledObject>();
			if (component == null)
			{
				Debug.LogError("Couldn't find a BrainControlledObject on '" + brain.UniqueObjectIDOfControlledObject + "'");
			}
			return component;
		}
		return null;
	}

	private void AppearInScene(BrainEffect.AppearTypes appearType, string optionalUniqueID = "")
	{
		if (controlled == null)
		{
			if (brain.BrainType == BrainData.BrainTypes.Bot)
			{
				controlled = BotManager.Instance.GetBot();
			}
			else if (brain.BrainType == BrainData.BrainTypes.CustomSceneObject)
			{
				controlled = GetControlledSceneObject();
			}
			else if (brain.BrainType == BrainData.BrainTypes.CustomSpawnableObject)
			{
				Debug.LogError("CustomSpawnableObject not yet implemented (would normally spawn here)");
			}
			controlled.Appear(brain);
			if (OnBrainControlledObjectAppeared != null)
			{
				OnBrainControlledObjectAppeared(brain);
			}
			switch (appearType)
			{
			case BrainEffect.AppearTypes.AtPosition:
				controlled.MoveToPosition(optionalUniqueID, 0f);
				break;
			case BrainEffect.AppearTypes.AtFirstWaypoint:
				controlled.MoveToWaypoint(optionalUniqueID, 0, 0f);
				break;
			case BrainEffect.AppearTypes.AtLastWaypoint:
				controlled.MoveToWaypoint(optionalUniqueID, -1, 0f);
				break;
			}
		}
	}

	private void DisappearFromScene()
	{
		if (!(controlled != null))
		{
			return;
		}
		BrainData.BrainTypes brainType = controlled.CurrentBrainData.BrainType;
		controlled.Disappear();
		switch (brainType)
		{
		case BrainData.BrainTypes.Bot:
			if (controlled is Bot)
			{
				BotManager.Instance.ReleaseBot(controlled as Bot);
			}
			break;
		case BrainData.BrainTypes.CustomSpawnableObject:
			Debug.LogError("CustomSpawnableObject not yet implemented (would normally de-spawn here)");
			break;
		}
		controlled = null;
		if (OnBrainControlledObjectDisappeared != null)
		{
			OnBrainControlledObjectDisappeared(brain);
		}
	}

	public int ManuallyChangeCounterAndGetNewValue(string id, int mod)
	{
		if (localCustomCounters.ContainsKey(id))
		{
			Dictionary<string, int> dictionary;
			Dictionary<string, int> dictionary2 = (dictionary = localCustomCounters);
			string key;
			string key2 = (key = id);
			int num = dictionary[key];
			dictionary2[key2] = num + mod;
		}
		else
		{
			localCustomCounters[id] = mod;
		}
		return localCustomCounters[id];
	}

	public void ManuallySetCounter(string id, int setTo)
	{
		localCustomCounters[id] = setTo;
	}

	private BrainEffect.CustomMessageTypes GetCustomMessageTypeFromInt(int number)
	{
		BrainEffect.CustomMessageTypes result = BrainEffect.CustomMessageTypes.LocalToThisBrain;
		foreach (int value in Enum.GetValues(typeof(BrainEffect.CustomMessageTypes)))
		{
			if (value == number)
			{
				return (BrainEffect.CustomMessageTypes)value;
			}
		}
		return result;
	}

	private void EntryRequestedAllOtherEffectsToCancel(BrainEntryStatusController entryStatus)
	{
		for (int i = 0; i < entryStatusControllersList.Count; i++)
		{
			entryStatusControllersList[i].CancelAllPendingEffects();
		}
	}

	private void EntryRequestedOtherEffectsOfTypeToCancel(BrainEntryStatusController entryStatus, BrainEffect.EffectTypes effectType)
	{
		for (int i = 0; i < entryStatusControllersList.Count; i++)
		{
			entryStatusControllersList[i].CancelPendingEffectsOfType(effectType);
		}
	}

	private void EffectTriggered(BrainEffect effect)
	{
		if (effect.EffectType == BrainEffect.EffectTypes.DebugLog)
		{
			Debug.Log("<color=green>Brain: </color>" + effect.TextInfo);
		}
		else if (effect.EffectType == BrainEffect.EffectTypes.Appear)
		{
			AppearInScene((BrainEffect.AppearTypes)effect.NumberInfo, effect.TextInfo);
		}
		else if (effect.EffectType == BrainEffect.EffectTypes.Disappear)
		{
			DisappearFromScene();
		}
		else if (effect.EffectType == BrainEffect.EffectTypes.GameEvent)
		{
			BrainEffect.GameEventTypes gameEventTypes = BrainEffect.GameEventTypes.ActionOnSelf;
			foreach (int value in Enum.GetValues(typeof(BrainEffect.GameEventTypes)))
			{
				if (value == effect.NumberInfo)
				{
					gameEventTypes = (BrainEffect.GameEventTypes)value;
				}
			}
			switch (gameEventTypes)
			{
			case BrainEffect.GameEventTypes.ActionNoItem:
				GameEventsManager.Instance.ActionOccurred(effect.ActionEventDataInfo.ActionEventName);
				break;
			case BrainEffect.GameEventTypes.ItemAction:
				GameEventsManager.Instance.ItemActionOccurred(effect.WorldItemDataInfo, effect.ActionEventDataInfo.ActionEventName);
				break;
			case BrainEffect.GameEventTypes.ActionOnSelf:
			{
				WorldItemData currentWorldItemData = controlled.GetCurrentWorldItemData();
				if (currentWorldItemData != null)
				{
					GameEventsManager.Instance.ItemActionOccurred(currentWorldItemData, effect.ActionEventDataInfo.ActionEventName);
				}
				else
				{
					Debug.LogError("Can't do a GameEvent ActionOnSelf because GetCurrentWorldItemData returned null for " + controlled.name, controlled.gameObject);
				}
				break;
			}
			}
		}
		else if (effect.EffectType == BrainEffect.EffectTypes.CustomMessage)
		{
			switch (GetCustomMessageTypeFromInt(effect.NumberInfo))
			{
			case BrainEffect.CustomMessageTypes.AcrossAllBrains:
				if (OnGlobalCustomMessage != null)
				{
					OnGlobalCustomMessage(effect.TextInfo);
				}
				break;
			case BrainEffect.CustomMessageTypes.LocalToThisBrain:
				if (OnLocalCustomMessage != null)
				{
					OnLocalCustomMessage(effect.TextInfo, brain);
				}
				break;
			case BrainEffect.CustomMessageTypes.SpecificBrainOnly:
				if (OnLocalCustomMessage != null)
				{
					OnLocalCustomMessage(effect.TextInfo, effect.BrainDataInfo);
				}
				break;
			}
		}
		else if (effect.EffectType == BrainEffect.EffectTypes.CustomMessageClear)
		{
			switch (GetCustomMessageTypeFromInt(effect.NumberInfo))
			{
			case BrainEffect.CustomMessageTypes.AcrossAllBrains:
				if (OnGlobalCustomMessageClear != null)
				{
					OnGlobalCustomMessageClear(effect.TextInfo);
				}
				break;
			case BrainEffect.CustomMessageTypes.LocalToThisBrain:
				if (OnLocalCustomMessageClear != null)
				{
					OnLocalCustomMessageClear(effect.TextInfo, brain);
				}
				break;
			case BrainEffect.CustomMessageTypes.SpecificBrainOnly:
				if (OnLocalCustomMessageClear != null)
				{
					OnLocalCustomMessageClear(effect.TextInfo, effect.BrainDataInfo);
				}
				break;
			}
		}
		else if (effect.EffectType == BrainEffect.EffectTypes.CustomCounterUp)
		{
			switch (GetCustomMessageTypeFromInt(effect.NumberInfo))
			{
			case BrainEffect.CustomMessageTypes.LocalToThisBrain:
				if (localCustomCounters.ContainsKey(effect.TextInfo))
				{
					Dictionary<string, int> dictionary;
					Dictionary<string, int> dictionary2 = (dictionary = localCustomCounters);
					string textInfo;
					string key = (textInfo = effect.TextInfo);
					int num = dictionary[textInfo];
					dictionary2[key] = num + 1;
				}
				else
				{
					localCustomCounters[effect.TextInfo] = 1;
				}
				if (OnLocalCustomCounterChangedSelf != null)
				{
					OnLocalCustomCounterChangedSelf(effect.TextInfo, localCustomCounters[effect.TextInfo], brain);
				}
				break;
			case BrainEffect.CustomMessageTypes.SpecificBrainOnly:
				if (OnLocalCustomCounterNeedsToChangeByAmount != null)
				{
					OnLocalCustomCounterNeedsToChangeByAmount(effect.TextInfo, 1, effect.BrainDataInfo);
				}
				break;
			case BrainEffect.CustomMessageTypes.AcrossAllBrains:
				if (OnGlobalCustomCountersChangeByAmount != null)
				{
					OnGlobalCustomCountersChangeByAmount(effect.TextInfo, 1);
				}
				break;
			}
		}
		else if (effect.EffectType == BrainEffect.EffectTypes.CustomCounterDown)
		{
			switch (GetCustomMessageTypeFromInt(effect.NumberInfo))
			{
			case BrainEffect.CustomMessageTypes.LocalToThisBrain:
				if (localCustomCounters.ContainsKey(effect.TextInfo))
				{
					Dictionary<string, int> dictionary3;
					Dictionary<string, int> dictionary4 = (dictionary3 = localCustomCounters);
					string textInfo;
					string key2 = (textInfo = effect.TextInfo);
					int num = dictionary3[textInfo];
					dictionary4[key2] = num - 1;
				}
				else
				{
					localCustomCounters[effect.TextInfo] = -1;
				}
				if (OnLocalCustomCounterChangedSelf != null)
				{
					OnLocalCustomCounterChangedSelf(effect.TextInfo, localCustomCounters[effect.TextInfo], brain);
				}
				break;
			case BrainEffect.CustomMessageTypes.SpecificBrainOnly:
				if (OnLocalCustomCounterNeedsToChangeByAmount != null)
				{
					OnLocalCustomCounterNeedsToChangeByAmount(effect.TextInfo, -1, effect.BrainDataInfo);
				}
				break;
			case BrainEffect.CustomMessageTypes.AcrossAllBrains:
				if (OnGlobalCustomCountersChangeByAmount != null)
				{
					OnGlobalCustomCountersChangeByAmount(effect.TextInfo, -1);
				}
				break;
			}
		}
		else if (effect.EffectType == BrainEffect.EffectTypes.CustomCounterSet)
		{
			switch (GetCustomMessageTypeFromInt(effect.NumberInfo))
			{
			case BrainEffect.CustomMessageTypes.LocalToThisBrain:
				localCustomCounters[effect.TextInfo] = Mathf.RoundToInt(effect.FloatInfo);
				if (OnLocalCustomCounterChangedSelf != null)
				{
					OnLocalCustomCounterChangedSelf(effect.TextInfo, localCustomCounters[effect.TextInfo], brain);
				}
				break;
			case BrainEffect.CustomMessageTypes.SpecificBrainOnly:
				if (OnLocalCustomCounterNeedsToSetToAmount != null)
				{
					OnLocalCustomCounterNeedsToSetToAmount(effect.TextInfo, Mathf.RoundToInt(effect.FloatInfo), effect.BrainDataInfo);
				}
				break;
			case BrainEffect.CustomMessageTypes.AcrossAllBrains:
				if (OnGlobalCustomCountersSetToAmount != null)
				{
					OnGlobalCustomCountersSetToAmount(effect.TextInfo, Mathf.RoundToInt(effect.FloatInfo));
				}
				break;
			}
		}
		else if (effect.EffectType == BrainEffect.EffectTypes.AnimateObject)
		{
			if (!effect.TextInfo.Contains("|"))
			{
				return;
			}
			string text = effect.TextInfo.Split('|')[0];
			string text2 = effect.TextInfo.Split('|')[1];
			UniqueObject objectByName = BotUniqueElementManager.Instance.GetObjectByName(text);
			if (!(objectByName != null))
			{
				return;
			}
			Animation component = objectByName.GetComponent<Animation>();
			if (component != null)
			{
				if (text2 != string.Empty)
				{
					component.Play(text2);
				}
				else
				{
					component.Play();
				}
			}
			else
			{
				Debug.LogError("Can't play animation on '" + text + "' because it has no animation component.");
			}
		}
		else
		{
			if (controlled == null)
			{
				return;
			}
			if (effect.EffectType == BrainEffect.EffectTypes.PlayVO)
			{
				switch ((BrainEffect.PlayVOTypes)effect.NumberInfo)
				{
				case BrainEffect.PlayVOTypes.SingleAudioClip:
					controlled.PlayVO(effect.AudioClipInfo, (BotVoiceController.VOImportance)Mathf.RoundToInt(effect.FloatInfo));
					break;
				case BrainEffect.PlayVOTypes.SingleBotVOInfo:
					controlled.PlayVO(effect.BotVOInfoDataInfo, (BotVoiceController.VOImportance)Mathf.RoundToInt(effect.FloatInfo));
					break;
				case BrainEffect.PlayVOTypes.RandomFromList:
				{
					int num2 = UnityEngine.Random.Range(0, effect.AudioClipListInfo.Count + effect.BotVOInfoDataListInfo.Count);
					if (num2 == lastPlayedRandomIndex)
					{
						num2++;
						if (num2 >= effect.AudioClipListInfo.Count + effect.BotVOInfoDataListInfo.Count)
						{
							num2 = 0;
						}
					}
					lastPlayedRandomIndex = num2;
					if (num2 < effect.AudioClipListInfo.Count)
					{
						controlled.PlayVO(effect.AudioClipListInfo[num2], (BotVoiceController.VOImportance)Mathf.RoundToInt(effect.FloatInfo));
						break;
					}
					int index = num2 - effect.AudioClipListInfo.Count;
					controlled.PlayVO(effect.BotVOInfoDataListInfo[index], (BotVoiceController.VOImportance)Mathf.RoundToInt(effect.FloatInfo));
					break;
				}
				}
			}
			else if (effect.EffectType == BrainEffect.EffectTypes.StopVO)
			{
				controlled.StopVO();
			}
			else if (effect.EffectType == BrainEffect.EffectTypes.Emote)
			{
				BotFaceEmote emote = BotFaceEmote.Idle;
				foreach (int value2 in Enum.GetValues(typeof(BotFaceEmote)))
				{
					if (((BotFaceEmote)value2).ToString() == effect.TextInfo)
					{
						emote = (BotFaceEmote)value2;
					}
				}
				controlled.Emote(emote, effect.SpriteInfo);
			}
			else if (effect.EffectType == BrainEffect.EffectTypes.MoveAlongPath)
			{
				controlled.MoveAlongPath(effect.TextInfo, (BrainEffect.PathTypes)effect.NumberInfo, (BrainEffect.PathLookTypes)Mathf.RoundToInt(effect.FloatInfo));
			}
			else if (effect.EffectType == BrainEffect.EffectTypes.MoveToWaypoint)
			{
				controlled.MoveToWaypoint(effect.TextInfo, effect.NumberInfo, effect.FloatInfo);
			}
			else if (effect.EffectType == BrainEffect.EffectTypes.MoveToPosition)
			{
				controlled.MoveToPosition(effect.TextInfo, effect.FloatInfo);
			}
			else if (effect.EffectType == BrainEffect.EffectTypes.StopMoving)
			{
				controlled.StopMoving();
			}
			else if (effect.EffectType == BrainEffect.EffectTypes.ChangeParent)
			{
				controlled.ChangeParent(effect.TextInfo, (BrainEffect.ChangeParentTypes)effect.NumberInfo, effect.FloatInfo);
			}
			else if (effect.EffectType == BrainEffect.EffectTypes.AddCostumePiece)
			{
				controlled.AddCostumePiece(effect.GameObjectInfo);
			}
			else if (effect.EffectType == BrainEffect.EffectTypes.ChangeCostume)
			{
				controlled.ChangeCostume(effect.CostumeDataInfo);
			}
			else if (effect.EffectType == BrainEffect.EffectTypes.ItemsOfInterest)
			{
				BrainEffect.ItemOfInterestTypes numberInfo = (BrainEffect.ItemOfInterestTypes)effect.NumberInfo;
				controlled.ItemsOfInterest(numberInfo, effect.WorldItemDataInfo, effect.FloatInfo);
			}
			else if (effect.EffectType == BrainEffect.EffectTypes.GrabItem)
			{
				controlled.GrabItem(effect.TextInfo, effect.NumberInfo == 1);
			}
			else if (effect.EffectType == BrainEffect.EffectTypes.EjectPrefab)
			{
				BotInventoryController.EjectTypes numberInfo2 = (BotInventoryController.EjectTypes)effect.NumberInfo;
				controlled.EjectPrefab(effect.GameObjectInfo, numberInfo2, effect.TextInfo.Trim(), effect.FloatInfo);
			}
			else if (effect.EffectType == BrainEffect.EffectTypes.EmptyInventory)
			{
				controlled.EmptyInventory((BotInventoryController.EmptyTypes)Mathf.RoundToInt(effect.FloatInfo), effect.TextInfo.Trim(), effect.NumberInfo == 1, effect.WorldItemDataInfo);
			}
			else if (effect.EffectType == BrainEffect.EffectTypes.FloatingHeight)
			{
				controlled.FloatingHeight((BrainEffect.FloatHeightTypes)effect.NumberInfo, effect.FloatInfo, effect.TextInfo);
			}
			else if (effect.EffectType == BrainEffect.EffectTypes.LookAt)
			{
				BrainEffect.LookAtTypes numberInfo3 = (BrainEffect.LookAtTypes)effect.NumberInfo;
				if (numberInfo3 == BrainEffect.LookAtTypes.Bot && effect.BrainDataInfo != null)
				{
					controlled.LookAt(numberInfo3, effect.BrainDataInfo.name, effect.FloatInfo);
				}
				else
				{
					controlled.LookAt(numberInfo3, effect.TextInfo, effect.FloatInfo);
				}
			}
			else if (effect.EffectType == BrainEffect.EffectTypes.ScriptedEffect)
			{
				controlled.ScriptedEffect(effect);
			}
			else if (effect.EffectType == BrainEffect.EffectTypes.ChangeVOProfile)
			{
				controlled.ChangeVOProfile();
			}
			else if (effect.EffectType != BrainEffect.EffectTypes.Appear && effect.EffectType != BrainEffect.EffectTypes.Disappear && effect.EffectType != BrainEffect.EffectTypes.GameEvent && effect.EffectType != BrainEffect.EffectTypes.DebugLog && effect.EffectType != BrainEffect.EffectTypes.CustomCounterDown && effect.EffectType != BrainEffect.EffectTypes.CustomCounterUp && effect.EffectType != BrainEffect.EffectTypes.CustomCounterSet && effect.EffectType != BrainEffect.EffectTypes.CustomMessage && effect.EffectType != BrainEffect.EffectTypes.CustomMessageClear)
			{
				Debug.LogWarning("Effect Type " + effect.EffectType.ToString() + " is not supported yet, but it did fire.");
			}
		}
	}
}
