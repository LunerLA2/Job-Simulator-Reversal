using System;
using OwlchemyVR;
using UnityEngine;

[Serializable]
public class SwapWorldItemWithAlternate
{
	[SerializeField]
	private WorldItemData worldItemToSwapOut;

	[SerializeField]
	private GameObject alternateToPrint;

	public WorldItemData SwapWorldItem
	{
		get
		{
			return worldItemToSwapOut;
		}
	}

	public GameObject AlternateGameObject
	{
		get
		{
			return alternateToPrint;
		}
	}
}
