using System;
using UnityEngine;

[Serializable]
public class ToppingPositionInfo
{
	[SerializeField]
	private Transform[] sharedPositions;

	public Transform[] SharedPositions
	{
		get
		{
			return sharedPositions;
		}
	}
}
