using System;
using UnityEngine;

[Serializable]
public class CostumePiece
{
	[SerializeField]
	private BotCostumeMountPoints mountPoint;

	[SerializeField]
	private GameObject artPrefab;

	public BotCostumeMountPoints MountPoint
	{
		get
		{
			return mountPoint;
		}
	}

	public GameObject ArtPrefab
	{
		get
		{
			return artPrefab;
		}
	}
}
