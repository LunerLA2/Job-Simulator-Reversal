using UnityEngine;

public class GameObjectPrefabSpawner : BasePrefabSpawner
{
	public const string PREFAB_SPAWNER_GAMEOBJECT_NAME_APPEND = "-PrefabSpawner";

	public bool spawnOnAwake = true;

	public bool spawnOnStart;

	public GameObject prefab;

	private void Awake()
	{
		if (spawnOnAwake)
		{
			spawnOnStart = false;
			SpawnPrefab();
		}
	}

	private void Start()
	{
		if (spawnOnStart)
		{
			SpawnPrefab();
		}
		if (spawnOnAwake || spawnOnStart)
		{
			Object.Destroy(base.gameObject);
		}
	}

	public void SpawnPrefabGO()
	{
		SpawnPrefab();
	}

	public override GameObject SpawnPrefab()
	{
		GameObject result = (lastSpawnedPrefabGO = Object.Instantiate(prefab, base.transform.position, base.transform.rotation) as GameObject);
		if (lastSpawnedPrefabGO != null)
		{
			SetupParenting();
		}
		return result;
	}

	private void OnDrawGizmos()
	{
		if (Application.isPlaying)
		{
			return;
		}
		Color yellow = Color.yellow;
		yellow.a = 0.7f;
		Gizmos.color = yellow;
		if (!(prefab != null))
		{
			return;
		}
		MeshFilter[] componentsInChildren = prefab.GetComponentsInChildren<MeshFilter>(true);
		if (componentsInChildren.Length > 0)
		{
			foreach (MeshFilter meshFilter in componentsInChildren)
			{
				Vector3 position = base.transform.TransformPoint(prefab.transform.InverseTransformPoint(meshFilter.transform.position));
				Quaternion rotation = base.transform.rotation * (meshFilter.transform.rotation * Quaternion.Inverse(prefab.transform.rotation));
				Gizmos.DrawMesh(meshFilter.sharedMesh, position, rotation, meshFilter.transform.lossyScale);
			}
		}
	}
}
