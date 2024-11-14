using System;
using UnityEngine;

public class LayerMaskHelper
{
	public static int OnlyIncluding(params int[] layers)
	{
		return MakeMask(layers);
	}

	public static int EverythingBut(params int[] layers)
	{
		return ~MakeMask(layers);
	}

	private static int MakeMask(params int[] layers)
	{
		int num = 0;
		foreach (int num2 in layers)
		{
			num |= 1 << num2;
		}
		return num;
	}

	public static bool IsLayerPartOfMask(int layer, int mask)
	{
		Debug.Log("Untested");
		return Convert.ToString(mask, 2)[layer] == '\u0001';
	}
}
