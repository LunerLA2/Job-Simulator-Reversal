using System;
using System.Collections;
using OwlchemyVR;
using UnityEngine;

public class TeaKettleController : MonoBehaviour
{
	[SerializeField]
	private GrabbableItem lidGrabbable;

	[SerializeField]
	private GrabbableItem baseGrabbable;

	[SerializeField]
	private InteractableItem baseInteractable;

	[SerializeField]
	private InteractableItem lidInteractable;

	[SerializeField]
	private SelectedChangeOutlineController lidOutlineController;

	[SerializeField]
	private TemperatureStateItem temperatureStateItem;

	[SerializeField]
	private ContainerFluidSystem fluidSystem;

	[SerializeField]
	private AudioSourceHelper audioSource;

	[SerializeField]
	private ParticleSystem steamParticle;

	private bool isBoiling;

	private float lastTemp;

	private float audioPitch = 0.5f;

	private float audioVolume;

	private void Update()
	{
		if (steamParticle != null && audioSource != null)
		{
			if (!isBoiling && temperatureStateItem.TemperatureCelsius >= 100f && fluidSystem.FluidFullPercent > 0f && lastTemp < temperatureStateItem.TemperatureCelsius)
			{
				isBoiling = true;
				StartCoroutine(StartBoilingAsync());
			}
			if (isBoiling && (fluidSystem.FluidFullPercent == 0f || temperatureStateItem.TemperatureCelsius < 100f || lastTemp > temperatureStateItem.TemperatureCelsius))
			{
				isBoiling = false;
				StartCoroutine(StopBoilingAsync());
			}
			lastTemp = temperatureStateItem.TemperatureCelsius;
		}
	}

	private void OnEnable()
	{
		GrabbableItem grabbableItem = baseGrabbable;
		grabbableItem.OnGrabbed = (Action<GrabbableItem>)Delegate.Combine(grabbableItem.OnGrabbed, new Action<GrabbableItem>(BaseWasPickedUp));
		GrabbableItem grabbableItem2 = baseGrabbable;
		grabbableItem2.OnReleasedWasNotSwappedBetweenHands = (Action<GrabbableItem>)Delegate.Combine(grabbableItem2.OnReleasedWasNotSwappedBetweenHands, new Action<GrabbableItem>(BaseWasPutDown));
	}

	private void OnDisable()
	{
		GrabbableItem grabbableItem = baseGrabbable;
		grabbableItem.OnGrabbed = (Action<GrabbableItem>)Delegate.Remove(grabbableItem.OnGrabbed, new Action<GrabbableItem>(BaseWasPickedUp));
		GrabbableItem grabbableItem2 = baseGrabbable;
		grabbableItem2.OnReleasedWasNotSwappedBetweenHands = (Action<GrabbableItem>)Delegate.Remove(grabbableItem2.OnReleasedWasNotSwappedBetweenHands, new Action<GrabbableItem>(BaseWasPutDown));
	}

	private void BaseWasPickedUp(GrabbableItem item)
	{
	}

	private void BaseWasPutDown(GrabbableItem item)
	{
	}

	private IEnumerator StartBoilingAsync()
	{
		steamParticle.Play();
		audioSource.Play();
		while (isBoiling && audioPitch < 1f && audioVolume < 1f)
		{
			if (audioPitch < 1f)
			{
				audioPitch += 0.01f;
				audioSource.SetPitch(audioPitch);
			}
			if (audioVolume < 1f)
			{
				audioVolume += 0.01f;
				audioSource.SetVolume(audioVolume);
			}
			yield return null;
		}
	}

	private IEnumerator StopBoilingAsync()
	{
		while (!isBoiling && audioPitch > 0.5f && audioVolume > 0f)
		{
			if (audioPitch > 0.5f)
			{
				audioPitch -= 0.01f;
				audioSource.SetPitch(audioPitch);
			}
			if (audioVolume > 0f)
			{
				audioVolume -= 0.01f;
				audioSource.SetVolume(audioVolume);
			}
			yield return null;
		}
		audioSource.Stop();
		steamParticle.Stop();
	}
}
