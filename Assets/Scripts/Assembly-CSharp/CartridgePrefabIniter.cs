using OwlchemyVR;
using UnityEngine;

public class CartridgePrefabIniter : MonoBehaviourPrefabIniter
{
	[SerializeField]
	private JobCartridge prefab;

	[SerializeField]
	private JobData jobData;

	[SerializeField]
	private JobData altJobForGamedev;

	[SerializeField]
	private Material cartridgeArt;

	[SerializeField]
	private Material cartridgeCompleteArt;

	[SerializeField]
	private Material cartridgeCompleteOvertimeArt;

	[SerializeField]
	private WorldItemData worldItemData;

	public override MonoBehaviour GetPrefab()
	{
		return prefab;
	}

	public override void Init(MonoBehaviour spawnedPrefab)
	{
		JobCartridge jobCartridge = spawnedPrefab as JobCartridge;
		JobStateData jobStateDataByJobData = GlobalStorage.Instance.GameStateData.GetJobStateDataByJobData(jobData);
		Material newArt = ((jobStateDataByJobData.GetPercentageComplete() != 1f) ? cartridgeArt : cartridgeCompleteArt);
		Material overtimeArt = ((jobStateDataByJobData.GetPercentageComplete() != 1f) ? cartridgeArt : cartridgeCompleteOvertimeArt);
		JobStateData altJobStateForGamedev = null;
		if (altJobForGamedev != null)
		{
			altJobStateForGamedev = GlobalStorage.Instance.GameStateData.GetJobStateDataByJobData(altJobForGamedev);
		}
		jobCartridge.SetUpCartridge(jobStateDataByJobData, altJobStateForGamedev, newArt, worldItemData, overtimeArt);
	}
}
