using System;
using UnityEngine;

[Serializable]
public class MicrowaveToppedFoodItemRecipe : MicrowaveRequirementListObject
{
	[SerializeField]
	private ToppedFoodItemController toppedFoodItemPrefab;

	public ToppedFoodItemController ToppedFoodItemPrefab
	{
		get
		{
			return toppedFoodItemPrefab;
		}
	}
}
