using OwlchemyVR;
using UnityEngine;

public class CashPrefabIniter : MonoBehaviourPrefabIniter
{
	[SerializeField]
	private CashController prefab;

	[SerializeField]
	private WorldItemData worldItemData;

	[SerializeField]
	private Material material;

	public override MonoBehaviour GetPrefab()
	{
		return prefab;
	}

	public override void Init(MonoBehaviour spawnedPrefab)
	{
		(spawnedPrefab as CashController).Setup(material, worldItemData);
	}
}
