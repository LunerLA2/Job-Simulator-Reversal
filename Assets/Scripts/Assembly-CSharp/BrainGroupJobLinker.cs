using UnityEngine;

public class BrainGroupJobLinker : MonoBehaviour
{
	[SerializeField]
	private string brainGroupDataName;

	private BrainGroupData brainGroupData;

	[SerializeField]
	private JobData jobData;

	[SerializeField]
	private bool resetOnDisable = true;

	[SerializeField]
	private bool ignoreIfEndlessMode = true;

	private void Awake()
	{
		if (!GenieManager.AreAnyJobGenieModesActive() || !GenieManager.DoesContainGenieMode(GlobalStorage.Instance.CurrentGenieModes, JobGenieCartridge.GenieModeTypes.EndlessMode) || !ignoreIfEndlessMode)
		{
			brainGroupData = Resources.Load<BrainGroupData>(brainGroupDataName);
			if (brainGroupData != null && jobData.BrainGroup != brainGroupData)
			{
				jobData.SetBrainGroup(brainGroupData);
			}
		}
	}

	private void OnDisable()
	{
		if (resetOnDisable)
		{
			jobData.SetBrainGroup(null);
		}
	}
}
