using System.Collections;
using System.Collections.Generic;
using OwlchemyVR;
using OwlchemyVR2;
using UnityEngine;

public class FridgePantryController : MonoBehaviour
{
	[SerializeField]
	private Animation scrubAnimation;

	[SerializeField]
	private float maxScrubSpeed = 1f;

	[SerializeField]
	private WorldItemData worldItemToActivateOnUpper;

	[SerializeField]
	private WorldItemData worldItemToActivateOnLower;

	[SerializeField]
	private BasePrefabSpawner[] upperFoodContainerSpawners;

	[SerializeField]
	private BasePrefabSpawner[] lowerFoodContainerSpawners;

	[SerializeField]
	private OwlchemyVR2.GrabbableSlider toggleSlider;

	[SerializeField]
	private AudioSourceHelper audioSource;

	[SerializeField]
	private AudioClip slidingLoopSound;

	private bool isScrubbing;

	private bool isLoopingSoundPlaying;

	private float cachedOriginalVolumeOfLoopingSound;

	private float loopingSoundTimeRemaining;

	private float loopingSoundDecayTime = 0.5f;

	private float targetScrubAmount;

	private float currentScrubAmount;

	private List<RespawningFoodContainer> upperFoodContainers = new List<RespawningFoodContainer>();

	private List<RespawningFoodContainer> lowerFoodContainers = new List<RespawningFoodContainer>();

	private void OnEnable()
	{
		toggleSlider.OnUpperLocked += ToggleSliderUpperLocked;
		toggleSlider.OnLowerLocked += ToggleSliderLowerLocked;
		toggleSlider.OnLowerUnlocked += SliderWasUnlockedLower;
		toggleSlider.OnUpperUnlocked += SliderWasUnlockedUpper;
	}

	private void OnDisable()
	{
		toggleSlider.OnUpperLocked -= ToggleSliderUpperLocked;
		toggleSlider.OnLowerLocked -= ToggleSliderLowerLocked;
		toggleSlider.OnLowerUnlocked -= SliderWasUnlockedLower;
		toggleSlider.OnUpperUnlocked -= SliderWasUnlockedUpper;
	}

	private void Awake()
	{
		if (audioSource != null)
		{
			cachedOriginalVolumeOfLoopingSound = audioSource.DefaultStartingVolume;
		}
		upperFoodContainers.Clear();
		lowerFoodContainers.Clear();
		for (int i = 0; i < upperFoodContainerSpawners.Length; i++)
		{
			RespawningFoodContainer[] componentsInChildren = upperFoodContainerSpawners[i].LastSpawnedPrefabGO.GetComponentsInChildren<RespawningFoodContainer>();
			if (componentsInChildren != null)
			{
				upperFoodContainers.AddRange(componentsInChildren);
			}
			else
			{
				Debug.LogError("No food spawner");
			}
		}
		for (int j = 0; j < lowerFoodContainerSpawners.Length; j++)
		{
			RespawningFoodContainer[] componentsInChildren2 = lowerFoodContainerSpawners[j].LastSpawnedPrefabGO.GetComponentsInChildren<RespawningFoodContainer>();
			if (componentsInChildren2 != null)
			{
				lowerFoodContainers.AddRange(componentsInChildren2);
			}
			else
			{
				Debug.LogError("No food spawner");
			}
		}
		for (int k = 0; k < lowerFoodContainers.Count; k++)
		{
			lowerFoodContainers[k].ForceAllDoorsClosed();
		}
		for (int l = 0; l < upperFoodContainers.Count; l++)
		{
			upperFoodContainers[l].ForceAllDoorsClosed();
		}
		StartCoroutine(UnlockOnceSettled());
	}

	private IEnumerator UnlockOnceSettled()
	{
		yield return new WaitForSeconds(0.05f);
		ScrubUpdate(toggleSlider.NormalizedAxisValue);
		for (int i = 0; i < upperFoodContainers.Count; i++)
		{
			upperFoodContainers[i].AllowOpening();
		}
	}

	private void Update()
	{
		ScrubUpdate(toggleSlider.NormalizedAxisValue);
		if (isLoopingSoundPlaying)
		{
			loopingSoundTimeRemaining -= Time.deltaTime;
			if (loopingSoundTimeRemaining <= 0f)
			{
				audioSource.Stop();
				isLoopingSoundPlaying = false;
			}
			else
			{
				float num = loopingSoundTimeRemaining / loopingSoundDecayTime;
				audioSource.SetVolume(num * cachedOriginalVolumeOfLoopingSound);
			}
		}
	}

	private void ScrubUpdate(float perc)
	{
		targetScrubAmount = perc;
		bool flag = isScrubbing;
		if (currentScrubAmount < targetScrubAmount)
		{
			currentScrubAmount = Mathf.Min(currentScrubAmount + Time.deltaTime * maxScrubSpeed, targetScrubAmount);
			isScrubbing = true;
		}
		else if (currentScrubAmount > targetScrubAmount)
		{
			currentScrubAmount = Mathf.Max(currentScrubAmount - Time.deltaTime * maxScrubSpeed, targetScrubAmount);
			isScrubbing = true;
		}
		else
		{
			isScrubbing = false;
		}
		if (audioSource != null && slidingLoopSound != null && isScrubbing)
		{
			if (!isLoopingSoundPlaying)
			{
				audioSource.SetVolume(cachedOriginalVolumeOfLoopingSound);
				audioSource.Play();
				isLoopingSoundPlaying = true;
			}
			loopingSoundTimeRemaining = loopingSoundDecayTime;
		}
		if (flag)
		{
			float time = currentScrubAmount * scrubAnimation.clip.length;
			scrubAnimation[scrubAnimation.clip.name].enabled = true;
			scrubAnimation[scrubAnimation.clip.name].weight = 1f;
			scrubAnimation[scrubAnimation.clip.name].time = time;
			scrubAnimation.Sample();
			scrubAnimation[scrubAnimation.clip.name].enabled = false;
		}
	}

	private void SliderWasUnlockedUpper(OwlchemyVR2.GrabbableSlider slider)
	{
		GameEventsManager.Instance.ItemActionOccurred(worldItemToActivateOnUpper, "CLOSED");
		SliderWasUnlocked(slider);
	}

	private void SliderWasUnlockedLower(OwlchemyVR2.GrabbableSlider slider)
	{
		GameEventsManager.Instance.ItemActionOccurred(worldItemToActivateOnLower, "CLOSED");
		SliderWasUnlocked(slider);
	}

	private void SliderWasUnlocked(OwlchemyVR2.GrabbableSlider slider)
	{
		for (int i = 0; i < upperFoodContainers.Count; i++)
		{
			upperFoodContainers[i].ForceAllDoorsClosed();
		}
		for (int j = 0; j < lowerFoodContainers.Count; j++)
		{
			lowerFoodContainers[j].ForceAllDoorsClosed();
		}
	}

	private void ToggleSliderUpperLocked(OwlchemyVR2.GrabbableSlider slider, bool isInitial)
	{
		if (!isInitial)
		{
			for (int i = 0; i < upperFoodContainers.Count; i++)
			{
				upperFoodContainers[i].AllowOpening();
			}
			GameEventsManager.Instance.ItemActionOccurred(worldItemToActivateOnUpper, "OPENED");
		}
	}

	private void ToggleSliderLowerLocked(OwlchemyVR2.GrabbableSlider slider, bool isInitial)
	{
		if (!isInitial)
		{
			for (int i = 0; i < lowerFoodContainers.Count; i++)
			{
				lowerFoodContainers[i].AllowOpening();
			}
			GameEventsManager.Instance.ItemActionOccurred(worldItemToActivateOnLower, "OPENED");
		}
	}
}
