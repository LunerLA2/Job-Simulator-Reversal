public class SubTaskActionEventConditionsContainer
{
	private SubtaskStatusController subtaskStatusController;

	private ActionEventCondition actionEventCondition;

	private PageData parentPageData;

	public SubtaskStatusController SubtaskStatusController
	{
		get
		{
			return subtaskStatusController;
		}
	}

	public ActionEventCondition ActionEventCondition
	{
		get
		{
			return actionEventCondition;
		}
	}

	public PageData ParentPageData
	{
		get
		{
			return parentPageData;
		}
	}

	public SubTaskActionEventConditionsContainer(SubtaskStatusController subtaskStatusController, ActionEventCondition actionEventCondition, PageData parentPage)
	{
		this.subtaskStatusController = subtaskStatusController;
		this.actionEventCondition = actionEventCondition;
		parentPageData = parentPage;
	}
}
