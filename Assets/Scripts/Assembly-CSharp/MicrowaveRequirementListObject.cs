using System;
using System.Collections.Generic;
using OwlchemyVR;
using UnityEngine;

[Serializable]
public class MicrowaveRequirementListObject
{
	[SerializeField]
	protected WorldItemData[] requiredPhysicalItems;

	[Tooltip("If true, ActionEvents.DESTROYED will happen on the ingredients when they are combined")]
	[SerializeField]
	protected bool physicalItemsMarkedDestroyed = true;

	public WorldItemData[] RequiredPhysicalItems
	{
		get
		{
			return requiredPhysicalItems;
		}
	}

	public bool ShouldItemsBeMarkedDestroyed
	{
		get
		{
			return physicalItemsMarkedDestroyed;
		}
	}

	public List<PickupableItem> CheckIfRequirementsAreMet(List<PickupableItem> availableItems)
	{
		List<PickupableItem> list = new List<PickupableItem>();
		List<WorldItemData> list2 = new List<WorldItemData>();
		list2.AddRange(requiredPhysicalItems);
		for (int i = 0; i < availableItems.Count; i++)
		{
			if (!(availableItems[i] != null))
			{
				continue;
			}
			WorldItem component = availableItems[i].GetComponent<WorldItem>();
			if (component != null && list2.Contains(component.Data))
			{
				list2.Remove(component.Data);
				list.Add(availableItems[i]);
				if (list2.Count == 0)
				{
					return list;
				}
			}
		}
		return list;
	}
}
