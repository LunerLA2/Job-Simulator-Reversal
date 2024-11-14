using System.Collections.Generic;
using OwlchemyVR;

public class GroupMasterPickupableItem : PickupableItem
{
	private List<PickupableItem> pickupableChildren = new List<PickupableItem>();

	public void SetupPickupableChildren()
	{
		PickupableItem[] componentsInChildren = GetComponentsInChildren<PickupableItem>();
		pickupableChildren.Clear();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			if (componentsInChildren[i] != this)
			{
				pickupableChildren.Add(componentsInChildren[i]);
			}
		}
	}
}
