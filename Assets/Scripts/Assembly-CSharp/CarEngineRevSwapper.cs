using UnityEngine;

public class CarEngineRevSwapper : MonoBehaviour
{
	[SerializeField]
	private AudioSourceHelper audioSrcHelper;

	[SerializeField]
	private AudioClip[] engineRevSounds;

	private void Awake()
	{
		audioSrcHelper.SetClip(engineRevSounds[Random.Range(0, engineRevSounds.Length)]);
	}
}
