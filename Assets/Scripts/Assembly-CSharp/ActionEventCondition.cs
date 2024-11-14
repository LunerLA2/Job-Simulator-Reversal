using System;
using OwlchemyVR;
using UnityEngine;

[Serializable]
public class ActionEventCondition
{
	[SerializeField]
	private ActionEventData actionEventData;

	[SerializeField]
	private WorldItemData worldItemData1;

	[SerializeField]
	private WorldItemData worldItemData2;

	[SerializeField]
	private float amount;

	[SerializeField]
	private bool isPositive = true;

	public ActionEventData ActionEventData
	{
		get
		{
			return actionEventData;
		}
	}

	public WorldItemData WorldItemData1
	{
		get
		{
			return worldItemData1;
		}
	}

	public WorldItemData WorldItemData2
	{
		get
		{
			return worldItemData2;
		}
	}

	public float Amount
	{
		get
		{
			return amount;
		}
	}

	public bool IsPositive
	{
		get
		{
			return isPositive;
		}
	}

	public void EditorSetActionEventData(ActionEventData data)
	{
		actionEventData = data;
	}

	public void EditorSetAmount(float amt)
	{
		amount = amt;
	}

	public void EditorSetIsPositive(bool isPos)
	{
		isPositive = isPos;
	}

	public void EditorSetWorldItem(WorldItemData data, int index)
	{
		switch (index)
		{
		case 1:
			worldItemData1 = data;
			break;
		case 2:
			worldItemData2 = data;
			break;
		default:
			Debug.LogError("Can't set worldItemData of index " + index);
			break;
		}
	}

	public bool IsSetUpCorrectly()
	{
		if (actionEventData == null)
		{
			return false;
		}
		if (worldItemData1 == null)
		{
			return false;
		}
		if (actionEventData.FormatType == ActionEventData.FormatTypes.WorldItemDataAppliedToWorldItemData && worldItemData2 == null)
		{
			return false;
		}
		return true;
	}
}
