using UnityEngine;

public class GameDeveloperWeapon : MonoBehaviour
{
	[SerializeField]
	private AudioClip fireSound;

	public void FireEffects()
	{
		AudioManager.Instance.Play(base.transform.position, fireSound, 1f, Random.Range(0.7f, 1.2f));
	}
}
