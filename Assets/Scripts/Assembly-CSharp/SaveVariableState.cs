using System;
using System.Xml.Serialization;

[Serializable]
public class SaveVariableState
{
	[XmlAttribute]
	public string name;

	[XmlAttribute("value")]
	public string valueStr;

	public void Store(string name, string valueStr)
	{
		this.name = name;
		this.valueStr = valueStr;
	}

	public void Restore()
	{
	}
}
