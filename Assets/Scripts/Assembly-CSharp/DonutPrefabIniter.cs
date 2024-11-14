using UnityEngine;

public class DonutPrefabIniter : MonoBehaviourPrefabIniter
{
	[SerializeField]
	private Donut prefab;

	[SerializeField]
	private Mesh wholeMesh;

	[SerializeField]
	private Mesh bittenMesh;

	[SerializeField]
	private Material mat;

	[SerializeField]
	private bool startsBitten;

	public override void Init(MonoBehaviour spawnedPrefab)
	{
		Donut donut = spawnedPrefab as Donut;
		donut.SetupDonut(wholeMesh, bittenMesh, mat, startsBitten);
	}

	public override MonoBehaviour GetPrefab()
	{
		return prefab;
	}
}
