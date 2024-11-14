using System.Collections.Generic;
using UnityEngine;

public class BlowableTriggerInfo
{
	private List<Collider> collidersInside = new List<Collider>();

	public BlowableItem BlowableItem { get; private set; }

	public BlowableTriggerInfo(BlowableItem blowableItem)
	{
		BlowableItem = blowableItem;
	}

	public int GetColliderInsideCount()
	{
		return collidersInside.Count;
	}

	public bool CheckIsColliderCurrentlyInside(Collider c)
	{
		return collidersInside.Contains(c);
	}

	public void AddCollider(Collider c)
	{
		collidersInside.Add(c);
	}

	public void RemoveCollider(Collider c)
	{
		collidersInside.Remove(c);
	}

	public void RevalidateColliders()
	{
		for (int i = 0; i < collidersInside.Count; i++)
		{
			if (collidersInside[i] == null || !collidersInside[i].enabled || !collidersInside[i].gameObject.activeInHierarchy)
			{
				collidersInside.RemoveAt(i);
				i--;
			}
		}
	}
}
