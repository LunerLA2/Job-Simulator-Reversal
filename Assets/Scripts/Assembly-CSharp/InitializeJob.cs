using UnityEngine;

public class InitializeJob : MonoBehaviour
{
	[SerializeField]
	private JobData jobData;

	private void Start()
	{
		BotManager.Instance.InitializeJob(jobData);
		JobBoardManager.instance.InitJob(jobData);
	}
}
