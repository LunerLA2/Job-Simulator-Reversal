using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class RigidbodyTriggerInfo
{
	private List<Collider> collidersInside = new List<Collider>();

	private int cachedCollidersCount;

	public Rigidbody Rigidbody { get; private set; }

	public RigidbodyTriggerInfo(Rigidbody rigidbody)
	{
		Rigidbody = rigidbody;
	}

	public int GetColliderInsideCount()
	{
		return cachedCollidersCount;
	}

	public bool CheckIsColliderCurrentlyInside(Collider c)
	{
		return collidersInside.Contains(c);
	}

	public void AddCollider(Collider c)
	{
		collidersInside.Add(c);
		cachedCollidersCount = collidersInside.Count;
	}

	public void RemoveCollider(Collider c)
	{
		collidersInside.Remove(c);
		cachedCollidersCount = collidersInside.Count;
	}

	public void RevalidateColliders()
	{
		Collider collider = null;
		for (int i = 0; i < cachedCollidersCount; i++)
		{
			collider = collidersInside[i];
			if (collider == null || !collider.enabled || !collider.gameObject.activeInHierarchy || collider.attachedRigidbody != Rigidbody)
			{
				collidersInside.RemoveAt(i);
				cachedCollidersCount = collidersInside.Count;
				i--;
			}
		}
	}
}
