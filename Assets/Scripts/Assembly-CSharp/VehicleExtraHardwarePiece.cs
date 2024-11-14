using System;
using UnityEngine;

[Serializable]
public class VehicleExtraHardwarePiece
{
	[SerializeField]
	private VehicleHardware hardwarePrefab;

	[SerializeField]
	private VehicleChassisController.ChassisHardwareMountPoint mountPoint;

	public void SpawnOnVehicle(VehicleController vehicle)
	{
		vehicle.SpawnExtraHardware(hardwarePrefab, mountPoint);
	}
}
