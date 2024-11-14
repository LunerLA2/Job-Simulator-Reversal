using UnityEngine;

public class CoffeeCupPrefabIniter : MonoBehaviourPrefabIniter
{
	[SerializeField]
	private CoffeeCup prefab;

	[SerializeField]
	private Material material;

	public override MonoBehaviour GetPrefab()
	{
		return prefab;
	}

	public override void Init(MonoBehaviour spawnedPrefab)
	{
		(spawnedPrefab as CoffeeCup).SetupCoffeeCup(material);
	}
}
