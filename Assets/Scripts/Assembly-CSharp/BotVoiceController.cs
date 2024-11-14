using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotVoiceController : MonoBehaviour
{
	public enum VOImportance
	{
		OverrideAllSpeakers = 0,
		OverrideOnlySelf = 1,
		DontPlayIfAnyoneSpeaking = 2,
		DontPlayIfSelfSpeaking = 3
	}

	private static List<VOContainer> globalCurrentlyPlaying = new List<VOContainer>();

	[SerializeField]
	protected AudioSourceHelper audioSrcHelper;

	[SerializeField]
	protected AudioSourceHelper voiceDetectionAudioSrcHelper;

	public Action<BotVoiceController> OnAudioPlayComplete;

	public Action<VOContainer> OnVOWasPlayed;

	public Action<BotFaceEmote, Sprite> OnBotEmoteWasTriggered;

	public Action<BotVoiceController, Coroutine> OnAudioSequencePlayComplete;

	private VOContainer currentlyPlaying;

	private List<int> currentlyInvokingEvents = new List<int>();

	private bool isVoicePlayingInProgress;

	private Coroutine audioSequenceCoroutine;

	public AudioSource GetTalkingAudioSource()
	{
		return voiceDetectionAudioSrcHelper.GetComponent<AudioSource>();
	}

	public void PlayAudioDelay(AudioClip clip, float delay, VOImportance importance)
	{
		PlayAudioDelay(new VOContainer(clip, importance), delay);
	}

	public void PlayAudioDelay(BotVOInfoData info, float delay, VOImportance importance)
	{
		PlayAudioDelay(new VOContainer(info, importance), delay);
	}

	public void PlayAudioDelay(VOContainer vo, float delay)
	{
		if (base.gameObject.activeInHierarchy)
		{
			StartCoroutine(WaitAndPlayAudio(vo, delay));
		}
	}

	private IEnumerator WaitAndPlayAudio(VOContainer vo, float delay)
	{
		yield return new WaitForSeconds(delay);
		PlayAudio(vo);
	}

	public Coroutine PlayAudioSequence(AudioClip[] clips, VOImportance importance, BotVOEmoteEvent[] emotes = null)
	{
		VOContainer[] array = new VOContainer[clips.Length];
		for (int i = 0; i < clips.Length; i++)
		{
			array[i] = new VOContainer(clips[i], importance);
		}
		return PlayAudioSequence(array, emotes);
	}

	public Coroutine PlayAudioSequence(BotVOInfoData[] infos, VOImportance importance, BotVOEmoteEvent[] emotes = null)
	{
		VOContainer[] array = new VOContainer[infos.Length];
		for (int i = 0; i < infos.Length; i++)
		{
			array[i] = new VOContainer(infos[i], importance);
		}
		return PlayAudioSequence(array, emotes);
	}

	public Coroutine PlayAudioSequence(VOContainer[] vos, BotVOEmoteEvent[] emotes = null, float[] bufferTimes = null)
	{
		if (base.gameObject.activeInHierarchy)
		{
			if (audioSequenceCoroutine != null && vos != null && vos.Length > 0)
			{
				VOContainer vOContainer = vos[0];
				if (vOContainer.Importance == VOImportance.OverrideAllSpeakers)
				{
					CancelAllCurrentVO();
				}
				else if (vOContainer.Importance == VOImportance.OverrideOnlySelf)
				{
					CancelMyCurrentVO();
				}
				else if (vOContainer.Importance == VOImportance.DontPlayIfAnyoneSpeaking)
				{
					if (globalCurrentlyPlaying.Count != 0)
					{
						return null;
					}
				}
				else if (vOContainer.Importance == VOImportance.DontPlayIfSelfSpeaking && isVoicePlayingInProgress)
				{
					return null;
				}
			}
			audioSequenceCoroutine = StartCoroutine(PlayAudioSequenceAsync(vos, emotes, bufferTimes));
		}
		else
		{
			if (audioSequenceCoroutine != null)
			{
				StopCoroutine(audioSequenceCoroutine);
			}
			audioSequenceCoroutine = null;
			Debug.LogWarning("Attempting to play Audio Sequence on an inactive Bot Voice Controller", base.gameObject);
		}
		return audioSequenceCoroutine;
	}

	private IEnumerator PlayAudioSequenceAsync(VOContainer[] vos, BotVOEmoteEvent[] emotes, float[] bufferTimes = null)
	{
		for (int i = 0; i < vos.Length; i++)
		{
			if (vos[i] == null || !(vos[i].Clip != null))
			{
				continue;
			}
			while (vos[i].Clip.loadState != AudioDataLoadState.Loaded)
			{
				if (vos[i].Clip.loadState != AudioDataLoadState.Loading)
				{
					vos[i].Clip.LoadAudioData();
				}
				yield return null;
			}
			SetCurrentlyPlaying(vos[i]);
			if (vos[i].IsSimple && emotes != null && i < emotes.Length && emotes[i] != null && OnBotEmoteWasTriggered != null)
			{
				OnBotEmoteWasTriggered(emotes[i].Emote, emotes[i].CustomGraphic);
			}
			yield return new WaitForSeconds(vos[i].Clip.length);
			while (audioSrcHelper.IsPlaying || voiceDetectionAudioSrcHelper.IsPlaying)
			{
				yield return null;
			}
			vos[i].Clip.UnloadAudioData();
			if (emotes != null && emotes.Length > i && OnBotEmoteWasTriggered != null)
			{
				OnBotEmoteWasTriggered(BotFaceEmote.Idle, emotes[i].CustomGraphic);
			}
			if (bufferTimes != null && i < bufferTimes.Length && bufferTimes[i] > 0f)
			{
				int sampleRate = 11025;
				AudioClip c = AudioClip.Create("silence", (int)((float)sampleRate * bufferTimes[i]), 1, sampleRate, false);
				SetCurrentlyPlaying(new VOContainer(c, vos[i].Importance));
				yield return new WaitForSeconds(c.length);
				while (audioSrcHelper.IsPlaying || voiceDetectionAudioSrcHelper.IsPlaying)
				{
					yield return null;
				}
				UnityEngine.Object.Destroy(c);
			}
		}
		if (OnAudioSequencePlayComplete != null)
		{
			OnAudioSequencePlayComplete(this, audioSequenceCoroutine);
		}
		audioSequenceCoroutine = null;
	}

	public void PlayAudio(AudioClip clip, VOImportance importance)
	{
		PlayAudio(new VOContainer(clip, importance));
	}

	public void PlayAudio(BotVOInfoData info, VOImportance importance)
	{
		PlayAudio(new VOContainer(info, importance));
	}

	public void PlayAudio(VOContainer vo)
	{
		InternalPlay(vo);
	}

	private void InternalPlay(VOContainer vo)
	{
		if (vo.Importance == VOImportance.OverrideAllSpeakers)
		{
			CancelAllCurrentVO();
			SetCurrentlyPlaying(vo);
		}
		else if (vo.Importance == VOImportance.OverrideOnlySelf)
		{
			CancelMyCurrentVO();
			SetCurrentlyPlaying(vo);
		}
		else if (vo.Importance == VOImportance.DontPlayIfAnyoneSpeaking)
		{
			if (globalCurrentlyPlaying.Count == 0)
			{
				SetCurrentlyPlaying(vo);
			}
		}
		else if (vo.Importance == VOImportance.DontPlayIfSelfSpeaking && !isVoicePlayingInProgress)
		{
			SetCurrentlyPlaying(vo);
		}
	}

	private void SetCurrentlyPlaying(VOContainer vo)
	{
		if (vo == null)
		{
			return;
		}
		if (!globalCurrentlyPlaying.Contains(vo))
		{
			globalCurrentlyPlaying.Add(vo);
		}
		currentlyPlaying = vo;
		if (currentlyPlaying == null)
		{
			return;
		}
		isVoicePlayingInProgress = true;
		audioSrcHelper.SetClip(vo.Clip);
		voiceDetectionAudioSrcHelper.SetClip(vo.Clip);
		TimeManager.Invoke(audioSrcHelper.Play, 1f / 90f);
		voiceDetectionAudioSrcHelper.Play();
		VOContainer vOContainer = currentlyPlaying;
		vOContainer.OnWasRemotelyCancelled = (Action)Delegate.Combine(vOContainer.OnWasRemotelyCancelled, new Action(VOWasRemotelyCancelled));
		if (!currentlyPlaying.IsSimple && currentlyPlaying.Info != null)
		{
			for (int i = 0; i < currentlyPlaying.Info.Events.Count; i++)
			{
				BotVOEmoteEvent botVOEmoteEvent = currentlyPlaying.Info.Events[i];
				TimeManager.Invoke(TriggerVOEmoteEvent, i, botVOEmoteEvent.Time);
				currentlyInvokingEvents.Add(i);
			}
		}
		if (OnVOWasPlayed != null)
		{
			OnVOWasPlayed(vo);
		}
		TimeManager.Invoke(VoicePlayComplete, vo.Clip.length);
	}

	private void TriggerVOEmoteEvent(int i)
	{
		if (currentlyPlaying == null || !currentlyInvokingEvents.Contains(i))
		{
			return;
		}
		if (currentlyPlaying.Info != null)
		{
			if (i >= 0 && i < currentlyPlaying.Info.Events.Count)
			{
				BotVOEmoteEvent botVOEmoteEvent = currentlyPlaying.Info.Events[i];
				if (OnBotEmoteWasTriggered != null)
				{
					OnBotEmoteWasTriggered(botVOEmoteEvent.Emote, botVOEmoteEvent.CustomGraphic);
				}
			}
			else
			{
				Debug.LogWarning("TriggerVOEmoteEvent fired with an invalid index of " + i + ", when currentlyPlaying had " + currentlyPlaying.Info.Events.Count + " events. (" + currentlyPlaying.Clip.name + ")");
			}
		}
		else
		{
			Debug.LogWarning("TriggerVOEmoteEvent fired even though currentlyPlaying (" + currentlyPlaying.Clip.name + ") doesn't use BotVOInfoData");
		}
		currentlyInvokingEvents.Remove(i);
	}

	private void VoicePlayComplete()
	{
		if (currentlyPlaying != null)
		{
			VOContainer vOContainer = currentlyPlaying;
			vOContainer.OnWasRemotelyCancelled = (Action)Delegate.Remove(vOContainer.OnWasRemotelyCancelled, new Action(VOWasRemotelyCancelled));
			if (globalCurrentlyPlaying.Contains(currentlyPlaying))
			{
				globalCurrentlyPlaying.Remove(currentlyPlaying);
			}
			currentlyPlaying = null;
		}
		isVoicePlayingInProgress = false;
		if (OnAudioPlayComplete != null)
		{
			OnAudioPlayComplete(this);
		}
	}

	private void VOWasRemotelyCancelled()
	{
		CancelMyCurrentVO();
	}

	public void CancelMyCurrentVO()
	{
		if (audioSequenceCoroutine != null)
		{
			StopCoroutine(audioSequenceCoroutine);
			audioSequenceCoroutine = null;
		}
		if (isVoicePlayingInProgress)
		{
			TimeManager.CancelInvoke(VoicePlayComplete);
			isVoicePlayingInProgress = false;
		}
		for (int i = 0; i < currentlyInvokingEvents.Count; i++)
		{
			TimeManager.CancelInvoke(TriggerVOEmoteEvent, currentlyInvokingEvents[i]);
		}
		currentlyInvokingEvents.Clear();
		if (OnBotEmoteWasTriggered != null)
		{
			OnBotEmoteWasTriggered(BotFaceEmote.Idle, null);
		}
		if (currentlyPlaying != null)
		{
			if (currentlyPlaying.Info != null && currentlyPlaying.Info.Events.Count > 0)
			{
				BotVOEmoteEvent botVOEmoteEvent = currentlyPlaying.Info.Events[currentlyPlaying.Info.Events.Count - 1];
				if (OnBotEmoteWasTriggered != null)
				{
					OnBotEmoteWasTriggered(botVOEmoteEvent.Emote, botVOEmoteEvent.CustomGraphic);
				}
			}
			VOContainer vOContainer = currentlyPlaying;
			vOContainer.OnWasRemotelyCancelled = (Action)Delegate.Remove(vOContainer.OnWasRemotelyCancelled, new Action(VOWasRemotelyCancelled));
			if (globalCurrentlyPlaying.Contains(currentlyPlaying))
			{
				globalCurrentlyPlaying.Remove(currentlyPlaying);
			}
			currentlyPlaying = null;
		}
		if (audioSrcHelper.IsPlaying)
		{
			audioSrcHelper.Stop();
			voiceDetectionAudioSrcHelper.Stop();
		}
	}

	public static void CancelAllCurrentVO()
	{
		if (globalCurrentlyPlaying != null)
		{
			for (int i = 0; i < globalCurrentlyPlaying.Count; i++)
			{
				globalCurrentlyPlaying[i].Cancel();
			}
		}
		globalCurrentlyPlaying.Clear();
	}

	public static void ClearAllGlobalCurrentlyPlaying()
	{
		globalCurrentlyPlaying.Clear();
	}
}
