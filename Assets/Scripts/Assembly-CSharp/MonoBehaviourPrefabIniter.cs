using UnityEngine;

[RequireComponent(typeof(MonoBehaviourPrefabSpawner))]
public abstract class MonoBehaviourPrefabIniter : MonoBehaviour
{
	public abstract MonoBehaviour GetPrefab();

	public abstract void Init(MonoBehaviour spawnedPrefab);
}
