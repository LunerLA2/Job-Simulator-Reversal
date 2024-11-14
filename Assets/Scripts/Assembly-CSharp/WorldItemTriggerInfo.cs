using System.Collections.Generic;
using OwlchemyVR;
using UnityEngine;

public class WorldItemTriggerInfo
{
	private List<Collider> collidersInside = new List<Collider>();

	public WorldItem WorldItem { get; private set; }

	public WorldItemTriggerInfo(WorldItem worldItem)
	{
		WorldItem = worldItem;
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
}
