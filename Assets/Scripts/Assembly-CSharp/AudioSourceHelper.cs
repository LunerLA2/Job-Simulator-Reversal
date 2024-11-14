using System;
using System.Collections;
using UnityEngine;

public class AudioSourceHelper : MonoBehaviour
{
	private enum FadeTypes
	{
		None = 0,
		FadeIn = 1,
		FadeOut = 2
	}

	private bool isPermanent = true;

	private bool isAddedToAudioManager;

	protected IAudioSourceLogic audioSrcLogic;

	private IAudioSourceLogic crossFadeOutAudioSrcLogic;

	private Transform followTarget;

	private bool isFollowLoopInProgress;

	public Action<AudioSourceHelper> OnFadeInComplete;

	public Action<AudioSourceHelper> OnFadeOutComplete;

	private FadeTypes fadeType;

	private float audioVolume = -1f;

	private float fadeTimePassed;

	private float timeToFade;

	private float crossFadeTimePassed;

	private float crossFadeTimeToFade;

	private bool hasAudioVolumeBeenSet;

	[SerializeField]
	private bool enableSpatialization = true;

	[SerializeField]
	private bool disableReflections = true;

	[Range(0f, 24f)]
	[SerializeField]
	private float gain;

	[SerializeField]
	private bool inverseSquareAttenuation;

	[Range(0f, 5000f)]
	[SerializeField]
	private float near = 1f;

	[Range(0f, 5000f)]
	[SerializeField]
	private float far = 10f;

	[SerializeField]
	private bool playOnStart;

	[SerializeField]
	private AudioClip defaultAudioClip;

	[SerializeField]
	private bool defaultIsLooping;

	[SerializeField]
	[Range(0f, 1f)]
	private float defaultStartingVolume = 1f;

	[SerializeField]
	private bool useAlreadyCreatedAudioSource;

	[SerializeField]
	private bool isBypass;

	[SerializeField]
	private bool useAudioSourceOnly;

	public float DefaultStartingVolume
	{
		get
		{
			return defaultStartingVolume;
		}
	}

	public bool IsPlaying
	{
		get
		{
			return audioSrcLogic != null && audioSrcLogic.IsPlaying();
		}
	}

	public bool IsFading
	{
		get
		{
			return fadeType != FadeTypes.None;
		}
	}

	public bool IsFadingIn
	{
		get
		{
			return fadeType == FadeTypes.FadeIn;
		}
	}

	public bool IsFadingOut
	{
		get
		{
			return fadeType == FadeTypes.FadeOut;
		}
	}

	public Transform FollowTarget
	{
		get
		{
			return followTarget;
		}
	}

	public bool EnableSpatialization
	{
		get
		{
			return enableSpatialization;
		}
		set
		{
			enableSpatialization = value;
			audioSrcLogic.SetEnableSpatialization(enableSpatialization);
		}
	}

	public virtual void Awake()
	{
		if (enableSpatialization == isBypass && isBypass)
		{
			Debug.LogWarning("Bypass will soon not be supported use enable spatialization instead, overriding enable spatalization", base.gameObject);
			enableSpatialization = !isBypass;
		}
		if (useAlreadyCreatedAudioSource)
		{
			GetAlreadyCreatedAudioSource();
		}
		else
		{
			CreateAudioSource();
		}
		if (defaultAudioClip != null)
		{
			SetClip(defaultAudioClip);
		}
		if (defaultIsLooping)
		{
			SetLooping(true);
		}
		if (defaultStartingVolume != 1f)
		{
			hasAudioVolumeBeenSet = true;
			audioVolume = defaultStartingVolume;
		}
		audioSrcLogic.Stop();
	}

	public void SetAsDynamic()
	{
		isPermanent = false;
		isAddedToAudioManager = true;
	}

	public void SetCrossFadeOutAudioSrc(IAudioSourceLogic audioSrcLogic)
	{
		crossFadeOutAudioSrcLogic = audioSrcLogic;
	}

	public void Unpause()
	{
		if (!hasAudioVolumeBeenSet)
		{
			audioVolume = audioSrcLogic.GetVolume();
			hasAudioVolumeBeenSet = true;
		}
		UpdateVolume();
		audioSrcLogic.Unpause();
	}

	public void Play()
	{
		if (audioSrcLogic.GetClip() == null)
		{
			Debug.LogWarning("Attempting to play audio source without a clip");
			return;
		}
		if (!hasAudioVolumeBeenSet)
		{
			audioVolume = audioSrcLogic.GetVolume();
			hasAudioVolumeBeenSet = true;
		}
		UpdateVolume();
		audioSrcLogic.SetEnableSpatialization(!enableSpatialization);
		audioSrcLogic.SetEnableSpatialization(enableSpatialization);
		audioSrcLogic.Play();
	}

	public void UnPause()
	{
		Unpause();
	}

	public void Stop()
	{
		audioSrcLogic.Stop();
	}

	public void Pause()
	{
		audioSrcLogic.Pause();
	}

	public void BeginFollow(Transform target)
	{
		followTarget = target;
		if (!isFollowLoopInProgress)
		{
			if (followTarget != null)
			{
				StartCoroutine(FollowLoop());
			}
		}
		else if (followTarget != null)
		{
			base.transform.position = followTarget.position;
		}
	}

	public void StopFollow()
	{
		if (isFollowLoopInProgress)
		{
			StopAllCoroutines();
			isFollowLoopInProgress = false;
			followTarget = null;
		}
	}

	private IEnumerator FollowLoop()
	{
		isFollowLoopInProgress = true;
		YieldInstruction delay = new WaitForSeconds(0.1f);
		do
		{
			if (followTarget != null)
			{
				base.transform.position = followTarget.position;
			}
			yield return delay;
		}
		while (audioSrcLogic.IsPlaying());
		isFollowLoopInProgress = false;
	}

	private void OnEnable()
	{
		audioSrcLogic.ToggleEnable(true);
		if (!isAddedToAudioManager && audioSrcLogic != null && isPermanent)
		{
			AudioManager.Instance.AddPermanentAudioSourceHelper(this);
			if (audioSrcLogic.IsPlayOnAwake() && !hasAudioVolumeBeenSet && audioSrcLogic.GetClip() != null)
			{
				Debug.LogWarning("Avoid using playOnAwake on the audiosrc because it will not get the correct volume set on the first frame:" + base.gameObject.name);
				SetVolume(audioSrcLogic.GetVolume());
				hasAudioVolumeBeenSet = true;
			}
			isAddedToAudioManager = true;
		}
	}

	private void OnDisable()
	{
		if (audioSrcLogic != null)
		{
			audioSrcLogic.ToggleEnable(false);
		}
		if (isAddedToAudioManager)
		{
			if (audioSrcLogic != null && isPermanent && AudioManager._noCreateInstance != null)
			{
				AudioManager._noCreateInstance.RemovePermanentAudioSourceHelper(this);
			}
			isAddedToAudioManager = false;
		}
	}

	private void Start()
	{
		if (playOnStart && audioSrcLogic != null)
		{
			Play();
		}
	}

	public bool IsFollowTargetAndClipEqual(Transform testFollowTarget, AudioClip testAudioClip)
	{
		return followTarget == testFollowTarget && audioSrcLogic.GetClip() == testAudioClip;
	}

	public void FadeOut(float fadeTime = 3f)
	{
		timeToFade = fadeTime;
		if (fadeType == FadeTypes.None)
		{
			fadeType = FadeTypes.FadeOut;
			StartCoroutine(FadeRoutine());
			return;
		}
		if (fadeType == FadeTypes.FadeIn)
		{
			FadeComplete();
		}
		fadeType = FadeTypes.FadeOut;
		fadeTimePassed = 0f;
	}

	public void FadeIn(float fadeTime = 3f)
	{
		timeToFade = fadeTime;
		if (fadeType == FadeTypes.None)
		{
			fadeType = FadeTypes.FadeIn;
			StartCoroutine(FadeRoutine());
			return;
		}
		if (fadeType == FadeTypes.FadeOut)
		{
			FadeComplete();
		}
		fadeType = FadeTypes.FadeIn;
		fadeTimePassed = 0f;
	}

	private IEnumerator FadeRoutine()
	{
		fadeTimePassed = 0f;
		if (fadeType == FadeTypes.FadeOut)
		{
			if (!audioSrcLogic.IsPlaying())
			{
				Play();
			}
		}
		else if (fadeType == FadeTypes.FadeIn)
		{
			audioSrcLogic.SetVolume(0f);
			if (!audioSrcLogic.IsPlaying())
			{
				Play();
			}
		}
		while (fadeType != 0)
		{
			if (fadeType == FadeTypes.FadeOut)
			{
				if (fadeTimePassed >= timeToFade)
				{
					audioSrcLogic.SetVolume(0f);
					Stop();
					break;
				}
				audioSrcLogic.SetVolume(Mathf.Lerp(audioVolume, 0f, fadeTimePassed / timeToFade) * AudioManager.GetMasterVolumePercentage());
			}
			else if (fadeType == FadeTypes.FadeIn)
			{
				if (fadeTimePassed >= timeToFade)
				{
					audioSrcLogic.SetVolume(audioVolume * AudioManager.GetMasterVolumePercentage());
					break;
				}
				audioSrcLogic.SetVolume(Mathf.Lerp(0f, audioVolume, fadeTimePassed / timeToFade) * AudioManager.GetMasterVolumePercentage());
			}
			fadeTimePassed += Time.deltaTime;
			yield return null;
		}
		if (fadeType != 0)
		{
			FadeComplete();
		}
	}

	private void FadeComplete()
	{
		if (fadeType == FadeTypes.FadeIn)
		{
			fadeType = FadeTypes.None;
			if (OnFadeInComplete != null)
			{
				OnFadeInComplete(this);
			}
		}
		else if (fadeType == FadeTypes.FadeOut)
		{
			fadeType = FadeTypes.None;
			if (OnFadeOutComplete != null)
			{
				OnFadeOutComplete(this);
			}
		}
	}

	public void FadeOutCrossFade(float fadeTime = 3f)
	{
		SwapCrossFadeInAudioSources();
		crossFadeTimeToFade = fadeTime;
		if (crossFadeOutAudioSrcLogic.IsPlaying())
		{
			fadeType = FadeTypes.FadeOut;
			StartCoroutine(FadeRoutineCrossFadeOut(0));
		}
		else
		{
			Debug.Log("DO NOTHING");
		}
	}

	private IEnumerator FadeRoutineCrossFadeOut(int timeIn)
	{
		crossFadeTimePassed = 0f;
		while (fadeType != 0)
		{
			if (fadeType == FadeTypes.FadeOut && crossFadeTimePassed / crossFadeTimeToFade > 0.4f)
			{
				FadeComplete();
			}
			if (crossFadeTimePassed >= crossFadeTimeToFade)
			{
				crossFadeOutAudioSrcLogic.SetVolume(0f);
				crossFadeOutAudioSrcLogic.Stop();
				break;
			}
			crossFadeOutAudioSrcLogic.SetVolume(Mathf.Lerp(audioVolume, 0f, crossFadeTimePassed / crossFadeTimeToFade) * AudioManager.GetMasterVolumePercentage());
			crossFadeTimePassed += Time.deltaTime;
			yield return null;
		}
		if (crossFadeOutAudioSrcLogic.IsPlaying())
		{
			crossFadeOutAudioSrcLogic.Stop();
		}
	}

	public void SetupAudioSource(AudioClip clip, float volume, float pitch, bool loop)
	{
		SetClip(clip);
		SetLooping(loop);
		SetVolume(volume);
		SetPitch(pitch);
	}

	public void SwapCrossFadeInAudioSources()
	{
		IAudioSourceLogic audioSourceLogic = audioSrcLogic;
		audioSrcLogic = crossFadeOutAudioSrcLogic;
		crossFadeOutAudioSrcLogic = audioSourceLogic;
	}

	public void UpdateVolume()
	{
		if (fadeType == FadeTypes.None)
		{
			audioSrcLogic.SetVolume(audioVolume * AudioManager.GetMasterVolumePercentage());
		}
	}

	public void SetVolume(float volume)
	{
		audioVolume = volume;
		hasAudioVolumeBeenSet = true;
		UpdateVolume();
	}

	public void SetPitch(float pitch)
	{
		audioSrcLogic.SetPitch(pitch);
		if (crossFadeOutAudioSrcLogic != null)
		{
			crossFadeOutAudioSrcLogic.SetPitch(pitch);
		}
	}

	public float GetPitch()
	{
		return audioSrcLogic.GetPitch();
	}

	public void SetClip(AudioClip clip)
	{
		audioSrcLogic.SetClip(clip);
	}

	public AudioClip GetClip()
	{
		return audioSrcLogic.GetClip();
	}

	public void SetLooping(bool loop)
	{
		audioSrcLogic.SetLoop(loop);
	}

	public void GetAlreadyCreatedAudioSource()
	{
		if (AudioManager.AudioSystemType == AudioManager.AudioSystemTypes.UnityAudioSource || useAudioSourceOnly)
		{
			UnityAudioSourceLogic unityAudioSourceLogic = new UnityAudioSourceLogic();
			AudioSource component = base.gameObject.GetComponent<AudioSource>();
			if (component != null)
			{
				if (enableSpatialization)
				{
					component.spatialBlend = 1f;
					component.spatialize = true;
				}
				unityAudioSourceLogic.Setup(component);
				audioSrcLogic = unityAudioSourceLogic;
			}
			else
			{
				Debug.LogWarning("Use Already Created Audio Source was checked but not audio source was found:" + base.gameObject.name);
			}
		}
		else if (AudioManager.AudioSystemType == AudioManager.AudioSystemTypes.OculusAudio)
		{
			OculusAudioSourceLogic oculusAudioSourceLogic = new OculusAudioSourceLogic();
			AudioSource source = base.gameObject.GetComponent<AudioSource>();
			if (source != null)
			{
				if (enableSpatialization)
				{
					source.spatialBlend = 1f;
				}
				if (source.outputAudioMixerGroup == null)
				{
					Debug.LogWarning("All audio sources now need mixed groups:" + base.gameObject.name, base.gameObject);
				}
				ONSPAudioSource oNSPAudioSource = base.gameObject.AddComponent<ONSPAudioSource>();
				oNSPAudioSource.DisableRfl = disableReflections;
				oNSPAudioSource.EnableSpatialization = enableSpatialization;
				oNSPAudioSource.Gain = gain;
				oNSPAudioSource.UseInvSqr = inverseSquareAttenuation;
				oNSPAudioSource.Near = near;
				oNSPAudioSource.Far = far;
				oculusAudioSourceLogic.Setup(oNSPAudioSource, source);
				audioSrcLogic = oculusAudioSourceLogic;
				oNSPAudioSource.SetParameters(ref source);
			}
			else
			{
				Debug.LogWarning("Use Already Created Audio Source was checked but not audio source was found:" + base.gameObject.name);
			}
		}
		else
		{
			Debug.LogWarning("Using already created audio sources is not yet supported for this audio type:" + base.gameObject.name);
		}
	}

	public void CreateAudioSource()
	{
		if (AudioManager.AudioSystemType == AudioManager.AudioSystemTypes.OculusAudio)
		{
			OculusAudioSourceLogic oculusAudioSourceLogic = new OculusAudioSourceLogic();
			AudioSource source = base.gameObject.AddComponent<AudioSource>();
			source.playOnAwake = false;
			source.loop = false;
			source.outputAudioMixerGroup = AudioManager.Instance.AudioMixerSettings.DefaultMixerGroup;
			if (enableSpatialization)
			{
				source.spatialBlend = 1f;
			}
			ONSPAudioSource oNSPAudioSource = base.gameObject.AddComponent<ONSPAudioSource>();
			oNSPAudioSource.DisableRfl = disableReflections;
			oNSPAudioSource.EnableSpatialization = enableSpatialization;
			oNSPAudioSource.Gain = gain;
			oNSPAudioSource.UseInvSqr = inverseSquareAttenuation;
			oNSPAudioSource.Near = near;
			oNSPAudioSource.Far = far;
			oculusAudioSourceLogic.Setup(oNSPAudioSource, source);
			audioSrcLogic = oculusAudioSourceLogic;
			oNSPAudioSource.SetParameters(ref source);
		}
		else if (AudioManager.AudioSystemType == AudioManager.AudioSystemTypes.UnityAudioSource)
		{
			UnityAudioSourceLogic unityAudioSourceLogic = new UnityAudioSourceLogic();
			AudioSource audioSource = base.gameObject.AddComponent<AudioSource>();
			audioSource.playOnAwake = false;
			audioSource.loop = false;
			if (enableSpatialization)
			{
				audioSource.spatialBlend = 1f;
				audioSource.spatialize = true;
			}
			unityAudioSourceLogic.Setup(audioSource);
			audioSrcLogic = unityAudioSourceLogic;
		}
		else
		{
			Debug.LogError("Unsupported audio systemtype in CreateAudioSource:" + AudioManager.AudioSystemType);
		}
	}
}
