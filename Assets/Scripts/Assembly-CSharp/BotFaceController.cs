using System;
using UnityEngine;

public class BotFaceController : MonoBehaviour
{
	private const int RENDER_TEXTURE_WIDTH = 256;

	private const int RENDER_TEXTURE_HEIGHT = 256;

	private const int RENDER_TEXTURE_DEPTH = 0;

	private const float DESIRED_FPS_DYNAMIC = 45f;

	private const float DESIRED_FPS_SIMPLE = 30f;

	private static bool USE_LOW_FPS_RENDER = true;

	private bool isSimple;

	private float lastRenderTextureCameraRender;

	private bool isVisible;

	private BotMouthStates mouthState;

	private BotFaceEmote eyesState;

	private BotFaceEmote emoteState;

	[SerializeField]
	private BotMouthData mouthData;

	[SerializeField]
	private GameObject eyesNormal;

	[SerializeField]
	private GameObject eyesHappy;

	[SerializeField]
	private GameObject eyesSad;

	[SerializeField]
	private GameObject eyesAngry;

	[SerializeField]
	private GameObject eyesShifty;

	[SerializeField]
	private GameObject eyesTechno;

	[SerializeField]
	private GameObject eyesTired;

	[SerializeField]
	private GameObject eyesTired2;

	private GameObject currentEyes;

	[SerializeField]
	private Camera faceCamera;

	[SerializeField]
	private MeshRenderer rollerRenderer;

	[SerializeField]
	private LineWave mouthLine;

	[SerializeField]
	private GameObject faceParent;

	[SerializeField]
	private SpriteRenderer customGraphic;

	private MouthLineSettings currentMouthSettings;

	private AudioSource talkingAudioSource;

	private bool wasTalking;

	private RenderTexture renderTexture;

	public Action OnFaceFinishedUniqueAction;

	private float[] spectrumSamples = new float[64];

	private bool faceNeedsLateUpdateRender;

	public RenderTexture RenderTexture
	{
		get
		{
			return renderTexture;
		}
	}

	private void Awake()
	{
		faceCamera.eventMask = 0;
		if (renderTexture == null)
		{
			renderTexture = new RenderTexture(256, 256, 0);
			faceCamera.targetTexture = renderTexture;
		}
		if (USE_LOW_FPS_RENDER)
		{
			faceCamera.enabled = false;
		}
		faceParent.SetActive(true);
		customGraphic.enabled = false;
		currentEyes = eyesNormal;
		ChangeMouthSettings(mouthData.GetMouthLineSettingsForEmote(BotFaceEmote.Idle), 0f, true);
		ForceRender();
	}

	public void SetAsSimple(bool _isSimple)
	{
		isSimple = _isSimple;
	}

	public void SetIsVisible(bool isVis)
	{
		isVisible = isVis;
		mouthLine.SetIsVisible(isVis);
	}

	public void Reset()
	{
		mouthState = BotMouthStates.Idle;
		emoteState = BotFaceEmote.Idle;
		faceParent.SetActive(true);
		customGraphic.enabled = false;
		ChangeMouthSettings(mouthData.GetMouthLineSettingsForEmote(BotFaceEmote.Idle), 0f, true);
		faceNeedsLateUpdateRender = true;
	}

	public void ForceRefreshSettings()
	{
		ChangeMouthSettingsBasedOnEmote(0f, true);
	}

	private void Update()
	{
		if (mouthState == BotMouthStates.Idle)
		{
			if (wasTalking)
			{
				wasTalking = false;
				ChangeMouthSettingsBasedOnEmote(0f, true);
			}
		}
		else
		{
			if (mouthState != BotMouthStates.Talking)
			{
				return;
			}
			float num = CalculateTalkingVolume();
			if (num > 1f)
			{
				ChangeMouthSettingsBasedOnEmote(num, true);
				if (!wasTalking)
				{
					wasTalking = true;
				}
			}
			else if (wasTalking)
			{
				wasTalking = false;
				ChangeMouthSettingsBasedOnEmote(0f, true);
			}
			if (!talkingAudioSource.isPlaying)
			{
				mouthState = BotMouthStates.Idle;
				CheckToSeeIfUniqueActionIsFinished();
			}
		}
	}

	private void LateUpdate()
	{
		if (faceNeedsLateUpdateRender)
		{
			ForceRender();
			faceNeedsLateUpdateRender = false;
		}
		if (USE_LOW_FPS_RENDER && isVisible)
		{
			float num = ((!isSimple) ? 45f : 30f);
			float num2 = lastRenderTextureCameraRender + 1f / num;
			if (Time.time >= num2)
			{
				faceCamera.Render();
				lastRenderTextureCameraRender = Time.time;
			}
		}
	}

	public void ForceRender()
	{
		if (USE_LOW_FPS_RENDER)
		{
			faceCamera.Render();
			lastRenderTextureCameraRender = Time.time;
		}
	}

	public void AssignAsUniqueFaceToBot(Bot bot)
	{
		if (bot.CostumeData != null)
		{
			rollerRenderer.material.SetColor("_EmissionColor", bot.CostumeData.MainScreenColor);
		}
		talkingAudioSource = bot.GetTalkingAudioSource();
		ChangeMouthSettingsBasedOnEmote(0f, true);
		faceNeedsLateUpdateRender = true;
	}

	public void BeginTalking()
	{
		mouthState = BotMouthStates.Talking;
	}

	public void StopTalking()
	{
		mouthState = BotMouthStates.Idle;
		CheckToSeeIfUniqueActionIsFinished();
	}

	public void SetEmote(BotFaceEmote emote)
	{
		faceParent.SetActive(true);
		customGraphic.enabled = false;
		emoteState = emote;
		ChangeMouthSettingsBasedOnEmote(0f, true);
		CheckToSeeIfUniqueActionIsFinished();
		faceNeedsLateUpdateRender = true;
	}

	public void SetCustomGraphic(Sprite sprite)
	{
		emoteState = BotFaceEmote.CustomGraphic;
		ChangeMouthSettingsNormal();
		faceParent.SetActive(false);
		customGraphic.enabled = true;
		customGraphic.sprite = sprite;
		faceNeedsLateUpdateRender = true;
	}

	private void ChangeMouthSettings(MouthLineSettings settings, float volume = 0f, bool forceToApply = false)
	{
		if (settings != null && (settings != currentMouthSettings || forceToApply))
		{
			settings.ApplyToLinewave(mouthLine, volume);
			currentMouthSettings = settings;
			SetEyesBasedOnEmote(emoteState);
		}
	}

	private void SetEyesBasedOnEmote(BotFaceEmote emote)
	{
		if (emote != eyesState)
		{
			if (currentEyes != null)
			{
				currentEyes.SetActive(false);
			}
			eyesState = emote;
			switch (emote)
			{
			case BotFaceEmote.Idle:
				eyesNormal.SetActive(true);
				currentEyes = eyesNormal;
				break;
			case BotFaceEmote.Angry:
				eyesAngry.SetActive(true);
				currentEyes = eyesAngry;
				break;
			case BotFaceEmote.Happy:
				eyesHappy.SetActive(true);
				currentEyes = eyesHappy;
				break;
			case BotFaceEmote.Sad:
				eyesSad.SetActive(true);
				currentEyes = eyesSad;
				break;
			case BotFaceEmote.Shifty:
				eyesShifty.SetActive(true);
				currentEyes = eyesShifty;
				break;
			case BotFaceEmote.Techno:
				eyesTechno.SetActive(true);
				currentEyes = eyesTechno;
				break;
			case BotFaceEmote.Tired:
				eyesTired.SetActive(true);
				currentEyes = eyesTired;
				break;
			case BotFaceEmote.Tired2:
				eyesTired2.SetActive(true);
				currentEyes = eyesTired2;
				break;
			}
		}
	}

	private void CheckToSeeIfUniqueActionIsFinished()
	{
		if (mouthState == BotMouthStates.Idle && emoteState == BotFaceEmote.Idle && OnFaceFinishedUniqueAction != null)
		{
			OnFaceFinishedUniqueAction();
		}
	}

	private void ChangeMouthSettingsNormal()
	{
		ChangeMouthSettingsBasedOnEmote(0f, false);
	}

	private void ChangeMouthSettingsBasedOnEmote(float talkVolume, bool force)
	{
		SetEyesBasedOnEmote(emoteState);
		ChangeMouthSettings(mouthData.GetMouthLineSettingsForEmote(emoteState), talkVolume, force);
	}

	private float CalculateTalkingVolume()
	{
		talkingAudioSource.GetSpectrumData(spectrumSamples, 0, FFTWindow.Triangle);
		float num = 0f;
		for (int i = 0; i < spectrumSamples.Length; i++)
		{
			num += spectrumSamples[i];
		}
		float currentVelocity = 0f;
		return Mathf.SmoothDamp(0f, num, ref currentVelocity, 0.02f) * 100f;
	}
}
