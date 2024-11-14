using System.Collections.Generic;
using UnityEngine;

public class BotUniqueElementManager : MonoBehaviour
{
	private static BotUniqueElementManager _instance;

	private Dictionary<string, UniqueObject> uniqueObjects = new Dictionary<string, UniqueObject>();

	private Dictionary<string, BotPath> botPaths = new Dictionary<string, BotPath>();

	public static BotUniqueElementManager _instanceNoCreate
	{
		get
		{
			return _instance;
		}
	}

	public static BotUniqueElementManager Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = Object.FindObjectOfType(typeof(BotUniqueElementManager)) as BotUniqueElementManager;
				if (_instance == null)
				{
					_instance = new GameObject("_BotUniqueElementManager").AddComponent<BotUniqueElementManager>();
				}
			}
			return _instance;
		}
	}

	public Dictionary<string, UniqueObject> UniqueObjects
	{
		get
		{
			return uniqueObjects;
		}
	}

	public Dictionary<string, BotPath> BotPaths
	{
		get
		{
			return botPaths;
		}
	}

	public BotPath GetPathByName(string n)
	{
		if (botPaths.ContainsKey(n))
		{
			return botPaths[n];
		}
		Debug.LogError("Couldn't find BotPath with the name '" + n + "'");
		return null;
	}

	public BotPathWaypoint GetWaypointOfPath(string pathName, int waypointIndex)
	{
		BotPath pathByName = GetPathByName(pathName);
		if (waypointIndex >= 0 && waypointIndex < pathByName.Waypoints.Length)
		{
			return pathByName.Waypoints[waypointIndex];
		}
		if (-waypointIndex <= pathByName.Waypoints.Length && waypointIndex < 0)
		{
			return pathByName.Waypoints[pathByName.Waypoints.Length + waypointIndex];
		}
		Debug.LogError("Waypoint index '" + waypointIndex + "' is out of bounds for path '" + pathName + "'");
		return null;
	}

	public UniqueObject GetObjectByName(string n)
	{
		if (uniqueObjects.ContainsKey(n))
		{
			return uniqueObjects[n];
		}
		Debug.LogError("Couldn't find UniqueObject with the name '" + n + "'");
		return null;
	}

	public UniqueObject GetObjectByNameNoErrorIfNull(string n)
	{
		if (uniqueObjects.ContainsKey(n))
		{
			return uniqueObjects[n];
		}
		return null;
	}

	public void RegisterPath(BotPath path)
	{
		if (!botPaths.ContainsKey(path.gameObject.name))
		{
			botPaths[path.gameObject.name] = path;
		}
		else
		{
			Debug.LogWarning("BotPath with name '" + path.gameObject.name + "' tried to register itself more than once.");
		}
	}

	public void RegisterObject(UniqueObject obj)
	{
		if (!uniqueObjects.ContainsKey(obj.gameObject.name))
		{
			uniqueObjects[obj.gameObject.name] = obj;
		}
		else
		{
			Debug.LogWarning("UniqueObject with name '" + obj.gameObject.name + "' tried to register itself more than once.");
		}
	}

	public void UnregisterObject(UniqueObject obj)
	{
		if (uniqueObjects.ContainsKey(obj.gameObject.name))
		{
			uniqueObjects.Remove(obj.gameObject.name);
		}
		else
		{
			Debug.LogWarning("Tried to remove UniqueObject '" + obj.gameObject.name + "' but it was already removed.");
		}
	}
}
