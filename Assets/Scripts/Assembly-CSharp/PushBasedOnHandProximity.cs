using System;
using OwlchemyVR;
using OwlchemyVR2;
using UnityEngine;

public class PushBasedOnHandProximity : MonoBehaviour
{
	[SerializeField]
	private OwlchemyVR2.GrabbableSlider sliderToPush;

	[SerializeField]
	private float pushSpeed = 1f;

	[SerializeField]
	private PlayerPartDetector handPushZone;

	[SerializeField]
	private ItemCollectionZone heldItemPushZone;

	private float handVel;

	private float newHandVel;

	private float decayTime = 0.03f;

	private void OnEnable()
	{
		GrabbableItem grabbable = sliderToPush.Grabbable;
		grabbable.OnGrabbed = (Action<GrabbableItem>)Delegate.Combine(grabbable.OnGrabbed, new Action<GrabbableItem>(Grabbed));
	}

	private void OnDisable()
	{
		GrabbableItem grabbable = sliderToPush.Grabbable;
		grabbable.OnGrabbed = (Action<GrabbableItem>)Delegate.Remove(grabbable.OnGrabbed, new Action<GrabbableItem>(Grabbed));
	}

	private void Grabbed(GrabbableItem obj)
	{
		handVel = 0f;
	}

	private void Update()
	{
		if (!sliderToPush.Grabbable.IsCurrInHand)
		{
			newHandVel = 0f;
			if (handPushZone.IsAnythingInside && handPushZone.DetectedHands.Count > 0)
			{
				for (int i = 0; i < handPushZone.DetectedHands.Count; i++)
				{
					newHandVel += handPushZone.DetectedHands[i].GetRawHandVelocity().magnitude;
				}
				newHandVel += handVel / (float)handPushZone.DetectedHands.Count;
			}
			if (heldItemPushZone.IsAnyHeldItemInside)
			{
				float num = 0f;
				for (int j = 0; j < heldItemPushZone.InCollectionZoneButCurrentlyInHand.Count; j++)
				{
					num += heldItemPushZone.InCollectionZoneButCurrentlyInHand[j].CurrInteractableHand.GrabbedItemCurrVelocity.magnitude;
				}
				newHandVel += num / (float)heldItemPushZone.InCollectionZoneButCurrentlyInHand.Count;
			}
			if (newHandVel > handVel)
			{
				handVel = newHandVel;
				sliderToPush.AddAxisForce(pushSpeed * Time.deltaTime, handVel);
				return;
			}
			handVel -= handVel * (Time.deltaTime / decayTime);
			if (handVel < newHandVel)
			{
				handVel = newHandVel;
			}
			if (handVel < 0f)
			{
				handVel = 0f;
			}
			else
			{
				sliderToPush.AddAxisForce(pushSpeed * Time.deltaTime, handVel);
			}
		}
		else
		{
			handVel = 0f;
		}
	}
}
