using UnityEngine;

public class BatteryGlowyBitData : ScriptableObject
{
	[SerializeField]
	private Color offColor;

	[SerializeField]
	private BatteryGlowyBit[] batteryGlowyBits;

	public Color OffColor
	{
		get
		{
			return offColor;
		}
	}

	public BatteryGlowyBit[] BatteryGlowyBits
	{
		get
		{
			return batteryGlowyBits;
		}
	}
}
