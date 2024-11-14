using System;
using System.Collections.Generic;
using OwlchemyVR;
using UnityEngine;

public class TriggerWorldItemController : MonoBehaviour
{
	public Action<WorldItem> OnWorldItemTriggerEnter;

	public Action<WorldItem> OnWorldItemTriggerExit;

	private List<WorldItemTriggerInfo> activeWorldItemsTriggerInfo = new List<WorldItemTriggerInfo>();

	private void OnTriggerEnter(Collider other)
	{
		WorldItem worldItemFromCollider = GetWorldItemFromCollider(other);
		if (worldItemFromCollider != null)
		{
			WorldItemTriggerInfo worldItemTriggerInfo = FindWorldItemTriggerInfo(worldItemFromCollider);
			if (worldItemTriggerInfo == null)
			{
				worldItemTriggerInfo = new WorldItemTriggerInfo(worldItemFromCollider);
				activeWorldItemsTriggerInfo.Add(worldItemTriggerInfo);
				worldItemTriggerInfo.AddCollider(other);
			}
			else if (!worldItemTriggerInfo.CheckIsColliderCurrentlyInside(other))
			{
				worldItemTriggerInfo.AddCollider(other);
			}
			if (worldItemTriggerInfo.GetColliderInsideCount() == 1 && OnWorldItemTriggerEnter != null)
			{
				OnWorldItemTriggerEnter(worldItemFromCollider);
			}
		}
	}

	private void OnTriggerExit(Collider other)
	{
		WorldItem worldItemFromCollider = GetWorldItemFromCollider(other);
		if (!(worldItemFromCollider != null))
		{
			return;
		}
		WorldItemTriggerInfo worldItemTriggerInfo = FindWorldItemTriggerInfo(worldItemFromCollider);
		if (worldItemTriggerInfo != null)
		{
			if (worldItemTriggerInfo.CheckIsColliderCurrentlyInside(other))
			{
				worldItemTriggerInfo.RemoveCollider(other);
			}
			if (worldItemTriggerInfo.GetColliderInsideCount() == 0)
			{
				activeWorldItemsTriggerInfo.Remove(worldItemTriggerInfo);
				if (OnWorldItemTriggerExit != null)
				{
					OnWorldItemTriggerExit(worldItemFromCollider);
				}
			}
		}
		else
		{
			Debug.LogWarning("Error in rigidbodyExitTracker: Missing TriggerInfo");
		}
	}

	private WorldItem GetWorldItemFromCollider(Collider c)
	{
		WorldItem worldItem = null;
		if (c.attachedRigidbody != null)
		{
			return c.attachedRigidbody.GetComponent<WorldItem>();
		}
		return c.GetComponent<WorldItem>();
	}

	private void Update()
	{
		if (activeWorldItemsTriggerInfo.Count <= 0)
		{
			return;
		}
		for (int i = 0; i < activeWorldItemsTriggerInfo.Count; i++)
		{
			if (activeWorldItemsTriggerInfo[i].WorldItem == null)
			{
				activeWorldItemsTriggerInfo.RemoveAt(i);
				i--;
			}
		}
	}

	private WorldItemTriggerInfo FindWorldItemTriggerInfo(WorldItem item)
	{
		for (int i = 0; i < activeWorldItemsTriggerInfo.Count; i++)
		{
			if (activeWorldItemsTriggerInfo[i].WorldItem == item)
			{
				return activeWorldItemsTriggerInfo[i];
			}
		}
		return null;
	}
}
