using System.Collections.Generic;
using UnityEngine;

public class CompoundEdibleItem : BasicEdibleItem
{
	[SerializeField]
	private AttachablePoint[] attachpointsToWatchForChildEdibles;

	private List<BasicEdibleItem> childEdibles = new List<BasicEdibleItem>();

	private void OnEnable()
	{
		for (int i = 0; i < attachpointsToWatchForChildEdibles.Length; i++)
		{
			attachpointsToWatchForChildEdibles[i].OnObjectWasAttached += ObjectAttached;
			attachpointsToWatchForChildEdibles[i].OnObjectWasDetached += ObjectDetached;
		}
	}

	private void OnDisable()
	{
		for (int i = 0; i < attachpointsToWatchForChildEdibles.Length; i++)
		{
			attachpointsToWatchForChildEdibles[i].OnObjectWasAttached -= ObjectAttached;
			attachpointsToWatchForChildEdibles[i].OnObjectWasDetached -= ObjectDetached;
		}
	}

	private void ObjectAttached(AttachablePoint point, AttachableObject obj)
	{
		BasicEdibleItem component = obj.PickupableItem.gameObject.GetComponent<BasicEdibleItem>();
		if (component != null && !childEdibles.Contains(component))
		{
			childEdibles.Add(component);
		}
	}

	private void ObjectDetached(AttachablePoint point, AttachableObject obj)
	{
		BasicEdibleItem component = obj.PickupableItem.gameObject.GetComponent<BasicEdibleItem>();
		if (component != null && childEdibles.Contains(component))
		{
			childEdibles.Remove(component);
		}
	}

	public override BiteResultInfo TakeBiteAndGetResult(HeadController head)
	{
		int num = -1;
		List<EdibleItem> list = new List<EdibleItem>();
		List<EdibleItem> list2 = new List<EdibleItem>();
		for (int i = 0; i < childEdibles.Count; i++)
		{
			if (childEdibles[i] != null && childEdibles[i].NumBitesTaken < base.NumBitesTaken)
			{
				num = i;
				break;
			}
		}
		if (num > -1)
		{
			return childEdibles[num].TakeBiteAndGetResult(head);
		}
		for (int j = 0; j < childEdibles.Count; j++)
		{
			if (!(childEdibles[j] != null) || childEdibles[j].NumBitesTaken != base.NumBitesTaken)
			{
				continue;
			}
			BiteResultInfo biteResultInfo = childEdibles[j].TakeBiteAndGetResult(head);
			if (biteResultInfo != null)
			{
				if (biteResultInfo.WasMainItemFullyConsumed)
				{
					list2.Add(childEdibles[j]);
				}
				else
				{
					list.Add(childEdibles[j]);
				}
			}
		}
		BiteResultInfo biteResultInfo2 = base.TakeBiteAndGetResult(head);
		return new BiteResultInfo(biteResultInfo2.WasMainItemFullyConsumed, list.ToArray(), list2.ToArray());
	}
}
