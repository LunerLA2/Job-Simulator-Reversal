using UnityEngine;

public class SoundActionOnImpact : MonoBehaviour
{
	public float maxSoundThresh = 10f;

	public float hitSoundThresh = 0.5f;

	public float minTimeBetweenSounds = 0.05f;

	public AudioClip hitSound;

	private float lastSoundTime;

	private bool isHittable = true;

	private void OnCollisionEnter(Collision c)
	{
		if (!(Time.timeSinceLevelLoad < 2f) && c.relativeVelocity.sqrMagnitude > hitSoundThresh && !(Time.time - lastSoundTime < minTimeBetweenSounds) && isHittable)
		{
			Hit(c.relativeVelocity.sqrMagnitude);
		}
	}

	private void Hit(float hitStrength)
	{
		lastSoundTime = Time.time;
		AudioManager.Instance.Play(base.transform.position, hitSound, Mathf.Clamp(hitStrength / maxSoundThresh, 0.1f, 1f), 1f);
	}
}
