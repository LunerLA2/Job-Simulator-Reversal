using System.Collections.Generic;
using UnityEngine;

public class JobData : ScriptableObject
{
	[SerializeField]
	private BrainGroupData brainGroup;

	[SerializeField]
	private BrainGroupData brainGroupPSVROverride;

	[SerializeField]
	private float secsOfBlankBeforeStartingJob;

	[SerializeField]
	private float secsOfBlankAfterCompletingJob;

	[SerializeField]
	private string promptTextBetweenTasks = string.Empty;

	[SerializeField]
	private SubtaskData subtaskToAdvanceBetweenTasks;

	[SerializeField]
	private bool countsAsGameProgression = true;

	[SerializeField]
	private List<TaskData> tasks = new List<TaskData>();

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

	public float SecsOfBlankBeforeStartingJob
	{
		get
		{
			return secsOfBlankBeforeStartingJob;
		}
	}

	public float SecsOfBlankAfterCompletingJob
	{
		get
		{
			return secsOfBlankAfterCompletingJob;
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

	public bool CountsAsGameProgression
	{
		get
		{
			return countsAsGameProgression;
		}
	}

	public List<TaskData> Tasks
	{
		get
		{
			return tasks;
		}
	}

	public void SetBrainGroup(BrainGroupData b)
	{
		brainGroup = b;
	}

	public void EditorSetBrainGroup(BrainGroupData b)
	{
		brainGroup = b;
	}

	public void EditorSetBrainGroupPSVR(BrainGroupData b)
	{
		brainGroupPSVROverride = b;
	}

	public void EditorSetSecsOfBlankBeforeStartingJob(float s)
	{
		secsOfBlankBeforeStartingJob = s;
	}

	public void EditorSetSecsOfBlankAfterCompletingJob(float s)
	{
		secsOfBlankAfterCompletingJob = s;
	}

	public void EditorSetPromptTextBetweenTasks(string t)
	{
		promptTextBetweenTasks = t;
	}

	public void EditorSetSubtaskToAdvanceBetweenTasks(SubtaskData s)
	{
		subtaskToAdvanceBetweenTasks = s;
	}

	public void EditorSetCountsAsGameProgression(bool c)
	{
		countsAsGameProgression = c;
	}
}
