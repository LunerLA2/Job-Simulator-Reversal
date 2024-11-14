using UnityEngine;

public class ChemicalContainerController : MonoBehaviour
{
	public Color chemicalColor;

	public ChemLabManager.Chemicals chemical;

	public Material chemicalMaterial;

	private void Awake()
	{
		chemicalMaterial = GetComponent<MeshRenderer>().material;
	}
}
