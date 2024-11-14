using System.Collections.Generic;
using OwlchemyVR;
using UnityEngine;

public class GameEventsManager : MonoBehaviour
{
	private Dictionary<string, ActionEventData> allActionEvents = new Dictionary<string, ActionEventData>();

	private bool debugMessages;

	private static GameEventsManager _instance;

	public static GameEventsManager Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = Object.FindObjectOfType(typeof(GameEventsManager)) as GameEventsManager;
				if (_instance == null)
				{
					_instance = new GameObject("_GameEventsManager").AddComponent<GameEventsManager>();
				}
			}
			return _instance;
		}
	}

	public static GameEventsManager _instanceNoCreate
	{
		get
		{
			return _instance;
		}
	}

	public void ScriptedCauseOccurred(string causeID)
	{
		if (BotManager.Instance != null)
		{
			BotManager.Instance.ScriptedCauseOccurred(causeID);
		}
	}

	public void ActionOccurred(string actionEventName)
	{
		ActionEventData actionEventDataFromActionEventName = GetActionEventDataFromActionEventName(actionEventName);
		BotManager.Instance.ActionOccurred(actionEventDataFromActionEventName);
	}

	public void ItemActionOccurred(WorldItemData worldItemData, string actionEventName)
	{
		ActionEventData actionEventDataFromActionEventName = GetActionEventDataFromActionEventName(actionEventName);
		CheckIfValidActionEventWithWorldItemData(actionEventDataFromActionEventName, worldItemData);
		if (actionEventDataFromActionEventName.FormatType == ActionEventData.FormatTypes.SingleWorldItemData && !actionEventDataFromActionEventName.ContainsAmount)
		{
			if (JobBoardManager.instance != null)
			{
				JobBoardManager.instance.ItemActionOccurred(actionEventDataFromActionEventName, worldItemData);
			}
			if (BotManager.Instance != null)
			{
				BotManager.Instance.ItemActionOccurred(actionEventDataFromActionEventName, worldItemData);
			}
			if (debugMessages)
			{
				Debug.Log("Send event: ItemAction: WorldItemData:" + worldItemData.name + ", data:" + actionEventDataFromActionEventName.ActionEventName);
			}
		}
		else
		{
			Debug.LogWarning("ActionEventData does not match ItemActionOccurred:" + actionEventDataFromActionEventName.name);
		}
	}

	public void ItemActionOccurredWithAmount(WorldItemData worldItemData, string actionEventName, float amount)
	{
		ActionEventData actionEventDataFromActionEventName = GetActionEventDataFromActionEventName(actionEventName);
		CheckIfValidActionEventWithWorldItemData(actionEventDataFromActionEventName, worldItemData);
		if (actionEventDataFromActionEventName.FormatType == ActionEventData.FormatTypes.SingleWorldItemData && actionEventDataFromActionEventName.ContainsAmount)
		{
			if (JobBoardManager.instance != null)
			{
				JobBoardManager.instance.ItemActionOccurredWithAmount(actionEventDataFromActionEventName, worldItemData, amount);
			}
			if (BotManager.Instance != null)
			{
				BotManager.Instance.ItemActionOccurredWithAmount(actionEventDataFromActionEventName, worldItemData, amount);
			}
			if (debugMessages)
			{
				Debug.Log("Send event: ItemAction: WorldItemData:" + worldItemData.name + ", data:" + actionEventDataFromActionEventName.ActionEventName + ", amount:" + amount);
			}
		}
		else
		{
			Debug.LogWarning("ActionEventData does not match ItemActionOccurredWithAmount:" + actionEventDataFromActionEventName.name);
		}
	}

	public void ItemAppliedToItemActionOccurred(WorldItemData worldItemData, WorldItemData appliedToWorldItemData, string actionEventName)
	{
		ActionEventData actionEventDataFromActionEventName = GetActionEventDataFromActionEventName(actionEventName);
		CheckIfValidActionEventWithWorldItemData(actionEventDataFromActionEventName, appliedToWorldItemData);
		if (actionEventDataFromActionEventName.FormatType == ActionEventData.FormatTypes.WorldItemDataAppliedToWorldItemData && !actionEventDataFromActionEventName.ContainsAmount)
		{
			if (JobBoardManager.instance != null)
			{
				JobBoardManager.instance.ItemAppliedToItemActionOccurred(actionEventDataFromActionEventName, worldItemData, appliedToWorldItemData);
			}
			if (BotManager.Instance != null)
			{
				BotManager.Instance.ItemAppliedToItemActionOccurred(actionEventDataFromActionEventName, worldItemData, appliedToWorldItemData);
			}
			if (debugMessages)
			{
				Debug.Log("Send event: ItemAction: WorldItemData From:" + worldItemData.name + ", WorldItemData To:" + appliedToWorldItemData.name + ", data:" + actionEventDataFromActionEventName.ActionEventName);
			}
		}
		else
		{
			Debug.LogWarning("ActionEventData does not match ItemFromToActionOccurred:" + actionEventDataFromActionEventName.name);
		}
	}

	public void ItemAppliedToItemActionOccurredWithAmount(WorldItemData worldItemData, WorldItemData appliedToWorldItemData, string actionEventName, float amount)
	{
		ActionEventData actionEventDataFromActionEventName = GetActionEventDataFromActionEventName(actionEventName);
		CheckIfValidActionEventWithWorldItemData(actionEventDataFromActionEventName, appliedToWorldItemData);
		if (actionEventDataFromActionEventName.FormatType == ActionEventData.FormatTypes.WorldItemDataAppliedToWorldItemData && actionEventDataFromActionEventName.ContainsAmount)
		{
			if (JobBoardManager.instance != null)
			{
				JobBoardManager.instance.ItemAppliedToItemActionOccurredWithAmount(actionEventDataFromActionEventName, worldItemData, appliedToWorldItemData, amount);
			}
			if (BotManager.Instance != null)
			{
				BotManager.Instance.ItemAppliedToItemActionOccurredWithAmount(actionEventDataFromActionEventName, worldItemData, appliedToWorldItemData, amount);
			}
			if (debugMessages)
			{
				Debug.Log("Send event: ItemAction: WorldItemData From:" + worldItemData.name + ", WorldItemData To:" + appliedToWorldItemData.name + ", data:" + actionEventDataFromActionEventName.ActionEventName + ", amount:" + amount);
			}
		}
		else
		{
			Debug.LogWarning("ActionEventData does not match ItemFromToActionOccurred:" + actionEventDataFromActionEventName.name);
		}
	}

	private void CheckIfValidActionEventWithWorldItemData(ActionEventData actionEventData, WorldItemData worldItemData)
	{
	}

	private ActionEventData GetActionEventDataFromActionEventName(string actionEventName)
	{
		ActionEventData value;
		if (!allActionEvents.TryGetValue(actionEventName, out value))
		{
			Debug.LogWarning("Unsupported Action Event Name:" + actionEventName);
		}
		return value;
	}

	private void Awake()
	{
		if (_instance == null)
		{
			_instance = this;
			Object.DontDestroyOnLoad(base.gameObject);
		}
		else if (_instance != this)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		Setup();
	}

	private void Setup()
	{
		allActionEvents.Clear();
		ActionEventData[] array = LoadAllActionEvents();
		foreach (ActionEventData actionEventData in array)
		{
			allActionEvents.Add(actionEventData.name, actionEventData);
		}
	}

	public static ActionEventData[] LoadAllActionEvents()
	{
		return Resources.LoadAll<ActionEventData>("Data/ActionEvents");
	}
}
