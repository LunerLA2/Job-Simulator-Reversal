using System;
using System.Collections.Generic;
using OwlchemyVR;
using UnityEngine;

public class ZeroGravitySuctionZone : MonoBehaviour
{
	[SerializeField]
	private RigidbodyEnterExitTriggerEvents suctionZone;

	[SerializeField]
	private Transform suctionToLocation;

	[SerializeField]
	private ParticleSystem suctionPFX;

	[SerializeField]
	private bool noSuction;

	[SerializeField]
	private bool suctionReversed;

	private List<PickupableItem> itemsInSuctionZone = new List<PickupableItem>();

	private List<PickupableItem> itemsInHandInSuctionZone = new List<PickupableItem>();

	private float pfxStartSpeed;

	private void Start()
	{
		if (suctionPFX != null)
		{
			pfxStartSpeed = suctionPFX.startSpeed;
		}
		SetPowerState(true);
	}

	private void Update()
	{
		for (int i = 0; i < itemsInSuctionZone.Count; i++)
		{
			if (itemsInSuctionZone[i] != null && itemsInSuctionZone[i].Rigidbody != null)
			{
				if (itemsInSuctionZone[i].IsCurrInHand)
				{
					itemsInHandInSuctionZone.Add(itemsInSuctionZone[i]);
					PickupableItem pickupableItem = itemsInSuctionZone[i];
					pickupableItem.OnReleased = (Action<GrabbableItem>)Delegate.Combine(pickupableItem.OnReleased, new Action<GrabbableItem>(PickupableReleasedInSuctionZone));
					itemsInSuctionZone.RemoveAt(i);
					i--;
				}
				else if (!noSuction)
				{
					Vector3 normalized = (suctionToLocation.position - itemsInSuctionZone[i].transform.position).normalized;
					if (suctionReversed)
					{
						normalized *= -1f;
					}
					itemsInSuctionZone[i].Rigidbody.velocity = Vector3.Lerp(itemsInSuctionZone[i].Rigidbody.velocity, normalized, Time.deltaTime * 5f);
				}
			}
			else
			{
				itemsInSuctionZone.RemoveAt(i);
				i--;
			}
		}
	}

	private void OnEnable()
	{
		RigidbodyEnterExitTriggerEvents rigidbodyEnterExitTriggerEvents = suctionZone;
		rigidbodyEnterExitTriggerEvents.OnRigidbodyEnterTrigger = (Action<Rigidbody>)Delegate.Combine(rigidbodyEnterExitTriggerEvents.OnRigidbodyEnterTrigger, new Action<Rigidbody>(ObjectEnteredSuctionZone));
		RigidbodyEnterExitTriggerEvents rigidbodyEnterExitTriggerEvents2 = suctionZone;
		rigidbodyEnterExitTriggerEvents2.OnRigidbodyExitTrigger = (Action<Rigidbody>)Delegate.Combine(rigidbodyEnterExitTriggerEvents2.OnRigidbodyExitTrigger, new Action<Rigidbody>(ObjectExitedSuctionZone));
		SetPowerState(true);
	}

	private void OnDisable()
	{
		RigidbodyEnterExitTriggerEvents rigidbodyEnterExitTriggerEvents = suctionZone;
		rigidbodyEnterExitTriggerEvents.OnRigidbodyEnterTrigger = (Action<Rigidbody>)Delegate.Remove(rigidbodyEnterExitTriggerEvents.OnRigidbodyEnterTrigger, new Action<Rigidbody>(ObjectEnteredSuctionZone));
		RigidbodyEnterExitTriggerEvents rigidbodyEnterExitTriggerEvents2 = suctionZone;
		rigidbodyEnterExitTriggerEvents2.OnRigidbodyExitTrigger = (Action<Rigidbody>)Delegate.Remove(rigidbodyEnterExitTriggerEvents2.OnRigidbodyExitTrigger, new Action<Rigidbody>(ObjectExitedSuctionZone));
		SetPowerState(false);
	}

	public void SetPowerState(bool isOn)
	{
		ParticleSystem.EmissionModule emission = suctionPFX.emission;
		emission.enabled = isOn;
		suctionZone.enabled = isOn;
		if (!isOn)
		{
			ClearLists();
		}
	}

	private void ClearLists()
	{
		while (itemsInSuctionZone.Count > 0)
		{
			itemsInSuctionZone[0].Rigidbody.useGravity = true;
			itemsInSuctionZone.RemoveAt(0);
		}
		while (itemsInHandInSuctionZone.Count > 0)
		{
			PickupableItem pickupableItem = itemsInSuctionZone[0];
			pickupableItem.OnReleased = (Action<GrabbableItem>)Delegate.Remove(pickupableItem.OnReleased, new Action<GrabbableItem>(PickupableReleasedInSuctionZone));
			itemsInSuctionZone.RemoveAt(0);
		}
	}

	public void ObjectEnteredSuctionZone(Rigidbody rb)
	{
		PickupableItem componentInParent = rb.GetComponentInParent<PickupableItem>();
		if (componentInParent != null)
		{
			if (componentInParent.IsCurrInHand && !itemsInHandInSuctionZone.Contains(componentInParent))
			{
				itemsInHandInSuctionZone.Add(componentInParent);
				componentInParent.OnReleased = (Action<GrabbableItem>)Delegate.Combine(componentInParent.OnReleased, new Action<GrabbableItem>(PickupableReleasedInSuctionZone));
			}
			else if (!componentInParent.IsCurrInHand && !itemsInSuctionZone.Contains(componentInParent))
			{
				itemsInSuctionZone.Add(componentInParent);
				rb.useGravity = false;
			}
		}
	}

	public void ObjectExitedSuctionZone(Rigidbody rb)
	{
		PickupableItem componentInParent = rb.GetComponentInParent<PickupableItem>();
		if (componentInParent != null)
		{
			if (itemsInHandInSuctionZone.Contains(componentInParent))
			{
				itemsInHandInSuctionZone.Remove(componentInParent);
				componentInParent.OnReleased = (Action<GrabbableItem>)Delegate.Remove(componentInParent.OnReleased, new Action<GrabbableItem>(PickupableReleasedInSuctionZone));
			}
			if (itemsInSuctionZone.Contains(componentInParent))
			{
				itemsInSuctionZone.Remove(componentInParent);
			}
			rb.useGravity = true;
		}
	}

	private void PickupableReleasedInSuctionZone(GrabbableItem grabbable)
	{
		Debug.Log("PickupableReleasedInSuctionZone");
		grabbable.Rigidbody.useGravity = false;
		PickupableItem componentInParent = grabbable.GetComponentInParent<PickupableItem>();
		if (componentInParent != null)
		{
			Debug.Log("item found");
			componentInParent.OnReleased = (Action<GrabbableItem>)Delegate.Remove(componentInParent.OnReleased, new Action<GrabbableItem>(PickupableReleasedInSuctionZone));
			itemsInHandInSuctionZone.Remove(componentInParent);
			itemsInSuctionZone.Add(componentInParent);
		}
	}

	public void SetSuctionReversed(bool isReversed)
	{
		suctionReversed = isReversed;
		if (isReversed)
		{
			suctionPFX.startSpeed = 0f - pfxStartSpeed;
		}
		else
		{
			suctionPFX.startSpeed = pfxStartSpeed;
		}
	}
}
