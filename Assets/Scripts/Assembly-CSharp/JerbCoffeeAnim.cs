using System.Collections;
using UnityEngine;

public class JerbCoffeeAnim : MonoBehaviour
{
	[SerializeField]
	private ParticleSystem SplashPart;

	[SerializeField]
	private AudioSource SplashAudio;

	[SerializeField]
	private AudioClip[] Clips;

	[SerializeField]
	private Animator CoffeeAnimator;

	[SerializeField]
	private AttachablePoint coffeCupAttachPoint;

	private void Start()
	{
		StartCoroutine(SplashCoffee());
	}

	private IEnumerator SplashCoffee()
	{
		while (true)
		{
			yield return new WaitForSeconds(Mathf.RoundToInt(Random.Range(20, 120)));
			if (!coffeCupAttachPoint.IsRefilling && coffeCupAttachPoint.IsOccupied)
			{
				CoffeeAnimator.SetTrigger("Splash");
				yield return new WaitForSeconds(0.1f);
				SplashPart.Play();
				SplashAudio.PlayOneShot(Clips[Mathf.RoundToInt(Random.Range(0, Clips.Length - 1))]);
			}
		}
	}
}
