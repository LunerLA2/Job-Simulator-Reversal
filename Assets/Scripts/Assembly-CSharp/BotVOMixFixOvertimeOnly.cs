using UnityEngine;

public class BotVOMixFixOvertimeOnly : MonoBehaviour
{
	[SerializeField]
	private float botVolume = 0.92f;

	private void Awake()
	{
		if (GenieManager.AreAnyJobGenieModesActive() && GenieManager.DoesContainGenieMode(GlobalStorage.Instance.CurrentGenieModes, JobGenieCartridge.GenieModeTypes.EndlessMode) && AudioManager.AudioSystemType == AudioManager.AudioSystemTypes.OculusAudio)
		{
			GetComponent<ONSPAudioSource>().Gain = 0f;
			AudioSource component = GetComponent<AudioSource>();
			component.SetSpatializerFloat(0, 0f);
			AudioSourceHelper component2 = GetComponent<AudioSourceHelper>();
			component2.SetVolume(botVolume);
		}
	}
}
