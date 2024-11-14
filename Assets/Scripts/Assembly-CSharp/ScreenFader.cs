using System.Collections;
using OwlchemyVR;
using UnityEngine;
using UnityEngine.Events;

public class ScreenFader : MonoBehaviour
{
	public UnityAction OnFadeInComplete;

	public UnityAction OnFadeOutComplete;

	private bool isAlwaysBlack;

	private bool usePostRender = true;

	private static ScreenFader instance;

	private Material m_material;

	private Coroutine fadeRoutine;

	private bool isFadeRoutineRunning;

	public bool IsAlwaysBlack
	{
		set
		{
			m_material.color = ((!value) ? Color.clear : Color.black);
			isAlwaysBlack = value;
		}
	}

	public static ScreenFader Instance
	{
		get
		{
			if (!instance)
			{
				instance = Camera.main.gameObject.AddComponent<ScreenFader>();
			}
			return instance;
		}
	}

	private void Awake()
	{
		if (!instance)
		{
			instance = this;
		}
		if (instance != this)
		{
			Object.Destroy(this);
		}
		if (VRPlatform.GetCurrVRPlatformType() == VRPlatformTypes.SteamVR)
		{
			m_material = new Material(Shader.Find("SteamVR/Unlit Transparent Color"));
			usePostRender = false;
		}
		else
		{
			m_material = new Material(Shader.Find("Oculus/Unlit Transparent Color"));
			usePostRender = true;
		}
	}

	private void OnLevelWasLoaded(int i)
	{
		Instance.IsAlwaysBlack = true;
		FadeIn(2.5f);
	}

	public void FadeIn(float fadeTime)
	{
		if (fadeRoutine != null)
		{
			StopCoroutine(fadeRoutine);
		}
		isFadeRoutineRunning = true;
		fadeRoutine = StartCoroutine(FadeRoutine(fadeTime, true));
	}

	public void FadeOut(float fadeTime)
	{
		if (fadeRoutine != null)
		{
			StopCoroutine(fadeRoutine);
		}
		isFadeRoutineRunning = true;
		fadeRoutine = StartCoroutine(FadeRoutine(fadeTime, false));
	}

	private IEnumerator FadeRoutine(float fadeTime, bool fadeIn)
	{
		SteamVR vr = null;
		if (VRPlatform.GetCurrVRPlatformType() == VRPlatformTypes.SteamVR)
		{
			vr = SteamVR.instance;
		}
		if (vr == null)
		{
			float startTime = Time.time;
			float t;
			do
			{
				t = (Time.time - startTime) / fadeTime;
				if (fadeIn)
				{
					m_material.color = Color.Lerp(Color.black, Color.clear, t);
				}
				else
				{
					m_material.color = Color.Lerp(Color.clear, Color.black, t);
				}
				yield return null;
			}
			while (!(t >= 1f));
			m_material.color = ((!fadeIn) ? Color.black : Color.clear);
		}
		else
		{
			if (fadeIn)
			{
				vr.compositor.FadeToColor(fadeTime, 0f, 0f, 0f, 0f, false);
			}
			else
			{
				vr.compositor.FadeToColor(fadeTime, 0f, 0f, 0f, 1f, false);
			}
			yield return new WaitForSeconds(fadeTime + 0.1f);
		}
		if (fadeIn && OnFadeInComplete != null)
		{
			OnFadeInComplete();
		}
		if (!fadeIn && OnFadeOutComplete != null)
		{
			OnFadeOutComplete();
		}
		yield return new WaitForEndOfFrame();
		if (fadeIn)
		{
			IsAlwaysBlack = false;
		}
		isFadeRoutineRunning = false;
		fadeRoutine = null;
	}

	private void OnPostRender()
	{
		if (usePostRender && m_material.color.a > 0f && (isFadeRoutineRunning || isAlwaysBlack))
		{
			m_material.SetPass(0);
			GL.PushMatrix();
			GL.LoadOrtho();
			GL.Color(m_material.color);
			GL.Begin(7);
			GL.Vertex3(-1f, -1f, 0f);
			GL.Vertex3(-1f, 1f, 0f);
			GL.Vertex3(1f, 1f, 0f);
			GL.Vertex3(1f, -1f, 0f);
			GL.End();
			GL.PopMatrix();
		}
	}
}
