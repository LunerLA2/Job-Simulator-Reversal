using System;
using System.Collections;
using OwlchemyVR;
using UnityEngine;

public class SimpleCleanup : MonoBehaviour
{
	[SerializeField]
	private CompoundItemCollectionZone itemCollectionZone;

	[SerializeField]
	private ParticleSystem poofEffect;

	private void OnEnable()
	{
		CompoundItemCollectionZone compoundItemCollectionZone = itemCollectionZone;
		compoundItemCollectionZone.OnItemsInCollectionAdded = (Action<CompoundItemCollectionZone, PickupableItem>)Delegate.Combine(compoundItemCollectionZone.OnItemsInCollectionAdded, new Action<CompoundItemCollectionZone, PickupableItem>(ItemAdded));
	}

	private void OnDisable()
	{
		CompoundItemCollectionZone compoundItemCollectionZone = itemCollectionZone;
		compoundItemCollectionZone.OnItemsInCollectionAdded = (Action<CompoundItemCollectionZone, PickupableItem>)Delegate.Remove(compoundItemCollectionZone.OnItemsInCollectionAdded, new Action<CompoundItemCollectionZone, PickupableItem>(ItemAdded));
	}

	private void ItemAdded(CompoundItemCollectionZone zone, PickupableItem item)
	{
		StartCoroutine(WaitAndDestroy(item));
	}

	private IEnumerator WaitAndDestroy(PickupableItem item)
	{
		yield return new WaitForSeconds(6f);
		if (item != null)
		{
			poofEffect.transform.position = item.transform.position;
			poofEffect.Play();
			UnityEngine.Object.Destroy(item.gameObject);
		}
	}
}
