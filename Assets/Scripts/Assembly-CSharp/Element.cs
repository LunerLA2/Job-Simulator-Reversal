using System;

[Serializable]
public class Element
{
	public int number;

	internal StateOfMatter state;

	public string name;

	public Element()
	{
		state = StateOfMatter.Solid;
	}

	public StateOfMatter GetState()
	{
		return state;
	}
}
