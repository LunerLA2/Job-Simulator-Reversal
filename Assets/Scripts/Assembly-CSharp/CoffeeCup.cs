using UnityEngine;

public class CoffeeCup : MonoBehaviour
{
	[SerializeField]
	private MeshRenderer meshRenderer;

	public void SetupCoffeeCup(Material material)
	{
		meshRenderer.sharedMaterial = material;
	}
}
