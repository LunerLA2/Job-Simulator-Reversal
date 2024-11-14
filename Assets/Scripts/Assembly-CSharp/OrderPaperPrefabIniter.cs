using OwlchemyVR;
using UnityEngine;

public class OrderPaperPrefabIniter : MonoBehaviourPrefabIniter
{
	[SerializeField]
	private OrderPaper prefab;

	[SerializeField]
	private Mesh mesh;

	[SerializeField]
	private WorldItemData worldItemData;

	public override MonoBehaviour GetPrefab()
	{
		return prefab;
	}

	public override void Init(MonoBehaviour spawnedPrefab)
	{
		(spawnedPrefab as OrderPaper).SetupOrderPaper(mesh, worldItemData);
	}
}
