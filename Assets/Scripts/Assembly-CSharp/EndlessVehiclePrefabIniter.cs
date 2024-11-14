using System;
using System.Collections.Generic;
using OwlchemyVR;
using UnityEngine;

public class EndlessVehiclePrefabIniter : MonoBehaviourPrefabIniter
{
	[Serializable]
	public class VehicleChassisGroup
	{
		[SerializeField]
		private VehicleChassisController chassisPrefab;

		[SerializeField]
		private VehicleChassisController chassisPrefabPSVR;

		[SerializeField]
		private VehicleBodyTextureInfo[] carTextures;

		[SerializeField]
		private VehicleInterior[] interiors;

		[SerializeField]
		private GameObject[] gasTankPrefabs;

		[SerializeField]
		private GameObject[] initialHeadlightPrefabs;

		public VehicleChassisController ChassisPrefab
		{
			get
			{
				return chassisPrefab;
			}
		}

		public VehicleChassisController ChassisPrefabPSVR
		{
			get
			{
				return chassisPrefabPSVR;
			}
		}

		public VehicleBodyTextureInfo[] CarTextures
		{
			get
			{
				return carTextures;
			}
		}

		public VehicleInterior[] Interiors
		{
			get
			{
				return interiors;
			}
		}

		public GameObject[] GasTankPrefabs
		{
			get
			{
				return gasTankPrefabs;
			}
		}

		public GameObject[] InitialHeadlightPrefabs
		{
			get
			{
				return initialHeadlightPrefabs;
			}
		}
	}

	[Serializable]
	public class VehicleBodyTextureInfo
	{
		[SerializeField]
		private Texture baseTexture;

		[SerializeField]
		private Texture detailTexture;

		[SerializeField]
		private bool hideDetailTextureOncePainted;

		[SerializeField]
		private bool swapDetailTextureOncePainted;

		[SerializeField]
		private Texture detailTextureOnPaint;

		public Texture BaseTexture
		{
			get
			{
				return baseTexture;
			}
		}

		public Texture DetailTexture
		{
			get
			{
				return detailTexture;
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

		public Texture DetailTextureOnPaint
		{
			get
			{
				return detailTextureOnPaint;
			}
		}
	}

	[Serializable]
	public class VehicleInterior
	{
		[SerializeField]
		private GameObject interiorPrefab;

		[SerializeField]
		private GameObject psvrInteriorPrefab;

		public GameObject InteriorPrefab
		{
			get
			{
				return interiorPrefab;
			}
		}

		public GameObject PSVRInteriorPrefab
		{
			get
			{
				return psvrInteriorPrefab;
			}
		}
	}

	[SerializeField]
	public VehicleController masterVehiclePrefab;

	[SerializeField]
	[Header("Chassis")]
	public VehicleChassisGroup[] chassisOptions;

	[SerializeField]
	public EndlessEnginePrefabIniter enginePrefab;

	[SerializeField]
	[Header("WorldItem Settings")]
	public WorldItemData vehicleWorldItemData;

	[SerializeField]
	public WorldItemData customHoodData;

	[SerializeField]
	public WorldItemData customDoorData;

	[SerializeField]
	public WorldItemData customTrunkData;

	[SerializeField]
	public WorldItemData interiorTrinketMountData;

	[SerializeField]
	[Tooltip("Prefabs you specify will be looped through until all wheel attachments are filled.")]
	[Header("Core Hardware")]
	public GameObject[] wheelOptions;

	[SerializeField]
	public int[] wheelIndexesThatStartDeflated;

	[SerializeField]
	public GameObject[] licensePlatePrefabs;

	[SerializeField]
	public GameObject[] hoodOrnamentPrefabs;

	[SerializeField]
	[Header("Misc Settings")]
	public string driverSeatUniqueObjectName = string.Empty;

	[SerializeField]
	public AudioClip[] optionalDriveInSounds;

	[SerializeField]
	public AudioClip[] optionalDriveOutSounds;

	public bool isFilterDirty;

	[SerializeField]
	public bool setFluidColor;

	[SerializeField]
	public WorldItemData initialFluid;

	public float initialFluidPercentFull = 0.75f;

	[SerializeField]
	public GameObject[] defaultTrinketObjects;

	[SerializeField]
	[Header("Additional Hardware")]
	public List<VehicleExtraHardwarePiece> extraHardware;

	[SerializeField]
	public VehicleExtraHardwarePiece[] extraHardwarePSVROnly;

	[SerializeField]
	public VehicleExtraHardwarePiece[] extraHardwaveNonPSVROnly;

	[Header("Pages That Change Car")]
	[SerializeField]
	public PageData[] sketchyPlatesPages;

	[SerializeField]
	public GameObject[] sketchyPlatePrefabs;

	[Space]
	[SerializeField]
	public PageData[] brokenTiresPages;

	[SerializeField]
	public GameObject[] brokenTirePrefabs;

	[Space]
	[SerializeField]
	public PageData[] brokenHeadlightsPages;

	[SerializeField]
	public GameObject[] brokenHeadlightPrefabs;

	[SerializeField]
	[Space]
	public PageData[] badAirFilterPages;

	[Space]
	[SerializeField]
	public PageData[] badBatteryPages;

	[Space]
	[SerializeField]
	public PageData[] brokenPistonsPages;

	[Space]
	[SerializeField]
	public PageData[] noPistonsPages;

	[Space]
	[SerializeField]
	public PageData[] engineOilFluidPages;

	[Space]
	[SerializeField]
	public PageData[] engineOverheatPages;

	[SerializeField]
	public VehicleExtraHardwarePiece overheatPrefab;

	public GameObject engine;

	public Transform enginePFX;

	public VehicleChassisGroup[] ChassisOptions
	{
		get
		{
			return chassisOptions;
		}
	}

	public override MonoBehaviour GetPrefab()
	{
		return masterVehiclePrefab;
	}

	public override void Init(MonoBehaviour spawnedPrefab)
	{
		VehicleController vehicleController = spawnedPrefab as VehicleController;
		VehicleChassisGroup randomItemInArray = GetRandomItemInArray(ChassisOptions);
		for (int i = 0; i < chassisOptions.Length; i++)
		{
			if (chassisOptions[i].ChassisPrefab.name == randomItemInArray.ChassisPrefab.name)
			{
				vehicleController.ChassisGroup = ChassisOptions[i];
			}
		}
		VehicleChassisController chassisPrefab = randomItemInArray.ChassisPrefab;
		VehicleChassisController chassisPrefabPSVR = randomItemInArray.ChassisPrefabPSVR;
		VehicleInterior randomItemInArray2 = GetRandomItemInArray(randomItemInArray.Interiors);
		GameObject interiorPrefab = randomItemInArray2.InteriorPrefab;
		GameObject pSVRInteriorPrefab = randomItemInArray2.PSVRInteriorPrefab;
		Color color = default(Color);
		color = UnityEngine.Random.ColorHSV(0f, 1f, 0.5f, 1f, 0.5f, 1f);
		VehicleBodyTextureInfo randomItemInArray3 = GetRandomItemInArray(randomItemInArray.CarTextures);
		Texture baseTexture = randomItemInArray3.BaseTexture;
		Texture detailTexture = randomItemInArray3.DetailTexture;
		bool hideDetailTextureOncePainted = randomItemInArray3.HideDetailTextureOncePainted;
		bool swapDetailTextureOncePainted = randomItemInArray3.SwapDetailTextureOncePainted;
		AudioClip randomItemInArray4 = GetRandomItemInArray(optionalDriveInSounds);
		AudioClip randomItemInArray5 = GetRandomItemInArray(optionalDriveOutSounds);
		vehicleController.SetTextureSettings(color, baseTexture, detailTexture);
		vehicleController.SetHideDetailTextureOncePainted(hideDetailTextureOncePainted);
		vehicleController.SetSwapDetailTextureOncePainted(swapDetailTextureOncePainted);
		vehicleController.SetupDrivingAudio(randomItemInArray4, randomItemInArray5);
		vehicleController.SpawnChassis(chassisPrefab, driverSeatUniqueObjectName, customDoorData, customHoodData, customTrunkData, vehicleWorldItemData);
		vehicleController.SpawnExtraHardware(interiorPrefab, VehicleChassisController.ChassisHardwareMountPoint.Interior);
		chassisPrefab.underLightColor = UnityEngine.Random.ColorHSV(0f, 1f, 0.7f, 1f, 0.5f, 1f, 0.3f, 0.6f);
		enginePrefab.useBadBattery = false;
		enginePrefab.useBadPistons = false;
		enginePrefab.useNoPistons = false;
		GameObject[] array = new GameObject[4];
		GameObject randomItemInArray6 = GetRandomItemInArray(wheelOptions);
		for (int j = 0; j < array.Length; j++)
		{
			array[j] = randomItemInArray6;
		}
		GameObject randomItemInArray7 = GetRandomItemInArray(randomItemInArray.InitialHeadlightPrefabs);
		GameObject randomItemInArray8 = GetRandomItemInArray(licensePlatePrefabs);
		GameObject randomItemInArray9 = GetRandomItemInArray(hoodOrnamentPrefabs);
		GameObject randomItemInArray10 = GetRandomItemInArray(randomItemInArray.GasTankPrefabs);
		Texture detailTextureOnPaint = randomItemInArray3.DetailTextureOnPaint;
		VehicleController.EndlessVehiclePartsOptions endlessVehiclePartsOptions = new VehicleController.EndlessVehiclePartsOptions();
		endlessVehiclePartsOptions.wheelPrefabsList = array;
		endlessVehiclePartsOptions.chosenWheel = randomItemInArray6;
		endlessVehiclePartsOptions.chosenHeadlightPrefab = randomItemInArray7;
		endlessVehiclePartsOptions.chosenLicensePlatePrefab = randomItemInArray8;
		endlessVehiclePartsOptions.chosenHoodOrnamentPrefab = randomItemInArray9;
		endlessVehiclePartsOptions.chosenGasTankPrefab = randomItemInArray10;
		endlessVehiclePartsOptions.detailCarTexture = detailTextureOnPaint;
		endlessVehiclePartsOptions.sketchyPlatesPagesOptions = sketchyPlatesPages;
		endlessVehiclePartsOptions.sketchyPlatePrefabsOptions = sketchyPlatePrefabs;
		endlessVehiclePartsOptions.brokenTiresPagesOptions = brokenTiresPages;
		endlessVehiclePartsOptions.brokenTirePrefabsOptions = brokenTirePrefabs;
		endlessVehiclePartsOptions.brokenHeadlightsPagesOptions = brokenHeadlightsPages;
		endlessVehiclePartsOptions.brokenHeadlightPrefabsOptions = brokenHeadlightPrefabs;
		endlessVehiclePartsOptions.badAirFilterPagesOptions = badAirFilterPages;
		endlessVehiclePartsOptions.badBatteryPagesOptions = badBatteryPages;
		endlessVehiclePartsOptions.brokenPistonsPagesOptions = brokenPistonsPages;
		endlessVehiclePartsOptions.noPistonsPagesOptions = noPistonsPages;
		endlessVehiclePartsOptions.engineOilFluidPagesOptions = engineOilFluidPages;
		endlessVehiclePartsOptions.engineOverheatPagesOptions = engineOverheatPages;
		endlessVehiclePartsOptions.overheatObject = overheatPrefab;
		endlessVehiclePartsOptions.enginePrefabIniter = enginePrefab;
		endlessVehiclePartsOptions.wheelOptionsList = wheelOptions;
		endlessVehiclePartsOptions.wheelIndexesToStartDeflated = wheelIndexesThatStartDeflated;
		endlessVehiclePartsOptions.licensePlatePrefabOptions = licensePlatePrefabs;
		endlessVehiclePartsOptions.hoodOrnamentPrefabsOptions = hoodOrnamentPrefabs;
		endlessVehiclePartsOptions.driverSeatUOName = driverSeatUniqueObjectName;
		endlessVehiclePartsOptions.optionalDriveInSoundsList = optionalDriveInSounds;
		endlessVehiclePartsOptions.optionalDriveOutSoundsList = optionalDriveOutSounds;
		endlessVehiclePartsOptions.isFilterSetDirty = isFilterDirty;
		endlessVehiclePartsOptions.needToSetFluidColor = setFluidColor;
		endlessVehiclePartsOptions.initialFluidData = initialFluid;
		endlessVehiclePartsOptions.initialFluidPercent = initialFluidPercentFull;
		endlessVehiclePartsOptions.defaultTrinketObjectsOptions = defaultTrinketObjects;
		endlessVehiclePartsOptions.extraHardwareOptions = extraHardware;
		endlessVehiclePartsOptions.extraHardwarePSVROnlyOptions = extraHardwarePSVROnly;
		endlessVehiclePartsOptions.extraHardwaveNonPSVROnlyOptions = extraHardwaveNonPSVROnly;
		endlessVehiclePartsOptions.vehicleWorldItem = vehicleWorldItemData;
		endlessVehiclePartsOptions.customHoodWorldItemData = customHoodData;
		endlessVehiclePartsOptions.customDoorWorldItemData = customDoorData;
		endlessVehiclePartsOptions.customTrunkWorldItemData = customTrunkData;
		endlessVehiclePartsOptions.interiorTrinketMountWorldItemData = interiorTrinketMountData;
		VehicleController.EndlessVehiclePartsOptions partOptions = endlessVehiclePartsOptions;
		EndlessCarPool.partOptions = partOptions;
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
