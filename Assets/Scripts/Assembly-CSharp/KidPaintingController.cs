using OwlchemyVR;
using UnityEngine;

public class KidPaintingController : MonoBehaviour
{
	[SerializeField]
	private WorldItem myWorldItem;

	[SerializeField]
	private UniqueObject uniqueObject;

	[SerializeField]
	private MeshRenderer meshRenderer;

	public void Setup(Material _mat, WorldItemData _worldItemData, string _uniqueID)
	{
		myWorldItem.ManualSetData(_worldItemData);
		meshRenderer.material = _mat;
		uniqueObject.ManualChangeName(_uniqueID);
	}
}
