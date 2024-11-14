using System;
using UnityEngine;

public class CollisionEvents : MonoBehaviour
{
	public Action<Collision> OnEnterCollision;

	public Action<Collision> OnExitCollision;

	private void OnCollisionEnter(Collision collisionInfo)
	{
		if (OnEnterCollision != null)
		{
			OnEnterCollision(collisionInfo);
		}
	}

	private void OnCollisionExit(Collision collisionInfo)
	{
		if (OnExitCollision != null)
		{
			OnExitCollision(collisionInfo);
		}
	}
}
