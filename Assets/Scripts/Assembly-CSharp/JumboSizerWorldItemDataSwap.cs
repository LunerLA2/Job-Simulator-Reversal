using System;
using OwlchemyVR;
using UnityEngine;

[Serializable]
public class JumboSizerWorldItemDataSwap
{
	[SerializeField]
	private WorldItemData from;

	[SerializeField]
	private WorldItemData to;

	public WorldItemData From
	{
		get
		{
			return from;
		}
	}

	public WorldItemData To
	{
		get
		{
			return to;
		}
	}
}
