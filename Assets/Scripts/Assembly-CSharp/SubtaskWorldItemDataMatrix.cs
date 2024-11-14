using System.Collections.Generic;
using OwlchemyVR;
using UnityEngine;

[CreateAssetMenu(fileName = "SubtaskWithWorldItemMatrix", menuName = "Subtask with WorldItem Matrix")]
public class SubtaskWorldItemDataMatrix : ScriptableObject
{
	[SerializeField]
	private SubtaskData subtaskToRandomize;

	[SerializeField]
	[Tooltip("List of random possible items to choose from.  If only one entry, will only choose that entry.  Cannoy be empty.")]
	private WorldItemWithIconData[] worldItemsWithIcons;

	[SerializeField]
	[Tooltip("Positive and Negative WorldEvent conditions for subtasks.  Leave empty if this subtask is not randomized.")]
	private SubtaskPositiveNegativeIndexPairs[] indexPairs;

	private int previousIndex;

	private SubtaskData reusableInstance;

	private WorldItemWithIconData currentWorldItemWithIconData;

	public SubtaskData SubtaskToRandomize
	{
		get
		{
			return subtaskToRandomize;
		}
	}

	public WorldItemWithIconData CurrentWorldItemWithIconData
	{
		get
		{
			return currentWorldItemWithIconData;
		}
	}

	private List<WorldItemWithIconData> Excludes(List<WorldItemData> wid_a, WorldItemWithIconData[] wid_b)
	{
		List<WorldItemWithIconData> list = new List<WorldItemWithIconData>();
		for (int i = 0; i < wid_b.Length; i++)
		{
			if (!wid_a.Contains(wid_b[i].WorldItemData))
			{
				list.Add(wid_b[i]);
			}
		}
		return list;
	}

	public SubtaskData GetRandomizedSubtask(List<WorldItemData> lastUsedItems)
	{
		if (reusableInstance == null)
		{
			reusableInstance = Object.Instantiate(subtaskToRandomize);
		}
		WorldItemWithIconData worldItemWithIconData = null;
		if (worldItemsWithIcons.Length == 1)
		{
			worldItemWithIconData = worldItemsWithIcons[0];
		}
		else
		{
			if (worldItemsWithIcons.Length <= 1)
			{
				Debug.LogError("List of WorldItemWithIconData cannot be empty!");
				return null;
			}
			if (worldItemsWithIcons.Length <= lastUsedItems.Count)
			{
				lastUsedItems.Clear();
			}
			List<WorldItemWithIconData> list = Excludes(lastUsedItems, worldItemsWithIcons);
			if (list.Count < 1)
			{
				list = new List<WorldItemWithIconData>(worldItemsWithIcons);
				Debug.LogError("Item reset happened! No items left...");
			}
			while (worldItemWithIconData == null)
			{
				int num = Random.Range(0, list.Count);
				if (num != previousIndex)
				{
					previousIndex = num;
					worldItemWithIconData = list[num];
					lastUsedItems.Add(worldItemWithIconData.WorldItemData);
				}
			}
		}
		currentWorldItemWithIconData = worldItemWithIconData;
		if (worldItemWithIconData.Icon != null)
		{
			if (reusableInstance.SubtaskIconLayoutType == SubtaskData.SubtaskIconLayoutTypes.SINGLEICON)
			{
				reusableInstance.LayoutDependantSpriteOne = worldItemWithIconData.Icon;
			}
			else if (reusableInstance.SubtaskIconLayoutType == SubtaskData.SubtaskIconLayoutTypes.DEFAULT)
			{
				reusableInstance.LayoutDependantSpriteOne = worldItemWithIconData.Icon;
			}
		}
		for (int i = 0; i < indexPairs.Length; i++)
		{
			reusableInstance.ActionEventConditions[indexPairs[i].positive].EditorSetWorldItem(worldItemWithIconData.WorldItemData, 1);
			reusableInstance.ActionEventConditions[indexPairs[i].negative].EditorSetWorldItem(worldItemWithIconData.WorldItemData, 1);
		}
		return reusableInstance;
	}

	public SubtaskData GetSubtaskReusingWorldItemData(WorldItemWithIconData worldItem)
	{
		if (reusableInstance == null)
		{
			reusableInstance = Object.Instantiate(subtaskToRandomize);
		}
		if (worldItem.Icon != null)
		{
			if (reusableInstance.SubtaskIconLayoutType == SubtaskData.SubtaskIconLayoutTypes.SINGLEICON)
			{
				reusableInstance.LayoutDependantSpriteOne = worldItem.Icon;
			}
			else if (reusableInstance.SubtaskIconLayoutType == SubtaskData.SubtaskIconLayoutTypes.DEFAULT)
			{
				reusableInstance.LayoutDependantSpriteOne = worldItem.Icon;
			}
		}
		for (int i = 0; i < indexPairs.Length; i++)
		{
			reusableInstance.ActionEventConditions[indexPairs[i].positive].EditorSetWorldItem(worldItem.WorldItemData, 1);
			reusableInstance.ActionEventConditions[indexPairs[i].negative].EditorSetWorldItem(worldItem.WorldItemData, 1);
		}
		return reusableInstance;
	}
}
