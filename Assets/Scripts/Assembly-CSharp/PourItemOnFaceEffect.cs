using System;
using System.Collections;
using System.Collections.Generic;
using OwlchemyVR;
using UnityEngine;

[Serializable]
public class PourItemOnFaceEffect
{
	[SerializeField]
	private List<WorldItemData> worldItemsThatTrigger = new List<WorldItemData>();

	public List<AudioClip> soundsToPlay = new List<AudioClip>();

	private ElementSequence<AudioClip> soundsToPlaySequence;

	public Material chunkMaterial;

	public Material sprayMaterial;

	private bool isBarfing;

	public List<WorldItemData> WorldItemsThatTrigger
	{
		get
		{
			return worldItemsThatTrigger;
		}
	}

	public bool IsTriggeredBy(WorldItemData wi)
	{
		for (int i = 0; i < worldItemsThatTrigger.Count; i++)
		{
			if (wi == worldItemsThatTrigger[i])
			{
				return true;
			}
		}
		return false;
	}

	public IEnumerator DoEffect(AudioSourceHelper pouredItemOnFaceAudioSourceHelper, ParticleSystem chunkParticles, ParticleSystem sprayParticles)
	{
		if (isBarfing)
		{
			yield break;
		}
		isBarfing = true;
		AnalyticsManager.CustomEvent("Barf", "Time", chunkParticles.duration);
		if (chunkMaterial != null)
		{
			chunkParticles.GetComponent<Renderer>().material = chunkMaterial;
		}
		else
		{
			Debug.LogWarning("Chunk Null");
		}
		if (sprayMaterial != null)
		{
			sprayParticles.GetComponent<Renderer>().material = sprayMaterial;
		}
		else
		{
			Debug.LogWarning("Spray Null");
		}
		chunkParticles.Play();
		sprayParticles.Play();
		if (soundsToPlay.Count > 0)
		{
			if (soundsToPlaySequence == null)
			{
				soundsToPlaySequence = new ElementSequence<AudioClip>(soundsToPlay.ToArray());
			}
			AudioClip clip = soundsToPlaySequence.GetNext();
			pouredItemOnFaceAudioSourceHelper.SetClip(clip);
			pouredItemOnFaceAudioSourceHelper.Play();
		}
		yield return new WaitForSeconds(chunkParticles.duration);
		isBarfing = false;
	}
}
