using OwlchemyVR;
using UnityEngine;

public class SpawnPrefabOnCollision : MonoBehaviour
{
	[SerializeField]
	private float breakForce;

	[SerializeField]
	private SpawnedByCollision prefabToSpawn;

	[SerializeField]
	private bool destroyThisObject = true;

	[SerializeField]
	private PickupableItem pickupable;

	private bool isBroken;

	private void OnCollisionEnter(Collision col)
	{
		if (isBroken)
		{
			return;
		}
		float num = 0f;
		num = ((!pickupable.IsCurrInHand) ? col.relativeVelocity.magnitude : pickupable.CurrInteractableHand.GetCurrentVelocity().magnitude);
		if (num > breakForce)
		{
			if (pickupable.IsCurrInHand)
			{
				pickupable.CurrInteractableHand.ManuallyReleaseJoint();
			}
			Vector3 collisionNormal = base.transform.eulerAngles;
			if (col.contacts.Length > 0)
			{
				collisionNormal = col.contacts[0].normal;
			}
			else
			{
				Debug.LogWarning("Could not find contacts to calculate normal");
			}
			DoCollision(col.relativeVelocity, collisionNormal);
		}
	}

	private void DoCollision(Vector3 collisionRelativeVelocity, Vector3 collisionNormal)
	{
		SpawnedByCollision spawnedByCollision = (SpawnedByCollision)Object.Instantiate(prefabToSpawn, base.transform.position, base.transform.rotation);
		spawnedByCollision.SetImpactDetails(collisionRelativeVelocity, collisionNormal);
		if (destroyThisObject)
		{
			isBroken = true;
			Object.Destroy(base.gameObject);
		}
	}
}
