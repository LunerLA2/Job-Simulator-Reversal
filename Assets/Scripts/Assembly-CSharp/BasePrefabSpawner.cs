using UnityEngine;

public class BasePrefabSpawner : MonoBehaviour
{
	protected GameObject lastSpawnedPrefabGO;

	public GameObject LastSpawnedPrefabGO
	{
		get
		{
			return lastSpawnedPrefabGO;
		}
	}

	public virtual GameObject SpawnPrefab()
	{
		return null;
	}

	protected void SetupParenting()
	{
		lastSpawnedPrefabGO.RemoveCloneFromName();
		lastSpawnedPrefabGO.transform.SetParent(base.transform.parent, true);
		lastSpawnedPrefabGO.transform.SetSiblingIndex(base.transform.GetSiblingIndex() + 1);
		lastSpawnedPrefabGO.transform.localScale = base.transform.localScale;
		if (!(base.transform.parent != null))
		{
			return;
		}
		BasePrefabSpawner basePrefabSpawner = lastSpawnedPrefabGO.GetComponent<BasePrefabSpawner>();
		if (!(basePrefabSpawner != null) || !(basePrefabSpawner.LastSpawnedPrefabGO != null))
		{
			return;
		}
		while (true)
		{
			BasePrefabSpawner component = basePrefabSpawner.LastSpawnedPrefabGO.GetComponent<BasePrefabSpawner>();
			if (component != null && component.LastSpawnedPrefabGO != null)
			{
				basePrefabSpawner = component;
				continue;
			}
			break;
		}
		basePrefabSpawner.LastSpawnedPrefabGO.transform.SetParent(base.transform.parent, true);
		basePrefabSpawner.LastSpawnedPrefabGO.transform.SetSiblingIndex(base.transform.GetSiblingIndex() + 1);
		lastSpawnedPrefabGO = basePrefabSpawner.LastSpawnedPrefabGO;
	}
}
