using System;
using UnityEngine;

[Serializable]
public class AttachLocationOverride
{
	[SerializeField]
	private Transform location;

	[SerializeField]
	private AttachablePointData attachablePointData;

	public Transform Location
	{
		get
		{
			return location;
		}
	}

	public AttachablePointData AttachablePointData
	{
		get
		{
			return attachablePointData;
		}
	}
}
