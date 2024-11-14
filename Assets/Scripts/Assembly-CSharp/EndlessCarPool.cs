using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EndlessCarPool : MonoBehaviour
{
	[SerializeField]
	private EndlessVehiclePrefabIniter endlessVehiclePrefab;

	[SerializeField]
	private EndlessEnginePrefabIniter endlessEnginePrefab;

	private List<VehicleController> vehicleList;

	private List<EndlessVehiclePrefabIniter.VehicleBodyTextureInfo> bodyTextureInfoList;

	private List<VehicleHardware> pistonList;

	public static VehicleController.EndlessVehiclePartsOptions partOptions;

	private VehicleController lastTaskCar;

	private void Start()
	{
		vehicleList = new List<VehicleController>();
		if (GenieManager.DoesContainGenieMode(GlobalStorage.Instance.CurrentGenieModes, JobGenieCartridge.GenieModeTypes.EndlessMode))
		{
			SpawnAndPoolAllChassis();
		}
	}

	private void SpawnAndPoolAllChassis()
	{
		for (int i = 0; i < endlessVehiclePrefab.ChassisOptions.Length; i++)
		{
			vehicleList.Add(AutoMechanicManager.SpawnAndPreloadVehicle(endlessVehiclePrefab.masterVehiclePrefab.gameObject, "RandomCar", endlessVehiclePrefab, endlessVehiclePrefab.ChassisOptions[i]));
			vehicleList[i].gameObject.SetActive(false);
		}
	}

	public VehicleController GetRandomCar()
	{
		if (lastTaskCar == null)
		{
			return GetInitialCar();
		}
		VehicleController vehicleController = null;
		if (lastTaskCar.ChassisGroup.ChassisPrefab.name.Contains("Jeep"))
		{
			vehicleController = vehicleList.Where((VehicleController v) => v.ChassisGroup.ChassisPrefab.name.Contains("Truck")).First();
		}
		else if (lastTaskCar.ChassisGroup.ChassisPrefab.name.Contains("Truck"))
		{
			vehicleController = vehicleList.Where((VehicleController v) => v.ChassisGroup.ChassisPrefab.name.Contains("Car")).First();
		}
		else if (lastTaskCar.ChassisGroup.ChassisPrefab.name.Contains("Car"))
		{
			vehicleController = vehicleList.Where((VehicleController v) => v.ChassisGroup.ChassisPrefab.name.Contains("Jeep")).First();
		}
		lastTaskCar = vehicleController;
		vehicleController.ChooseThisCarForEndlessTask(partOptions);
		return vehicleController;
	}

	private VehicleController GetInitialCar()
	{
		int num = Random.Range(0, vehicleList.Count);
		for (int i = 0; i < endlessVehiclePrefab.ChassisOptions.Length; i++)
		{
			if (i == num)
			{
				vehicleList[i].ChooseThisCarForEndlessTask(partOptions);
			}
			else
			{
				vehicleList[i].gameObject.SetActive(false);
			}
		}
		lastTaskCar = vehicleList[num];
		return vehicleList[num];
	}
}
