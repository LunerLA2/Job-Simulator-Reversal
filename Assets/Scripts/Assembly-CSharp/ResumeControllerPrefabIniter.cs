using OwlchemyVR;
using UnityEngine;

public class ResumeControllerPrefabIniter : MonoBehaviourPrefabIniter
{
	[SerializeField]
	private ResumeController prefab;

	[SerializeField]
	private int portraitIndex;

	[SerializeField]
	private WorldItemData worldItemData;

	public override MonoBehaviour GetPrefab()
	{
		return prefab;
	}

	public override void Init(MonoBehaviour spawnedPrefab)
	{
		ResumeController resumeController = spawnedPrefab as ResumeController;
		resumeController.SetupEmployeeEvaluation(portraitIndex, worldItemData);
	}
}
