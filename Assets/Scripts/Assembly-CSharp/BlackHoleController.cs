using System;
using System.Collections;
using System.Collections.Generic;
using OwlchemyVR;
using UnityEngine;

public class BlackHoleController : MonoBehaviour
{
	[SerializeField]
	private WorldItem myWorldItem;

	[SerializeField]
	private RigidbodyEnterExitTriggerEvents destroyRegionEvents;

	[SerializeField]
	private RigidbodyEnterExitTriggerEvents gravityRegion;

	[SerializeField]
	private GameObject center;

	[SerializeField]
	private AudioSourceHelper destroyAudioSource;

	[SerializeField]
	private AudioClip destroySound;

	[SerializeField]
	private ParticleSystem destroyParticle;

	private List<PickupableItem> itemsInGravity = new List<PickupableItem>();

	private List<PickupableItem> itemsInDestroyRegion = new List<PickupableItem>();

	private List<PickupableItem> itemInHandInGravityZone = new List<PickupableItem>();

	private float destroyDuration = 0.3f;

	private void Start()
	{
	}

	private void Update()
	{
		UpdateGravityRegion();
	}

	private void UpdateGravityRegion()
	{
		for (int i = 0; i < itemInHandInGravityZone.Count; i++)
		{
			if (!itemInHandInGravityZone[i].IsCurrInHand)
			{
				if (itemsInGravity.Contains(itemInHandInGravityZone[i]))
				{
					itemInHandInGravityZone[i].Rigidbody.useGravity = false;
				}
				else
				{
					itemInHandInGravityZone[i].Rigidbody.useGravity = true;
				}
				itemInHandInGravityZone.RemoveAt(i);
			}
		}
		for (int j = 0; j < itemsInGravity.Count; j++)
		{
			if (!itemsInGravity[j].IsCurrInHand)
			{
				itemsInGravity[j].Rigidbody.velocity = Vector3.Lerp(itemsInGravity[j].Rigidbody.velocity, (base.transform.position - itemsInGravity[j].transform.position).normalized * 0.1f, Time.deltaTime * 5f);
			}
		}
	}

	private void UpdateSuckRegion()
	{
	}

	private void OnEnable()
	{
		RigidbodyEnterExitTriggerEvents rigidbodyEnterExitTriggerEvents = destroyRegionEvents;
		rigidbodyEnterExitTriggerEvents.OnRigidbodyEnterTrigger = (Action<Rigidbody>)Delegate.Combine(rigidbodyEnterExitTriggerEvents.OnRigidbodyEnterTrigger, new Action<Rigidbody>(ObjectEnteredDestroyRegion));
		RigidbodyEnterExitTriggerEvents rigidbodyEnterExitTriggerEvents2 = destroyRegionEvents;
		rigidbodyEnterExitTriggerEvents2.OnRigidbodyExitTrigger = (Action<Rigidbody>)Delegate.Combine(rigidbodyEnterExitTriggerEvents2.OnRigidbodyExitTrigger, new Action<Rigidbody>(ObjectExitedDestroyRegion));
		RigidbodyEnterExitTriggerEvents rigidbodyEnterExitTriggerEvents3 = gravityRegion;
		rigidbodyEnterExitTriggerEvents3.OnRigidbodyEnterTrigger = (Action<Rigidbody>)Delegate.Combine(rigidbodyEnterExitTriggerEvents3.OnRigidbodyEnterTrigger, new Action<Rigidbody>(ObjectEnteredGravityRegion));
		RigidbodyEnterExitTriggerEvents rigidbodyEnterExitTriggerEvents4 = gravityRegion;
		rigidbodyEnterExitTriggerEvents4.OnRigidbodyExitTrigger = (Action<Rigidbody>)Delegate.Combine(rigidbodyEnterExitTriggerEvents4.OnRigidbodyExitTrigger, new Action<Rigidbody>(ObjectExitedGravityRegion));
	}

	private void OnDisable()
	{
		RigidbodyEnterExitTriggerEvents rigidbodyEnterExitTriggerEvents = destroyRegionEvents;
		rigidbodyEnterExitTriggerEvents.OnRigidbodyEnterTrigger = (Action<Rigidbody>)Delegate.Remove(rigidbodyEnterExitTriggerEvents.OnRigidbodyEnterTrigger, new Action<Rigidbody>(ObjectEnteredDestroyRegion));
		RigidbodyEnterExitTriggerEvents rigidbodyEnterExitTriggerEvents2 = destroyRegionEvents;
		rigidbodyEnterExitTriggerEvents2.OnRigidbodyExitTrigger = (Action<Rigidbody>)Delegate.Remove(rigidbodyEnterExitTriggerEvents2.OnRigidbodyExitTrigger, new Action<Rigidbody>(ObjectExitedDestroyRegion));
		RigidbodyEnterExitTriggerEvents rigidbodyEnterExitTriggerEvents3 = gravityRegion;
		rigidbodyEnterExitTriggerEvents3.OnRigidbodyEnterTrigger = (Action<Rigidbody>)Delegate.Remove(rigidbodyEnterExitTriggerEvents3.OnRigidbodyEnterTrigger, new Action<Rigidbody>(ObjectEnteredGravityRegion));
		RigidbodyEnterExitTriggerEvents rigidbodyEnterExitTriggerEvents4 = gravityRegion;
		rigidbodyEnterExitTriggerEvents4.OnRigidbodyExitTrigger = (Action<Rigidbody>)Delegate.Remove(rigidbodyEnterExitTriggerEvents4.OnRigidbodyExitTrigger, new Action<Rigidbody>(ObjectExitedGravityRegion));
	}

	public void ObjectEnteredDestroyRegion(Rigidbody rb)
	{
		PickupableItem componentInParent = rb.GetComponentInParent<PickupableItem>();
		if (componentInParent != null && !itemsInDestroyRegion.Contains(componentInParent))
		{
			if (itemsInGravity.Contains(componentInParent))
			{
				itemsInGravity.Remove(componentInParent);
			}
			StartCoroutine(DestroyObjectAsync(componentInParent));
		}
	}

	private IEnumerator DestroyObjectAsync(PickupableItem pickupable)
	{
		itemsInDestroyRegion.Add(pickupable);
		if (pickupable.IsCurrInHand)
		{
			pickupable.CurrInteractableHand.TryRelease();
		}
		pickupable.enabled = false;
		Rigidbody[] componentsInChildren = pickupable.GetComponentsInChildren<Rigidbody>();
		foreach (Rigidbody rb in componentsInChildren)
		{
			rb.isKinematic = true;
		}
		pickupable.transform.SetParent(GlobalStorage.Instance.ContentRoot);
		Transform tr = pickupable.transform;
		if (destroySound != null)
		{
			AudioManager.Instance.Play(destroyAudioSource.transform, destroySound, 1f, 1f);
		}
		Vector3 startPos = tr.position;
		Vector3 center = base.transform.position;
		float destroyTime = 0f;
		while (destroyTime < destroyDuration)
		{
			float progress = destroyTime / destroyDuration;
			tr.position = Vector3.Lerp(startPos, center, progress);
			tr.localScale = Vector3.one * Mathf.Lerp(1f, 0.01f, progress);
			destroyTime += Time.deltaTime;
			yield return null;
		}
		itemsInDestroyRegion.Remove(pickupable);
		GameEventsManager.Instance.ItemActionOccurred(myWorldItem.Data, "USED");
		WorldItem worldItem = pickupable.GetComponent<WorldItem>();
		if (worldItem != null)
		{
			GameEventsManager.Instance.ItemActionOccurred(worldItem.Data, "DESTROYED");
		}
		UnityEngine.Object.Destroy(pickupable.gameObject);
		if (destroyParticle != null)
		{
			destroyParticle.Play();
		}
	}

	public void ObjectExitedDestroyRegion(Rigidbody rb)
	{
	}

	public void ObjectEnteredGravityRegion(Rigidbody rb)
	{
		PickupableItem componentInParent = rb.GetComponentInParent<PickupableItem>();
		if (componentInParent != null && !itemsInGravity.Contains(componentInParent))
		{
			itemsInGravity.Add(componentInParent);
			componentInParent.transform.SetParent(center.transform);
			if (!componentInParent.IsCurrInHand)
			{
				rb.useGravity = false;
			}
			else
			{
				itemInHandInGravityZone.Add(componentInParent);
			}
		}
	}

	public void ObjectExitedGravityRegion(Rigidbody rb)
	{
		PickupableItem componentInParent = rb.GetComponentInParent<PickupableItem>();
		if (componentInParent != null && itemsInGravity.Contains(componentInParent))
		{
			componentInParent.transform.SetParent(GlobalStorage.Instance.ContentRoot);
			if (!componentInParent.IsCurrInHand)
			{
				rb.useGravity = true;
			}
			else
			{
				itemInHandInGravityZone.Add(componentInParent);
			}
			itemsInGravity.Remove(componentInParent);
		}
	}
}
