using System;
using UnityEngine;

public class MagicReceiver : MonoBehaviour
{
	[SerializeField]
	private GameObject replaceMeWithThisPrefab;

	[SerializeField]
	private ParticleSystem[] particlesOnMagic;

	[SerializeField]
	private Animation[] animateOnMagic;

	[SerializeField]
	private AudioClip soundEffectOnMagic;

	public Action<MagicReceiver> OnMagicHappened;

	public void HitByMagic()
	{
		bool flag = false;
		if (particlesOnMagic.Length > 0)
		{
			for (int i = 0; i < particlesOnMagic.Length; i++)
			{
				particlesOnMagic[i].Play();
				flag = true;
			}
		}
		if (animateOnMagic.Length > 0)
		{
			for (int j = 0; j < animateOnMagic.Length; j++)
			{
				animateOnMagic[j].Play();
				flag = true;
			}
		}
		if (soundEffectOnMagic != null)
		{
			AudioManager.Instance.Play(base.transform.position, soundEffectOnMagic, 1f, 1f);
		}
		if (replaceMeWithThisPrefab != null)
		{
			UnityEngine.Object.Instantiate(replaceMeWithThisPrefab, base.transform.position, base.transform.rotation);
			UnityEngine.Object.Destroy(base.gameObject);
			flag = true;
		}
		if (flag && OnMagicHappened != null)
		{
			OnMagicHappened(this);
		}
	}
}
