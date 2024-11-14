using OwlchemyVR;
using UnityEngine;

public class GumPack : MonoBehaviour
{
	[SerializeField]
	private MeshRenderer meshRenderer;

	[SerializeField]
	private WorldItem wItem;

	public void Init(Material material, WorldItemData itemData)
	{
		meshRenderer.sharedMaterial = material;
		wItem.ManualSetData(itemData);
	}
}
