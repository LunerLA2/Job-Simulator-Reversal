using OwlchemyVR;
using UnityEngine;

public class ChemicalBottlePrefabIniter : MonoBehaviourPrefabIniter
{
	[SerializeField]
	private ChemicalBottle prefab;

	[SerializeField]
	private Material material;

	[SerializeField]
	private WorldItemData fluidToDispense;

	public override MonoBehaviour GetPrefab()
	{
		return prefab;
	}

	public override void Init(MonoBehaviour spawnedPrefab)
	{
		(spawnedPrefab as ChemicalBottle).SetupChemicalBottle(material, fluidToDispense);
	}
}
