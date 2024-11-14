using System.Collections;
using UnityEngine;

public class CorkboardController : MonoBehaviour
{
	[SerializeField]
	private GameObjectPrefabSpawner[] postitSpawners;

	[SerializeField]
	private AttachablePoint point;

	private bool hasSpawned;

	private void OnEnable()
	{
		if (!hasSpawned)
		{
			StartCoroutine(SpawnAsync());
		}
	}

	private IEnumerator SpawnAsync()
	{
		yield return null;
		yield return null;
		yield return null;
		for (int i = 0; i < postitSpawners.Length; i++)
		{
			GameObject go = postitSpawners[i].SpawnPrefab();
			AttachableObject obj = go.GetComponent<AttachableObject>();
			obj.AttachTo(point);
		}
		hasSpawned = true;
	}
}
