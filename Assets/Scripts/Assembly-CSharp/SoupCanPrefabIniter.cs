using UnityEngine;

public class SoupCanPrefabIniter : MonoBehaviourPrefabIniter
{
	[SerializeField]
	private SoupCan prefab;

	[SerializeField]
	private Material material;

	public override MonoBehaviour GetPrefab()
	{
		return prefab;
	}

	public override void Init(MonoBehaviour spawnedPrefab)
	{
		(spawnedPrefab as SoupCan).SetupSoupCan(material);
	}
}
