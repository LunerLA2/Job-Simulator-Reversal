using OwlchemyVR;
using UnityEngine;

public class FilingCabinetController : MonoBehaviour
{
	private Transform topDrawer;

	[SerializeField]
	private GameObjectPrefabSpawner topDrawerPrefabSpawner;

	private float sendEventWhenZPasses = 0.2f;

	private float resetEventWhenZPasses = 0.1f;

	private WorldItem worldItem;

	private bool topDrawerOpen;

	private void Awake()
	{
		worldItem = GetComponent<WorldItem>();
		topDrawer = topDrawerPrefabSpawner.LastSpawnedPrefabGO.transform;
	}

	private void Update()
	{
		if (!topDrawerOpen)
		{
			if (topDrawer.localPosition.z >= sendEventWhenZPasses)
			{
				topDrawerOpen = true;
				GameEventsManager.Instance.ItemActionOccurred(worldItem.Data, "OPENED");
			}
		}
		else if (topDrawer.localPosition.z <= resetEventWhenZPasses)
		{
			topDrawerOpen = false;
			GameEventsManager.Instance.ItemActionOccurred(worldItem.Data, "CLOSED");
		}
	}
}
