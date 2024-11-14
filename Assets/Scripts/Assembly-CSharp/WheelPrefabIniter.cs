using OwlchemyVR;
using UnityEngine;

public class WheelPrefabIniter : MonoBehaviourPrefabIniter
{
	[SerializeField]
	private Wheel prefab;

	[SerializeField]
	private WorldItemData worldItemData;

	[SerializeField]
	private bool usesInflation = true;

	[SerializeField]
	private Mesh tireMesh;

	[SerializeField]
	private Material tireMaterial;

	[SerializeField]
	private Material hubcapMaterial;

	public override MonoBehaviour GetPrefab()
	{
		return prefab;
	}

	public override void Init(MonoBehaviour spawnedPrefab)
	{
		(spawnedPrefab as Wheel).Setup(worldItemData, usesInflation, tireMesh, tireMaterial, hubcapMaterial);
	}
}
