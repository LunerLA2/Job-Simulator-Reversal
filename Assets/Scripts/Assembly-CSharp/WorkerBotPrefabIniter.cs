using UnityEngine;

public class WorkerBotPrefabIniter : MonoBehaviourPrefabIniter
{
	[SerializeField]
	private WorkerBotController prefab;

	[SerializeField]
	private BotCostumeData costumeData;

	[SerializeField]
	private AudioClip[] hitVoClips;

	[SerializeField]
	private bool hideBotDuringIdle = true;

	[SerializeField]
	private bool voOnCubicleHit;

	[SerializeField]
	private Vector3 idleLocalRotation = Vector3.zero;

	public override MonoBehaviour GetPrefab()
	{
		return prefab;
	}

	public override void Init(MonoBehaviour spawnedPrefab)
	{
		WorkerBotController workerBotController = spawnedPrefab as WorkerBotController;
		workerBotController.SetCostume(costumeData);
		workerBotController.Setup(hideBotDuringIdle, voOnCubicleHit, idleLocalRotation);
		workerBotController.SetVoClipArray(hitVoClips);
	}
}
