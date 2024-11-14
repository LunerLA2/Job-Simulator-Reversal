using System;

[Serializable]
public class SolidElement : Element
{
	public SolidElement()
	{
		state = StateOfMatter.Solid;
	}
}
