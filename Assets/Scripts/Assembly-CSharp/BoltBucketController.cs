using System;
using OwlchemyVR;
using UnityEngine;

public class BoltBucketController : MonoBehaviour
{
	[SerializeField]
	private TriggerListener triggerListener;

	[SerializeField]
	private Transform leftLid;

	[SerializeField]
	private Transform rightLid;

	[SerializeField]
	private Transform spawnerContainer;

	private float lidZScale = 1f;

	private bool open;

	[SerializeField]
	private GameObject boltPileDetail;

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

	private void OnEnable()
	{
		TriggerListener obj = triggerListener;
		obj.OnEnter = (Action<TriggerEventInfo>)Delegate.Combine(obj.OnEnter, new Action<TriggerEventInfo>(Enter));
		TriggerListener obj2 = triggerListener;
		obj2.OnExit = (Action<TriggerEventInfo>)Delegate.Combine(obj2.OnExit, new Action<TriggerEventInfo>(Exit));
		ItemCollectionZone obj3 = itemCollectionZone;
		obj3.OnItemsInCollectionRemoved = (Action<ItemCollectionZone, PickupableItem>)Delegate.Combine(obj3.OnItemsInCollectionRemoved, new Action<ItemCollectionZone, PickupableItem>(ItemRemovedFromCollectionZone));
	}

	private void OnDisable()
	{
		TriggerListener obj = triggerListener;
		obj.OnEnter = (Action<TriggerEventInfo>)Delegate.Remove(obj.OnEnter, new Action<TriggerEventInfo>(Enter));
		TriggerListener obj2 = triggerListener;
		obj2.OnExit = (Action<TriggerEventInfo>)Delegate.Remove(obj2.OnExit, new Action<TriggerEventInfo>(Exit));
		ItemCollectionZone obj3 = itemCollectionZone;
		obj3.OnItemsInCollectionRemoved = (Action<ItemCollectionZone, PickupableItem>)Delegate.Remove(obj3.OnItemsInCollectionRemoved, new Action<ItemCollectionZone, PickupableItem>(ItemRemovedFromCollectionZone));
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
		if (info.other.gameObject.layer == 10)
		{
			open = true;
		}
	}

	private void Exit(TriggerEventInfo info)
	{
		if (info.other.gameObject.layer == 10)
		{
			open = false;
			for (int i = 0; i < itemRespawners.Length; i++)
			{
				itemRespawners[i].RespawnIfRequired(0.15f);
			}
		}
	}

	private void Update()
	{
		lidZScale = Mathf.Lerp(lidZScale, (!open) ? 1 : 0, Time.deltaTime * 10f);
		boltPileDetail.gameObject.SetActive(lidZScale <= 0.95f);
		leftLid.localScale = new Vector3(leftLid.localScale.x, leftLid.localScale.y, lidZScale);
		rightLid.localScale = new Vector3(rightLid.localScale.x, rightLid.localScale.y, lidZScale);
	}
}
