using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using OwlchemyVR;
using UnityEngine;

public class VehicleChassisController : MonoBehaviour
{
	public enum ChassisHardwareMountPoint
	{
		Root = 0,
		Interior = 1,
		LicensePlate = 2,
		EachWheel = 3,
		EachHeadlight = 4,
		Engine = 5,
		Hood = 6,
		GasTank = 7,
		Trunk = 8,
		HoodOrnament = 9
	}

	public enum Gear
	{
		Drive = 0,
		Reverse = 1
	}

	private int frameOfLastLockedUpdate = -1;

	[HideInInspector]
	public Color underLightColor = Color.black;

	[SerializeField]
	private MeshRenderer underLightMesh;

	[SerializeField]
	private UniqueObject driverSeatUniqueObject;

	[SerializeField]
	private WorldItem vehicleWorldItem;

	[SerializeField]
	private Transform interiorLocation;

	[SerializeField]
	private Transform engineLocation;

	[SerializeField]
	private Transform hoodLocation;

	[SerializeField]
	private Transform trunkLocation;

	[SerializeField]
	private Transform gasTankLocation;

	private Transform engineTransform;

	private bool engineIsShown = true;

	private VehicleEngineHardware _engine;

	private CarInteriorDashboardHardware _dashboard;

	[SerializeField]
	private VehicleDoorDefinition[] doorDefinitions;

	[SerializeField]
	private Transform driversSeatParent;

	[SerializeField]
	private Transform[] wheelAttachmentLocationsOnLeft;

	[SerializeField]
	private Transform[] wheelAttachmentLocationsOnRight;

	[SerializeField]
	private Transform[] headlightAttachmentLocations;

	private VehicleAttachableHardware[] headlightAttachmentLocationsVAH = new VehicleAttachableHardware[2];

	[SerializeField]
	private Transform licensePlateAttachPoint;

	[SerializeField]
	private Transform[] hoodOrnamentAttachmentLocations;

	[SerializeField]
	private Transform interiorTrinketMountLocation;

	private List<AttachablePoint> headlightAttachPoints;

	private List<AttachablePoint> hoodAttachPoints;

	private TrinketMountController interiorTrinketController;

	private VehicleAttachableHardware licensePlateAttachPointVehicleAttachableHardware;

	[SerializeField]
	private Transform leftBound;

	[SerializeField]
	private Transform rightBound;

	[SerializeField]
	private Transform frontBound;

	[SerializeField]
	private Transform backBound;

	[SerializeField]
	private ItemCollectionZone interiorCollectionZone;

	[SerializeField]
	private int psvrMaxItemsAllowedInInterior = 6;

	[SerializeField]
	private ParticleSystem psvrDestroyInteriorItemEffect;

	[SerializeField]
	private AudioClip psvrDestroyInteriorItemSound;

	private List<GameObject> killableInteriorObjectsInOrder = new List<GameObject>();

	[SerializeField]
	private ItemCollectionZone hoodCollectionZone;

	[SerializeField]
	private bool hoodCanBeNukedWhenLeaving = true;

	[SerializeField]
	private ItemCollectionZone trunkCollectionZone;

	[SerializeField]
	private bool trunkCanBeNukedWhenLeaving;

	[SerializeField]
	private int psvrMaxItemsAllowedInTrunk = 6;

	[SerializeField]
	private bool trunkCanBeCompletelyHiddenWhileMoving;

	private bool trunkIsTemporaryLocked;

	private List<PickupableItem> temporarilyHiddenTrunkItems = new List<PickupableItem>();

	private List<VehicleItemRelativeTransformInfo> temporaryRelativeTrunkItemPositions = new List<VehicleItemRelativeTransformInfo>();

	private List<GameObject> killableTrunkObjectsInOrder = new List<GameObject>();

	private ItemCollectionZone[] interiorItemCollectionZones;

	[SerializeField]
	private Transform[] wheelsThatSteer;

	[SerializeField]
	private float maxSteeringAngle = 5f;

	private Quaternion[] steeringWheelsOriginalRotations;

	private float currentSteerAmount;

	[SerializeField]
	private Color headlightStartColor;

	private Gear currentGear;

	private bool doorsBeingForcedClosed;

	private List<WheelAttachmentManager> spawnedWheelsOnLeft = new List<WheelAttachmentManager>();

	private List<WheelAttachmentManager> spawnedWheelsOnRight = new List<WheelAttachmentManager>();

	private bool doorsAreLocked = true;

	private List<VehicleHardware> hardwareOnChassis = new List<VehicleHardware>();

	[SerializeField]
	private List<AttachablePoint> trackedAttachPoints = new List<AttachablePoint>();

	[SerializeField]
	private Transform[] optimizableTransforms;

	[SerializeField]
	private Transform[] preoptimizedTransforms;

	[SerializeField]
	private ParticleSystem[] exhaustPFX;

	[SerializeField]
	private ParticleSystem[] flameExhaustPFX;

	[SerializeField]
	private AudioSourceHelper[] flameExhaustSoundHelpers;

	[SerializeField]
	private AudioClip flameExhaustClip;

	[SerializeField]
	private Transform shifterTransformToAdjust;

	private Transform cachedShifterTransform;

	private List<VehicleReparentableTransform> optimizeTransformsList = new List<VehicleReparentableTransform>();

	private List<VehicleReparentableTransform> preoptimizedTransformsList = new List<VehicleReparentableTransform>();

	private Transform optimizationTemporaryParent;

	private List<PickupableItem> pickupablesLockedLastTime = new List<PickupableItem>();

	private List<VehicleItemRelativeTransformInfo> itemRelativeTransforms = new List<VehicleItemRelativeTransformInfo>();

	private bool itemsAreLockedAndNeedUpdating;

	private GameObject[] pistons;

	private bool isDoingDelayedHideEngine;

	private static int FRAMES_PER_LOCKED_ITEM_UPDATE
	{
		get
		{
			return 1;
		}
	}

	public UniqueObject DriverSeatUniqueObject
	{
		get
		{
			return driverSeatUniqueObject;
		}
	}

	public WorldItem GetVehicleWorldItem
	{
		get
		{
			return vehicleWorldItem;
		}
	}

	public VehicleEngineHardware Engine
	{
		get
		{
			if (_engine == null)
			{
				_engine = engineTransform.GetComponentInChildren<VehicleEngineHardware>();
			}
			return _engine;
		}
	}

	public CarInteriorDashboardHardware Dashboard
	{
		get
		{
			if (_dashboard == null)
			{
				_dashboard = base.gameObject.GetComponentInChildren<CarInteriorDashboardHardware>();
				if (_dashboard != null)
				{
					cachedShifterTransform = _dashboard.GearShifterTransform;
				}
			}
			return _dashboard;
		}
	}

	public GameObject EngineTransform
	{
		get
		{
			if (engineTransform == null)
			{
				return null;
			}
			return engineTransform.gameObject;
		}
	}

	public GasTankController GasTank
	{
		get
		{
			return gasTankLocation.GetComponentInChildren<GasTankController>();
		}
	}

	public List<AttachablePoint> HeadlightAttachPoints
	{
		get
		{
			if (headlightAttachPoints == null)
			{
				headlightAttachPoints = new List<AttachablePoint>();
				for (int i = 0; i < headlightAttachmentLocations.Length; i++)
				{
					headlightAttachPoints.Add(headlightAttachmentLocations[i].GetComponentInChildren<AttachablePoint>());
				}
			}
			return headlightAttachPoints;
		}
	}

	public List<AttachablePoint> HoodAttachPoints
	{
		get
		{
			if (hoodAttachPoints == null)
			{
				hoodAttachPoints = new List<AttachablePoint>();
				for (int i = 0; i < hoodOrnamentAttachmentLocations.Length; i++)
				{
					hoodAttachPoints.Add(hoodOrnamentAttachmentLocations[i].GetComponent<AttachablePoint>());
				}
			}
			return hoodAttachPoints;
		}
	}

	public TrinketMountController InteriorTrinketController
	{
		get
		{
			if (interiorTrinketController == null)
			{
				interiorTrinketController = interiorTrinketMountLocation.GetComponentInChildren<TrinketMountController>();
			}
			return interiorTrinketController;
		}
	}

	public Transform LicensePlateAttachPoint
	{
		get
		{
			return licensePlateAttachPoint;
		}
	}

	public Vector3 LeftBoundPosition
	{
		get
		{
			return leftBound.position;
		}
	}

	public Vector3 RightBoundPosition
	{
		get
		{
			return rightBound.position;
		}
	}

	public Vector3 FrontBoundPosition
	{
		get
		{
			return frontBound.position;
		}
	}

	public Vector3 BackBoundPosition
	{
		get
		{
			return backBound.position;
		}
	}

	public bool TrunkCanBeNukedWhenLeaving
	{
		get
		{
			return trunkCanBeNukedWhenLeaving;
		}
	}

	public bool DoorsAreLocked
	{
		get
		{
			return doorsAreLocked;
		}
	}

	public ParticleSystem[] ExhaustPFX
	{
		get
		{
			return exhaustPFX;
		}
	}

	public ParticleSystem[] FlameExhaustPFX
	{
		get
		{
			return flameExhaustPFX;
		}
	}

	public GameObject[] Pistons
	{
		get
		{
			return pistons;
		}
	}

	public List<VehicleItemRelativeTransformInfo> ItemRelativeTransforms
	{
		get
		{
			return itemRelativeTransforms;
		}
	}

	public Transform DriversSeatParent
	{
		get
		{
			return driversSeatParent;
		}
	}

	public List<WheelAttachmentManager> GetAllWheelAttachmentManagers()
	{
		List<WheelAttachmentManager> list = new List<WheelAttachmentManager>(spawnedWheelsOnLeft);
		list.AddRange(spawnedWheelsOnRight);
		return list;
	}

	private void Awake()
	{
		interiorItemCollectionZones = new ItemCollectionZone[3] { interiorCollectionZone, hoodCollectionZone, trunkCollectionZone };
		interiorItemCollectionZones = interiorItemCollectionZones.Where((ItemCollectionZone item) => item != null).ToArray();
	}

	public void Init()
	{
		GameObject gameObject = new GameObject("OptimizedCarTransformHolder");
		gameObject.transform.SetParent(GlobalStorage.Instance.ContentRoot);
		gameObject.transform.SetToDefaultPosRotScale();
		optimizationTemporaryParent = gameObject.transform;
		if (underLightColor == Color.black || !GenieManager.DoesContainGenieMode(GlobalStorage.Instance.CurrentGenieModes, JobGenieCartridge.GenieModeTypes.EndlessMode))
		{
			if (underLightMesh != null)
			{
				underLightMesh.enabled = false;
			}
		}
		else if (underLightMesh != null)
		{
			underLightMesh.enabled = true;
			underLightMesh.material.SetColor("_TintColor", underLightColor);
		}
		for (int i = 0; i < optimizableTransforms.Length; i++)
		{
			VehicleReparentableTransform item = new VehicleReparentableTransform(optimizableTransforms[i], optimizationTemporaryParent);
			optimizeTransformsList.Add(item);
		}
		for (int j = 0; j < preoptimizedTransforms.Length; j++)
		{
			VehicleReparentableTransform vehicleReparentableTransform = new VehicleReparentableTransform(preoptimizedTransforms[j], optimizationTemporaryParent);
			preoptimizedTransformsList.Add(vehicleReparentableTransform);
			vehicleReparentableTransform.SetReparentedState(true);
		}
		for (int k = 0; k < doorDefinitions.Length; k++)
		{
			doorDefinitions[k].InitDoorWorldItem();
		}
	}

	public void CleanupAndPrepareToExit()
	{
		if (trunkCanBeNukedWhenLeaving && trunkCollectionZone != null)
		{
			ClearTrunkOfItems();
			if (trunkCanBeCompletelyHiddenWhileMoving)
			{
				for (int i = 0; i < temporarilyHiddenTrunkItems.Count; i++)
				{
					GameEventsManager.Instance.ItemActionOccurred(temporarilyHiddenTrunkItems[i].InteractableItem.WorldItemData, "DESTROYED");
					UnityEngine.Object.Destroy(temporarilyHiddenTrunkItems[i].gameObject);
				}
				temporarilyHiddenTrunkItems.Clear();
				temporaryRelativeTrunkItemPositions.Clear();
			}
		}
		if (!hoodCanBeNukedWhenLeaving || !(hoodCollectionZone != null))
		{
			return;
		}
		for (int j = 0; j < hoodCollectionZone.ItemsInCollection.Count; j++)
		{
			if (hoodCollectionZone.ItemsInCollection[j] != null)
			{
				UnityEngine.Object.Destroy(hoodCollectionZone.ItemsInCollection[j].gameObject);
			}
		}
		hoodCollectionZone.ItemsInCollection.Clear();
	}

	public void ClearTrunkOfItems()
	{
		for (int i = 0; i < trunkCollectionZone.ItemsInCollection.Count; i++)
		{
			if (trunkCollectionZone.ItemsInCollection[i] != null)
			{
				GameEventsManager.Instance.ItemActionOccurred(trunkCollectionZone.ItemsInCollection[i].InteractableItem.WorldItemData, "DESTROYED");
				UnityEngine.Object.Destroy(trunkCollectionZone.ItemsInCollection[i].gameObject);
			}
		}
		trunkCollectionZone.ItemsInCollection.Clear();
	}

	private void OnEnable()
	{
		for (int i = 0; i < interiorItemCollectionZones.Length; i++)
		{
			ItemCollectionZone obj = interiorItemCollectionZones[i];
			obj.OnItemsInCollectionRemoved = (Action<ItemCollectionZone, PickupableItem>)Delegate.Combine(obj.OnItemsInCollectionRemoved, new Action<ItemCollectionZone, PickupableItem>(ItemRemovedFromCar));
		}
		if (interiorCollectionZone != null)
		{
			ItemCollectionZone itemCollectionZone = interiorCollectionZone;
			itemCollectionZone.OnItemsInCollectionAdded = (Action<ItemCollectionZone, PickupableItem>)Delegate.Combine(itemCollectionZone.OnItemsInCollectionAdded, new Action<ItemCollectionZone, PickupableItem>(ItemAddedToInterior));
			ItemCollectionZone itemCollectionZone2 = interiorCollectionZone;
			itemCollectionZone2.OnItemsInCollectionRemoved = (Action<ItemCollectionZone, PickupableItem>)Delegate.Combine(itemCollectionZone2.OnItemsInCollectionRemoved, new Action<ItemCollectionZone, PickupableItem>(ItemRemovedFromInterior));
		}
		if (trunkCollectionZone != null)
		{
			ItemCollectionZone itemCollectionZone3 = trunkCollectionZone;
			itemCollectionZone3.OnItemsInCollectionAdded = (Action<ItemCollectionZone, PickupableItem>)Delegate.Combine(itemCollectionZone3.OnItemsInCollectionAdded, new Action<ItemCollectionZone, PickupableItem>(ItemAddedToTrunk));
			ItemCollectionZone itemCollectionZone4 = trunkCollectionZone;
			itemCollectionZone4.OnItemsInCollectionRemoved = (Action<ItemCollectionZone, PickupableItem>)Delegate.Combine(itemCollectionZone4.OnItemsInCollectionRemoved, new Action<ItemCollectionZone, PickupableItem>(ItemRemovedFromTrunk));
		}
		for (int j = 0; j < doorDefinitions.Length; j++)
		{
			doorDefinitions[j].DoorHinge.OnLowerLocked += DoorClosed;
			doorDefinitions[j].DoorHinge.OnLowerUnlocked += DoorOpened;
		}
		for (int k = 0; k < trackedAttachPoints.Count; k++)
		{
			if (trackedAttachPoints[k] != null)
			{
				trackedAttachPoints[k].OnObjectWasAttached += RegisteredAttachPointAttached;
				trackedAttachPoints[k].OnObjectWasDetached += RegisteredAttachPointDetached;
			}
			else
			{
				trackedAttachPoints.RemoveAt(k);
				k--;
			}
		}
	}

	private void OnDisable()
	{
		for (int i = 0; i < interiorItemCollectionZones.Length; i++)
		{
			ItemCollectionZone obj = interiorItemCollectionZones[i];
			obj.OnItemsInCollectionRemoved = (Action<ItemCollectionZone, PickupableItem>)Delegate.Remove(obj.OnItemsInCollectionRemoved, new Action<ItemCollectionZone, PickupableItem>(ItemRemovedFromCar));
		}
		if (interiorCollectionZone != null)
		{
			ItemCollectionZone itemCollectionZone = interiorCollectionZone;
			itemCollectionZone.OnItemsInCollectionAdded = (Action<ItemCollectionZone, PickupableItem>)Delegate.Remove(itemCollectionZone.OnItemsInCollectionAdded, new Action<ItemCollectionZone, PickupableItem>(ItemAddedToInterior));
			ItemCollectionZone itemCollectionZone2 = interiorCollectionZone;
			itemCollectionZone2.OnItemsInCollectionRemoved = (Action<ItemCollectionZone, PickupableItem>)Delegate.Remove(itemCollectionZone2.OnItemsInCollectionRemoved, new Action<ItemCollectionZone, PickupableItem>(ItemRemovedFromInterior));
		}
		if (trunkCollectionZone != null)
		{
			ItemCollectionZone itemCollectionZone3 = trunkCollectionZone;
			itemCollectionZone3.OnItemsInCollectionAdded = (Action<ItemCollectionZone, PickupableItem>)Delegate.Remove(itemCollectionZone3.OnItemsInCollectionAdded, new Action<ItemCollectionZone, PickupableItem>(ItemAddedToTrunk));
			ItemCollectionZone itemCollectionZone4 = trunkCollectionZone;
			itemCollectionZone4.OnItemsInCollectionRemoved = (Action<ItemCollectionZone, PickupableItem>)Delegate.Remove(itemCollectionZone4.OnItemsInCollectionRemoved, new Action<ItemCollectionZone, PickupableItem>(ItemRemovedFromTrunk));
		}
		for (int j = 0; j < doorDefinitions.Length; j++)
		{
			doorDefinitions[j].DoorHinge.OnLowerLocked -= DoorClosed;
			doorDefinitions[j].DoorHinge.OnLowerUnlocked -= DoorOpened;
		}
		for (int k = 0; k < trackedAttachPoints.Count; k++)
		{
			if (trackedAttachPoints[k] != null)
			{
				trackedAttachPoints[k].OnObjectWasAttached -= RegisteredAttachPointAttached;
				trackedAttachPoints[k].OnObjectWasDetached -= RegisteredAttachPointDetached;
			}
			else
			{
				trackedAttachPoints.RemoveAt(k);
				k--;
			}
		}
	}

	private void ItemAddedToInterior(ItemCollectionZone zone, PickupableItem item)
	{
		ProcessLimitedItemAdd(killableInteriorObjectsInOrder, item, psvrMaxItemsAllowedInInterior);
	}

	private void ItemRemovedFromInterior(ItemCollectionZone zone, PickupableItem item)
	{
		ProcessLimitedItemRemove(killableInteriorObjectsInOrder, item);
	}

	private void ItemAddedToTrunk(ItemCollectionZone zone, PickupableItem item)
	{
		ProcessLimitedItemAdd(killableTrunkObjectsInOrder, item, psvrMaxItemsAllowedInTrunk);
	}

	private void ItemRemovedFromTrunk(ItemCollectionZone zone, PickupableItem item)
	{
		ProcessLimitedItemRemove(killableTrunkObjectsInOrder, item);
	}

	private void ProcessLimitedItemAdd(List<GameObject> list, PickupableItem item, int maxItemsAllowed)
	{
		if (maxItemsAllowed != -1)
		{
		}
	}

	private void ProcessLimitedItemRemove(List<GameObject> list, PickupableItem item)
	{
	}

	private void RemoveNullsFromList(List<GameObject> list)
	{
		for (int i = 0; i < list.Count; i++)
		{
			if (list[i] == null)
			{
				list.RemoveAt(i);
				i--;
			}
		}
	}

	private void RegisteredAttachPointAttached(AttachablePoint point, AttachableObject obj)
	{
		GameEventsManager.Instance.ItemAppliedToItemActionOccurred(obj.PickupableItem.InteractableItem.WorldItemData, vehicleWorldItem.Data, "ATTACHED_TO");
	}

	private void RegisteredAttachPointDetached(AttachablePoint point, AttachableObject obj)
	{
		GameEventsManager.Instance.ItemAppliedToItemActionOccurred(obj.PickupableItem.InteractableItem.WorldItemData, vehicleWorldItem.Data, "DEATTACHED_FROM");
		if (point.Data.name == "VehicleGloveBoxAttachPoint")
		{
			AchievementManager.CompleteAchievement(8);
		}
	}

	public void SetDriverSeatName(string n)
	{
		if (driverSeatUniqueObject != null)
		{
			driverSeatUniqueObject.ManualChangeName(n);
		}
	}

	public void SetupWorldItems(WorldItemData customHoodData, WorldItemData customTrunkData, WorldItemData customDoorData, WorldItemData vehicleWorldItemData)
	{
		for (int i = 0; i < doorDefinitions.Length; i++)
		{
			doorDefinitions[i].InitDoorWorldItem();
			if (!(doorDefinitions[i].DoorWorldItem != null))
			{
				continue;
			}
			if (doorDefinitions[i].DoorType != 0 && doorDefinitions[i].DoorType != VehicleDoorTypes.Trunk)
			{
				doorDefinitions[i].DoorWorldItem.ManualSetData(customDoorData);
			}
			else if (doorDefinitions[i].DoorType == VehicleDoorTypes.Hood)
			{
				if (customHoodData != null)
				{
					doorDefinitions[i].DoorWorldItem.ManualSetData(customHoodData);
				}
			}
			else if (doorDefinitions[i].DoorType == VehicleDoorTypes.Trunk && customTrunkData != null)
			{
				doorDefinitions[i].DoorWorldItem.ManualSetData(customTrunkData);
			}
		}
		if (vehicleWorldItemData != null && vehicleWorldItem != null)
		{
			vehicleWorldItem.ManualSetData(vehicleWorldItemData);
		}
	}

	public GameObject SpawnExtraHardware(GameObject go, ChassisHardwareMountPoint mountPoint)
	{
		if (go == null)
		{
			return null;
		}
		GameObject gameObject = UnityEngine.Object.Instantiate(go);
		BasePrefabSpawner component = gameObject.GetComponent<BasePrefabSpawner>();
		if (component != null)
		{
			gameObject = component.LastSpawnedPrefabGO;
		}
		VehicleHardware component2 = gameObject.GetComponent<VehicleHardware>();
		InternalMountExistingHardware(component2, mountPoint);
		return gameObject;
	}

	public GameObject SpawnExtraHardware(VehicleHardware hardwarePrefab, ChassisHardwareMountPoint mountPoint)
	{
		if (hardwarePrefab == null)
		{
			return null;
		}
		VehicleHardware hardware = UnityEngine.Object.Instantiate(hardwarePrefab);
		InternalMountExistingHardware(hardware, mountPoint);
		return hardwarePrefab.gameObject;
	}

	private void AddHardwareToCollectionZone(VehicleHardware hardware, ItemCollectionZone zone)
	{
		PickupableItem[] componentsInChildren = hardware.GetComponentsInChildren<PickupableItem>();
		PickupableItem[] array = componentsInChildren;
		foreach (PickupableItem item in array)
		{
			zone.ItemsInCollection.Add(item);
		}
	}

	private void InternalMountExistingHardware(VehicleHardware hardware, ChassisHardwareMountPoint mountPoint)
	{
		switch (mountPoint)
		{
		case ChassisHardwareMountPoint.Root:
			hardware.transform.SetParent(base.transform);
			break;
		case ChassisHardwareMountPoint.Interior:
			hardware.transform.SetParent(interiorLocation);
			AddHardwareToCollectionZone(hardware, interiorCollectionZone);
			if (_dashboard == null && hardware is CarInteriorDashboardHardware)
			{
				_dashboard = hardware as CarInteriorDashboardHardware;
				cachedShifterTransform = _dashboard.GearShifterTransform;
			}
			break;
		case ChassisHardwareMountPoint.LicensePlate:
			hardware.transform.SetParent(licensePlateAttachPoint);
			break;
		case ChassisHardwareMountPoint.Engine:
			hardware.transform.SetParent(engineLocation);
			engineTransform = hardware.transform;
			SetEngineVisible(false);
			if (_engine == null && hardware is VehicleEngineHardware)
			{
				_engine = hardware as VehicleEngineHardware;
			}
			break;
		case ChassisHardwareMountPoint.Hood:
			hardware.transform.SetParent(hoodLocation);
			break;
		case ChassisHardwareMountPoint.GasTank:
			hardware.transform.SetParent(gasTankLocation);
			break;
		case ChassisHardwareMountPoint.Trunk:
			hardware.transform.SetParent(trunkLocation);
			AddHardwareToCollectionZone(hardware, trunkCollectionZone);
			break;
		default:
			Debug.LogError("ChassisHardwareMountPoint '" + mountPoint.ToString() + "' is not supported yet.");
			hardware.transform.SetParent(base.transform);
			break;
		}
		hardware.transform.SetToDefaultPosRotScale();
		InternalRegisterHardware(hardware);
	}

	private void InternalRegisterHardware(VehicleHardware hardware)
	{
		hardware.AttachToChassis(this);
		hardwareOnChassis.Add(hardware);
		trackedAttachPoints.AddRange(hardware.TrackedAttachPoints);
		AttachableObject componentInChildren = hardware.GetComponentInChildren<AttachableObject>();
		for (int i = 0; i < hardware.TrackedAttachPoints.Length; i++)
		{
			hardware.TrackedAttachPoints[i].OnObjectWasAttached += RegisteredAttachPointAttached;
			hardware.TrackedAttachPoints[i].OnObjectWasDetached += RegisteredAttachPointDetached;
		}
		for (int j = 0; j < hardware.OptimizableTransforms.Length; j++)
		{
			VehicleReparentableTransform item = new VehicleReparentableTransform(hardware.OptimizableTransforms[j], optimizationTemporaryParent);
			optimizeTransformsList.Add(item);
		}
	}

	public void DeregisterHardware(VehicleHardware hardware)
	{
		List<int> list = new List<int>();
		for (int i = 0; i < optimizeTransformsList.Count; i++)
		{
			for (int j = 0; j < hardware.OptimizableTransforms.Length; j++)
			{
				if (optimizeTransformsList[i].TransformToReparent == hardware.OptimizableTransforms[j])
				{
					list.Add(i);
				}
			}
		}
		for (int k = 0; k < list.Count; k++)
		{
			optimizeTransformsList.Remove(optimizeTransformsList[list[k]]);
		}
		for (int l = 0; l < hardware.TrackedAttachPoints.Length; l++)
		{
			hardware.TrackedAttachPoints[l].OnObjectWasAttached -= RegisteredAttachPointAttached;
			hardware.TrackedAttachPoints[l].OnObjectWasDetached -= RegisteredAttachPointDetached;
		}
		for (int m = 0; m < hardware.TrackedAttachPoints.Length; m++)
		{
			trackedAttachPoints.Remove(hardware.TrackedAttachPoints[m]);
		}
		hardwareOnChassis.Remove(hardware);
	}

	public void SpawnWheels(WheelAttachmentManager wheelAttachmentPrefab, GameObject[] wheelPrefabs, int[] indexesToStartDeflated)
	{
		spawnedWheelsOnLeft.Clear();
		spawnedWheelsOnRight.Clear();
		int num = 0;
		int num2 = 0;
		if (wheelAttachmentLocationsOnLeft != null)
		{
			for (int i = 0; i < wheelAttachmentLocationsOnLeft.Length; i++)
			{
				WheelAttachmentManager wheelAttachmentManager = SpawnWheelForLocation(wheelAttachmentLocationsOnLeft[i], wheelAttachmentPrefab, wheelPrefabs[num2]);
				if (Array.IndexOf(indexesToStartDeflated, num) > -1)
				{
					wheelAttachmentManager.SetWheelInflation(0f);
				}
				spawnedWheelsOnLeft.Add(wheelAttachmentManager);
				num++;
				num2++;
				if (num2 >= wheelPrefabs.Length)
				{
					num2 = 0;
				}
			}
		}
		if (wheelAttachmentLocationsOnRight != null)
		{
			for (int j = 0; j < wheelAttachmentLocationsOnRight.Length; j++)
			{
				WheelAttachmentManager wheelAttachmentManager2 = SpawnWheelForLocation(wheelAttachmentLocationsOnRight[j], wheelAttachmentPrefab, wheelPrefabs[num2]);
				if (Array.IndexOf(indexesToStartDeflated, num) > -1)
				{
					wheelAttachmentManager2.SetWheelInflation(0f);
				}
				spawnedWheelsOnRight.Add(wheelAttachmentManager2);
				num++;
				num2++;
				if (num2 >= wheelPrefabs.Length)
				{
					num2 = 0;
				}
			}
		}
		steeringWheelsOriginalRotations = new Quaternion[wheelsThatSteer.Length];
		for (int k = 0; k < wheelsThatSteer.Length; k++)
		{
			steeringWheelsOriginalRotations[k] = wheelsThatSteer[k].localRotation;
		}
	}

	private WheelAttachmentManager SpawnWheelForLocation(Transform location, WheelAttachmentManager wheelAttachmentPrefab, GameObject wheelPrefab)
	{
		WheelAttachmentManager wheelAttachmentManager = UnityEngine.Object.Instantiate(wheelAttachmentPrefab);
		wheelAttachmentManager.transform.SetParent(location);
		wheelAttachmentManager.transform.SetToDefaultPosRotScale();
		wheelAttachmentManager.SpawnWheel(wheelPrefab);
		wheelAttachmentManager.transform.Rotate(Vector3.right * UnityEngine.Random.Range(0f, 360f));
		InternalRegisterHardware(wheelAttachmentManager);
		return wheelAttachmentManager;
	}

	public void SpawnHeadlights(VehicleAttachableHardware[] headlightAttachmentPrefab, GameObject headlightPrefab)
	{
		if (!(headlightPrefab != null))
		{
			return;
		}
		for (int i = 0; i < headlightAttachmentLocations.Length; i++)
		{
			if (headlightAttachmentLocationsVAH[i] == null)
			{
				headlightAttachmentLocationsVAH[i] = UnityEngine.Object.Instantiate(headlightAttachmentPrefab[i % headlightAttachmentPrefab.Length]);
			}
			headlightAttachmentLocationsVAH[i].transform.SetParent(headlightAttachmentLocations[i]);
			headlightAttachmentLocationsVAH[i].transform.SetToDefaultPosRotScale();
			headlightAttachmentLocationsVAH[i].SpawnPrefab(headlightPrefab);
			InternalRegisterHardware(headlightAttachmentLocationsVAH[i]);
			headlightAttachmentLocationsVAH[i].GetComponentInChildren<HeadlightBulbController>().GetHeadlightRenderer().material.color = headlightStartColor;
		}
	}

	public void SpawnHoodOrnaments(VehicleAttachableHardware hoodOrnamentAttachmentPrefab, GameObject hoodOrnamentPrefab)
	{
		if (!(hoodOrnamentAttachmentPrefab != null) || !(hoodOrnamentPrefab != null))
		{
			return;
		}
		for (int i = 0; i < hoodOrnamentAttachmentLocations.Length; i++)
		{
			if (hoodOrnamentAttachmentLocations[i].gameObject.activeSelf)
			{
				VehicleAttachableHardware vehicleAttachableHardware = UnityEngine.Object.Instantiate(hoodOrnamentAttachmentPrefab);
				vehicleAttachableHardware.transform.SetParent(hoodOrnamentAttachmentLocations[i]);
				vehicleAttachableHardware.transform.SetToDefaultPosRotScale();
				vehicleAttachableHardware.SpawnPrefab(hoodOrnamentPrefab);
				InternalRegisterHardware(vehicleAttachableHardware);
			}
		}
	}

	public void SpawnTrinketMount(TrinketMountController trinketMountPrefab, GameObject[] initialTrinkets, WorldItemData mountWorldItemData)
	{
		if (trinketMountPrefab != null && interiorTrinketMountLocation != null)
		{
			TrinketMountController trinketMountController = UnityEngine.Object.Instantiate(trinketMountPrefab);
			trinketMountController.transform.SetParent(interiorTrinketMountLocation);
			trinketMountController.transform.SetToDefaultPosRotScale();
			if (mountWorldItemData != null)
			{
				trinketMountController.WorldItem.ManualSetData(mountWorldItemData);
			}
			trinketMountController.SpawnTrinketItems(initialTrinkets, true);
		}
	}

	public void SpawnLicensePlate(VehicleAttachableHardware licensePlateAttachmentPrefab, GameObject licensePlatePrefab)
	{
		if (licensePlatePrefab != null)
		{
			if (licensePlateAttachPointVehicleAttachableHardware == null)
			{
				licensePlateAttachPointVehicleAttachableHardware = UnityEngine.Object.Instantiate(licensePlateAttachmentPrefab);
			}
			licensePlateAttachPointVehicleAttachableHardware.transform.SetParent(licensePlateAttachPoint);
			licensePlateAttachPointVehicleAttachableHardware.transform.SetToDefaultPosRotScale();
			licensePlateAttachPointVehicleAttachableHardware.SpawnPrefab(licensePlatePrefab);
			InternalRegisterHardware(licensePlateAttachPointVehicleAttachableHardware);
		}
	}

	private void DoorOpened(GrabbableHinge hinge)
	{
		VehicleDoorDefinition vehicleDoorDefinition = FindDoorDefinitionByHinge(hinge);
		if (vehicleDoorDefinition != null)
		{
			if (vehicleDoorDefinition.DoorType == VehicleDoorTypes.Hood)
			{
				SetEngineVisible(true);
			}
			if (!doorsBeingForcedClosed)
			{
				StartCoroutine(DelayedItemAction(vehicleDoorDefinition.DoorWorldItem.Data, "OPENED", 75));
			}
		}
	}

	private IEnumerator DelayedItemAction(WorldItemData data, string eventName, int delayFrames)
	{
		for (int i = 0; i < delayFrames; i++)
		{
			yield return null;
		}
		GameEventsManager.Instance.ItemActionOccurred(data, eventName);
	}

	private void DoorClosed(GrabbableHinge hinge, bool isInitial)
	{
		VehicleDoorDefinition vehicleDoorDefinition = FindDoorDefinitionByHinge(hinge);
		if (vehicleDoorDefinition != null)
		{
			if (vehicleDoorDefinition.DoorType == VehicleDoorTypes.Hood && !isDoingDelayedHideEngine)
			{
				isDoingDelayedHideEngine = true;
				StartCoroutine(WaitAndHideEngine());
			}
			StartCoroutine(DelayedItemAction(FindDoorDefinitionByHinge(hinge).DoorWorldItem.Data, "CLOSED", 75));
		}
	}

	private IEnumerator WaitAndHideEngine()
	{
		yield return new WaitForSeconds(1f);
		if (isDoingDelayedHideEngine)
		{
			SetEngineVisible(false);
		}
	}

	private VehicleDoorDefinition FindDoorDefinitionByHinge(GrabbableHinge hinge)
	{
		for (int i = 0; i < doorDefinitions.Length; i++)
		{
			if (hinge == doorDefinitions[i].DoorHinge)
			{
				return doorDefinitions[i];
			}
		}
		return null;
	}

	public void ForceCloseDoors(VehicleDoorTypes doorsToClose = VehicleDoorTypes.All, bool instant = false)
	{
		doorsBeingForcedClosed = true;
		if (!doorsAreLocked)
		{
			doorsAreLocked = true;
			SetLockedStateOfItemsWithinCar(true);
		}
		for (int i = 0; i < doorDefinitions.Length; i++)
		{
			if (IsDoorTypeIncluded(doorDefinitions[i].DoorType, doorsToClose))
			{
				StartCoroutine(doorDefinitions[i].ForceDoorClosed(instant));
			}
		}
		StartCoroutine(WaitAndSetThatForcedDoorCloseFinished());
		if (doorsToClose == VehicleDoorTypes.All || doorsToClose == VehicleDoorTypes.Hood)
		{
			SetEngineVisible(false);
		}
		if (doorsToClose == VehicleDoorTypes.All)
		{
			SetOptimizedState(true);
		}
	}

	private IEnumerator WaitAndSetThatForcedDoorCloseFinished()
	{
		yield return new WaitForSeconds(0.25f);
		doorsBeingForcedClosed = false;
	}

	public void ForceOpenDoors(VehicleDoorTypes doorsToOpen = VehicleDoorTypes.All, bool setOptimizedState = false, bool unlock = true)
	{
		if (setOptimizedState)
		{
			SetOptimizedState(false);
		}
		doorsBeingForcedClosed = false;
		for (int i = 0; i < doorDefinitions.Length; i++)
		{
			if (IsDoorTypeIncluded(doorDefinitions[i].DoorType, doorsToOpen))
			{
				StartCoroutine(doorDefinitions[i].ForceDoorOpen());
			}
		}
		if (doorsAreLocked && unlock)
		{
			doorsAreLocked = false;
			SetLockedStateOfItemsWithinCar(false);
		}
	}

	public void AllowDoorOpening(VehicleDoorTypes doorsToAllow = VehicleDoorTypes.All)
	{
		SetOptimizedState(false);
		for (int i = 0; i < doorDefinitions.Length; i++)
		{
			if (IsDoorTypeIncluded(doorDefinitions[i].DoorType, doorsToAllow))
			{
				doorDefinitions[i].AllowOpening();
			}
		}
		if (doorsAreLocked)
		{
			doorsAreLocked = false;
			SetLockedStateOfItemsWithinCar(false);
		}
	}

	private bool IsDoorTypeIncluded(VehicleDoorTypes doorInQuestion, VehicleDoorTypes typesToInclude)
	{
		switch (typesToInclude)
		{
		case VehicleDoorTypes.All:
			return true;
		case VehicleDoorTypes.FrontDoors:
			return doorInQuestion == VehicleDoorTypes.DriverDoor || doorInQuestion == VehicleDoorTypes.PassengerDoor;
		default:
			return typesToInclude == doorInQuestion;
		}
	}

	private void ItemRemovedFromCar(ItemCollectionZone zone, PickupableItem item)
	{
		if (!doorsAreLocked && item.transform.IsChildOf(base.transform))
		{
			AttachableObject component = item.GetComponent<AttachableObject>();
			if (component == null)
			{
				item.transform.SetParent(GlobalStorage.Instance.ContentRoot, true);
			}
			else if (component.CurrentlyAttachedTo == null)
			{
				item.transform.SetParent(GlobalStorage.Instance.ContentRoot, true);
			}
		}
	}

	private void Update()
	{
		if (itemsAreLockedAndNeedUpdating && Time.frameCount > frameOfLastLockedUpdate + (FRAMES_PER_LOCKED_ITEM_UPDATE - 1))
		{
			UpdateLockedItems();
			frameOfLastLockedUpdate = Time.frameCount;
		}
	}

	private void UpdateLockedItems()
	{
		for (int i = 0; i < itemRelativeTransforms.Count; i++)
		{
			itemRelativeTransforms[i].Update();
		}
	}

	public void DestroyLockedPickupables()
	{
		for (int i = 0; i < pickupablesLockedLastTime.Count; i++)
		{
			if (pickupablesLockedLastTime[i] != null && pickupablesLockedLastTime[i].gameObject != null)
			{
				UnityEngine.Object.Destroy(pickupablesLockedLastTime[i].gameObject);
			}
		}
		pickupablesLockedLastTime.Clear();
	}

	public void SetLockedStateOfItemsWithinCar(bool state)
	{
		bool flag = false;
		if (state != trunkIsTemporaryLocked)
		{
			trunkIsTemporaryLocked = state;
			flag = true;
		}
		if (state)
		{
			pickupablesLockedLastTime.Clear();
			if (flag)
			{
				temporarilyHiddenTrunkItems.Clear();
				temporaryRelativeTrunkItemPositions.Clear();
			}
		}
		bool flag2 = false;
		if (state != itemsAreLockedAndNeedUpdating)
		{
			itemsAreLockedAndNeedUpdating = state;
			itemRelativeTransforms.Clear();
			if (state)
			{
				flag2 = true;
				frameOfLastLockedUpdate = -1;
			}
		}
		for (int i = 0; i < interiorItemCollectionZones.Length; i++)
		{
			if (interiorItemCollectionZones[i] == trunkCollectionZone && trunkCanBeCompletelyHiddenWhileMoving)
			{
				if (!flag)
				{
					continue;
				}
				for (int j = 0; j < interiorItemCollectionZones[i].ItemsInCollection.Count; j++)
				{
					PickupableItem pickupableItem = interiorItemCollectionZones[i].ItemsInCollection[j];
					if (!state || !(pickupableItem.Rigidbody != null))
					{
						continue;
					}
					AttachableObject component = pickupableItem.GetComponent<AttachableObject>();
					bool flag3 = false;
					if (component != null)
					{
						if (!IsObjectAttachedToTrackedAttachPoint(component))
						{
							pickupableItem.Rigidbody.isKinematic = true;
							temporaryRelativeTrunkItemPositions.Add(new VehicleItemRelativeTransformInfo(pickupableItem.transform, base.transform));
							pickupableItem.transform.position += Vector3.up * 1500f;
							temporarilyHiddenTrunkItems.Add(pickupableItem);
						}
					}
					else
					{
						pickupableItem.Rigidbody.isKinematic = true;
						temporaryRelativeTrunkItemPositions.Add(new VehicleItemRelativeTransformInfo(pickupableItem.transform, base.transform));
						pickupableItem.transform.position += Vector3.up * 1500f;
						temporarilyHiddenTrunkItems.Add(pickupableItem);
					}
				}
				continue;
			}
			for (int k = 0; k < interiorItemCollectionZones[i].ItemsInCollection.Count; k++)
			{
				PickupableItem pickupableItem2 = interiorItemCollectionZones[i].ItemsInCollection[k];
				if (!(pickupableItem2 != null))
				{
					continue;
				}
				AttachableObject component2 = pickupableItem2.GetComponent<AttachableObject>();
				bool flag4 = false;
				if (component2 != null)
				{
					flag4 = IsObjectAttachedToTrackedAttachPoint(component2);
					if (!flag4 && pickupableItem2.Rigidbody != null)
					{
						pickupableItem2.Rigidbody.isKinematic = state;
					}
				}
				else if (pickupableItem2.Rigidbody != null)
				{
					pickupableItem2.Rigidbody.isKinematic = state;
				}
				pickupableItem2.enabled = !state;
				if (state && !pickupablesLockedLastTime.Contains(pickupableItem2))
				{
					pickupablesLockedLastTime.Add(pickupableItem2);
				}
				if (state && !flag4 && flag2)
				{
					itemRelativeTransforms.Add(new VehicleItemRelativeTransformInfo(pickupableItem2.transform, base.transform));
				}
			}
		}
		if (state)
		{
			return;
		}
		for (int l = 0; l < pickupablesLockedLastTime.Count; l++)
		{
			if (!(pickupablesLockedLastTime[l] != null) || pickupablesLockedLastTime[l].enabled)
			{
				continue;
			}
			pickupablesLockedLastTime[l].enabled = true;
			PickupableItem pickupableItem3 = pickupablesLockedLastTime[l];
			AttachableObject component3 = pickupableItem3.GetComponent<AttachableObject>();
			bool flag5 = false;
			if (component3 != null)
			{
				if (!IsObjectAttachedToTrackedAttachPoint(component3) && pickupableItem3.Rigidbody != null)
				{
					pickupableItem3.Rigidbody.isKinematic = false;
				}
			}
			else if (pickupableItem3.Rigidbody != null)
			{
				pickupableItem3.Rigidbody.isKinematic = false;
			}
		}
		pickupablesLockedLastTime.Clear();
		if (!flag)
		{
			return;
		}
		for (int m = 0; m < temporarilyHiddenTrunkItems.Count; m++)
		{
			PickupableItem pickupableItem4 = temporarilyHiddenTrunkItems[m];
			if (pickupableItem4.Rigidbody != null)
			{
				temporaryRelativeTrunkItemPositions[m].Update();
				pickupableItem4.Rigidbody.isKinematic = false;
			}
		}
		temporarilyHiddenTrunkItems.Clear();
		temporaryRelativeTrunkItemPositions.Clear();
	}

	private bool IsObjectAttachedToTrackedAttachPoint(AttachableObject obj)
	{
		for (int i = 0; i < trackedAttachPoints.Count; i++)
		{
			if (trackedAttachPoints[i].AttachedObjects.Contains(obj))
			{
				return true;
			}
		}
		return false;
	}

	public void AnimateWheelsWithTween(float duration, GoEaseType easeType)
	{
		StartCoroutine(AnimateWheelsWithTweenAsync(duration, easeType));
	}

	private IEnumerator AnimateWheelsWithTweenAsync(float duration, GoEaseType easeType)
	{
		int flatWheels = 0;
		for (int n = 0; n < spawnedWheelsOnLeft.Count; n++)
		{
			if (spawnedWheelsOnLeft[n].GetWheelIsFlat())
			{
				flatWheels++;
			}
			spawnedWheelsOnLeft[n].SetFlatFX(spawnedWheelsOnLeft[n].GetWheelIsFlat());
		}
		for (int m = 0; m < spawnedWheelsOnRight.Count; m++)
		{
			if (spawnedWheelsOnRight[m].GetWheelIsFlat())
			{
				flatWheels++;
			}
			spawnedWheelsOnRight[m].SetFlatFX(spawnedWheelsOnRight[m].GetWheelIsFlat());
		}
		for (int l = 0; l < spawnedWheelsOnLeft.Count; l++)
		{
			Go.to(spawnedWheelsOnLeft[l].SpinTransform, duration, new GoTweenConfig().localEulerAngles(Vector3.right * -5000f, true).setEaseType(easeType));
		}
		for (int k = 0; k < spawnedWheelsOnRight.Count; k++)
		{
			Go.to(spawnedWheelsOnRight[k].SpinTransform, duration, new GoTweenConfig().localEulerAngles(Vector3.right * 5000f, true).setEaseType(easeType));
		}
		float percWheelsFlat = (float)flatWheels / (float)(spawnedWheelsOnLeft.Count + spawnedWheelsOnRight.Count);
		float t = duration;
		while (t > 0f)
		{
			t -= Time.deltaTime;
			if (flatWheels > 0)
			{
				base.transform.localEulerAngles = Vector3.forward * Mathf.Sin(Time.time * 57.29578f * 500f) * Mathf.Lerp(0.5f, 1.5f, percWheelsFlat);
			}
			yield return null;
		}
		for (int j = 0; j < spawnedWheelsOnLeft.Count; j++)
		{
			spawnedWheelsOnLeft[j].SetFlatFX(false);
		}
		for (int i = 0; i < spawnedWheelsOnRight.Count; i++)
		{
			spawnedWheelsOnRight[i].SetFlatFX(false);
		}
	}

	public void SpinWheelsByAmount(float amt)
	{
		float num = 1f;
		if (currentGear == Gear.Reverse)
		{
			num = -1f;
		}
		for (int i = 0; i < spawnedWheelsOnLeft.Count; i++)
		{
			spawnedWheelsOnLeft[i].SpinTransform.Rotate(Vector3.right * (0f - amt) * num);
		}
		for (int j = 0; j < spawnedWheelsOnRight.Count; j++)
		{
			spawnedWheelsOnRight[j].SpinTransform.Rotate(Vector3.right * amt * num);
		}
	}

	public void SteerWheelsByAmount(float amt)
	{
		if (wheelsThatSteer != null && steeringWheelsOriginalRotations != null)
		{
			currentSteerAmount += amt;
			if (currentSteerAmount > maxSteeringAngle)
			{
				currentSteerAmount = maxSteeringAngle;
			}
			if (currentSteerAmount < 0f - maxSteeringAngle)
			{
				currentSteerAmount = 0f - maxSteeringAngle;
			}
			for (int i = 0; i < wheelsThatSteer.Length; i++)
			{
				wheelsThatSteer[i].localRotation = steeringWheelsOriginalRotations[i] * Quaternion.Euler(Vector3.up * currentSteerAmount);
			}
		}
	}

	public void ChangeGear(Gear gear)
	{
		currentGear = gear;
	}

	private void OnDrawGizmos()
	{
		if (wheelAttachmentLocationsOnLeft != null)
		{
			Gizmos.color = Color.cyan;
			for (int i = 0; i < wheelAttachmentLocationsOnLeft.Length; i++)
			{
				if (wheelAttachmentLocationsOnLeft[i] != null)
				{
					Gizmos.DrawWireSphere(wheelAttachmentLocationsOnLeft[i].position, 0.15f);
				}
			}
		}
		if (wheelAttachmentLocationsOnRight != null)
		{
			Gizmos.color = Color.cyan;
			for (int j = 0; j < wheelAttachmentLocationsOnRight.Length; j++)
			{
				if (wheelAttachmentLocationsOnRight[j] != null)
				{
					Gizmos.DrawWireSphere(wheelAttachmentLocationsOnRight[j].position, 0.15f);
				}
			}
		}
		if (leftBound != null && rightBound != null && frontBound != null && backBound != null)
		{
			Gizmos.color = Color.red;
			Gizmos.DrawLine(leftBound.position, rightBound.position);
			Gizmos.DrawLine(frontBound.position, backBound.position);
		}
		if (headlightAttachmentLocations != null)
		{
			Gizmos.color = Color.yellow;
			for (int k = 0; k < headlightAttachmentLocations.Length; k++)
			{
				if (headlightAttachmentLocations[k] != null)
				{
					Gizmos.DrawLine(headlightAttachmentLocations[k].position, headlightAttachmentLocations[k].position + headlightAttachmentLocations[k].forward * -0.1f);
				}
			}
		}
		if (licensePlateAttachPoint != null)
		{
			Gizmos.color = Color.green;
			Gizmos.DrawWireSphere(licensePlateAttachPoint.position, 0.15f);
		}
		if (interiorLocation != null)
		{
			Gizmos.color = Color.magenta;
			Gizmos.DrawWireSphere(interiorLocation.position, 0.15f);
		}
	}

	private void SetEngineVisible(bool state)
	{
		isDoingDelayedHideEngine = false;
		if (!(engineTransform == null) && state != engineIsShown)
		{
			engineIsShown = state;
			if (state)
			{
				engineTransform.SetParent(engineLocation, false);
				engineTransform.gameObject.SetActive(true);
			}
			else
			{
				engineTransform.gameObject.SetActive(false);
				engineTransform.SetParent(optimizationTemporaryParent, false);
			}
		}
	}

	public void SetOptimizedState(bool state)
	{
		if (cachedShifterTransform != null && shifterTransformToAdjust != null)
		{
			shifterTransformToAdjust.localEulerAngles = cachedShifterTransform.localEulerAngles + Vector3.right * 35f;
		}
		if (state)
		{
			for (int i = 0; i < hardwareOnChassis.Count; i++)
			{
				hardwareOnChassis[i].WillBecomeOptimized();
			}
		}
		if (state)
		{
			SetEngineVisible(false);
			SetLockedStateOfItemsWithinCar(true);
		}
		for (int j = 0; j < optimizeTransformsList.Count; j++)
		{
			optimizeTransformsList[j].SetReparentedState(state);
		}
		for (int k = 0; k < preoptimizedTransformsList.Count; k++)
		{
			preoptimizedTransformsList[k].SetReparentedState(!state);
		}
		for (int l = 0; l < optimizeTransformsList.Count; l++)
		{
			optimizeTransformsList[l].SetColliderState(false);
		}
		for (int m = 0; m < preoptimizedTransformsList.Count; m++)
		{
			preoptimizedTransformsList[m].SetColliderState(false);
		}
		for (int n = 0; n < optimizeTransformsList.Count; n++)
		{
			optimizeTransformsList[n].SetColliderState(true);
		}
		for (int num = 0; num < preoptimizedTransformsList.Count; num++)
		{
			preoptimizedTransformsList[num].SetColliderState(true);
		}
		if (!state)
		{
			SetLockedStateOfItemsWithinCar(false);
		}
		if (!state)
		{
			for (int num2 = 0; num2 < hardwareOnChassis.Count; num2++)
			{
				hardwareOnChassis[num2].HasBecomeUnoptimized();
			}
		}
	}

	public void StartFlameSFX()
	{
		AudioSourceHelper[] array = flameExhaustSoundHelpers;
		foreach (AudioSourceHelper audioSourceHelper in array)
		{
			audioSourceHelper.enabled = true;
			if (!audioSourceHelper.IsPlaying)
			{
				audioSourceHelper.SetLooping(true);
				audioSourceHelper.SetClip(flameExhaustClip);
				audioSourceHelper.Play();
			}
		}
	}

	public void StopFlameSFX()
	{
		AudioSourceHelper[] array = flameExhaustSoundHelpers;
		foreach (AudioSourceHelper audioSourceHelper in array)
		{
			if (audioSourceHelper.IsPlaying)
			{
				audioSourceHelper.Stop();
			}
			audioSourceHelper.enabled = false;
		}
	}
}
