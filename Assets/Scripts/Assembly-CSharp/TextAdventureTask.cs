using UnityEngine;

[CreateAssetMenu(menuName = "Text Adventure Task", order = 999)]
public class TextAdventureTask : ScriptableObject
{
	[SerializeField]
	private string textBody;

	[SerializeField]
	private string[] options;

	[SerializeField]
	private string[] results;

	[SerializeField]
	private int[] goToTaskOnComplete;

	[SerializeField]
	private bool isStartOfNewTask;

	private bool hasShownCompleteTaskScreen;

	public string TextBody
	{
		get
		{
			return textBody;
		}
	}

	public string[] Options
	{
		get
		{
			return options;
		}
	}

	public string[] Results
	{
		get
		{
			return results;
		}
	}

	public int[] GoToTaskOnComplete
	{
		get
		{
			return goToTaskOnComplete;
		}
	}

	public bool IsStartOfNewTask
	{
		get
		{
			return isStartOfNewTask;
		}
	}

	public bool HasShownCompleteTaskScreen
	{
		get
		{
			return hasShownCompleteTaskScreen;
		}
	}

	public void SetHasShownCompleteTaskScreen(bool b)
	{
		hasShownCompleteTaskScreen = b;
	}
}
