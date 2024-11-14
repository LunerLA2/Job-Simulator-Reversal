using System;
using System.Collections.Generic;
using UnityEngine;

public class RigidbodyEnterExitCollisionEvents : MonoBehaviour
{
	public Action<RigidbodyCollisionInfo> OnRigidbodyEnterCollision;

	public Action<RigidbodyCollisionInfo> OnRigidbodyExitCollision;

	private List<RigidbodyCollisionInfo> activeRigidbodiesCollisionInfo = new List<RigidbodyCollisionInfo>();

	public List<RigidbodyCollisionInfo> ActiveRigidbodiesCollisionInfo
	{
		get
		{
			return activeRigidbodiesCollisionInfo;
		}
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (collision.rigidbody != null)
		{
			Rigidbody rigidbody = collision.rigidbody;
			Collider collider = collision.collider;
			RigidbodyCollisionInfo rigidbodyCollisionInfo = FindRigidbodyCollisionInfo(rigidbody);
			bool flag = false;
			if (rigidbodyCollisionInfo == null)
			{
				rigidbodyCollisionInfo = new RigidbodyCollisionInfo(collision);
				activeRigidbodiesCollisionInfo.Add(rigidbodyCollisionInfo);
				rigidbodyCollisionInfo.AddCollider(collider);
				flag = true;
			}
			else if (!rigidbodyCollisionInfo.CheckIsColliderCurrentlyInside(collider))
			{
				rigidbodyCollisionInfo.AddCollider(collider);
				flag = true;
			}
			if (flag && rigidbodyCollisionInfo.GetColliderInsideCount() == 1 && OnRigidbodyEnterCollision != null)
			{
				OnRigidbodyEnterCollision(rigidbodyCollisionInfo);
			}
		}
	}

	private void OnCollisionExit(Collision collision)
	{
		Debug.Log("Exit:" + collision.collider);
		Collider collider = collision.collider;
		if (!(collision.rigidbody != null))
		{
			return;
		}
		Rigidbody rigidbody = collision.rigidbody;
		RigidbodyCollisionInfo rigidbodyCollisionInfo = FindRigidbodyCollisionInfo(rigidbody);
		if (rigidbodyCollisionInfo != null)
		{
			if (rigidbodyCollisionInfo.CheckIsColliderCurrentlyInside(collider))
			{
				rigidbodyCollisionInfo.RemoveCollider(collider);
			}
			if (rigidbodyCollisionInfo.GetColliderInsideCount() == 0)
			{
				activeRigidbodiesCollisionInfo.Remove(rigidbodyCollisionInfo);
				if (OnRigidbodyExitCollision != null)
				{
					OnRigidbodyExitCollision(rigidbodyCollisionInfo);
				}
			}
		}
		else if (rigidbody != null)
		{
			Debug.LogWarning("Error in rigidbodyExitTracker: Missing CollisionInfo:" + rigidbody.name);
		}
		else
		{
			Debug.LogWarning("Error in rigidbodyExitTracker: Missing CollisionInfo");
		}
	}

	private void Update()
	{
		if (activeRigidbodiesCollisionInfo.Count <= 0)
		{
			return;
		}
		for (int i = 0; i < activeRigidbodiesCollisionInfo.Count; i++)
		{
			if (activeRigidbodiesCollisionInfo[i].Rigidbody == null)
			{
				Debug.Log("Remove not found");
				activeRigidbodiesCollisionInfo.RemoveAt(i);
				i--;
			}
		}
	}

	private RigidbodyCollisionInfo FindRigidbodyCollisionInfo(Rigidbody r)
	{
		for (int i = 0; i < activeRigidbodiesCollisionInfo.Count; i++)
		{
			if (activeRigidbodiesCollisionInfo[i].Rigidbody == r)
			{
				return activeRigidbodiesCollisionInfo[i];
			}
		}
		return null;
	}

	public void Clear()
	{
		ActiveRigidbodiesCollisionInfo.Clear();
	}
}
