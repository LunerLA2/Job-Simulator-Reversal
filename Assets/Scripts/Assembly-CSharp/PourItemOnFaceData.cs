using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PourItemOnFaceData : ScriptableObject
{
	[SerializeField]
	private List<PourItemOnFaceEffect> pourItemOnFaceEffects;

	public List<PourItemOnFaceEffect> PourItemOnFaceEffects
	{
		get
		{
			return pourItemOnFaceEffects;
		}
	}
}
