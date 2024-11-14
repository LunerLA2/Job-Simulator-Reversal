using OwlchemyVR;
using UnityEngine;

public class MagicWand : MonoBehaviour
{
	[SerializeField]
	private PickupableItem item;

	[SerializeField]
	private ParticleSystem[] pfxMagicIsActive;

	[SerializeField]
	private ParticleSystem pfxVelocityIsOK;

	[SerializeField]
	private ParticleSystem pfxBurst;

	[SerializeField]
	private AudioClip soundEffectOnReady;

	[SerializeField]
	private AudioClip soundEffectOnShoot;

	[SerializeField]
	private Animation previewDirectionAnimation;

	[SerializeField]
	private MagicProjectile projectileToSpawn;

	[SerializeField]
	private Transform projectileShootOutOf;

	[SerializeField]
	private float minimumVelocityToCharge;

	[SerializeField]
	private float secondsOfChargingRequired;

	private Vector3 prevPos;

	private float amountOfCharge;

	private bool isCharged;

	private float timeSpentStill;

	private void Start()
	{
		prevPos = base.transform.position;
		previewDirectionAnimation.gameObject.SetActive(false);
	}

	private void Update()
	{
		if (item.IsCurrInHand)
		{
			float num = Mathf.Abs(((base.transform.position - prevPos) / Time.deltaTime).magnitude);
			prevPos = base.transform.position;
			if (num >= minimumVelocityToCharge)
			{
				amountOfCharge += Time.deltaTime;
				pfxVelocityIsOK.emissionRate = num * 15f;
				timeSpentStill = 0f;
				if (previewDirectionAnimation.isPlaying)
				{
					previewDirectionAnimation.Stop();
					previewDirectionAnimation.gameObject.SetActive(false);
				}
			}
			else
			{
				timeSpentStill += Time.deltaTime;
				if (isCharged && timeSpentStill > 0.25f && !previewDirectionAnimation.isPlaying)
				{
					previewDirectionAnimation.gameObject.SetActive(true);
					previewDirectionAnimation.Play();
				}
				if (timeSpentStill >= 1f)
				{
					amountOfCharge = 0f;
					pfxVelocityIsOK.emissionRate = 0f;
					if (isCharged)
					{
						pfxBurst.Play();
						Object.Instantiate(projectileToSpawn, projectileShootOutOf.position, projectileShootOutOf.rotation);
						AudioManager.Instance.Play(base.transform.position, soundEffectOnShoot, 0.25f, 1f);
					}
				}
			}
			if (amountOfCharge >= secondsOfChargingRequired)
			{
				amountOfCharge = secondsOfChargingRequired;
				if (!isCharged)
				{
					isCharged = true;
					SetStateOfMagicEffects(true);
					AudioManager.Instance.Play(base.transform.position, soundEffectOnReady, 1f, 1f);
				}
			}
			else if (isCharged)
			{
				isCharged = false;
				SetStateOfMagicEffects(false);
			}
		}
		else
		{
			if (previewDirectionAnimation.isPlaying)
			{
				previewDirectionAnimation.Stop();
				previewDirectionAnimation.gameObject.SetActive(false);
			}
			prevPos = base.transform.position;
			pfxVelocityIsOK.emissionRate = 0f;
			amountOfCharge = 0f;
			if (isCharged)
			{
				isCharged = false;
				SetStateOfMagicEffects(false);
			}
		}
	}

	private void SetStateOfMagicEffects(bool state)
	{
		for (int i = 0; i < pfxMagicIsActive.Length; i++)
		{
			if (state)
			{
				pfxMagicIsActive[i].Play();
			}
			else
			{
				pfxMagicIsActive[i].Stop();
			}
		}
	}
}
