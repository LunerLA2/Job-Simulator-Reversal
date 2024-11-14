using System;
using System.Collections.Generic;
using OwlchemyVR;
using UnityEngine;

public class WorldItemTrackingManager : MonoBehaviour
{
	private Dictionary<WorldItemData, int> totalNumOfWorldItems = new Dictionary<WorldItemData, int>();

	public Action<WorldItemData, int> OnItemAdded;

	public Action<WorldItemData, int, bool> OnItemRemoved;

	private bool applicationIsQuitting;

	private static WorldItemTrackingManager _instance;

	public static WorldItemTrackingManager InstanceNoCreate
	{
		get
		{
			return _instance;
		}
	}

	public static WorldItemTrackingManager Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = UnityEngine.Object.FindObjectOfType(typeof(WorldItemTrackingManager)) as WorldItemTrackingManager;
				if (_instance == null)
				{
					_instance = new GameObject("_WorldItemTrackingManager").AddComponent<WorldItemTrackingManager>();
				}
			}
			return _instance;
		}
	}

	public void ItemAdded(WorldItemData worldItemData)
	{
		int num;
		if (totalNumOfWorldItems.ContainsKey(worldItemData))
		{
			num = totalNumOfWorldItems[worldItemData] + 1;
			totalNumOfWorldItems[worldItemData] = num;
		}
		else
		{
			num = 1;
			totalNumOfWorldItems.Add(worldItemData, num);
		}
		if (OnItemAdded != null)
		{
			OnItemAdded(worldItemData, num);
		}
	}

	public void ItemRemoved(WorldItemData worldItemData, bool isBeingSwitched = false, bool suppressEvents = false)
	{
		int num = 0;
		if (totalNumOfWorldItems.ContainsKey(worldItemData))
		{
			num = totalNumOfWorldItems[worldItemData];
			if (num > 0)
			{
				num--;
				totalNumOfWorldItems[worldItemData] = num;
			}
			else
			{
				Debug.LogWarning(string.Concat("Some how worldItemData:", worldItemData, ", was removed more times that it was added"));
			}
		}
		else
		{
			Debug.LogWarning(string.Concat("Tried to remove worldItemData:", worldItemData, ", when the list does not contain that item"));
		}
		if (!applicationIsQuitting && !suppressEvents && OnItemRemoved != null)
		{
			OnItemRemoved(worldItemData, num, isBeingSwitched);
		}
	}

	public void ClearDictionary()
	{
		totalNumOfWorldItems.Clear();
	}

	private void OnApplicationQuit()
	{
		applicationIsQuitting = true;
	}

	public int GetCurrentNumberOfItems(WorldItemData item)
	{
		if (totalNumOfWorldItems.ContainsKey(item))
		{
			return totalNumOfWorldItems[item];
		}
		return 0;
	}

	private void Awake()
	{
		if (_instance == null)
		{
			_instance = this;
			UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		}
		else if (_instance != this)
		{
			UnityEngine.Object.Destroy(base.gameObject);
			return;
		}
		Setup();
	}

	private void Setup()
	{
	}
}
