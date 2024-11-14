using System;
using OwlchemyVR;
using UnityEngine;

public class ToggleConstantForceBasedOnInHand : MonoBehaviour
{
	[SerializeField]
	private ConstantForce constantForceComponent;

	[SerializeField]
	private PickupableItem pickupableItem;

	[SerializeField]
	private bool disableConstantForceOnCollision;

	private bool inNoGravityMode;

	private void Awake()
	{
		if (GenieManager.AreAnyJobGenieModesActive() && GenieManager.DoesContainGenieMode(GlobalStorage.Instance.CurrentGenieModes, JobGenieCartridge.GenieModeTypes.NoGravityMode))
		{
			constantForceComponent.enabled = false;
			inNoGravityMode = true;
		}
	}

	private void OnEnable()
	{
		PickupableItem obj = pickupableItem;
		obj.OnGrabbed = (Action<GrabbableItem>)Delegate.Combine(obj.OnGrabbed, new Action<GrabbableItem>(Grabbed));
		PickupableItem obj2 = pickupableItem;
		obj2.OnReleased = (Action<GrabbableItem>)Delegate.Combine(obj2.OnReleased, new Action<GrabbableItem>(Released));
	}

	private void OnDisable()
	{
		PickupableItem obj = pickupableItem;
		obj.OnGrabbed = (Action<GrabbableItem>)Delegate.Remove(obj.OnGrabbed, new Action<GrabbableItem>(Grabbed));
		PickupableItem obj2 = pickupableItem;
		obj2.OnReleased = (Action<GrabbableItem>)Delegate.Remove(obj2.OnReleased, new Action<GrabbableItem>(Released));
	}

	private void Grabbed(GrabbableItem item)
	{
		constantForceComponent.enabled = false;
	}

	private void Released(GrabbableItem item)
	{
		if (!inNoGravityMode)
		{
			constantForceComponent.enabled = true;
		}
	}

	private void OnCollisionEnter(Collision c)
	{
		if (disableConstantForceOnCollision)
		{
			constantForceComponent.enabled = false;
		}
	}
}
