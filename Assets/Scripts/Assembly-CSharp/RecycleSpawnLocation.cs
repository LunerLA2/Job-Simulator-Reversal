using System;
using UnityEngine;

[Serializable]
public class RecycleSpawnLocation
{
	public enum LocationSpawnTypes
	{
		DropAtPosition = 0,
		LookForAttachpoint = 1,
		HoverAtPosition = 2
	}

	[SerializeField]
	private string uniqueObjectID = string.Empty;

	[SerializeField]
	private LocationSpawnTypes spawnType;

	public string UniqueObjectID
	{
		get
		{
			return uniqueObjectID;
		}
	}

	public LocationSpawnTypes SpawnType
	{
		get
		{
			return spawnType;
		}
	}

	public void EditorSetUniqueObjectID(string i)
	{
		uniqueObjectID = i;
	}

	public void EditorSetSpawnType(LocationSpawnTypes t)
	{
		spawnType = t;
	}
}
