using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
	public enum AudioSystemTypes
	{
		UnityAudioSource = 0,
		TBEAudioSource = 1,
		OculusAudio = 2
	}

	private const int NUM_FRAME_DIVIDE_DEACTIVATION_AUDIO_SOURCES_ACROSS = 5;

	private static readonly AudioSystemTypes audioSystemType = AudioSystemTypes.OculusAudio;

	private List<AudioSourceHelper> permanentAudioSourceList;

	private List<AudioSourceHelper> masterDynamicAudioSourceList;

	private List<AudioSourceHelper> dynamicAudioSourceHelperPool;

	private List<AudioSourceHelper> pausedAudioSources;

	private int initalAudioSourcesCount = 20;

	private static float volumeMasterPercentage = 1f;

	private int lastFrameIndexDynamicAudioSourceHelperDeactivator;

	private int deactivatorFrameCount;

	private int tempNumCheckedThisFrame;

	private AudioMixerGroup _micMixerGroup;

	public static AudioSystemTypes AudioSystemType
	{
		get
		{
			return audioSystemType;
		}
	}

	public bool IsAudioPaused { get; private set; }

	public AudioMixerSettings AudioMixerSettings { get; private set; }

	public AudioMixerGroup MicMixerGroup
	{
		get
		{
			return _micMixerGroup;
		}
		set
		{
			_micMixerGroup = value;
		}
	}

	public MicrophoneManager MicrophoneManager { get; private set; }

	public static AudioManager Instance
	{
		get
		{
			if (_noCreateInstance == null)
			{
				_noCreateInstance = Object.FindObjectOfType(typeof(AudioManager)) as AudioManager;
				if (_noCreateInstance == null)
				{
					_noCreateInstance = new GameObject("_AudioManager").AddComponent<AudioManager>();
					_noCreateInstance.Setup();
				}
			}
			return _noCreateInstance;
		}
	}

	public static AudioManager _noCreateInstance { get; private set; }

	public AudioManager()
	{
		IsAudioPaused = false;
	}

	private void Awake()
	{
		if (_noCreateInstance == null)
		{
			_noCreateInstance = this;
			Object.DontDestroyOnLoad(base.gameObject);
		}
		else if (_noCreateInstance != this)
		{
			Object.Destroy(base.gameObject);
		}
	}

	private void Setup()
	{
		masterDynamicAudioSourceList = new List<AudioSourceHelper>();
		dynamicAudioSourceHelperPool = new List<AudioSourceHelper>();
		permanentAudioSourceList = new List<AudioSourceHelper>();
		AudioMixerSettings = Resources.Load("AudioMixerSettings", typeof(AudioMixerSettings)) as AudioMixerSettings;
		if (AudioMixerSettings == null)
		{
			Debug.LogError("Missing Audio Mixer Settings in Resources");
		}
		for (int i = 0; i < initalAudioSourcesCount; i++)
		{
			GenerateAdditionalCustomAudioSourceHelper();
		}
		GameObject gameObject = new GameObject();
		gameObject.name = "MicrophoneManager";
		gameObject.transform.parent = base.transform;
		MicrophoneManager = gameObject.AddComponent<MicrophoneManager>();
	}

	public void NewSceneLoaded(string sceneName)
	{
		Debug.Log("New Scene Loaded");
		AudioMixerSettings.SetSnapshotBySceneName(sceneName);
	}

	public void SetMasterVolumePercentage(float volumePercent)
	{
		volumeMasterPercentage = volumePercent;
		MasterVolumeChange();
	}

	public static float GetMasterVolumePercentage()
	{
		return volumeMasterPercentage;
	}

	public void AddPermanentAudioSourceHelper(AudioSourceHelper audioSrcHelper)
	{
		permanentAudioSourceList.Add(audioSrcHelper);
	}

	public void RemovePermanentAudioSourceHelper(AudioSourceHelper audioSrcHelper)
	{
		permanentAudioSourceList.Remove(audioSrcHelper);
	}

	private AudioSourceHelper GenerateAdditionalCustomAudioSourceHelper()
	{
		AudioSourceHelper audioSourceHelper = CreateCustomAudioSourceHelper();
		masterDynamicAudioSourceList.Add(audioSourceHelper);
		dynamicAudioSourceHelperPool.Add(audioSourceHelper);
		return audioSourceHelper;
	}

	private AudioSourceHelper GetAvailableAudioSourceHelperAuto(Transform followTarget)
	{
		AudioSourceHelper audioSourceHelper = null;
		for (int i = 0; i < dynamicAudioSourceHelperPool.Count; i++)
		{
			if (!dynamicAudioSourceHelperPool[i].IsPlaying)
			{
				audioSourceHelper = dynamicAudioSourceHelperPool[i];
				break;
			}
		}
		if (audioSourceHelper == null)
		{
			audioSourceHelper = GenerateAdditionalCustomAudioSourceHelper();
		}
		if (followTarget != null)
		{
			audioSourceHelper.BeginFollow(followTarget);
		}
		else
		{
			audioSourceHelper.StopFollow();
			audioSourceHelper.transform.localPosition = Vector3.zero;
		}
		audioSourceHelper.enabled = true;
		return audioSourceHelper;
	}

	public void PlayWithAudioSrcHelper(AudioSourceHelper audioSrcHelper, AudioClip clip, float volume = 1f, float pitch = 1f, bool loop = false)
	{
		PlayCustomAudioInternal(audioSrcHelper, clip, volume, pitch, loop);
	}

	public void Play(Vector3 soundPos, AudioClip clip, float volume = 1f, float pitch = 1f)
	{
		AudioSourceHelper availableAudioSourceHelperAuto = GetAvailableAudioSourceHelperAuto(null);
		availableAudioSourceHelperAuto.transform.position = soundPos;
		PlayCustomAudioInternal(availableAudioSourceHelperAuto, clip, volume, pitch, false);
	}

	public AudioSourceHelper PlayLooping(Vector3 soundPos, AudioClip clip, float volume = 1f, float pitch = 1f)
	{
		AudioSourceHelper availableAudioSourceHelperAuto = GetAvailableAudioSourceHelperAuto(null);
		availableAudioSourceHelperAuto.transform.position = soundPos;
		PlayCustomAudioInternal(availableAudioSourceHelperAuto, clip, volume, pitch, true);
		return availableAudioSourceHelperAuto;
	}

	public void Play(Transform followTarget, AudioClip clip, float volume = 1f, float pitch = 1f)
	{
		AudioSourceHelper availableAudioSourceHelperAuto = GetAvailableAudioSourceHelperAuto(followTarget);
		PlayCustomAudioInternal(availableAudioSourceHelperAuto, clip, volume, pitch, false);
	}

	public AudioSourceHelper PlayLooping(Transform followTarget, AudioClip clip, float volume = 1f, float pitch = 1f)
	{
		AudioSourceHelper availableAudioSourceHelperAuto = GetAvailableAudioSourceHelperAuto(followTarget);
		PlayCustomAudioInternal(availableAudioSourceHelperAuto, clip, volume, pitch, true);
		return availableAudioSourceHelperAuto;
	}

	public void Play2D(AudioClip clip)
	{
		Play2D(clip, 1f, 1f);
	}

	public void Play2D(AudioClip clip, float volume, float pitch)
	{
		Play(null, clip, volume, pitch);
	}

	public void Stop2D(AudioClip clip)
	{
		Stop(null, clip);
	}

	public void Stop(Transform followTarget, AudioClip clip)
	{
		if (clip == null)
		{
			return;
		}
		for (int i = 0; i < dynamicAudioSourceHelperPool.Count; i++)
		{
			if (dynamicAudioSourceHelperPool[i].IsFollowTargetAndClipEqual(followTarget, clip) && dynamicAudioSourceHelperPool[i].IsPlaying)
			{
				dynamicAudioSourceHelperPool[i].Stop();
				break;
			}
		}
	}

	private AudioSourceHelper PlayCustomAudioInternal(AudioSourceHelper audioSrcHelper, AudioClip clip, float volume, float pitch, bool loop)
	{
		audioSrcHelper.SetupAudioSource(clip, volume, pitch, loop);
		audioSrcHelper.Play();
		return audioSrcHelper;
	}

	private AudioSourceHelper CreateCustomAudioSourceHelper()
	{
		GameObject gameObject = new GameObject("audioSrc");
		Object.DontDestroyOnLoad(gameObject);
		AudioSourceHelper audioSourceHelper = gameObject.AddComponent<AudioSourceHelper>();
		audioSourceHelper.SetAsDynamic();
		audioSourceHelper.enabled = false;
		gameObject.transform.parent = base.transform;
		return audioSourceHelper;
	}

	public void PauseAllSounds()
	{
		if (IsAudioPaused)
		{
			return;
		}
		IsAudioPaused = true;
		if (pausedAudioSources == null)
		{
			pausedAudioSources = new List<AudioSourceHelper>();
		}
		foreach (AudioSourceHelper permanentAudioSource in permanentAudioSourceList)
		{
			if (permanentAudioSource.IsPlaying)
			{
				pausedAudioSources.Add(permanentAudioSource);
				permanentAudioSource.Pause();
			}
		}
		foreach (AudioSourceHelper masterDynamicAudioSource in masterDynamicAudioSourceList)
		{
			if (masterDynamicAudioSource.IsPlaying)
			{
				pausedAudioSources.Add(masterDynamicAudioSource);
				masterDynamicAudioSource.Pause();
			}
		}
	}

	public void UnPauseAllSounds()
	{
		if (!IsAudioPaused)
		{
			return;
		}
		IsAudioPaused = false;
		foreach (AudioSourceHelper pausedAudioSource in pausedAudioSources)
		{
			pausedAudioSource.UnPause();
		}
		pausedAudioSources.Clear();
	}

	private void MasterVolumeChange()
	{
		foreach (AudioSourceHelper permanentAudioSource in permanentAudioSourceList)
		{
			if (permanentAudioSource.IsPlaying)
			{
				permanentAudioSource.UpdateVolume();
			}
		}
		foreach (AudioSourceHelper masterDynamicAudioSource in masterDynamicAudioSourceList)
		{
			if (masterDynamicAudioSource.IsPlaying)
			{
				masterDynamicAudioSource.UpdateVolume();
			}
		}
	}

	private void Update()
	{
		for (int i = 0; i < dynamicAudioSourceHelperPool.Count; i++)
		{
			if (!dynamicAudioSourceHelperPool[i].IsPlaying && dynamicAudioSourceHelperPool[i].EnableSpatialization)
			{
				dynamicAudioSourceHelperPool[i].enabled = false;
			}
		}
	}
}
