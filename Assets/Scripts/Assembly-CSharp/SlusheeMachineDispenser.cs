using System;
using OwlchemyVR;
using UnityEngine;

public class SlusheeMachineDispenser : MonoBehaviour
{
	[SerializeField]
	private GravityDispensingItem gravityDispensingItem;

	[SerializeField]
	private GrabbableItem item;

	private HapticInfoObject hapticInfoObj = new HapticInfoObject(90f);

	private bool isRunning;

	private void OnEnable()
	{
		GrabbableItem grabbableItem = item;
		grabbableItem.OnGrabbed = (Action<GrabbableItem>)Delegate.Combine(grabbableItem.OnGrabbed, new Action<GrabbableItem>(ItemGrabbed));
		GrabbableItem grabbableItem2 = item;
		grabbableItem2.OnReleased = (Action<GrabbableItem>)Delegate.Combine(grabbableItem2.OnReleased, new Action<GrabbableItem>(ItemReleased));
	}

	private void OnDisable()
	{
		GrabbableItem grabbableItem = item;
		grabbableItem.OnGrabbed = (Action<GrabbableItem>)Delegate.Remove(grabbableItem.OnGrabbed, new Action<GrabbableItem>(ItemGrabbed));
		GrabbableItem grabbableItem2 = item;
		grabbableItem2.OnReleased = (Action<GrabbableItem>)Delegate.Remove(grabbableItem2.OnReleased, new Action<GrabbableItem>(ItemReleased));
	}

	private void ItemGrabbed(GrabbableItem grabbableItem)
	{
		if (isRunning)
		{
			item.CurrInteractableHand.HapticsController.AddNewHaptic(hapticInfoObj);
		}
	}

	private void ItemReleased(GrabbableItem grabbableItem)
	{
		if (isRunning)
		{
			item.CurrInteractableHand.HapticsController.RemoveHaptic(hapticInfoObj);
		}
	}

	public void TurnOn()
	{
		if (!isRunning)
		{
			gravityDispensingItem.enabled = true;
			isRunning = true;
			if (item.IsCurrInHand)
			{
				item.CurrInteractableHand.HapticsController.AddNewHaptic(hapticInfoObj);
			}
		}
	}

	public void TurnOff()
	{
		if (isRunning)
		{
			gravityDispensingItem.enabled = false;
			isRunning = false;
			if (item.IsCurrInHand)
			{
				item.CurrInteractableHand.HapticsController.RemoveHaptic(hapticInfoObj);
			}
		}
	}
}
