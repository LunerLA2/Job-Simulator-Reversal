using System;
using System.Collections;
using System.Collections.Generic;
using OwlchemyVR;
using TMPro;
using UnityEngine;

public class MicrowaveController : KitchenTool
{
	private const float SPIN_DURATION = 3f;

	[SerializeField]
	[Header("Recipes")]
	private MicrowaveRecipe[] recipes;

	[SerializeField]
	private MicrowaveToppedFoodItemRecipe[] toppedFoodItemRecipes;

	[Header("Other definitions")]
	[SerializeField]
	private MicrowaveToppingIngredientPair[] toppingIngredientPairs;

	[Header("Misc")]
	[SerializeField]
	private WorldItem myWorldItem;

	[SerializeField]
	private TextMeshPro timerTextMesh;

	[SerializeField]
	private Collider doorCollider;

	[SerializeField]
	private GrabbableHinge doorHinge;

	[SerializeField]
	private Material cookingMicrowaveMat;

	[SerializeField]
	private ItemCollectionZone itemCollectionZone;

	[SerializeField]
	private ParticleSystem smokeBurstParticle;

	[SerializeField]
	private Renderer[] swapRenderersWhenCooking;

	[SerializeField]
	private Transform spawnResultAtLocation;

	[SerializeField]
	private GameObject radioactivePrefab;

	[SerializeField]
	private Transform beepAudioLocation;

	[SerializeField]
	private Transform doorAudioLocation;

	[SerializeField]
	private AudioSourceHelper spinAudioSource;

	[SerializeField]
	private AudioClip microwaveOpenAudioClip;

	[SerializeField]
	private AudioClip microwaveCloseAudioClip;

	[SerializeField]
	private AudioClip microwaveCompleteAudioClip;

	[SerializeField]
	private Light interiorLight;

	private MicrowaveState state;

	private Material startingMicrowaveMat;

	private float doorTimeout;

	private float spinTimeLeft;

	private bool didBurnSomething;

	private Quaternion initialDoorRot;

	private List<PickupableItem> miscItemsCurrentlyInside = new List<PickupableItem>();

	private List<WorldItemData> itemsThatCountAsMisc = new List<WorldItemData>();

	public Action<MicrowaveController> OnCookingComplete;

	public MicrowaveState State
	{
		get
		{
			return state;
		}
	}

	public ItemCollectionZone ItemCollectionZone
	{
		get
		{
			return itemCollectionZone;
		}
	}

	private void Awake()
	{
		startingMicrowaveMat = swapRenderersWhenCooking[0].material;
		initialDoorRot = doorHinge.transform.localRotation;
		for (int i = 0; i < toppingIngredientPairs.Length; i++)
		{
			for (int j = 0; j < toppingIngredientPairs[i].PossibleInputWorldItems.Length; j++)
			{
				if (!itemsThatCountAsMisc.Contains(toppingIngredientPairs[i].PossibleInputWorldItems[j]))
				{
					itemsThatCountAsMisc.Add(toppingIngredientPairs[i].PossibleInputWorldItems[j]);
				}
			}
		}
		for (int k = 0; k < toppedFoodItemRecipes.Length; k++)
		{
			for (int l = 0; l < toppedFoodItemRecipes[k].RequiredPhysicalItems.Length; l++)
			{
				if (itemsThatCountAsMisc.Contains(toppedFoodItemRecipes[k].RequiredPhysicalItems[l]))
				{
					itemsThatCountAsMisc.Remove(toppedFoodItemRecipes[k].RequiredPhysicalItems[l]);
				}
			}
		}
	}

	private void OnEnable()
	{
		doorHinge.OnLowerLocked += DoorClosed;
		doorHinge.OnLowerUnlocked += DoorOpened;
		ItemCollectionZone obj = itemCollectionZone;
		obj.OnItemsInCollectionAdded = (Action<ItemCollectionZone, PickupableItem>)Delegate.Combine(obj.OnItemsInCollectionAdded, new Action<ItemCollectionZone, PickupableItem>(ItemEnteredZone));
		ItemCollectionZone obj2 = itemCollectionZone;
		obj2.OnItemsInCollectionRemoved = (Action<ItemCollectionZone, PickupableItem>)Delegate.Combine(obj2.OnItemsInCollectionRemoved, new Action<ItemCollectionZone, PickupableItem>(ItemExitedZone));
		timerTextMesh.text = "READY";
	}

	private void OnDisable()
	{
		doorHinge.OnLowerLocked -= DoorClosed;
		doorHinge.OnLowerUnlocked -= DoorOpened;
		ItemCollectionZone obj = itemCollectionZone;
		obj.OnItemsInCollectionAdded = (Action<ItemCollectionZone, PickupableItem>)Delegate.Remove(obj.OnItemsInCollectionAdded, new Action<ItemCollectionZone, PickupableItem>(ItemEnteredZone));
		ItemCollectionZone obj2 = itemCollectionZone;
		obj2.OnItemsInCollectionRemoved = (Action<ItemCollectionZone, PickupableItem>)Delegate.Remove(obj2.OnItemsInCollectionRemoved, new Action<ItemCollectionZone, PickupableItem>(ItemExitedZone));
	}

	private void ItemEnteredZone(ItemCollectionZone zone, PickupableItem item)
	{
		WorldItem component = item.GetComponent<WorldItem>();
		if (!(component != null))
		{
			return;
		}
		GameEventsManager.Instance.ItemAppliedToItemActionOccurred(component.Data, myWorldItem.Data, "ADDED_TO");
		if (itemsThatCountAsMisc.Contains(component.Data) && !miscItemsCurrentlyInside.Contains(item))
		{
			miscItemsCurrentlyInside.Add(item);
			if (miscItemsCurrentlyInside.Count == 1)
			{
				GameEventsManager.Instance.ItemActionOccurred(myWorldItem.Data, "MISC_ITEM_ADDED");
			}
		}
	}

	private void ItemExitedZone(ItemCollectionZone zone, PickupableItem item)
	{
		WorldItem component = item.GetComponent<WorldItem>();
		if (!(component != null))
		{
			return;
		}
		GameEventsManager.Instance.ItemAppliedToItemActionOccurred(component.Data, myWorldItem.Data, "REMOVED_FROM");
		if (itemsThatCountAsMisc.Contains(component.Data) && miscItemsCurrentlyInside.Contains(item))
		{
			miscItemsCurrentlyInside.Remove(item);
			if (miscItemsCurrentlyInside.Count == 0)
			{
				GameEventsManager.Instance.ItemActionOccurred(myWorldItem.Data, "MISC_ITEM_REMOVED");
			}
		}
	}

	private void Update()
	{
		if (state == MicrowaveState.Activated)
		{
			RefreshTimerText(spinTimeLeft);
			spinTimeLeft = Mathf.Max(0f, spinTimeLeft - Time.deltaTime);
			if (spinTimeLeft <= 0f && state != MicrowaveState.Cooking)
			{
				state = MicrowaveState.Cooking;
				StartCoroutine(CookingDone());
			}
		}
		if (doorTimeout > 0f)
		{
			doorTimeout = Mathf.Max(0f, doorTimeout - Time.deltaTime);
			if (doorTimeout <= 0f)
			{
				doorCollider.enabled = true;
				doorHinge.enabled = true;
			}
		}
	}

	public void MicrowaveButtonPressed()
	{
		if (state == MicrowaveState.Idle && doorHinge.IsLowerLocked)
		{
			StartCoroutine(WaitAndStartCooking(0.1f));
		}
	}

	private void StartCooking()
	{
		state = MicrowaveState.Activated;
		spinAudioSource.Play();
		didBurnSomething = false;
		spinTimeLeft = 3f;
		SetCookingMaterials();
	}

	private void RefreshTimerText(float time)
	{
		timerTextMesh.text = "0:0" + Mathf.Max(0, Mathf.CeilToInt(time));
	}

	private void CancelCooking()
	{
		StopAllCoroutines();
		spinTimeLeft = 0f;
		state = MicrowaveState.Idle;
		doorHinge.Grabbable.enabled = true;
		spinAudioSource.Stop();
		timerTextMesh.text = "READY";
		SetNonCookingMaterials();
	}

	private IEnumerator CookingDone()
	{
		List<PickupableItem> itemsToPhysicallyDestroy = new List<PickupableItem>();
		List<PickupableItem> itemsToDoDestroyEvent = new List<PickupableItem>();
		List<PickupableItem> itemsRemaining = new List<PickupableItem>();
		itemsRemaining.AddRange(itemCollectionZone.ItemsInCollection);
		List<PickupableItem> itemsUsedInRecipe3 = new List<PickupableItem>();
		for (int m = 0; m < toppedFoodItemRecipes.Length; m++)
		{
			itemsUsedInRecipe3 = toppedFoodItemRecipes[m].CheckIfRequirementsAreMet(itemsRemaining);
			if (itemsUsedInRecipe3.Count < toppedFoodItemRecipes[m].RequiredPhysicalItems.Length)
			{
				continue;
			}
			for (int n2 = 0; n2 < itemsUsedInRecipe3.Count; n2++)
			{
				if (itemsRemaining.Contains(itemsUsedInRecipe3[n2]))
				{
					itemsRemaining.Remove(itemsUsedInRecipe3[n2]);
				}
				itemsToPhysicallyDestroy.Add(itemsUsedInRecipe3[n2]);
				if (toppedFoodItemRecipes[m].ShouldItemsBeMarkedDestroyed)
				{
					itemsToDoDestroyEvent.Add(itemsUsedInRecipe3[n2]);
				}
			}
			ToppedFoodItemController toppedFood = UnityEngine.Object.Instantiate(toppedFoodItemRecipes[m].ToppedFoodItemPrefab);
			SetupSpawnedItem(toppedFood.gameObject);
			int toppingSlotsLeftToFill = toppedFoodItemRecipes[m].ToppedFoodItemPrefab.MaximumToppings;
			List<GameObject> toppingPrefabs = new List<GameObject>();
			for (int r = 0; r < itemsRemaining.Count; r++)
			{
				for (int t = 0; t < toppingIngredientPairs.Length; t++)
				{
					bool itemIsAvailableThatMatchesThisPair = false;
					for (int p = 0; p < toppingIngredientPairs[t].PossibleInputWorldItems.Length; p++)
					{
						if (itemsRemaining[r].InteractableItem.WorldItemData == toppingIngredientPairs[t].PossibleInputWorldItems[p])
						{
							itemIsAvailableThatMatchesThisPair = true;
							break;
						}
					}
					if (itemIsAvailableThatMatchesThisPair)
					{
						itemsToPhysicallyDestroy.Add(itemsRemaining[r]);
						if (toppedFoodItemRecipes[m].ShouldItemsBeMarkedDestroyed)
						{
							itemsToDoDestroyEvent.Add(itemsRemaining[r]);
						}
						itemsRemaining.RemoveAt(r);
						r--;
						toppingPrefabs.Add(toppingIngredientPairs[t].OutputToppingPrefab);
						toppingSlotsLeftToFill--;
						break;
					}
				}
				if (toppingSlotsLeftToFill == 0)
				{
					break;
				}
			}
			toppedFood.SetupToppings(toppingPrefabs);
		}
		for (int l = 0; l < recipes.Length; l++)
		{
			itemsUsedInRecipe3 = recipes[l].CheckIfRequirementsAreMet(itemsRemaining);
			if (itemsUsedInRecipe3.Count < recipes[l].RequiredPhysicalItems.Length)
			{
				continue;
			}
			yield return new WaitForSeconds(0.01f);
			GameObject go = UnityEngine.Object.Instantiate(recipes[l].ResultPrefab);
			SetupSpawnedItem(go);
			for (int n = 0; n < itemsUsedInRecipe3.Count; n++)
			{
				if (itemsRemaining.Contains(itemsUsedInRecipe3[n]))
				{
					itemsRemaining.Remove(itemsUsedInRecipe3[n]);
				}
				itemsToPhysicallyDestroy.Add(itemsUsedInRecipe3[n]);
				if (recipes[l].ShouldItemsBeMarkedDestroyed)
				{
					itemsToDoDestroyEvent.Add(itemsUsedInRecipe3[n]);
				}
			}
		}
		for (int k = 0; k < itemsRemaining.Count; k++)
		{
			yield return new WaitForSeconds(0.01f);
			if (!(itemsRemaining[k] != null))
			{
				continue;
			}
			GameObject pfx = UnityEngine.Object.Instantiate(radioactivePrefab, Vector3.zero, Quaternion.identity) as GameObject;
			pfx.transform.SetParent(itemsRemaining[k].transform, false);
			CookableItem cook = itemsRemaining[k].GetComponent<CookableItem>();
			if (cook != null)
			{
				cook.CookInstantly();
				continue;
			}
			yield return new WaitForSeconds(0.01f);
			TemperatureStateItem temp = itemsRemaining[k].GetComponent<TemperatureStateItem>();
			if (temp != null)
			{
				temp.SetManualTemperature(120f);
			}
		}
		for (int j = 0; j < itemsToDoDestroyEvent.Count; j++)
		{
			if (!(itemsToDoDestroyEvent[j] != null))
			{
				continue;
			}
			GameEventsManager.Instance.ItemActionOccurred(itemsToDoDestroyEvent[j].InteractableItem.WorldItemData, "DESTROYED");
			if (itemsThatCountAsMisc.Contains(itemsToDoDestroyEvent[j].InteractableItem.WorldItemData) && miscItemsCurrentlyInside.Contains(itemsToDoDestroyEvent[j]))
			{
				miscItemsCurrentlyInside.Remove(itemsToDoDestroyEvent[j]);
				if (miscItemsCurrentlyInside.Count == 0)
				{
					GameEventsManager.Instance.ItemActionOccurred(myWorldItem.Data, "MISC_ITEM_REMOVED");
				}
			}
		}
		for (int i = 0; i < itemsToPhysicallyDestroy.Count; i++)
		{
			if (itemsToPhysicallyDestroy[i] != null)
			{
				yield return new WaitForSeconds(0.01f);
				if (itemsToPhysicallyDestroy[i] != null && itemsToPhysicallyDestroy[i].gameObject != null)
				{
					UnityEngine.Object.Destroy(itemsToPhysicallyDestroy[i].gameObject);
				}
			}
		}
		spinAudioSource.Stop();
		AudioManager.Instance.Play(beepAudioLocation, microwaveCompleteAudioClip, 1f, 1f);
		timerTextMesh.text = "DONE";
		doorHinge.Grabbable.enabled = true;
		SetNonCookingMaterials();
		state = MicrowaveState.Idle;
		GameEventsManager.Instance.ItemActionOccurred(myWorldItem.Data, "USED");
		if (OnCookingComplete != null)
		{
			OnCookingComplete(this);
		}
	}

	private IEnumerator MakeDoorTriggerACollider()
	{
		yield return new WaitForSeconds(1f);
		doorCollider.enabled = true;
	}

	private void DoorClosed(GrabbableHinge hinge, bool isInitial)
	{
		interiorLight.enabled = false;
		doorHinge.transform.localRotation = initialDoorRot;
		doorHinge.GetComponent<ForceLockJoint>().ResetMemory();
		timerTextMesh.text = "READY";
		if (!isInitial)
		{
			AudioManager.Instance.Play(doorAudioLocation, microwaveCloseAudioClip, 1f, 1f);
		}
	}

	private void DoorOpened(GrabbableHinge hinge)
	{
		if (state == MicrowaveState.Idle)
		{
			interiorLight.enabled = true;
			AudioManager.Instance.Play(doorAudioLocation, microwaveOpenAudioClip, 1f, 1f);
			if (didBurnSomething)
			{
				smokeBurstParticle.Play();
				didBurnSomething = false;
			}
			doorCollider.enabled = false;
			doorHinge.enabled = false;
			doorTimeout = 0.5f;
			timerTextMesh.text = "CLOSE DOOR";
			StartCoroutine(MakeDoorTriggerACollider());
		}
	}

	private IEnumerator WaitAndStartCooking(float waitTime)
	{
		state = MicrowaveState.Activating;
		doorHinge.Grabbable.enabled = false;
		RefreshTimerText(3f);
		yield return new WaitForSeconds(waitTime);
		StartCooking();
	}

	private void SetCookingMaterials()
	{
		for (int i = 0; i < swapRenderersWhenCooking.Length; i++)
		{
			swapRenderersWhenCooking[i].material = cookingMicrowaveMat;
		}
	}

	private void SetNonCookingMaterials()
	{
		for (int i = 0; i < swapRenderersWhenCooking.Length; i++)
		{
			swapRenderersWhenCooking[i].material = startingMicrowaveMat;
		}
	}

	private void SetupSpawnedItem(GameObject go)
	{
		BasePrefabSpawner component = go.GetComponent<BasePrefabSpawner>();
		if (component != null)
		{
			go = component.LastSpawnedPrefabGO;
		}
		go.transform.position = spawnResultAtLocation.transform.position;
		go.transform.rotation = Quaternion.identity;
		go.transform.SetParent(GlobalStorage.Instance.ContentRoot, true);
		WorldItem component2 = go.GetComponent<WorldItem>();
		if (component2 != null)
		{
			GameEventsManager.Instance.ItemActionOccurred(component2.Data, "CREATED");
		}
	}

	public override void OnDismiss()
	{
		base.OnDismiss();
		CancelCooking();
		if (!doorHinge.IsLowerLocked)
		{
			doorHinge.LockLower();
		}
	}
}
