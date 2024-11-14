using System.Collections.Generic;
using OwlchemyVR;
using UnityEngine;

public class BrainData : ScriptableObject
{
	public enum BrainTypes
	{
		Bot = 0,
		CustomSceneObject = 1,
		CustomSpawnableObject = 2
	}

	[SerializeField]
	private BrainTypes brainType;

	[SerializeField]
	private List<string> folderIDs = new List<string>();

	[SerializeField]
	private WorldItemData worldItemData;

	[SerializeField]
	private BotCostumeData costumeData;

	[SerializeField]
	private bool canBeHitWhileTweening = true;

	[SerializeField]
	private bool canBeHitWhileInTransit;

	[SerializeField]
	private bool canBeHitWhileParented;

	[SerializeField]
	private bool hideHoverFxWhileParented = true;

	[SerializeField]
	private bool isOptimizedBackgroundBot;

	[SerializeField]
	private string uniqueObjectIDOfControlledObject;

	[SerializeField]
	private bool presentInSceneByDefault = true;

	[SerializeField]
	private BrainControlledObject controlledPrefab;

	[SerializeField]
	private List<BrainEntry> entries = new List<BrainEntry>();

	[SerializeField]
	private List<BrainData> includeBrains = new List<BrainData>();

	public BrainTypes BrainType
	{
		get
		{
			return brainType;
		}
	}

	public List<string> FolderIDs
	{
		get
		{
			return folderIDs;
		}
	}

	public WorldItemData WorldItemData
	{
		get
		{
			return worldItemData;
		}
	}

	public BotCostumeData CostumeData
	{
		get
		{
			return costumeData;
		}
	}

	public bool CanBeHitWhileTweening
	{
		get
		{
			return canBeHitWhileTweening;
		}
	}

	public bool CanBeHitWhileInTransit
	{
		get
		{
			return canBeHitWhileInTransit;
		}
	}

	public bool CanBeHitWhileParented
	{
		get
		{
			return canBeHitWhileParented;
		}
	}

	public bool HideHoverFxWhileParented
	{
		get
		{
			return hideHoverFxWhileParented;
		}
	}

	public bool IsOptimizedBackgroundBot
	{
		get
		{
			return isOptimizedBackgroundBot;
		}
	}

	public string UniqueObjectIDOfControlledObject
	{
		get
		{
			return uniqueObjectIDOfControlledObject;
		}
	}

	public bool PresentInSceneByDefault
	{
		get
		{
			return presentInSceneByDefault;
		}
	}

	public BrainControlledObject ControlledPrefab
	{
		get
		{
			return controlledPrefab;
		}
	}

	public List<BrainEntry> Entries
	{
		get
		{
			return entries;
		}
	}

	public List<BrainData> IncludeBrains
	{
		get
		{
			return includeBrains;
		}
	}

	public void InternalSetWorldItemData(WorldItemData data)
	{
		worldItemData = data;
	}

	public void InternalSetCostumeData(BotCostumeData c)
	{
		costumeData = c;
	}

	public void InternalSetCanBeHitWhileTweening(bool h)
	{
		canBeHitWhileTweening = h;
	}

	public void InternalSetCanBeHitWhileInTransit(bool h)
	{
		canBeHitWhileInTransit = h;
	}

	public void InternalSetCanBeHitWhileParented(bool h)
	{
		canBeHitWhileParented = h;
	}

	public void InternalSetHideHoverFxWhileParented(bool h)
	{
		hideHoverFxWhileParented = h;
	}

	public void InternalSetIsOptimizedBackgroundBot(bool i)
	{
		isOptimizedBackgroundBot = i;
	}

	public void InternalSetPresentInSceneByDefault(bool p)
	{
		presentInSceneByDefault = p;
	}

	public void InternalSetUniqueObjectIDOfControlledObject(string id)
	{
		uniqueObjectIDOfControlledObject = id;
	}

	public void InternalSetControlledPrefab(BrainControlledObject p)
	{
		controlledPrefab = p;
	}

	public BrainEntry[] GetAllEntries()
	{
		List<BrainEntry> list = new List<BrainEntry>();
		for (int i = 0; i < entries.Count; i++)
		{
			list.Add(entries[i]);
		}
		for (int j = 0; j < includeBrains.Count; j++)
		{
			BrainEntry[] allEntries = includeBrains[j].GetAllEntries();
			for (int k = 0; k < allEntries.Length; k++)
			{
				list.Add(allEntries[k]);
			}
		}
		return list.ToArray();
	}

	public BotCostumeData GetCostumeData()
	{
		if (costumeData != null)
		{
			return costumeData;
		}
		for (int i = 0; i < includeBrains.Count; i++)
		{
			if (includeBrains[i].BrainType == brainType)
			{
				BotCostumeData botCostumeData = includeBrains[i].GetCostumeData();
				if (botCostumeData != null)
				{
					return botCostumeData;
				}
			}
		}
		return null;
	}

	public WorldItemData GetWorldItemData()
	{
		if (this.worldItemData != null)
		{
			return this.worldItemData;
		}
		for (int i = 0; i < includeBrains.Count; i++)
		{
			if (includeBrains[i].BrainType == brainType)
			{
				WorldItemData worldItemData = includeBrains[i].GetWorldItemData();
				if (worldItemData != null)
				{
					return worldItemData;
				}
			}
		}
		return null;
	}

	public void InternalSetBrainType(BrainTypes t)
	{
		brainType = t;
	}
}
