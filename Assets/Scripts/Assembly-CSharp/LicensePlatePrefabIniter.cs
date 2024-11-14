using OwlchemyVR;
using UnityEngine;

public class LicensePlatePrefabIniter : MonoBehaviourPrefabIniter
{
	[SerializeField]
	private LicensePlateController prefab;

	[SerializeField]
	private Material material;

	[SerializeField]
	private WorldItemData optionalWorldItemOverride;

	public override MonoBehaviour GetPrefab()
	{
		return prefab;
	}

	public override void Init(MonoBehaviour spawnedPrefab)
	{
		(spawnedPrefab as LicensePlateController).Setup(material, optionalWorldItemOverride);
	}
}
