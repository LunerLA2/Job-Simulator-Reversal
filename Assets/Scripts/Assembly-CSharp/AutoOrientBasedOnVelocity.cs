using System;
using OwlchemyVR;
using UnityEngine;

public class AutoOrientBasedOnVelocity : MonoBehaviour
{
	[SerializeField]
	private PickupableItem pickupableItem;

	[SerializeField]
	private bool orientWhenReleased = true;

	[SerializeField]
	private bool orientConstantly;

	[SerializeField]
	private float orientSmoothingSpeed = 45f;

	[SerializeField]
	private float onlyOrientIfVelocityHigherThan;

	[SerializeField]
	private Vector3 forwardAxis = Vector3.forward;

	[SerializeField]
	private Vector3 rightAxis = Vector3.right;

	private Rigidbody rb;

	private Quaternion rotationOffset;

	private void Awake()
	{
		rb = GetComponent<Rigidbody>();
		Matrix4x4 identity = Matrix4x4.identity;
		identity.SetColumn(0, rightAxis);
		identity.SetColumn(1, Vector3.Cross(forwardAxis, rightAxis));
		identity.SetColumn(2, forwardAxis);
		rotationOffset = Quaternion.Inverse(identity.GetRotation());
	}

	private void OnEnable()
	{
		PickupableItem obj = pickupableItem;
		obj.OnReleased = (Action<GrabbableItem>)Delegate.Combine(obj.OnReleased, new Action<GrabbableItem>(Released));
	}

	private void OnDisable()
	{
		PickupableItem obj = pickupableItem;
		obj.OnReleased = (Action<GrabbableItem>)Delegate.Remove(obj.OnReleased, new Action<GrabbableItem>(Released));
	}

	private void Released(GrabbableItem item)
	{
		if (orientWhenReleased)
		{
			Orient();
		}
	}

	private void Update()
	{
		if (orientConstantly && !pickupableItem.IsCurrInHand)
		{
			Orient();
		}
	}

	private void Orient()
	{
		if (!(rb == null) && rb.velocity.magnitude >= onlyOrientIfVelocityHigherThan)
		{
			float maxRadiansDelta = Time.deltaTime * ((float)Math.PI / 180f) * orientSmoothingSpeed;
			Vector3 normalized = rb.velocity.normalized;
			Vector3 target = ((!(normalized == Vector3.up) && !(normalized == Vector3.down)) ? Vector3.Cross(Vector3.up, normalized).normalized : Vector3.right);
			Vector3 current = base.transform.TransformDirection(forwardAxis);
			Vector3 current2 = base.transform.TransformDirection(rightAxis);
			Vector3 vector = Vector3.RotateTowards(current, normalized, maxRadiansDelta, 0f);
			Vector3 vector2 = Vector3.RotateTowards(current2, target, maxRadiansDelta, 0f);
			Matrix4x4 identity = Matrix4x4.identity;
			identity.SetColumn(0, vector2);
			identity.SetColumn(1, Vector3.Cross(vector, vector2));
			identity.SetColumn(2, vector);
			base.transform.rotation = identity.GetRotation() * rotationOffset;
		}
	}
}
