public class TaskStateData
{
	public string ID { get; private set; }

	public string CustomAction { get; private set; }

	public string CustomActionParam { get; private set; }

	public bool IsCompleted { get; private set; }

	public TaskStateData(string id)
	{
		ID = id;
	}

	public void SetCustomData(string customAction, string customActionParam = "")
	{
		CustomAction = customAction;
		CustomActionParam = customActionParam;
	}

	public void SetIsCompleted(bool wasCompleted)
	{
		IsCompleted = wasCompleted;
	}
}
