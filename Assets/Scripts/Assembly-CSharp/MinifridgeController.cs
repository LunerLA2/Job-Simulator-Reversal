using OwlchemyVR;
using UnityEngine;

public class MinifridgeController : MonoBehaviour
{
	[SerializeField]
	private Transform spawnerContainer;

	[SerializeField]
	private ItemCollectionZone itemCollectionZone;

	private ItemRespawner[] itemRespawners;

	private void Awake()
	{
		itemRespawners = spawnerContainer.GetComponentsInChildren<ItemRespawner>();
		for (int i = 0; i < itemRespawners.Length; i++)
		{
			itemRespawners[i].RespawnIfRequired();
		}
	}

	private void ItemRemovedFromCollectionZone(ItemCollectionZone zone, PickupableItem pickupable)
	{
		if (!pickupable.transform.IsChildOf(base.transform))
		{
			return;
		}
		for (int i = 0; i < itemRespawners.Length; i++)
		{
			if (itemRespawners[i].LastItemSpawned == pickupable)
			{
				itemRespawners[i].LastItemRemovedFromContainer();
			}
		}
		pickupable.transform.SetParent(GlobalStorage.Instance.ContentRoot, true);
	}

	private void Enter(TriggerEventInfo info)
	{
		if (info.other.gameObject.layer != 10)
		{
		}
	}

	private void Exit(TriggerEventInfo info)
	{
		if (info.other.gameObject.layer == 10)
		{
			for (int i = 0; i < itemRespawners.Length; i++)
			{
				itemRespawners[i].RespawnIfRequired(0.15f);
			}
		}
	}
}
