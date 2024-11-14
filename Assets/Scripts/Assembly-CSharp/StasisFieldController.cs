using System;
using System.Collections;
using OwlchemyVR;
using UnityEngine;

public class StasisFieldController : MonoBehaviour
{
	[SerializeField]
	private float drag = 10f;

	[SerializeField]
	private float angularDrag = 0.5f;

	[SerializeField]
	private ItemCollectionZone itemCollectionZone;

	[SerializeField]
	private AudioSourceHelper hoverAudio;

	private bool canAddItems = true;

	public void SetCanAddItems(bool can)
	{
		canAddItems = can;
	}

	private void OnEnable()
	{
		ItemCollectionZone obj = itemCollectionZone;
		obj.OnItemsInCollectionAdded = (Action<ItemCollectionZone, PickupableItem>)Delegate.Combine(obj.OnItemsInCollectionAdded, new Action<ItemCollectionZone, PickupableItem>(ItemAdded));
		ItemCollectionZone obj2 = itemCollectionZone;
		obj2.OnItemsInCollectionRemoved = (Action<ItemCollectionZone, PickupableItem>)Delegate.Combine(obj2.OnItemsInCollectionRemoved, new Action<ItemCollectionZone, PickupableItem>(ItemRemoved));
	}

	private void OnDisable()
	{
		ItemCollectionZone obj = itemCollectionZone;
		obj.OnItemsInCollectionAdded = (Action<ItemCollectionZone, PickupableItem>)Delegate.Remove(obj.OnItemsInCollectionAdded, new Action<ItemCollectionZone, PickupableItem>(ItemAdded));
		ItemCollectionZone obj2 = itemCollectionZone;
		obj2.OnItemsInCollectionRemoved = (Action<ItemCollectionZone, PickupableItem>)Delegate.Remove(obj2.OnItemsInCollectionRemoved, new Action<ItemCollectionZone, PickupableItem>(ItemRemoved));
	}

	private void Start()
	{
		if (hoverAudio != null)
		{
			hoverAudio.Stop();
		}
	}

	private void ItemAdded(ItemCollectionZone zone, PickupableItem item)
	{
		if (!canAddItems)
		{
			return;
		}
		StasisFieldItem stasisFieldItem = item.GetComponent<StasisFieldItem>() ?? item.gameObject.AddComponent<StasisFieldItem>();
		if (stasisFieldItem != null)
		{
			RigidbodyRemover component = item.GetComponent<RigidbodyRemover>();
			if (component == null)
			{
				stasisFieldItem.ActivateStasis(drag, angularDrag);
			}
			else
			{
				StartCoroutine(ActivateStasisAsync(stasisFieldItem));
			}
		}
		if (zone.ItemsInCollection.Count == 1 && hoverAudio != null)
		{
			if (hoverAudio.IsPlaying)
			{
				hoverAudio.Play();
			}
			hoverAudio.FadeIn(0.25f);
		}
	}

	private IEnumerator ActivateStasisAsync(StasisFieldItem item)
	{
		yield return null;
		item.ActivateStasis(drag, angularDrag);
	}

	private void ItemRemoved(ItemCollectionZone zone, PickupableItem item)
	{
		StasisFieldItem component = item.GetComponent<StasisFieldItem>();
		if (component != null)
		{
			component.DeactivateStasis();
		}
		if (zone.ItemsInCollection.Count == 0 && hoverAudio != null)
		{
			hoverAudio.FadeOut(0.25f);
		}
	}
}
