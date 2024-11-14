using OwlchemyVR;
using UnityEngine;

public class ChemicalBottle : MonoBehaviour
{
	[SerializeField]
	private GravityDispensingItem dispenseItem;

	[SerializeField]
	private MeshRenderer meshRenderer;

	public void SetupChemicalBottle(Material material, WorldItemData fluidToDispense)
	{
		dispenseItem.SetFluidToDispense(fluidToDispense);
		if (material == null)
		{
			meshRenderer.material.SetColor("_DiffColor", fluidToDispense.OverallColor);
		}
		else
		{
			meshRenderer.sharedMaterial = material;
		}
	}
}
