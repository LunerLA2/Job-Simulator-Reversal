using OwlchemyVR;
using UnityEngine;

public class JobGenieCartridgePrefabIniter : MonoBehaviourPrefabIniter
{
	[SerializeField]
	private JobGenieCartridge prefab;

	[SerializeField]
	private JobGenieCartridge.GenieModeTypes genieCartType;

	[SerializeField]
	private Material material;

	[SerializeField]
	private WorldItemData worldItemData;

	public override MonoBehaviour GetPrefab()
	{
		return prefab;
	}

	public override void Init(MonoBehaviour spawnedPrefab)
	{
		(spawnedPrefab as JobGenieCartridge).SetupMaterial(material);
		(spawnedPrefab as JobGenieCartridge).SetGenieModeType(genieCartType, worldItemData);
	}
}
