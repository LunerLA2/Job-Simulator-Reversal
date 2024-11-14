using System.Collections.Generic;

public class BrainCauseListObject
{
	private List<BrainCauseListEntry> causeList;

	public List<BrainCauseListEntry> CauseList
	{
		get
		{
			return causeList;
		}
		set
		{
			causeList = value;
		}
	}

	public void CounterActionOccurred(int amt, BrainData localTo = null)
	{
		for (int i = 0; i < causeList.Count; i++)
		{
			if (localTo == null || localTo == causeList[i].parentBrain.Brain)
			{
				causeList[i].cause.CheckCounter(amt);
			}
		}
	}

	public void InstantCounterActionOccurred(int amt, BrainData localTo = null)
	{
		for (int i = 0; i < causeList.Count; i++)
		{
			if (localTo == null || localTo == causeList[i].parentBrain.Brain)
			{
				causeList[i].cause.InstantCheckCounter(amt);
			}
		}
	}

	public void IsCounterActionOccurred(int amt, BrainData localTo = null)
	{
		for (int i = 0; i < causeList.Count; i++)
		{
			if (localTo == null || localTo == causeList[i].parentBrain.Brain)
			{
				causeList[i].cause.IsCounterUpdate(amt);
			}
		}
	}

	public void InstantActionOccurred(BrainData localTo = null)
	{
		for (int i = 0; i < causeList.Count; i++)
		{
			if (localTo == null || localTo == causeList[i].parentBrain.Brain)
			{
				causeList[i].cause.InstantComplete();
			}
		}
	}

	public void InstantActionOccurredWithAmount(float amt)
	{
		for (int i = 0; i < causeList.Count; i++)
		{
			causeList[i].cause.InstantCheckAmount(amt);
		}
	}

	public void PositiveActionOccurred(BrainData localTo = null)
	{
		for (int i = 0; i < causeList.Count; i++)
		{
			if (localTo == null || localTo == causeList[i].parentBrain.Brain)
			{
				causeList[i].cause.Complete();
			}
		}
	}

	public void PositiveActionOccurredWithAmount(float amt)
	{
		for (int i = 0; i < causeList.Count; i++)
		{
			causeList[i].cause.CheckAmount(amt);
		}
	}

	public void NegativeActionOccurred(BrainData localTo = null)
	{
		for (int i = 0; i < causeList.Count; i++)
		{
			if (localTo == null || localTo == causeList[i].parentBrain.Brain)
			{
				causeList[i].cause.Uncomplete();
			}
		}
	}
}
