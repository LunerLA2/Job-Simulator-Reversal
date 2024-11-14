using OwlchemyVR;
using UnityEngine;

public class IDCardController : MonoBehaviour
{
	[SerializeField]
	private WorldItem myWorldItem;

	[SerializeField]
	private MeshRenderer meshRenderer;

	public void Setup(Material mat, WorldItemData worldItemData)
	{
		meshRenderer.material = mat;
		myWorldItem.ManualSetData(worldItemData);
	}
}
