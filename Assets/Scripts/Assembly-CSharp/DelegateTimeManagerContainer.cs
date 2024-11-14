public class DelegateTimeManagerContainer
{
	public enum ParameterTypes
	{
		None = 0,
		Int = 1
	}

	public ParameterTypes ParameterType;

	public float DelayUntilSeconds { get; private set; }

	public TimeManager.DelegateTimeComplete DelegateTimeComplete { get; private set; }

	public TimeManager.DelegateTimeComplete_Int DelegateTimeCompleteInt { get; private set; }

	public int ParameterInt { get; private set; }

	public DelegateTimeManagerContainer()
	{
		Clear();
	}

	public DelegateTimeManagerContainer Init(TimeManager.DelegateTimeComplete delegateTimeComplete, float delayUntilSeconds)
	{
		DelegateTimeComplete = delegateTimeComplete;
		DelayUntilSeconds = delayUntilSeconds;
		ParameterType = ParameterTypes.None;
		return this;
	}

	public DelegateTimeManagerContainer Init(TimeManager.DelegateTimeComplete_Int delegateTimeCompleteInt, int value, float delayUntilSeconds)
	{
		DelegateTimeCompleteInt = delegateTimeCompleteInt;
		DelayUntilSeconds = delayUntilSeconds;
		ParameterType = ParameterTypes.Int;
		ParameterInt = value;
		return this;
	}

	public void Clear()
	{
		DelegateTimeComplete = null;
		DelegateTimeCompleteInt = null;
		DelayUntilSeconds = 0f;
		ParameterType = ParameterTypes.None;
		ParameterInt = 0;
	}

	public void Invoke()
	{
		if (ParameterType == ParameterTypes.None)
		{
			if (DelegateTimeComplete != null && DelegateTimeComplete.Target != null)
			{
				DelegateTimeComplete();
			}
		}
		else if (ParameterType == ParameterTypes.Int && DelegateTimeCompleteInt != null && DelegateTimeCompleteInt.Target != null)
		{
			DelegateTimeCompleteInt(ParameterInt);
		}
	}

	public bool IsTheSame(TimeManager.DelegateTimeComplete delegateTimeComplete)
	{
		if (ParameterType == ParameterTypes.None && DelegateTimeComplete == delegateTimeComplete)
		{
			return true;
		}
		return false;
	}

	public bool IsTheSame(TimeManager.DelegateTimeComplete_Int delegateTimeCompleteInt, int value)
	{
		if (ParameterType == ParameterTypes.Int && DelegateTimeCompleteInt == delegateTimeCompleteInt && ParameterInt == value)
		{
			return true;
		}
		return false;
	}
}
