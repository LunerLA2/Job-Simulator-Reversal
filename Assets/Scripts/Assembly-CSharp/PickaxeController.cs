using System;
using OwlchemyVR;
using UnityEngine;

public class PickaxeController : MonoBehaviour
{
	[SerializeField]
	private Collider[] tipColliders;

	[SerializeField]
	private GrabbableItem grabbableItem;

	private void OnCollisionEnter(Collision collision)
	{
		if (!grabbableItem.IsCurrInHand || collision.rigidbody == null || collision.relativeVelocity.magnitude < 0.7f)
		{
			return;
		}
		SculptureController component = collision.rigidbody.GetComponent<SculptureController>();
		if (component == null)
		{
			return;
		}
		for (int i = 0; i < collision.contacts.Length; i++)
		{
			if (Array.IndexOf(tipColliders, collision.contacts[i].thisCollider) != -1)
			{
				component.PickaxeHit();
				break;
			}
		}
	}
}
