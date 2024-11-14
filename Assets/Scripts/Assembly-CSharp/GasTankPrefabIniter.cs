using UnityEngine;

public class GasTankPrefabIniter : MonoBehaviourPrefabIniter
{
	[SerializeField]
	private GasTankController masterPrefab;

	[SerializeField]
	private Mesh doorMesh;

	public override MonoBehaviour GetPrefab()
	{
		return masterPrefab;
	}

	public override void Init(MonoBehaviour spawnedPrefab)
	{
		(spawnedPrefab as GasTankController).Setup(doorMesh);
	}
}
