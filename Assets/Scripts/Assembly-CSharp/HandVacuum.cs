using System;
using System.Collections;
using System.Collections.Generic;
using OwlchemyVR;
using UnityEngine;

public class HandVacuum : MonoBehaviour
{
	private class StashedItemInfo
	{
		public PickupableItem Item;

		public Transform CachedParent;

		public bool WasKinematic;
	}

	[SerializeField]
	private WorldItem myWorldItem;

	[SerializeField]
	private PickupableItem pickupable;

	[SerializeField]
	private int maxStashedItems = 5;

	[SerializeField]
	private RigidbodyEnterExitTriggerEvents destroyRegionEvents;

	[SerializeField]
	private RigidbodyEnterExitTriggerEvents vacuumRegion;

	[SerializeField]
	private Transform vacuumTo;

	[SerializeField]
	private ParticleSystem vacuumPFX;

	[SerializeField]
	private AudioSourceHelper destroyAudioSource;

	[SerializeField]
	private AudioClip destroySound;

	[SerializeField]
	private ParticleSystem destroyParticle;

	[SerializeField]
	private GrabbableItem nozzleGrabbable;

	[SerializeField]
	private ForceLockJoint nozzleForceLockJoint;

	[SerializeField]
	private WorldItemData[] doNotDestroyItems;

	private List<PickupableItem> itemsInVacuumZone = new List<PickupableItem>();

	private List<PickupableItem> itemsInDestroyRegion = new List<PickupableItem>();

	private List<PickupableItem> itemsInHandInVacuumZone = new List<PickupableItem>();

	private float destroyDuration = 0.1f;

	private bool isActive;

	private float prevNozzleAngle;

	private HandVacuumState state;

	private float pfxStartSpeed;

	private LinkedList<StashedItemInfo> stashedItems;

	public HandVacuumState State
	{
		get
		{
			return state;
		}
	}

	private void Start()
	{
		stashedItems = new LinkedList<StashedItemInfo>();
		pfxStartSpeed = vacuumPFX.startSpeed;
		SetPowerState(false);
	}

	private void Update()
	{
		if (state == HandVacuumState.Suck || state == HandVacuumState.Blow)
		{
			UpdateActiveState();
		}
	}

	private void UpdateActiveState()
	{
		for (int i = 0; i < itemsInHandInVacuumZone.Count; i++)
		{
			if (itemsInHandInVacuumZone[i].IsCurrInHand)
			{
				continue;
			}
			if (itemsInHandInVacuumZone[i].Rigidbody != null)
			{
				if (itemsInVacuumZone.Contains(itemsInHandInVacuumZone[i]))
				{
					itemsInHandInVacuumZone[i].Rigidbody.useGravity = false;
				}
				else
				{
					itemsInHandInVacuumZone[i].Rigidbody.useGravity = true;
				}
			}
			itemsInHandInVacuumZone.RemoveAt(i);
			i--;
		}
		for (int j = 0; j < itemsInVacuumZone.Count; j++)
		{
			if (!itemsInVacuumZone[j].IsCurrInHand)
			{
				if (itemsInVacuumZone[j].Rigidbody != null)
				{
					Vector3 normalized = (vacuumTo.position - itemsInVacuumZone[j].transform.position).normalized;
					if (state == HandVacuumState.Blow)
					{
						normalized *= -1f;
					}
					itemsInVacuumZone[j].Rigidbody.velocity = Vector3.Lerp(itemsInVacuumZone[j].Rigidbody.velocity, normalized, Time.deltaTime * 5f);
				}
				else
				{
					itemsInVacuumZone.RemoveAt(j);
					j--;
				}
			}
			else if (!itemsInHandInVacuumZone.Contains(itemsInVacuumZone[j]))
			{
				itemsInHandInVacuumZone.Add(itemsInVacuumZone[j]);
			}
		}
	}

	private void OnEnable()
	{
		RigidbodyEnterExitTriggerEvents rigidbodyEnterExitTriggerEvents = destroyRegionEvents;
		rigidbodyEnterExitTriggerEvents.OnRigidbodyEnterTrigger = (Action<Rigidbody>)Delegate.Combine(rigidbodyEnterExitTriggerEvents.OnRigidbodyEnterTrigger, new Action<Rigidbody>(ObjectEnteredDestroyRegion));
		RigidbodyEnterExitTriggerEvents rigidbodyEnterExitTriggerEvents2 = destroyRegionEvents;
		rigidbodyEnterExitTriggerEvents2.OnRigidbodyExitTrigger = (Action<Rigidbody>)Delegate.Combine(rigidbodyEnterExitTriggerEvents2.OnRigidbodyExitTrigger, new Action<Rigidbody>(ObjectExitedDestroyRegion));
		RigidbodyEnterExitTriggerEvents rigidbodyEnterExitTriggerEvents3 = vacuumRegion;
		rigidbodyEnterExitTriggerEvents3.OnRigidbodyEnterTrigger = (Action<Rigidbody>)Delegate.Combine(rigidbodyEnterExitTriggerEvents3.OnRigidbodyEnterTrigger, new Action<Rigidbody>(ObjectEnteredGravityRegion));
		RigidbodyEnterExitTriggerEvents rigidbodyEnterExitTriggerEvents4 = vacuumRegion;
		rigidbodyEnterExitTriggerEvents4.OnRigidbodyExitTrigger = (Action<Rigidbody>)Delegate.Combine(rigidbodyEnterExitTriggerEvents4.OnRigidbodyExitTrigger, new Action<Rigidbody>(ObjectExitedGravityRegion));
		PickupableItem pickupableItem = pickupable;
		pickupableItem.OnGrabbed = (Action<GrabbableItem>)Delegate.Combine(pickupableItem.OnGrabbed, new Action<GrabbableItem>(OnVacuumGrabbed));
		PickupableItem pickupableItem2 = pickupable;
		pickupableItem2.OnReleased = (Action<GrabbableItem>)Delegate.Combine(pickupableItem2.OnReleased, new Action<GrabbableItem>(OnVacuumReleased));
		GrabbableItem grabbableItem = nozzleGrabbable;
		grabbableItem.OnGrabbed = (Action<GrabbableItem>)Delegate.Combine(grabbableItem.OnGrabbed, new Action<GrabbableItem>(NozzleGrabbed));
		GrabbableItem grabbableItem2 = nozzleGrabbable;
		grabbableItem2.OnReleased = (Action<GrabbableItem>)Delegate.Combine(grabbableItem2.OnReleased, new Action<GrabbableItem>(NozzleReleased));
	}

	private void OnDisable()
	{
		RigidbodyEnterExitTriggerEvents rigidbodyEnterExitTriggerEvents = destroyRegionEvents;
		rigidbodyEnterExitTriggerEvents.OnRigidbodyEnterTrigger = (Action<Rigidbody>)Delegate.Remove(rigidbodyEnterExitTriggerEvents.OnRigidbodyEnterTrigger, new Action<Rigidbody>(ObjectEnteredDestroyRegion));
		RigidbodyEnterExitTriggerEvents rigidbodyEnterExitTriggerEvents2 = destroyRegionEvents;
		rigidbodyEnterExitTriggerEvents2.OnRigidbodyExitTrigger = (Action<Rigidbody>)Delegate.Remove(rigidbodyEnterExitTriggerEvents2.OnRigidbodyExitTrigger, new Action<Rigidbody>(ObjectExitedDestroyRegion));
		RigidbodyEnterExitTriggerEvents rigidbodyEnterExitTriggerEvents3 = vacuumRegion;
		rigidbodyEnterExitTriggerEvents3.OnRigidbodyEnterTrigger = (Action<Rigidbody>)Delegate.Remove(rigidbodyEnterExitTriggerEvents3.OnRigidbodyEnterTrigger, new Action<Rigidbody>(ObjectEnteredGravityRegion));
		RigidbodyEnterExitTriggerEvents rigidbodyEnterExitTriggerEvents4 = vacuumRegion;
		rigidbodyEnterExitTriggerEvents4.OnRigidbodyExitTrigger = (Action<Rigidbody>)Delegate.Remove(rigidbodyEnterExitTriggerEvents4.OnRigidbodyExitTrigger, new Action<Rigidbody>(ObjectExitedGravityRegion));
		PickupableItem pickupableItem = pickupable;
		pickupableItem.OnGrabbed = (Action<GrabbableItem>)Delegate.Remove(pickupableItem.OnGrabbed, new Action<GrabbableItem>(OnVacuumGrabbed));
		PickupableItem pickupableItem2 = pickupable;
		pickupableItem2.OnReleased = (Action<GrabbableItem>)Delegate.Remove(pickupableItem2.OnReleased, new Action<GrabbableItem>(OnVacuumReleased));
		GrabbableItem grabbableItem = nozzleGrabbable;
		grabbableItem.OnGrabbed = (Action<GrabbableItem>)Delegate.Remove(grabbableItem.OnGrabbed, new Action<GrabbableItem>(NozzleGrabbed));
		GrabbableItem grabbableItem2 = nozzleGrabbable;
		grabbableItem2.OnReleased = (Action<GrabbableItem>)Delegate.Remove(grabbableItem2.OnReleased, new Action<GrabbableItem>(NozzleReleased));
	}

	private void OnVacuumGrabbed(GrabbableItem grabbable)
	{
	}

	private void OnVacuumReleased(GrabbableItem grabbable)
	{
		SetPowerState(false);
	}

	public void TogglePower()
	{
		SetPowerState(!isActive);
	}

	private void SetPowerState(bool isOn)
	{
		isActive = isOn;
		ParticleSystem.EmissionModule emission = vacuumPFX.emission;
		emission.enabled = isOn;
		vacuumRegion.enabled = isOn;
		destroyRegionEvents.enabled = isOn;
		SetVacuumStateByNozzleAngle();
		if (state == HandVacuumState.Off)
		{
			ClearLists();
		}
	}

	private void SetVacuumStateByNozzleAngle()
	{
		float num = Mathf.Abs(nozzleGrabbable.transform.localEulerAngles.z);
		if (!isActive)
		{
			state = HandVacuumState.Off;
		}
		else if (Mathf.Abs(180f - num) <= 10f)
		{
			state = HandVacuumState.Blow;
			StartCoroutine(ExpellItemsAsync());
			vacuumPFX.startSpeed = 0f - pfxStartSpeed;
		}
		else
		{
			state = HandVacuumState.Suck;
			vacuumPFX.startSpeed = pfxStartSpeed;
		}
	}

	private void ClearLists()
	{
		while (itemsInVacuumZone.Count > 0)
		{
			itemsInVacuumZone[0].Rigidbody.useGravity = true;
			itemsInVacuumZone.RemoveAt(0);
		}
		itemsInHandInVacuumZone.Clear();
	}

	public void ObjectEnteredDestroyRegion(Rigidbody rb)
	{
		if (state != 0)
		{
			return;
		}
		PickupableItem componentInParent = rb.GetComponentInParent<PickupableItem>();
		WorldItem componentInParent2 = rb.GetComponentInParent<WorldItem>();
		TrinketSpringController component = rb.GetComponent<TrinketSpringController>();
		if (component != null)
		{
			return;
		}
		if (componentInParent2 != null)
		{
			for (int i = 0; i < doNotDestroyItems.Length; i++)
			{
				if (componentInParent2.Data == doNotDestroyItems[i])
				{
					return;
				}
			}
		}
		if (!(componentInParent != null) || itemsInDestroyRegion.Contains(componentInParent))
		{
			return;
		}
		AttachableObject component2 = componentInParent.GetComponent<AttachableObject>();
		if (!(component2 != null) || !(component2.CurrentlyAttachedTo != null))
		{
			if (itemsInVacuumZone.Contains(componentInParent))
			{
				itemsInVacuumZone.Remove(componentInParent);
			}
			StartCoroutine(DestroyObjectAsync(componentInParent));
		}
	}

	private IEnumerator DestroyObjectAsync(PickupableItem pickupable)
	{
		itemsInVacuumZone.Remove(pickupable);
		itemsInDestroyRegion.Add(pickupable);
		if (pickupable.IsCurrInHand)
		{
			pickupable.CurrInteractableHand.TryRelease();
		}
		pickupable.enabled = false;
		bool itemWasKinematic = true;
		Rigidbody[] componentsInChildren = pickupable.GetComponentsInChildren<Rigidbody>();
		foreach (Rigidbody rb in componentsInChildren)
		{
			itemWasKinematic = rb.isKinematic;
			rb.isKinematic = true;
		}
		WorldItem worldItem = pickupable.GetComponent<WorldItem>();
		Transform tr = pickupable.transform;
		if (destroySound != null)
		{
			AudioManager.Instance.Play(destroyAudioSource.transform, destroySound, 1f, 1f);
		}
		Vector3 startPos = tr.position;
		Vector3 center = vacuumTo.position;
		float destroyTime = 0f;
		while (destroyTime < destroyDuration && pickupable != null)
		{
			float progress = destroyTime / destroyDuration;
			tr.position = Vector3.Lerp(startPos, center, progress);
			tr.localScale = Vector3.one * Mathf.Lerp(1f, 0.01f, progress);
			destroyTime += Time.deltaTime;
			yield return null;
		}
		if (pickupable != null)
		{
			itemsInDestroyRegion.Remove(pickupable);
			GameEventsManager.Instance.ItemActionOccurred(myWorldItem.Data, "USED");
			if (worldItem != null)
			{
				GameEventsManager.Instance.ItemAppliedToItemActionOccurred(worldItem.Data, myWorldItem.Data, "ADDED_TO");
				GameEventsManager.Instance.ItemActionOccurred(worldItem.Data, "DESTROYED");
			}
			StashedItemInfo itemInfo = new StashedItemInfo
			{
				Item = pickupable,
				CachedParent = pickupable.transform.parent
			};
			pickupable.gameObject.SetActive(false);
			if (pickupable.Rigidbody != null)
			{
				itemInfo.WasKinematic = itemWasKinematic;
				pickupable.Rigidbody.isKinematic = true;
			}
			stashedItems.AddFirst(itemInfo);
			if (stashedItems.Count > maxStashedItems)
			{
				UnityEngine.Object.Destroy(stashedItems.Last.Value.Item.gameObject);
				stashedItems.RemoveLast();
			}
			if (destroyParticle != null)
			{
				destroyParticle.Play();
			}
		}
	}

	public void ObjectExitedDestroyRegion(Rigidbody rb)
	{
	}

	public void ObjectEnteredGravityRegion(Rigidbody rb)
	{
		if (!isActive)
		{
			return;
		}
		PickupableItem componentInParent = rb.GetComponentInParent<PickupableItem>();
		if (componentInParent != null && !itemsInVacuumZone.Contains(componentInParent))
		{
			itemsInVacuumZone.Add(componentInParent);
			if (!componentInParent.IsCurrInHand)
			{
				rb.useGravity = false;
			}
			else
			{
				itemsInHandInVacuumZone.Add(componentInParent);
			}
		}
	}

	public void ObjectExitedGravityRegion(Rigidbody rb)
	{
		if (!isActive)
		{
			return;
		}
		PickupableItem componentInParent = rb.GetComponentInParent<PickupableItem>();
		if (componentInParent != null && itemsInVacuumZone.Contains(componentInParent))
		{
			if (!componentInParent.IsCurrInHand)
			{
				rb.useGravity = true;
			}
			else
			{
				itemsInHandInVacuumZone.Add(componentInParent);
			}
			itemsInVacuumZone.Remove(componentInParent);
		}
	}

	private IEnumerator ExpellItemsAsync()
	{
		while (state == HandVacuumState.Blow && stashedItems.Count > 0)
		{
			StashedItemInfo info = stashedItems.First.Value;
			PickupableItem item = info.Item;
			item.gameObject.SetActive(true);
			item.enabled = true;
			item.transform.position = vacuumTo.position;
			item.Rigidbody.isKinematic = info.WasKinematic;
			item.transform.localScale = Vector3.one;
			stashedItems.RemoveFirst();
			yield return new WaitForSeconds(1f);
		}
	}

	private float GetNozzleAngle()
	{
		float num;
		for (num = nozzleGrabbable.transform.localEulerAngles.z; num <= -360f; num += 360f)
		{
		}
		while (num > 0f)
		{
			num -= 360f;
		}
		return num;
	}

	private void SetNozzleAngle(float angle)
	{
		nozzleGrabbable.transform.localEulerAngles = new Vector3(0f, 0f, angle);
		nozzleForceLockJoint.ResetRotationMemory();
		SetVacuumStateByNozzleAngle();
	}

	private float SnapNozzleToNearest180()
	{
		float nozzleAngle = GetNozzleAngle();
		nozzleAngle = Mathf.Round(nozzleAngle / 180f) * 180f;
		if (nozzleAngle != prevNozzleAngle)
		{
			prevNozzleAngle = nozzleAngle;
			Click();
		}
		SetNozzleAngle(nozzleAngle);
		return nozzleAngle;
	}

	private void Click()
	{
	}

	private void NozzleGrabbed(GrabbableItem grabbable)
	{
		nozzleForceLockJoint.lockZRot = false;
	}

	private void NozzleReleased(GrabbableItem grabbable)
	{
		nozzleForceLockJoint.lockZRot = true;
		SnapNozzleToNearest180();
	}
}
