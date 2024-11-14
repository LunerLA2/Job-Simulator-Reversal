using System;
using System.Collections.Generic;
using OwlchemyVR;
using UnityEngine;

public class ChangeParentOfSpawnedObjectsWhenGrabbed : MonoBehaviour
{
	[SerializeField]
	private BasePrefabSpawner[] spawners;

	[SerializeField]
	private Transform setParentTo;

	private List<GrabbableItem> trackedItems = new List<GrabbableItem>();

	private void OnEnable()
	{
		for (int i = 0; i < spawners.Length; i++)
		{
			GrabbableItem component = spawners[i].LastSpawnedPrefabGO.GetComponent<GrabbableItem>();
			if (component != null)
			{
				component.OnGrabbed = (Action<GrabbableItem>)Delegate.Combine(component.OnGrabbed, new Action<GrabbableItem>(Grabbed));
				trackedItems.Add(component);
			}
		}
	}

	private void OnDisable()
	{
		for (int i = 0; i < trackedItems.Count; i++)
		{
			if (trackedItems[i] != null)
			{
				GrabbableItem grabbableItem = trackedItems[i];
				grabbableItem.OnGrabbed = (Action<GrabbableItem>)Delegate.Remove(grabbableItem.OnGrabbed, new Action<GrabbableItem>(Grabbed));
			}
		}
	}

	private void Grabbed(GrabbableItem item)
	{
		item.OnGrabbed = (Action<GrabbableItem>)Delegate.Remove(item.OnGrabbed, new Action<GrabbableItem>(Grabbed));
		if (setParentTo != null)
		{
			item.transform.SetParent(setParentTo, true);
		}
		else
		{
			item.transform.SetParent(GlobalStorage.Instance.ContentRoot, true);
		}
	}
}
