using System;
using OwlchemyVR;
using UnityEngine;

public class ChemLabManager : MonoBehaviour
{
	public enum ObjectStates
	{
		Basic = 0,
		Frozen = 1,
		Burning = 2
	}

	public enum Chemicals
	{
		Red = 0,
		Green = 1,
		Blue = 2,
		Bleach = 3,
		LiquidNitrogen = 4,
		Embiggening = 5,
		Smallening = 6,
		Water = 7
	}

	[Serializable]
	public class Task
	{
		public int TaskID;
	}

	public ItemCollectionZone centrifugeCollectionZone;

	private Task currentTask;

	[SerializeField]
	private Transform spawnPoint;

	[SerializeField]
	private GameObject[] spawnableObject;

	[SerializeField]
	private float spawnRadius = 0.5f;

	private void OnEnable()
	{
		ItemCollectionZone itemCollectionZone = centrifugeCollectionZone;
		itemCollectionZone.OnItemsInCollectionAdded = (Action<ItemCollectionZone, PickupableItem>)Delegate.Combine(itemCollectionZone.OnItemsInCollectionAdded, new Action<ItemCollectionZone, PickupableItem>(CheckContainer));
	}

	private void OnDisable()
	{
		ItemCollectionZone itemCollectionZone = centrifugeCollectionZone;
		itemCollectionZone.OnItemsInCollectionAdded = (Action<ItemCollectionZone, PickupableItem>)Delegate.Remove(itemCollectionZone.OnItemsInCollectionAdded, new Action<ItemCollectionZone, PickupableItem>(CheckContainer));
	}

	private void CheckContainer(ItemCollectionZone itemCollectionZone, PickupableItem pickupableItem)
	{
	}

	public void SpawnTransmuteableObject()
	{
		UnityEngine.Object.Instantiate(spawnableObject[UnityEngine.Random.Range(0, spawnableObject.Length)], UnityEngine.Random.insideUnitSphere * spawnRadius + spawnPoint.position, Quaternion.identity);
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = new Color(0.5f, 0f, 0f, 0.25f);
		if (spawnPoint != null)
		{
			Gizmos.DrawSphere(spawnPoint.position, spawnRadius);
		}
	}
}
