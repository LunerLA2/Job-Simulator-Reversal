using System;

public class BrainCauseStatusController
{
	private bool isCompleted;

	private BrainCause cause;

	public Action<BrainCauseStatusController> OnComplete;

	public Action<BrainCauseStatusController> OnUncomplete;

	public bool IsCompleted
	{
		get
		{
			return isCompleted;
		}
	}

	public BrainCause Cause
	{
		get
		{
			return cause;
		}
	}

	public BrainCauseStatusController(BrainCause _cause)
	{
		cause = _cause;
		isCompleted = false;
	}

	public void InstantCheckCounter(int req)
	{
		if (Cause.CauseType == BrainCause.CauseTypes.EventCustomCounterGTE && req >= Cause.NumberData)
		{
			InstantComplete();
		}
		else if (Cause.CauseType == BrainCause.CauseTypes.EventCustomCounterLTE && req <= Cause.NumberData)
		{
			InstantComplete();
		}
	}

	public void IsCounterUpdate(int req)
	{
		if (Cause.CauseType == BrainCause.CauseTypes.IsCustomCounterGTE)
		{
			if (req >= Cause.NumberData)
			{
				Complete();
			}
			else
			{
				Uncomplete();
			}
		}
		else if (Cause.CauseType == BrainCause.CauseTypes.IsCustomCounterLTE)
		{
			if (req <= Cause.NumberData)
			{
				Complete();
			}
			else
			{
				Uncomplete();
			}
		}
		if (Cause.CauseType == BrainCause.CauseTypes.IsCustomCounterEQ)
		{
			if (req == Cause.NumberData)
			{
				Complete();
			}
			else
			{
				Uncomplete();
			}
		}
	}

	public void CheckCounter(int req)
	{
		if (Cause.CauseType == BrainCause.CauseTypes.WasCustomCounterGTE && req >= Cause.NumberData)
		{
			Complete();
		}
		else if (Cause.CauseType == BrainCause.CauseTypes.WasCustomCounterLTE && req <= Cause.NumberData)
		{
			Complete();
		}
	}

	public void InstantCheckAmount(float amt)
	{
		if (amt >= Cause.Amount)
		{
			InstantComplete();
		}
	}

	public void CheckAmount(float amt)
	{
		if (amt >= Cause.Amount)
		{
			Complete();
		}
	}

	public void InstantComplete()
	{
		if (!isCompleted)
		{
			isCompleted = true;
			if (OnComplete != null)
			{
				OnComplete(this);
			}
			isCompleted = false;
		}
	}

	public void Complete()
	{
		if (!isCompleted)
		{
			isCompleted = true;
			if (OnComplete != null)
			{
				OnComplete(this);
			}
		}
	}

	public void Uncomplete()
	{
		if (isCompleted)
		{
			isCompleted = false;
			if (OnUncomplete != null)
			{
				OnUncomplete(this);
			}
		}
	}

	public void Reset()
	{
		isCompleted = false;
	}
}
