using System;
using UnityEngine;

[Serializable]
public class MicrowaveRecipe : MicrowaveRequirementListObject
{
	[SerializeField]
	private GameObject resultPrefab;

	public GameObject ResultPrefab
	{
		get
		{
			return resultPrefab;
		}
	}
}
