using System;
using System.Collections;
using System.Collections.Generic;
using OwlchemyVR;
using UnityEngine;

public class BotInventoryController : MonoBehaviour
{
	public enum EjectTypes
	{
		PhysicsPush = 0,
		TweenToLocation = 1,
		HoldOutToPlayer = 2,
		HoverToLocation = 3,
		CreateNearMe = 4
	}

	public enum EmptyTypes
	{
		TweenToLocation = 0,
		PhysicsPush = 1,
		HoldOutToPlayer = 2,
		HoverToLocation = 3,
		DestroyContents = 4,
		DropContents = 5,
		DestroyHeldOutItems = 6
	}

	public enum ItemFilteringTypes
	{
		OnlyTakeItemsOfInterest = 0,
		TakeAnyItem = 1,
		TakeAnyItemNotOfInterest = 2
	}

	private const float EJECT_FORCE = 200f;

	[SerializeField]
	private GameObject inventoryTriggersRoot;

	private Transform cachedInventoryParent;

	[SerializeField]
	private SphereCollider inventoryAdjustableRadius;

	private WorldItem myWorldItem;

	[SerializeField]
	private RigidbodyEnterExitTriggerEvents takeItemTrigger;

	[SerializeField]
	private Transform regularItemInventoryLocation;

	[SerializeField]
	private Transform largeItemInventoryLocation;

	[SerializeField]
	private Transform ejectLocation;

	[SerializeField]
	private Transform handOverLocation;

	[SerializeField]
	private Transform handOverEffect;

	[SerializeField]
	private BotInventoryFlyoutItem flyoutItemPrefab;

	[SerializeField]
	private AudioClip ejectPhysicsPushSound;

	[SerializeField]
	private AudioClip ejectTweenToLocationSound;

	[SerializeField]
	private AudioClip ejectHoverToLocationSound;

	[SerializeField]
	private AudioClip ejectHoldOutToPlayerSound;

	private List<BotInventoryEntry> currentInventory = new List<BotInventoryEntry>();

	private List<WorldItemData> itemsOfInterest = new List<WorldItemData>();

	private List<Transform> itemsBeingHeldOut = new List<Transform>();

	private List<BotInventoryFlyoutItem> flyoutItems = new List<BotInventoryFlyoutItem>();

	private ItemFilteringTypes itemFilteringType;

	private List<Rigidbody> recentlyEjectedItems = new List<Rigidbody>();

	public List<BotInventoryEntry> CurrentInventory
	{
		get
		{
			return currentInventory;
		}
	}

	private void Awake()
	{
		myWorldItem = GetComponent<WorldItem>();
		cachedInventoryParent = inventoryTriggersRoot.transform.parent;
		SetSystemObjectState(false);
	}

	private void OnEnable()
	{
		RigidbodyEnterExitTriggerEvents rigidbodyEnterExitTriggerEvents = takeItemTrigger;
		rigidbodyEnterExitTriggerEvents.OnRigidbodyEnterTrigger = (Action<Rigidbody>)Delegate.Combine(rigidbodyEnterExitTriggerEvents.OnRigidbodyEnterTrigger, new Action<Rigidbody>(RigidbodyEntered));
	}

	private void OnDisable()
	{
		RigidbodyEnterExitTriggerEvents rigidbodyEnterExitTriggerEvents = takeItemTrigger;
		rigidbodyEnterExitTriggerEvents.OnRigidbodyEnterTrigger = (Action<Rigidbody>)Delegate.Remove(rigidbodyEnterExitTriggerEvents.OnRigidbodyEnterTrigger, new Action<Rigidbody>(RigidbodyEntered));
	}

	private void SetSystemObjectState(bool state)
	{
		inventoryTriggersRoot.transform.SetParent((!state) ? GlobalStorage.Instance.ContentRoot : cachedInventoryParent, false);
		inventoryTriggersRoot.transform.localPosition = Vector3.zero;
		inventoryTriggersRoot.transform.localRotation = Quaternion.identity;
		inventoryTriggersRoot.transform.localScale = Vector3.one;
		inventoryTriggersRoot.SetActive(state);
	}

	public void SetRadiusMultiplier(float mult)
	{
		inventoryAdjustableRadius.radius = mult;
		inventoryAdjustableRadius.center = Vector3.forward * (mult - 1f);
	}

	public void SetItemFilteringType(ItemFilteringTypes type)
	{
		itemFilteringType = type;
		if (itemFilteringType == ItemFilteringTypes.TakeAnyItem || itemFilteringType == ItemFilteringTypes.TakeAnyItemNotOfInterest)
		{
			if (!inventoryTriggersRoot.activeSelf)
			{
				SetSystemObjectState(true);
			}
		}
		else if (itemFilteringType == ItemFilteringTypes.OnlyTakeItemsOfInterest)
		{
			SetSystemObjectState(itemsOfInterest.Count > 0);
		}
	}

	public void AddItemOfInterest(WorldItemData item)
	{
		if (!itemsOfInterest.Contains(item))
		{
			itemsOfInterest.Add(item);
		}
		if (!inventoryTriggersRoot.activeSelf)
		{
			SetSystemObjectState(true);
		}
	}

	public void RemoveItemOfInterest(WorldItemData item)
	{
		if (itemsOfInterest.Contains(item))
		{
			itemsOfInterest.Remove(item);
		}
		if (itemsOfInterest.Count == 0 && itemFilteringType == ItemFilteringTypes.OnlyTakeItemsOfInterest && inventoryTriggersRoot.activeSelf)
		{
			SetSystemObjectState(false);
		}
	}

	public void ClearItemsOfInterest()
	{
		itemsOfInterest.Clear();
		if (itemFilteringType == ItemFilteringTypes.OnlyTakeItemsOfInterest)
		{
			SetSystemObjectState(false);
		}
	}

	private void RigidbodyEntered(Rigidbody r)
	{
		if (recentlyEjectedItems.Contains(r) || GetInventoryEntryByTransform(r.transform) != null)
		{
			return;
		}
		WorldItem component = r.GetComponent<WorldItem>();
		if (!(component != null))
		{
			return;
		}
		bool flag = false;
		if (itemFilteringType == ItemFilteringTypes.OnlyTakeItemsOfInterest)
		{
			flag = itemsOfInterest.Contains(component.Data);
		}
		else if (itemFilteringType == ItemFilteringTypes.TakeAnyItem)
		{
			if (r.GetComponent<PickupableItem>() != null)
			{
				flag = true;
			}
		}
		else if (itemFilteringType == ItemFilteringTypes.TakeAnyItemNotOfInterest && r.GetComponent<PickupableItem>() != null)
		{
			flag = !itemsOfInterest.Contains(component.Data);
		}
		if (!flag)
		{
			return;
		}
		GrabbableItem component2 = r.GetComponent<GrabbableItem>();
		if (component2 != null && component2.CurrInteractableHand != null)
		{
			component2.CurrInteractableHand.TryRelease(false);
		}
		AddItemToInventory(r.transform);
		GameEventsManager.Instance.ItemAppliedToItemActionOccurred(component.Data, myWorldItem.Data, "ADDED_TO");
		GameEventsManager.Instance.ItemActionOccurred(myWorldItem.Data, "MISC_ITEM_ADDED");
		if (itemFilteringType == ItemFilteringTypes.OnlyTakeItemsOfInterest)
		{
			GameEventsManager.Instance.ItemActionOccurred(myWorldItem.Data, "BOT_RECEIVED_DESIRED_ITEM");
		}
		else if (itemFilteringType == ItemFilteringTypes.TakeAnyItem)
		{
			if (itemsOfInterest.Contains(component.Data))
			{
				GameEventsManager.Instance.ItemActionOccurred(myWorldItem.Data, "BOT_RECEIVED_DESIRED_ITEM");
			}
			else
			{
				GameEventsManager.Instance.ItemActionOccurred(myWorldItem.Data, "BOT_RECEIVED_UNDESIRED_ITEM");
			}
		}
		else if (itemFilteringType == ItemFilteringTypes.TakeAnyItemNotOfInterest)
		{
			GameEventsManager.Instance.ItemActionOccurred(myWorldItem.Data, "BOT_RECEIVED_UNDESIRED_ITEM");
		}
	}

	public void AddItemToInventory(Transform tr, bool isLargeItem = false)
	{
		bool flag = isLargeItem;
		BotInventoryCustomMountPosition component = tr.GetComponent<BotInventoryCustomMountPosition>();
		if (component != null && component.RegisterAsLargeWhenGrabbedNormally)
		{
			flag = true;
		}
		BotInventoryEntry botInventoryEntry = new BotInventoryEntry(tr, flag);
		if (botInventoryEntry.Rigidbody != null)
		{
			botInventoryEntry.Rigidbody.isKinematic = true;
		}
		tr.parent = ((!flag) ? regularItemInventoryLocation.transform : largeItemInventoryLocation.transform);
		currentInventory.Add(botInventoryEntry);
		if (botInventoryEntry.GrabbableItem != null)
		{
			if (botInventoryEntry.GrabbableItem.IsCurrInHand)
			{
				botInventoryEntry.GrabbableItem.CurrInteractableHand.TryRelease(false);
			}
			botInventoryEntry.GrabbableItem.enabled = false;
		}
		StartCoroutine(InternalAddIndividualItemToInventory(botInventoryEntry, flag));
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		for (int i = 0; i < currentInventory.Count; i++)
		{
			if (currentInventory[i] != null && currentInventory[i].Transform != null)
			{
				Gizmos.DrawWireCube(currentInventory[i].Transform.position + currentInventory[i].DistFromBoundsCenter, currentInventory[i].ColliderBounds.size);
			}
		}
	}

	private void TweenInventoryIntoCorrectPositions()
	{
		float num = 0f;
		for (int i = 0; i < currentInventory.Count; i++)
		{
			BotInventoryEntry botInventoryEntry = currentInventory[i];
			if (botInventoryEntry == null)
			{
				continue;
			}
			float num2 = Mathf.Max(botInventoryEntry.ColliderBounds.size.y + 0.02f, 0.05f);
			num += num2 / 2f;
			Vector3 endValue = Vector3.up * num - botInventoryEntry.DistFromBoundsCenter;
			Quaternion endValue2 = Quaternion.identity;
			if (botInventoryEntry.MountPosition != null)
			{
				if (botInventoryEntry.IsLarge)
				{
					endValue = botInventoryEntry.MountPosition.LocalPosWhenHeldAsLargeItem;
					endValue2 = Quaternion.Euler(botInventoryEntry.MountPosition.LocalRotWhenHeldAsLargeItem);
				}
				else
				{
					endValue2 = Quaternion.Euler(botInventoryEntry.MountPosition.LocalRotWhenHeldAsSmallItem);
				}
			}
			else if (botInventoryEntry.IsLarge)
			{
				endValue = Vector3.zero;
			}
			Go.killAllTweensWithTarget(botInventoryEntry.Transform);
			Go.to(botInventoryEntry.Transform, 1f, new GoTweenConfig().localPosition(endValue).localRotation(endValue2).setEaseType(GoEaseType.QuadInOut));
			num += num2 / 2f;
		}
	}

	private IEnumerator InternalAddIndividualItemToInventory(BotInventoryEntry entry, bool isLarge = false)
	{
		TweenInventoryIntoCorrectPositions();
		yield return new WaitForSeconds(1f);
		if (entry.GrabbableItem != null)
		{
			entry.GrabbableItem.enabled = true;
		}
		if (currentInventory.Contains(entry) && entry.GrabbableItem != null)
		{
			GrabbableItem grabbableItem = entry.GrabbableItem;
			grabbableItem.OnGrabbed = (Action<GrabbableItem>)Delegate.Combine(grabbableItem.OnGrabbed, new Action<GrabbableItem>(ItemWasGrabbedOutOfInventory));
		}
	}

	public void ManuallyAddRecentlyEjectedItem(Rigidbody rb)
	{
		if (!recentlyEjectedItems.Contains(rb) && rb != null)
		{
			recentlyEjectedItems.Add(rb);
			StartCoroutine(WaitAndRemoveRecentlyEjectedStatus(rb));
		}
	}

	private void ItemWasGrabbedOutOfInventory(GrabbableItem item)
	{
		Debug.Log("item grabbed out of inventory: " + item.name);
		item.OnGrabbed = (Action<GrabbableItem>)Delegate.Remove(item.OnGrabbed, new Action<GrabbableItem>(ItemWasGrabbedOutOfInventory));
		Rigidbody rigidbody = item.Rigidbody;
		if (!recentlyEjectedItems.Contains(rigidbody) && rigidbody != null)
		{
			recentlyEjectedItems.Add(rigidbody);
			StartCoroutine(WaitAndRemoveRecentlyEjectedStatus(rigidbody));
		}
		WorldItem component = item.GetComponent<WorldItem>();
		if (component != null)
		{
			GameEventsManager.Instance.ItemAppliedToItemActionOccurred(component.Data, myWorldItem.Data, "REMOVED_FROM");
			GameEventsManager.Instance.ItemActionOccurred(myWorldItem.Data, "MISC_ITEM_REMOVED");
		}
		BotInventoryEntry inventoryEntryByTransform = GetInventoryEntryByTransform(item.transform);
		if (inventoryEntryByTransform != null)
		{
			inventoryEntryByTransform.Transform.SetParent(GlobalStorage.Instance.ContentRoot, true);
			currentInventory.Remove(inventoryEntryByTransform);
		}
		TweenInventoryIntoCorrectPositions();
	}

	public void RemoveAllItemsFromInventory(EmptyTypes emptyType, Transform removeToLocation, WorldItemData widFilter)
	{
		if (emptyType == EmptyTypes.DestroyContents && widFilter == null)
		{
			Cleanup(false);
		}
		else
		{
			StartCoroutine(InternalRemoveAllItemsFromInventory(emptyType, removeToLocation, widFilter));
		}
	}

	public void RemoveMostRecentItemFromInventory(EmptyTypes emptyType, Transform removeToLocation, WorldItemData widFilter)
	{
		if (currentInventory.Count <= 0)
		{
			return;
		}
		BotInventoryEntry botInventoryEntry = currentInventory[currentInventory.Count - 1];
		if (widFilter != null)
		{
			for (int num = currentInventory.Count - 1; num >= 0; num--)
			{
				if (currentInventory[num] != null && currentInventory[num].GrabbableItem != null && currentInventory[num].GrabbableItem.InteractableItem.WorldItemData == widFilter)
				{
					botInventoryEntry = currentInventory[num];
					break;
				}
			}
		}
		switch (emptyType)
		{
		case EmptyTypes.PhysicsPush:
			StartCoroutine(InternalEjectAnyTransform(botInventoryEntry.Transform, EjectTypes.PhysicsPush, null, botInventoryEntry.OriginalRBKinematicSetting));
			break;
		case EmptyTypes.TweenToLocation:
			StartCoroutine(InternalEjectAnyTransform(botInventoryEntry.Transform, EjectTypes.TweenToLocation, removeToLocation, botInventoryEntry.OriginalRBKinematicSetting));
			break;
		default:
			Debug.LogError("EmptyType " + emptyType.ToString() + " not yet supported for this action.");
			break;
		}
		currentInventory.Remove(botInventoryEntry);
	}

	private IEnumerator InternalRemoveAllItemsFromInventory(EmptyTypes emptyType, Transform removeToLocation, WorldItemData widFilter)
	{
		BotInventoryEntry[] inv = currentInventory.ToArray();
		for (int i = 0; i < inv.Length; i++)
		{
			bool doThisItem = false;
			if (widFilter == null)
			{
				doThisItem = true;
			}
			else if (inv[i].GrabbableItem != null && inv[i].GrabbableItem.InteractableItem.WorldItemData == widFilter)
			{
				doThisItem = true;
			}
			if (doThisItem)
			{
				currentInventory.Remove(inv[i]);
				TweenInventoryIntoCorrectPositions();
				switch (emptyType)
				{
				case EmptyTypes.PhysicsPush:
					yield return StartCoroutine(InternalEjectAnyTransform(inv[i].Transform, EjectTypes.PhysicsPush, null, inv[i].OriginalRBKinematicSetting));
					break;
				case EmptyTypes.TweenToLocation:
					yield return StartCoroutine(InternalEjectAnyTransform(inv[i].Transform, EjectTypes.TweenToLocation, removeToLocation, inv[i].OriginalRBKinematicSetting));
					break;
				default:
					Debug.LogError("EmptyType " + emptyType.ToString() + " not yet supported for this action.");
					break;
				}
			}
		}
		if (widFilter == null)
		{
			currentInventory.Clear();
		}
	}

	public void EjectPrefab(GameObject prefab, EjectTypes ejectType, Transform ejectToLocation = null, float forceMultiplier = 1f)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(prefab, ejectLocation.position, ejectLocation.rotation) as GameObject;
		BasePrefabSpawner component = gameObject.GetComponent<BasePrefabSpawner>();
		if (component != null)
		{
			gameObject = component.LastSpawnedPrefabGO;
		}
		gameObject.transform.SetParent(GlobalStorage.Instance.ContentRoot, true);
		EjectExistingNonInventoryObject(gameObject.transform, ejectType, ejectToLocation, forceMultiplier);
	}

	public GameObject EjectPrefabAndReturnInstance(GameObject prefab, EjectTypes ejectType, Transform ejectToLocation = null, float forceMultiplier = 1f)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(prefab, ejectLocation.position, ejectLocation.rotation) as GameObject;
		BasePrefabSpawner component = gameObject.GetComponent<BasePrefabSpawner>();
		if (component != null)
		{
			gameObject = component.LastSpawnedPrefabGO;
		}
		gameObject.transform.SetParent(GlobalStorage.Instance.ContentRoot, true);
		EjectExistingNonInventoryObject(gameObject.transform, ejectType, ejectToLocation, forceMultiplier);
		return gameObject;
	}

	public void EjectExistingNonInventoryObject(Transform obj, EjectTypes ejectType, Transform ejectToLocation = null, float forceMultiplier = 1f)
	{
		obj.position = ejectLocation.position;
		obj.rotation = ejectLocation.rotation;
		Rigidbody component = obj.GetComponent<Rigidbody>();
		bool kinematicWhenFinished = false;
		if (component != null)
		{
			kinematicWhenFinished = component.isKinematic;
		}
		StartCoroutine(InternalEjectAnyTransform(obj, ejectType, ejectToLocation, kinematicWhenFinished, forceMultiplier));
	}

	private IEnumerator WaitAndRemoveRecentlyEjectedStatus(Rigidbody rb)
	{
		yield return new WaitForSeconds(1f);
		if (recentlyEjectedItems.Contains(rb))
		{
			recentlyEjectedItems.Remove(rb);
		}
		for (int i = 0; i < recentlyEjectedItems.Count; i++)
		{
			if (recentlyEjectedItems[i] == null)
			{
				recentlyEjectedItems.RemoveAt(i);
				i--;
			}
		}
	}

	private IEnumerator InternalEjectAnyTransform(Transform obj, EjectTypes ejectType, Transform ejectToLocation = null, bool kinematicWhenFinished = false, float forceMultiplier = 1f)
	{
		Rigidbody rb = obj.GetComponent<Rigidbody>();
		if (!recentlyEjectedItems.Contains(rb) && rb != null)
		{
			recentlyEjectedItems.Add(rb);
			StartCoroutine(WaitAndRemoveRecentlyEjectedStatus(rb));
		}
		obj.SetParent(GlobalStorage.Instance.ContentRoot, true);
		GrabbableItem gi = obj.GetComponent<GrabbableItem>();
		if (gi != null)
		{
			WorldItemData wid = gi.InteractableItem.WorldItemData;
			if (wid != null)
			{
				GameEventsManager.Instance.ItemAppliedToItemActionOccurred(wid, myWorldItem.Data, "REMOVED_FROM");
				GameEventsManager.Instance.ItemActionOccurred(wid, "MISC_ITEM_REMOVED");
			}
			gi.OnGrabbed = (Action<GrabbableItem>)Delegate.Remove(gi.OnGrabbed, new Action<GrabbableItem>(ItemWasGrabbedOutOfInventory));
			if (gi.IsCurrInHand)
			{
				gi.CurrInteractableHand.TryRelease(false);
			}
		}
		switch (ejectType)
		{
		case EjectTypes.TweenToLocation:
			if (ejectToLocation != null)
			{
				if (ejectTweenToLocationSound != null)
				{
					AudioManager.Instance.Play(ejectLocation, ejectTweenToLocationSound, 1f, 1f);
				}
				if (gi != null)
				{
					gi.enabled = false;
				}
				float tweenTime = Vector3.Distance(obj.position, ejectToLocation.position);
				Go.to(obj, tweenTime, new GoTweenConfig().position(ejectToLocation.position).rotation(ejectToLocation.rotation).setEaseType(GoEaseType.QuadInOut));
				yield return new WaitForSeconds(tweenTime + 0.01f);
				if (rb != null)
				{
					rb.isKinematic = kinematicWhenFinished;
					rb.velocity = Vector3.zero;
					rb.angularVelocity = Vector3.zero;
				}
				if (gi != null)
				{
					gi.enabled = true;
				}
			}
			else
			{
				Debug.LogWarning("Tried to eject item to location but location was null");
			}
			break;
		case EjectTypes.HoverToLocation:
			if (ejectToLocation != null)
			{
				if (ejectHoverToLocationSound != null)
				{
					AudioManager.Instance.Play(ejectLocation, ejectHoverToLocationSound, 1f, 1f);
				}
				if (rb != null)
				{
					rb.isKinematic = true;
				}
				BotInventoryFlyoutItem flyout = UnityEngine.Object.Instantiate(flyoutItemPrefab);
				flyout.transform.SetParent(GlobalStorage.Instance.ContentRoot, false);
				flyout.transform.position = ejectLocation.position;
				flyout.transform.rotation = ejectLocation.rotation;
				flyout.DoFlyout(gi, ejectToLocation);
				flyout.OnItemWasPickedUp = (Action<BotInventoryFlyoutItem>)Delegate.Combine(flyout.OnItemWasPickedUp, new Action<BotInventoryFlyoutItem>(FlyoutItemWasGrabbed));
				flyoutItems.Add(flyout);
			}
			else
			{
				Debug.LogWarning("Tried to hover item to location but location was null");
			}
			break;
		case EjectTypes.PhysicsPush:
			if (ejectPhysicsPushSound != null)
			{
				AudioManager.Instance.Play(ejectLocation, ejectPhysicsPushSound, 1f, 1f);
			}
			yield return new WaitForSeconds(0.1f);
			obj.position = ejectLocation.position;
			obj.rotation = ejectLocation.rotation;
			if (rb != null)
			{
				rb.isKinematic = false;
				rb.AddForce(ejectLocation.transform.forward * 200f * forceMultiplier);
			}
			else
			{
				Debug.LogError("Tried to eject item " + obj.name + " from inventory but it has no rigidbody");
			}
			yield return new WaitForSeconds(0.6f);
			break;
		case EjectTypes.HoldOutToPlayer:
			if (ejectHoldOutToPlayerSound != null)
			{
				AudioManager.Instance.Play(ejectLocation, ejectHoldOutToPlayerSound, 1f, 1f);
			}
			if (!itemsBeingHeldOut.Contains(obj))
			{
				itemsBeingHeldOut.Add(obj);
			}
			obj.transform.SetParent(handOverLocation, true);
			handOverEffect.gameObject.SetActive(true);
			handOverEffect.SetParent(obj.transform);
			handOverEffect.localPosition = Vector3.zero;
			handOverEffect.localRotation = Quaternion.identity;
			if (rb != null)
			{
				rb.isKinematic = true;
			}
			if (gi != null)
			{
				gi.enabled = false;
			}
			Go.to(obj, 1f, new GoTweenConfig().position(handOverLocation.position).rotation(handOverLocation.rotation).setEaseType(GoEaseType.QuadInOut));
			yield return new WaitForSeconds(1f);
			if (gi != null)
			{
				gi.enabled = true;
				gi.OnGrabbed = (Action<GrabbableItem>)Delegate.Combine(gi.OnGrabbed, new Action<GrabbableItem>(HeldOutItemWasGrabbed));
			}
			break;
		}
		if (ejectType != EjectTypes.CreateNearMe)
		{
			UniqueObject uo = obj.GetComponent<UniqueObject>();
			if (uo != null)
			{
				uo.WasEjectedFromInventoryOfBot();
			}
		}
	}

	private void FlyoutItemWasGrabbed(BotInventoryFlyoutItem flyoutItem)
	{
		flyoutItem.OnItemWasPickedUp = (Action<BotInventoryFlyoutItem>)Delegate.Remove(flyoutItem.OnItemWasPickedUp, new Action<BotInventoryFlyoutItem>(FlyoutItemWasGrabbed));
		if (flyoutItems.Contains(flyoutItem))
		{
			flyoutItems.Remove(flyoutItem);
		}
		WorldItem component = flyoutItem.Grabbable.gameObject.GetComponent<WorldItem>();
		if (component != null)
		{
			GameEventsManager.Instance.ItemAppliedToItemActionOccurred(component.Data, myWorldItem.Data, "REMOVED_FROM");
		}
	}

	private void HeldOutItemWasGrabbed(GrabbableItem grabbableItem)
	{
		if (itemsBeingHeldOut.Contains(grabbableItem.transform))
		{
			itemsBeingHeldOut.Remove(grabbableItem.transform);
		}
		Rigidbody rigidbody = grabbableItem.Rigidbody;
		if (!recentlyEjectedItems.Contains(rigidbody) && rigidbody != null)
		{
			recentlyEjectedItems.Add(rigidbody);
			StartCoroutine(WaitAndRemoveRecentlyEjectedStatus(rigidbody));
		}
		handOverEffect.SetParent(handOverLocation, false);
		handOverEffect.gameObject.SetActive(false);
		grabbableItem.OnGrabbed = (Action<GrabbableItem>)Delegate.Remove(grabbableItem.OnGrabbed, new Action<GrabbableItem>(HeldOutItemWasGrabbed));
		WorldItem component = grabbableItem.gameObject.GetComponent<WorldItem>();
		if (component != null)
		{
			GameEventsManager.Instance.ItemAppliedToItemActionOccurred(component.Data, myWorldItem.Data, "REMOVED_FROM");
		}
		grabbableItem.transform.SetParent(GlobalStorage.Instance.ContentRoot, true);
	}

	public void ClearHeldOutItems()
	{
		handOverEffect.SetParent(handOverLocation, false);
		handOverEffect.gameObject.SetActive(false);
		for (int i = 0; i < itemsBeingHeldOut.Count; i++)
		{
			GrabbableItem component = itemsBeingHeldOut[i].gameObject.GetComponent<GrabbableItem>();
			if (component != null)
			{
				component.OnGrabbed = (Action<GrabbableItem>)Delegate.Remove(component.OnGrabbed, new Action<GrabbableItem>(HeldOutItemWasGrabbed));
			}
			UnityEngine.Object.Destroy(component.gameObject);
		}
		itemsBeingHeldOut.Clear();
		for (int j = 0; j < flyoutItems.Count; j++)
		{
			flyoutItems[j].Cancel();
			BotInventoryFlyoutItem botInventoryFlyoutItem = flyoutItems[j];
			botInventoryFlyoutItem.OnItemWasPickedUp = (Action<BotInventoryFlyoutItem>)Delegate.Remove(botInventoryFlyoutItem.OnItemWasPickedUp, new Action<BotInventoryFlyoutItem>(FlyoutItemWasGrabbed));
			UnityEngine.Object.Destroy(flyoutItems[j].gameObject);
		}
		flyoutItems.Clear();
	}

	private BotInventoryEntry GetInventoryEntryByTransform(Transform tr)
	{
		for (int i = 0; i < currentInventory.Count; i++)
		{
			if (currentInventory[i].Transform == tr)
			{
				return currentInventory[i];
			}
		}
		return null;
	}

	public void Cleanup(bool clearItemsOfInterest = true)
	{
		handOverEffect.SetParent(handOverLocation, false);
		handOverEffect.gameObject.SetActive(false);
		for (int i = 0; i < currentInventory.Count; i++)
		{
			if (currentInventory[i] != null && currentInventory[i].Transform != null)
			{
				UnityEngine.Object.Destroy(currentInventory[i].Transform.gameObject);
			}
		}
		for (int j = 0; j < itemsBeingHeldOut.Count; j++)
		{
			if (itemsBeingHeldOut[j] != null)
			{
				UnityEngine.Object.Destroy(itemsBeingHeldOut[j].gameObject);
			}
		}
		for (int k = 0; k < flyoutItems.Count; k++)
		{
			if (flyoutItems[k] != null)
			{
				BotInventoryFlyoutItem botInventoryFlyoutItem = flyoutItems[k];
				botInventoryFlyoutItem.OnItemWasPickedUp = (Action<BotInventoryFlyoutItem>)Delegate.Remove(botInventoryFlyoutItem.OnItemWasPickedUp, new Action<BotInventoryFlyoutItem>(FlyoutItemWasGrabbed));
			}
		}
		currentInventory.Clear();
		flyoutItems.Clear();
		if (clearItemsOfInterest)
		{
			itemsOfInterest.Clear();
		}
	}
}
