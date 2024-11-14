using UnityEngine;

public class NamePlatePrefabIniter : MonoBehaviourPrefabIniter
{
	[SerializeField]
	private NamePlate prefab;

	[SerializeField]
	private Material mainMaterial;

	[SerializeField]
	private int titleIndex;

	public override void Init(MonoBehaviour spawnedPrefab)
	{
		NamePlate namePlate = spawnedPrefab as NamePlate;
		namePlate.Setup(titleIndex, mainMaterial);
	}

	public override MonoBehaviour GetPrefab()
	{
		return prefab;
	}
}
