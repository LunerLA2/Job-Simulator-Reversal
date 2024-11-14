using System;
using System.Collections;
using OwlchemyVR;
using UnityEngine;

public class PromotionCakeController : MonoBehaviour
{
	[SerializeField]
	private SubtaskData subtaskToMakeSlicesPickupable;

	[SerializeField]
	private BasePrefabSpawner[] slicePrefabSpawners;

	private PickupableItem[] slicePickupables;

	private PickupableItem[] candlePickupables;

	private BasicEdibleItem[] sliceEdibles;

	private void OnEnable()
	{
		JobBoardManager instance = JobBoardManager.instance;
		instance.OnSubtaskComplete = (Action<SubtaskStatusController>)Delegate.Combine(instance.OnSubtaskComplete, new Action<SubtaskStatusController>(SubtaskCompleted));
	}

	private void OnDisable()
	{
		JobBoardManager instance = JobBoardManager.instance;
		instance.OnSubtaskComplete = (Action<SubtaskStatusController>)Delegate.Remove(instance.OnSubtaskComplete, new Action<SubtaskStatusController>(SubtaskCompleted));
	}

	private void Start()
	{
		StartCoroutine(WaitFrameAndCacheInfo());
	}

	private IEnumerator WaitFrameAndCacheInfo()
	{
		yield return null;
		slicePickupables = new PickupableItem[slicePrefabSpawners.Length];
		candlePickupables = new PickupableItem[slicePrefabSpawners.Length];
		sliceEdibles = new BasicEdibleItem[slicePrefabSpawners.Length];
		for (int k = 0; k < slicePrefabSpawners.Length; k++)
		{
			slicePickupables[k] = slicePrefabSpawners[k].LastSpawnedPrefabGO.GetComponent<PickupableItem>();
			slicePickupables[k].enabled = false;
			slicePickupables[k].Rigidbody.isKinematic = true;
			slicePickupables[k].GetComponent<UniqueObject>().AssociatedAttachpoint = slicePickupables[k].gameObject.GetComponentInChildren<AttachablePoint>();
		}
		yield return null;
		yield return null;
		yield return null;
		for (int j = 0; j < slicePickupables.Length; j++)
		{
			Candle candle = slicePickupables[j].gameObject.GetComponentInChildren<Candle>();
			candlePickupables[j] = candle.gameObject.GetComponent<PickupableItem>();
			candlePickupables[j].enabled = false;
		}
		yield return null;
		for (int i = 0; i < slicePrefabSpawners.Length; i++)
		{
			sliceEdibles[i] = slicePrefabSpawners[i].LastSpawnedPrefabGO.GetComponent<BasicEdibleItem>();
			sliceEdibles[i].enabled = false;
		}
	}

	private void SubtaskCompleted(SubtaskStatusController subtaskStatus)
	{
		if (subtaskStatus.Data == subtaskToMakeSlicesPickupable)
		{
			StartCoroutine(WaitAndEnableItems());
		}
	}

	private IEnumerator WaitAndEnableItems()
	{
		yield return new WaitForSeconds(1f);
		for (int i = 0; i < slicePickupables.Length; i++)
		{
			slicePickupables[i].enabled = true;
			slicePickupables[i].Rigidbody.isKinematic = true;
			candlePickupables[i].enabled = true;
			sliceEdibles[i].enabled = true;
			slicePickupables[i].GetComponent<AttachableObject>().Detach();
		}
		yield return null;
	}
}
