using OwlchemyVR;
using UnityEngine;

public class CashController : MonoBehaviour
{
	[SerializeField]
	private WorldItem myWorldItem;

	[SerializeField]
	private MeshRenderer visuals;

	public void Setup(Material mat, WorldItemData wi)
	{
		myWorldItem.ManualSetData(wi);
		visuals.material = mat;
	}
}
