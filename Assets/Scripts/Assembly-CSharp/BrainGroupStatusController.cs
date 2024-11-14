using System;
using System.Collections.Generic;
using System.Text;
using OwlchemyVR;
using PSC;
using UnityEngine;

public class BrainGroupStatusController
{
	private const string KEY_SEPERATOR = ":";

	private StringBuilder sb = new StringBuilder();

	private Dictionary<string, BrainCauseListObject> quickLookupForCausesCompleting = new Dictionary<string, BrainCauseListObject>();

	private List<BrainStatusController> brainStatusControllers;

	public List<BrainStatusController> BrainStatusControllers
	{
		get
		{
			return brainStatusControllers;
		}
	}

	public BrainGroupStatusController(BrainGroupData data)
	{
		brainStatusControllers = new List<BrainStatusController>();
		for (int i = 0; i < data.BrainDatas.Length; i++)
		{
			BrainStatusController brainStatusController = new BrainStatusController(data.BrainDatas[i]);
			brainStatusControllers.Add(brainStatusController);
			brainStatusController.OnBrainControlledObjectAppeared = (Action<BrainData>)Delegate.Combine(brainStatusController.OnBrainControlledObjectAppeared, new Action<BrainData>(BrainControlledObjectAppeared));
			brainStatusController.OnBrainControlledObjectDisappeared = (Action<BrainData>)Delegate.Combine(brainStatusController.OnBrainControlledObjectDisappeared, new Action<BrainData>(BrainControlledObjectDisappeared));
			brainStatusController.OnGlobalCustomMessage = (Action<string>)Delegate.Combine(brainStatusController.OnGlobalCustomMessage, new Action<string>(CustomMessageHappenedAcrossAllBrains));
			brainStatusController.OnGlobalCustomMessageClear = (Action<string>)Delegate.Combine(brainStatusController.OnGlobalCustomMessageClear, new Action<string>(CustomMessageClearedAcrossAllBrains));
			brainStatusController.OnLocalCustomMessage = (Action<string, BrainData>)Delegate.Combine(brainStatusController.OnLocalCustomMessage, new Action<string, BrainData>(CustomMessageHappenedOnOneBrain));
			brainStatusController.OnLocalCustomMessageClear = (Action<string, BrainData>)Delegate.Combine(brainStatusController.OnLocalCustomMessageClear, new Action<string, BrainData>(CustomMessageClearedOnOneBrain));
			brainStatusController.OnLocalCustomCounterChangedSelf = (Action<string, int, BrainData>)Delegate.Combine(brainStatusController.OnLocalCustomCounterChangedSelf, new Action<string, int, BrainData>(RecheckCustomCountersAfterLocalChange));
			brainStatusController.OnLocalCustomCounterNeedsToChangeByAmount = (Action<string, int, BrainData>)Delegate.Combine(brainStatusController.OnLocalCustomCounterNeedsToChangeByAmount, new Action<string, int, BrainData>(ApplyCounterChangeToSpecificBrainAndRecheck));
			brainStatusController.OnLocalCustomCounterNeedsToSetToAmount = (Action<string, int, BrainData>)Delegate.Combine(brainStatusController.OnLocalCustomCounterNeedsToSetToAmount, new Action<string, int, BrainData>(ApplyCounterSetToSpecificBrainAndRecheck));
			brainStatusController.OnGlobalCustomCountersChangeByAmount = (Action<string, int>)Delegate.Combine(brainStatusController.OnGlobalCustomCountersChangeByAmount, new Action<string, int>(ApplyCounterChangeToAllBrainsAndRecheck));
			brainStatusController.OnGlobalCustomCountersSetToAmount = (Action<string, int>)Delegate.Combine(brainStatusController.OnGlobalCustomCountersSetToAmount, new Action<string, int>(ApplyCounterSetToAllBrainsAndRecheck));
		}
		BuildQuickLookupDictionary();
		AddEventsToJobBoardManager(JobBoardManager.instance);
	}

	private void BuildQuickLookupDictionary()
	{
		quickLookupForCausesCompleting.Clear();
		for (int i = 0; i < brainStatusControllers.Count; i++)
		{
			BrainStatusController brainStatusController = brainStatusControllers[i];
			for (int j = 0; j < brainStatusController.EntryStatusControllersList.Count; j++)
			{
				BrainEntryStatusController brainEntryStatusController = brainStatusController.EntryStatusControllersList[j];
				for (int k = 0; k < brainEntryStatusController.CauseStatusControllerList.Count; k++)
				{
					BrainCauseStatusController brainCauseStatusController = brainEntryStatusController.CauseStatusControllerList[k];
					if (!brainCauseStatusController.Cause.IsSetUpCorrectly())
					{
						Debug.LogError(brainStatusController.Brain.name + "; Entry " + j + "; Cause " + k + " is not set up properly.", brainStatusController.Brain);
						continue;
					}
					sb.Length = 0;
					sb.Append(brainCauseStatusController.Cause.CauseType.ToString());
					if (brainCauseStatusController.Cause.CauseType == BrainCause.CauseTypes.WasJobStarted || brainCauseStatusController.Cause.CauseType == BrainCause.CauseTypes.WasJobCompleted || brainCauseStatusController.Cause.CauseType == BrainCause.CauseTypes.EventJobStarted || brainCauseStatusController.Cause.CauseType == BrainCause.CauseTypes.EventJobCompleted)
					{
						sb.Append(":");
						sb.Append(brainCauseStatusController.Cause.JobData.name);
					}
					else if (brainCauseStatusController.Cause.CauseType == BrainCause.CauseTypes.WasTaskShown || brainCauseStatusController.Cause.CauseType == BrainCause.CauseTypes.WasTaskCompleted || brainCauseStatusController.Cause.CauseType == BrainCause.CauseTypes.IsTaskActive || brainCauseStatusController.Cause.CauseType == BrainCause.CauseTypes.EventTaskShown || brainCauseStatusController.Cause.CauseType == BrainCause.CauseTypes.EventTaskCompleted || brainCauseStatusController.Cause.CauseType == BrainCause.CauseTypes.WasTaskStarted || brainCauseStatusController.Cause.CauseType == BrainCause.CauseTypes.EventTaskStarted || brainCauseStatusController.Cause.CauseType == BrainCause.CauseTypes.EventTaskEnded || brainCauseStatusController.Cause.CauseType == BrainCause.CauseTypes.WasTaskEnded || brainCauseStatusController.Cause.CauseType == BrainCause.CauseTypes.EventTaskCompletedSuccess || brainCauseStatusController.Cause.CauseType == BrainCause.CauseTypes.EventTaskCompletedFailure)
					{
						sb.Append(":");
						sb.Append(brainCauseStatusController.Cause.TaskData.name);
					}
					else if (brainCauseStatusController.Cause.CauseType == BrainCause.CauseTypes.WasPageShown || brainCauseStatusController.Cause.CauseType == BrainCause.CauseTypes.WasPageCompleted || brainCauseStatusController.Cause.CauseType == BrainCause.CauseTypes.IsPageActive || brainCauseStatusController.Cause.CauseType == BrainCause.CauseTypes.EventPageShown || brainCauseStatusController.Cause.CauseType == BrainCause.CauseTypes.EventPageCompleted || brainCauseStatusController.Cause.CauseType == BrainCause.CauseTypes.WasPageStarted || brainCauseStatusController.Cause.CauseType == BrainCause.CauseTypes.WasPageEnded || brainCauseStatusController.Cause.CauseType == BrainCause.CauseTypes.EventPageStarted || brainCauseStatusController.Cause.CauseType == BrainCause.CauseTypes.EventPageEnded)
					{
						sb.Append(":");
						sb.Append(brainCauseStatusController.Cause.PageData.name);
					}
					else if (brainCauseStatusController.Cause.CauseType == BrainCause.CauseTypes.WasSubtaskCompleted || brainCauseStatusController.Cause.CauseType == BrainCause.CauseTypes.EventSubtaskCompleted)
					{
						sb.Append(":");
						sb.Append(brainCauseStatusController.Cause.SubtaskData.name);
					}
					else if (brainCauseStatusController.Cause.CauseType == BrainCause.CauseTypes.WasActionNoItem || brainCauseStatusController.Cause.CauseType == BrainCause.CauseTypes.WasItemAction || brainCauseStatusController.Cause.CauseType == BrainCause.CauseTypes.WasItemActionWithAmount || brainCauseStatusController.Cause.CauseType == BrainCause.CauseTypes.WasItemOntoItem || brainCauseStatusController.Cause.CauseType == BrainCause.CauseTypes.WasItemOntoItemWithAmount || brainCauseStatusController.Cause.CauseType == BrainCause.CauseTypes.EventActionNoItem || brainCauseStatusController.Cause.CauseType == BrainCause.CauseTypes.EventItemAction || brainCauseStatusController.Cause.CauseType == BrainCause.CauseTypes.EventItemActionWithAmount || brainCauseStatusController.Cause.CauseType == BrainCause.CauseTypes.EventItemOntoItem || brainCauseStatusController.Cause.CauseType == BrainCause.CauseTypes.EventItemOntoItemWithAmount)
					{
						sb.Append(":");
						sb.Append(brainCauseStatusController.Cause.ActionEventData.ActionEventName);
						if (brainCauseStatusController.Cause.ActionEventData.FormatType == ActionEventData.FormatTypes.SingleWorldItemData)
						{
							sb.Append(":");
							sb.Append(brainCauseStatusController.Cause.WorldItemData.ItemName);
						}
						else if (brainCauseStatusController.Cause.ActionEventData.FormatType == ActionEventData.FormatTypes.WorldItemDataAppliedToWorldItemData)
						{
							sb.Append(":");
							sb.Append(brainCauseStatusController.Cause.WorldItemData.ItemName);
							sb.Append(":");
							sb.Append(brainCauseStatusController.Cause.AppliedToWorldItemData.ItemName);
						}
					}
					else if (brainCauseStatusController.Cause.CauseType == BrainCause.CauseTypes.IsBrainActive)
					{
						sb.Append(":");
						sb.Append(brainCauseStatusController.Cause.BrainData.name);
					}
					else if (brainCauseStatusController.Cause.CauseType == BrainCause.CauseTypes.CurrentVRPlatform || brainCauseStatusController.Cause.CauseType == BrainCause.CauseTypes.CurrentVRPlatformHardwareType)
					{
						sb.Append(":");
						sb.Append(brainCauseStatusController.Cause.NumberData.ToString());
					}
					else if (brainCauseStatusController.Cause.CauseType == BrainCause.CauseTypes.RoomLayoutIs)
					{
						sb.Append(":");
						sb.Append(brainCauseStatusController.Cause.LayoutData.name.ToString());
					}
					else if (brainCauseStatusController.Cause.CauseType == BrainCause.CauseTypes.WasCustomMessageReceived || brainCauseStatusController.Cause.CauseType == BrainCause.CauseTypes.EventCustomMessageReceived || brainCauseStatusController.Cause.CauseType == BrainCause.CauseTypes.ScriptedCause)
					{
						sb.Append(":");
						sb.Append(brainCauseStatusController.Cause.TextData);
					}
					else if (brainCauseStatusController.Cause.CauseType == BrainCause.CauseTypes.WasCustomCounterGTE || brainCauseStatusController.Cause.CauseType == BrainCause.CauseTypes.WasCustomCounterLTE || brainCauseStatusController.Cause.CauseType == BrainCause.CauseTypes.EventCustomCounterGTE || brainCauseStatusController.Cause.CauseType == BrainCause.CauseTypes.EventCustomCounterLTE || brainCauseStatusController.Cause.CauseType == BrainCause.CauseTypes.IsCustomCounterGTE || brainCauseStatusController.Cause.CauseType == BrainCause.CauseTypes.IsCustomCounterLTE || brainCauseStatusController.Cause.CauseType == BrainCause.CauseTypes.IsCustomCounterEQ)
					{
						sb.Append(":");
						sb.Append(brainCauseStatusController.Cause.TextData);
					}
					BrainCauseListObject value;
					if (quickLookupForCausesCompleting.TryGetValue(sb.ToString(), out value))
					{
						value.CauseList.Add(new BrainCauseListEntry(brainCauseStatusController, brainStatusController));
						continue;
					}
					value = new BrainCauseListObject();
					value.CauseList = new List<BrainCauseListEntry>();
					value.CauseList.Add(new BrainCauseListEntry(brainCauseStatusController, brainStatusController));
					quickLookupForCausesCompleting.Add(sb.ToString(), value);
				}
			}
		}
	}

	private void AddEventsToJobBoardManager(JobBoardManager manager)
	{
		manager.OnJobStarted = (Action<JobStatusController>)Delegate.Combine(manager.OnJobStarted, new Action<JobStatusController>(JobStarted));
		manager.OnJobComplete = (Action<JobStatusController>)Delegate.Combine(manager.OnJobComplete, new Action<JobStatusController>(JobComplete));
		manager.OnTaskStarted = (Action<TaskStatusController>)Delegate.Combine(manager.OnTaskStarted, new Action<TaskStatusController>(TaskStarted));
		manager.OnTaskShown = (Action<TaskStatusController>)Delegate.Combine(manager.OnTaskShown, new Action<TaskStatusController>(TaskShown));
		manager.OnTaskComplete = (Action<TaskStatusController>)Delegate.Combine(manager.OnTaskComplete, new Action<TaskStatusController>(TaskComplete));
		manager.OnTaskEnded = (Action<TaskStatusController>)Delegate.Combine(manager.OnTaskEnded, new Action<TaskStatusController>(TaskEnded));
		manager.OnPageStarted = (Action<PageStatusController>)Delegate.Combine(manager.OnPageStarted, new Action<PageStatusController>(PageStarted));
		manager.OnPageShown = (Action<PageStatusController>)Delegate.Combine(manager.OnPageShown, new Action<PageStatusController>(PageShown));
		manager.OnPageEnded = (Action<PageStatusController>)Delegate.Combine(manager.OnPageEnded, new Action<PageStatusController>(PageEnded));
		manager.OnPageComplete = (Action<PageStatusController>)Delegate.Combine(manager.OnPageComplete, new Action<PageStatusController>(PageComplete));
		manager.OnSubtaskComplete = (Action<SubtaskStatusController>)Delegate.Combine(manager.OnSubtaskComplete, new Action<SubtaskStatusController>(SubtaskComplete));
		manager.OnSandboxPhaseStarted = (Action)Delegate.Combine(manager.OnSandboxPhaseStarted, new Action(SandboxPhaseStarted));
		manager.OnStartedFromFirstTask = (Action)Delegate.Combine(manager.OnStartedFromFirstTask, new Action(StartedFromFirstTask));
		manager.OnDidntStartFromFirstTask = (Action)Delegate.Combine(manager.OnDidntStartFromFirstTask, new Action(DidntStartFromFirstTask));
	}

	public void Begin()
	{
		PlatformSettingEvent();
		HardwareSettingEvent();
		RoomLayoutSettingEvent();
		StartEvent();
	}

	private void StartEvent()
	{
		sb.Length = 0;
		sb.Append(BrainCause.CauseTypes.Start.ToString());
		if (quickLookupForCausesCompleting.ContainsKey(sb.ToString()))
		{
			BrainCauseListObject brainCauseListObject = quickLookupForCausesCompleting[sb.ToString()];
			brainCauseListObject.PositiveActionOccurred();
		}
	}

	private void RoomLayoutSettingEvent()
	{
		if (Room.activeRoom != null)
		{
			if (Room.activeRoom.configuration != null)
			{
				sb.Length = 0;
				sb.Append(BrainCause.CauseTypes.RoomLayoutIs.ToString());
				sb.Append(":");
				sb.Append(Room.activeRoom.configuration.name);
				if (quickLookupForCausesCompleting.ContainsKey(sb.ToString()))
				{
					BrainCauseListObject brainCauseListObject = quickLookupForCausesCompleting[sb.ToString()];
					brainCauseListObject.PositiveActionOccurred();
				}
			}
			else
			{
				Debug.LogWarning("PSC was run with 'None' selected for layout, so brains looking for specific layouts will never find anything.");
			}
		}
		else
		{
			Debug.LogError("Can't do RoomLayoutSettingEvent because there is no activeRoom. Scene probably needs Room script in it.");
		}
	}

	private void PlatformSettingEvent()
	{
		VRPlatformTypes currVRPlatformType = VRPlatform.GetCurrVRPlatformType();
		sb.Length = 0;
		sb.Append(BrainCause.CauseTypes.CurrentVRPlatform.ToString());
		sb.Append(":");
		StringBuilder stringBuilder = sb;
		int num = (int)currVRPlatformType;
		stringBuilder.Append(num.ToString());
		if (quickLookupForCausesCompleting.ContainsKey(sb.ToString()))
		{
			BrainCauseListObject brainCauseListObject = quickLookupForCausesCompleting[sb.ToString()];
			brainCauseListObject.PositiveActionOccurred();
		}
	}

	private void HardwareSettingEvent()
	{
		VRPlatformHardwareType currVRPlatformHardwareType = VRPlatform.GetCurrVRPlatformHardwareType();
		sb.Length = 0;
		sb.Append(BrainCause.CauseTypes.CurrentVRPlatformHardwareType.ToString());
		sb.Append(":");
		StringBuilder stringBuilder = sb;
		int num = (int)currVRPlatformHardwareType;
		stringBuilder.Append(num.ToString());
		if (quickLookupForCausesCompleting.ContainsKey(sb.ToString()))
		{
			BrainCauseListObject brainCauseListObject = quickLookupForCausesCompleting[sb.ToString()];
			brainCauseListObject.PositiveActionOccurred();
		}
	}

	private void SandboxPhaseStarted()
	{
		sb.Length = 0;
		sb.Append(BrainCause.CauseTypes.SandboxPhaseStart.ToString());
		if (quickLookupForCausesCompleting.ContainsKey(sb.ToString()))
		{
			BrainCauseListObject brainCauseListObject = quickLookupForCausesCompleting[sb.ToString()];
			brainCauseListObject.PositiveActionOccurred();
		}
	}

	private void StartedFromFirstTask()
	{
		sb.Length = 0;
		sb.Append(BrainCause.CauseTypes.StartedAtFirstTask.ToString());
		if (quickLookupForCausesCompleting.ContainsKey(sb.ToString()))
		{
			BrainCauseListObject brainCauseListObject = quickLookupForCausesCompleting[sb.ToString()];
			brainCauseListObject.PositiveActionOccurred();
		}
	}

	private void DidntStartFromFirstTask()
	{
		sb.Length = 0;
		sb.Append(BrainCause.CauseTypes.DidntStartAtFirstTask.ToString());
		if (quickLookupForCausesCompleting.ContainsKey(sb.ToString()))
		{
			BrainCauseListObject brainCauseListObject = quickLookupForCausesCompleting[sb.ToString()];
			brainCauseListObject.PositiveActionOccurred();
		}
	}

	private void JobStarted(JobStatusController job)
	{
		sb.Length = 0;
		sb.Append(BrainCause.CauseTypes.WasJobStarted.ToString());
		sb.Append(":");
		sb.Append(job.Data.name);
		if (quickLookupForCausesCompleting.ContainsKey(sb.ToString()))
		{
			BrainCauseListObject brainCauseListObject = quickLookupForCausesCompleting[sb.ToString()];
			brainCauseListObject.PositiveActionOccurred();
		}
		sb.Length = 0;
		sb.Append(BrainCause.CauseTypes.EventJobStarted.ToString());
		sb.Append(":");
		sb.Append(job.Data.name);
		if (quickLookupForCausesCompleting.ContainsKey(sb.ToString()))
		{
			BrainCauseListObject brainCauseListObject2 = quickLookupForCausesCompleting[sb.ToString()];
			brainCauseListObject2.InstantActionOccurred();
		}
	}

	private void JobComplete(JobStatusController job)
	{
		sb.Length = 0;
		sb.Append(BrainCause.CauseTypes.WasJobCompleted.ToString());
		sb.Append(":");
		sb.Append(job.Data.name);
		if (quickLookupForCausesCompleting.ContainsKey(sb.ToString()))
		{
			BrainCauseListObject brainCauseListObject = quickLookupForCausesCompleting[sb.ToString()];
			brainCauseListObject.PositiveActionOccurred();
		}
		sb.Length = 0;
		sb.Append(BrainCause.CauseTypes.EventJobCompleted.ToString());
		sb.Append(":");
		sb.Append(job.Data.name);
		if (quickLookupForCausesCompleting.ContainsKey(sb.ToString()))
		{
			BrainCauseListObject brainCauseListObject2 = quickLookupForCausesCompleting[sb.ToString()];
			brainCauseListObject2.InstantActionOccurred();
		}
	}

	private void TaskStarted(TaskStatusController task)
	{
		sb.Length = 0;
		sb.Append(BrainCause.CauseTypes.WasTaskStarted.ToString());
		sb.Append(":");
		sb.Append(task.Data.name);
		if (quickLookupForCausesCompleting.ContainsKey(sb.ToString()))
		{
			BrainCauseListObject brainCauseListObject = quickLookupForCausesCompleting[sb.ToString()];
			brainCauseListObject.PositiveActionOccurred();
		}
		sb.Length = 0;
		sb.Append(BrainCause.CauseTypes.EventTaskStarted.ToString());
		sb.Append(":");
		sb.Append(task.Data.name);
		if (quickLookupForCausesCompleting.ContainsKey(sb.ToString()))
		{
			BrainCauseListObject brainCauseListObject2 = quickLookupForCausesCompleting[sb.ToString()];
			brainCauseListObject2.InstantActionOccurred();
		}
		sb.Length = 0;
		sb.Append(BrainCause.CauseTypes.EventAnyTaskStarted.ToString());
		if (quickLookupForCausesCompleting.ContainsKey(sb.ToString()))
		{
			BrainCauseListObject brainCauseListObject3 = quickLookupForCausesCompleting[sb.ToString()];
			brainCauseListObject3.InstantActionOccurred();
		}
	}

	private void TaskShown(TaskStatusController task)
	{
		sb.Length = 0;
		sb.Append(BrainCause.CauseTypes.WasTaskShown.ToString());
		sb.Append(":");
		sb.Append(task.Data.name);
		if (quickLookupForCausesCompleting.ContainsKey(sb.ToString()))
		{
			BrainCauseListObject brainCauseListObject = quickLookupForCausesCompleting[sb.ToString()];
			brainCauseListObject.PositiveActionOccurred();
		}
		sb.Length = 0;
		sb.Append(BrainCause.CauseTypes.IsTaskActive.ToString());
		sb.Append(":");
		sb.Append(task.Data.name);
		if (quickLookupForCausesCompleting.ContainsKey(sb.ToString()))
		{
			BrainCauseListObject brainCauseListObject2 = quickLookupForCausesCompleting[sb.ToString()];
			brainCauseListObject2.PositiveActionOccurred();
		}
		sb.Length = 0;
		sb.Append(BrainCause.CauseTypes.EventTaskShown.ToString());
		sb.Append(":");
		sb.Append(task.Data.name);
		if (quickLookupForCausesCompleting.ContainsKey(sb.ToString()))
		{
			BrainCauseListObject brainCauseListObject3 = quickLookupForCausesCompleting[sb.ToString()];
			brainCauseListObject3.InstantActionOccurred();
		}
	}

	private void TaskComplete(TaskStatusController task)
	{
		sb.Length = 0;
		sb.Append(BrainCause.CauseTypes.WasTaskCompleted.ToString());
		sb.Append(":");
		sb.Append(task.Data.name);
		if (quickLookupForCausesCompleting.ContainsKey(sb.ToString()))
		{
			BrainCauseListObject brainCauseListObject = quickLookupForCausesCompleting[sb.ToString()];
			brainCauseListObject.PositiveActionOccurred();
		}
		sb.Length = 0;
		sb.Append(BrainCause.CauseTypes.IsTaskActive.ToString());
		sb.Append(":");
		sb.Append(task.Data.name);
		if (quickLookupForCausesCompleting.ContainsKey(sb.ToString()))
		{
			BrainCauseListObject brainCauseListObject2 = quickLookupForCausesCompleting[sb.ToString()];
			brainCauseListObject2.NegativeActionOccurred();
		}
		sb.Length = 0;
		sb.Append(BrainCause.CauseTypes.EventTaskCompleted.ToString());
		sb.Append(":");
		sb.Append(task.Data.name);
		if (quickLookupForCausesCompleting.ContainsKey(sb.ToString()))
		{
			BrainCauseListObject brainCauseListObject3 = quickLookupForCausesCompleting[sb.ToString()];
			brainCauseListObject3.InstantActionOccurred();
		}
		sb.Length = 0;
		sb.Append(BrainCause.CauseTypes.EventAnyTaskCompleted.ToString());
		if (quickLookupForCausesCompleting.ContainsKey(sb.ToString()))
		{
			BrainCauseListObject brainCauseListObject4 = quickLookupForCausesCompleting[sb.ToString()];
			brainCauseListObject4.InstantActionOccurred();
		}
		if (task.IsSuccess)
		{
			sb.Length = 0;
			sb.Append(BrainCause.CauseTypes.EventTaskCompletedSuccess.ToString());
			sb.Append(":");
			sb.Append(task.Data.name);
			if (quickLookupForCausesCompleting.ContainsKey(sb.ToString()))
			{
				BrainCauseListObject brainCauseListObject5 = quickLookupForCausesCompleting[sb.ToString()];
				brainCauseListObject5.InstantActionOccurred();
			}
			sb.Length = 0;
			sb.Append(BrainCause.CauseTypes.EventAnyTaskCompletedSuccess.ToString());
			if (quickLookupForCausesCompleting.ContainsKey(sb.ToString()))
			{
				BrainCauseListObject brainCauseListObject6 = quickLookupForCausesCompleting[sb.ToString()];
				brainCauseListObject6.InstantActionOccurred();
			}
		}
		else
		{
			sb.Length = 0;
			sb.Append(BrainCause.CauseTypes.EventTaskCompletedFailure.ToString());
			sb.Append(":");
			sb.Append(task.Data.name);
			if (quickLookupForCausesCompleting.ContainsKey(sb.ToString()))
			{
				BrainCauseListObject brainCauseListObject7 = quickLookupForCausesCompleting[sb.ToString()];
				brainCauseListObject7.InstantActionOccurred();
			}
			sb.Length = 0;
			sb.Append(BrainCause.CauseTypes.EventAnyTaskCompletedFailure.ToString());
			if (quickLookupForCausesCompleting.ContainsKey(sb.ToString()))
			{
				BrainCauseListObject brainCauseListObject8 = quickLookupForCausesCompleting[sb.ToString()];
				brainCauseListObject8.InstantActionOccurred();
			}
		}
	}

	private void TaskEnded(TaskStatusController task)
	{
		sb.Length = 0;
		sb.Append(BrainCause.CauseTypes.WasTaskEnded.ToString());
		sb.Append(":");
		sb.Append(task.Data.name);
		if (quickLookupForCausesCompleting.ContainsKey(sb.ToString()))
		{
			BrainCauseListObject brainCauseListObject = quickLookupForCausesCompleting[sb.ToString()];
			brainCauseListObject.PositiveActionOccurred();
		}
		sb.Length = 0;
		sb.Append(BrainCause.CauseTypes.EventTaskEnded.ToString());
		sb.Append(":");
		sb.Append(task.Data.name);
		if (quickLookupForCausesCompleting.ContainsKey(sb.ToString()))
		{
			BrainCauseListObject brainCauseListObject2 = quickLookupForCausesCompleting[sb.ToString()];
			brainCauseListObject2.InstantActionOccurred();
		}
		sb.Length = 0;
		sb.Append(BrainCause.CauseTypes.EventAnyTaskEnded.ToString());
		if (quickLookupForCausesCompleting.ContainsKey(sb.ToString()))
		{
			BrainCauseListObject brainCauseListObject3 = quickLookupForCausesCompleting[sb.ToString()];
			brainCauseListObject3.InstantActionOccurred();
		}
		if (task.IsSuccess)
		{
			sb.Length = 0;
			sb.Append(BrainCause.CauseTypes.EventAnyTaskEndedSuccess.ToString());
			if (quickLookupForCausesCompleting.ContainsKey(sb.ToString()))
			{
				BrainCauseListObject brainCauseListObject4 = quickLookupForCausesCompleting[sb.ToString()];
				brainCauseListObject4.InstantActionOccurred();
			}
		}
		else
		{
			sb.Length = 0;
			sb.Append(BrainCause.CauseTypes.EventAnyTaskEndedFailure.ToString());
			if (quickLookupForCausesCompleting.ContainsKey(sb.ToString()))
			{
				BrainCauseListObject brainCauseListObject5 = quickLookupForCausesCompleting[sb.ToString()];
				brainCauseListObject5.InstantActionOccurred();
			}
		}
	}

	private void PageStarted(PageStatusController page)
	{
		sb.Length = 0;
		sb.Append(BrainCause.CauseTypes.WasPageStarted.ToString());
		sb.Append(":");
		sb.Append(page.Data.name);
		if (quickLookupForCausesCompleting.ContainsKey(sb.ToString()))
		{
			BrainCauseListObject brainCauseListObject = quickLookupForCausesCompleting[sb.ToString()];
			brainCauseListObject.PositiveActionOccurred();
		}
		sb.Length = 0;
		sb.Append(BrainCause.CauseTypes.EventPageStarted.ToString());
		sb.Append(":");
		sb.Append(page.Data.name);
		if (quickLookupForCausesCompleting.ContainsKey(sb.ToString()))
		{
			BrainCauseListObject brainCauseListObject2 = quickLookupForCausesCompleting[sb.ToString()];
			brainCauseListObject2.InstantActionOccurred();
		}
		sb.Length = 0;
		sb.Append(BrainCause.CauseTypes.EventAnyPageStarted.ToString());
		if (quickLookupForCausesCompleting.ContainsKey(sb.ToString()))
		{
			BrainCauseListObject brainCauseListObject3 = quickLookupForCausesCompleting[sb.ToString()];
			brainCauseListObject3.InstantActionOccurred();
		}
		if (!page.IsCompleted)
		{
			sb.Length = 0;
			sb.Append(BrainCause.CauseTypes.EventAnyPageStartedIncomplete.ToString());
			if (quickLookupForCausesCompleting.ContainsKey(sb.ToString()))
			{
				BrainCauseListObject brainCauseListObject4 = quickLookupForCausesCompleting[sb.ToString()];
				brainCauseListObject4.InstantActionOccurred();
			}
		}
	}

	private void PageEnded(PageStatusController page)
	{
		sb.Length = 0;
		sb.Append(BrainCause.CauseTypes.WasPageEnded.ToString());
		sb.Append(":");
		sb.Append(page.Data.name);
		if (quickLookupForCausesCompleting.ContainsKey(sb.ToString()))
		{
			BrainCauseListObject brainCauseListObject = quickLookupForCausesCompleting[sb.ToString()];
			brainCauseListObject.PositiveActionOccurred();
		}
		sb.Length = 0;
		sb.Append(BrainCause.CauseTypes.EventPageEnded.ToString());
		sb.Append(":");
		sb.Append(page.Data.name);
		if (quickLookupForCausesCompleting.ContainsKey(sb.ToString()))
		{
			BrainCauseListObject brainCauseListObject2 = quickLookupForCausesCompleting[sb.ToString()];
			brainCauseListObject2.InstantActionOccurred();
		}
	}

	private void PageShown(PageStatusController page)
	{
		sb.Length = 0;
		sb.Append(BrainCause.CauseTypes.WasPageShown.ToString());
		sb.Append(":");
		sb.Append(page.Data.name);
		if (quickLookupForCausesCompleting.ContainsKey(sb.ToString()))
		{
			BrainCauseListObject brainCauseListObject = quickLookupForCausesCompleting[sb.ToString()];
			brainCauseListObject.PositiveActionOccurred();
		}
		sb.Length = 0;
		sb.Append(BrainCause.CauseTypes.IsPageActive.ToString());
		sb.Append(":");
		sb.Append(page.Data.name);
		if (quickLookupForCausesCompleting.ContainsKey(sb.ToString()))
		{
			BrainCauseListObject brainCauseListObject2 = quickLookupForCausesCompleting[sb.ToString()];
			brainCauseListObject2.PositiveActionOccurred();
		}
		sb.Length = 0;
		sb.Append(BrainCause.CauseTypes.EventPageShown.ToString());
		sb.Append(":");
		sb.Append(page.Data.name);
		if (quickLookupForCausesCompleting.ContainsKey(sb.ToString()))
		{
			BrainCauseListObject brainCauseListObject3 = quickLookupForCausesCompleting[sb.ToString()];
			brainCauseListObject3.InstantActionOccurred();
		}
		sb.Length = 0;
		sb.Append(BrainCause.CauseTypes.EventAnyPageShown.ToString());
		if (quickLookupForCausesCompleting.ContainsKey(sb.ToString()))
		{
			BrainCauseListObject brainCauseListObject4 = quickLookupForCausesCompleting[sb.ToString()];
			brainCauseListObject4.InstantActionOccurred();
		}
	}

	private void PageComplete(PageStatusController page)
	{
		sb.Length = 0;
		sb.Append(BrainCause.CauseTypes.WasPageCompleted.ToString());
		sb.Append(":");
		sb.Append(page.Data.name);
		if (quickLookupForCausesCompleting.ContainsKey(sb.ToString()))
		{
			BrainCauseListObject brainCauseListObject = quickLookupForCausesCompleting[sb.ToString()];
			brainCauseListObject.PositiveActionOccurred();
		}
		sb.Length = 0;
		sb.Append(BrainCause.CauseTypes.IsPageActive.ToString());
		sb.Append(":");
		sb.Append(page.Data.name);
		if (quickLookupForCausesCompleting.ContainsKey(sb.ToString()))
		{
			BrainCauseListObject brainCauseListObject2 = quickLookupForCausesCompleting[sb.ToString()];
			brainCauseListObject2.NegativeActionOccurred();
		}
		sb.Length = 0;
		sb.Append(BrainCause.CauseTypes.EventPageCompleted.ToString());
		sb.Append(":");
		sb.Append(page.Data.name);
		if (quickLookupForCausesCompleting.ContainsKey(sb.ToString()))
		{
			BrainCauseListObject brainCauseListObject3 = quickLookupForCausesCompleting[sb.ToString()];
			brainCauseListObject3.InstantActionOccurred();
		}
		sb.Length = 0;
		sb.Append(BrainCause.CauseTypes.EventAnyPageCompleted.ToString());
		if (quickLookupForCausesCompleting.ContainsKey(sb.ToString()))
		{
			BrainCauseListObject brainCauseListObject4 = quickLookupForCausesCompleting[sb.ToString()];
			brainCauseListObject4.InstantActionOccurred();
		}
	}

	private void SubtaskComplete(SubtaskStatusController subtask)
	{
		sb.Length = 0;
		sb.Append(BrainCause.CauseTypes.WasSubtaskCompleted.ToString());
		sb.Append(":");
		sb.Append(subtask.Data.name);
		if (quickLookupForCausesCompleting.ContainsKey(sb.ToString()))
		{
			BrainCauseListObject brainCauseListObject = quickLookupForCausesCompleting[sb.ToString()];
			brainCauseListObject.PositiveActionOccurred();
		}
		sb.Length = 0;
		sb.Append(BrainCause.CauseTypes.EventSubtaskCompleted.ToString());
		sb.Append(":");
		sb.Append(subtask.Data.name);
		if (quickLookupForCausesCompleting.ContainsKey(sb.ToString()))
		{
			BrainCauseListObject brainCauseListObject2 = quickLookupForCausesCompleting[sb.ToString()];
			brainCauseListObject2.InstantActionOccurred();
		}
		sb.Length = 0;
		sb.Append(BrainCause.CauseTypes.EventAnySubtaskCompleted.ToString());
		if (quickLookupForCausesCompleting.ContainsKey(sb.ToString()))
		{
			BrainCauseListObject brainCauseListObject3 = quickLookupForCausesCompleting[sb.ToString()];
			brainCauseListObject3.InstantActionOccurred();
		}
	}

	private void BrainControlledObjectAppeared(BrainData brain)
	{
		sb.Length = 0;
		sb.Append(BrainCause.CauseTypes.IsBrainActive.ToString());
		sb.Append(":");
		sb.Append(brain.name);
		if (quickLookupForCausesCompleting.ContainsKey(sb.ToString()))
		{
			BrainCauseListObject brainCauseListObject = quickLookupForCausesCompleting[sb.ToString()];
			brainCauseListObject.PositiveActionOccurred();
		}
	}

	private void BrainControlledObjectDisappeared(BrainData brain)
	{
		sb.Length = 0;
		sb.Append(BrainCause.CauseTypes.IsBrainActive.ToString());
		sb.Append(":");
		sb.Append(brain.name);
		if (quickLookupForCausesCompleting.ContainsKey(sb.ToString()))
		{
			BrainCauseListObject brainCauseListObject = quickLookupForCausesCompleting[sb.ToString()];
			brainCauseListObject.NegativeActionOccurred();
		}
	}

	public void ScriptedCauseHappened(string _name)
	{
		sb.Length = 0;
		sb.Append(BrainCause.CauseTypes.ScriptedCause.ToString());
		sb.Append(":");
		sb.Append(_name);
		if (quickLookupForCausesCompleting.ContainsKey(sb.ToString()))
		{
			BrainCauseListObject brainCauseListObject = quickLookupForCausesCompleting[sb.ToString()];
			brainCauseListObject.InstantActionOccurred();
		}
	}

	private void CustomMessageHappenedOnOneBrain(string msg, BrainData localTo)
	{
		sb.Length = 0;
		sb.Append(BrainCause.CauseTypes.WasCustomMessageReceived.ToString());
		sb.Append(":");
		sb.Append(msg);
		if (quickLookupForCausesCompleting.ContainsKey(sb.ToString()))
		{
			BrainCauseListObject brainCauseListObject = quickLookupForCausesCompleting[sb.ToString()];
			brainCauseListObject.PositiveActionOccurred(localTo);
		}
		sb.Length = 0;
		sb.Append(BrainCause.CauseTypes.EventCustomMessageReceived.ToString());
		sb.Append(":");
		sb.Append(msg);
		if (quickLookupForCausesCompleting.ContainsKey(sb.ToString()))
		{
			BrainCauseListObject brainCauseListObject2 = quickLookupForCausesCompleting[sb.ToString()];
			brainCauseListObject2.InstantActionOccurred(localTo);
		}
	}

	private void CustomMessageClearedOnOneBrain(string msg, BrainData localTo)
	{
		sb.Length = 0;
		sb.Append(BrainCause.CauseTypes.WasCustomMessageReceived.ToString());
		sb.Append(":");
		sb.Append(msg);
		if (quickLookupForCausesCompleting.ContainsKey(sb.ToString()))
		{
			BrainCauseListObject brainCauseListObject = quickLookupForCausesCompleting[sb.ToString()];
			brainCauseListObject.NegativeActionOccurred(localTo);
		}
	}

	private void CustomMessageHappenedAcrossAllBrains(string msg)
	{
		sb.Length = 0;
		sb.Append(BrainCause.CauseTypes.WasCustomMessageReceived.ToString());
		sb.Append(":");
		sb.Append(msg);
		if (quickLookupForCausesCompleting.ContainsKey(sb.ToString()))
		{
			BrainCauseListObject brainCauseListObject = quickLookupForCausesCompleting[sb.ToString()];
			brainCauseListObject.PositiveActionOccurred();
		}
		sb.Length = 0;
		sb.Append(BrainCause.CauseTypes.EventCustomMessageReceived.ToString());
		sb.Append(":");
		sb.Append(msg);
		if (quickLookupForCausesCompleting.ContainsKey(sb.ToString()))
		{
			BrainCauseListObject brainCauseListObject2 = quickLookupForCausesCompleting[sb.ToString()];
			brainCauseListObject2.InstantActionOccurred();
		}
	}

	private void CustomMessageClearedAcrossAllBrains(string msg)
	{
		sb.Length = 0;
		sb.Append(BrainCause.CauseTypes.WasCustomMessageReceived.ToString());
		sb.Append(":");
		sb.Append(msg);
		if (quickLookupForCausesCompleting.ContainsKey(sb.ToString()))
		{
			BrainCauseListObject brainCauseListObject = quickLookupForCausesCompleting[sb.ToString()];
			brainCauseListObject.NegativeActionOccurred();
		}
	}

	private void ApplyCounterChangeToAllBrainsAndRecheck(string name, int countChange)
	{
		int num = 0;
		for (int i = 0; i < brainStatusControllers.Count; i++)
		{
			num = brainStatusControllers[i].ManuallyChangeCounterAndGetNewValue(name, countChange);
			RecheckCustomCountersAfterLocalChange(name, num, brainStatusControllers[i].Brain);
		}
	}

	private void ApplyCounterSetToAllBrainsAndRecheck(string name, int setTo)
	{
		for (int i = 0; i < brainStatusControllers.Count; i++)
		{
			brainStatusControllers[i].ManuallySetCounter(name, setTo);
			RecheckCustomCountersAfterLocalChange(name, setTo, brainStatusControllers[i].Brain);
		}
	}

	private void ApplyCounterChangeToSpecificBrainAndRecheck(string name, int countChange, BrainData brain)
	{
		for (int i = 0; i < brainStatusControllers.Count; i++)
		{
			if (brainStatusControllers[i].Brain == brain)
			{
				int count = brainStatusControllers[i].ManuallyChangeCounterAndGetNewValue(name, countChange);
				RecheckCustomCountersAfterLocalChange(name, count, brainStatusControllers[i].Brain);
				break;
			}
		}
	}

	private void ApplyCounterSetToSpecificBrainAndRecheck(string name, int setTo, BrainData brain)
	{
		for (int i = 0; i < brainStatusControllers.Count; i++)
		{
			if (brainStatusControllers[i].Brain == brain)
			{
				brainStatusControllers[i].ManuallySetCounter(name, setTo);
				RecheckCustomCountersAfterLocalChange(name, setTo, brainStatusControllers[i].Brain);
				break;
			}
		}
	}

	private void RecheckCustomCountersAfterLocalChange(string name, int count, BrainData localTo)
	{
		sb.Length = 0;
		sb.Append(BrainCause.CauseTypes.WasCustomCounterGTE.ToString());
		sb.Append(":");
		sb.Append(name);
		if (quickLookupForCausesCompleting.ContainsKey(sb.ToString()))
		{
			BrainCauseListObject brainCauseListObject = quickLookupForCausesCompleting[sb.ToString()];
			brainCauseListObject.CounterActionOccurred(count, localTo);
		}
		sb.Length = 0;
		sb.Append(BrainCause.CauseTypes.WasCustomCounterLTE.ToString());
		sb.Append(":");
		sb.Append(name);
		if (quickLookupForCausesCompleting.ContainsKey(sb.ToString()))
		{
			BrainCauseListObject brainCauseListObject2 = quickLookupForCausesCompleting[sb.ToString()];
			brainCauseListObject2.CounterActionOccurred(count, localTo);
		}
		sb.Length = 0;
		sb.Append(BrainCause.CauseTypes.EventCustomCounterGTE.ToString());
		sb.Append(":");
		sb.Append(name);
		if (quickLookupForCausesCompleting.ContainsKey(sb.ToString()))
		{
			BrainCauseListObject brainCauseListObject3 = quickLookupForCausesCompleting[sb.ToString()];
			brainCauseListObject3.InstantCounterActionOccurred(count, localTo);
		}
		sb.Length = 0;
		sb.Append(BrainCause.CauseTypes.EventCustomCounterLTE.ToString());
		sb.Append(":");
		sb.Append(name);
		if (quickLookupForCausesCompleting.ContainsKey(sb.ToString()))
		{
			BrainCauseListObject brainCauseListObject4 = quickLookupForCausesCompleting[sb.ToString()];
			brainCauseListObject4.InstantCounterActionOccurred(count, localTo);
		}
		sb.Length = 0;
		sb.Append(BrainCause.CauseTypes.IsCustomCounterGTE.ToString());
		sb.Append(":");
		sb.Append(name);
		if (quickLookupForCausesCompleting.ContainsKey(sb.ToString()))
		{
			BrainCauseListObject brainCauseListObject5 = quickLookupForCausesCompleting[sb.ToString()];
			brainCauseListObject5.IsCounterActionOccurred(count, localTo);
		}
		sb.Length = 0;
		sb.Append(BrainCause.CauseTypes.IsCustomCounterLTE.ToString());
		sb.Append(":");
		sb.Append(name);
		if (quickLookupForCausesCompleting.ContainsKey(sb.ToString()))
		{
			BrainCauseListObject brainCauseListObject6 = quickLookupForCausesCompleting[sb.ToString()];
			brainCauseListObject6.IsCounterActionOccurred(count, localTo);
		}
		sb.Length = 0;
		sb.Append(BrainCause.CauseTypes.IsCustomCounterEQ.ToString());
		sb.Append(":");
		sb.Append(name);
		if (quickLookupForCausesCompleting.ContainsKey(sb.ToString()))
		{
			BrainCauseListObject brainCauseListObject7 = quickLookupForCausesCompleting[sb.ToString()];
			brainCauseListObject7.IsCounterActionOccurred(count, localTo);
		}
	}

	public void ActionOccurred(ActionEventData actionEventData)
	{
		sb.Length = 0;
		sb.Append(BrainCause.CauseTypes.WasActionNoItem.ToString());
		sb.Append(":");
		sb.Append(actionEventData.ActionEventName);
		if (quickLookupForCausesCompleting.ContainsKey(sb.ToString()))
		{
			BrainCauseListObject brainCauseListObject = quickLookupForCausesCompleting[sb.ToString()];
			brainCauseListObject.PositiveActionOccurred();
		}
		sb.Length = 0;
		sb.Append(BrainCause.CauseTypes.EventActionNoItem.ToString());
		sb.Append(":");
		sb.Append(actionEventData.ActionEventName);
		if (quickLookupForCausesCompleting.ContainsKey(sb.ToString()))
		{
			BrainCauseListObject brainCauseListObject2 = quickLookupForCausesCompleting[sb.ToString()];
			brainCauseListObject2.InstantActionOccurred();
		}
	}

	public void ItemActionOccurred(ActionEventData actionEventData, WorldItemData worldItemData)
	{
		sb.Length = 0;
		sb.Append(BrainCause.CauseTypes.WasItemAction.ToString());
		sb.Append(":");
		sb.Append(actionEventData.ActionEventName);
		sb.Append(":");
		sb.Append(worldItemData.ItemName);
		if (quickLookupForCausesCompleting.ContainsKey(sb.ToString()))
		{
			BrainCauseListObject brainCauseListObject = quickLookupForCausesCompleting[sb.ToString()];
			brainCauseListObject.PositiveActionOccurred();
		}
		sb.Length = 0;
		sb.Append(BrainCause.CauseTypes.EventItemAction.ToString());
		sb.Append(":");
		sb.Append(actionEventData.ActionEventName);
		sb.Append(":");
		sb.Append(worldItemData.ItemName);
		if (quickLookupForCausesCompleting.ContainsKey(sb.ToString()))
		{
			BrainCauseListObject brainCauseListObject2 = quickLookupForCausesCompleting[sb.ToString()];
			brainCauseListObject2.InstantActionOccurred();
		}
	}

	public void ItemActionOccurredWithAmount(ActionEventData actionEventData, WorldItemData worldItemData, float amount)
	{
		sb.Length = 0;
		sb.Append(BrainCause.CauseTypes.WasItemActionWithAmount.ToString());
		sb.Append(":");
		sb.Append(actionEventData.ActionEventName);
		sb.Append(":");
		sb.Append(worldItemData.ItemName);
		if (quickLookupForCausesCompleting.ContainsKey(sb.ToString()))
		{
			BrainCauseListObject brainCauseListObject = quickLookupForCausesCompleting[sb.ToString()];
			brainCauseListObject.PositiveActionOccurredWithAmount(amount);
		}
		sb.Length = 0;
		sb.Append(BrainCause.CauseTypes.EventItemActionWithAmount.ToString());
		sb.Append(":");
		sb.Append(actionEventData.ActionEventName);
		sb.Append(":");
		sb.Append(worldItemData.ItemName);
		if (quickLookupForCausesCompleting.ContainsKey(sb.ToString()))
		{
			BrainCauseListObject brainCauseListObject2 = quickLookupForCausesCompleting[sb.ToString()];
			brainCauseListObject2.InstantActionOccurredWithAmount(amount);
		}
	}

	public void ItemAppliedToItemActionOccurred(ActionEventData actionEventData, WorldItemData worldItemData, WorldItemData appliedToWorldItemData)
	{
		sb.Length = 0;
		sb.Append(BrainCause.CauseTypes.WasItemOntoItem.ToString());
		sb.Append(":");
		sb.Append(actionEventData.ActionEventName);
		sb.Append(":");
		sb.Append(worldItemData.ItemName);
		sb.Append(":");
		sb.Append(appliedToWorldItemData.ItemName);
		if (quickLookupForCausesCompleting.ContainsKey(sb.ToString()))
		{
			BrainCauseListObject brainCauseListObject = quickLookupForCausesCompleting[sb.ToString()];
			brainCauseListObject.PositiveActionOccurred();
		}
		sb.Length = 0;
		sb.Append(BrainCause.CauseTypes.EventItemOntoItem.ToString());
		sb.Append(":");
		sb.Append(actionEventData.ActionEventName);
		sb.Append(":");
		sb.Append(worldItemData.ItemName);
		sb.Append(":");
		sb.Append(appliedToWorldItemData.ItemName);
		if (quickLookupForCausesCompleting.ContainsKey(sb.ToString()))
		{
			BrainCauseListObject brainCauseListObject2 = quickLookupForCausesCompleting[sb.ToString()];
			brainCauseListObject2.InstantActionOccurred();
		}
	}

	public void ItemAppliedToItemActionOccurredWithAmount(ActionEventData actionEventData, WorldItemData worldItemData, WorldItemData appliedToWorldItemData, float amount)
	{
		sb.Length = 0;
		sb.Append(BrainCause.CauseTypes.WasItemOntoItemWithAmount.ToString());
		sb.Append(":");
		sb.Append(actionEventData.ActionEventName);
		sb.Append(":");
		sb.Append(worldItemData.ItemName);
		sb.Append(":");
		sb.Append(appliedToWorldItemData.ItemName);
		if (quickLookupForCausesCompleting.ContainsKey(sb.ToString()))
		{
			BrainCauseListObject brainCauseListObject = quickLookupForCausesCompleting[sb.ToString()];
			brainCauseListObject.PositiveActionOccurredWithAmount(amount);
		}
		sb.Length = 0;
		sb.Append(BrainCause.CauseTypes.EventItemOntoItemWithAmount.ToString());
		sb.Append(":");
		sb.Append(actionEventData.ActionEventName);
		sb.Append(":");
		sb.Append(worldItemData.ItemName);
		sb.Append(":");
		sb.Append(appliedToWorldItemData.ItemName);
		if (quickLookupForCausesCompleting.ContainsKey(sb.ToString()))
		{
			BrainCauseListObject brainCauseListObject2 = quickLookupForCausesCompleting[sb.ToString()];
			brainCauseListObject2.InstantActionOccurredWithAmount(amount);
		}
	}
}
