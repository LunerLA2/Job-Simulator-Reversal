using System;
using UnityEngine;
using UnityEngine.UI;

public class SubtaskDisplayController : MonoBehaviour
{
	private const float MAX_PARTIALLY_COMPLETED_FILL_AMOUNT = 0.88f;

	[SerializeField]
	private GameObject[] quantityBoxes;

	[SerializeField]
	private Text[] quantityLabels;

	[SerializeField]
	private Color partiallyCompletedColor;

	[SerializeField]
	private Color fullyCompletedColor;

	[SerializeField]
	private Transform counterHolder;

	private int counter;

	[SerializeField]
	private Transform completionTogglePrefab;

	private Transform[] completionToggleInstance;

	private Toggle[] toggleInstance;

	[SerializeField]
	private Image default_Left;

	[SerializeField]
	private Image default_Right;

	[SerializeField]
	private Image single_Single;

	[SerializeField]
	private Image subtaskRight_fill;

	[SerializeField]
	private Image subtaskSingle_fill;

	[SerializeField]
	private Transform subtaskLeftContainer;

	[SerializeField]
	private Transform subtaskRightContainer;

	[SerializeField]
	private Transform subtaskSingleContainer;

	private SubtaskStatusController currSubtaskStatus;

	private void AddEventsTobSubtask(SubtaskStatusController subtaskStatus)
	{
		subtaskStatus.OnComplete = (Action<SubtaskStatusController>)Delegate.Combine(subtaskStatus.OnComplete, new Action<SubtaskStatusController>(SubtaskComplete));
		subtaskStatus.OnStatusChanged = (Action<SubtaskStatusController>)Delegate.Combine(subtaskStatus.OnStatusChanged, new Action<SubtaskStatusController>(SubtaskStatusChanged));
		subtaskStatus.OnAmountStatusChanged = (Action<SubtaskStatusController, float, float>)Delegate.Combine(subtaskStatus.OnAmountStatusChanged, new Action<SubtaskStatusController, float, float>(SubtaskAmountStatusChanged));
	}

	private void RemoveEventsFromSubtask(SubtaskStatusController subtaskStatus)
	{
		subtaskStatus.OnComplete = (Action<SubtaskStatusController>)Delegate.Remove(subtaskStatus.OnComplete, new Action<SubtaskStatusController>(SubtaskComplete));
		subtaskStatus.OnStatusChanged = (Action<SubtaskStatusController>)Delegate.Remove(subtaskStatus.OnStatusChanged, new Action<SubtaskStatusController>(SubtaskStatusChanged));
		subtaskStatus.OnAmountStatusChanged = (Action<SubtaskStatusController, float, float>)Delegate.Remove(subtaskStatus.OnAmountStatusChanged, new Action<SubtaskStatusController, float, float>(SubtaskAmountStatusChanged));
	}

	public bool IsSubtaskCurrentlyDisplayed(SubtaskStatusController subtaskStatus)
	{
		return currSubtaskStatus == subtaskStatus;
	}

	public void SetNewSubtask(SubtaskStatusController subtaskStatus)
	{
		if (currSubtaskStatus != null)
		{
			RemoveEventsFromSubtask(currSubtaskStatus);
		}
		currSubtaskStatus = subtaskStatus;
		AddEventsTobSubtask(currSubtaskStatus);
		switch (currSubtaskStatus.Data.SubtaskIconLayoutType)
		{
		case SubtaskData.SubtaskIconLayoutTypes.DEFAULT:
			default_Left.sprite = currSubtaskStatus.Data.LayoutDependantSpriteOne;
			default_Right.sprite = currSubtaskStatus.Data.LayoutDependantSpriteTwo;
			subtaskLeftContainer.gameObject.SetActive(true);
			subtaskRightContainer.gameObject.SetActive(true);
			subtaskSingleContainer.gameObject.SetActive(false);
			break;
		case SubtaskData.SubtaskIconLayoutTypes.SINGLEICON:
			single_Single.sprite = currSubtaskStatus.Data.LayoutDependantSpriteOne;
			subtaskLeftContainer.gameObject.SetActive(false);
			subtaskRightContainer.gameObject.SetActive(false);
			subtaskSingleContainer.gameObject.SetActive(true);
			break;
		}
		if (completionToggleInstance != null)
		{
			for (int i = 0; i < completionToggleInstance.Length; i++)
			{
				if (completionToggleInstance[i] != null)
				{
					UnityEngine.Object.Destroy(completionToggleInstance[i].gameObject);
				}
			}
			completionToggleInstance = null;
		}
		if (toggleInstance != null)
		{
			toggleInstance = null;
		}
		completionToggleInstance = new Transform[currSubtaskStatus.Data.CounterSize];
		toggleInstance = new Toggle[currSubtaskStatus.Data.CounterSize];
		for (int j = 0; j < currSubtaskStatus.Data.CounterSize; j++)
		{
			completionToggleInstance[j] = UnityEngine.Object.Instantiate(completionTogglePrefab);
			completionToggleInstance[j].SetParent(counterHolder, false);
			toggleInstance[j] = completionToggleInstance[j].GetComponent<Toggle>();
		}
		SetQuantityBoxState(currSubtaskStatus.Data.CounterSize > 1 && !currSubtaskStatus.Data.HideCounterOnJobBoard);
		SetQuantityLabel("x" + currSubtaskStatus.Data.CounterSize);
		SetSubtaskStatus(subtaskStatus);
	}

	private void SubtaskComplete(SubtaskStatusController subtaskStatus)
	{
		SetSubtaskStatus(subtaskStatus);
	}

	private void SubtaskStatusChanged(SubtaskStatusController subtaskStatus)
	{
		SetSubtaskStatus(subtaskStatus);
	}

	private void SetSubtaskStatus(SubtaskStatusController subtaskStatus)
	{
		if (subtaskStatus.Data.CounterSize > 0)
		{
			if (toggleInstance != null)
			{
				for (int i = 0; i < subtaskStatus.Data.CounterSize; i++)
				{
					toggleInstance[i].isOn = subtaskStatus.CurrentCount > i || subtaskStatus.IsCompleted;
				}
			}
			else
			{
				Debug.LogError("Subtask " + subtaskStatus.Data.name + " has a counterSize but no toggles were created - something has gone wrong.");
			}
		}
		float amt = 0f;
		if (subtaskStatus.Data.CounterSize > 1)
		{
			amt = (float)subtaskStatus.CurrentCount / (float)subtaskStatus.Data.CounterSize;
		}
		if (subtaskStatus.IsCompleted)
		{
			amt = 1f;
		}
		UpdateFills(amt);
	}

	private void SubtaskAmountStatusChanged(SubtaskStatusController subtaskStatus, float amount, float neededAmount)
	{
		float amt = amount / neededAmount;
		if (subtaskStatus.IsCompleted)
		{
			amt = 1f;
		}
		UpdateFills(amt);
	}

	private void UpdateFills(float amt)
	{
		subtaskRight_fill.color = ((amt != 1f) ? partiallyCompletedColor : fullyCompletedColor);
		subtaskSingle_fill.color = ((amt != 1f) ? partiallyCompletedColor : fullyCompletedColor);
		if (amt == 1f)
		{
			subtaskRight_fill.fillAmount = 1f;
			subtaskSingle_fill.fillAmount = 1f;
		}
		else
		{
			subtaskRight_fill.fillAmount = amt * 0.88f;
			subtaskSingle_fill.fillAmount = amt * 0.88f;
		}
		JobBoardManager.instance.MarkAsDirty();
	}

	private void SetQuantityLabel(string t)
	{
		for (int i = 0; i < quantityLabels.Length; i++)
		{
			quantityLabels[i].text = t;
		}
	}

	private void SetQuantityBoxState(bool s)
	{
		for (int i = 0; i < quantityBoxes.Length; i++)
		{
			quantityBoxes[i].SetActive(s);
		}
	}

	public void ResetUIComponents()
	{
		if (toggleInstance != null)
		{
			for (int i = 0; i < toggleInstance.Length; i++)
			{
				toggleInstance[i].isOn = false;
			}
		}
		subtaskRight_fill.fillAmount = 0f;
		subtaskSingle_fill.fillAmount = 0f;
	}

	public void Initialize()
	{
	}
}
