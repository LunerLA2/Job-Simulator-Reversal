using System.Collections.Generic;

public class SubTaskActionEventContainerListObject
{
	private List<SubTaskActionEventConditionsContainer> containerList;

	public List<SubTaskActionEventConditionsContainer> ContainerList
	{
		get
		{
			return containerList;
		}
		set
		{
			containerList = value;
		}
	}

	public void ActionOccurred(PageData currentVisiblePage)
	{
		for (int i = 0; i < containerList.Count; i++)
		{
			if (!containerList[i].SubtaskStatusController.Data.OnlyTrackProgressWhenVisible || currentVisiblePage == containerList[i].ParentPageData || containerList[i].ParentPageData == null)
			{
				if (containerList[i].ActionEventCondition.IsPositive)
				{
					containerList[i].SubtaskStatusController.PositiveActionOccurred(containerList[i].ActionEventCondition);
				}
				else
				{
					containerList[i].SubtaskStatusController.NegativeActionOccurred(containerList[i].ActionEventCondition);
				}
			}
		}
	}

	public void ActionOccurredWithAmount(float amount, PageData currentVisiblePage)
	{
		for (int i = 0; i < containerList.Count; i++)
		{
			if (!containerList[i].SubtaskStatusController.Data.OnlyTrackProgressWhenVisible || currentVisiblePage == containerList[i].ParentPageData || containerList[i].ParentPageData == null)
			{
				if (containerList[i].ActionEventCondition.IsPositive)
				{
					containerList[i].SubtaskStatusController.PositiveActionOccurredWithAmount(containerList[i].ActionEventCondition, amount);
				}
				else
				{
					containerList[i].SubtaskStatusController.NegativeActionOccurredWithAmount(containerList[i].ActionEventCondition, amount);
				}
			}
		}
	}
}
