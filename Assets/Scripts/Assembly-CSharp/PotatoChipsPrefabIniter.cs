using UnityEngine;

public class PotatoChipsPrefabIniter : MonoBehaviourPrefabIniter
{
	[SerializeField]
	private PotatoChips prefab;

	[SerializeField]
	private Material bagMaterial;

	public override void Init(MonoBehaviour spawnedPrefab)
	{
		(spawnedPrefab as PotatoChips).Setup(bagMaterial);
	}

	public override MonoBehaviour GetPrefab()
	{
		return prefab;
	}
}
