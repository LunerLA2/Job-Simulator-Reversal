using OwlchemyVR;
using UnityEngine;

public class PaintBottle : MonoBehaviour
{
	[SerializeField]
	private GravityDispensingItem dispenseItem;

	[SerializeField]
	private MeshRenderer meshRenderer;

	public void SetupPaintBottle(Material material, WorldItemData fluidToDispense)
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
