using System;
using System.Collections;
using System.Collections.Generic;
using OwlchemyVR;
using UnityEngine;

public class RespawningFoodContainer : MonoBehaviour
{
	[SerializeField]
	private bool doRespawn = true;

	[SerializeField]
	private GrabbableHinge[] doorHinges;

	[SerializeField]
	private ItemCollectionZone itemCollectionZone;

	[SerializeField]
	private GameObject spawnerContainer;

	[SerializeField]
	private WorldItem doorWorldItem;

	[SerializeField]
	private bool doOpenCloseGameEvents;

	[SerializeField]
	private bool spawnOnClose = true;

	[SerializeField]
	private bool spawnOnOpen;

	[SerializeField]
	private AudioClip doorClosedAmbience;

	[SerializeField]
	private AudioClip doorOpenAmbience;

	[SerializeField]
	private AudioClip doorOpeningAudio;

	[SerializeField]
	private AudioClip doorClosingAudio;

	[SerializeField]
	private AudioSourceHelper ambientAudioSource;

	[SerializeField]
	private AudioSourceHelper doorAudioSource;

	private ItemRespawner[] itemRespawners;

	private int doorCount;

	[SerializeField]
	private bool doUnparenting;

	private bool isUnparented;

	private List<PickupableItem> unparentedItems;

	private List<Vector3> unparentedOriginalPositions;

	private List<Transform> unparentedOriginalParents;

	private void Awake()
	{
		itemRespawners = spawnerContainer.GetComponentsInChildren<ItemRespawner>();
	}

	private void Start()
	{
		StartCoroutine(WaitAndSetLockedState(true, 0.01f));
	}

	private IEnumerator WaitAndSetLockedState(bool locked, float delay)
	{
		yield return new WaitForSeconds(delay);
		SetLockedStateOfItemsWithinContainer(locked);
	}

	private void OnEnable()
	{
		for (int i = 0; i < doorHinges.Length; i++)
		{
			doorHinges[i].OnLowerUnlocked += DoorOpened;
			doorHinges[i].OnLowerLocked += DoorClosed;
		}
		ItemCollectionZone obj = itemCollectionZone;
		obj.OnItemsInCollectionAdded = (Action<ItemCollectionZone, PickupableItem>)Delegate.Combine(obj.OnItemsInCollectionAdded, new Action<ItemCollectionZone, PickupableItem>(ItemAddedToCollectionZone));
		ItemCollectionZone obj2 = itemCollectionZone;
		obj2.OnItemsInCollectionRemoved = (Action<ItemCollectionZone, PickupableItem>)Delegate.Combine(obj2.OnItemsInCollectionRemoved, new Action<ItemCollectionZone, PickupableItem>(ItemRemovedFromCollectionZone));
	}

	private void OnDisable()
	{
		for (int i = 0; i < doorHinges.Length; i++)
		{
			doorHinges[i].OnLowerUnlocked -= DoorOpened;
			doorHinges[i].OnLowerLocked -= DoorClosed;
		}
		ItemCollectionZone obj = itemCollectionZone;
		obj.OnItemsInCollectionAdded = (Action<ItemCollectionZone, PickupableItem>)Delegate.Remove(obj.OnItemsInCollectionAdded, new Action<ItemCollectionZone, PickupableItem>(ItemAddedToCollectionZone));
		ItemCollectionZone obj2 = itemCollectionZone;
		obj2.OnItemsInCollectionRemoved = (Action<ItemCollectionZone, PickupableItem>)Delegate.Remove(obj2.OnItemsInCollectionRemoved, new Action<ItemCollectionZone, PickupableItem>(ItemRemovedFromCollectionZone));
	}

	private void ItemAddedToCollectionZone(ItemCollectionZone zone, PickupableItem pickupable)
	{
		if (!pickupable.transform.IsChildOf(spawnerContainer.transform))
		{
			pickupable.transform.SetParent(spawnerContainer.transform, true);
		}
	}

	private void ItemRemovedFromCollectionZone(ItemCollectionZone zone, PickupableItem pickupable)
	{
		if (!pickupable.transform.IsChildOf(spawnerContainer.transform))
		{
			return;
		}
		if (doRespawn)
		{
			for (int i = 0; i < itemRespawners.Length; i++)
			{
				if (itemRespawners[i].LastItemSpawned == pickupable)
				{
					itemRespawners[i].LastItemRemovedFromContainer();
				}
			}
		}
		pickupable.transform.SetParent(GlobalStorage.Instance.ContentRoot, true);
	}

	public void AllowOpening()
	{
		for (int i = 0; i < doorHinges.Length; i++)
		{
			doorHinges[i].Grabbable.enabled = true;
		}
		RefreshSpawnsAndLockItemsIfAllDoorsClosed();
	}

	public void ForceAllDoorsClosed()
	{
		for (int i = 0; i < doorHinges.Length; i++)
		{
			StartCoroutine(ForceDoorClosedAsync(doorHinges[i]));
		}
	}

	private IEnumerator ForceDoorClosedAsync(GrabbableHinge hinge)
	{
		if (hinge.Grabbable.IsCurrInHand)
		{
			hinge.Grabbable.CurrInteractableHand.ManuallyReleaseJoint();
		}
		hinge.UnlockUpper();
		hinge.Grabbable.enabled = false;
		bool previousKin = hinge.Grabbable.Rigidbody.isKinematic;
		hinge.Grabbable.Rigidbody.isKinematic = true;
		Go.to(hinge.transform, 0.2f, new GoTweenConfig().localRotation(Quaternion.identity).setEaseType(GoEaseType.QuadInOut));
		yield return new WaitForSeconds(0.2f);
		hinge.Grabbable.Rigidbody.isKinematic = previousKin;
		hinge.LockLower();
	}

	private void DoorOpened(GrabbableHinge hinge)
	{
		doorCount++;
		if (doorCount > doorHinges.Length)
		{
			doorCount = doorHinges.Length;
		}
		if (doorOpenAmbience != null && ambientAudioSource.GetClip() != doorOpenAmbience)
		{
			ambientAudioSource.SetClip(doorOpenAmbience);
			ambientAudioSource.Play();
		}
		if (doorOpeningAudio != null)
		{
			doorAudioSource.SetClip(doorOpeningAudio);
			doorAudioSource.Play();
		}
		if (spawnOnOpen && doRespawn)
		{
			RespawnAll();
		}
		SetLockedStateOfItemsWithinContainer(false);
		if (doOpenCloseGameEvents)
		{
			GameEventsManager.Instance.ItemActionOccurred(doorWorldItem.Data, "OPENED");
		}
	}

	private void DoorClosed(GrabbableHinge hinge, bool isInitial)
	{
		doorCount--;
		if (doorCount < 0)
		{
			doorCount = 0;
		}
		if (doorClosedAmbience != null && doorCount <= 0)
		{
			ambientAudioSource.SetClip(doorClosedAmbience);
			ambientAudioSource.Play();
		}
		if (doorClosingAudio != null)
		{
			doorAudioSource.SetClip(doorClosingAudio);
			doorAudioSource.Play();
		}
		if (spawnOnClose)
		{
			RefreshSpawnsAndLockItemsIfAllDoorsClosed();
		}
		if (doOpenCloseGameEvents)
		{
			GameEventsManager.Instance.ItemActionOccurred(doorWorldItem.Data, "CLOSED");
		}
	}

	public void RefreshSpawnsAndLockItemsIfAllDoorsClosed()
	{
		bool flag = true;
		for (int i = 0; i < doorHinges.Length; i++)
		{
			if (!doorHinges[i].IsLowerLocked)
			{
				flag = false;
				break;
			}
		}
		if (flag)
		{
			if (doRespawn)
			{
				RespawnAll();
			}
			StartCoroutine(WaitAndSetLockedState(true, 0.01f));
		}
	}

	private void RespawnAll()
	{
		for (int i = 0; i < itemRespawners.Length; i++)
		{
			itemRespawners[i].RespawnIfRequired();
		}
	}

	private void SetLockedStateOfItemsWithinContainer(bool state)
	{
		if (doUnparenting)
		{
			if (state == isUnparented)
			{
				return;
			}
			isUnparented = state;
			if (state)
			{
				unparentedItems = new List<PickupableItem>();
				unparentedOriginalPositions = new List<Vector3>();
				unparentedOriginalParents = new List<Transform>();
				unparentedItems.AddRange(itemCollectionZone.ItemsInCollection);
				for (int i = 0; i < unparentedItems.Count; i++)
				{
					if (unparentedItems[i].Rigidbody != null)
					{
						unparentedItems[i].Rigidbody.isKinematic = true;
					}
					else
					{
						Debug.LogError(unparentedItems[i].gameObject.name + " doesn't have a Rigidbody", unparentedItems[i].gameObject);
					}
					unparentedItems[i].enabled = false;
					unparentedOriginalPositions.Add(unparentedItems[i].transform.localPosition);
					unparentedOriginalParents.Add(unparentedItems[i].transform.parent);
					unparentedItems[i].transform.SetParent(GlobalStorage.Instance.ContentRoot);
					unparentedItems[i].transform.position += Vector3.up * 1000f;
				}
			}
			else if (unparentedItems != null)
			{
				for (int j = 0; j < unparentedItems.Count; j++)
				{
					unparentedItems[j].transform.SetParent(unparentedOriginalParents[j]);
					unparentedItems[j].transform.localPosition = unparentedOriginalPositions[j];
					unparentedItems[j].enabled = true;
					unparentedItems[j].Rigidbody.isKinematic = false;
					unparentedItems[j].Rigidbody.velocity = Vector3.zero;
					unparentedItems[j].Rigidbody.angularVelocity = Vector3.zero;
				}
				unparentedItems = null;
				unparentedOriginalPositions = null;
			}
			else
			{
				Debug.LogError("Unparented items list was null! :: " + base.gameObject.name, base.gameObject);
			}
		}
		else
		{
			for (int k = 0; k < itemCollectionZone.ItemsInCollection.Count; k++)
			{
				itemCollectionZone.ItemsInCollection[k].Rigidbody.isKinematic = state;
				itemCollectionZone.ItemsInCollection[k].enabled = !state;
			}
		}
	}
}
