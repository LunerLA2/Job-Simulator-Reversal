using UnityEngine;
using UnityEngine.Audio;

public class AudioPrefLoader : MonoBehaviour
{
	public AudioMixer MicMixer;

	private void Awake()
	{
		AudioMixerGroup[] array = MicMixer.FindMatchingGroups("Master");
		AudioManager.Instance.MicMixerGroup = array[0];
	}
}
