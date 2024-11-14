using OwlchemyVR;
using UnityEngine;

public class PistonPrefabIniter : MonoBehaviourPrefabIniter
{
	[SerializeField]
	private PistonController prefab;

	[SerializeField]
	private Mesh pistonMesh;

	[SerializeField]
	private WorldItemData pistonWorldItemData;

	public override MonoBehaviour GetPrefab()
	{
		return prefab;
	}

	public override void Init(MonoBehaviour spawnedPrefab)
	{
		(spawnedPrefab as PistonController).Init(pistonMesh, pistonWorldItemData);
	}
}
