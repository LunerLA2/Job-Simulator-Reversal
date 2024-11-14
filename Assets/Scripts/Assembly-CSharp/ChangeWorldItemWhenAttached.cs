using System;
using OwlchemyVR;
using UnityEngine;

public class ChangeWorldItemWhenAttached : MonoBehaviour
{
	public enum CompletionCondition
	{
		AllPoints = 0,
		AnyPoint = 1
	}

	[SerializeField]
	private bool sendCreateAndDestroyEvents;

	[SerializeField]
	private AttachablePoint[] attachablePoints;

	[SerializeField]
	private bool acceptAnyAttachedWorldData = true;

	[SerializeField]
	private WorldItemData[] validAttachedWorldDatas;

	[SerializeField]
	private CompletionCondition completionCondition;

	[SerializeField]
	private WorldItem myWorldItemToChange;

	[SerializeField]
	private WorldItemData myIncompleteWorldItemData;

	[SerializeField]
	private WorldItemData myCompleteWorldItemData;

	private void OnEnable()
	{
		for (int i = 0; i < attachablePoints.Length; i++)
		{
			attachablePoints[i].OnObjectWasAttached += Attached;
			attachablePoints[i].OnObjectWasDetached += Detached;
		}
	}

	private void OnDisable()
	{
		for (int i = 0; i < attachablePoints.Length; i++)
		{
			attachablePoints[i].OnObjectWasAttached -= Attached;
			attachablePoints[i].OnObjectWasDetached -= Detached;
		}
	}

	private void Attached(AttachablePoint point, AttachableObject o)
	{
		CheckCompletionState();
	}

	private void Detached(AttachablePoint point, AttachableObject o)
	{
		CheckCompletionState();
	}

	private void CheckCompletionState()
	{
		bool flag = myWorldItemToChange.Data == myCompleteWorldItemData;
		bool flag2 = false;
		bool flag3 = true;
		for (int i = 0; i < attachablePoints.Length; i++)
		{
			AttachablePoint attachablePoint = attachablePoints[i];
			bool flag4 = false;
			for (int j = 0; j < attachablePoint.AttachedObjects.Count; j++)
			{
				AttachableObject attachableObject = attachablePoint.AttachedObjects[j];
				if (acceptAnyAttachedWorldData)
				{
					flag4 = true;
					break;
				}
				WorldItem component = attachableObject.GetComponent<WorldItem>();
				if (component != null && component.Data != null && Array.IndexOf(validAttachedWorldDatas, component.Data) != -1)
				{
					flag4 = true;
					break;
				}
			}
			if (completionCondition == CompletionCondition.AllPoints && !flag4)
			{
				flag3 = false;
				break;
			}
			if (completionCondition == CompletionCondition.AnyPoint && flag4)
			{
				flag2 = true;
				break;
			}
		}
		if (completionCondition == CompletionCondition.AllPoints)
		{
			flag2 = flag3;
		}
		if (!flag && flag2)
		{
			myWorldItemToChange.ManualSetData(myCompleteWorldItemData);
			if (sendCreateAndDestroyEvents)
			{
				GameEventsManager.Instance.ItemActionOccurred(myCompleteWorldItemData, "CREATED");
			}
		}
		else if (flag && !flag2)
		{
			if (sendCreateAndDestroyEvents)
			{
				GameEventsManager.Instance.ItemActionOccurred(myCompleteWorldItemData, "DESTROYED");
			}
			myWorldItemToChange.ManualSetData(myIncompleteWorldItemData);
		}
	}
}
