using UnityEngine;

public class RadioChangeChannelInfinite : MonoBehaviour
{
	[SerializeField]
	private AudioSourceHelper audioSourceHelper;

	[SerializeField]
	private RadioChannelController radioController;

	[SerializeField]
	[Space]
	private AudioClip soundClip;

	[SerializeField]
	private bool changeName;

	[SerializeField]
	private string channelName;

	private void Start()
	{
		if (GenieManager.AreAnyJobGenieModesActive() && GenieManager.DoesContainGenieMode(GlobalStorage.Instance.CurrentGenieModes, JobGenieCartridge.GenieModeTypes.EndlessMode))
		{
			audioSourceHelper.SetClip(soundClip);
			audioSourceHelper.Play();
			if (changeName)
			{
				radioController.ChannelInfo = channelName;
			}
		}
	}
}
