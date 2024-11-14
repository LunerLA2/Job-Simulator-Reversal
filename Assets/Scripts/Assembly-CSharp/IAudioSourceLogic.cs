using UnityEngine;
using UnityEngine.Audio;

public interface IAudioSourceLogic
{
	void Play();

	void Pause();

	void Unpause();

	void Stop();

	void SetVolume(float volume);

	float GetVolume();

	void SetPitch(float pitch);

	float GetPitch();

	void SetClip(AudioClip audioClip);

	AudioClip GetClip();

	void SetLoop(bool loop);

	bool GetLoop();

	void SetAudioMixerGroup(AudioMixerGroup audioMixerGroup);

	AudioMixerGroup GetAudioMixerGroup();

	bool IsPlaying();

	void SetEnableSpatialization(bool enableSpatialization);

	void ToggleEnable(bool enable);

	bool IsPlayOnAwake();
}
