using System;
using OwlchemyVR;
using UnityEngine;

[RequireComponent(typeof(BlowableItem))]
public class InflatableItem : MonoBehaviour
{
	[SerializeField]
	private BlowableItem blowableItem;

	[SerializeField]
	private Animation scrubAnimation;

	[SerializeField]
	private float secondsOfBlowingToInflate = 3f;

	[SerializeField]
	private float secondsOfNonBlowingBeforeStartingToDeflate = 0.5f;

	[SerializeField]
	private bool allowDeflatingWhileInMouth;

	[SerializeField]
	private AudioSourceHelper inflationSound;

	[SerializeField]
	private AudioClip soundWhileInflating;

	[SerializeField]
	private Collider[] physicsMaterialApplyTo;

	[SerializeField]
	private PhysicMaterial physicsMaterialWhenInflated;

	[SerializeField]
	private ConstantForce constantForceWhenInflated;

	[SerializeField]
	private ParticleSystem deflationParticle;

	[SerializeField]
	private AutoOrientPickupable autoOrientOnlyBeforeFullyInflating;

	private float fillAmount;

	private bool filled;

	private float timeSinceLastBlow;

	private float deflationSpeedMultiplier = 2f;

	private ParticleSystem.EmissionModule em;

	private void OnEnable()
	{
		BlowableItem obj = blowableItem;
		obj.OnWasBlown = (Action<BlowableItem, float, HeadController>)Delegate.Combine(obj.OnWasBlown, new Action<BlowableItem, float, HeadController>(BlownUp));
	}

	private void OnDisable()
	{
		BlowableItem obj = blowableItem;
		obj.OnWasBlown = (Action<BlowableItem, float, HeadController>)Delegate.Remove(obj.OnWasBlown, new Action<BlowableItem, float, HeadController>(BlownUp));
	}

	private void Start()
	{
		UpdateAnimationScrub(0f);
		inflationSound.SetClip(soundWhileInflating);
		inflationSound.SetLooping(true);
		blowableItem.enabled = true;
	}

	private void Update()
	{
		if (!filled && (!blowableItem.InMouth || allowDeflatingWhileInMouth))
		{
			if (timeSinceLastBlow < secondsOfNonBlowingBeforeStartingToDeflate)
			{
				timeSinceLastBlow += Time.deltaTime;
				return;
			}
			if (fillAmount > 0f)
			{
				BlownUp(blowableItem, (0f - Time.deltaTime) * deflationSpeedMultiplier);
				em = deflationParticle.emission;
				em.enabled = true;
				return;
			}
			if (inflationSound.IsPlaying && soundWhileInflating != null)
			{
				inflationSound.Stop();
			}
			em = deflationParticle.emission;
			em.enabled = false;
		}
		else
		{
			em = deflationParticle.emission;
			em.enabled = false;
		}
	}

	private void BlownUp(BlowableItem blowableItem, float amount, HeadController headController)
	{
		BlownUp(blowableItem, amount);
	}

	private void BlownUp(BlowableItem blowableItem, float amount)
	{
		if (filled)
		{
			return;
		}
		if (!inflationSound.IsPlaying && soundWhileInflating != null)
		{
			inflationSound.Play();
		}
		if (amount > 0f)
		{
			timeSinceLastBlow = 0f;
		}
		fillAmount += amount / secondsOfBlowingToInflate;
		if (fillAmount > 1f)
		{
			FinishedBlowingUp();
		}
		else
		{
			filled = false;
			blowableItem.enabled = true;
			if (fillAmount < 0f)
			{
				fillAmount = 0f;
			}
		}
		UpdateAnimationScrub(fillAmount);
	}

	private void FinishedBlowingUp()
	{
		fillAmount = 1f;
		filled = true;
		blowableItem.enabled = false;
		if (inflationSound.IsPlaying && soundWhileInflating != null)
		{
			inflationSound.Stop();
		}
		if (autoOrientOnlyBeforeFullyInflating != null)
		{
			autoOrientOnlyBeforeFullyInflating.enabled = false;
		}
		for (int i = 0; i < physicsMaterialApplyTo.Length; i++)
		{
			physicsMaterialApplyTo[i].material = physicsMaterialWhenInflated;
		}
		if (GenieManager.AreAnyJobGenieModesActive() && !GenieManager.DoesContainGenieMode(GlobalStorage.Instance.CurrentGenieModes, JobGenieCartridge.GenieModeTypes.NoGravityMode))
		{
			constantForceWhenInflated.enabled = true;
		}
		ParticleSystem.EmissionModule emission = deflationParticle.emission;
		emission.enabled = false;
	}

	private void UpdateAnimationScrub(float scrubPercentage)
	{
		float time = scrubPercentage * scrubAnimation.clip.length;
		scrubAnimation[scrubAnimation.clip.name].enabled = true;
		scrubAnimation[scrubAnimation.clip.name].weight = 1f;
		scrubAnimation[scrubAnimation.clip.name].time = time;
		scrubAnimation.Sample();
		scrubAnimation[scrubAnimation.clip.name].enabled = false;
	}
}
