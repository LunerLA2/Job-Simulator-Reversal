using System;
using OwlchemyVR;
using PSC;
using UnityEngine;

[Serializable]
public class BrainCause
{
	public enum CauseTypes
	{
		WasJobStarted = 0,
		WasJobCompleted = 1,
		WasTaskStarted = 34,
		WasTaskShown = 2,
		WasTaskCompleted = 3,
		WasTaskEnded = 36,
		WasPageStarted = 38,
		WasPageShown = 10,
		WasPageCompleted = 11,
		WasPageEnded = 39,
		WasSubtaskCompleted = 4,
		WasActionNoItem = 5,
		WasItemAction = 6,
		WasItemActionWithAmount = 7,
		WasItemOntoItem = 8,
		WasItemOntoItemWithAmount = 9,
		Start = 12,
		IsTaskActive = 13,
		IsPageActive = 14,
		IsBrainActive = 15,
		WasCustomMessageReceived = 16,
		WasCustomCounterGTE = 17,
		WasCustomCounterLTE = 18,
		EventJobStarted = 19,
		EventJobCompleted = 20,
		EventTaskStarted = 35,
		EventTaskShown = 21,
		EventTaskCompleted = 22,
		EventTaskEnded = 37,
		EventPageStarted = 40,
		EventPageShown = 23,
		EventPageCompleted = 24,
		EventPageEnded = 41,
		EventSubtaskCompleted = 25,
		EventActionNoItem = 26,
		EventItemAction = 27,
		EventItemActionWithAmount = 28,
		EventItemOntoItem = 29,
		EventItemOntoItemWithAmount = 30,
		EventCustomMessageReceived = 31,
		EventCustomCounterGTE = 32,
		EventCustomCounterLTE = 33,
		ScriptedCause = 42,
		SandboxPhaseStart = 43,
		IsCustomCounterGTE = 44,
		IsCustomCounterLTE = 45,
		CurrentVRPlatform = 46,
		CurrentVRPlatformHardwareType = 50,
		RoomLayoutIs = 47,
		StartedAtFirstTask = 48,
		DidntStartAtFirstTask = 49,
		IsCustomCounterEQ = 51,
		EventAnyTaskStarted = 52,
		EventAnyTaskCompleted = 53,
		EventAnyTaskCompletedSuccess = 54,
		EventAnyTaskCompletedFailure = 55,
		EventAnyPageCompleted = 56,
		EventAnySubtaskCompleted = 57,
		EventAnyTaskEnded = 58,
		EventAnyTaskEndedSuccess = 59,
		EventAnyTaskEndedFailure = 60,
		EventAnyPageStarted = 61,
		EventAnyPageShown = 62,
		EventTaskCompletedSuccess = 63,
		EventTaskCompletedFailure = 64,
		EventAnyPageStartedIncomplete = 65
	}

	[SerializeField]
	private CauseTypes causeType = CauseTypes.WasTaskCompleted;

	[SerializeField]
	private JobData jobData;

	[SerializeField]
	private TaskData taskData;

	[SerializeField]
	private PageData pageData;

	[SerializeField]
	private SubtaskData subtaskData;

	[SerializeField]
	private ActionEventData actionEventData;

	[SerializeField]
	private WorldItemData worldItemData;

	[SerializeField]
	private WorldItemData appliedToWorldItemData;

	[SerializeField]
	private float amount;

	[SerializeField]
	private BrainData brainData;

	[SerializeField]
	private string textData = string.Empty;

	[SerializeField]
	private int numberData;

	[SerializeField]
	private LayoutConfiguration layoutData;

	public CauseTypes CauseType
	{
		get
		{
			return causeType;
		}
	}

	public JobData JobData
	{
		get
		{
			return jobData;
		}
	}

	public TaskData TaskData
	{
		get
		{
			return taskData;
		}
	}

	public PageData PageData
	{
		get
		{
			return pageData;
		}
	}

	public SubtaskData SubtaskData
	{
		get
		{
			return subtaskData;
		}
	}

	public ActionEventData ActionEventData
	{
		get
		{
			return actionEventData;
		}
	}

	public WorldItemData WorldItemData
	{
		get
		{
			return worldItemData;
		}
	}

	public WorldItemData AppliedToWorldItemData
	{
		get
		{
			return appliedToWorldItemData;
		}
	}

	public float Amount
	{
		get
		{
			return amount;
		}
	}

	public BrainData BrainData
	{
		get
		{
			return brainData;
		}
	}

	public string TextData
	{
		get
		{
			return textData;
		}
	}

	public int NumberData
	{
		get
		{
			return numberData;
		}
	}

	public LayoutConfiguration LayoutData
	{
		get
		{
			return layoutData;
		}
	}

	public BrainCause()
	{
	}

	public BrainCause(BrainCause copyFrom)
	{
		causeType = copyFrom.CauseType;
		jobData = copyFrom.JobData;
		taskData = copyFrom.TaskData;
		pageData = copyFrom.PageData;
		subtaskData = copyFrom.SubtaskData;
		actionEventData = copyFrom.ActionEventData;
		worldItemData = copyFrom.WorldItemData;
		appliedToWorldItemData = copyFrom.AppliedToWorldItemData;
		amount = copyFrom.Amount;
		brainData = copyFrom.BrainData;
		textData = copyFrom.TextData;
		numberData = copyFrom.NumberData;
		layoutData = copyFrom.layoutData;
	}

	public void InternalSetCauseType(CauseTypes type)
	{
		causeType = type;
	}

	public void InternalSetJobData(JobData job)
	{
		jobData = job;
	}

	public void InternalSetTaskData(TaskData data)
	{
		taskData = data;
	}

	public void InternalSetPageData(PageData data)
	{
		pageData = data;
	}

	public void InternalSetSubtaskData(SubtaskData data)
	{
		subtaskData = data;
	}

	public void InternalSetActionEventData(ActionEventData data)
	{
		actionEventData = data;
	}

	public void InternalSetWorldItemData(WorldItemData data)
	{
		worldItemData = data;
	}

	public void InternalSetAppliedToWorldItemData(WorldItemData data)
	{
		appliedToWorldItemData = data;
	}

	public void InternalSetAmount(float amt)
	{
		amount = amt;
	}

	public void InternalSetBrainData(BrainData data)
	{
		brainData = data;
	}

	public void InternalSetTextData(string t)
	{
		textData = t;
	}

	public void InternalSetNumberData(int n)
	{
		numberData = n;
	}

	public void InternalSetLayoutData(LayoutConfiguration layout)
	{
		layoutData = layout;
	}

	public bool IsSetUpCorrectly()
	{
		if (causeType == CauseTypes.IsBrainActive)
		{
			return brainData != null;
		}
		if (causeType == CauseTypes.WasJobCompleted || causeType == CauseTypes.WasJobStarted || causeType == CauseTypes.EventJobCompleted || causeType == CauseTypes.EventJobStarted)
		{
			return jobData != null;
		}
		if (causeType == CauseTypes.WasPageCompleted || causeType == CauseTypes.IsPageActive || causeType == CauseTypes.WasPageShown || causeType == CauseTypes.EventPageCompleted || causeType == CauseTypes.EventPageShown || causeType == CauseTypes.WasPageStarted || causeType == CauseTypes.WasPageEnded || causeType == CauseTypes.EventPageEnded || causeType == CauseTypes.EventPageStarted)
		{
			return pageData != null;
		}
		if (causeType == CauseTypes.WasTaskCompleted || causeType == CauseTypes.IsTaskActive || causeType == CauseTypes.WasTaskShown || causeType == CauseTypes.EventTaskCompleted || causeType == CauseTypes.EventTaskShown || causeType == CauseTypes.EventTaskStarted || causeType == CauseTypes.WasTaskStarted || causeType == CauseTypes.WasTaskEnded || causeType == CauseTypes.EventTaskEnded || causeType == CauseTypes.EventTaskCompletedSuccess || causeType == CauseTypes.EventTaskCompletedFailure)
		{
			return taskData != null;
		}
		if (causeType == CauseTypes.WasSubtaskCompleted || causeType == CauseTypes.EventSubtaskCompleted)
		{
			return subtaskData != null;
		}
		if (causeType == CauseTypes.WasActionNoItem || causeType == CauseTypes.EventActionNoItem)
		{
			return actionEventData != null;
		}
		if (causeType == CauseTypes.WasItemAction || causeType == CauseTypes.EventItemAction)
		{
			return actionEventData != null && worldItemData != null;
		}
		if (causeType == CauseTypes.WasItemActionWithAmount || causeType == CauseTypes.EventItemActionWithAmount)
		{
			return actionEventData != null && worldItemData != null;
		}
		if (causeType == CauseTypes.WasItemOntoItem || causeType == CauseTypes.EventItemOntoItem)
		{
			return actionEventData != null && worldItemData != null && appliedToWorldItemData != null;
		}
		if (causeType == CauseTypes.WasItemOntoItemWithAmount || causeType == CauseTypes.EventItemOntoItemWithAmount)
		{
			return actionEventData != null && worldItemData != null && appliedToWorldItemData != null;
		}
		if (causeType == CauseTypes.WasCustomMessageReceived || causeType == CauseTypes.EventCustomMessageReceived)
		{
			return textData != string.Empty;
		}
		if (causeType == CauseTypes.WasCustomCounterGTE || causeType == CauseTypes.EventCustomCounterGTE || causeType == CauseTypes.IsCustomCounterGTE || causeType == CauseTypes.WasCustomCounterLTE || causeType == CauseTypes.EventCustomCounterLTE || causeType == CauseTypes.IsCustomCounterLTE || causeType == CauseTypes.IsCustomCounterEQ)
		{
			return textData != string.Empty;
		}
		return true;
	}

	public override string ToString()
	{
		string text = causeType.ToString();
		if (causeType == CauseTypes.IsBrainActive && brainData != null)
		{
			text = text + ": " + brainData.name;
		}
		else if ((causeType == CauseTypes.WasJobCompleted || causeType == CauseTypes.WasJobStarted || causeType == CauseTypes.EventJobCompleted || causeType == CauseTypes.EventJobStarted) && jobData != null)
		{
			text = text + ": " + jobData.name;
		}
		else if ((causeType == CauseTypes.WasPageCompleted || causeType == CauseTypes.IsPageActive || causeType == CauseTypes.WasPageShown || causeType == CauseTypes.EventPageCompleted || causeType == CauseTypes.EventPageShown || causeType == CauseTypes.WasPageEnded || causeType == CauseTypes.WasPageStarted || causeType == CauseTypes.EventPageStarted || causeType == CauseTypes.EventPageEnded) && pageData != null)
		{
			text = text + ": " + pageData.name;
		}
		else if ((causeType == CauseTypes.WasTaskCompleted || causeType == CauseTypes.IsTaskActive || causeType == CauseTypes.WasTaskShown || causeType == CauseTypes.EventTaskCompleted || causeType == CauseTypes.EventTaskShown || causeType == CauseTypes.EventTaskStarted || causeType == CauseTypes.WasTaskStarted || causeType == CauseTypes.WasTaskEnded || causeType == CauseTypes.EventTaskEnded || causeType == CauseTypes.EventTaskCompletedSuccess || causeType == CauseTypes.EventTaskCompletedFailure) && taskData != null)
		{
			text = text + ": " + taskData.name;
		}
		else if ((causeType == CauseTypes.WasSubtaskCompleted || causeType == CauseTypes.EventSubtaskCompleted) && subtaskData != null)
		{
			text = text + ": " + subtaskData.name;
		}
		else if ((causeType == CauseTypes.WasActionNoItem || causeType == CauseTypes.EventActionNoItem) && actionEventData != null)
		{
			text = text + ": " + actionEventData.name;
		}
		else if ((causeType == CauseTypes.WasItemAction || causeType == CauseTypes.EventItemAction) && actionEventData != null && worldItemData != null)
		{
			string text2 = text;
			text = text2 + ": " + actionEventData.name + "->" + worldItemData.ItemName;
		}
		else if ((causeType == CauseTypes.WasItemActionWithAmount || causeType == CauseTypes.EventItemActionWithAmount) && actionEventData != null && worldItemData != null)
		{
			string text2 = text;
			text = text2 + ": " + actionEventData.name + "->" + worldItemData.ItemName + " (" + amount + ")";
		}
		else if ((causeType == CauseTypes.WasItemOntoItem || causeType == CauseTypes.EventItemOntoItem) && actionEventData != null && worldItemData != null && appliedToWorldItemData != null)
		{
			string text2 = text;
			text = text2 + ": " + actionEventData.name + "->" + worldItemData.ItemName + "->" + appliedToWorldItemData.ItemName;
		}
		else if ((causeType == CauseTypes.WasItemOntoItemWithAmount || causeType == CauseTypes.EventItemOntoItemWithAmount) && actionEventData != null && worldItemData != null && appliedToWorldItemData != null)
		{
			string text2 = text;
			text = text2 + ": " + actionEventData.name + "->" + worldItemData.ItemName + "->" + appliedToWorldItemData.ItemName + " (" + amount + ")";
		}
		else if (causeType == CauseTypes.WasCustomMessageReceived || causeType == CauseTypes.EventCustomMessageReceived)
		{
			text = text + ": " + textData;
		}
		else if (causeType == CauseTypes.WasCustomCounterGTE || causeType == CauseTypes.EventCustomCounterGTE || causeType == CauseTypes.WasCustomCounterLTE || causeType == CauseTypes.EventCustomCounterLTE || causeType == CauseTypes.IsCustomCounterLTE || causeType == CauseTypes.IsCustomCounterGTE || causeType == CauseTypes.IsCustomCounterEQ)
		{
			string text2 = text;
			text = text2 + ": " + textData + " (" + numberData + ")";
		}
		else if (causeType == CauseTypes.RoomLayoutIs)
		{
			if (layoutData != null)
			{
				text = text + ": " + layoutData.name;
			}
		}
		else if (causeType == CauseTypes.CurrentVRPlatform)
		{
			bool flag = false;
			VRPlatformTypes vRPlatformTypes = VRPlatformTypes.SteamVR;
			foreach (int value in Enum.GetValues(typeof(VRPlatformTypes)))
			{
				if (value == numberData)
				{
					flag = true;
					vRPlatformTypes = (VRPlatformTypes)value;
				}
			}
			text = ((!flag) ? (text + ": [UNKNOWN PLATFORM]") : (text + ": " + vRPlatformTypes));
		}
		else if (causeType == CauseTypes.CurrentVRPlatformHardwareType)
		{
			bool flag2 = false;
			VRPlatformHardwareType vRPlatformHardwareType = VRPlatformHardwareType.SteamVRCompatible;
			foreach (int value2 in Enum.GetValues(typeof(VRPlatformHardwareType)))
			{
				if (value2 == numberData)
				{
					flag2 = true;
					vRPlatformHardwareType = (VRPlatformHardwareType)value2;
				}
			}
			text = ((!flag2) ? (text + ": [UNKNOWN HARDWARE]") : (text + ": " + vRPlatformHardwareType));
		}
		return text;
	}
}
