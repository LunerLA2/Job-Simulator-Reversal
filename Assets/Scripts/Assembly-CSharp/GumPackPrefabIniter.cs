using OwlchemyVR;
using UnityEngine;

public class GumPackPrefabIniter : MonoBehaviourPrefabIniter
{
	[SerializeField]
	private GumPack prefab;

	[SerializeField]
	private Material material;

	[SerializeField]
	private WorldItemData itemData;

	public override MonoBehaviour GetPrefab()
	{
		return prefab;
	}

	public override void Init(MonoBehaviour spawnedPrefab)
	{
		(spawnedPrefab as GumPack).Init(material, itemData);
	}
}
