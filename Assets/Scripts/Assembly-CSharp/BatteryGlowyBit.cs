using System;
using OwlchemyVR;
using UnityEngine;

[Serializable]
public class BatteryGlowyBit
{
	[SerializeField]
	private Color glowColor;

	[SerializeField]
	private WorldItemData[] batteryItemData;

	public Color GlowColor
	{
		get
		{
			return glowColor;
		}
	}

	public WorldItemData[] BatteryItemData
	{
		get
		{
			return batteryItemData;
		}
	}
}
