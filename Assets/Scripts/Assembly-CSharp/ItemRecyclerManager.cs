using System;
using System.Collections;
using System.Collections.Generic;
using OwlchemyVR;
using UnityEngine;

public class ItemRecyclerManager : MonoBehaviour
{
	[SerializeField]
	protected SceneRecyclingData recyclingData;

	[SerializeField]
	protected SceneRecyclingData overtimeRecycleData;

	private bool isEndlessMode;

	[SerializeField]
	private CleanupZone cleanupZone;

	[SerializeField]
	private BotInventoryFlyoutItem flyoutItemPrefab;

	[SerializeField]
	private AudioClip soundToPlayOnSpawn;

	[SerializeField]
	private ParticleSystem pfxToPlayOnSpawn;

	private int frameOfLastBeneficialEvent = -1;

	private List<WorldItemData> allWorldItemsBeingWatched;

	private Dictionary<WorldItemData, RecycleInfo> recycleInfoLookup;

	private Dictionary<int, int> lastPrefabIndexSpawnedForInfoOfIndex;

	public SceneRecyclingData RecyclingData
	{
		get
		{
			return recyclingData;
		}
	}

	public int NumberOfItemThatNeedsToExistRightNow(WorldItemData item)
	{
		if (item == null)
		{
			return -1;
		}
		if (recycleInfoLookup.ContainsKey(item))
		{
			if (DoesRecycleInfoNeedToMaintainQuantityRightNow(recycleInfoLookup[item]))
			{
				return recycleInfoLookup[item].MinimumQuantityToMaintainInScene;
			}
			return -1;
		}
		return -1;
	}

	private void Awake()
	{
		if (GenieManager.DoesContainGenieMode(GlobalStorage.Instance.CurrentGenieModes, JobGenieCartridge.GenieModeTypes.EndlessMode) && overtimeRecycleData != null)
		{
			Debug.Log("Overriding SceneRecyclingData for Endless Mode using : " + overtimeRecycleData.name);
			recyclingData = overtimeRecycleData;
			isEndlessMode = true;
		}
		BuildLookupDictionary();
	}

	protected void BuildLookupDictionary()
	{
		StopAllCoroutines();
		allWorldItemsBeingWatched = new List<WorldItemData>();
		recycleInfoLookup = new Dictionary<WorldItemData, RecycleInfo>();
		lastPrefabIndexSpawnedForInfoOfIndex = new Dictionary<int, int>();
		for (int i = 0; i < recyclingData.RecycleInfos.Count; i++)
		{
			RecycleInfo recycleInfo = recyclingData.RecycleInfos[i];
			for (int j = 0; j < recycleInfo.WorldItemsToMonitor.Count; j++)
			{
				if (!allWorldItemsBeingWatched.Contains(recycleInfo.WorldItemsToMonitor[j]))
				{
					allWorldItemsBeingWatched.Add(recycleInfo.WorldItemsToMonitor[j]);
					recycleInfoLookup[recycleInfo.WorldItemsToMonitor[j]] = recycleInfo;
					continue;
				}
				Debug.LogError("WorldItemData " + recycleInfo.WorldItemsToMonitor[j].ItemName + " is used in " + recyclingData.name + " more than once!");
			}
			lastPrefabIndexSpawnedForInfoOfIndex[i] = -1;
		}
	}

	private void OnEnable()
	{
		WorldItemTrackingManager instance = WorldItemTrackingManager.Instance;
		instance.OnItemRemoved = (Action<WorldItemData, int, bool>)Delegate.Combine(instance.OnItemRemoved, new Action<WorldItemData, int, bool>(ItemWasRemovedFromWorld));
		JobBoardManager instance2 = JobBoardManager.instance;
		instance2.OnPageStarted = (Action<PageStatusController>)Delegate.Combine(instance2.OnPageStarted, new Action<PageStatusController>(PageChanged));
		JobBoardManager instance3 = JobBoardManager.instance;
		instance3.OnTaskStarted = (Action<TaskStatusController>)Delegate.Combine(instance3.OnTaskStarted, new Action<TaskStatusController>(TaskChanged));
		JobBoardManager instance4 = JobBoardManager.instance;
		instance4.OnSubtaskCounterChange = (Action<SubtaskStatusController, bool>)Delegate.Combine(instance4.OnSubtaskCounterChange, new Action<SubtaskStatusController, bool>(SubtaskCounterChange));
	}

	private void OnDisable()
	{
		if (WorldItemTrackingManager.InstanceNoCreate != null)
		{
			WorldItemTrackingManager instanceNoCreate = WorldItemTrackingManager.InstanceNoCreate;
			instanceNoCreate.OnItemRemoved = (Action<WorldItemData, int, bool>)Delegate.Remove(instanceNoCreate.OnItemRemoved, new Action<WorldItemData, int, bool>(ItemWasRemovedFromWorld));
		}
		JobBoardManager instance = JobBoardManager.instance;
		instance.OnPageStarted = (Action<PageStatusController>)Delegate.Remove(instance.OnPageStarted, new Action<PageStatusController>(PageChanged));
		JobBoardManager instance2 = JobBoardManager.instance;
		instance2.OnTaskStarted = (Action<TaskStatusController>)Delegate.Remove(instance2.OnTaskStarted, new Action<TaskStatusController>(TaskChanged));
		JobBoardManager instance3 = JobBoardManager.instance;
		instance3.OnSubtaskCounterChange = (Action<SubtaskStatusController, bool>)Delegate.Remove(instance3.OnSubtaskCounterChange, new Action<SubtaskStatusController, bool>(SubtaskCounterChange));
	}

	private void ItemWasRemovedFromWorld(WorldItemData data, int newCount, bool isBeingSwitched)
	{
		if (recycleInfoLookup.ContainsKey(data))
		{
			StartCoroutine(WaitForSwitchAndThenCheck(data));
		}
	}

	private IEnumerator WaitForSwitchAndThenCheck(WorldItemData data)
	{
		yield return null;
		CheckRecycleInfo(recycleInfoLookup[data]);
	}

	private void SubtaskCounterChange(SubtaskStatusController subtask, bool isPositive)
	{
		if (isPositive)
		{
			frameOfLastBeneficialEvent = Time.frameCount;
		}
	}

	private void TaskChanged(TaskStatusController task)
	{
		RecheckDueToTaskOrPageChange();
	}

	private void PageChanged(PageStatusController page)
	{
		RecheckDueToTaskOrPageChange();
	}

	private void RecheckDueToTaskOrPageChange()
	{
		for (int i = 0; i < allWorldItemsBeingWatched.Count; i++)
		{
			RecycleInfo recycleInfo = recycleInfoLookup[allWorldItemsBeingWatched[i]];
			if (!recycleInfo.AlwaysMaintainQuantity && recycleInfo.RefillQuantityOnTaskOrPageStart)
			{
				CheckRecycleInfo(recycleInfo);
			}
		}
	}

	public bool WouldRecyclerHandleThis(WorldItemData data)
	{
		if (!base.gameObject.activeInHierarchy)
		{
			return false;
		}
		if (recycleInfoLookup.ContainsKey(data))
		{
			RecycleInfo recycleInfo = recycleInfoLookup[data];
			if (DoesRecycleInfoNeedToMaintainQuantityRightNow(recycleInfo) && recycleInfo.MinimumQuantityToMaintainInScene > 0)
			{
				int num = GetCountOfItemsInWorldForRecycleInfo(recycleInfo) - 1;
				if (num < recycleInfo.MinimumQuantityToMaintainInScene)
				{
					return true;
				}
			}
		}
		return false;
	}

	private void CheckRecycleInfo(RecycleInfo info)
	{
		if (!DoesRecycleInfoNeedToMaintainQuantityRightNow(info) || info.MinimumQuantityToMaintainInScene <= 0)
		{
			return;
		}
		int countOfItemsInWorldForRecycleInfo = GetCountOfItemsInWorldForRecycleInfo(info);
		if (countOfItemsInWorldForRecycleInfo < info.MinimumQuantityToMaintainInScene)
		{
			for (int i = countOfItemsInWorldForRecycleInfo; i < info.MinimumQuantityToMaintainInScene; i++)
			{
				RespawnOneItem(info);
			}
		}
	}

	private int GetCountOfItemsInWorldForRecycleInfo(RecycleInfo info)
	{
		int num = 0;
		for (int i = 0; i < info.WorldItemsToMonitor.Count; i++)
		{
			num += WorldItemTrackingManager.Instance.GetCurrentNumberOfItems(info.WorldItemsToMonitor[i]);
			num -= cleanupZone.GetCountOfSpecificItemThatExistedInitially(info.WorldItemsToMonitor[i]);
			num -= cleanupZone.GetCountOfTrackedItemsReadyToBeDestroyed(info.WorldItemsToMonitor[i]);
		}
		return num;
	}

	private bool DoesRecycleInfoNeedToMaintainQuantityRightNow(RecycleInfo info)
	{
		if (info.AlwaysMaintainQuantity)
		{
			return true;
		}
		if (info.IgnoreIfDestructionWasBeneficial && Time.frameCount - frameOfLastBeneficialEvent <= 1)
		{
			return false;
		}
		if (isEndlessMode)
		{
			if (JobBoardManager.instance.GetCurrentPageData() == null)
			{
				return false;
			}
			if (info.MaintainQuantityDuringPagesStringNames.Contains(JobBoardManager.instance.GetCurrentPageData().name))
			{
				return true;
			}
			return false;
		}
		if (info.MaintainQuantityDuringTasks.Contains(JobBoardManager.instance.GetCurrentTaskData()))
		{
			return true;
		}
		if (info.MaintainQuantityDuringPages.Contains(JobBoardManager.instance.GetCurrentPageData()))
		{
			return true;
		}
		return false;
	}

	public void RespawnOneItem(RecycleInfo info)
	{
		GameObject nextPrefabToSpawn = GetNextPrefabToSpawn(info);
		UniqueObject uniqueObject = null;
		RecycleSpawnLocation recycleSpawnLocation = null;
		List<RecycleSpawnLocation> prioritizedListOfSpawnLocations = GetPrioritizedListOfSpawnLocations(info);
		string empty = string.Empty;
		RecycleSpawnLocation recycleSpawnLocation2 = null;
		for (int i = 0; i < prioritizedListOfSpawnLocations.Count; i++)
		{
			recycleSpawnLocation2 = prioritizedListOfSpawnLocations[i];
			empty = recycleSpawnLocation2.UniqueObjectID;
			UniqueObject objectByName = BotUniqueElementManager.Instance.GetObjectByName(empty);
			if (!(objectByName != null))
			{
				continue;
			}
			if (recycleSpawnLocation2.SpawnType == RecycleSpawnLocation.LocationSpawnTypes.LookForAttachpoint)
			{
				if (!objectByName.AssociatedAttachpoint.IsOccupied)
				{
					uniqueObject = objectByName;
					recycleSpawnLocation = recycleSpawnLocation2;
					break;
				}
				continue;
			}
			if (objectByName.AssociatedClearAreaChecker != null)
			{
				if (objectByName.AssociatedClearAreaChecker.NumberOfCollidersInArea() == 0)
				{
					uniqueObject = objectByName;
					recycleSpawnLocation = recycleSpawnLocation2;
					break;
				}
				continue;
			}
			uniqueObject = objectByName;
			recycleSpawnLocation = recycleSpawnLocation2;
			break;
		}
		if (uniqueObject == null && recyclingData.FallbackRespawnLocationID != string.Empty)
		{
			uniqueObject = BotUniqueElementManager.Instance.GetObjectByName(recyclingData.FallbackRespawnLocationID);
		}
		GameObject gameObject = UnityEngine.Object.Instantiate(nextPrefabToSpawn, uniqueObject.transform.position, uniqueObject.transform.rotation) as GameObject;
		BasePrefabSpawner component = gameObject.GetComponent<BasePrefabSpawner>();
		if (component != null)
		{
			gameObject = component.LastSpawnedPrefabGO;
		}
		gameObject.transform.SetParent(GlobalStorage.Instance.ContentRoot, true);
		if (recycleSpawnLocation != null)
		{
			if (recycleSpawnLocation.SpawnType == RecycleSpawnLocation.LocationSpawnTypes.LookForAttachpoint)
			{
				AttachableObject component2 = gameObject.GetComponent<AttachableObject>();
				if (component2 != null)
				{
					component2.AttachTo(uniqueObject.AssociatedAttachpoint);
				}
				else
				{
					Debug.LogError(gameObject.name + " was told to recycle on an Attachpoint, but it has no AttachableObject", gameObject);
				}
			}
			else if (recycleSpawnLocation.SpawnType == RecycleSpawnLocation.LocationSpawnTypes.HoverAtPosition)
			{
				Rigidbody component3 = gameObject.GetComponent<Rigidbody>();
				GrabbableItem component4 = gameObject.GetComponent<GrabbableItem>();
				if (component3 != null)
				{
					component3.isKinematic = true;
				}
				BotInventoryFlyoutItem botInventoryFlyoutItem = UnityEngine.Object.Instantiate(flyoutItemPrefab);
				botInventoryFlyoutItem.transform.SetParent(GlobalStorage.Instance.ContentRoot, false);
				botInventoryFlyoutItem.transform.position = uniqueObject.transform.position;
				botInventoryFlyoutItem.transform.rotation = uniqueObject.transform.rotation;
				botInventoryFlyoutItem.SetupGrabbableEvent(component4);
				botInventoryFlyoutItem.AssignItem();
			}
		}
		if (soundToPlayOnSpawn != null)
		{
			AudioManager.Instance.Play(gameObject.transform.position, soundToPlayOnSpawn, 1f, 1f);
		}
		if (pfxToPlayOnSpawn != null)
		{
			pfxToPlayOnSpawn.transform.position = gameObject.transform.position;
			pfxToPlayOnSpawn.Play();
		}
	}

	public void RespawnItemWithItem(PickupableItem item)
	{
		UniqueObject uniqueObject = null;
		RecycleSpawnLocation recycleSpawnLocation = null;
		WorldItemData worldItemData = null;
		if (item != null)
		{
			worldItemData = item.InteractableItem.WorldItemData;
		}
		if (worldItemData == null)
		{
			return;
		}
		RecycleInfo recycleInfo = null;
		if (recycleInfoLookup.ContainsKey(worldItemData))
		{
			recycleInfo = recycleInfoLookup[worldItemData];
		}
		if (recycleInfo == null)
		{
			return;
		}
		List<RecycleSpawnLocation> prioritizedListOfSpawnLocations = GetPrioritizedListOfSpawnLocations(recycleInfo);
		string empty = string.Empty;
		RecycleSpawnLocation recycleSpawnLocation2 = null;
		for (int i = 0; i < prioritizedListOfSpawnLocations.Count; i++)
		{
			recycleSpawnLocation2 = prioritizedListOfSpawnLocations[i];
			empty = recycleSpawnLocation2.UniqueObjectID;
			UniqueObject objectByName = BotUniqueElementManager.Instance.GetObjectByName(empty);
			if (!(objectByName != null))
			{
				continue;
			}
			if (recycleSpawnLocation2.SpawnType == RecycleSpawnLocation.LocationSpawnTypes.LookForAttachpoint)
			{
				if (!objectByName.AssociatedAttachpoint.IsOccupied)
				{
					uniqueObject = objectByName;
					recycleSpawnLocation = recycleSpawnLocation2;
					break;
				}
				continue;
			}
			if (objectByName.AssociatedClearAreaChecker != null)
			{
				if (objectByName.AssociatedClearAreaChecker.NumberOfCollidersInArea() == 0)
				{
					uniqueObject = objectByName;
					recycleSpawnLocation = recycleSpawnLocation2;
					break;
				}
				continue;
			}
			uniqueObject = objectByName;
			recycleSpawnLocation = recycleSpawnLocation2;
			break;
		}
		if (uniqueObject == null && recyclingData.FallbackRespawnLocationID != string.Empty)
		{
			uniqueObject = BotUniqueElementManager.Instance.GetObjectByName(recyclingData.FallbackRespawnLocationID);
		}
		item.transform.position = uniqueObject.transform.position;
		item.transform.rotation = uniqueObject.transform.rotation;
		Rigidbody rigidbody = item.Rigidbody;
		if (rigidbody != null)
		{
			rigidbody.MovePosition(item.transform.position);
			rigidbody.MoveRotation(item.transform.rotation);
			rigidbody.velocity = Vector3.zero;
			rigidbody.angularVelocity = Vector3.zero;
		}
		GameObject gameObject = item.gameObject;
		if (recycleSpawnLocation != null)
		{
			if (recycleSpawnLocation.SpawnType == RecycleSpawnLocation.LocationSpawnTypes.LookForAttachpoint)
			{
				AttachableObject component = gameObject.GetComponent<AttachableObject>();
				if (component != null)
				{
					component.AttachTo(uniqueObject.AssociatedAttachpoint);
				}
				else
				{
					Debug.LogError(gameObject.name + " was told to recycle on an Attachpoint, but it has no AttachableObject", gameObject);
				}
			}
			else if (recycleSpawnLocation.SpawnType == RecycleSpawnLocation.LocationSpawnTypes.HoverAtPosition)
			{
				Rigidbody component2 = gameObject.GetComponent<Rigidbody>();
				GrabbableItem component3 = gameObject.GetComponent<GrabbableItem>();
				if (component2 != null)
				{
					component2.isKinematic = true;
				}
				BotInventoryFlyoutItem botInventoryFlyoutItem = UnityEngine.Object.Instantiate(flyoutItemPrefab);
				botInventoryFlyoutItem.transform.SetParent(GlobalStorage.Instance.ContentRoot, false);
				botInventoryFlyoutItem.transform.position = uniqueObject.transform.position;
				botInventoryFlyoutItem.transform.rotation = uniqueObject.transform.rotation;
				botInventoryFlyoutItem.SetupGrabbableEvent(component3);
				botInventoryFlyoutItem.AssignItem();
			}
		}
		if (soundToPlayOnSpawn != null)
		{
			AudioManager.Instance.Play(gameObject.transform.position, soundToPlayOnSpawn, 1f, 1f);
		}
		if (pfxToPlayOnSpawn != null)
		{
			pfxToPlayOnSpawn.transform.position = gameObject.transform.position;
			pfxToPlayOnSpawn.Play();
		}
	}

	private GameObject GetNextPrefabToSpawn(RecycleInfo info)
	{
		int num = 0;
		if (info.PrefabSelectionMode == RecycleInfo.RespawnPrefabSelectionMode.Random)
		{
			num = UnityEngine.Random.Range(0, info.PrefabsToSpawnAsReplacement.Count);
		}
		else if (info.PrefabSelectionMode == RecycleInfo.RespawnPrefabSelectionMode.CycleInOrder)
		{
			int key = recyclingData.RecycleInfos.IndexOf(info);
			num = lastPrefabIndexSpawnedForInfoOfIndex[key] + 1;
			if (num >= info.PrefabsToSpawnAsReplacement.Count || num < 0)
			{
				num = 0;
			}
			lastPrefabIndexSpawnedForInfoOfIndex[key] = num;
		}
		return info.PrefabsToSpawnAsReplacement[num];
	}

	private List<RecycleSpawnLocation> GetPrioritizedListOfSpawnLocations(RecycleInfo info)
	{
		List<RecycleSpawnLocation> list = new List<RecycleSpawnLocation>();
		if (info.LocationSelectionMode == RecycleInfo.RespawnLocationSelectionMode.Random)
		{
			List<RecycleSpawnLocation> list2 = new List<RecycleSpawnLocation>();
			list2.AddRange(info.PossibleSpawnLocations);
			while (list2.Count > 0)
			{
				int index = UnityEngine.Random.Range(0, list2.Count);
				list.Add(list2[index]);
				list2.RemoveAt(index);
			}
		}
		else if (info.LocationSelectionMode == RecycleInfo.RespawnLocationSelectionMode.PrioritizeAsListed)
		{
			list.AddRange(info.PossibleSpawnLocations);
		}
		return list;
	}
}
