using UnityEngine;

public class StamperControllerPrefabIniter : MonoBehaviourPrefabIniter
{
	[SerializeField]
	private StamperController prefab;

	[SerializeField]
	private GameObject stampPrefab;

	[SerializeField]
	private Material meshMaterial;

	[SerializeField]
	private StampWorldItemSwitch[] worldItemSwitches;

	public override MonoBehaviour GetPrefab()
	{
		return prefab;
	}

	public override void Init(MonoBehaviour spawnedPrefab)
	{
		StamperController stamperController = spawnedPrefab as StamperController;
		stamperController.SetupStamper(stampPrefab, meshMaterial, worldItemSwitches);
	}
}
