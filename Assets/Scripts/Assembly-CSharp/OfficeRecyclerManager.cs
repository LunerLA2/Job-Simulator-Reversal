using UnityEngine;

public class OfficeRecyclerManager : ItemRecyclerManager
{
	[SerializeField]
	private SceneRecyclingData recycleDataForGameDev;

	private void Start()
	{
		if (GenieManager.AreAnyJobGenieModesActive() && GenieManager.DoesContainGenieMode(GlobalStorage.Instance.CurrentGenieModes, JobGenieCartridge.GenieModeTypes.OfficeModMode))
		{
			recyclingData = recycleDataForGameDev;
		}
		if (OfficeManager.Instance.AlwaysLoadGameDevJob)
		{
			recyclingData = recycleDataForGameDev;
		}
		if (recyclingData == recycleDataForGameDev)
		{
			BuildLookupDictionary();
		}
	}
}
