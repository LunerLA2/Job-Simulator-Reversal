using OwlchemyVR;
using UnityEngine;

public class MagazineController : MonoBehaviour
{
	[SerializeField]
	private MeshFilter meshFilter;

	[SerializeField]
	private MeshRenderer meshRenderer;

	[SerializeField]
	private SelectedChangeOutlineController outlineController;

	[SerializeField]
	private WorldItem worldItem;

	public void Setup(Mesh mesh, Material mat, WorldItemData data)
	{
		meshFilter.mesh = mesh;
		meshRenderer.material = mat;
		outlineController.ForceRefreshMeshes();
		if (data != null)
		{
			worldItem.ManualSetData(data);
		}
	}
}
