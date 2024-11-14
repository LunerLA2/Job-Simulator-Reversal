using System;
using OwlchemyVR;
using UnityEngine;

[Serializable]
public class BlendResult
{
	[SerializeField]
	private WorldItemData blendItem;

	[SerializeField]
	private float fluidAmountMultiplier = 1f;

	public WorldItemData BlendItem
	{
		get
		{
			return blendItem;
		}
	}

	public float FluidAmountMultiplier
	{
		get
		{
			return fluidAmountMultiplier;
		}
	}
}
