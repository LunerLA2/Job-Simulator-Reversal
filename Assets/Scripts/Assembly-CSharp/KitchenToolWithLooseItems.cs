using System;
using System.Collections.Generic;
using OwlchemyVR;
using UnityEngine;

public class KitchenToolWithLooseItems : KitchenTool
{
	[SerializeField]
	private BasePrefabSpawner[] looseItemSpawners;

	[SerializeField]
	private BasePrefabSpawner[] looseItemsStartKinematic;

	private GameObject[] looseItemsSpawned;

	private GameObject[] looseItemsKinematicSpawned;

	private bool didFirstSummon;

	private List<GrabbableItem> grabbablesStillKinematic = new List<GrabbableItem>();

	private void Awake()
	{
		looseItemsSpawned = new GameObject[looseItemSpawners.Length];
		for (int i = 0; i < looseItemSpawners.Length; i++)
		{
			GameObject gameObject = looseItemSpawners[i].SpawnPrefab();
			gameObject = looseItemSpawners[i].LastSpawnedPrefabGO;
			gameObject.GetComponent<Rigidbody>().isKinematic = true;
			gameObject.GetComponent<GrabbableItem>().enabled = false;
			looseItemsSpawned[i] = gameObject;
		}
		looseItemsKinematicSpawned = new GameObject[looseItemsStartKinematic.Length];
		for (int j = 0; j < looseItemsStartKinematic.Length; j++)
		{
			GameObject gameObject2 = looseItemsStartKinematic[j].SpawnPrefab();
			gameObject2 = looseItemsStartKinematic[j].LastSpawnedPrefabGO;
			gameObject2.GetComponent<Rigidbody>().isKinematic = true;
			looseItemsKinematicSpawned[j] = gameObject2;
			GrabbableItem component = gameObject2.GetComponent<GrabbableItem>();
			component.enabled = false;
			component.OnGrabbed = (Action<GrabbableItem>)Delegate.Combine(component.OnGrabbed, new Action<GrabbableItem>(KinematicItemGrabbed));
			grabbablesStillKinematic.Add(component);
		}
	}

	private void KinematicItemGrabbed(GrabbableItem item)
	{
		item.OnGrabbed = (Action<GrabbableItem>)Delegate.Remove(item.OnGrabbed, new Action<GrabbableItem>(KinematicItemGrabbed));
		if (grabbablesStillKinematic.Contains(item))
		{
			grabbablesStillKinematic.Remove(item);
		}
		item.transform.SetParent(GlobalStorage.Instance.ContentRoot, true);
	}

	public override void OnDismiss()
	{
		for (int i = 0; i < grabbablesStillKinematic.Count; i++)
		{
			if (grabbablesStillKinematic[i] != null)
			{
				grabbablesStillKinematic[i].transform.SetParent(base.transform, true);
			}
		}
		base.OnDismiss();
	}

	public override void OnSummon()
	{
		if (looseItemsSpawned != null)
		{
			for (int i = 0; i < looseItemSpawners.Length; i++)
			{
				GameObject gameObject = looseItemsSpawned[i];
				if (gameObject != null)
				{
					gameObject.GetComponent<Rigidbody>().isKinematic = false;
					gameObject.GetComponent<GrabbableItem>().enabled = true;
					if (!didFirstSummon)
					{
						gameObject.transform.SetParent(GlobalStorage.Instance.ContentRoot, true);
					}
				}
			}
			looseItemsSpawned = null;
		}
		if (looseItemsKinematicSpawned != null)
		{
			for (int j = 0; j < looseItemsKinematicSpawned.Length; j++)
			{
				GameObject gameObject2 = looseItemsKinematicSpawned[j];
				if (gameObject2 != null)
				{
					gameObject2.GetComponent<GrabbableItem>().enabled = true;
					if (!didFirstSummon)
					{
						gameObject2.transform.SetParent(GlobalStorage.Instance.ContentRoot, true);
					}
				}
			}
			looseItemsKinematicSpawned = null;
		}
		didFirstSummon = true;
	}
}
