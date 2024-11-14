using System;
using System.Collections;
using System.Collections.Generic;
using OwlchemyVR;
using UnityEngine;

public class ServingPlate : MonoBehaviour
{
	[SerializeField]
	private WorldItem plateWorldItem;

	[SerializeField]
	private ItemCollectionZone itemCollectionZone;

	[SerializeField]
	private GameObject levitateEffect;

	[SerializeField]
	private Animator toGoBox;

	[SerializeField]
	private WorldItemData toGoBoxData;

	[SerializeField]
	private AudioClip boxAppearSoundClip;

	[SerializeField]
	private ParticleSystem toGoBoxAppearPoof;

	[SerializeField]
	private Transform FoodParent;

	[SerializeField]
	private Transform foodCaptureTarget;

	[SerializeField]
	private TaskData timmyTomatoTask;

	[SerializeField]
	private WorldItemData tomatoFluid;

	[SerializeField]
	private WorldItemData allergySpecialCase;

	private bool isInTimmyTask;

	private bool isInTransit;

	private Transform tableTransform;

	private bool isEndlessMode;

	[SerializeField]
	private MeshRenderer plateRenderer;

	public Animator ToGoBoxAnimator
	{
		get
		{
			return toGoBox;
		}
	}

	public Transform TableTransform
	{
		get
		{
			return tableTransform;
		}
	}

	private void OnEnable()
	{
		ItemCollectionZone obj = itemCollectionZone;
		obj.OnItemsInCollectionAdded = (Action<ItemCollectionZone, PickupableItem>)Delegate.Combine(obj.OnItemsInCollectionAdded, new Action<ItemCollectionZone, PickupableItem>(ItemAdded));
		ItemCollectionZone obj2 = itemCollectionZone;
		obj2.OnItemsInCollectionRemoved = (Action<ItemCollectionZone, PickupableItem>)Delegate.Combine(obj2.OnItemsInCollectionRemoved, new Action<ItemCollectionZone, PickupableItem>(ItemRemoved));
		JobBoardManager instance = JobBoardManager.instance;
		instance.OnTaskStarted = (Action<TaskStatusController>)Delegate.Combine(instance.OnTaskStarted, new Action<TaskStatusController>(TaskStarted));
		JobBoardManager instance2 = JobBoardManager.instance;
		instance2.OnTaskComplete = (Action<TaskStatusController>)Delegate.Combine(instance2.OnTaskComplete, new Action<TaskStatusController>(TaskCompleted));
		if (GenieManager.AreAnyJobGenieModesActive() && GenieManager.DoesContainGenieMode(GlobalStorage.Instance.CurrentGenieModes, JobGenieCartridge.GenieModeTypes.EndlessMode))
		{
			isEndlessMode = true;
			plateRenderer.enabled = false;
		}
	}

	private void OnDisable()
	{
		ItemCollectionZone obj = itemCollectionZone;
		obj.OnItemsInCollectionAdded = (Action<ItemCollectionZone, PickupableItem>)Delegate.Remove(obj.OnItemsInCollectionAdded, new Action<ItemCollectionZone, PickupableItem>(ItemAdded));
		ItemCollectionZone obj2 = itemCollectionZone;
		obj2.OnItemsInCollectionRemoved = (Action<ItemCollectionZone, PickupableItem>)Delegate.Remove(obj2.OnItemsInCollectionRemoved, new Action<ItemCollectionZone, PickupableItem>(ItemRemoved));
		JobBoardManager instance = JobBoardManager.instance;
		instance.OnTaskStarted = (Action<TaskStatusController>)Delegate.Remove(instance.OnTaskStarted, new Action<TaskStatusController>(TaskStarted));
		JobBoardManager instance2 = JobBoardManager.instance;
		instance2.OnTaskComplete = (Action<TaskStatusController>)Delegate.Remove(instance2.OnTaskComplete, new Action<TaskStatusController>(TaskCompleted));
	}

	private void TaskStarted(TaskStatusController task)
	{
		if (task.Data == timmyTomatoTask)
		{
			isInTimmyTask = true;
		}
		else
		{
			isInTimmyTask = false;
		}
		if (isEndlessMode && !toGoBox.gameObject.activeSelf)
		{
			toGoBox.gameObject.SetActive(true);
			AudioManager.Instance.Play(base.transform, boxAppearSoundClip, 1f, 1f);
			toGoBoxAppearPoof.Play();
		}
	}

	private void TaskCompleted(TaskStatusController taskStatus)
	{
		if (isEndlessMode && taskStatus.IsSuccess)
		{
			toGoBox.SetTrigger("close");
			ItemCollectionZone obj = itemCollectionZone;
			obj.OnItemsInCollectionAdded = (Action<ItemCollectionZone, PickupableItem>)Delegate.Remove(obj.OnItemsInCollectionAdded, new Action<ItemCollectionZone, PickupableItem>(ItemAdded));
			ItemCollectionZone obj2 = itemCollectionZone;
			obj2.OnItemsInCollectionRemoved = (Action<ItemCollectionZone, PickupableItem>)Delegate.Remove(obj2.OnItemsInCollectionRemoved, new Action<ItemCollectionZone, PickupableItem>(ItemRemoved));
			UnityEngine.Object.Destroy(itemCollectionZone.RigidbodyTriggerEvents);
			itemCollectionZone.enabled = false;
		}
	}

	private void PageCompleted(PageStatusController page)
	{
		if (!isEndlessMode)
		{
			return;
		}
		for (int i = 0; i < page.Data.Subtasks.Count; i++)
		{
			for (int j = 0; j < page.Data.Subtasks[i].ActionEventConditions.Count; j++)
			{
				for (int k = 0; k < itemCollectionZone.ItemsInCollection.Count; k++)
				{
					PickupableItem pickupableItem = itemCollectionZone.ItemsInCollection[k];
					WorldItemData worldItemData = pickupableItem.InteractableItem.WorldItemData;
					if ((worldItemData == page.Data.Subtasks[i].ActionEventConditions[j].WorldItemData1 || (worldItemData == page.Data.Subtasks[i].ActionEventConditions[j].WorldItemData2 && (bool)worldItemData)) && page.Data.Subtasks[i].ActionEventConditions[j].IsPositive)
					{
						Debug.Log("Correct Item Found. Preparing to put in mini box");
					}
				}
			}
		}
	}

	private void Awake()
	{
		if (!GenieManager.AreAnyJobGenieModesActive() || !GenieManager.DoesContainGenieMode(GlobalStorage.Instance.CurrentGenieModes, JobGenieCartridge.GenieModeTypes.EndlessMode))
		{
			List<Vector3> list = new List<Vector3>();
			list.Add(base.transform.position + Vector3.right + Vector3.down);
			list.Add(base.transform.position + Vector3.right + Vector3.down);
			list.Add(base.transform.position + Vector3.right);
			list.Add(base.transform.position);
			list.Add(base.transform.position);
			base.transform.position += Vector3.right + Vector3.down;
			isInTransit = true;
			StartCoroutine(SendAlongPathInternal(new GoSpline(list), false, true));
		}
	}

	public bool ReadyToSendAway()
	{
		bool result = !isInTransit && itemCollectionZone.ItemsInCollection.Count > 0;
		if (isEndlessMode)
		{
			result = true;
		}
		return result;
	}

	public void SendToTable(Transform midwayPoint, Transform table, bool destroyOnFinish = false)
	{
		tableTransform = table;
		isInTransit = true;
		List<Vector3> list = new List<Vector3>();
		list.Add(base.transform.position);
		list.Add(base.transform.position);
		list.Add(base.transform.position + Vector3.up * 0.1f);
		list.Add(midwayPoint.position);
		list.Add(table.position);
		list.Add(table.position);
		StartCoroutine(SendAlongPathInternal(new GoSpline(list), true, false, destroyOnFinish));
	}

	private IEnumerator SendAlongPathInternal(GoSpline positionPath, bool doPlating, bool endTransitWhenFinished, bool destroyOnFinish = false)
	{
		if (doPlating)
		{
			PlateAllItems();
		}
		levitateEffect.SetActive(true);
		Go.to(base.transform, 3f, new GoTweenConfig().positionPath(positionPath).setEaseType(GoEaseType.QuadInOut));
		yield return new WaitForSeconds(3f);
		levitateEffect.SetActive(false);
		if (endTransitWhenFinished)
		{
			isInTransit = false;
			PickupableItem pickupableItem2 = null;
			for (int i = 0; i < itemCollectionZone.ItemsInCollection.Count; i++)
			{
				pickupableItem2 = itemCollectionZone.ItemsInCollection[i];
				if (pickupableItem2 != null)
				{
					PlateActivatedEvent();
				}
			}
		}
		if (destroyOnFinish)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	public void RemoveFromTable()
	{
		Debug.Log("remove from table was called");
		itemCollectionZone.DestroyAllItemsInCollectionZone();
		UnityEngine.Object.Destroy(base.gameObject);
	}

	public void PlateAllItems()
	{
		PickupableItem pickupableItem = null;
		bool flag = false;
		for (int i = 0; i < itemCollectionZone.ItemsInCollection.Count; i++)
		{
			pickupableItem = itemCollectionZone.ItemsInCollection[i];
			if (!(pickupableItem != null))
			{
				continue;
			}
			if (isInTimmyTask && !flag)
			{
				BreadSliceController component = pickupableItem.gameObject.GetComponent<BreadSliceController>();
				if (component != null)
				{
					if (component.GetAmountOfGivenFluidThatExistsAsSauce(tomatoFluid) >= 5f)
					{
						GameEventsManager.Instance.ItemActionOccurred(allergySpecialCase, "ACTIVATED");
						flag = true;
					}
				}
				else
				{
					WorldItem worldItem = pickupableItem.InteractableItem.WorldItem;
					if (worldItem != null && worldItem.Data == tomatoFluid)
					{
						GameEventsManager.Instance.ItemActionOccurred(allergySpecialCase, "ACTIVATED");
						flag = true;
					}
				}
			}
			if (pickupableItem.InteractableItem.WorldItemData != null)
			{
				GameEventsManager.Instance.ItemAppliedToItemActionOccurred(pickupableItem.InteractableItem.WorldItemData, plateWorldItem.Data, "SERVED_BY");
			}
			WorldItem worldItem2 = pickupableItem.InteractableItem.WorldItem;
			if (worldItem2 != null)
			{
				worldItem2.enabled = false;
			}
			Rigidbody rigidbody = pickupableItem.Rigidbody;
			if (rigidbody != null)
			{
				UnityEngine.Object.Destroy(rigidbody);
			}
			pickupableItem.transform.SetParent(FoodParent, true);
			StartCoroutine(WaitAndDetachItem(pickupableItem));
		}
		StartCoroutine(WaitAndSetQuantityToZero());
	}

	private IEnumerator WaitAndSetQuantityToZero()
	{
		yield return new WaitForSeconds(1f);
		GameEventsManager.Instance.ItemActionOccurredWithAmount(plateWorldItem.Data, "FILLED_TO_AMOUNT", 0f);
		GameEventsManager.Instance.ItemActionOccurred(plateWorldItem.Data, "DEACTIVATED");
	}

	private IEnumerator WaitAndDetachItem(PickupableItem item)
	{
		yield return new WaitForSeconds(2f);
		if (item != null)
		{
			ItemRemoved(itemCollectionZone, item);
		}
	}

	private void ItemAdded(ItemCollectionZone zone, PickupableItem item)
	{
		WorldItem worldItem = item.InteractableItem.WorldItem;
		if (!(worldItem.Data == toGoBoxData) && (!isEndlessMode || toGoBox.gameObject.activeInHierarchy))
		{
			if (worldItem != null)
			{
				GameEventsManager.Instance.ItemAppliedToItemActionOccurred(worldItem.Data, plateWorldItem.Data, "ATTACHED_TO");
			}
			if (isEndlessMode)
			{
				StartCoroutine(ShrinkAndDestroyItem(item));
			}
			if (!isInTransit)
			{
				PlateActivatedEvent();
			}
		}
	}

	private IEnumerator ShrinkAndDestroyItem(PickupableItem objectToDestroy)
	{
		if (objectToDestroy.IsCurrInHand)
		{
			objectToDestroy.CurrInteractableHand.TryRelease();
		}
		objectToDestroy.enabled = false;
		objectToDestroy.GetComponentInChildren<Rigidbody>().isKinematic = true;
		float duration = 0.33f;
		float currentTime = 0f;
		Vector3 originalSize = objectToDestroy.transform.localScale;
		Vector3 originalPosition = objectToDestroy.transform.position;
		while (currentTime < duration)
		{
			if (objectToDestroy != null)
			{
				objectToDestroy.transform.localScale = Vector3.Lerp(originalSize, Vector3.zero, currentTime / duration);
				objectToDestroy.transform.position = Vector3.Lerp(originalPosition, foodCaptureTarget.position, currentTime / duration);
				currentTime += Time.deltaTime;
				yield return null;
				continue;
			}
			yield break;
		}
		if (objectToDestroy == null)
		{
			Debug.LogWarning("Pickupable became null, ignoring");
		}
		else
		{
			UnityEngine.Object.Destroy(objectToDestroy.gameObject);
		}
	}

	private void PlateActivatedEvent()
	{
		GameEventsManager.Instance.ItemActionOccurredWithAmount(plateWorldItem.Data, "FILLED_TO_AMOUNT", itemCollectionZone.ItemsInCollection.Count);
		GameEventsManager.Instance.ItemActionOccurred(plateWorldItem.Data, "ACTIVATED");
	}

	private void ItemRemoved(ItemCollectionZone zone, PickupableItem item)
	{
		WorldItem worldItem = item.InteractableItem.WorldItem;
		if (worldItem != null)
		{
			GameEventsManager.Instance.ItemAppliedToItemActionOccurred(worldItem.Data, plateWorldItem.Data, "DEATTACHED_FROM");
		}
		if (!isInTransit)
		{
			GameEventsManager.Instance.ItemActionOccurredWithAmount(plateWorldItem.Data, "FILLED_TO_AMOUNT", itemCollectionZone.ItemsInCollection.Count);
			if (itemCollectionZone.ItemsInCollection.Count == 0)
			{
				GameEventsManager.Instance.ItemActionOccurred(plateWorldItem.Data, "DEACTIVATED");
			}
		}
	}
}
