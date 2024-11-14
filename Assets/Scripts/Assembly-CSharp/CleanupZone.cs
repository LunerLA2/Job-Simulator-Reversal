using System;
using System.Collections;
using System.Collections.Generic;
using OwlchemyVR;
using UnityEngine;

public class CleanupZone : MonoBehaviour
{
	private const string AUTO_MECHANIC_LEVEL_ID = "AutoMechanic";

	private int MAX_ITEMS_OUTSIDE_PLAYSPACE = 10;

	private int MAX_ITEMS_OUTSIDE_PLAYSPACE_PSVR = 3;

	private int MAX_ITEMS_OUTSIDE_PLAYSPACE_AUTO = 5;

	private int MAX_ITEMS_OUTSIDE_PLAYSPACE_AUTO_PSVR = 3;

	private float secondsToWaitBetweenChecks = 0.5f;

	private float minimumSecondsOfStillnessBeforeDestroying = 1.5f;

	private float velocityThresholdForStillness = 0.25f;

	[SerializeField]
	private bool destroyItemsThatExistByDefault;

	[SerializeField]
	private bool destroyAllItems;

	[SerializeField]
	private ItemRecyclerManager recyclingManager;

	[SerializeField]
	private CompoundItemCollectionZone itemCollectionZone;

	[SerializeField]
	private CompoundItemCollectionZone safeZone;

	private List<PickupableItem> itemsThatExistedOnSceneLoad = new List<PickupableItem>();

	private Dictionary<WorldItemData, int> itemDataCountsThatExistedOnSceneLoad = new Dictionary<WorldItemData, int>();

	private bool hasInitializedItemList;

	[SerializeField]
	private AudioClip soundToPlayOnDestroy;

	[SerializeField]
	private ParticleSystem pfxToPlayOnDestroy;

	[SerializeField]
	private FilteredRigidbodyEnterExitTriggerEvents[] triggerEventsToOptimize;

	private float lastCheckTime;

	private List<PickupableItem> allItemsBeingTracked = new List<PickupableItem>();

	private List<CleanupZoneTrackedItem> trackedItems = new List<CleanupZoneTrackedItem>();

	private int GetMaxItemsOutsidePlayspace()
	{
		string empty = string.Empty;
		if (JobBoardManager.instance.EndlessModeStatusController != null && JobBoardManager.instance.EndlessModeStatusController.JobStateData != null)
		{
			empty = JobBoardManager.instance.EndlessModeStatusController.JobStateData.ID;
		}
		else
		{
			if (JobBoardManager.instance.JobStatusController == null || JobBoardManager.instance.JobStatusController.JobStateData == null)
			{
				return 0;
			}
			empty = JobBoardManager.instance.JobStatusController.JobStateData.ID;
		}
		if (empty == "AutoMechanic")
		{
			return MAX_ITEMS_OUTSIDE_PLAYSPACE_AUTO;
		}
		return MAX_ITEMS_OUTSIDE_PLAYSPACE;
	}

	private void OnEnable()
	{
		CompoundItemCollectionZone compoundItemCollectionZone = itemCollectionZone;
		compoundItemCollectionZone.OnItemsInCollectionRemoved = (Action<CompoundItemCollectionZone, PickupableItem>)Delegate.Combine(compoundItemCollectionZone.OnItemsInCollectionRemoved, new Action<CompoundItemCollectionZone, PickupableItem>(ItemRemoved));
	}

	private void OnDisable()
	{
		CompoundItemCollectionZone compoundItemCollectionZone = itemCollectionZone;
		compoundItemCollectionZone.OnItemsInCollectionRemoved = (Action<CompoundItemCollectionZone, PickupableItem>)Delegate.Remove(compoundItemCollectionZone.OnItemsInCollectionRemoved, new Action<CompoundItemCollectionZone, PickupableItem>(ItemRemoved));
	}

	private void ItemRemoved(CompoundItemCollectionZone zone, PickupableItem item)
	{
		StopTrackingItem(item);
	}

	private void Start()
	{
		StartCoroutine(WaitAndListInitialItems());
	}

	private IEnumerator WaitAndListInitialItems()
	{
		yield return null;
		itemsThatExistedOnSceneLoad.AddRange(itemCollectionZone.ItemsInCollection);
		for (int k = 0; k < itemsThatExistedOnSceneLoad.Count; k++)
		{
			WorldItemData data = itemsThatExistedOnSceneLoad[k].InteractableItem.WorldItemData;
			if (data != null)
			{
				if (itemDataCountsThatExistedOnSceneLoad.ContainsKey(data))
				{
					Dictionary<WorldItemData, int> dictionary;
					Dictionary<WorldItemData, int> dictionary2 = (dictionary = itemDataCountsThatExistedOnSceneLoad);
					WorldItemData key;
					WorldItemData key2 = (key = data);
					int num = dictionary[key];
					dictionary2[key2] = num + 1;
				}
				else
				{
					itemDataCountsThatExistedOnSceneLoad[data] = 1;
				}
			}
		}
		if (!destroyItemsThatExistByDefault)
		{
			List<Rigidbody> rigidbodiesToIgnore = new List<Rigidbody>();
			for (int j = 0; j < triggerEventsToOptimize.Length; j++)
			{
				FilteredRigidbodyEnterExitTriggerEvents filter = triggerEventsToOptimize[j];
				for (int l = 0; l < filter.ActiveRigidbodiesTriggerInfo.Count; l++)
				{
					if (filter.ActiveRigidbodiesTriggerInfo[l].Rigidbody != null)
					{
						rigidbodiesToIgnore.Add(filter.ActiveRigidbodiesTriggerInfo[l].Rigidbody);
					}
				}
			}
			for (int i = 0; i < triggerEventsToOptimize.Length; i++)
			{
				triggerEventsToOptimize[i].AddToListOfRigidbodiesToIgnore(rigidbodiesToIgnore);
			}
		}
		hasInitializedItemList = true;
	}

	private void Update()
	{
		if (hasInitializedItemList && Time.realtimeSinceStartup - lastCheckTime > secondsToWaitBetweenChecks)
		{
			CheckForItemsThatNeedToBeTracked();
			UpdateTrackedItemsThatAreWaitingForTimers();
			lastCheckTime = Time.realtimeSinceStartup;
		}
	}

	private void UpdateTrackedItemsThatAreWaitingForTimers()
	{
		for (int i = 0; i < trackedItems.Count; i++)
		{
			if (!trackedItems[i].HasTimerFinished && Time.realtimeSinceStartup - trackedItems[i].TimeEnteredCleanupZone >= minimumSecondsOfStillnessBeforeDestroying)
			{
				trackedItems[i].FinishTimer();
				TrackedItemTimerHasFinished(trackedItems[i]);
			}
		}
	}

	private void TrackedItemTimerHasFinished(CleanupZoneTrackedItem item)
	{
		if (GetMaxItemsOutsidePlayspace() == 0)
		{
			Debug.Log("Destroy one because the max items outside is 0");
			DestroyOldestObject();
			return;
		}
		int num = -1;
		if (recyclingManager != null)
		{
			num = recyclingManager.NumberOfItemThatNeedsToExistRightNow(item.PickupableItem.InteractableItem.WorldItemData);
		}
		else if (destroyAllItems)
		{
			num = 0;
		}
		if (num > -1)
		{
			int countOfTrackedItemsReadyToBeDestroyed = GetCountOfTrackedItemsReadyToBeDestroyed(item.PickupableItem.InteractableItem.WorldItemData);
			int currentNumberOfItems = WorldItemTrackingManager.Instance.GetCurrentNumberOfItems(item.PickupableItem.InteractableItem.WorldItemData);
			int countOfSpecificItemThatExistedInitially = GetCountOfSpecificItemThatExistedInitially(item.PickupableItem.InteractableItem.WorldItemData);
			Debug.Log("Important item finished timer (" + item.PickupableItem.InteractableItem.WorldItemData.ItemFullName + "): " + currentNumberOfItems + " total in world - " + countOfTrackedItemsReadyToBeDestroyed + " outside playspace - " + countOfSpecificItemThatExistedInitially + " existed on load / " + num + " required in playspace");
			if (currentNumberOfItems - countOfSpecificItemThatExistedInitially - countOfTrackedItemsReadyToBeDestroyed < num || num == 0)
			{
				Debug.Log("There are not enough of the item in the playspace, destroy one to trigger the recycler");
				DestroyOldestObject(item.PickupableItem.InteractableItem.WorldItemData, true);
			}
		}
		if (trackedItems.Count > GetMaxItemsOutsidePlayspace())
		{
			DestroyOldestObject();
		}
	}

	public int GetCountOfSpecificItemThatExistedInitially(WorldItemData item)
	{
		if (hasInitializedItemList && !destroyItemsThatExistByDefault)
		{
			if (itemDataCountsThatExistedOnSceneLoad.ContainsKey(item))
			{
				return itemDataCountsThatExistedOnSceneLoad[item];
			}
			return 0;
		}
		return 0;
	}

	public int GetCountOfTrackedItemsReadyToBeDestroyed(WorldItemData worldItemData)
	{
		int num = 0;
		for (int i = 0; i < trackedItems.Count; i++)
		{
			if (trackedItems[i].HasTimerFinished && trackedItems[i].PickupableItem.InteractableItem.WorldItemData == worldItemData)
			{
				num++;
			}
		}
		return num;
	}

	private void CheckForItemsThatNeedToBeTracked()
	{
		for (int i = 0; i < itemCollectionZone.ItemsInCollection.Count; i++)
		{
			PickupableItem pickupableItem = itemCollectionZone.ItemsInCollection[i];
			if (allItemsBeingTracked.Contains(pickupableItem))
			{
				continue;
			}
			bool flag = false;
			if (pickupableItem.Rigidbody != null && !pickupableItem.Rigidbody.isKinematic)
			{
				if (pickupableItem.Rigidbody.IsSleeping())
				{
					flag = true;
				}
				else if (pickupableItem.Rigidbody.velocity.magnitude <= velocityThresholdForStillness)
				{
					flag = true;
				}
			}
			if (!flag)
			{
				continue;
			}
			bool flag2 = false;
			if (safeZone != null && safeZone.ItemsInCollection.Contains(pickupableItem))
			{
				flag2 = true;
			}
			if (!flag2)
			{
				TrinketSpringController component = pickupableItem.gameObject.GetComponent<TrinketSpringController>();
				if (component != null)
				{
					flag2 = true;
				}
			}
			if (!flag2)
			{
				StasisFieldItem component2 = pickupableItem.gameObject.GetComponent<StasisFieldItem>();
				if (component2 != null && component2.IsInStasis)
				{
					flag2 = true;
				}
			}
			if (!flag2)
			{
				BeginTrackingItem(pickupableItem);
			}
		}
	}

	private void BeginTrackingItem(PickupableItem item)
	{
		CheckForNullPickupables();
		if (allItemsBeingTracked.Contains(item))
		{
			Debug.LogError("Item is being tracked twice: " + item.gameObject.name, item.gameObject);
			return;
		}
		allItemsBeingTracked.Add(item);
		CleanupZoneTrackedItem item2 = new CleanupZoneTrackedItem(item);
		trackedItems.Add(item2);
	}

	private void StopTrackingItem(PickupableItem item)
	{
		if (allItemsBeingTracked.Contains(item))
		{
			allItemsBeingTracked.Remove(item);
		}
		CleanupZoneTrackedItem cleanupZoneTrackedItem = FindTrackedItem(item);
		if (cleanupZoneTrackedItem != null && trackedItems.Contains(cleanupZoneTrackedItem))
		{
			trackedItems.Remove(cleanupZoneTrackedItem);
		}
		CheckForNullPickupables();
	}

	private void DestroyOldestObject(WorldItemData ofWorldItemType = null, bool definitelyIsImportant = false)
	{
		CleanupZoneTrackedItem cleanupZoneTrackedItem = FindOldestTrackedItem(ofWorldItemType);
		if (cleanupZoneTrackedItem != null)
		{
			if (cleanupZoneTrackedItem.PickupableItem != null)
			{
				StopTrackingItem(cleanupZoneTrackedItem.PickupableItem);
				bool flag = definitelyIsImportant;
				if (!flag && ofWorldItemType != null && recyclingManager != null)
				{
					flag = recyclingManager.NumberOfItemThatNeedsToExistRightNow(ofWorldItemType) > -1;
				}
				if (soundToPlayOnDestroy != null && flag)
				{
					AudioManager.Instance.Play(cleanupZoneTrackedItem.PickupableItem.transform.position, soundToPlayOnDestroy, 1f, 1f);
				}
				if (pfxToPlayOnDestroy != null)
				{
					pfxToPlayOnDestroy.transform.position = cleanupZoneTrackedItem.PickupableItem.transform.position;
					pfxToPlayOnDestroy.Play();
				}
				GameEventsManager.Instance.ItemActionOccurred(cleanupZoneTrackedItem.PickupableItem.InteractableItem.WorldItemData, "DESTROYED");
				Debug.Log("Item was cleaned up: " + cleanupZoneTrackedItem.PickupableItem.gameObject.name);
				UnityEngine.Object.Destroy(cleanupZoneTrackedItem.PickupableItem.gameObject);
			}
		}
		else
		{
			Debug.LogWarning("There was no oldest item to destroy - this should probably never happen, but it won't cause any new problems");
		}
	}

	private CleanupZoneTrackedItem FindOldestTrackedItem(WorldItemData ofWorldItemType = null)
	{
		CheckForNullPickupables();
		CleanupZoneTrackedItem cleanupZoneTrackedItem = null;
		for (int i = 0; i < trackedItems.Count; i++)
		{
			if (cleanupZoneTrackedItem == null)
			{
				if (ofWorldItemType == null || ofWorldItemType == trackedItems[i].PickupableItem.InteractableItem.WorldItemData)
				{
					cleanupZoneTrackedItem = trackedItems[i];
				}
			}
			else if ((ofWorldItemType == null || trackedItems[i].PickupableItem.InteractableItem.WorldItemData == ofWorldItemType) && trackedItems[i].TimeEnteredCleanupZone < cleanupZoneTrackedItem.TimeEnteredCleanupZone)
			{
				cleanupZoneTrackedItem = trackedItems[i];
			}
		}
		return cleanupZoneTrackedItem;
	}

	private CleanupZoneTrackedItem FindTrackedItem(PickupableItem pickupableItem)
	{
		CheckForNullPickupables();
		for (int i = 0; i < trackedItems.Count; i++)
		{
			if (trackedItems[i].PickupableItem == pickupableItem)
			{
				return trackedItems[i];
			}
		}
		return null;
	}

	private void CheckForNullPickupables()
	{
		for (int i = 0; i < trackedItems.Count; i++)
		{
			if (trackedItems[i].PickupableItem == null)
			{
				Debug.Log("CleanupItem " + i + " became null, forgetting it.");
				trackedItems.RemoveAt(i);
				i--;
			}
		}
	}
}
