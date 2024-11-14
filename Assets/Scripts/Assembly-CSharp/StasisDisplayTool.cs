using System;
using System.Collections;
using System.Collections.Generic;
using OwlchemyVR;
using UnityEngine;

public class StasisDisplayTool : KitchenTool
{
	private const string DISPLAY_UNIQUE_OBJ = "EnergyCanDisplayController";

	private const float SCALE_SHRINK_ON_DISMISS = 0.65f;

	[SerializeField]
	private WorldItem myWorldItem;

	[SerializeField]
	private ItemCollectionZone stasisField;

	[SerializeField]
	private Rigidbody buttonRB;

	[SerializeField]
	private StasisFieldController stasisController;

	[SerializeField]
	private Transform transformToMove;

	[SerializeField]
	private Transform positionWhenInChooser;

	private List<PickupableItem> itemsWhenDismissed = new List<PickupableItem>();

	private bool itemsLockedState;

	private bool hasBeenFinished;

	private void OnEnable()
	{
		ItemCollectionZone itemCollectionZone = stasisField;
		itemCollectionZone.OnItemsInCollectionAdded = (Action<ItemCollectionZone, PickupableItem>)Delegate.Combine(itemCollectionZone.OnItemsInCollectionAdded, new Action<ItemCollectionZone, PickupableItem>(ItemEnteredZone));
		ItemCollectionZone itemCollectionZone2 = stasisField;
		itemCollectionZone2.OnItemsInCollectionRemoved = (Action<ItemCollectionZone, PickupableItem>)Delegate.Combine(itemCollectionZone2.OnItemsInCollectionRemoved, new Action<ItemCollectionZone, PickupableItem>(ItemExitedZone));
	}

	private void OnDisable()
	{
		ItemCollectionZone itemCollectionZone = stasisField;
		itemCollectionZone.OnItemsInCollectionAdded = (Action<ItemCollectionZone, PickupableItem>)Delegate.Remove(itemCollectionZone.OnItemsInCollectionAdded, new Action<ItemCollectionZone, PickupableItem>(ItemEnteredZone));
		ItemCollectionZone itemCollectionZone2 = stasisField;
		itemCollectionZone2.OnItemsInCollectionRemoved = (Action<ItemCollectionZone, PickupableItem>)Delegate.Remove(itemCollectionZone2.OnItemsInCollectionRemoved, new Action<ItemCollectionZone, PickupableItem>(ItemExitedZone));
	}

	private void ItemEnteredZone(ItemCollectionZone zone, PickupableItem item)
	{
		GameEventsManager.Instance.ItemAppliedToItemActionOccurred(item.InteractableItem.WorldItemData, myWorldItem.Data, "ADDED_TO");
	}

	private void ItemExitedZone(ItemCollectionZone zone, PickupableItem item)
	{
		GameEventsManager.Instance.ItemAppliedToItemActionOccurred(item.InteractableItem.WorldItemData, myWorldItem.Data, "REMOVED_FROM");
	}

	public override void OnDismiss()
	{
		if (!itemsLockedState)
		{
			itemsLockedState = true;
			itemsWhenDismissed.Clear();
			itemsWhenDismissed.AddRange(stasisField.ItemsInCollection);
			SetItemsLockedToDisplayState(itemsWhenDismissed, true);
			for (int i = 0; i < itemsWhenDismissed.Count; i++)
			{
				parentStasher.ForceRemoveItemFromStashing(itemsWhenDismissed[i]);
			}
		}
		buttonRB.isKinematic = true;
		Go.killAllTweensWithTarget(transformToMove);
		transformToMove.localScale = Vector3.one;
		Go.to(transformToMove, 1f, new GoTweenConfig().scale(new Vector3(1f, 0.65f, 1f)).setEaseType(GoEaseType.QuadIn));
		base.OnDismiss();
	}

	public override void WasCompletelyDismissed()
	{
		base.WasCompletelyDismissed();
		transformToMove.localScale = Vector3.one;
		UniqueObject objectByName = BotUniqueElementManager.Instance.GetObjectByName("EnergyCanDisplayController");
		EnergyDisplayShelfController component = objectByName.gameObject.GetComponent<EnergyDisplayShelfController>();
		component.ReceiveDisplay(transformToMove, parentStasher);
	}

	public override void BeganBeingSummoned()
	{
		base.BeganBeingSummoned();
		buttonRB.isKinematic = true;
		transformToMove.SetParent(positionWhenInChooser);
		transformToMove.SetToDefaultPosRotScale();
		transformToMove.localScale = new Vector3(1f, 0.65f, 1f);
		Go.killAllTweensWithTarget(transformToMove);
		Go.to(transformToMove, 1f, new GoTweenConfig().scale(Vector3.one).setEaseType(GoEaseType.QuadOut).setDelay(0.7f));
	}

	public override void OnSummon()
	{
		base.OnSummon();
		buttonRB.isKinematic = false;
		hasBeenFinished = false;
		if (itemsLockedState)
		{
			itemsLockedState = false;
			SetItemsLockedToDisplayState(itemsWhenDismissed, false);
		}
	}

	public void FinishedButtonPressed()
	{
		if (parentStasher != null)
		{
			if (!hasBeenFinished)
			{
				hasBeenFinished = true;
			}
			StartCoroutine(FinishAsync());
		}
		else
		{
			Debug.LogError("Finished Button was pressed but there is no ref to the parentStasher!", base.gameObject);
		}
	}

	private IEnumerator FinishAsync()
	{
		yield return new WaitForSeconds(1f);
		KitchenCounterController counter = parentStasher.gameObject.GetComponent<KitchenCounterController>();
		if (counter != null)
		{
			counter.Dial.SetSnapIndex(1);
			counter.ManuallyRefresh();
		}
		else
		{
			Debug.LogError("Finished button pressed but no counterController found on " + parentStasher.gameObject.name, parentStasher.gameObject);
		}
	}

	private void SetItemsLockedToDisplayState(List<PickupableItem> list, bool state)
	{
		stasisController.SetCanAddItems(!state);
		for (int i = 0; i < list.Count; i++)
		{
			list[i].gameObject.SetActive(true);
			if (!state)
			{
				list[i].transform.SetParent(GlobalStorage.Instance.ContentRoot, true);
				list[i].Rigidbody.isKinematic = false;
			}
			else
			{
				list[i].Rigidbody.isKinematic = true;
				list[i].transform.SetParent(transformToMove, true);
			}
			list[i].Rigidbody.velocity = Vector3.zero;
			list[i].Rigidbody.angularVelocity = Vector3.zero;
			list[i].enabled = !state;
		}
	}
}
