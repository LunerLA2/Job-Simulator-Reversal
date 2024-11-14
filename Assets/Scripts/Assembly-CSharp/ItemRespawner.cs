using System;
using System.Collections;
using OwlchemyVR;
using UnityEngine;

public class ItemRespawner : MonoBehaviour
{
	private const float DIST_FAR_ENOUGH_TO_ALLOW_RESPAWN = 0.25f;

	[SerializeField]
	private BasePrefabSpawner spawnerInScene;

	private PickupableItem lastItemSpawned;

	private EdibleItem lastItemEdible;

	private ShatterOnCollision lastItemShatter;

	private bool isRespawnAvailable = true;

	private bool isWaitingOnGrab;

	public PickupableItem LastItemSpawned
	{
		get
		{
			return lastItemSpawned;
		}
	}

	private void OnDisable()
	{
		if (isWaitingOnGrab)
		{
			if (lastItemSpawned != null)
			{
				PickupableItem pickupableItem = lastItemSpawned;
				pickupableItem.OnGrabbed = (Action<GrabbableItem>)Delegate.Remove(pickupableItem.OnGrabbed, new Action<GrabbableItem>(ItemPickedUp));
			}
			isWaitingOnGrab = false;
			isRespawnAvailable = true;
		}
	}

	private void RespawnItem()
	{
		if (isRespawnAvailable)
		{
			if (isWaitingOnGrab)
			{
				Debug.LogWarning("Should never be waiting on a grab when you are respawning, event issue");
			}
			isRespawnAvailable = false;
			spawnerInScene.SpawnPrefab();
			GameObject lastSpawnedPrefabGO = spawnerInScene.LastSpawnedPrefabGO;
			lastItemSpawned = lastSpawnedPrefabGO.GetComponent<PickupableItem>();
			if (lastItemSpawned == null)
			{
				Debug.LogError("'" + lastSpawnedPrefabGO.name + "' doesn't have a pickupable so it shouldn't be used in a respawner: " + base.gameObject.name);
			}
			else
			{
				lastItemSpawned.transform.parent = base.transform;
				lastItemSpawned.transform.position = base.transform.position;
				lastItemSpawned.transform.rotation = base.transform.rotation;
				lastItemSpawned.gameObject.RemoveCloneFromName();
				isWaitingOnGrab = true;
				PickupableItem pickupableItem = lastItemSpawned;
				pickupableItem.OnGrabbed = (Action<GrabbableItem>)Delegate.Combine(pickupableItem.OnGrabbed, new Action<GrabbableItem>(ItemPickedUp));
			}
			lastItemEdible = lastSpawnedPrefabGO.GetComponentInChildren<EdibleItem>();
			if (lastItemEdible != null)
			{
				EdibleItem edibleItem = lastItemEdible;
				edibleItem.OnFullyConsumed = (Action<EdibleItem>)Delegate.Combine(edibleItem.OnFullyConsumed, new Action<EdibleItem>(ItemEaten));
			}
			lastItemShatter = lastSpawnedPrefabGO.GetComponentInChildren<ShatterOnCollision>();
			if (lastItemShatter != null)
			{
				ShatterOnCollision shatterOnCollision = lastItemShatter;
				shatterOnCollision.OnShatter = (Action<ShatterOnCollision>)Delegate.Combine(shatterOnCollision.OnShatter, new Action<ShatterOnCollision>(ItemShattered));
			}
		}
	}

	private void ItemPickedUp(GrabbableItem item)
	{
		if (lastItemSpawned == null)
		{
			isRespawnAvailable = true;
			isWaitingOnGrab = false;
		}
		else if (item == lastItemSpawned)
		{
			PickupableItem pickupableItem = lastItemSpawned;
			pickupableItem.OnGrabbed = (Action<GrabbableItem>)Delegate.Remove(pickupableItem.OnGrabbed, new Action<GrabbableItem>(ItemPickedUp));
			isWaitingOnGrab = false;
			isRespawnAvailable = true;
		}
		else
		{
			Debug.LogError("Should never be a case where the item pickedup in the respawner isn't the last item spawned, error in event handling");
		}
	}

	private void ItemEaten(EdibleItem item)
	{
		if (lastItemSpawned == null)
		{
			isRespawnAvailable = true;
			isWaitingOnGrab = false;
		}
		else if (item == lastItemEdible)
		{
			EdibleItem edibleItem = lastItemEdible;
			edibleItem.OnFullyConsumed = (Action<EdibleItem>)Delegate.Remove(edibleItem.OnFullyConsumed, new Action<EdibleItem>(ItemEaten));
			PickupableItem pickupableItem = lastItemSpawned;
			pickupableItem.OnGrabbed = (Action<GrabbableItem>)Delegate.Remove(pickupableItem.OnGrabbed, new Action<GrabbableItem>(ItemPickedUp));
			lastItemEdible = null;
			isWaitingOnGrab = false;
			isRespawnAvailable = true;
		}
		else
		{
			Debug.LogError("ItemEaten did not find the lastItemEdible, likely error in event handling");
		}
	}

	private void ItemShattered(ShatterOnCollision item)
	{
		if (lastItemSpawned == null)
		{
			isRespawnAvailable = true;
			isWaitingOnGrab = false;
		}
		else if (item == lastItemShatter)
		{
			ShatterOnCollision shatterOnCollision = lastItemShatter;
			shatterOnCollision.OnShatter = (Action<ShatterOnCollision>)Delegate.Remove(shatterOnCollision.OnShatter, new Action<ShatterOnCollision>(ItemShattered));
			PickupableItem pickupableItem = lastItemSpawned;
			pickupableItem.OnGrabbed = (Action<GrabbableItem>)Delegate.Remove(pickupableItem.OnGrabbed, new Action<GrabbableItem>(ItemPickedUp));
			lastItemShatter = null;
			isWaitingOnGrab = false;
			isRespawnAvailable = true;
		}
	}

	public void RespawnIfRequired()
	{
		RespawnIfRequired(0f);
	}

	public void RespawnIfRequired(float delay)
	{
		if (isRespawnAvailable && IsDistanceFromRespawnEnough())
		{
			if (delay > 0f)
			{
				StartCoroutine(AttemptRespawnAfterDelay(delay));
			}
			else
			{
				RespawnItem();
			}
		}
	}

	private IEnumerator AttemptRespawnAfterDelay(float delay)
	{
		yield return new WaitForSeconds(delay);
		if (isRespawnAvailable && IsDistanceFromRespawnEnough())
		{
			RespawnItem();
		}
	}

	private bool IsDistanceFromRespawnEnough()
	{
		bool result = false;
		if (lastItemSpawned != null)
		{
			if (Vector3.Distance(base.transform.position, lastItemSpawned.transform.position) > 0.25f)
			{
				result = true;
			}
		}
		else
		{
			result = true;
		}
		return result;
	}

	public void LastItemRemovedFromContainer()
	{
		PickupableItem pickupableItem = lastItemSpawned;
		pickupableItem.OnGrabbed = (Action<GrabbableItem>)Delegate.Remove(pickupableItem.OnGrabbed, new Action<GrabbableItem>(ItemPickedUp));
		isWaitingOnGrab = false;
		isRespawnAvailable = true;
	}
}
