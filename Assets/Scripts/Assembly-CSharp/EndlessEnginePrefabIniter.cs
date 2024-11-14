using UnityEngine;

public class EndlessEnginePrefabIniter : MonoBehaviourPrefabIniter
{
	[SerializeField]
	private VehicleEngineHardware[] prefabs;

	[SerializeField]
	private GameObject[] goodBatteryPrefabs;

	[SerializeField]
	private GameObject[] goodPistonPrefabs;

	[SerializeField]
	private GameObject[] goodAirFilterPrefabs;

	[SerializeField]
	private GameObject[] badBatteryPrefabs;

	[SerializeField]
	private GameObject[] badPistonPrefabs;

	private GameObject batteryPrefab;

	private GameObject pistonPrefab;

	private GameObject airFilterPrefab;

	[HideInInspector]
	public bool useBadBattery;

	[HideInInspector]
	public bool useBadPistons;

	[HideInInspector]
	public bool useNoPistons;

	public override MonoBehaviour GetPrefab()
	{
		return GetRandomItemInArray(prefabs);
	}

	public override void Init(MonoBehaviour spawnedPrefab)
	{
		batteryPrefab = GetRandomItemInArray(goodBatteryPrefabs);
		pistonPrefab = GetRandomItemInArray(goodPistonPrefabs);
		airFilterPrefab = GetRandomItemInArray(goodAirFilterPrefabs);
		if (useBadBattery)
		{
			batteryPrefab = GetRandomItemInArray(badBatteryPrefabs);
		}
		if (useBadPistons)
		{
			pistonPrefab = GetRandomItemInArray(badPistonPrefabs);
		}
		if (useNoPistons)
		{
			pistonPrefab = null;
		}
		(spawnedPrefab as VehicleEngineHardware).Setup(batteryPrefab, pistonPrefab, airFilterPrefab);
	}

	private T GetRandomItemInArray<T>(T[] items)
	{
		if (items == null || items.Length == 0)
		{
			Debug.LogWarning("Passed empty or null array to GetRandomItemInArray. If you did not forget to set values, ignore this message.");
			return default(T);
		}
		return items[Random.Range(0, items.Length)];
	}
}
