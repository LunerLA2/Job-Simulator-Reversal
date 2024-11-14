using System;
using System.Collections.Generic;
using OwlchemyVR;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class BrainEffect
{
	public enum EffectTypes
	{
		DebugLog = -1,
		PlayVO = 0,
		CustomMessage = 1,
		MoveAlongPath = 2,
		MoveToWaypoint = 3,
		Emote = 4,
		StopVO = 5,
		MoveToPosition = 6,
		AddCostumePiece = 7,
		Appear = 8,
		Disappear = 9,
		ChangeCostume = 10,
		ItemsOfInterest = 11,
		GrabItem = 12,
		EjectPrefab = 13,
		EmptyInventory = 14,
		FloatingHeight = 15,
		LookAt = 16,
		CustomCounterUp = 17,
		CustomCounterDown = 18,
		CustomCounterSet = 24,
		CustomMessageClear = 19,
		ScriptedEffect = 20,
		AnimateObject = 21,
		StopMoving = 22,
		GameEvent = 23,
		ChangeParent = 35,
		ChangeVOProfile = 25
	}

	public enum TimingTypes
	{
		Normal = 0,
		WithPrevious = 1,
		AfterPrevious = 2
	}

	public enum AppearTypes
	{
		Default = 0,
		AtPosition = 1,
		AtFirstWaypoint = 2,
		AtLastWaypoint = 3
	}

	public enum GameEventTypes
	{
		ActionOnSelf = 0,
		ItemAction = 1,
		ActionNoItem = 2
	}

	public enum PathTypes
	{
		Forwards = 0,
		ForwardsSlideIn = 1,
		Backwards = 2,
		BackwardsSlideIn = 3
	}

	public enum PathLookTypes
	{
		LookAlongPath = 0,
		ContinueLookAtLogic = 1
	}

	public enum ItemOfInterestTypes
	{
		AddInventoryItem = 0,
		RemoveInventoryItem = 1,
		ClearAllInventoryItems = 2,
		AddSightItem = 3,
		RemoveSightItem = 4,
		ClearAllSightItems = 5,
		AddGlobalItem = 6,
		RemoveGlobalItem = 7,
		ClearAllGlobalItems = 8,
		ChangeSightRadiusMultiplier = 9,
		ChangeInventoryRadiusMultiplier = 10,
		ChangeGlobalRadiusMultiplier = 11,
		ChangeFilteringType = 12
	}

	public enum LookAtTypes
	{
		Player = 0,
		Object = 1,
		WorldAngle = 2,
		Nothing = 3,
		Bot = 4
	}

	public enum FloatHeightTypes
	{
		MatchPlayer = 0,
		WorldHeight = 1,
		RandomBob = 2,
		SineWave = 3
	}

	public enum PlayVOTypes
	{
		SingleAudioClip = 0,
		SingleBotVOInfo = 1,
		RandomFromList = 2
	}

	public enum ChangeParentTypes
	{
		SnapLocalPosAndRotToZero = 0,
		SnapLocalPosToZero = 1,
		SnapLocalRotToZero = 2,
		TweenLocalPosToZero = 3,
		TweenLocalRotToZero = 4,
		TweenLocalPosAndRotToZero = 5,
		KeepWorldPosAndRot = 6
	}

	public enum CustomMessageTypes
	{
		LocalToThisBrain = 0,
		AcrossAllBrains = 1,
		SpecificBrainOnly = 2
	}

	[SerializeField]
	private float delay;

	[SerializeField]
	private TimingTypes timingType;

	[SerializeField]
	private EffectTypes effectType;

	[FormerlySerializedAs("voClip")]
	[SerializeField]
	private AudioClip audioClipInfo;

	[SerializeField]
	private List<AudioClip> audioClipListInfo = new List<AudioClip>();

	[SerializeField]
	private BotVOInfoData botVOInfoDataInfo;

	[SerializeField]
	private List<BotVOInfoData> botVOInfoDataListInfo = new List<BotVOInfoData>();

	[FormerlySerializedAs("logText")]
	[SerializeField]
	private string textInfo;

	[SerializeField]
	private int numberInfo;

	[SerializeField]
	private float floatInfo;

	[SerializeField]
	private GameObject gameObjectInfo;

	[SerializeField]
	private BotCostumeData costumeDataInfo;

	[SerializeField]
	private Sprite spriteInfo;

	[SerializeField]
	private WorldItemData worldItemDataInfo;

	[SerializeField]
	private ActionEventData actionEventDataInfo;

	[SerializeField]
	private BrainData brainDataInfo;

	public float Delay
	{
		get
		{
			return delay;
		}
	}

	public TimingTypes TimingType
	{
		get
		{
			return timingType;
		}
	}

	public EffectTypes EffectType
	{
		get
		{
			return effectType;
		}
	}

	public AudioClip AudioClipInfo
	{
		get
		{
			return audioClipInfo;
		}
	}

	public List<AudioClip> AudioClipListInfo
	{
		get
		{
			return audioClipListInfo;
		}
	}

	public string TextInfo
	{
		get
		{
			return textInfo;
		}
	}

	public int NumberInfo
	{
		get
		{
			return numberInfo;
		}
	}

	public float FloatInfo
	{
		get
		{
			return floatInfo;
		}
	}

	public GameObject GameObjectInfo
	{
		get
		{
			return gameObjectInfo;
		}
	}

	public BotCostumeData CostumeDataInfo
	{
		get
		{
			return costumeDataInfo;
		}
	}

	public Sprite SpriteInfo
	{
		get
		{
			return spriteInfo;
		}
	}

	public BotVOInfoData BotVOInfoDataInfo
	{
		get
		{
			return botVOInfoDataInfo;
		}
	}

	public List<BotVOInfoData> BotVOInfoDataListInfo
	{
		get
		{
			return botVOInfoDataListInfo;
		}
	}

	public WorldItemData WorldItemDataInfo
	{
		get
		{
			return worldItemDataInfo;
		}
	}

	public BrainData BrainDataInfo
	{
		get
		{
			return brainDataInfo;
		}
	}

	public ActionEventData ActionEventDataInfo
	{
		get
		{
			return actionEventDataInfo;
		}
	}

	public BrainEffect()
	{
	}

	public BrainEffect(BrainEffect copyFrom)
	{
		delay = copyFrom.Delay;
		timingType = copyFrom.TimingType;
		effectType = copyFrom.EffectType;
		audioClipInfo = copyFrom.AudioClipInfo;
		textInfo = copyFrom.TextInfo;
		numberInfo = copyFrom.NumberInfo;
		floatInfo = copyFrom.FloatInfo;
		gameObjectInfo = copyFrom.GameObjectInfo;
		costumeDataInfo = copyFrom.CostumeDataInfo;
		spriteInfo = copyFrom.SpriteInfo;
		botVOInfoDataInfo = copyFrom.BotVOInfoDataInfo;
		worldItemDataInfo = copyFrom.WorldItemDataInfo;
		brainDataInfo = copyFrom.BrainDataInfo;
		actionEventDataInfo = copyFrom.ActionEventDataInfo;
		audioClipListInfo.Clear();
		audioClipListInfo.AddRange(copyFrom.audioClipListInfo);
		botVOInfoDataListInfo.Clear();
		botVOInfoDataListInfo.AddRange(copyFrom.BotVOInfoDataListInfo);
	}

	public void InternalSetEffectType(EffectTypes type)
	{
		effectType = type;
	}

	public void InternalSetDelay(float del)
	{
		delay = del;
	}

	public void InternalSetTimingType(TimingTypes t)
	{
		timingType = t;
	}

	public void InternalSetAudioClipInfo(AudioClip clip)
	{
		audioClipInfo = clip;
	}

	public void InternalSetTextInfo(string t)
	{
		textInfo = t;
	}

	public void InternalSetNumberInfo(int i)
	{
		numberInfo = i;
	}

	public void InternalSetFloatInfo(float f)
	{
		floatInfo = f;
	}

	public void InternalSetGameObjectInfo(GameObject g)
	{
		gameObjectInfo = g;
	}

	public void InternalSetCostumeDataInfo(BotCostumeData c)
	{
		costumeDataInfo = c;
	}

	public void InternalSetSpriteInfo(Sprite s)
	{
		spriteInfo = s;
	}

	public void InternalSetBotVOInfoDataInfo(BotVOInfoData b)
	{
		botVOInfoDataInfo = b;
	}

	public void InternalSetWorldItemDataInfo(WorldItemData d)
	{
		worldItemDataInfo = d;
	}

	public void InternalSetBrainDataInfo(BrainData d)
	{
		brainDataInfo = d;
	}

	public void InternalSetActionEventDataInfo(ActionEventData a)
	{
		actionEventDataInfo = a;
	}

	public float GetDuration()
	{
		float result = 0f;
		if (effectType == EffectTypes.MoveAlongPath)
		{
			if (Application.isPlaying && BotUniqueElementManager._instanceNoCreate != null)
			{
				BotPath pathByName = BotUniqueElementManager._instanceNoCreate.GetPathByName(textInfo);
				result = pathByName.timeToCompletePath;
			}
		}
		else if (effectType == EffectTypes.MoveToPosition || effectType == EffectTypes.MoveToWaypoint)
		{
			result = floatInfo;
		}
		else if (effectType == EffectTypes.PlayVO)
		{
			if (audioClipInfo != null)
			{
				result = audioClipInfo.length;
			}
		}
		else if (effectType == EffectTypes.ChangeParent)
		{
			ChangeParentTypes changeParentTypes = (ChangeParentTypes)numberInfo;
			if (changeParentTypes == ChangeParentTypes.TweenLocalPosAndRotToZero || changeParentTypes == ChangeParentTypes.TweenLocalPosToZero || changeParentTypes == ChangeParentTypes.TweenLocalRotToZero)
			{
				result = floatInfo;
			}
		}
		return result;
	}

	public override string ToString()
	{
		return EffectType.ToString() + ": " + Delay + "sec delay";
	}
}
