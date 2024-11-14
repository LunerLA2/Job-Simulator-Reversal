using System;
using OwlchemyVR;
using UnityEngine;

[Serializable]
public class MicrowaveToppingIngredientPair
{
	[SerializeField]
	private WorldItemData[] possibleInputWorldItems;

	[SerializeField]
	private GameObject outputToppingPrefab;

	public WorldItemData[] PossibleInputWorldItems
	{
		get
		{
			return possibleInputWorldItems;
		}
	}

	public GameObject OutputToppingPrefab
	{
		get
		{
			return outputToppingPrefab;
		}
	}
}
