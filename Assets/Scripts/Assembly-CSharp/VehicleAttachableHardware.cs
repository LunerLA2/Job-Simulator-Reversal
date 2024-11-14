using UnityEngine;

public class VehicleAttachableHardware : VehicleHardware
{
	[SerializeField]
	private AttachablePoint attachablePoint;

	public void SpawnPrefab(GameObject hardwarePrefab)
	{
		GameObject gameObject = Object.Instantiate(hardwarePrefab);
		BasePrefabSpawner component = gameObject.GetComponent<BasePrefabSpawner>();
		if (component != null)
		{
			gameObject = component.LastSpawnedPrefabGO;
		}
		AttachableObject component2 = gameObject.GetComponent<AttachableObject>();
		component2.AttachTo(attachablePoint, -1, true, true);
	}
}
