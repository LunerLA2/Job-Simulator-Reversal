using OwlchemyVR;
using UnityEngine;

public class ComputerCD : MonoBehaviour
{
	[SerializeField]
	private MeshRenderer meshRenderer;

	private ComputerProgramID programID;

	public ComputerProgramID ProgramID
	{
		get
		{
			return programID;
		}
	}

	public void SetupCD(ComputerProgramID programID, WorldItemData itemData, Material material)
	{
		this.programID = programID;
		GetComponent<WorldItem>().ManualSetData(itemData);
		meshRenderer.sharedMaterial = material;
	}
}
