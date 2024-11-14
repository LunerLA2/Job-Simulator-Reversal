using System;
using System.Collections;
using UnityEngine;

public class AutoMechanicManager : BrainControlledObject
{
	[Serializable]
	public struct CarTaskPair
	{
		public GameObject carPrefab;

		public TaskData[] tasksToBringInThisCar;
	}

	private static AutoMechanicManager instance;

	[SerializeField]
	private UniqueObject endlessDriversSeatUOPrefab;

	[SerializeField]
	private static VehicleController currentVehicle;

	[SerializeField]
	private Transform safeSpawnLocation;

	[SerializeField]
	[Header("Controllers")]
	private ProfitCounterController profitCounterController;

	[SerializeField]
	[Header("Paths")]
	private BotPath driveInPath;

	[SerializeField]
	private BotPath driveOutPath;

	[SerializeField]
	[Header("ProgressionEvents")]
	private JobData jobData;

	[SerializeField]
	protected bool ALWAYS_LOAD_ENDLESS_MODE;

	[SerializeField]
	protected string endlessModeConfigDataName;

	protected EndlessModeData endlessModeConfigData;

	[SerializeField]
	[Header("Vehicles")]
	private CarTaskPair[] carTaskPairs;

	[SerializeField]
	private GameObject vehiclePrefabForSandboxMode;

	[SerializeField]
	private GameObject vehiclePrefabForEndlessMode;

	[SerializeField]
	private EndlessCarPool endlessCarPool;

	private VehicleController[] spawnedCarsRelatingToTaskPairs;

	private VehicleController spawnedCarForSandboxMode;

	private bool hasBroughtInSandboxCar;

	private LiftController liftController;

	private VehicleController spawnedCarForEndlessMode;

	private VehicleController lastVehicle;

	public static AutoMechanicManager Instance
	{
		get
		{
			return instance;
		}
	}

	public static VehicleController CurrentVehicle
	{
		get
		{
			return currentVehicle;
		}
	}

	public Vector3 SafeSpawnLocation
	{
		get
		{
			return safeSpawnLocation.position;
		}
	}

	public ProfitCounterController ProfitCounterController
	{
		get
		{
			return profitCounterController;
		}
	}

	public BotPath DriveInPath
	{
		get
		{
			return driveInPath;
		}
	}

	public BotPath DriveOutPath
	{
		get
		{
			return driveOutPath;
		}
	}

	private void Awake()
	{
		if (ALWAYS_LOAD_ENDLESS_MODE || GenieManager.DoesContainGenieMode(GlobalStorage.Instance.CurrentGenieModes, JobGenieCartridge.GenieModeTypes.EndlessMode))
		{
			endlessModeConfigData = Resources.Load(endlessModeConfigDataName) as EndlessModeData;
			if (endlessModeConfigData == null)
			{
				Debug.LogError("Endless mode was specified but no EndlessModeConfigData is specified or the asset name given to " + base.gameObject.name + " to load is wrong.", base.gameObject);
			}
		}
		if (!instance)
		{
			instance = this;
		}
		if (instance != this)
		{
			UnityEngine.Object.Destroy(this);
		}
		for (int i = 0; i < carTaskPairs.Length; i++)
		{
			if (carTaskPairs[i].carPrefab == null)
			{
				Debug.LogError("A Car is not assigned in the Car Task Pair array on " + base.name);
				return;
			}
		}
		spawnedCarsRelatingToTaskPairs = new VehicleController[carTaskPairs.Length];
		for (int j = 0; j < carTaskPairs.Length; j++)
		{
			string text = string.Empty;
			for (int k = 0; k < carTaskPairs[j].tasksToBringInThisCar.Length; k++)
			{
				text = text + "_" + carTaskPairs[j].tasksToBringInThisCar[k].ShortTaskName;
			}
			spawnedCarsRelatingToTaskPairs[j] = SpawnAndPreloadVehicle(carTaskPairs[j].carPrefab, text);
		}
		spawnedCarForSandboxMode = SpawnAndPreloadVehicle(vehiclePrefabForSandboxMode, "SandboxMode");
	}

	public static VehicleController SpawnAndPreloadVehicle(GameObject vehiclePrefab, string generatedName, EndlessVehiclePrefabIniter endlessVehicleOptions = null, EndlessVehiclePrefabIniter.VehicleChassisGroup chassis = null)
	{
		if (chassis != null && endlessVehicleOptions != null && GenieManager.DoesContainGenieMode(GlobalStorage.Instance.CurrentGenieModes, JobGenieCartridge.GenieModeTypes.EndlessMode))
		{
			return SpawnEndlessVehicle(vehiclePrefab, endlessVehicleOptions, chassis);
		}
		GameObject gameObject = UnityEngine.Object.Instantiate(vehiclePrefab);
		BasePrefabSpawner component = gameObject.GetComponent<BasePrefabSpawner>();
		if (component != null)
		{
			gameObject = component.LastSpawnedPrefabGO;
		}
		VehicleController component2 = gameObject.GetComponent<VehicleController>();
		component2.SetOptimizedState(true);
		gameObject.SetActive(false);
		gameObject.transform.SetParent(GlobalStorage.Instance.ContentRoot);
		gameObject.name = "GeneratedCar:" + generatedName;
		return component2;
	}

	private static VehicleController SpawnEndlessVehicle(GameObject vehiclePrefab, EndlessVehiclePrefabIniter endlessVehicleOptions, EndlessVehiclePrefabIniter.VehicleChassisGroup chassis)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(vehiclePrefab);
		gameObject.name = "Endless " + chassis.ChassisPrefab.name;
		VehicleController component = gameObject.GetComponent<VehicleController>();
		component.endlessDriversSeatPrefab = instance.endlessDriversSeatUOPrefab;
		component.ChassisGroup = chassis;
		VehicleChassisController chassisPrefab = chassis.ChassisPrefab;
		VehicleChassisController chassisPrefabPSVR = chassis.ChassisPrefabPSVR;
		EndlessVehiclePrefabIniter.VehicleInterior randomItemInArray = GetRandomItemInArray(chassis.Interiors);
		GameObject interiorPrefab = randomItemInArray.InteriorPrefab;
		GameObject pSVRInteriorPrefab = randomItemInArray.PSVRInteriorPrefab;
		Color color = default(Color);
		color = UnityEngine.Random.ColorHSV(0f, 1f, 0.5f, 1f, 0.5f, 1f);
		EndlessVehiclePrefabIniter.VehicleBodyTextureInfo randomItemInArray2 = GetRandomItemInArray(chassis.CarTextures);
		Texture baseTexture = randomItemInArray2.BaseTexture;
		Texture detailTexture = randomItemInArray2.DetailTexture;
		bool hideDetailTextureOncePainted = randomItemInArray2.HideDetailTextureOncePainted;
		bool swapDetailTextureOncePainted = randomItemInArray2.SwapDetailTextureOncePainted;
		AudioClip randomItemInArray3 = GetRandomItemInArray(endlessVehicleOptions.optionalDriveInSounds);
		AudioClip randomItemInArray4 = GetRandomItemInArray(endlessVehicleOptions.optionalDriveOutSounds);
		component.SetTextureSettings(color, baseTexture, detailTexture);
		component.SetHideDetailTextureOncePainted(hideDetailTextureOncePainted);
		component.SetSwapDetailTextureOncePainted(swapDetailTextureOncePainted);
		component.SetupDrivingAudio(randomItemInArray3, randomItemInArray4);
		component.SpawnChassis(chassisPrefab, endlessVehicleOptions.driverSeatUniqueObjectName, endlessVehicleOptions.customDoorData, endlessVehicleOptions.customHoodData, endlessVehicleOptions.customTrunkData, endlessVehicleOptions.vehicleWorldItemData);
		component.SpawnExtraHardware(interiorPrefab, VehicleChassisController.ChassisHardwareMountPoint.Interior);
		chassisPrefab.underLightColor = UnityEngine.Random.ColorHSV(0f, 1f, 0.7f, 1f, 0.5f, 1f, 0.3f, 0.6f);
		endlessVehicleOptions.enginePrefab.useBadBattery = false;
		endlessVehicleOptions.enginePrefab.useBadPistons = false;
		endlessVehicleOptions.enginePrefab.useNoPistons = false;
		GameObject[] array = new GameObject[4];
		GameObject randomItemInArray5 = GetRandomItemInArray(endlessVehicleOptions.wheelOptions);
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = randomItemInArray5;
		}
		GameObject randomItemInArray6 = GetRandomItemInArray(chassis.InitialHeadlightPrefabs);
		GameObject randomItemInArray7 = GetRandomItemInArray(endlessVehicleOptions.licensePlatePrefabs);
		GameObject randomItemInArray8 = GetRandomItemInArray(endlessVehicleOptions.hoodOrnamentPrefabs);
		GameObject randomItemInArray9 = GetRandomItemInArray(chassis.GasTankPrefabs);
		Texture detailTextureOnPaint = randomItemInArray2.DetailTextureOnPaint;
		VehicleController.EndlessVehiclePartsOptions endlessVehiclePartsOptions = new VehicleController.EndlessVehiclePartsOptions();
		endlessVehiclePartsOptions.chassisGroup = chassis;
		endlessVehiclePartsOptions.wheelPrefabsList = array;
		endlessVehiclePartsOptions.chosenWheel = randomItemInArray5;
		endlessVehiclePartsOptions.chosenHeadlightPrefab = randomItemInArray6;
		endlessVehiclePartsOptions.chosenLicensePlatePrefab = randomItemInArray7;
		endlessVehiclePartsOptions.chosenHoodOrnamentPrefab = randomItemInArray8;
		endlessVehiclePartsOptions.chosenGasTankPrefab = randomItemInArray9;
		endlessVehiclePartsOptions.detailCarTexture = detailTextureOnPaint;
		endlessVehiclePartsOptions.sketchyPlatesPagesOptions = endlessVehicleOptions.sketchyPlatesPages;
		endlessVehiclePartsOptions.sketchyPlatePrefabsOptions = endlessVehicleOptions.sketchyPlatePrefabs;
		endlessVehiclePartsOptions.brokenTiresPagesOptions = endlessVehicleOptions.brokenTiresPages;
		endlessVehiclePartsOptions.brokenTirePrefabsOptions = endlessVehicleOptions.brokenTirePrefabs;
		endlessVehiclePartsOptions.brokenHeadlightsPagesOptions = endlessVehicleOptions.brokenHeadlightsPages;
		endlessVehiclePartsOptions.brokenHeadlightPrefabsOptions = endlessVehicleOptions.brokenHeadlightPrefabs;
		endlessVehiclePartsOptions.badAirFilterPagesOptions = endlessVehicleOptions.badAirFilterPages;
		endlessVehiclePartsOptions.badBatteryPagesOptions = endlessVehicleOptions.badBatteryPages;
		endlessVehiclePartsOptions.brokenPistonsPagesOptions = endlessVehicleOptions.brokenPistonsPages;
		endlessVehiclePartsOptions.noPistonsPagesOptions = endlessVehicleOptions.noPistonsPages;
		endlessVehiclePartsOptions.engineOilFluidPagesOptions = endlessVehicleOptions.engineOilFluidPages;
		endlessVehiclePartsOptions.engineOverheatPagesOptions = endlessVehicleOptions.engineOverheatPages;
		endlessVehiclePartsOptions.overheatObject = endlessVehicleOptions.overheatPrefab;
		endlessVehiclePartsOptions.enginePrefabIniter = endlessVehicleOptions.enginePrefab;
		endlessVehiclePartsOptions.wheelOptionsList = endlessVehicleOptions.wheelOptions;
		endlessVehiclePartsOptions.wheelIndexesToStartDeflated = endlessVehicleOptions.wheelIndexesThatStartDeflated;
		endlessVehiclePartsOptions.licensePlatePrefabOptions = endlessVehicleOptions.licensePlatePrefabs;
		endlessVehiclePartsOptions.hoodOrnamentPrefabsOptions = endlessVehicleOptions.hoodOrnamentPrefabs;
		endlessVehiclePartsOptions.driverSeatUOName = endlessVehicleOptions.driverSeatUniqueObjectName;
		endlessVehiclePartsOptions.optionalDriveInSoundsList = endlessVehicleOptions.optionalDriveInSounds;
		endlessVehiclePartsOptions.optionalDriveOutSoundsList = endlessVehicleOptions.optionalDriveOutSounds;
		endlessVehiclePartsOptions.isFilterSetDirty = endlessVehicleOptions.isFilterDirty;
		endlessVehiclePartsOptions.needToSetFluidColor = endlessVehicleOptions.setFluidColor;
		endlessVehiclePartsOptions.initialFluidData = endlessVehicleOptions.initialFluid;
		endlessVehiclePartsOptions.initialFluidPercent = endlessVehicleOptions.initialFluidPercentFull;
		endlessVehiclePartsOptions.defaultTrinketObjectsOptions = endlessVehicleOptions.defaultTrinketObjects;
		endlessVehiclePartsOptions.extraHardwareOptions = endlessVehicleOptions.extraHardware;
		endlessVehiclePartsOptions.extraHardwarePSVROnlyOptions = endlessVehicleOptions.extraHardwarePSVROnly;
		endlessVehiclePartsOptions.extraHardwaveNonPSVROnlyOptions = endlessVehicleOptions.extraHardwaveNonPSVROnly;
		endlessVehiclePartsOptions.vehicleWorldItem = endlessVehicleOptions.vehicleWorldItemData;
		endlessVehiclePartsOptions.customHoodWorldItemData = endlessVehicleOptions.customHoodData;
		endlessVehiclePartsOptions.customDoorWorldItemData = endlessVehicleOptions.customDoorData;
		endlessVehiclePartsOptions.customTrunkWorldItemData = endlessVehicleOptions.customTrunkData;
		endlessVehiclePartsOptions.interiorTrinketMountWorldItemData = endlessVehicleOptions.interiorTrinketMountData;
		VehicleController.EndlessVehiclePartsOptions endlessVehiclePartsOptions2 = (EndlessCarPool.partOptions = endlessVehiclePartsOptions);
		component.SpawnTrinketMount(endlessVehiclePartsOptions2.defaultTrinketObjectsOptions, endlessVehiclePartsOptions2.interiorTrinketMountWorldItemData);
		return component;
	}

	private static T GetRandomItemInArray<T>(T[] items)
	{
		if (items == null || items.Length == 0)
		{
			Debug.LogWarning("Passed empty or null array to GetRandomItemInArray. If you did not forget to set values, ignore this message.");
			return default(T);
		}
		return items[UnityEngine.Random.Range(0, items.Length)];
	}

	private IEnumerator Start()
	{
		bool isInEndlessMode = false;
		if (GenieManager.DoesContainGenieMode(GlobalStorage.Instance.CurrentGenieModes, JobGenieCartridge.GenieModeTypes.EndlessMode))
		{
			isInEndlessMode = true;
		}
		if (isInEndlessMode)
		{
			yield return null;
			RenderSettings.ambientLight = new Color(0.5f, 0.4f, 0.49f);
			JobStatusController jobStatusController = new JobStatusController(jobData);
			BotManager.Instance.InitializeEndlessMode(endlessModeConfigData);
			JobBoardManager.instance.InitEndlessMode(endlessModeConfigData, jobStatusController.JobStateData);
		}
		else
		{
			BotManager.Instance.InitializeJob(jobData);
			JobBoardManager.instance.InitJob(jobData);
		}
	}

	public void RegisterLiftController(LiftController lift)
	{
		if (liftController != null)
		{
			LiftController obj = liftController;
			obj.OnVehicleWasDropped = (Action<VehicleController>)Delegate.Remove(obj.OnVehicleWasDropped, new Action<VehicleController>(VehicleWasDropped));
		}
		liftController = lift;
		LiftController obj2 = liftController;
		obj2.OnVehicleWasDropped = (Action<VehicleController>)Delegate.Combine(obj2.OnVehicleWasDropped, new Action<VehicleController>(VehicleWasDropped));
	}

	private void OnEnable()
	{
		JobBoardManager jobBoardManager = JobBoardManager.instance;
		jobBoardManager.OnTaskStarted = (Action<TaskStatusController>)Delegate.Combine(jobBoardManager.OnTaskStarted, new Action<TaskStatusController>(TaskStarted));
	}

	private void OnDisable()
	{
		JobBoardManager jobBoardManager = JobBoardManager.instance;
		jobBoardManager.OnTaskStarted = (Action<TaskStatusController>)Delegate.Remove(jobBoardManager.OnTaskStarted, new Action<TaskStatusController>(TaskStarted));
		if (liftController != null)
		{
			LiftController obj = liftController;
			obj.OnVehicleWasDropped = (Action<VehicleController>)Delegate.Remove(obj.OnVehicleWasDropped, new Action<VehicleController>(VehicleWasDropped));
		}
	}

	private void TaskStarted(TaskStatusController status)
	{
		for (int i = 0; i < carTaskPairs.Length; i++)
		{
			if (Array.IndexOf(carTaskPairs[i].tasksToBringInThisCar, status.Data) > -1)
			{
				if (spawnedCarsRelatingToTaskPairs[i] == null)
				{
					Debug.LogError(carTaskPairs[i].carPrefab.name + " wasn't pooled when the job started, can't use!");
				}
				StartCoroutine(BringInNewVehicle(spawnedCarsRelatingToTaskPairs[i]));
				break;
			}
		}
		if (GenieManager.AreAnyJobGenieModesActive() && GenieManager.DoesContainGenieMode(GlobalStorage.Instance.CurrentGenieModes, JobGenieCartridge.GenieModeTypes.EndlessMode) && JobBoardManager.instance.EndlessModeStatusController.GetCurrentGoal() != null && !JobBoardManager.instance.EndlessModeStatusController.GetCurrentGoal().IsCompleted)
		{
			spawnedCarForEndlessMode = endlessCarPool.GetRandomCar();
			StartCoroutine(BringInNewVehicle(spawnedCarForEndlessMode));
		}
	}

	public void ChainWasPulledInEndlessMode()
	{
		StartCoroutine(BringInEndlessModeVehicle());
	}

	private IEnumerator BringInEndlessModeVehicle()
	{
		if (currentVehicle != null)
		{
			liftController.SetDropButtonCoverOpen(false);
			if (liftController.DropCoroutine == null)
			{
				liftController.DropCar();
			}
			while (liftController.DropCoroutine != null)
			{
				yield return null;
			}
			if (GenieManager.AreAnyJobGenieModesActive() && GenieManager.DoesContainGenieMode(GlobalStorage.Instance.CurrentGenieModes, JobGenieCartridge.GenieModeTypes.EndlessMode) && JobBoardManager.instance.EndlessModeStatusController.GetCurrentGoal() != null && !JobBoardManager.instance.EndlessModeStatusController.GetCurrentGoal().IsCompleted)
			{
				JobBoardManager.instance.EndlessModeStatusController.ForceJobComplete(false, true);
			}
		}
	}

	public void ChainWasPulledInSandboxMode()
	{
		if (!hasBroughtInSandboxCar)
		{
			hasBroughtInSandboxCar = true;
			Debug.Log("job completed, bringing in sandbox car");
			StartCoroutine(BringInSandboxModeVehicle());
		}
	}

	private IEnumerator BringInSandboxModeVehicle()
	{
		StartCoroutine(BringInNewVehicle(spawnedCarForSandboxMode));
		yield return new WaitForSeconds(7.5f);
		if (liftController != null)
		{
			liftController.LiftVehicle();
		}
	}

	public override void ScriptedEffect(BrainEffect effect)
	{
		if (effect.TextInfo.ToLower() == "drop")
		{
			if (liftController != null)
			{
				liftController.DropCar();
			}
			else
			{
				Debug.LogError("Can't drop without reference to a LiftController");
			}
		}
		else if (effect.TextInfo.ToLower() == "lift")
		{
			if (liftController != null)
			{
				liftController.LiftVehicle();
			}
			else
			{
				Debug.LogError("Can't lift without reference to a LiftController");
			}
		}
		else if (effect.TextInfo.ToLower() == "exit")
		{
			StartCoroutine(VehicleDrivesOut(currentVehicle));
		}
		else if (effect.TextInfo.ToLower() == "opendriverdoor")
		{
			if (currentVehicle != null)
			{
				currentVehicle.ForceOpenDoors(VehicleDoorTypes.DriverDoor, false);
			}
			else
			{
				Debug.LogError("Can't do an OpenDoor because currentVehicle is null");
			}
		}
		else if (effect.TextInfo.ToLower() == "openpassengerdoor")
		{
			if (currentVehicle != null)
			{
				currentVehicle.ForceOpenDoors(VehicleDoorTypes.PassengerDoor);
			}
			else
			{
				Debug.LogError("Can't do an OpenDoor because currentVehicle is null");
			}
		}
		else if (effect.TextInfo.ToLower() == "closealldoors")
		{
			if (currentVehicle != null)
			{
				currentVehicle.ForceCloseDoors();
			}
			else
			{
				Debug.LogError("Can't do a CloseDoors because currentVehicle is null");
			}
		}
		else if (effect.TextInfo.ToLower() == "destroylastvehicle")
		{
			if (lastVehicle != null)
			{
				UnityEngine.Object.Destroy(lastVehicle.gameObject);
			}
			else
			{
				Debug.LogError("Can't do a Destroy because lastVehicle is null");
			}
		}
		else if (effect.TextInfo.ToLower() == "skiptaskiftasknotcomplete")
		{
			if (JobBoardManager.instance != null && JobBoardManager.instance.EndlessModeStatusController.GetCurrentGoal() != null && !JobBoardManager.instance.EndlessModeStatusController.GetCurrentGoal().IsCompleted)
			{
				StartCoroutine(BringInEndlessModeVehicle());
			}
		}
		else
		{
			Debug.LogError("Unrecognized effect for AutoMechanicManager: \"" + effect.TextInfo + "\"");
		}
	}

	private IEnumerator BringInNewVehicle(VehicleController vehicle)
	{
		while (currentVehicle != null)
		{
			yield return new WaitForEndOfFrame();
		}
		currentVehicle = vehicle;
		currentVehicle.gameObject.SetActive(true);
		vehicle.StartDriveIn();
	}

	private void VehicleWasDropped(VehicleController vehicle)
	{
		if (vehicle != currentVehicle)
		{
			Debug.LogError("Dropped a car that was not the CurrentCar, something is broken.");
		}
		GameEventsManager.Instance.ScriptedCauseOccurred("CarDropped");
	}

	private IEnumerator VehicleDrivesOut(VehicleController vehicle)
	{
		vehicle.ForceCloseDoors();
		vehicle.StartDriveOut(4.5f);
		yield return new WaitForSeconds(4.5f);
		vehicle.gameObject.SetActive(false);
		lastVehicle = vehicle;
		currentVehicle = null;
	}
}
