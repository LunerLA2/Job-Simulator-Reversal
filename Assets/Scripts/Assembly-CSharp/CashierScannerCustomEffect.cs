using System;
using OwlchemyVR;
using UnityEngine;

[Serializable]
public class CashierScannerCustomEffect
{
	[SerializeField]
	private WorldItemData itemData;

	[SerializeField]
	private AudioClip customSound;

	[SerializeField]
	private string customPrice = string.Empty;

	public WorldItemData ItemData
	{
		get
		{
			return itemData;
		}
	}

	public AudioClip CustomSound
	{
		get
		{
			return customSound;
		}
	}

	public string CustomPrice
	{
		get
		{
			return customPrice;
		}
	}
}
