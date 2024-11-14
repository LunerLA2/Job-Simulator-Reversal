using UnityEngine;

public class PotatoChips : MonoBehaviour
{
	[SerializeField]
	private MeshRenderer meshRenderer;

	public void Setup(Material mat)
	{
		meshRenderer.material = mat;
	}
}
