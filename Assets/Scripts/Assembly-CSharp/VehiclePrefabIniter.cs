using OwlchemyVR;
using UnityEngine;

public class VehiclePrefabIniter : MonoBehaviourPrefabIniter
{
	[SerializeField]
	private VehicleController masterVehiclePrefab;

	[Header("Chassis")]
	[SerializeField]
	private VehicleChassisController chassisPrefab;

	[SerializeField]
	private VehicleChassisController chassisPrefabPSVR;

	[SerializeField]
	private Color defaultColor;

	[SerializeField]
	private Texture baseCarTexture;

	[SerializeField]
	private Texture detailCarTexture;

	[SerializeField]
	private bool hideDetailTextureOncePainted;

	[SerializeField]
	private bool swapDetailTextureOncePainted;

	[SerializeField]
	private Texture detailCarTextureOnPaint;

	[SerializeField]
	private GameObject interiorPrefab;

	[SerializeField]
	private GameObject psvrInteriorPrefab;

	[SerializeField]
	private GameObject enginePrefab;

	[Header("WorldItem Settings")]
	[SerializeField]
	private WorldItemData vehicleWorldItemData;

	[SerializeField]
	private WorldItemData customHoodData;

	[SerializeField]
	private WorldItemData customDoorData;

	[SerializeField]
	private WorldItemData customTrunkData;

	[SerializeField]
	private WorldItemData interiorTrinketMountData;

	[Header("Core Hardware")]
	[SerializeField]
	[Tooltip("Prefabs you specify will be looped through until all wheel attachments are filled.")]
	private GameObject[] wheelPrefabs;

	[SerializeField]
	private int[] wheelIndexesThatStartDeflated;

	[SerializeField]
	private GameObject headlightPrefab;

	[SerializeField]
	private GameObject licensePlatePrefab;

	[SerializeField]
	private GameObject gasTankPrefab;

	[SerializeField]
	private GameObject hoodOrnamentPrefab;

	[Header("Misc Settings")]
	[SerializeField]
	private string driverSeatUniqueObjectName = string.Empty;

	[SerializeField]
	private AudioClip optionalDriveInSound;

	[SerializeField]
	private AudioClip optionalDriveOutSound;

	[SerializeField]
	private bool isFilterDirty;

	[SerializeField]
	private bool setFluidColor;

	[SerializeField]
	private WorldItemData initialFluid;

	[Range(0f, 1f)]
	[SerializeField]
	private float initialFluidPercentFull;

	[SerializeField]
	private GameObject[] defaultTrinketObjects;

	[Header("Additional Hardware")]
	[SerializeField]
	private VehicleExtraHardwarePiece[] extraHardware;

	[SerializeField]
	private VehicleExtraHardwarePiece[] extraHardwarePSVROnly;

	[SerializeField]
	private VehicleExtraHardwarePiece[] extraHardwaveNonPSVROnly;

	private GameObject engine;

	public override MonoBehaviour GetPrefab()
	{
		return masterVehiclePrefab;
	}

	public override void Init(MonoBehaviour spawnedPrefab)
	{
		VehicleController vehicleController = spawnedPrefab as VehicleController;
		vehicleController.SetTextureSettings(defaultColor, baseCarTexture, detailCarTexture);
		vehicleController.SetHideDetailTextureOncePainted(hideDetailTextureOncePainted);
		vehicleController.SetSwapDetailTextureOncePainted(swapDetailTextureOncePainted);
		vehicleController.SetupDrivingAudio(optionalDriveInSound, optionalDriveOutSound);
		vehicleController.SpawnChassis(chassisPrefab, driverSeatUniqueObjectName, customDoorData, customHoodData, customTrunkData, vehicleWorldItemData);
		vehicleController.SpawnExtraHardware(interiorPrefab, VehicleChassisController.ChassisHardwareMountPoint.Interior);
		engine = vehicleController.SpawnExtraHardware(enginePrefab, VehicleChassisController.ChassisHardwareMountPoint.Engine);
		vehicleController.SpawnWheels(wheelPrefabs, wheelIndexesThatStartDeflated);
		vehicleController.SpawnHeadlights(headlightPrefab);
		vehicleController.SpawnLicensePlate(licensePlatePrefab);
		vehicleController.SpawnHoodOrnament(hoodOrnamentPrefab);
		vehicleController.SpawnTrinketMount(defaultTrinketObjects, interiorTrinketMountData);
		vehicleController.SpawnExtraHardware(gasTankPrefab, VehicleChassisController.ChassisHardwareMountPoint.GasTank);
		vehicleController.SetDetailTextureOnPaint(detailCarTextureOnPaint);
		for (int i = 0; i < extraHardware.Length; i++)
		{
			extraHardware[i].SpawnOnVehicle(vehicleController);
		}
		for (int j = 0; j < extraHardwaveNonPSVROnly.Length; j++)
		{
			extraHardwaveNonPSVROnly[j].SpawnOnVehicle(vehicleController);
		}
		if (isFilterDirty)
		{
			engine.GetComponentInChildren<VehicleAirFilter>().SetDirty();
		}
		if (setFluidColor)
		{
			DipstickController componentInChildren = engine.GetComponentInChildren<DipstickController>();
			if (componentInChildren != null)
			{
				componentInChildren.ChangeInitialFluid(initialFluid, initialFluidPercentFull);
			}
		}
		vehicleController.SetLockedStateOfItemsWithinCar(true);
	}
}
