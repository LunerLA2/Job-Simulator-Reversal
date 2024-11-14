using OwlchemyVR;
using UnityEngine;

public class LicensePlateController : MonoBehaviour
{
	[SerializeField]
	private MeshRenderer mainRenderer;

	[SerializeField]
	private WorldItem myWorldItem;

	public void Setup(Material mat, WorldItemData wi = null)
	{
		mainRenderer.material = mat;
		if (wi != null)
		{
			myWorldItem.ManualSetData(wi);
		}
	}
}
