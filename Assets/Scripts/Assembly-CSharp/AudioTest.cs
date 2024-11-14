using System.Collections;
using UnityEngine;

public class AudioTest : MonoBehaviour
{
	[SerializeField]
	private AudioSource audioSource;

	[SerializeField]
	private AudioClip clipToTest;

	[SerializeField]
	private MeshRenderer meshRenderer;

	private void OnEnable()
	{
		Debug.LogWarning("AUDIO TEST - On Enable");
		audioSource.clip = clipToTest;
	}

	private void Awake()
	{
		Debug.LogWarning("AUDIO TEST - AWAKE");
	}

	private IEnumerator Start()
	{
		Debug.LogWarning("AUDIO TEST SPHERE STARTED");
		Debug.LogWarning(audioSource.clip);
		while (true)
		{
			Debug.LogWarning("Was Playing " + audioSource.isPlaying);
			audioSource.Play();
			Debug.LogWarning(audioSource.clip.name + " Played");
			Debug.LogWarning("Is Playing " + audioSource.isPlaying);
			yield return new WaitForSeconds(2f);
		}
	}

	private void Update()
	{
		meshRenderer.material.color = ((!audioSource.isPlaying) ? Color.white : Color.red);
	}
}
