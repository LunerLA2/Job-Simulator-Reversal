using UnityEngine;

public class DirtyDishPrefabIniter : MonoBehaviourPrefabIniter
{
	[SerializeField]
	private DirtyDishController prefab;

	[SerializeField]
	private Material dirtMaterial;

	[SerializeField]
	private float maxAlphaCutoff = 1f;

	public override MonoBehaviour GetPrefab()
	{
		return prefab;
	}

	public override void Init(MonoBehaviour spawnedPrefab)
	{
		(spawnedPrefab as DirtyDishController).Setup(dirtMaterial, maxAlphaCutoff);
	}
}
