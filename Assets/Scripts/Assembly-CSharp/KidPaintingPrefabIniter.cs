using OwlchemyVR;
using UnityEngine;

public class KidPaintingPrefabIniter : MonoBehaviourPrefabIniter
{
	[SerializeField]
	private KidPaintingController prefab;

	[SerializeField]
	private Material material;

	[SerializeField]
	private WorldItemData worldItemData;

	[SerializeField]
	private string uniqueID = string.Empty;

	public override MonoBehaviour GetPrefab()
	{
		return prefab;
	}

	public override void Init(MonoBehaviour spawnedPrefab)
	{
		(spawnedPrefab as KidPaintingController).Setup(material, worldItemData, uniqueID);
	}
}
