using UnityEngine;

public class SodaBottlePrefabIniter : MonoBehaviourPrefabIniter
{
	[SerializeField]
	private SodaBottleController prefab;

	[SerializeField]
	private Material material;

	[SerializeField]
	private Color pfxTint = Color.white;

	public override MonoBehaviour GetPrefab()
	{
		return prefab;
	}

	public override void Init(MonoBehaviour spawnedPrefab)
	{
		(spawnedPrefab as SodaBottleController).Setup(material, pfxTint);
	}
}
