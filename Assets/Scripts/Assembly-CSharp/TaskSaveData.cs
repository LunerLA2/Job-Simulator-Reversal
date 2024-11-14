using System;
using System.Xml.Serialization;

[Serializable]
public class TaskSaveData
{
	[XmlAttribute("id")]
	public string ID { get; private set; }

	[XmlAttribute("wasCompleted")]
	public bool WasCompleted { get; private set; }

	[XmlElement]
	public CustomActionSaveData CustomAction { get; private set; }

	public void SetID(string id)
	{
		ID = id;
	}

	public void SetWasCompleted(bool wasCompleted)
	{
		WasCompleted = wasCompleted;
	}

	public void SetCustomData(CustomActionSaveData customAction)
	{
		CustomAction = customAction;
	}
}
