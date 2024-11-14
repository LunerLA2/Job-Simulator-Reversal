using System;
using UnityEngine;

[Serializable]
public class ServingPlateTableInfo
{
	[SerializeField]
	private PageData duringPage;

	[SerializeField]
	private Transform targetTable;

	public PageData DuringPage
	{
		get
		{
			return duringPage;
		}
	}

	public Transform TargetTable
	{
		get
		{
			return targetTable;
		}
	}
}
