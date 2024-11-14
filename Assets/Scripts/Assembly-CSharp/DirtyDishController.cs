using UnityEngine;

public class DirtyDishController : MonoBehaviour
{
	[SerializeField]
	private MeshRenderer dirtRenderer;

	[SerializeField]
	private WashableItem washableItem;

	public void Setup(Material dirtMat, float alphaCutoff)
	{
		dirtRenderer.material = dirtMat;
		washableItem.SetHighestAlphaCutoffValue(alphaCutoff);
	}
}
