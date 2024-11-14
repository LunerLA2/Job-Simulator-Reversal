using System;
using OwlchemyVR;

[Serializable]
public class LiquidElement : Element
{
	public WorldItemData fluid;

	public LiquidElement()
	{
		state = StateOfMatter.Liquid;
	}
}
