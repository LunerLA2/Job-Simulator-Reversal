using UnityEngine;

public class LevelManager : MonoBehaviour
{
	[SerializeField]
	protected JobData jobData;

	[SerializeField]
	protected bool ALWAYS_LOAD_ENDLESS_MODE;

	[SerializeField]
	protected string endlessModeConfigDataName;

	protected EndlessModeData endlessModeConfigData;

	public virtual void Awake()
	{
		if (ALWAYS_LOAD_ENDLESS_MODE || GenieManager.DoesContainGenieMode(GlobalStorage.Instance.CurrentGenieModes, JobGenieCartridge.GenieModeTypes.EndlessMode))
		{
			endlessModeConfigData = Resources.Load(endlessModeConfigDataName) as EndlessModeData;
			if (endlessModeConfigData == null)
			{
				Debug.LogError("Endless mode was specified but no EndlessModeConfigData is specified or the name given to " + base.gameObject.name + " was wrong.", base.gameObject);
			}
		}
	}
}
