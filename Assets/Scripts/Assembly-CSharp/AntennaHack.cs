using System;
using OwlchemyVR;
using UnityEngine;

public class AntennaHack : MonoBehaviour
{
	[SerializeField]
	private PickupableItem pickupableItem;

	[SerializeField]
	private float breakDistance;

	[SerializeField]
	private float breakHeight;

	private void OnEnable()
	{
		PickupableItem obj = pickupableItem;
		obj.OnReleasedWasNotSwappedBetweenHands = (Action<GrabbableItem>)Delegate.Combine(obj.OnReleasedWasNotSwappedBetweenHands, new Action<GrabbableItem>(Release));
	}

	private void OnDisable()
	{
		PickupableItem obj = pickupableItem;
		obj.OnReleasedWasNotSwappedBetweenHands = (Action<GrabbableItem>)Delegate.Remove(obj.OnReleasedWasNotSwappedBetweenHands, new Action<GrabbableItem>(Release));
	}

	private void LateUpdate()
	{
		if (pickupableItem.IsCurrInHand)
		{
			base.transform.LookAt(pickupableItem.CurrInteractableHand.transform);
			if (Vector3.Distance(pickupableItem.CurrInteractableHand.transform.position, base.transform.position) > breakDistance)
			{
				pickupableItem.CurrInteractableHand.ManuallyReleaseJoint();
			}
			if (pickupableItem.CurrInteractableHand.transform.position.y < base.transform.position.y + breakHeight)
			{
				pickupableItem.CurrInteractableHand.ManuallyReleaseJoint();
			}
		}
	}

	private void Release(GrabbableItem item)
	{
	}

	private void OnDrawGizmos()
	{
		Gizmos.DrawWireSphere(base.transform.position, breakDistance);
		Vector3 position = base.transform.position;
		position.y += breakHeight;
		Vector3 size = Vector3.one * breakDistance * 0.5f;
		size.y = 0.0001f;
		Gizmos.DrawWireCube(position, size);
	}
}
