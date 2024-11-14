using System;
using System.Collections.Generic;
using OwlchemyVR;
using UnityEngine;

public class HotDogRollerController : MonoBehaviour
{
	[SerializeField]
	private ItemCollectionZone collectionZone;

	private float minAngle = 1f;

	[SerializeField]
	private float rotationSpeed = 0.5f;

	private Vector3 rollAxis;

	private List<RollableItem> rollables;

	[SerializeField]
	private GrabbableSlider drawerSlider;

	[SerializeField]
	private GameObject bunParent;

	private void OnEnable()
	{
		ItemCollectionZone itemCollectionZone = collectionZone;
		itemCollectionZone.OnItemsInCollectionAdded = (Action<ItemCollectionZone, PickupableItem>)Delegate.Combine(itemCollectionZone.OnItemsInCollectionAdded, new Action<ItemCollectionZone, PickupableItem>(ItemAdded));
		ItemCollectionZone itemCollectionZone2 = collectionZone;
		itemCollectionZone2.OnItemsInCollectionRemoved = (Action<ItemCollectionZone, PickupableItem>)Delegate.Combine(itemCollectionZone2.OnItemsInCollectionRemoved, new Action<ItemCollectionZone, PickupableItem>(ItemRemoved));
		drawerSlider.OnLowerLocked += DrawerSlider_OnLowerLocked;
		drawerSlider.OnLowerUnlocked += DrawerSlider_OnLowerUnlocked;
	}

	private void OnDisable()
	{
		ItemCollectionZone itemCollectionZone = collectionZone;
		itemCollectionZone.OnItemsInCollectionAdded = (Action<ItemCollectionZone, PickupableItem>)Delegate.Remove(itemCollectionZone.OnItemsInCollectionAdded, new Action<ItemCollectionZone, PickupableItem>(ItemAdded));
		ItemCollectionZone itemCollectionZone2 = collectionZone;
		itemCollectionZone2.OnItemsInCollectionRemoved = (Action<ItemCollectionZone, PickupableItem>)Delegate.Remove(itemCollectionZone2.OnItemsInCollectionRemoved, new Action<ItemCollectionZone, PickupableItem>(ItemRemoved));
		drawerSlider.OnLowerLocked -= DrawerSlider_OnLowerLocked;
		drawerSlider.OnLowerUnlocked -= DrawerSlider_OnLowerUnlocked;
	}

	private void DrawerSlider_OnLowerUnlocked(GrabbableSlider slider)
	{
		SetBunParentState(true);
	}

	private void DrawerSlider_OnLowerLocked(GrabbableSlider slider, bool isInitial)
	{
		SetBunParentState(false);
	}

	private void Start()
	{
		rollables = new List<RollableItem>();
		rollAxis = base.transform.right;
	}

	private void ItemAdded(ItemCollectionZone zone, PickupableItem item)
	{
		RollableItem component = item.GetComponent<RollableItem>();
		if (component != null && !rollables.Contains(component))
		{
			rollables.Add(component);
		}
	}

	private void ItemRemoved(ItemCollectionZone zone, PickupableItem item)
	{
		RollableItem component = item.GetComponent<RollableItem>();
		if (component != null && rollables.Contains(component))
		{
			rollables.Remove(component);
			component.SetRotationSpeed(0f);
		}
	}

	private void Update()
	{
		for (int i = 0; i < rollables.Count; i++)
		{
			if (rollables[i] != null)
			{
				if (IsWithinRotationAngle(rollables[i].transform))
				{
					if (!rollables[i].IsRotating)
					{
						rollables[i].SetRotationSpeed(rotationSpeed);
					}
				}
				else if (rollables[i].IsRotating)
				{
					rollables[i].SetRotationSpeed(0f);
				}
			}
			else
			{
				rollables.RemoveAt(i);
			}
		}
	}

	private bool IsWithinRotationAngle(Transform t)
	{
		return Vector3.Angle(t.forward, rollAxis) <= minAngle || Vector3.Angle(t.forward, -rollAxis) <= minAngle;
	}

	private void SetBunParentState(bool state)
	{
		if (state)
		{
			bunParent.transform.SetParent(drawerSlider.transform, false);
			bunParent.transform.SetToDefaultPosRotScale();
			bunParent.gameObject.SetActive(true);
		}
		else
		{
			bunParent.gameObject.SetActive(false);
			bunParent.transform.SetParent(GlobalStorage.Instance.ContentRoot, false);
		}
	}
}
