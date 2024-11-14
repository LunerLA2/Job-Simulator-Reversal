using System;
using OwlchemyVR;
using UnityEngine;

public class ChangeKinematicWhenEjectedFromInventory : MonoBehaviour
{
	[SerializeField]
	private Rigidbody[] existingRBs;

	[SerializeField]
	private BasePrefabSpawner[] spawnersWithRBs;

	[SerializeField]
	private UniqueObject uniqueObjectToBeEjected;

	private Rigidbody[] rbs;

	[SerializeField]
	private bool kinematicStateOnStart = true;

	[SerializeField]
	private bool kinematicStateOnEject;

	[SerializeField]
	private bool unparentOnEject;

	[SerializeField]
	private bool alsoLookForPickupables;

	[SerializeField]
	private bool pickupableStateOnStart;

	[SerializeField]
	private bool pickupableStateOnEject = true;

	private PickupableItem[] pickupables;

	private void Start()
	{
		rbs = new Rigidbody[existingRBs.Length + spawnersWithRBs.Length];
		for (int i = 0; i < existingRBs.Length; i++)
		{
			rbs[i] = existingRBs[i];
		}
		for (int j = 0; j < spawnersWithRBs.Length; j++)
		{
			Rigidbody component = spawnersWithRBs[j].LastSpawnedPrefabGO.GetComponent<Rigidbody>();
			rbs[j + existingRBs.Length] = component;
		}
		for (int k = 0; k < rbs.Length; k++)
		{
			rbs[k].isKinematic = kinematicStateOnStart;
		}
		if (!alsoLookForPickupables)
		{
			return;
		}
		pickupables = new PickupableItem[existingRBs.Length + spawnersWithRBs.Length];
		for (int l = 0; l < existingRBs.Length; l++)
		{
			pickupables[l] = existingRBs[l].GetComponent<PickupableItem>();
		}
		for (int m = 0; m < spawnersWithRBs.Length; m++)
		{
			pickupables[m + existingRBs.Length] = spawnersWithRBs[m].LastSpawnedPrefabGO.GetComponent<PickupableItem>();
		}
		for (int n = 0; n < pickupables.Length; n++)
		{
			if (pickupables[n] != null)
			{
				pickupables[n].enabled = pickupableStateOnStart;
			}
		}
	}

	private void OnEnable()
	{
		UniqueObject uniqueObject = uniqueObjectToBeEjected;
		uniqueObject.OnWasEjectedFromInventoryOfBot = (Action)Delegate.Combine(uniqueObject.OnWasEjectedFromInventoryOfBot, new Action(Ejected));
	}

	private void OnDisable()
	{
		UniqueObject uniqueObject = uniqueObjectToBeEjected;
		uniqueObject.OnWasEjectedFromInventoryOfBot = (Action)Delegate.Remove(uniqueObject.OnWasEjectedFromInventoryOfBot, new Action(Ejected));
	}

	private void Ejected()
	{
		for (int i = 0; i < rbs.Length; i++)
		{
			if (rbs[i] != null)
			{
				rbs[i].isKinematic = kinematicStateOnEject;
				if (unparentOnEject)
				{
					rbs[i].transform.SetParent(GlobalStorage.Instance.ContentRoot, true);
				}
			}
		}
		if (!alsoLookForPickupables)
		{
			return;
		}
		for (int j = 0; j < pickupables.Length; j++)
		{
			if (pickupables[j] != null)
			{
				pickupables[j].enabled = pickupableStateOnEject;
			}
		}
	}
}
