using UnityEngine;

public class CandyBarPrefabIniter : MonoBehaviourPrefabIniter
{
	[SerializeField]
	private CandyBar prefab;

	[SerializeField]
	private Mesh wholeMesh;

	[SerializeField]
	private Mesh oneBiteMesh;

	[SerializeField]
	private Mesh twoBiteMesh;

	public override MonoBehaviour GetPrefab()
	{
		return prefab;
	}

	public override void Init(MonoBehaviour spawnedPrefab)
	{
		(spawnedPrefab as CandyBar).SetupCandyBar(wholeMesh, oneBiteMesh, twoBiteMesh);
	}
}
