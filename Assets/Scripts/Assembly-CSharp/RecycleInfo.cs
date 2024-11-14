using System;
using System.Collections.Generic;
using OwlchemyVR;
using UnityEngine;

[Serializable]
public class RecycleInfo
{
	public enum RespawnPrefabSelectionMode
	{
		Random = 0,
		CycleInOrder = 1
	}

	public enum RespawnLocationSelectionMode
	{
		PrioritizeAsListed = 0,
		Random = 1
	}

	[SerializeField]
	private List<WorldItemData> worldItemsToMonitor = new List<WorldItemData>();

	[SerializeField]
	private int minimumQuantityToMaintainInScene = 1;

	[SerializeField]
	private bool alwaysMaintainQuantity = true;

	[SerializeField]
	private bool ignoreIfDestructionWasBeneficial;

	[SerializeField]
	private bool refillQuantityOnTaskOrPageStart;

	[SerializeField]
	private List<TaskData> maintainQuantityDuringTasks = new List<TaskData>();

	[SerializeField]
	private List<PageData> maintainQuantityDuringPages = new List<PageData>();

	[SerializeField]
	private RespawnPrefabSelectionMode prefabSelectionMode;

	[SerializeField]
	private List<GameObject> prefabsToSpawnAsReplacement = new List<GameObject>();

	[SerializeField]
	private RespawnLocationSelectionMode locationSelectionMode;

	[SerializeField]
	private List<RecycleSpawnLocation> possibleSpawnLocations = new List<RecycleSpawnLocation>();

	public List<WorldItemData> WorldItemsToMonitor
	{
		get
		{
			return worldItemsToMonitor;
		}
	}

	public int MinimumQuantityToMaintainInScene
	{
		get
		{
			return minimumQuantityToMaintainInScene;
		}
	}

	public bool AlwaysMaintainQuantity
	{
		get
		{
			return alwaysMaintainQuantity;
		}
	}

	public bool IgnoreIfDestructionWasBeneficial
	{
		get
		{
			return ignoreIfDestructionWasBeneficial;
		}
	}

	public bool RefillQuantityOnTaskOrPageStart
	{
		get
		{
			return refillQuantityOnTaskOrPageStart;
		}
	}

	public List<TaskData> MaintainQuantityDuringTasks
	{
		get
		{
			return maintainQuantityDuringTasks;
		}
	}

	public List<PageData> MaintainQuantityDuringPages
	{
		get
		{
			return maintainQuantityDuringPages;
		}
	}

	public List<string> MaintainQuantityDuringPagesStringNames
	{
		get
		{
			List<string> list = new List<string>();
			for (int i = 0; i < maintainQuantityDuringPages.Count; i++)
			{
				list.Add(maintainQuantityDuringPages[i].name);
			}
			return list;
		}
	}

	public RespawnPrefabSelectionMode PrefabSelectionMode
	{
		get
		{
			return prefabSelectionMode;
		}
	}

	public List<GameObject> PrefabsToSpawnAsReplacement
	{
		get
		{
			return prefabsToSpawnAsReplacement;
		}
	}

	public RespawnLocationSelectionMode LocationSelectionMode
	{
		get
		{
			return locationSelectionMode;
		}
	}

	public List<RecycleSpawnLocation> PossibleSpawnLocations
	{
		get
		{
			return possibleSpawnLocations;
		}
	}

	public void EditorSetMinimumQuantityToMaintainInScene(int m)
	{
		minimumQuantityToMaintainInScene = m;
	}

	public void EditorSetAlwaysMaintainQuantity(bool a)
	{
		alwaysMaintainQuantity = a;
	}

	public void EditorSetIgnoreIfDestructionWasBeneficial(bool i)
	{
		ignoreIfDestructionWasBeneficial = i;
	}

	public void EditorSetRefillQuantityOnTaskOrPageStart(bool r)
	{
		refillQuantityOnTaskOrPageStart = r;
	}

	public void EditorSetPrefabSelectionMode(RespawnPrefabSelectionMode m)
	{
		prefabSelectionMode = m;
	}

	public void EditorSetLocationSelectionMode(RespawnLocationSelectionMode m)
	{
		locationSelectionMode = m;
	}
}
