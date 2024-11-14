using UnityEngine;

public class GameDevTextureSwaps : MonoBehaviour
{
	[SerializeField]
	private GameDevTextureSwapInfo[] infos;

	private void Start()
	{
		if (GenieManager.AreAnyJobGenieModesActive() && GenieManager.DoesContainGenieMode(GlobalStorage.Instance.CurrentGenieModes, JobGenieCartridge.GenieModeTypes.OfficeModMode))
		{
			for (int i = 0; i < infos.Length; i++)
			{
				infos[i].Apply();
			}
		}
	}
}
