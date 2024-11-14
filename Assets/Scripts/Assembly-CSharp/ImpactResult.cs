using System;
using UnityEngine;

[Serializable]
public class ImpactResult
{
	[SerializeField]
	private GameObject gameObjectTarget;

	[SerializeField]
	private float initialForceMultiplier = 1f;

	[SerializeField]
	private Vector3 initialForceAddon = Vector3.zero;

	[SerializeField]
	private float initialTorque = 1f;

	[SerializeField]
	private bool spawnAsAdditionalObject;

	public GameObject GameObjectTarget
	{
		get
		{
			return gameObjectTarget;
		}
	}

	public bool SpawnAsAdditionalObject
	{
		get
		{
			return spawnAsAdditionalObject;
		}
	}

	public void Do(Vector3 impactRelativeVelocity, Vector3 collisionNormal)
	{
		if (!spawnAsAdditionalObject)
		{
			Rigidbody component = gameObjectTarget.GetComponent<Rigidbody>();
			if (initialForceMultiplier > 0f || initialForceAddon.magnitude > 0f)
			{
				component.AddRelativeForce(impactRelativeVelocity * initialForceMultiplier + initialForceAddon);
			}
			if (initialTorque > 0f)
			{
				component.AddRelativeTorque(initialTorque, initialTorque, initialTorque);
			}
		}
	}
}
