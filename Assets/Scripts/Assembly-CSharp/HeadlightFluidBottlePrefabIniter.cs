using OwlchemyVR;
using UnityEngine;

public class HeadlightFluidBottlePrefabIniter : MonoBehaviourPrefabIniter
{
	[SerializeField]
	private HeadlightFluidBottleController prefab;

	[SerializeField]
	private WorldItemData bottleWorldItem;

	[SerializeField]
	private WorldItemData fluidWorldItem;

	[SerializeField]
	private Material bottleMaterial;

	public override MonoBehaviour GetPrefab()
	{
		return prefab;
	}

	public override void Init(MonoBehaviour spawnedPrefab)
	{
		(spawnedPrefab as HeadlightFluidBottleController).Setup(bottleWorldItem, fluidWorldItem, bottleMaterial);
	}
}
