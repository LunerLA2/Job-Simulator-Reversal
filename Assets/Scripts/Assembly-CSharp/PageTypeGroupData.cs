using System.Collections.Generic;
using System.Linq;
using OwlchemyVR;
using UnityEngine;

[CreateAssetMenu(fileName = "PageTypeGroupData", menuName = "Page Type Group Data")]
public class PageTypeGroupData : ScriptableObject
{
	[SerializeField]
	private List<PageData> pages;

	private List<int> pageIndices;

	private int currentPageIndex;

	[SerializeField]
	private List<SubtaskWorldItemDataMatrix> subtasksWithConditionsToRandomize;

	[SerializeField]
	private bool reuseWorldItemDataForAllSubtasksOnPage;

	[SerializeField]
	private bool useDeckCount;

	[Tooltip("Referring the the pages not the subtasks")]
	[SerializeField]
	private List<int> numPagesInDeck;

	private PageNameItemMatchedPair currentPageItemNamePair;

	private PageData reusableInstance;

	public List<PageData> Pages
	{
		get
		{
			return pages;
		}
	}

	public List<SubtaskWorldItemDataMatrix> SubtasksWithConditionsToRandomize
	{
		get
		{
			return subtasksWithConditionsToRandomize;
		}
	}

	public PageNameItemMatchedPair CurrentPageItemNamePair
	{
		get
		{
			if (currentPageItemNamePair == null)
			{
				Debug.LogWarning("current page item name pair is null");
				return null;
			}
			return currentPageItemNamePair;
		}
	}

	public void Init()
	{
		currentPageIndex = 0;
		if (useDeckCount && numPagesInDeck.Count > 0 && pages.Count != numPagesInDeck.Count)
		{
			Debug.LogError("Page count and deck count do not match in page type group data");
		}
		if (useDeckCount && pages.Count == numPagesInDeck.Count)
		{
			pageIndices = new List<int>();
			for (int i = 0; i < pages.Count; i++)
			{
				for (int j = 0; j < numPagesInDeck[i]; j++)
				{
					pageIndices.Add(i);
				}
			}
		}
		else if (pages.Count == 1 && (pageIndices == null || pageIndices.Count < 1))
		{
			if (pageIndices == null)
			{
				pageIndices = new List<int>();
			}
			pageIndices.Add(0);
		}
		else
		{
			pageIndices = Enumerable.Range(0, pages.Count).ToList();
		}
		pageIndices.Shuffle();
	}

	public void ClearPageItemNamePairs()
	{
		currentPageItemNamePair = null;
	}

	public PageData GetRandomPage(List<WorldItemData> lastUsedItems, bool returnInstance = false)
	{
		if (pages.Count == 1 && subtasksWithConditionsToRandomize.Count == 0)
		{
			return pages[0];
		}
		PageData pageData = null;
		int index = pageIndices[currentPageIndex];
		pageData = pages[index];
		currentPageIndex++;
		if (currentPageIndex >= pageIndices.Count)
		{
			currentPageIndex = 0;
			pageIndices.Shuffle();
		}
		if (reusableInstance != null)
		{
			reusableInstance.Subtasks = new List<SubtaskData>(pageData.Subtasks);
		}
		SubtaskData subtaskData = null;
		WorldItemWithIconData worldItemWithIconData = null;
		int num = -1;
		for (int i = 0; i < subtasksWithConditionsToRandomize.Count; i++)
		{
			if (!pageData.Subtasks.Contains(subtasksWithConditionsToRandomize[i].SubtaskToRandomize))
			{
				continue;
			}
			if (reuseWorldItemDataForAllSubtasksOnPage && worldItemWithIconData != null)
			{
				subtaskData = GetSubtaskReusingWorldItemData(worldItemWithIconData, subtasksWithConditionsToRandomize[i]);
			}
			else
			{
				subtaskData = subtasksWithConditionsToRandomize[i].GetRandomizedSubtask(lastUsedItems);
				worldItemWithIconData = SubtasksWithConditionsToRandomize[i].CurrentWorldItemWithIconData;
			}
			num = pageData.Subtasks.IndexOf(subtasksWithConditionsToRandomize[i].SubtaskToRandomize);
			returnInstance = true;
			currentPageItemNamePair = new PageNameItemMatchedPair();
			currentPageItemNamePair.WorldItemWithIconData = worldItemWithIconData;
			currentPageItemNamePair.PageName = pageData.name;
			if (subtaskData != null && num >= 0 && num < pageData.Subtasks.Count)
			{
				if (reusableInstance == null)
				{
					reusableInstance = Object.Instantiate(pageData);
					reusableInstance.name = pageData.name;
				}
				reusableInstance.Subtasks[num] = subtaskData;
			}
			else
			{
				Debug.LogError("SUBTASK INDEX IS BORKED!");
			}
		}
		if (returnInstance)
		{
			reusableInstance.name = pageData.name;
			return reusableInstance;
		}
		return pageData;
	}

	private SubtaskData GetSubtaskReusingWorldItemData(WorldItemWithIconData worldItemWithIconData, SubtaskWorldItemDataMatrix subtaskMatrix)
	{
		return subtaskMatrix.GetSubtaskReusingWorldItemData(worldItemWithIconData);
	}
}
