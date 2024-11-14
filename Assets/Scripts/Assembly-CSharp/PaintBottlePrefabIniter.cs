using OwlchemyVR;
using UnityEngine;

public class PaintBottlePrefabIniter : MonoBehaviourPrefabIniter
{
	[SerializeField]
	private PaintBottle prefab;

	[SerializeField]
	private Material material;

	[SerializeField]
	private WorldItemData fluidToDispense;

	public override MonoBehaviour GetPrefab()
	{
		return prefab;
	}

	public override void Init(MonoBehaviour spawnedPrefab)
	{
		(spawnedPrefab as PaintBottle).SetupPaintBottle(material, fluidToDispense);
	}
}
