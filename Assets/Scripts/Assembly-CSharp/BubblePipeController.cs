using System;
using UnityEngine;

public class BubblePipeController : MonoBehaviour
{
	private const int EMISSION_ON_BLOW = 24;

	private const float BURST_COOLDOWN = 0.5f;

	[SerializeField]
	private AudioClip bubbleBlowClip;

	[SerializeField]
	private AudioSourceHelper audioSourceHelper;

	[SerializeField]
	private ParticleSystem particleSystem;

	[SerializeField]
	private BlowableItem blowableItem;

	private ParticleSystem.MinMaxCurve baseEmissionRate;

	private float cooldownTime;

	private float baseLifetime;

	private bool isCooling;

	private void Awake()
	{
		baseEmissionRate = particleSystem.emission.rate;
		baseLifetime = particleSystem.startLifetime;
	}

	private void OnEnable()
	{
		BlowableItem obj = blowableItem;
		obj.OnWasBlown = (Action<BlowableItem, float, HeadController>)Delegate.Combine(obj.OnWasBlown, new Action<BlowableItem, float, HeadController>(OnWasBlown));
	}

	private void OnDisable()
	{
		BlowableItem obj = blowableItem;
		obj.OnWasBlown = (Action<BlowableItem, float, HeadController>)Delegate.Remove(obj.OnWasBlown, new Action<BlowableItem, float, HeadController>(OnWasBlown));
	}

	private void OnWasBlown(BlowableItem item, float amount, HeadController headController)
	{
		ParticleSystem.EmissionModule emission = particleSystem.emission;
		emission.rate = new ParticleSystem.MinMaxCurve(24f);
		particleSystem.startLifetime = baseLifetime * 2f;
		cooldownTime = 0.5f;
		isCooling = true;
		if (!audioSourceHelper.IsPlaying)
		{
			audioSourceHelper.SetClip(bubbleBlowClip);
			audioSourceHelper.SetLooping(true);
			audioSourceHelper.Play();
		}
	}

	private void Update()
	{
		if (!isCooling)
		{
			return;
		}
		cooldownTime -= Time.deltaTime;
		if (cooldownTime <= 0f)
		{
			isCooling = false;
			ParticleSystem.EmissionModule emission = particleSystem.emission;
			emission.rate = baseEmissionRate;
			particleSystem.startLifetime = baseLifetime;
			if (audioSourceHelper.IsPlaying)
			{
				audioSourceHelper.Stop();
			}
		}
	}
}
