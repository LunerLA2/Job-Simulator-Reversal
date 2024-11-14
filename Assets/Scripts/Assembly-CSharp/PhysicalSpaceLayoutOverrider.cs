using PSC;
using UnityEngine;

public class PhysicalSpaceLayoutOverrider : MonoBehaviour
{
	private LayoutConfiguration storedActualLayoutToUse;

	[SerializeField]
	private LayoutConfiguration layoutToLoadForDollhouseMode;

	private void Awake()
	{
		storedActualLayoutToUse = Room.defaultLayoutToLoad;
		if (GenieManager.AreAnyJobGenieModesActive() && GenieManager.DoesContainGenieMode(GlobalStorage.Instance.CurrentGenieModes, JobGenieCartridge.GenieModeTypes.DollhouseMode))
		{
			Room.defaultLayoutToLoad = layoutToLoadForDollhouseMode;
		}
	}

	private void Start()
	{
		Room.defaultLayoutToLoad = storedActualLayoutToUse;
	}
}
