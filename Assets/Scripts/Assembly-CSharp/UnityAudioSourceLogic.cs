using UnityEngine;
using UnityEngine.Audio;

public class UnityAudioSourceLogic : IAudioSourceLogic
{
	private AudioSource audioSrc;

	public void Setup(AudioSource audioSrc)
	{
		this.audioSrc = audioSrc;
	}

	public void Play()
	{
		audioSrc.timeSamples = 0;
		audioSrc.Play();
	}

	public void Pause()
	{
		audioSrc.Pause();
	}

	public void Unpause()
	{
		audioSrc.UnPause();
	}

	public void Stop()
	{
		audioSrc.Stop();
	}

	public void SetVolume(float volume)
	{
		audioSrc.volume = volume;
	}

	public float GetVolume()
	{
		return audioSrc.volume;
	}

	public void SetPitch(float pitch)
	{
		audioSrc.pitch = pitch;
	}

	public float GetPitch()
	{
		return audioSrc.pitch;
	}

	public void SetClip(AudioClip audioClip)
	{
		audioSrc.clip = audioClip;
	}

	public AudioClip GetClip()
	{
		return audioSrc.clip;
	}

	public void SetLoop(bool loop)
	{
		audioSrc.loop = loop;
	}

	public bool GetLoop()
	{
		return audioSrc.loop;
	}

	public void SetAudioMixerGroup(AudioMixerGroup audioMixerGroup)
	{
		audioSrc.outputAudioMixerGroup = audioMixerGroup;
	}

	public AudioMixerGroup GetAudioMixerGroup()
	{
		return audioSrc.outputAudioMixerGroup;
	}

	public bool IsPlaying()
	{
		return audioSrc.isPlaying;
	}

	public bool IsPlayOnAwake()
	{
		return audioSrc.playOnAwake;
	}

	public void SetEnableSpatialization(bool enableSpatialization)
	{
		audioSrc.spatialize = enableSpatialization;
	}

	public void ToggleEnable(bool enable)
	{
		audioSrc.enabled = enable;
	}
}
