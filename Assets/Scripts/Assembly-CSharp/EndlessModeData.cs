using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu]
public class EndlessModeData : ScriptableObject
{
	public enum EndlessVORequestType
	{
		PerPage = 0,
		UpFront = 1,
		Random = 2
	}

	[SerializeField]
	private BrainGroupData brainGroup;

	[SerializeField]
	private BrainGroupData brainGroupPSVROverride;

	[SerializeField]
	private string promptTextBetweenTasks = string.Empty;

	[SerializeField]
	private SubtaskData subtaskToAdvanceBetweenTasks;

	[SerializeField]
	private float secsOfBlankBeforeStarting;

	[SerializeField]
	private float secsOfBlankAfterCompleting;

	[SerializeField]
	private float secsBeforeSkipItemAvailable = 5f;

	[SerializeField]
	private float secsOfBlankWhenPromoting = 5f;

	[SerializeField]
	private List<TaskData> goals = new List<TaskData>();

	[SerializeField]
	private BotCostumePieceGroup botBodyOptions;

	[SerializeField]
	private BotCostumePieceGroup botGlassesOptions;

	[SerializeField]
	private BotCostumePieceGroup botHatOptions;

	[SerializeField]
	private List<TaskWithRandomSubtaskData> randomizedTasks;

	[SerializeField]
	private List<PageTypeGroupData> pageTypes;

	[SerializeField]
	private EndlessVORequestType voRequestType;

	[SerializeField]
	private BotVOProfileData[] botVOProfiles;

	[SerializeField]
	private PageData[] requiredPages;

	[SerializeField]
	private TaskData baseTask;

	[SerializeField]
	private EndlessModeTaskRandomizationType randomizationType = EndlessModeTaskRandomizationType.UseTasksButRandomizeSubtaskConditions;

	[SerializeField]
	private int minimumNumberOfRandomPages = 2;

	[SerializeField]
	private int numTasksTrackedBeforeRepeat = 3;

	[SerializeField]
	private int numTasksTrackedBeforeRepeatWorldItems = 3;

	[SerializeField]
	private float percentChanceGenerateFromPages = 0.5f;

	[SerializeField]
	public TaskData goalForTesting;

	public BrainGroupData BrainGroup
	{
		get
		{
			return brainGroup;
		}
	}

	public BrainGroupData BrainGroupPSVR
	{
		get
		{
			return brainGroupPSVROverride;
		}
	}

	public BrainGroupData DesiredBrainGroup
	{
		get
		{
			return brainGroup;
		}
	}

	public string PromptTextBetweenTasks
	{
		get
		{
			return promptTextBetweenTasks;
		}
	}

	public SubtaskData SubtaskToAdvanceBetweenTasks
	{
		get
		{
			return subtaskToAdvanceBetweenTasks;
		}
	}

	public float SecsOfBlankBeforeStarting
	{
		get
		{
			return secsOfBlankBeforeStarting;
		}
	}

	public float SecsOfBlankAfterCompleting
	{
		get
		{
			return secsOfBlankAfterCompleting;
		}
	}

	public float SecsBeforeSkipItemAvailable
	{
		get
		{
			return secsBeforeSkipItemAvailable;
		}
	}

	public float SecsOfBlankWhenPromoting
	{
		get
		{
			return secsOfBlankWhenPromoting;
		}
	}

	public BotCostumePieceGroup BotBodyOptions
	{
		get
		{
			return botBodyOptions;
		}
	}

	public BotCostumePieceGroup BotGlassesOptions
	{
		get
		{
			return botGlassesOptions;
		}
	}

	public BotCostumePieceGroup BotHatOptions
	{
		get
		{
			return botHatOptions;
		}
	}

	public List<TaskWithRandomSubtaskData> RandomizedTasks
	{
		get
		{
			return randomizedTasks;
		}
	}

	public List<PageTypeGroupData> PageTypes
	{
		get
		{
			return pageTypes;
		}
	}

	public EndlessVORequestType VORequestType
	{
		get
		{
			return voRequestType;
		}
	}

	public BotVOProfileData[] BotVOProfiles
	{
		get
		{
			return botVOProfiles;
		}
	}

	public List<TaskData> Goals
	{
		get
		{
			return goals;
		}
	}

	public PageData[] RequiredPages
	{
		get
		{
			return requiredPages;
		}
	}

	public TaskData BaseTask
	{
		get
		{
			return baseTask;
		}
	}

	public EndlessModeTaskRandomizationType RandomizationType
	{
		get
		{
			return randomizationType;
		}
	}

	public int MinimumNumberOfRandomPages
	{
		get
		{
			return minimumNumberOfRandomPages;
		}
	}

	public int NumTasksTrackedBeforeRepeat
	{
		get
		{
			return numTasksTrackedBeforeRepeat;
		}
	}

	public int NumTasksTrackedBeforeRepeatWorldItems
	{
		get
		{
			return numTasksTrackedBeforeRepeatWorldItems;
		}
	}

	public float PercentChanceGenerateFromPages
	{
		get
		{
			return percentChanceGenerateFromPages;
		}
	}

	public TaskData GoalForTesting
	{
		get
		{
			return goalForTesting.GetTaskData();
		}
	}

	public void EditorSetBrainGroup(BrainGroupData b)
	{
		brainGroup = b;
	}

	public void EditorSetBrainGroupPSVR(BrainGroupData b)
	{
		brainGroupPSVROverride = b;
	}

	public void EditorSetSecsOfBlankBeforeStarting(float s)
	{
		secsOfBlankBeforeStarting = s;
	}

	public void EditorSetSecsOfBlankAfterCompleting(float s)
	{
		secsOfBlankAfterCompleting = s;
	}

	public void EditorSetSecsBeforeSkipItemAvailable(float s)
	{
		secsBeforeSkipItemAvailable = s;
	}

	public void EditorSetSecsOfBlankWhenPromoting(float s)
	{
		secsOfBlankWhenPromoting = s;
	}

	public void EditorSetNumTasksTrackedBeforeRepeat(int n)
	{
		numTasksTrackedBeforeRepeat = n;
	}

	public void EditorSetNumTasksTrackedBeforeRepeatWorldItems(int n)
	{
		numTasksTrackedBeforeRepeatWorldItems = n;
	}

	public void EditorSetPercentChanceGenerateFromPages(float n)
	{
		percentChanceGenerateFromPages = n;
	}

	public void EditorSetPromptTextBetweenTasks(string t)
	{
		promptTextBetweenTasks = t;
	}

	public void EditorSetSubtaskToAdvanceBetweenTasks(SubtaskData s)
	{
		subtaskToAdvanceBetweenTasks = s;
	}

	public void EditorSetBotBodyOptions(BotCostumePieceGroup b)
	{
		botBodyOptions = b;
	}

	public void EditorSetBotGlassesOptions(BotCostumePieceGroup b)
	{
		botGlassesOptions = b;
	}

	public void EditorSetBotHatOptions(BotCostumePieceGroup b)
	{
		botHatOptions = b;
	}

	public TaskData GetNewInstanceOfBaseTaskData()
	{
		TaskData taskData = UnityEngine.Object.Instantiate(baseTask);
		taskData.name = baseTask.name;
		return taskData;
	}

	public TaskData GetGoalAtIndex(int index)
	{
		return goals[index].GetTaskData();
	}
}
