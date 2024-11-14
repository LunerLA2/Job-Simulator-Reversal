using System;
using System.Collections;
using System.Collections.Generic;
using OwlchemyVR;
using UnityEngine;

public class VehicleController : MonoBehaviour
{
	public class EndlessVehiclePartsOptions
	{
		public EndlessVehiclePrefabIniter.VehicleChassisGroup chassisGroup;

		public GameObject[] wheelPrefabsList;

		public GameObject chosenWheel;

		public GameObject chosenHeadlightPrefab;

		public GameObject chosenLicensePlatePrefab;

		public GameObject chosenHoodOrnamentPrefab;

		public GameObject chosenGasTankPrefab;

		public Texture detailCarTexture;

		public PageData[] sketchyPlatesPagesOptions;

		public GameObject[] sketchyPlatePrefabsOptions;

		public PageData[] brokenTiresPagesOptions;

		public GameObject[] brokenTirePrefabsOptions;

		public PageData[] brokenHeadlightsPagesOptions;

		public GameObject[] brokenHeadlightPrefabsOptions;

		public PageData[] badAirFilterPagesOptions;

		public PageData[] badBatteryPagesOptions;

		public PageData[] brokenPistonsPagesOptions;

		public PageData[] noPistonsPagesOptions;

		public PageData[] engineOilFluidPagesOptions;

		public PageData[] engineOverheatPagesOptions;

		public VehicleExtraHardwarePiece overheatObject;

		public EndlessEnginePrefabIniter enginePrefabIniter;

		public GameObject[] wheelOptionsList;

		public int[] wheelIndexesToStartDeflated;

		public GameObject[] licensePlatePrefabOptions;

		public GameObject[] hoodOrnamentPrefabsOptions;

		public string driverSeatUOName;

		public AudioClip[] optionalDriveInSoundsList;

		public AudioClip[] optionalDriveOutSoundsList;

		public bool isFilterSetDirty;

		public bool needToSetFluidColor;

		public WorldItemData initialFluidData;

		public float initialFluidPercent;

		public GameObject[] defaultTrinketObjectsOptions;

		public List<VehicleExtraHardwarePiece> extraHardwareOptions;

		public VehicleExtraHardwarePiece[] extraHardwarePSVROnlyOptions;

		public VehicleExtraHardwarePiece[] extraHardwaveNonPSVROnlyOptions;

		public WorldItemData vehicleWorldItem;

		public WorldItemData customHoodWorldItemData;

		public WorldItemData customDoorWorldItemData;

		public WorldItemData customTrunkWorldItemData;

		public WorldItemData interiorTrinketMountWorldItemData;
	}

	private const string ENDLESS_DRIVERS_SEAT_UO_NAME = "EndlessVehicleDriverSeat";

	public UniqueObject endlessDriversSeatPrefab;

	private static UniqueObject endlessDriversSeatUO;

	[SerializeField]
	private WheelAttachmentManager masterWheelAttachmentControllerPrefab;

	[SerializeField]
	private VehicleAttachableHardware[] masterHeadlightAttachmentControllerPrefab;

	[SerializeField]
	private VehicleAttachableHardware masterLicensePlateAttachmentControllerPrefab;

	[SerializeField]
	private VehicleAttachableHardware masterHoodOrnamentAttacmentControllerPrefab;

	[SerializeField]
	private TrinketMountController masterTrinketMountPrefab;

	[SerializeField]
	private AudioSourceHelper carDrivingAudioSource;

	private AudioClip drivingAudioOnEnter;

	private AudioClip drivingAudioOnExit;

	private bool hideDetailTextureOncePainted;

	private bool swapDetailTextureOncePainted;

	private bool isThisTheEndlessChosenOne;

	private EndlessVehiclePrefabIniter.VehicleChassisGroup chassisGroup;

	private GameObject gasTankObj;

	public Texture detailTextureOnPaint;

	private Color defaultColor;

	private Texture baseCarTexture;

	private Texture detailCarTexture;

	private VehicleChassisController chassis;

	private UniqueObject EndlessDriversSeatUO
	{
		get
		{
			if (endlessDriversSeatUO == null)
			{
				endlessDriversSeatUO = BotUniqueElementManager._instanceNoCreate.GetObjectByNameNoErrorIfNull("EndlessVehicleDriverSeat");
				if (endlessDriversSeatUO == null)
				{
					endlessDriversSeatUO = UnityEngine.Object.Instantiate(endlessDriversSeatPrefab, chassis.DriversSeatParent) as UniqueObject;
					endlessDriversSeatUO.transform.localPosition = Vector3.zero;
					endlessDriversSeatUO.transform.localRotation = Quaternion.identity;
				}
			}
			return endlessDriversSeatUO;
		}
		set
		{
			endlessDriversSeatUO = value;
		}
	}

	public bool HideDetailTextureOncePainted
	{
		get
		{
			return hideDetailTextureOncePainted;
		}
	}

	public bool SwapDetailTextureOncePainted
	{
		get
		{
			return swapDetailTextureOncePainted;
		}
	}

	public Vector3 LeftBoundPosition
	{
		get
		{
			return chassis.LeftBoundPosition;
		}
	}

	public Vector3 RightBoundPosition
	{
		get
		{
			return chassis.RightBoundPosition;
		}
	}

	public Vector3 FrontBoundPosition
	{
		get
		{
			return chassis.FrontBoundPosition;
		}
	}

	public Vector3 BackBoundPosition
	{
		get
		{
			return chassis.BackBoundPosition;
		}
	}

	public VehicleChassisController GetChassis
	{
		get
		{
			return chassis;
		}
	}

	public EndlessVehiclePrefabIniter.VehicleChassisGroup ChassisGroup
	{
		get
		{
			return chassisGroup;
		}
		set
		{
			chassisGroup = value;
		}
	}

	private void Start()
	{
		if (GenieManager.DoesContainGenieMode(GlobalStorage.Instance.CurrentGenieModes, JobGenieCartridge.GenieModeTypes.EndlessMode))
		{
		}
	}

	public void SetHideDetailTextureOncePainted(bool v)
	{
		hideDetailTextureOncePainted = v;
	}

	public void SetSwapDetailTextureOncePainted(bool v)
	{
		swapDetailTextureOncePainted = v;
	}

	public void SetupDrivingAudio(AudioClip clipOnEnter, AudioClip clipOnExit)
	{
		drivingAudioOnEnter = clipOnEnter;
		drivingAudioOnExit = clipOnExit;
	}

	public VehicleChassisController SpawnChassis(VehicleChassisController _chassis, string _driverSeatID, WorldItemData _customDoorData, WorldItemData _customHoodData, WorldItemData _customTrunkData, WorldItemData _vehicleWorldItemData)
	{
		chassis = UnityEngine.Object.Instantiate(_chassis);
		chassis.transform.parent = base.transform;
		chassis.transform.SetToDefaultPosRotScale();
		carDrivingAudioSource.transform.parent = chassis.transform;
		chassis.Init();
		if (!GenieManager.DoesContainGenieMode(GlobalStorage.Instance.CurrentGenieModes, JobGenieCartridge.GenieModeTypes.EndlessMode))
		{
			chassis.SetDriverSeatName(_driverSeatID);
		}
		chassis.SetupWorldItems(_customHoodData, _customTrunkData, _customDoorData, _vehicleWorldItemData);
		return chassis;
	}

	public void SetTextureSettings(Color _defaultColor, Texture _baseTex, Texture _detailTex)
	{
		defaultColor = _defaultColor;
		baseCarTexture = _baseTex;
		detailCarTexture = _detailTex;
	}

	public GameObject SpawnExtraHardware(VehicleHardware hardwarePrefab, VehicleChassisController.ChassisHardwareMountPoint mountPoint)
	{
		return chassis.SpawnExtraHardware(hardwarePrefab, mountPoint);
	}

	public GameObject SpawnExtraHardware(GameObject go, VehicleChassisController.ChassisHardwareMountPoint mountPoint)
	{
		return chassis.SpawnExtraHardware(go, mountPoint);
	}

	public void SpawnWheels(GameObject[] wheelPrefabs, int[] indexesToStartDeflated)
	{
		chassis.SpawnWheels(masterWheelAttachmentControllerPrefab, wheelPrefabs, indexesToStartDeflated);
	}

	public void SpawnHeadlights(GameObject headlightPrefab)
	{
		chassis.SpawnHeadlights(masterHeadlightAttachmentControllerPrefab, headlightPrefab);
	}

	public void SpawnLicensePlate(GameObject licensePlatePrefab)
	{
		chassis.SpawnLicensePlate(masterLicensePlateAttachmentControllerPrefab, licensePlatePrefab);
	}

	public void SpawnHoodOrnament(GameObject HoodOrnamentPrefab)
	{
		chassis.SpawnHoodOrnaments(masterHoodOrnamentAttacmentControllerPrefab, HoodOrnamentPrefab);
	}

	public void SpawnTrinketMount(GameObject[] initialObjects, WorldItemData worldItemData)
	{
		chassis.SpawnTrinketMount(masterTrinketMountPrefab, initialObjects, worldItemData);
	}

	public void StartDriveIn(float duration = 6f)
	{
		TexturePainterController.Instance.SetupTextures(baseCarTexture, detailCarTexture);
		TexturePainterController.Instance.SetTintColor(defaultColor);
		TexturePainterController.Instance.Refresh();
		StartCoroutine(DriveInAsync(duration));
	}

	private IEnumerator DriveInAsync(float duration = 6f)
	{
		SetDashboardState(CarInteriorDashboardHardware.PedalState.Drive);
		chassis.ForceCloseDoors(VehicleDoorTypes.All, true);
		base.transform.position = AutoMechanicManager.Instance.SafeSpawnLocation;
		if (drivingAudioOnEnter != null)
		{
			carDrivingAudioSource.SetClip(drivingAudioOnEnter);
			carDrivingAudioSource.SetLooping(true);
			carDrivingAudioSource.Play();
		}
		Animator chassisAnimator = chassis.GetComponent<Animator>();
		if (GenieManager.DoesContainGenieMode(GlobalStorage.Instance.CurrentGenieModes, JobGenieCartridge.GenieModeTypes.EndlessMode) && chassisAnimator != null)
		{
			base.transform.eulerAngles = new Vector3(0f, 180f, 0f);
			chassisAnimator.Play("Car_DriftIn");
			base.transform.position = AutoMechanicManager.Instance.DriveInPath.Waypoints[7].transform.position;
		}
		else
		{
			Go.to(base.transform, duration, new GoTweenConfig().positionPath(AutoMechanicManager.Instance.DriveInPath.GetPathAsGoSpline(true, false), false, GoLookAtType.NextPathNode).setEaseType(GoEaseType.QuadOut));
		}
		if (chassis.Dashboard != null)
		{
			chassis.Dashboard.StartEngineSound();
		}
		yield return new WaitForSeconds(0.5f);
		if (GenieManager.DoesContainGenieMode(GlobalStorage.Instance.CurrentGenieModes, JobGenieCartridge.GenieModeTypes.EndlessMode) && chassisAnimator != null)
		{
			duration = chassisAnimator.GetCurrentAnimatorStateInfo(0).length;
		}
		chassis.AnimateWheelsWithTween(duration - 1f, GoEaseType.QuadOut);
		yield return new WaitForSeconds(duration - 0.5f);
		carDrivingAudioSource.Stop();
		SetDashboardState(CarInteriorDashboardHardware.PedalState.Idle);
	}

	public void StartDriveOut(float duration = 6f)
	{
		StartCoroutine(DriveOutAsync(duration));
	}

	private IEnumerator DriveOutAsync(float duration = 6f)
	{
		SetDashboardState(CarInteriorDashboardHardware.PedalState.Drive);
		chassis.ForceCloseDoors();
		chassis.AnimateWheelsWithTween(duration, GoEaseType.SineIn);
		if (drivingAudioOnExit != null)
		{
			carDrivingAudioSource.SetClip(drivingAudioOnExit);
			carDrivingAudioSource.SetLooping(true);
			carDrivingAudioSource.Play();
		}
		chassis.CleanupAndPrepareToExit();
		if (GenieManager.DoesContainGenieMode(GlobalStorage.Instance.CurrentGenieModes, JobGenieCartridge.GenieModeTypes.EndlessMode) && chassis.GetComponent<Animator>() != null)
		{
			chassis.GetComponent<Animator>().Play("Car_DriftOut");
		}
		else
		{
			Go.to(base.transform, duration, new GoTweenConfig().positionPath(AutoMechanicManager.Instance.DriveOutPath.GetPathAsGoSpline(true, base.transform.position, false), false, GoLookAtType.NextPathNode).setEaseType(GoEaseType.SineIn));
		}
		yield return new WaitForSeconds(duration);
		if (chassis.Dashboard != null)
		{
			chassis.Dashboard.StopEngineSound();
		}
		carDrivingAudioSource.Stop();
		if (GenieManager.DoesContainGenieMode(GlobalStorage.Instance.CurrentGenieModes, JobGenieCartridge.GenieModeTypes.EndlessMode))
		{
			UnlinkThisCarFromCurrentEndlessTask();
		}
	}

	private void SetDashboardState(CarInteriorDashboardHardware.PedalState newState)
	{
		if (chassis.Dashboard != null)
		{
			chassis.Dashboard.CurrentPedalState = newState;
		}
	}

	public void ForceCloseDoors(VehicleDoorTypes doorsToClose = VehicleDoorTypes.All)
	{
		chassis.ForceCloseDoors(doorsToClose);
	}

	public void ForceOpenDoors(VehicleDoorTypes doorsToOpen = VehicleDoorTypes.All, bool unlock = true)
	{
		chassis.ForceOpenDoors(doorsToOpen, false, unlock);
	}

	public void AllowDoorOpening(VehicleDoorTypes doorsToAllow = VehicleDoorTypes.All)
	{
		chassis.AllowDoorOpening(doorsToAllow);
	}

	public void SetOptimizedState(bool state)
	{
		chassis.SetOptimizedState(state);
	}

	public void SetLockedStateOfItemsWithinCar(bool state)
	{
		chassis.SetLockedStateOfItemsWithinCar(state);
	}

	public void SetDetailTextureOnPaint(Texture newTexture)
	{
		detailTextureOnPaint = newTexture;
	}

	public void ChooseThisCarForEndlessTask(EndlessVehiclePartsOptions partsOptions)
	{
		isThisTheEndlessChosenOne = true;
		RandomizeChassisColor();
		EndlessDriversSeatUO.ManualChangeName("EndlessVehicleDriverSeat");
		EndlessDriversSeatUO.RemainRegisteredWhileInactive = true;
		ReparentDriversSeatUOEndless();
		AutoMechanicManager.Instance.StartCoroutine(FillWithPartsOnceTaskStarts(partsOptions));
	}

	private void ReparentDriversSeatUO()
	{
		if (chassis.DriverSeatUniqueObject.transform.parent != chassis.DriversSeatParent)
		{
			chassis.DriverSeatUniqueObject.transform.SetParent(chassis.DriversSeatParent);
			chassis.DriverSeatUniqueObject.transform.localPosition = Vector3.zero;
			chassis.DriverSeatUniqueObject.transform.localRotation = Quaternion.identity;
		}
	}

	private void ReparentDriversSeatUOEndless()
	{
		if (EndlessDriversSeatUO.transform.parent != chassis.DriversSeatParent)
		{
			EndlessDriversSeatUO.transform.SetParent(chassis.DriversSeatParent);
			endlessDriversSeatUO.transform.localPosition = Vector3.zero;
			endlessDriversSeatUO.transform.localRotation = Quaternion.identity;
		}
	}

	private void RandomizeChassisColor()
	{
		Color color = default(Color);
		color = UnityEngine.Random.ColorHSV(0f, 1f, 0.5f, 1f, 0.5f, 1f);
		EndlessVehiclePrefabIniter.VehicleBodyTextureInfo randomItemInArray = GetRandomItemInArray(chassisGroup.CarTextures);
		Texture baseTexture = randomItemInArray.BaseTexture;
		Texture detailTexture = randomItemInArray.DetailTexture;
		SetTextureSettings(color, baseTexture, detailTexture);
		SetHideDetailTextureOncePainted(hideDetailTextureOncePainted);
		SetSwapDetailTextureOncePainted(swapDetailTextureOncePainted);
	}

	public void UnlinkThisCarFromCurrentEndlessTask()
	{
		isThisTheEndlessChosenOne = false;
		AutoMechanicManager.Instance.StartCoroutine(DestroyAllPresentParts());
	}

	private IEnumerator DestroyAllPresentParts()
	{
		yield return AutoMechanicManager.Instance.StartCoroutine(KillAllWheels());
		chassis.DeregisterHardware(chassis.Engine);
		UnityEngine.Object.DestroyImmediate(chassis.Engine.gameObject);
		yield return null;
		yield return AutoMechanicManager.Instance.StartCoroutine(KillAllOrnaments());
		chassis.InteriorTrinketController.ClearAndDeleteAllItems(chassis);
		yield return null;
		if (!chassis.TrunkCanBeNukedWhenLeaving)
		{
			chassis.ClearTrunkOfItems();
		}
		yield return null;
		KillLicensePlate();
		yield return null;
		yield return AutoMechanicManager.Instance.StartCoroutine(KillAllHeadlights());
		UnityEngine.Object.DestroyImmediate(gasTankObj);
		yield return null;
		EnginePourCooldownController overheatFX = chassis.GetComponentInChildren<EnginePourCooldownController>(true);
		if (overheatFX != null)
		{
			UnityEngine.Object.Destroy(overheatFX.gameObject);
		}
		yield return null;
		chassis.DestroyLockedPickupables();
	}

	private void KillLicensePlate()
	{
		AttachablePoint componentInChildren = chassis.LicensePlateAttachPoint.GetComponentInChildren<AttachablePoint>();
		if (componentInChildren.AttachedObjects.Count > 0 && componentInChildren.AttachedObjects[0] != null)
		{
			AttachableObject attachableObject = componentInChildren.AttachedObjects[0];
			componentInChildren.Detach(attachableObject);
			attachableObject.ManuallyClearInRanges();
			UnityEngine.Object.DestroyImmediate(attachableObject.gameObject);
		}
	}

	private IEnumerator KillAllHeadlights()
	{
		for (int i = 0; i < chassis.HeadlightAttachPoints.Count; i++)
		{
			if (chassis.HeadlightAttachPoints[i].AttachedObjects.Count > 0 && chassis.HeadlightAttachPoints[i].AttachedObjects[0] != null)
			{
				AttachableObject attachedLight = chassis.HeadlightAttachPoints[i].AttachedObjects[0];
				chassis.HeadlightAttachPoints[i].Detach(attachedLight, false, true);
				attachedLight.ManuallyClearInRanges();
				UnityEngine.Object.DestroyImmediate(attachedLight.gameObject);
				yield return null;
			}
		}
	}

	private IEnumerator KillAllOrnaments()
	{
		for (int i = 0; i < chassis.HoodAttachPoints.Count; i++)
		{
			if (chassis.HoodAttachPoints[i].AttachedObjects.Count > 0 && chassis.HoodAttachPoints[i].AttachedObjects[0] != null)
			{
				AttachableObject attachedOrnament = chassis.HoodAttachPoints[i].AttachedObjects[0];
				chassis.HoodAttachPoints[i].Detach(attachedOrnament, false, true);
				attachedOrnament.ManuallyClearInRanges();
				UnityEngine.Object.DestroyImmediate(attachedOrnament.gameObject);
				yield return null;
			}
		}
	}

	private IEnumerator KillAllWheels()
	{
		List<WheelAttachmentManager> allWheelManagers = chassis.GetAllWheelAttachmentManagers();
		for (int i = 0; i < allWheelManagers.Count; i++)
		{
			allWheelManagers[i].SetFlatFX(false);
			UnityEngine.Object.Destroy(allWheelManagers[i].gameObject);
			yield return null;
		}
		allWheelManagers.Clear();
	}

	public IEnumerator FillWithPartsOnceTaskStarts(EndlessVehiclePartsOptions partsOptions)
	{
		TaskStatusController task = JobBoardManager.instance.EndlessModeStatusController.GetCurrentGoal();
		while (task == null && !base.gameObject.activeInHierarchy)
		{
			yield return null;
		}
		partsOptions.chosenLicensePlatePrefab = null;
		partsOptions.chosenWheel = null;
		partsOptions.chosenHeadlightPrefab = null;
		bool removeOverheatPFX = true;
		partsOptions.isFilterSetDirty = false;
		partsOptions.enginePrefabIniter.useBadBattery = false;
		partsOptions.enginePrefabIniter.useBadPistons = false;
		partsOptions.enginePrefabIniter.useNoPistons = false;
		for (int k = 0; k < task.PageStatusControllerList.Count; k++)
		{
			PageData page = task.PageStatusControllerList[k].Data;
			int index9 = Array.IndexOf(partsOptions.sketchyPlatesPagesOptions, page);
			if (index9 > -1)
			{
				partsOptions.chosenLicensePlatePrefab = GetRandomItemInArray(partsOptions.sketchyPlatePrefabsOptions);
			}
			index9 = Array.IndexOf(partsOptions.brokenTiresPagesOptions, page);
			if (index9 > -1)
			{
				partsOptions.chosenWheel = GetRandomItemInArray(partsOptions.brokenTirePrefabsOptions);
				for (int m = 0; m < partsOptions.wheelPrefabsList.Length; m++)
				{
					partsOptions.wheelPrefabsList[m] = partsOptions.chosenWheel;
				}
			}
			index9 = Array.IndexOf(partsOptions.brokenHeadlightsPagesOptions, page);
			if (index9 > -1)
			{
				partsOptions.chosenHeadlightPrefab = GetRandomItemInArray(partsOptions.brokenHeadlightPrefabsOptions);
			}
			index9 = Array.IndexOf(partsOptions.badAirFilterPagesOptions, page);
			if (index9 > -1)
			{
				partsOptions.isFilterSetDirty = true;
			}
			index9 = Array.IndexOf(partsOptions.badBatteryPagesOptions, page);
			if (index9 > -1)
			{
				partsOptions.enginePrefabIniter.useBadBattery = true;
			}
			index9 = Array.IndexOf(partsOptions.brokenPistonsPagesOptions, page);
			if (index9 > -1)
			{
				partsOptions.enginePrefabIniter.useBadPistons = true;
			}
			index9 = Array.IndexOf(partsOptions.noPistonsPagesOptions, page);
			if (index9 > -1)
			{
				partsOptions.enginePrefabIniter.useNoPistons = true;
			}
			index9 = Array.IndexOf(partsOptions.engineOilFluidPagesOptions, page);
			if (index9 > -1)
			{
				partsOptions.initialFluidPercent = 0f;
			}
			index9 = Array.IndexOf(partsOptions.engineOverheatPagesOptions, page);
			if (index9 > -1)
			{
				partsOptions.extraHardwareOptions.Add(partsOptions.overheatObject);
				removeOverheatPFX = false;
			}
			yield return null;
		}
		if (removeOverheatPFX && partsOptions.extraHardwareOptions.Contains(partsOptions.overheatObject))
		{
			partsOptions.extraHardwareOptions.Remove(partsOptions.overheatObject);
		}
		if (partsOptions.chosenLicensePlatePrefab == null)
		{
			partsOptions.chosenLicensePlatePrefab = GetRandomItemInArray(partsOptions.licensePlatePrefabOptions);
		}
		if (partsOptions.chosenWheel == null)
		{
			partsOptions.chosenWheel = GetRandomItemInArray(partsOptions.wheelOptionsList);
			for (int l = 0; l < partsOptions.wheelPrefabsList.Length; l++)
			{
				partsOptions.wheelPrefabsList[l] = partsOptions.chosenWheel;
			}
		}
		if (partsOptions.chosenHeadlightPrefab == null)
		{
			partsOptions.chosenHeadlightPrefab = GetRandomItemInArray(partsOptions.chassisGroup.InitialHeadlightPrefabs);
		}
		GameObject engine = SpawnExtraHardware(partsOptions.enginePrefabIniter.gameObject, VehicleChassisController.ChassisHardwareMountPoint.Engine);
		yield return null;
		SpawnWheels(partsOptions.wheelPrefabsList, partsOptions.wheelIndexesToStartDeflated);
		yield return null;
		SpawnHeadlights(partsOptions.chosenHeadlightPrefab);
		yield return null;
		SpawnLicensePlate(partsOptions.chosenLicensePlatePrefab);
		yield return null;
		gasTankObj = SpawnExtraHardware(GetRandomItemInArray(chassisGroup.GasTankPrefabs), VehicleChassisController.ChassisHardwareMountPoint.GasTank);
		yield return null;
		SetDetailTextureOnPaint(partsOptions.detailCarTexture);
		yield return null;
		for (int j = 0; j < partsOptions.extraHardwareOptions.Count; j++)
		{
			partsOptions.extraHardwareOptions[j].SpawnOnVehicle(this);
			yield return null;
		}
		for (int i = 0; i < partsOptions.extraHardwaveNonPSVROnlyOptions.Length; i++)
		{
			partsOptions.extraHardwaveNonPSVROnlyOptions[i].SpawnOnVehicle(this);
			yield return null;
		}
		if (partsOptions.isFilterSetDirty)
		{
			engine.GetComponentInChildren<VehicleAirFilter>().SetDirty();
		}
		DipstickController dipstickController = engine.GetComponentInChildren<DipstickController>();
		VehicleEngineHardware engineHardware = engine.GetComponentInChildren<VehicleEngineHardware>();
		if (dipstickController != null && engineHardware != null)
		{
			dipstickController.SetEnginePFXParent(engineHardware.GetEnginePFXContainer());
		}
		else
		{
			Debug.Log("Could not get dipstickController or engineHardware");
		}
		if (partsOptions.needToSetFluidColor && dipstickController != null)
		{
			dipstickController.ChangeInitialFluid(partsOptions.initialFluidData, partsOptions.initialFluidPercent);
		}
		SetLockedStateOfItemsWithinCar(true);
	}

	private T GetRandomItemInArray<T>(T[] items)
	{
		if (items == null || items.Length == 0)
		{
			Debug.LogWarning("Passed empty or null array to GetRandomItemInArray. If you did not forget to set values, ignore this message.");
			return default(T);
		}
		return items[UnityEngine.Random.Range(0, items.Length)];
	}
}
