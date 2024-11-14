using UnityEngine;

public class MonoBehaviourPrefabSpawner : BasePrefabSpawner
{
	public const string PREFAB_SPAWNER_GAMEOBJECT_NAME_APPEND = "-PrefabSpawner";

	public MonoBehaviourPrefabIniter prefabIniter;

	private MonoBehaviour lastSpawnedPrefab;

	public MonoBehaviour LastSpawnedPrefab
	{
		get
		{
			return lastSpawnedPrefab;
		}
	}

	protected void Awake()
	{
		SpawnPrefab();
		prefabIniter.Init(LastSpawnedPrefab);
	}

	protected void Start()
	{
		Object.Destroy(base.gameObject);
	}

	public override GameObject SpawnPrefab()
	{
		MonoBehaviour monoBehaviour = Object.Instantiate(prefabIniter.GetPrefab(), base.transform.position, base.transform.rotation) as MonoBehaviour;
		lastSpawnedPrefab = monoBehaviour;
		if (lastSpawnedPrefab != null)
		{
			lastSpawnedPrefabGO = lastSpawnedPrefab.gameObject;
			SetupParenting();
		}
		else
		{
			lastSpawnedPrefabGO = null;
		}
		return lastSpawnedPrefabGO;
	}

	private void OnDrawGizmos()
	{
		if (Application.isPlaying || prefabIniter == null || prefabIniter.GetPrefab() == null)
		{
			return;
		}
		GameObject gameObject = prefabIniter.GetPrefab().gameObject;
		if (gameObject == null)
		{
			return;
		}
		Color yellow = Color.yellow;
		yellow.a = 0.7f;
		Gizmos.color = yellow;
		if (!(gameObject != null))
		{
			return;
		}
		MeshFilter[] componentsInChildren = gameObject.gameObject.GetComponentsInChildren<MeshFilter>(true);
		if (componentsInChildren.Length > 0)
		{
			foreach (MeshFilter meshFilter in componentsInChildren)
			{
				Vector3 position = base.transform.TransformPoint(gameObject.transform.InverseTransformPoint(meshFilter.transform.position));
				Quaternion rotation = base.transform.rotation * (meshFilter.transform.rotation * Quaternion.Inverse(gameObject.transform.rotation));
				Gizmos.DrawMesh(meshFilter.sharedMesh, position, rotation, meshFilter.transform.lossyScale);
			}
		}
	}
}
