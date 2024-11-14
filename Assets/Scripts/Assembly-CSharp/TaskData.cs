using System.Collections.Generic;
using PSC;
using UnityEngine;

[CreateAssetMenu]
public class TaskData : ScriptableObject
{
	public static float MINIMUM_SECS_OF_TASK_COMPLETE_GRAPHIC = 2f;

	[SerializeField]
	[HideInInspector]
	private string taskHeader = string.Empty;

	[SerializeField]
	[HideInInspector]
	private string shortTaskName = string.Empty;

	[SerializeField]
	[HideInInspector]
	private List<PageData> pages = new List<PageData>();

	[HideInInspector]
	[SerializeField]
	private float secsOfBlankBeforeShowing;

	[HideInInspector]
	[SerializeField]
	private float secsOfTaskCompleteScreenAtEnd;

	[HideInInspector]
	[SerializeField]
	private float secsOfBlankAfterCompleting;

	private string id;

	public string ID
	{
		get
		{
			return id;
		}
	}

	public string TaskHeader
	{
		get
		{
			return taskHeader;
		}
	}

	public string ShortTaskName
	{
		get
		{
			return shortTaskName;
		}
	}

	public List<PageData> Pages
	{
		get
		{
			return pages;
		}
	}

	public float SecsOfBlankBeforeShowing
	{
		get
		{
			return secsOfBlankBeforeShowing;
		}
	}

	public float SecsOfTaskCompleteScreenAtEnd
	{
		get
		{
			return secsOfTaskCompleteScreenAtEnd;
		}
	}

	public float SecsOfBlankAfterCompleting
	{
		get
		{
			return secsOfBlankAfterCompleting;
		}
	}

	public TaskData Clone()
	{
		TaskData taskData = Object.Instantiate(this);
		taskData.name = base.name;
		taskData.InitClone();
		taskData.Pages.AddRange(pages);
		return taskData;
	}

	public void InitClone()
	{
		pages = new List<PageData>();
		id = base.name;
	}

	private void OnEnable()
	{
		id = base.name;
	}

	public List<PageData> GetListOfPagesForActiveRoomLayout()
	{
		List<PageData> list = new List<PageData>();
		for (int i = 0; i < pages.Count; i++)
		{
			bool flag = true;
			if (Room.activeRoom != null)
			{
				flag = pages[i].ShouldAppearOnLayout(Room.activeRoom.configuration);
			}
			else if (pages[i].OnlyAppearOnCertainLayouts)
			{
				Debug.LogWarning("There is no Room script in this scene, but " + pages[i].name + " is set to only appear on certain layouts. The job will probably not work quite right.");
			}
			if (flag)
			{
				list.Add(pages[i]);
			}
		}
		return list;
	}

	public void EditorSetSecsOfBlankBeforeShowing(float secs)
	{
		secsOfBlankBeforeShowing = secs;
	}

	public void EditorSetSecsOfTaskCompleteScreenAtEnd(float secs)
	{
		secsOfTaskCompleteScreenAtEnd = secs;
	}

	public void EditorSetSecsOfBlankAfterCompleting(float secs)
	{
		secsOfBlankAfterCompleting = secs;
	}

	public void EditorSetTaskHeader(string t)
	{
		taskHeader = t;
	}

	public void EditorSetShortName(string t)
	{
		shortTaskName = t;
	}

	public virtual TaskData GetTaskData()
	{
		return this;
	}
}
