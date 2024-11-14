using System;
using OwlchemyVR;
using UnityEngine;

public class TwistableItem : MonoBehaviour
{
	private const float MAX_OFFSET = 0.15f;

	[SerializeField]
	private GrabbableItem grabbableItem;

	[SerializeField]
	private bool twistOnX = true;

	[SerializeField]
	private bool twistOnY = true;

	[SerializeField]
	private bool twistOnZ = true;

	private Vector3 initialLocalGrabPos;

	private Quaternion relativeRotAtGrab;

	private Transform handTransform;

	private Vector3 originalLocalEulerAngles;

	private void Awake()
	{
		originalLocalEulerAngles = base.transform.localEulerAngles;
	}

	private void OnEnable()
	{
		GrabbableItem obj = grabbableItem;
		obj.OnGrabbed = (Action<GrabbableItem>)Delegate.Combine(obj.OnGrabbed, new Action<GrabbableItem>(Grabbed));
		GrabbableItem obj2 = grabbableItem;
		obj2.OnGrabbedUpdate = (Action<GrabbableItem>)Delegate.Combine(obj2.OnGrabbedUpdate, new Action<GrabbableItem>(GrabbedUpdate));
	}

	private void OnDisable()
	{
		GrabbableItem obj = grabbableItem;
		obj.OnGrabbed = (Action<GrabbableItem>)Delegate.Remove(obj.OnGrabbed, new Action<GrabbableItem>(Grabbed));
		GrabbableItem obj2 = grabbableItem;
		obj2.OnGrabbedUpdate = (Action<GrabbableItem>)Delegate.Remove(obj2.OnGrabbedUpdate, new Action<GrabbableItem>(GrabbedUpdate));
	}

	private void Grabbed(GrabbableItem item)
	{
		handTransform = grabbableItem.CurrInteractableHand.transform;
		initialLocalGrabPos = base.transform.InverseTransformPoint(handTransform.position);
		relativeRotAtGrab = Quaternion.Inverse(handTransform.rotation) * base.transform.rotation;
	}

	private void GrabbedUpdate(GrabbableItem item)
	{
		Vector3 vector = base.transform.InverseTransformPoint(handTransform.position);
		if (base.transform.TransformVector(vector - initialLocalGrabPos).magnitude > 0.15f)
		{
			grabbableItem.CurrInteractableHand.TryRelease(false);
			return;
		}
		base.transform.rotation = handTransform.rotation * relativeRotAtGrab;
		if (!twistOnX || !twistOnY || !twistOnZ)
		{
			base.transform.localEulerAngles = new Vector3((!twistOnX) ? originalLocalEulerAngles.x : base.transform.localEulerAngles.x, (!twistOnY) ? originalLocalEulerAngles.y : base.transform.localEulerAngles.y, (!twistOnZ) ? originalLocalEulerAngles.z : base.transform.localEulerAngles.z);
		}
	}
}
