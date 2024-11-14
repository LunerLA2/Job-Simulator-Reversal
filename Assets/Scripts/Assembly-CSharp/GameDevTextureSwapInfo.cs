using System;
using UnityEngine;

[Serializable]
public class GameDevTextureSwapInfo
{
	[SerializeField]
	private MeshRenderer[] renderers;

	[SerializeField]
	private Material materialInGameDevMode;

	public void Apply()
	{
		for (int i = 0; i < renderers.Length; i++)
		{
			renderers[i].sharedMaterial = materialInGameDevMode;
		}
	}
}
