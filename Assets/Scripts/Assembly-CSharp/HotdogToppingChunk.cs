using System;
using UnityEngine;

[Serializable]
public class HotdogToppingChunk
{
	private const float TOPPING_COLOR_LERP_SPEED = 5f;

	[SerializeField]
	private ParticleImpactZone particleImpactZone;

	[SerializeField]
	private MeshRenderer meshRenderer;

	private Color currentColor = Color.white;

	private bool hasBeenTouched;

	public ParticleImpactZone ParticleImpactZone
	{
		get
		{
			return particleImpactZone;
		}
	}

	public void Init()
	{
		meshRenderer.gameObject.SetActive(false);
		hasBeenTouched = false;
	}

	public void ApplyColor(Color c)
	{
		if (!hasBeenTouched)
		{
			currentColor = c;
			meshRenderer.material.SetColor("_DiffColor", c);
			hasBeenTouched = true;
		}
		else
		{
			currentColor = Color.Lerp(currentColor, c, 5f * Time.deltaTime);
			meshRenderer.material.SetColor("_DiffColor", currentColor);
		}
		meshRenderer.gameObject.SetActive(true);
	}
}
