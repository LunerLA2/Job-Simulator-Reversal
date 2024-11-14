using UnityEngine;

public class CCube : MonoBehaviour
{
	[SerializeField]
	private GameObjectPrefabSpawner prefabSpawner;

	[SerializeField]
	private GameObject prefab;

	private void Start()
	{
		if (ExtraPrefs.ExtraProgress == 5)
		{
			prefabSpawner.prefab = prefab;
			prefabSpawner.SpawnPrefab();
		}
	}
}
