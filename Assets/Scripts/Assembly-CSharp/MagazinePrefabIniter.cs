using OwlchemyVR;
using UnityEngine;

public class MagazinePrefabIniter : MonoBehaviourPrefabIniter
{
	[SerializeField]
	private MagazineController prefab;

	[SerializeField]
	private Mesh mesh;

	[SerializeField]
	private Material material;

	[SerializeField]
	private WorldItemData alternateItemData;

	public override MonoBehaviour GetPrefab()
	{
		return prefab;
	}

	public override void Init(MonoBehaviour spawnedPrefab)
	{
		(spawnedPrefab as MagazineController).Setup(mesh, material, alternateItemData);
	}
}
