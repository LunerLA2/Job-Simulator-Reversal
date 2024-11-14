using System;
using System.Collections;
using System.Collections.Generic;
using OwlchemyVR;
using UnityEngine;

public class ParticleImpactEffect : MonoBehaviour
{
	private const float EFFECT_DISTANCE_THRESHOLD = 0.05f;

	[SerializeField]
	private ParticleImpactZone particleImpactZone;

	[SerializeField]
	private ParticleSystem effectPrefab;

	[SerializeField]
	private ParticleImpactEffectDefinition[] specificFluidEffects;

	[SerializeField]
	private float timeBeforeHidingEffect = 1f;

	[SerializeField]
	private AudioSourceHelper optionalLoopingImpactSoundSource;

	private float loopingImpactSoundTimeRemaing;

	private bool isLoopingImpactSoundPlaying;

	private float loopingImpactSoundDecayTime = 1f;

	private float cachedOriginalVolumeOfLoopingSound = 1f;

	private List<ParticleSystem> activeEffects = new List<ParticleSystem>();

	private ObjectPool<ParticleSystem> effectPool;

	private Dictionary<string, ObjectPool<ParticleSystem>> specificEffectPools = new Dictionary<string, ObjectPool<ParticleSystem>>();

	private Dictionary<string, List<ParticleSystem>> specificActiveEffects = new Dictionary<string, List<ParticleSystem>>();

	private void Awake()
	{
		if (optionalLoopingImpactSoundSource != null)
		{
			cachedOriginalVolumeOfLoopingSound = optionalLoopingImpactSoundSource.DefaultStartingVolume;
		}
		int num = 5;
		effectPool = new ObjectPool<ParticleSystem>(effectPrefab, num, false, true, base.transform, Vector3.zero);
		List<ParticleSystem> list = new List<ParticleSystem>();
		for (int i = 0; i < num; i++)
		{
			ParticleSystem particleSystem = effectPool.Fetch();
			ParticleSystem.EmissionModule emission = particleSystem.emission;
			emission.enabled = false;
			list.Add(particleSystem);
		}
		for (int j = 0; j < list.Count; j++)
		{
			effectPool.Release(list[j]);
		}
		if (specificFluidEffects == null)
		{
			return;
		}
		for (int k = 0; k < specificFluidEffects.Length; k++)
		{
			ObjectPool<ParticleSystem> objectPool = new ObjectPool<ParticleSystem>(specificFluidEffects[k].EffectPrefab, num, false, true, base.transform, Vector3.zero);
			specificEffectPools[specificFluidEffects[k].FluidData.name] = objectPool;
			specificActiveEffects[specificFluidEffects[k].FluidData.name] = new List<ParticleSystem>();
			list.Clear();
			for (int l = 0; l < num; l++)
			{
				ParticleSystem particleSystem2 = objectPool.Fetch();
				ParticleSystem.EmissionModule emission = particleSystem2.emission;
				emission.enabled = false;
				list.Add(particleSystem2);
			}
			for (int m = 0; m < list.Count; m++)
			{
				objectPool.Release(list[m]);
			}
		}
	}

	private void OnEnable()
	{
		ParticleImpactZone obj = particleImpactZone;
		obj.OnSpecificParticleAppliedUpdate = (Action<ParticleImpactZone, WorldItemData, Vector3>)Delegate.Combine(obj.OnSpecificParticleAppliedUpdate, new Action<ParticleImpactZone, WorldItemData, Vector3>(SpecificFluidPouredOnAtPosition));
		ParticleImpactZone obj2 = particleImpactZone;
		obj2.OnAnyParticleAppliedUpdate = (Action<ParticleImpactZone, Vector3>)Delegate.Combine(obj2.OnAnyParticleAppliedUpdate, new Action<ParticleImpactZone, Vector3>(AnyParticleAppliedUpdate));
	}

	private void OnDisable()
	{
		ParticleImpactZone obj = particleImpactZone;
		obj.OnSpecificParticleAppliedUpdate = (Action<ParticleImpactZone, WorldItemData, Vector3>)Delegate.Remove(obj.OnSpecificParticleAppliedUpdate, new Action<ParticleImpactZone, WorldItemData, Vector3>(SpecificFluidPouredOnAtPosition));
		ParticleImpactZone obj2 = particleImpactZone;
		obj2.OnAnyParticleAppliedUpdate = (Action<ParticleImpactZone, Vector3>)Delegate.Remove(obj2.OnAnyParticleAppliedUpdate, new Action<ParticleImpactZone, Vector3>(AnyParticleAppliedUpdate));
	}

	private void AnyParticleAppliedUpdate(ParticleImpactZone zone, Vector3 impactPosition)
	{
		if (optionalLoopingImpactSoundSource != null)
		{
			optionalLoopingImpactSoundSource.transform.position = impactPosition;
			if (!isLoopingImpactSoundPlaying)
			{
				optionalLoopingImpactSoundSource.SetVolume(cachedOriginalVolumeOfLoopingSound);
				optionalLoopingImpactSoundSource.Play();
				isLoopingImpactSoundPlaying = true;
			}
			loopingImpactSoundTimeRemaing = loopingImpactSoundDecayTime;
		}
	}

	private void Update()
	{
		if (isLoopingImpactSoundPlaying)
		{
			loopingImpactSoundTimeRemaing -= Time.deltaTime;
			if (loopingImpactSoundTimeRemaing <= 0f)
			{
				optionalLoopingImpactSoundSource.Stop();
				isLoopingImpactSoundPlaying = false;
			}
			else
			{
				float num = loopingImpactSoundTimeRemaing / loopingImpactSoundDecayTime;
				optionalLoopingImpactSoundSource.SetVolume(num * cachedOriginalVolumeOfLoopingSound);
			}
		}
	}

	private void SpecificFluidPouredOnAtPosition(ParticleImpactZone zone, WorldItemData data, Vector3 position)
	{
		bool flag = false;
		ParticleSystem particleSystem = null;
		ObjectPool<ParticleSystem> objectPool = effectPool;
		List<ParticleSystem> list = activeEffects;
		for (int i = 0; i < specificFluidEffects.Length; i++)
		{
			if (specificFluidEffects[i].FluidData == data)
			{
				particleSystem = GetActiveEffectWithinDistanceFromList(position, specificActiveEffects[data.name]);
				objectPool = specificEffectPools[data.name];
				list = specificActiveEffects[data.name];
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			particleSystem = GetActiveEffectWithinDistanceFromList(position, list);
		}
		if (particleSystem != null)
		{
			particleSystem.transform.position = position;
			return;
		}
		ParticleSystem particleSystem2 = objectPool.Fetch(position, Quaternion.identity);
		ParticleSystem.EmissionModule emission = particleSystem2.emission;
		emission.enabled = true;
		list.Add(particleSystem2);
		StartCoroutine(HideEffectAfterDelay(particleSystem2, list, objectPool));
	}

	private IEnumerator HideEffectAfterDelay(ParticleSystem effect, List<ParticleSystem> listToUse, ObjectPool<ParticleSystem> poolToUse)
	{
		yield return new WaitForSeconds(timeBeforeHidingEffect);
		if (listToUse.Contains(effect))
		{
			listToUse.Remove(effect);
		}
		poolToUse.Release(effect);
		ParticleSystem.EmissionModule em = effect.emission;
		em.enabled = false;
	}

	private ParticleSystem GetActiveEffectWithinDistanceFromList(Vector3 pos, List<ParticleSystem> list)
	{
		for (int i = 0; i < list.Count; i++)
		{
			if (Vector3.Distance(pos, list[i].transform.position) <= 0.05f)
			{
				return list[i];
			}
		}
		return null;
	}
}
