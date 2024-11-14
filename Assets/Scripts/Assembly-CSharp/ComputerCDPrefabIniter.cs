using System;
using OwlchemyVR;
using UnityEngine;

public class ComputerCDPrefabIniter : MonoBehaviourPrefabIniter
{
	[Serializable]
	protected struct RandomProgram
	{
		public ComputerProgramID program;

		public WorldItemData itemData;

		public Material material;
	}

	[SerializeField]
	private ComputerCD prefab;

	[SerializeField]
	private Material material;

	[SerializeField]
	private ComputerProgramID programID;

	[SerializeField]
	private WorldItemData itemData;

	[SerializeField]
	private bool spawnRandomCD;

	[SerializeField]
	private RandomProgram[] randomPrograms;

	public override MonoBehaviour GetPrefab()
	{
		return prefab;
	}

	public override void Init(MonoBehaviour spawnedPrefab)
	{
		if (spawnRandomCD)
		{
			if (randomPrograms.Length > 0)
			{
				RandomProgram randomProgram = randomPrograms[UnityEngine.Random.Range(0, randomPrograms.Length)];
				programID = randomProgram.program;
				itemData = randomProgram.itemData;
				material = randomProgram.material;
			}
			else
			{
				Debug.LogError("randomPrograms array must be greater than 0 if spawnRandomCD is true");
			}
		}
		((ComputerCD)spawnedPrefab).SetupCD(programID, itemData, material);
	}
}
