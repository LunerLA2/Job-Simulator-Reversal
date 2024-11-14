using System;
using OwlchemyVR;

[Serializable]
public class DetailedFloorPickupInfo
{
	public WorldItemData data;

	public bool destroyIfNoLocationFound = true;

	public RecycleSpawnLocation[] spawnLocations;
}
