using System.Collections.Generic;
using UnityEngine;

public class RigidbodyCollisionInfo
{
	private List<Collider> collidersInside = new List<Collider>();

	public Rigidbody Rigidbody { get; private set; }

	public Collision InitCollision { get; private set; }

	public RigidbodyCollisionInfo(Collision initCollision)
	{
		InitCollision = initCollision;
		Rigidbody = initCollision.rigidbody;
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
