using System;
using OwlchemyVR;
using UnityEngine;

public class ResizingHandle : MonoBehaviour
{
	[SerializeField]
	private GrabbableItem handle;

	private InteractionHandController hand;

	private float timeSinceGrab;

	[SerializeField]
	private Transform scaleController;

	private float breakDistance = 0.125f;

	private float distance;

	private float maxScale = 1.2f;

	private float minScale = 0.15f;

	private Vector3 grabDelta;

	private Vector3 targetPosition;

	public GrabbableItem Handle
	{
		get
		{
			return handle;
		}
	}

	private void OnEnable()
	{
		GrabbableItem grabbableItem = handle;
		grabbableItem.OnGrabbed = (Action<GrabbableItem>)Delegate.Combine(grabbableItem.OnGrabbed, new Action<GrabbableItem>(Grabbed));
	}

	private void OnDisable()
	{
		GrabbableItem grabbableItem = handle;
		grabbableItem.OnGrabbed = (Action<GrabbableItem>)Delegate.Remove(grabbableItem.OnGrabbed, new Action<GrabbableItem>(Grabbed));
	}

	private void Grabbed(GrabbableItem item)
	{
		hand = handle.CurrInteractableHand;
		timeSinceGrab = Time.time;
		grabDelta = hand.transform.position - base.transform.position;
	}

	private void Update()
	{
		if (handle.IsCurrInHand)
		{
			targetPosition = hand.transform.position - grabDelta;
			base.transform.position = targetPosition;
			base.transform.localPosition = new Vector3(base.transform.localPosition.x, base.transform.localPosition.y, 0f);
			distance = Vector3.Distance(scaleController.position, base.transform.position);
			scaleController.localScale = Vector3.one * Mathf.Clamp(distance, minScale, maxScale);
			if (Vector3.Distance(base.transform.position, targetPosition) > breakDistance)
			{
				hand.TryRelease();
			}
		}
		else if (base.transform.localPosition != Vector3.zero)
		{
			base.transform.localPosition = Vector3.zero;
		}
	}
}
