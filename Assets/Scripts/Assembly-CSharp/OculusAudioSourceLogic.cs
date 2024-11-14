using UnityEngine;
using UnityEngine.Audio;

public class OculusAudioSourceLogic : IAudioSourceLogic
{
	private ONSPAudioSource oculusAudioSrc;

	private AudioSource unityAudioSrc;

	public void Setup(ONSPAudioSource audioSrc, AudioSource unityAudioSrc)
	{
		oculusAudioSrc = audioSrc;
		this.unityAudioSrc = unityAudioSrc;
	}

	public void Play()
	{
		unityAudioSrc.timeSamples = 0;
		unityAudioSrc.Play();
	}

	public void Pause()
	{
		unityAudioSrc.Pause();
	}

	public void Unpause()
	{
		unityAudioSrc.UnPause();
	}

	public void Stop()
	{
		unityAudioSrc.Stop();
	}

	public void SetVolume(float volume)
	{
		unityAudioSrc.volume = volume;
	}

	public float GetVolume()
	{
		return unityAudioSrc.volume;
	}

	public void SetPitch(float pitch)
	{
		unityAudioSrc.pitch = pitch;
	}

	public float GetPitch()
	{
		return unityAudioSrc.pitch;
	}

	public void SetClip(AudioClip audioClip)
	{
		unityAudioSrc.clip = audioClip;
	}

	public AudioClip GetClip()
	{
		return unityAudioSrc.clip;
	}

	public void SetLoop(bool loop)
	{
		unityAudioSrc.loop = loop;
	}

	public bool GetLoop()
	{
		return unityAudioSrc.loop;
	}

	public void SetAudioMixerGroup(AudioMixerGroup audioMixerGroup)
	{
		unityAudioSrc.outputAudioMixerGroup = audioMixerGroup;
	}

	public AudioMixerGroup GetAudioMixerGroup()
	{
		return unityAudioSrc.outputAudioMixerGroup;
	}

	public bool IsPlaying()
	{
		return unityAudioSrc.isPlaying;
	}

	public bool IsPlayOnAwake()
	{
		return unityAudioSrc.playOnAwake;
	}

	public void SetEnableSpatialization(bool enableSpatialization)
	{
		oculusAudioSrc.EnableSpatialization = enableSpatialization;
		unityAudioSrc.spatialize = enableSpatialization;
	}

	public void ToggleEnable(bool enable)
	{
		oculusAudioSrc.enabled = enable;
		unityAudioSrc.enabled = enable;
	}
}
