using System;
using System.Collections.Generic;
using OwlchemyVR;
using UnityEngine;

public class StackableController : MonoBehaviour
{
	[SerializeField]
	private bool isAreaCurrentlyStackable = true;

	private List<Stackable> activeIngredients = new List<Stackable>();

	[SerializeField]
	private GroupMasterPickupableItem masterPickupablePrefab;

	private GroupMasterPickupableItem activeMasterPickupableItem;

	[SerializeField]
	private List<WorldItemData> requiredItemTypes;

	[SerializeField]
	private WorldItemData resultItem;

	public Action<Stackable> OnIngredientAdded;

	public Action<PickupableItem> OnStackablePickupableMade;

	[SerializeField]
	private Collider nonTriggerCollider;

	private void OnTriggerStay(Collider other)
	{
		if (isAreaCurrentlyStackable && activeMasterPickupableItem == null)
		{
			Stackable stackableFromCollider = GetStackableFromCollider(other);
			if (stackableFromCollider != null && stackableFromCollider.PickupableItem.InteractableItem.ItemName == requiredItemTypes[0].ItemName && stackableFromCollider.IsStackingReady(nonTriggerCollider, 10f))
			{
				AddIngredient(stackableFromCollider);
			}
		}
	}

	private void EnableAreaCurrentlyStackable()
	{
		isAreaCurrentlyStackable = true;
	}

	public Stackable GetStackableFromCollider(Collider c)
	{
		Stackable result = null;
		if (c.attachedRigidbody != null)
		{
			result = c.attachedRigidbody.GetComponent<Stackable>();
		}
		return result;
	}

	public void AddIngredient(Stackable stackable)
	{
		if (activeIngredients.Count <= 0 || !(stackable.PickupableItem.InteractableItem.ItemName == requiredItemTypes[0].ItemName) || !(activeIngredients[activeIngredients.Count - 1].PickupableItem.InteractableItem.ItemName == requiredItemTypes[0].ItemName))
		{
			if (activeMasterPickupableItem == null)
			{
				activeMasterPickupableItem = (GroupMasterPickupableItem)UnityEngine.Object.Instantiate(masterPickupablePrefab, stackable.transform.position, Quaternion.identity);
				GetComponent<Collider>().enabled = false;
			}
			stackable.ApplyStacking(activeMasterPickupableItem, this);
			stackable.transform.parent = activeMasterPickupableItem.transform;
			if (activeIngredients.Count > 0)
			{
				activeIngredients[activeIngredients.Count - 1].SetAsDisabledStackableLevel();
			}
			activeIngredients.Add(stackable);
			if (OnIngredientAdded != null)
			{
				OnIngredientAdded(stackable);
			}
			if (CheckIfStackMeetsRequirements())
			{
				CompleteStack();
			}
		}
	}

	private bool CheckIfStackMeetsRequirements()
	{
		List<bool> list = new List<bool>();
		for (int i = 0; i < requiredItemTypes.Count; i++)
		{
			list.Add(false);
		}
		for (int j = 0; j < activeIngredients.Count; j++)
		{
			Stackable stackable = activeIngredients[j];
			for (int k = 0; k < requiredItemTypes.Count; k++)
			{
				if (requiredItemTypes[k].ItemName == stackable.PickupableItem.InteractableItem.ItemName && !list[k])
				{
					list[k] = true;
					break;
				}
			}
		}
		bool result = true;
		for (int l = 0; l < list.Count; l++)
		{
			if (!list[l])
			{
				result = false;
			}
		}
		return result;
	}

	private void CompleteStack()
	{
		activeIngredients[activeIngredients.Count - 1].SetAsDisabledStackableLevel();
		for (int i = 0; i < activeIngredients.Count; i++)
		{
			activeIngredients[i].gameObject.layer = 8;
			activeIngredients[i].GetComponent<SelectedChangeOutlineController>().SetSpecialHighlight(false);
			activeIngredients[i].GetComponent<SelectedChangeOutlineController>().ForceConnectionToInteractableItem(activeMasterPickupableItem.InteractableItem);
		}
		Rigidbody component = activeMasterPickupableItem.GetComponent<Rigidbody>();
		component.isKinematic = false;
		activeIngredients[activeIngredients.Count - 1].SetAsDisabledStackableLevel();
		activeMasterPickupableItem.SetupPickupableChildren();
		activeMasterPickupableItem.enabled = true;
		activeIngredients.Clear();
		activeMasterPickupableItem.InteractableItem.ManualSetWorldItemData(resultItem);
		if (OnStackablePickupableMade != null)
		{
			OnStackablePickupableMade(activeMasterPickupableItem);
		}
		activeMasterPickupableItem = null;
		isAreaCurrentlyStackable = true;
		GetComponent<Collider>().enabled = true;
	}
}
