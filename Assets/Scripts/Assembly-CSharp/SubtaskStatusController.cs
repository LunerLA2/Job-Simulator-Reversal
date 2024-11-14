using System;

public class SubtaskStatusController
{
	private bool isCompleted;

	private bool isCompletionLocked;

	private int currentCount;

	private bool _isSuccess = true;

	private bool _isSkipped;

	private SubtaskData subtaskData;

	public Action<SubtaskStatusController> OnStatusChanged;

	public Action<SubtaskStatusController, float, float> OnAmountStatusChanged;

	public Action<SubtaskStatusController> OnComplete;

	public Action<SubtaskStatusController> OnUncomplete;

	public Action<SubtaskStatusController, bool> OnCounterChange;

	public bool IsCompleted
	{
		get
		{
			return isCompleted;
		}
	}

	public int CurrentCount
	{
		get
		{
			return currentCount;
		}
	}

	public bool isSuccess
	{
		get
		{
			return _isSuccess;
		}
	}

	public bool isSkipped
	{
		get
		{
			return _isSkipped;
		}
	}

	public SubtaskData Data
	{
		get
		{
			return subtaskData;
		}
	}

	public SubtaskStatusController(SubtaskData subtaskData)
	{
		this.subtaskData = subtaskData;
		currentCount = 0;
		isCompleted = false;
	}

	public void AutoComplete(bool disallowUncompletingFromThisPointOn = false)
	{
		if (disallowUncompletingFromThisPointOn)
		{
			isCompletionLocked = true;
		}
		if (!isCompleted)
		{
			currentCount = Data.CounterSize;
			if (currentCount == 0)
			{
				currentCount = 1;
			}
			isCompleted = true;
			if (OnComplete != null)
			{
				OnComplete(this);
			}
		}
	}

	public void PositiveActionOccurred(ActionEventCondition actionEventCondition)
	{
		if (isCompletionLocked)
		{
			return;
		}
		currentCount++;
		if (isCompleted)
		{
			return;
		}
		if (currentCount >= Data.CounterSize)
		{
			isCompleted = true;
			if (OnComplete != null)
			{
				OnComplete(this);
			}
			return;
		}
		if (OnStatusChanged != null)
		{
			OnStatusChanged(this);
		}
		if (OnCounterChange != null)
		{
			OnCounterChange(this, true);
		}
	}

	public void PositiveActionOccurredWithAmount(ActionEventCondition actionEventCondition, float amount)
	{
		if (isCompletionLocked || isCompleted)
		{
			return;
		}
		if (amount >= actionEventCondition.Amount)
		{
			isCompleted = true;
			if (OnComplete != null)
			{
				OnComplete(this);
			}
		}
		else if (OnAmountStatusChanged != null)
		{
			OnAmountStatusChanged(this, amount, actionEventCondition.Amount);
		}
	}

	public void NegativeActionOccurred(ActionEventCondition actionEventCondition)
	{
		if (isCompletionLocked)
		{
			return;
		}
		if (subtaskData.CounterSize > 0)
		{
			if (currentCount <= 0)
			{
				return;
			}
			currentCount--;
		}
		if (isCompleted)
		{
			bool flag = true;
			if (subtaskData.CounterSize == 0)
			{
				flag = true;
			}
			else if (currentCount >= subtaskData.CounterSize)
			{
				flag = false;
			}
			if (flag)
			{
				isCompleted = false;
				if (OnUncomplete != null)
				{
					OnUncomplete(this);
				}
			}
		}
		if (OnStatusChanged != null)
		{
			OnStatusChanged(this);
		}
		if (OnCounterChange != null)
		{
			OnCounterChange(this, false);
		}
	}

	public void NegativeActionOccurredWithAmount(ActionEventCondition actionEventCondition, float amount)
	{
		if (isCompletionLocked)
		{
			return;
		}
		if (isCompleted && amount < actionEventCondition.Amount)
		{
			isCompleted = false;
			if (OnUncomplete != null)
			{
				OnUncomplete(this);
			}
		}
		if (OnAmountStatusChanged != null)
		{
			OnAmountStatusChanged(this, amount, actionEventCondition.Amount);
		}
	}

	public void ForceUncomplete()
	{
		if (isCompleted)
		{
			isCompletionLocked = false;
			isCompleted = false;
			if (OnUncomplete != null)
			{
				OnUncomplete(this);
			}
			if (OnStatusChanged != null)
			{
				OnStatusChanged(this);
			}
			if (OnCounterChange != null)
			{
				OnCounterChange(this, false);
			}
		}
	}

	public void ForceComplete(bool success = true, bool skipped = false)
	{
		_isSkipped = skipped;
		_isSuccess = success;
		AutoComplete();
	}
}
