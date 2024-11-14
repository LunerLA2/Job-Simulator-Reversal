using OwlchemyVR;
using UnityEngine;

public class HeadlightFluidBottleController : MonoBehaviour
{
	[SerializeField]
	private WorldItem myWorldItem;

	[SerializeField]
	private GravityDispensingItem gravityDispensingItem;

	[SerializeField]
	private MeshRenderer meshRenderer;

	public void Setup(WorldItemData worldItem, WorldItemData fluidToDispense, Material mainMaterial)
	{
		myWorldItem.ManualSetData(worldItem);
		gravityDispensingItem.SetFluidToDispense(fluidToDispense);
		meshRenderer.material = mainMaterial;
	}
}
