using System.Collections.Generic;
using OwlchemyVR;
using UnityEngine;

[CreateAssetMenu(fileName = "TaskWithRandomSubtaskData", menuName = "Task with Random Subtask Data")]
public class TaskWithRandomSubtaskData : ScriptableObject
{
	[SerializeField]
	private TaskData associatedTask;

	[SerializeField]
	private List<SubtaskWorldItemDataMatrix> subtasksToRandomize;

	private TaskData reusableInstance;

	[SerializeField]
	private string taskHeader;

	private List<PageNameItemMatchedPair> currentPageNameItemMatchedPairs;

	[SerializeField]
	private AudioClip missingAudioClip;

	[SerializeField]
	private List<PageWithAlternatePagesData> pagesWithAlternates;

	public TaskData AssociatedTask
	{
		get
		{
			return associatedTask;
		}
	}

	public List<PageNameItemMatchedPair> CurrentPageNameItemMatchedPairs
	{
		get
		{
			return currentPageNameItemMatchedPairs;
		}
	}

	public TaskData GetRandomizedTaskData(List<WorldItemData> lastUsedItems)
	{
		if (reusableInstance == null)
		{
			reusableInstance = Object.Instantiate(associatedTask);
			reusableInstance.name = associatedTask.name;
		}
		RandomizeSubtasks(lastUsedItems);
		return reusableInstance;
	}

	private void RandomizeSubtasks(List<WorldItemData> lastUsedItems)
	{
		currentPageNameItemMatchedPairs = new List<PageNameItemMatchedPair>();
		int num = 0;
		int num2 = 0;
		PageData[] requiredPages = JobBoardManager.instance.EndlessModeStatusController.Data.RequiredPages;
		for (int i = 0; i < requiredPages.Length; i++)
		{
			if (associatedTask.Pages.Contains(requiredPages[i]))
			{
				num2++;
			}
		}
		num = ((num2 != requiredPages.Length) ? associatedTask.Pages.Count : (associatedTask.Pages.Count - num2));
		for (int j = 0; j < num; j++)
		{
			PageData pageData = associatedTask.Pages[j];
			PageNameItemMatchedPair pageNameItemMatchedPair = new PageNameItemMatchedPair();
			string pageName = associatedTask.Pages[j].name;
			pageNameItemMatchedPair.PageName = pageName;
			if (pagesWithAlternates.Count > 0)
			{
				for (int k = 0; k < pagesWithAlternates.Count; k++)
				{
					if (pagesWithAlternates[k].BasePage == associatedTask.Pages[j])
					{
						pageData = pagesWithAlternates[k].GetRandomPage();
						if (pageData != associatedTask.Pages[j])
						{
							pageNameItemMatchedPair.WorldItemWithIconData = pagesWithAlternates[k].CurrentWorldItemWithIconData;
							pageNameItemMatchedPair.PageName = pageData.name;
						}
					}
				}
			}
			if (reusableInstance.Pages[j] != pageData)
			{
				reusableInstance.Pages[j] = pageData;
				reusableInstance.Pages[j].name = pageData.name;
			}
			for (int l = 0; l < subtasksToRandomize.Count; l++)
			{
				for (int m = 0; m < pageData.Subtasks.Count; m++)
				{
					if (pageData.Subtasks[m] == subtasksToRandomize[l].SubtaskToRandomize)
					{
						reusableInstance.Pages[j] = Object.Instantiate(pageData);
						reusableInstance.Pages[j].name = pageData.name;
						reusableInstance.Pages[j].Subtasks[m] = subtasksToRandomize[l].GetRandomizedSubtask(lastUsedItems);
						pageNameItemMatchedPair.WorldItemWithIconData = subtasksToRandomize[l].CurrentWorldItemWithIconData;
					}
				}
			}
			currentPageNameItemMatchedPairs.Add(pageNameItemMatchedPair);
		}
		reusableInstance.EditorSetTaskHeader(taskHeader);
	}
}
