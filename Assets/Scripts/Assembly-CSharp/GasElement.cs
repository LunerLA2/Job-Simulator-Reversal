using System;
using UnityEngine;

[Serializable]
public class GasElement : Element
{
	public Color color;

	public GasElement()
	{
		state = StateOfMatter.Gas;
	}
}
