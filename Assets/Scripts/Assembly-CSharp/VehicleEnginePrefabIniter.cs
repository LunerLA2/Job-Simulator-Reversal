using UnityEngine;

public class VehicleEnginePrefabIniter : MonoBehaviourPrefabIniter
{
	[SerializeField]
	private VehicleEngineHardware prefab;

	[SerializeField]
	private GameObject batteryPrefab;

	[SerializeField]
	private GameObject pistonPrefab;

	[SerializeField]
	private GameObject airFilterPrefab;

	public override MonoBehaviour GetPrefab()
	{
		return prefab;
	}

	public override void Init(MonoBehaviour spawnedPrefab)
	{
		(spawnedPrefab as VehicleEngineHardware).Setup(batteryPrefab, pistonPrefab, airFilterPrefab);
	}
}
