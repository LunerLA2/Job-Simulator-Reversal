using System.Collections;
using UnityEngine;

public class OVRScreenFade : MonoBehaviour
{
	public float fadeTime = 2f;

	public Color fadeColor = new Color(0.01f, 0.01f, 0.01f, 1f);

	private Material fadeMaterial;

	private bool isFading;

	private YieldInstruction fadeInstruction = new WaitForEndOfFrame();

	private void Awake()
	{
		fadeMaterial = new Material(Shader.Find("Oculus/Unlit Transparent Color"));
	}

	private void OnEnable()
	{
		StartCoroutine(FadeIn());
	}

	private void OnLevelWasLoaded(int level)
	{
		StartCoroutine(FadeIn());
	}

	private void OnDestroy()
	{
		if (fadeMaterial != null)
		{
			Object.Destroy(fadeMaterial);
		}
	}

	private IEnumerator FadeIn()
	{
		float elapsedTime = 0f;
		fadeMaterial.color = fadeColor;
		Color color = fadeColor;
		isFading = true;
		while (elapsedTime < fadeTime)
		{
			yield return fadeInstruction;
			elapsedTime += Time.deltaTime;
			color.a = 1f - Mathf.Clamp01(elapsedTime / fadeTime);
			fadeMaterial.color = color;
		}
		isFading = false;
	}

	private void OnPostRender()
	{
		if (isFading)
		{
			fadeMaterial.SetPass(0);
			GL.PushMatrix();
			GL.LoadOrtho();
			GL.Color(fadeMaterial.color);
			GL.Begin(7);
			GL.Vertex3(0f, 0f, -12f);
			GL.Vertex3(0f, 1f, -12f);
			GL.Vertex3(1f, 1f, -12f);
			GL.Vertex3(1f, 0f, -12f);
			GL.End();
			GL.PopMatrix();
		}
	}
}
