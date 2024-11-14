using System.Collections.Generic;
using OwlchemyVR;
using UnityEngine;

public class SpawnLimitManager : MonoBehaviour
{
	public const int DEFAULT_POOL_LIMIT = 10;

	private static SpawnLimitManager _instance;

	private Dictionary<WorldItemData, List<GameObject>> pools;

	private List<GameObject> generalPool;

	public static SpawnLimitManager Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = Object.FindObjectOfType(typeof(SpawnLimitManager)) as SpawnLimitManager;
				if (_instance == null)
				{
					_instance = new GameObject("_SpawnLimitManager").AddComponent<SpawnLimitManager>();
				}
			}
			return _instance;
		}
	}

	private void Awake()
	{
		if (_instance == null)
		{
			_instance = this;
			Object.DontDestroyOnLoad(base.gameObject);
		}
		else if (_instance != this)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		pools = new Dictionary<WorldItemData, List<GameObject>>();
		generalPool = new List<GameObject>();
	}

	public void AddObject(GameObject go)
	{
		WorldItem component = go.GetComponent<WorldItem>();
		if (component != null)
		{
			AddObject(go, component.Data);
		}
		else
		{
			AddObject(go, null);
		}
	}

	public void AddObject(GameObject go, WorldItemData worldItemData)
	{
		List<GameObject> value = null;
		if (worldItemData != null)
		{
			if (!pools.TryGetValue(worldItemData, out value))
			{
				value = new List<GameObject>();
				pools.Add(worldItemData, value);
			}
		}
		else
		{
			value = generalPool;
		}
		value.Add(go);
		while (value.Count > 10)
		{
			GameObject gameObject = value[0];
			if (gameObject != null)
			{
				Object.Destroy(gameObject);
			}
			value.RemoveAt(0);
		}
	}
}
