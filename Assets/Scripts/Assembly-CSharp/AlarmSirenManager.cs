using System.Collections;
using UnityEngine;

public class AlarmSirenManager : MonoBehaviour
{
	[SerializeField]
	private Animation anim;

	[SerializeField]
	private Light directionalLight;

	[SerializeField]
	private Light animLight1;

	[SerializeField]
	private Light animLight2;

	private Color cachedAmbientLight;

	public Color ambientColor;

	[SerializeField]
	private bool turnOnObjects;

	private float duration;

	private bool sironOn;

	[SerializeField]
	private Transform glowObj;

	private void Awake()
	{
		cachedAmbientLight = RenderSettings.ambientLight;
	}

	public void StartSiron()
	{
		if (!sironOn)
		{
			sironOn = true;
			if ((bool)anim)
			{
				anim.Play();
			}
			RenderSettings.ambientLight = ambientColor;
			directionalLight.enabled = false;
			if ((bool)animLight1)
			{
				animLight1.enabled = true;
			}
			if ((bool)animLight2)
			{
				animLight2.enabled = true;
			}
			StartCoroutine(SironTimer());
			if (turnOnObjects)
			{
				glowObj.gameObject.SetActive(true);
			}
		}
	}

	private IEnumerator SironTimer()
	{
		yield return new WaitForSeconds(10f);
		directionalLight.enabled = true;
		RenderSettings.ambientLight = cachedAmbientLight;
		if ((bool)animLight1)
		{
			animLight1.enabled = false;
		}
		if ((bool)animLight2)
		{
			animLight2.enabled = false;
		}
		sironOn = false;
		if ((bool)anim)
		{
			anim.Stop();
		}
		if (turnOnObjects)
		{
			glowObj.gameObject.SetActive(false);
		}
	}

	public void StopSiron()
	{
		StopCoroutine(SironTimer());
		directionalLight.enabled = true;
		RenderSettings.ambientLight = cachedAmbientLight;
		if ((bool)animLight1)
		{
			animLight1.enabled = false;
		}
		if ((bool)animLight2)
		{
			animLight2.enabled = false;
		}
		sironOn = false;
		if (turnOnObjects)
		{
			glowObj.gameObject.SetActive(false);
		}
		if ((bool)anim)
		{
			anim.Stop();
		}
	}
}
