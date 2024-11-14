using OwlchemyVR;
using UnityEngine;

public class IDCardPrefabIniter : MonoBehaviourPrefabIniter
{
	[SerializeField]
	private IDCardController prefab;

	[SerializeField]
	private Material material;

	[SerializeField]
	private WorldItemData worldItemData;

	public override MonoBehaviour GetPrefab()
	{
		return prefab;
	}

	public override void Init(MonoBehaviour spawnedPrefab)
	{
		(spawnedPrefab as IDCardController).Setup(material, worldItemData);
	}
}
