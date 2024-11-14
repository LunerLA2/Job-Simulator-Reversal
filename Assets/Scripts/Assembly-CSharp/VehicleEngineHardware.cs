using UnityEngine;

public class VehicleEngineHardware : VehicleHardware
{
	private const string PFX_PATH = "EngineBlock/Mesh/EngineBlockMesh/PFX_TubeSmoke";

	[SerializeField]
	private AttachablePoint[] batteryAttachPoints;

	[SerializeField]
	private AttachablePoint[] pistonAttachablePoints;

	[SerializeField]
	private AttachablePoint[] airFilterAttachablePoints;

	private Transform enginePFX;

	public AttachablePoint[] BatteryAttachPoints
	{
		get
		{
			return batteryAttachPoints;
		}
	}

	public AttachablePoint[] PistonAttachablePoints
	{
		get
		{
			return pistonAttachablePoints;
		}
	}

	public AttachablePoint[] AirFilterAttachablePoints
	{
		get
		{
			return airFilterAttachablePoints;
		}
	}

	public void Setup(GameObject batteryPrefab, GameObject pistonPrefab, GameObject airFilterPrefab)
	{
		AttachablePoint[] array = batteryAttachPoints;
		foreach (AttachablePoint attachablePoint in array)
		{
			SpawnPrefab(batteryPrefab, attachablePoint);
		}
		AttachablePoint[] array2 = pistonAttachablePoints;
		foreach (AttachablePoint attachablePoint2 in array2)
		{
			if (pistonPrefab != null)
			{
				SpawnPrefab(pistonPrefab, attachablePoint2);
			}
		}
		AttachablePoint[] array3 = AirFilterAttachablePoints;
		foreach (AttachablePoint attachablePoint3 in array3)
		{
			SpawnPrefab(airFilterPrefab, attachablePoint3);
		}
		enginePFX = base.transform.Find("EngineBlock/Mesh/EngineBlockMesh/PFX_TubeSmoke");
	}

	private void SpawnPrefab(GameObject prefab, AttachablePoint attachablePoint)
	{
		GameObject gameObject = Object.Instantiate(prefab);
		BasePrefabSpawner component = gameObject.GetComponent<BasePrefabSpawner>();
		if (component != null)
		{
			gameObject = component.LastSpawnedPrefabGO;
		}
		AttachableObject component2 = gameObject.GetComponent<AttachableObject>();
		component2.AttachTo(attachablePoint, -1, true, true);
	}

	public Transform GetEnginePFXContainer()
	{
		if (enginePFX != null)
		{
			return enginePFX;
		}
		Debug.LogWarning("GetEnginePFXContainer was called on a VehicleEngineHardware without EnginePFX being set.");
		return null;
	}
}
