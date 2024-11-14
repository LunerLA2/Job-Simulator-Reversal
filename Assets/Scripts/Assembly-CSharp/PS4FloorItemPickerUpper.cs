using System;
using System.Collections.Generic;
using OwlchemyVR;
using UnityEngine;

public class PS4FloorItemPickerUpper : MonoBehaviour
{
	[SerializeField]
	private ItemCollectionZone floorZone;

	[SerializeField]
	private AudioClip soundToPlayOnMove;

	[SerializeField]
	private ParticleSystem pfxToPlayOnPlace;

	[SerializeField]
	private ParticleSystem pfxToPlayOnMove;

	[SerializeField]
	private SphereCollider[] validSpawnAreas;

	[SerializeField]
	private ItemRecyclerManager itemRecyclerManager;

	[SerializeField]
	private bool checkForTrinkets;

	private bool moreItemsThenCanHandle;

	private float timeSinceLastRespawn;

	private float timeBetweenRespawns = 0.5f;

	[SerializeField]
	private List<WorldItemData> itemsNotToPickup = new List<WorldItemData>();

	private HashSet<WorldItemData> itemsNotToPickupHashSetCache = new HashSet<WorldItemData>();

	[SerializeField]
	private List<CustomFloorPickupInfo> customRespawnPoints = new List<CustomFloorPickupInfo>();

	private Dictionary<WorldItemData, CustomFloorPickupInfo> customRespawnPointsDictionary = new Dictionary<WorldItemData, CustomFloorPickupInfo>();

	[SerializeField]
	private List<DetailedFloorPickupInfo> detailedPickupInfos = new List<DetailedFloorPickupInfo>();

	private Dictionary<WorldItemData, DetailedFloorPickupInfo> detailedPickupsDictionary = new Dictionary<WorldItemData, DetailedFloorPickupInfo>();

	private Transform lastSpawnAreaUsedTransform;

	private void Awake()
	{
		if (validSpawnAreas != null)
		{
			for (int i = 0; i < validSpawnAreas.Length; i++)
			{
				validSpawnAreas[i].gameObject.SetActive(false);
			}
		}
		for (int j = 0; j < itemsNotToPickup.Count; j++)
		{
			if (itemsNotToPickup[j] != null)
			{
				itemsNotToPickupHashSetCache.Add(itemsNotToPickup[j]);
			}
		}
		customRespawnPointsDictionary = new Dictionary<WorldItemData, CustomFloorPickupInfo>();
		for (int k = 0; k < customRespawnPoints.Count; k++)
		{
			if (customRespawnPoints[k] != null)
			{
				customRespawnPointsDictionary.Add(customRespawnPoints[k].data, customRespawnPoints[k]);
			}
		}
		detailedPickupsDictionary = new Dictionary<WorldItemData, DetailedFloorPickupInfo>();
		for (int l = 0; l < detailedPickupInfos.Count; l++)
		{
			if (detailedPickupInfos[l].data != null)
			{
				detailedPickupsDictionary.Add(detailedPickupInfos[l].data, detailedPickupInfos[l]);
			}
		}
	}

	private void OnEnable()
	{
		ItemCollectionZone itemCollectionZone = floorZone;
		itemCollectionZone.OnItemsInCollectionAdded = (Action<ItemCollectionZone, PickupableItem>)Delegate.Combine(itemCollectionZone.OnItemsInCollectionAdded, new Action<ItemCollectionZone, PickupableItem>(ItemTouchedFloor));
	}

	private void OnDisable()
	{
		ItemCollectionZone itemCollectionZone = floorZone;
		itemCollectionZone.OnItemsInCollectionAdded = (Action<ItemCollectionZone, PickupableItem>)Delegate.Remove(itemCollectionZone.OnItemsInCollectionAdded, new Action<ItemCollectionZone, PickupableItem>(ItemTouchedFloor));
	}

	private void Update()
	{
		if (moreItemsThenCanHandle && Time.time > timeSinceLastRespawn + timeBetweenRespawns)
		{
			CheckForItemsToRespawn();
		}
	}

	private void CheckForItemsToRespawn()
	{
		if (floorZone.NumOfItemsInCollectionZone > 0)
		{
			if (floorZone.ItemsInCollection[0] != null)
			{
				if (floorZone.ItemsInCollection.Count == 1)
				{
					moreItemsThenCanHandle = false;
				}
				RespawnItem(floorZone.ItemsInCollection[0]);
			}
		}
		else
		{
			moreItemsThenCanHandle = false;
		}
	}

	private void ItemTouchedFloor(ItemCollectionZone zone, PickupableItem item)
	{
		bool flag = true;
		if (item.Rigidbody == null)
		{
			flag = false;
		}
		else if (item.Rigidbody.isKinematic)
		{
			flag = false;
		}
		if (flag && checkForTrinkets && item.GetComponent<TrinketSpringController>() != null)
		{
			flag = false;
		}
		if (flag)
		{
			if (!moreItemsThenCanHandle && Time.time > timeSinceLastRespawn + timeBetweenRespawns)
			{
				RespawnItem(item);
			}
			else
			{
				moreItemsThenCanHandle = true;
			}
		}
	}

	private void RespawnItem(PickupableItem item)
	{
		WorldItemData worldItemData = item.InteractableItem.WorldItemData;
		if (itemRecyclerManager != null && itemRecyclerManager.WouldRecyclerHandleThis(worldItemData))
		{
			if (pfxToPlayOnMove != null)
			{
				pfxToPlayOnMove.transform.position = item.transform.position;
				pfxToPlayOnMove.Play();
			}
			itemRecyclerManager.RespawnItemWithItem(item);
		}
		else if (!itemsNotToPickupHashSetCache.Contains(worldItemData))
		{
			Vector3 position = Vector3.zero;
			AttachablePoint attachablePoint = null;
			bool flag = false;
			timeSinceLastRespawn = Time.time;
			if (detailedPickupsDictionary.ContainsKey(worldItemData))
			{
				bool flag2 = false;
				for (int i = 0; i < detailedPickupsDictionary[worldItemData].spawnLocations.Length; i++)
				{
					RecycleSpawnLocation recycleSpawnLocation = detailedPickupsDictionary[worldItemData].spawnLocations[i];
					UniqueObject objectByName = BotUniqueElementManager.Instance.GetObjectByName(recycleSpawnLocation.UniqueObjectID);
					if (recycleSpawnLocation.SpawnType == RecycleSpawnLocation.LocationSpawnTypes.DropAtPosition)
					{
						position = objectByName.transform.position;
						flag2 = true;
						break;
					}
					if (recycleSpawnLocation.SpawnType == RecycleSpawnLocation.LocationSpawnTypes.LookForAttachpoint)
					{
						position = objectByName.transform.position;
						attachablePoint = objectByName.AssociatedAttachpoint;
						if (attachablePoint != null)
						{
							if (attachablePoint.NumAttachedObjects == 0)
							{
								flag2 = true;
								break;
							}
							attachablePoint = null;
						}
					}
					else
					{
						Debug.LogError("Detailed Pickup info type not supported: " + recycleSpawnLocation.SpawnType);
					}
				}
				if (!flag2 && detailedPickupsDictionary[worldItemData].destroyIfNoLocationFound)
				{
					flag = true;
				}
			}
			else if (customRespawnPointsDictionary.ContainsKey(worldItemData))
			{
				Transform spawnPos = customRespawnPointsDictionary[worldItemData].spawnPos;
				position = ((!(spawnPos != null)) ? GetBestPosition(item.transform.position) : spawnPos.position);
			}
			else
			{
				position = GetBestPosition(item.transform.position);
			}
			if (pfxToPlayOnMove != null)
			{
				pfxToPlayOnMove.transform.position = item.transform.position;
				pfxToPlayOnMove.Play();
			}
			if (!flag)
			{
				item.transform.position = position;
				item.transform.rotation = Quaternion.identity;
				item.Rigidbody.MovePosition(position);
				item.Rigidbody.MoveRotation(Quaternion.identity);
				item.Rigidbody.velocity = Vector3.zero;
				item.Rigidbody.angularVelocity = Vector3.zero;
				if (attachablePoint != null)
				{
					AttachableObject component = item.GetComponent<AttachableObject>();
					if (component != null)
					{
						component.AttachTo(attachablePoint);
					}
					else
					{
						Debug.LogError("Tried to respawn an object on an attachablePoint but " + item.gameObject.name + " isn't an attachableObject", item.gameObject);
					}
				}
				if (soundToPlayOnMove != null)
				{
					AudioManager.Instance.Play(item.transform.position, soundToPlayOnMove, 1f, 1f);
				}
				if (pfxToPlayOnPlace != null)
				{
					pfxToPlayOnPlace.transform.position = position;
					pfxToPlayOnPlace.Play();
				}
			}
			else
			{
				if (item.InteractableItem.WorldItemData != null)
				{
					GameEventsManager.Instance.ItemActionOccurred(item.InteractableItem.WorldItemData, "DESTROYED");
				}
				UnityEngine.Object.Destroy(item.gameObject);
			}
		}
		else
		{
			if (pfxToPlayOnMove != null)
			{
				pfxToPlayOnMove.transform.position = item.transform.position;
				pfxToPlayOnMove.Play();
			}
			if (item.InteractableItem.WorldItemData != null)
			{
				GameEventsManager.Instance.ItemActionOccurred(item.InteractableItem.WorldItemData, "DESTROYED");
			}
			UnityEngine.Object.Destroy(item.gameObject);
		}
	}

	private Vector3 GetBestPosition(Vector3 itemPos)
	{
		Vector3 result = Vector3.forward * 1000f;
		Transform transform = null;
		for (int i = 0; i < validSpawnAreas.Length; i++)
		{
			if (validSpawnAreas[i].transform != lastSpawnAreaUsedTransform || validSpawnAreas.Length <= 1)
			{
				float num = Vector2.Distance(new Vector2(itemPos.x, itemPos.z), new Vector2(validSpawnAreas[i].transform.position.x, validSpawnAreas[i].transform.position.z));
				if (num < Vector2.Distance(new Vector2(itemPos.x, itemPos.z), new Vector2(result.x, result.z)))
				{
					transform = validSpawnAreas[i].transform;
					result = transform.position;
				}
			}
		}
		if (transform != null)
		{
			lastSpawnAreaUsedTransform = transform;
		}
		return result;
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.blue;
		if (validSpawnAreas == null)
		{
			return;
		}
		for (int i = 0; i < validSpawnAreas.Length; i++)
		{
			if (validSpawnAreas[i] != null)
			{
				Gizmos.DrawWireSphere(validSpawnAreas[i].transform.position, validSpawnAreas[i].radius);
			}
		}
	}
}
