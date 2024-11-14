using System;
using System.Xml.Serialization;

[Serializable]
public class CustomActionSaveData
{
	[XmlAttribute("actionID")]
	public string ActionID { get; private set; }

	[XmlText]
	public string ActionParam { get; private set; }

	public void SetCustomActionData(string id, string param = "")
	{
		ActionID = id;
		ActionParam = param;
	}
}
