using System.Collections;
using UnityEngine;

public class OptionLongPressHandler : MonoBehaviour
{
	[SerializeField]
	private GameObject root;

	[SerializeField]
	private KeyCode editorTestKey = KeyCode.Period;

	[SerializeField]
	private MeshRenderer[] renderers;

	private float pulseRoutineDuration = 8f;

	private Color[] initialColor;

	private Coroutine fadeRoutine;

	private void Start()
	{
		Object.DontDestroyOnLoad(base.gameObject);
		initialColor = new Color[renderers.Length];
		for (int i = 0; i < renderers.Length; i++)
		{
			initialColor[i] = renderers[i].material.color;
		}
		root.SetActive(false);
	}

	private IEnumerator FadeRoutine()
	{
		root.SetActive(true);
		float startTime = Time.time;
		float t;
		do
		{
			t = (Time.time - startTime) / pulseRoutineDuration;
			for (int i = 0; i < renderers.Length; i++)
			{
				renderers[i].material.color = Color.Lerp(initialColor[i], Color.clear, t);
			}
			yield return null;
		}
		while (!(t >= 1f));
		root.SetActive(false);
	}
}
