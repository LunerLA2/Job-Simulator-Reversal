using System;
using OwlchemyVR;
using UnityEngine;

[Serializable]
public class StampWorldItemSwitch
{
	[SerializeField]
	private WorldItemData fromData;

	[SerializeField]
	private WorldItemData toData;

	public WorldItemData FromData
	{
		get
		{
			return fromData;
		}
	}

	public WorldItemData ToData
	{
		get
		{
			return toData;
		}
	}
}
