using System;
using System.Collections;
using System.Collections.Generic;
using OwlchemyVR;
using UnityEngine;

public class TrinketMountController : MonoBehaviour
{
	private const string EXIT_BURRITO_WORLDITEM_NAME = "MenuExitFood";

	private const string COMPANION_CAM_WORLDITEM_NAME = "CompanionSecurityCam";

	private static List<TrinketMountController> allMountControllers = new List<TrinketMountController>();

	[SerializeField]
	private RigidbodyEnterExitTriggerEvents triggerEvent;

	private List<PickupableItem> mountedItems = new List<PickupableItem>();

	private List<PickupableItem> itemsInRange = new List<PickupableItem>();

	[SerializeField]
	private Rigidbody hangingPivot;

	[SerializeField]
	private Material lineMaterial;

	[SerializeField]
	private GameObject[] initialAttachedItems;

	[SerializeField]
	private GameObject[] EndlessAttachedItems;

	[SerializeField]
	private bool respawnInitialItem;

	[SerializeField]
	private float respawnDelay = 2f;

	[SerializeField]
	private int maxCapacity = 4;

	private List<LineRenderer> lineRenderers = new List<LineRenderer>();

	private LineRenderer temporaryLine;

	public Transform temporaryLineTransform;

	[SerializeField]
	private Material tempLineMaterial;

	[SerializeField]
	private WorldItem worldItem;

	private GameObject lastSpawnedObject;

	private bool needRespawn;

	private bool waitingOnRespawn;

	public WorldItem WorldItem
	{
		get
		{
			return worldItem;
		}
	}

	private void OnEnable()
	{
		if (!allMountControllers.Contains(this))
		{
			allMountControllers.Add(this);
		}
		RigidbodyEnterExitTriggerEvents rigidbodyEnterExitTriggerEvents = triggerEvent;
		rigidbodyEnterExitTriggerEvents.OnRigidbodyEnterTrigger = (Action<Rigidbody>)Delegate.Combine(rigidbodyEnterExitTriggerEvents.OnRigidbodyEnterTrigger, new Action<Rigidbody>(ObjectEnteredRange));
		RigidbodyEnterExitTriggerEvents rigidbodyEnterExitTriggerEvents2 = triggerEvent;
		rigidbodyEnterExitTriggerEvents2.OnRigidbodyExitTrigger = (Action<Rigidbody>)Delegate.Combine(rigidbodyEnterExitTriggerEvents2.OnRigidbodyExitTrigger, new Action<Rigidbody>(ObjectExitedRange));
	}

	private void OnDisable()
	{
		if (allMountControllers.Contains(this))
		{
			allMountControllers.Remove(this);
		}
		RigidbodyEnterExitTriggerEvents rigidbodyEnterExitTriggerEvents = triggerEvent;
		rigidbodyEnterExitTriggerEvents.OnRigidbodyEnterTrigger = (Action<Rigidbody>)Delegate.Remove(rigidbodyEnterExitTriggerEvents.OnRigidbodyEnterTrigger, new Action<Rigidbody>(ObjectEnteredRange));
		RigidbodyEnterExitTriggerEvents rigidbodyEnterExitTriggerEvents2 = triggerEvent;
		rigidbodyEnterExitTriggerEvents2.OnRigidbodyExitTrigger = (Action<Rigidbody>)Delegate.Remove(rigidbodyEnterExitTriggerEvents2.OnRigidbodyExitTrigger, new Action<Rigidbody>(ObjectExitedRange));
	}

	private void Awake()
	{
		GameObject original = Resources.Load<GameObject>("TrinketLine");
		for (int i = 0; i < maxCapacity; i++)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(original, base.transform.position, base.transform.rotation) as GameObject;
			gameObject.transform.SetParent(base.transform, true);
			lineRenderers.Add(gameObject.GetComponent<LineRenderer>());
		}
		temporaryLine = base.gameObject.GetComponent<LineRenderer>();
		if (temporaryLine == null)
		{
			temporaryLine = base.gameObject.AddComponent<LineRenderer>();
		}
		temporaryLine.SetWidth(0.01f, 0.01f);
		temporaryLine.SetVertexCount(2);
		temporaryLine.material = tempLineMaterial;
		if (GenieManager.DoesContainGenieMode(GlobalStorage.Instance.CurrentGenieModes, JobGenieCartridge.GenieModeTypes.EndlessMode) && (float)EndlessAttachedItems.Length > 0f)
		{
			SpawnTrinketItems(EndlessAttachedItems);
		}
		else
		{
			SpawnTrinketItems(initialAttachedItems);
		}
	}

	public void SpawnTrinketItems(GameObject[] items, bool startKinematic = false)
	{
		for (int i = 0; i < items.Length; i++)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(items[i], hangingPivot.transform.position, hangingPivot.transform.rotation) as GameObject;
			BasePrefabSpawner component = gameObject.GetComponent<BasePrefabSpawner>();
			if (component != null)
			{
				gameObject = component.LastSpawnedPrefabGO;
			}
			if (startKinematic)
			{
				Rigidbody component2 = gameObject.GetComponent<Rigidbody>();
				component2.isKinematic = true;
			}
			GrabbableItem component3 = gameObject.GetComponent<GrabbableItem>();
			if (component3 != null)
			{
				MountItem(component3);
			}
			else
			{
				Debug.LogError("Can't mount " + gameObject.name + " on a trinket because it has no grabbableItem.", gameObject);
			}
			if (i == 0)
			{
				lastSpawnedObject = gameObject;
			}
		}
	}

	private void Update()
	{
		needRespawn = true;
		for (int i = 0; i < mountedItems.Count; i++)
		{
			if (respawnInitialItem && mountedItems[i].gameObject == lastSpawnedObject && waitingOnRespawn)
			{
				needRespawn = false;
				waitingOnRespawn = false;
				StopAllCoroutines();
			}
			if (mountedItems[i] != null)
			{
				if (i < lineRenderers.Count && i >= 0)
				{
					lineRenderers[i].enabled = true;
					lineRenderers[i].SetPosition(0, hangingPivot.position);
					lineRenderers[i].SetPosition(1, mountedItems[i].transform.position);
				}
			}
			else
			{
				Debug.LogWarning("MountedItem " + i + " on " + base.gameObject.name + " disappeared unexpectedly.", base.gameObject);
				mountedItems.RemoveAt(i);
				i--;
			}
		}
		if (respawnInitialItem && needRespawn && !waitingOnRespawn)
		{
			StartCoroutine(RespawnObject());
		}
		for (int j = 0; j < lineRenderers.Count; j++)
		{
			if (j >= mountedItems.Count)
			{
				lineRenderers[j].enabled = false;
			}
		}
		for (int k = 0; k < allMountControllers.Count; k++)
		{
			if (allMountControllers[k] != null)
			{
				TrinketMountController trinketMountController = allMountControllers[k];
				if (temporaryLineTransform == null)
				{
					temporaryLine.enabled = false;
					return;
				}
				if (trinketMountController != this && trinketMountController.temporaryLineTransform == temporaryLineTransform)
				{
					temporaryLine.enabled = false;
					return;
				}
			}
		}
		temporaryLine.enabled = true;
		temporaryLine.SetPosition(0, hangingPivot.position);
		temporaryLine.SetPosition(1, temporaryLineTransform.position);
	}

	private IEnumerator RespawnObject()
	{
		waitingOnRespawn = true;
		yield return new WaitForSeconds(respawnDelay);
		if (GenieManager.DoesContainGenieMode(GlobalStorage.Instance.CurrentGenieModes, JobGenieCartridge.GenieModeTypes.EndlessMode) && (float)EndlessAttachedItems.Length > 0f)
		{
			GameObject go = UnityEngine.Object.Instantiate(EndlessAttachedItems[0], base.transform.position, base.transform.rotation) as GameObject;
			BasePrefabSpawner b = go.GetComponent<BasePrefabSpawner>();
			if (b != null)
			{
				go = b.LastSpawnedPrefabGO;
			}
			GrabbableItem grab = go.GetComponent<GrabbableItem>();
			if (grab != null)
			{
				MountItem(grab);
			}
			lastSpawnedObject = go;
			waitingOnRespawn = false;
		}
		else
		{
			GameObject go2 = UnityEngine.Object.Instantiate(initialAttachedItems[0], base.transform.position, base.transform.rotation) as GameObject;
			BasePrefabSpawner b2 = go2.GetComponent<BasePrefabSpawner>();
			if (b2 != null)
			{
				go2 = b2.LastSpawnedPrefabGO;
			}
			GrabbableItem grab2 = go2.GetComponent<GrabbableItem>();
			if (grab2 != null)
			{
				MountItem(grab2);
			}
			lastSpawnedObject = go2;
			waitingOnRespawn = false;
		}
	}

	private void ObjectEnteredRange(Rigidbody rb)
	{
		PickupableItem component = rb.GetComponent<PickupableItem>();
		if (component == null || !component.IsCurrInHand || itemsInRange.Contains(component) || temporaryLineTransform != null || mountedItems.Count >= maxCapacity || (component.InteractableItem.WorldItem != null && (component.InteractableItem.WorldItemData.ItemName == "MenuExitFood" || component.InteractableItem.WorldItemData.ItemName == "CompanionSecurityCam")))
		{
			return;
		}
		SelectedChangeOutlineController componentInChildren = rb.GetComponentInChildren<SelectedChangeOutlineController>();
		if (componentInChildren != null)
		{
			if (mountedItems.Contains(component))
			{
				componentInChildren.SetSpecialHighlight(false);
				return;
			}
			componentInChildren.SetSpecialHighlight(true);
		}
		if (!itemsInRange.Contains(component))
		{
			itemsInRange.Add(component);
			if (!mountedItems.Contains(component))
			{
				temporaryLineTransform = component.transform;
				component.OnReleasedWasNotSwappedBetweenHands = (Action<GrabbableItem>)Delegate.Combine(component.OnReleasedWasNotSwappedBetweenHands, new Action<GrabbableItem>(MountItem));
			}
		}
	}

	private void ObjectExitedRange(Rigidbody rb)
	{
		PickupableItem component = rb.GetComponent<PickupableItem>();
		if (!(component == null) && (!(component.InteractableItem.WorldItem != null) || (!(component.InteractableItem.WorldItemData.ItemName == "MenuExitFood") && !(component.InteractableItem.WorldItemData.ItemName == "CompanionSecurityCam"))))
		{
			SelectedChangeOutlineController componentInChildren = rb.GetComponentInChildren<SelectedChangeOutlineController>();
			if (componentInChildren != null)
			{
				componentInChildren.SetSpecialHighlight(false);
			}
			if (itemsInRange.Contains(component))
			{
				itemsInRange.Remove(component);
			}
			if (!mountedItems.Contains(component))
			{
				component.OnReleasedWasNotSwappedBetweenHands = (Action<GrabbableItem>)Delegate.Remove(component.OnReleasedWasNotSwappedBetweenHands, new Action<GrabbableItem>(MountItem));
				temporaryLineTransform = null;
			}
		}
	}

	private void MountItem(GrabbableItem grabbableItem)
	{
		if (grabbableItem.GetComponent<TrinketSpringController>() != null)
		{
			Debug.Log("Tried to attach object to trinketMount, but it was already mounted to something else.", grabbableItem.gameObject);
			return;
		}
		PickupableItem component = grabbableItem.GetComponent<PickupableItem>();
		if (component != null && component.InteractableItem.WorldItem != null && (component.InteractableItem.WorldItemData.ItemName == "MenuExitFood" || component.InteractableItem.WorldItemData.ItemName == "CompanionSecurityCam"))
		{
			return;
		}
		Rigidbody component2 = component.GetComponent<Rigidbody>();
		SelectedChangeOutlineController componentInChildren = component2.GetComponentInChildren<SelectedChangeOutlineController>();
		if (componentInChildren != null)
		{
			componentInChildren.SetSpecialHighlight(false);
		}
		mountedItems.Add(component);
		if (mountedItems.Count > lineRenderers.Count)
		{
			Debug.LogError("More items were mounted to " + base.gameObject.name + " (" + mountedItems.Count + ") than there are lineRenderers: " + lineRenderers.Count + "), max quantity: " + maxCapacity);
			for (int i = maxCapacity; i < mountedItems.Count; i++)
			{
				DismountItem(mountedItems[i]);
			}
		}
		else
		{
			component.OnReleasedWasNotSwappedBetweenHands = (Action<GrabbableItem>)Delegate.Remove(component.OnReleasedWasNotSwappedBetweenHands, new Action<GrabbableItem>(MountItem));
			component.OnGrabbed = (Action<GrabbableItem>)Delegate.Combine(component.OnGrabbed, new Action<GrabbableItem>(DismountItem));
			TrinketSpringController trinketSpringController = component2.gameObject.AddComponent<TrinketSpringController>();
			trinketSpringController.springJoint.connectedBody = hangingPivot;
			temporaryLineTransform = null;
			if (worldItem != null)
			{
				GameEventsManager.Instance.ItemAppliedToItemActionOccurred(grabbableItem.GetComponent<WorldItem>().Data, worldItem.Data, "ATTACHED_TO");
				GameEventsManager.Instance.ItemActionOccurred(worldItem.Data, "ACTIVATED");
			}
			grabbableItem.transform.parent = base.transform;
		}
	}

	private void DismountItem(GrabbableItem g_item)
	{
		PickupableItem component = g_item.GetComponent<PickupableItem>();
		Rigidbody component2 = component.GetComponent<Rigidbody>();
		if (!mountedItems.Contains(component))
		{
			Debug.LogWarning("Tried to dismount a trinket item that was never mounted: " + component.name, component.gameObject);
			return;
		}
		mountedItems.Remove(component);
		component.OnGrabbed = (Action<GrabbableItem>)Delegate.Remove(component.OnGrabbed, new Action<GrabbableItem>(DismountItem));
		UnityEngine.Object.Destroy(component2.GetComponent<TrinketSpringController>());
		if (itemsInRange.Contains(component))
		{
			component.OnReleasedWasNotSwappedBetweenHands = (Action<GrabbableItem>)Delegate.Remove(component.OnReleasedWasNotSwappedBetweenHands, new Action<GrabbableItem>(MountItem));
			itemsInRange.Remove(component);
			ObjectEnteredRange(component2);
		}
		g_item.transform.parent = GlobalStorage.Instance.ContentRoot;
		temporaryLineTransform = null;
		if (worldItem != null)
		{
			GameEventsManager.Instance.ItemAppliedToItemActionOccurred(g_item.GetComponent<WorldItem>().Data, worldItem.Data, "DEATTACHED_FROM");
			GameEventsManager.Instance.ItemActionOccurred(worldItem.Data, "DEACTIVATED");
		}
	}

	public void ClearAndDeleteAllItems(VehicleChassisController chassis)
	{
		while (mountedItems.Count > 0)
		{
			PickupableItem pickupableItem = mountedItems[0];
			DismountItem(pickupableItem);
			chassis.ItemRelativeTransforms.Remove(new VehicleItemRelativeTransformInfo(pickupableItem.transform, chassis.transform));
			UnityEngine.Object.DestroyImmediate(pickupableItem.gameObject);
		}
	}
}
