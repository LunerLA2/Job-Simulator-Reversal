using System;
using System.Collections;
using OwlchemyVR;
using UnityEngine;

public class PotController : KitchenTool
{
	[SerializeField]
	private float requiredFluidAmountML = 50f;

	[SerializeField]
	private int itemsRequiredToMakeSoup = 2;

	[SerializeField]
	private TemperatureStateItem potTemperature;

	[SerializeField]
	private float temperatureRequiredToCook = 70f;

	[SerializeField]
	private SoupCan soupCanPrefab;

	[SerializeField]
	private Transform spawnResultAt;

	[SerializeField]
	private WorldItem myWorldItem;

	[SerializeField]
	private WorldItemData soupCanWorldItemData;

	[SerializeField]
	private ItemCollectionZone itemCollectionZone;

	[SerializeField]
	private ParticleCollectionZone particleCollectionZone;

	[SerializeField]
	private float cookDuration;

	[SerializeField]
	private ParticleSystem cookingParticles;

	[SerializeField]
	private AudioSourceHelper cookingAudioSource;

	[SerializeField]
	private AudioClip cookFinishedSound;

	private bool canCook = true;

	private bool isCooking;

	private bool isTemperatureHighEnough;

	private float cookCheckInterval = 1f;

	public bool IsCooking
	{
		get
		{
			return isCooking;
		}
	}

	private void OnEnable()
	{
		ItemCollectionZone obj = itemCollectionZone;
		obj.OnItemsInCollectionAdded = (Action<ItemCollectionZone, PickupableItem>)Delegate.Combine(obj.OnItemsInCollectionAdded, new Action<ItemCollectionZone, PickupableItem>(ItemEnteredZone));
		ItemCollectionZone obj2 = itemCollectionZone;
		obj2.OnItemsInCollectionRemoved = (Action<ItemCollectionZone, PickupableItem>)Delegate.Combine(obj2.OnItemsInCollectionRemoved, new Action<ItemCollectionZone, PickupableItem>(ItemExitedZone));
		ParticleCollectionZone obj3 = particleCollectionZone;
		obj3.OnParticleIsCollecting = (Action<ParticleCollectionZone, WorldItemData, float>)Delegate.Combine(obj3.OnParticleIsCollecting, new Action<ParticleCollectionZone, WorldItemData, float>(ParticleIsCollecting));
		TemperatureStateItem temperatureStateItem = potTemperature;
		temperatureStateItem.OnTemperatureChangeWholeUnit = (Action<TemperatureStateItem>)Delegate.Combine(temperatureStateItem.OnTemperatureChangeWholeUnit, new Action<TemperatureStateItem>(PotTemperatureChanged));
	}

	private void OnDisable()
	{
		ItemCollectionZone obj = itemCollectionZone;
		obj.OnItemsInCollectionAdded = (Action<ItemCollectionZone, PickupableItem>)Delegate.Remove(obj.OnItemsInCollectionAdded, new Action<ItemCollectionZone, PickupableItem>(ItemEnteredZone));
		ItemCollectionZone obj2 = itemCollectionZone;
		obj2.OnItemsInCollectionRemoved = (Action<ItemCollectionZone, PickupableItem>)Delegate.Remove(obj2.OnItemsInCollectionRemoved, new Action<ItemCollectionZone, PickupableItem>(ItemExitedZone));
		ParticleCollectionZone obj3 = particleCollectionZone;
		obj3.OnParticleIsCollecting = (Action<ParticleCollectionZone, WorldItemData, float>)Delegate.Remove(obj3.OnParticleIsCollecting, new Action<ParticleCollectionZone, WorldItemData, float>(ParticleIsCollecting));
		TemperatureStateItem temperatureStateItem = potTemperature;
		temperatureStateItem.OnTemperatureChangeWholeUnit = (Action<TemperatureStateItem>)Delegate.Remove(temperatureStateItem.OnTemperatureChangeWholeUnit, new Action<TemperatureStateItem>(PotTemperatureChanged));
	}

	private void PotTemperatureChanged(TemperatureStateItem temp)
	{
		GameEventsManager.Instance.ItemActionOccurredWithAmount(myWorldItem.Data, "HEATED_TO_DEGREES", temp.TemperatureCelsius);
		GameEventsManager.Instance.ItemActionOccurredWithAmount(myWorldItem.Data, "HEATED_TO_DEGREES_MINUS_ROOM_TEMP", temp.TemperatureCelsius - 21f);
	}

	private void Update()
	{
		if (potTemperature.TemperatureCelsius >= temperatureRequiredToCook)
		{
			if (!isTemperatureHighEnough)
			{
				cookingParticles.Play();
				isTemperatureHighEnough = true;
			}
		}
		else if (isTemperatureHighEnough)
		{
			cookingParticles.Stop();
			isTemperatureHighEnough = false;
		}
		if (isTemperatureHighEnough)
		{
			cookCheckInterval -= Time.deltaTime;
			if (cookCheckInterval <= 0f)
			{
				TryCook();
				cookCheckInterval += 2f;
			}
		}
	}

	private void ItemEnteredZone(ItemCollectionZone zone, PickupableItem item)
	{
		WorldItem component = item.GetComponent<WorldItem>();
		if (component != null)
		{
			GameEventsManager.Instance.ItemAppliedToItemActionOccurred(component.Data, myWorldItem.Data, "ADDED_TO");
			GameEventsManager.Instance.ItemActionOccurred(myWorldItem.Data, "ACTIVATED");
		}
	}

	private void ItemExitedZone(ItemCollectionZone zone, PickupableItem item)
	{
		WorldItem component = item.GetComponent<WorldItem>();
		if (component != null)
		{
			GameEventsManager.Instance.ItemAppliedToItemActionOccurred(component.Data, myWorldItem.Data, "REMOVED_FROM");
			GameEventsManager.Instance.ItemActionOccurred(myWorldItem.Data, "DEACTIVATED");
		}
	}

	private void ParticleIsCollecting(ParticleCollectionZone zone, WorldItemData data, float amt)
	{
	}

	private void TryCook()
	{
		if (canCook && !isCooking)
		{
			StartCoroutine(TryCookAsync());
		}
	}

	private IEnumerator TryCookAsync()
	{
		isCooking = true;
		if (particleCollectionZone.GetTotalQuantity() >= requiredFluidAmountML && itemCollectionZone.ItemsInCollection.Count >= itemsRequiredToMakeSoup)
		{
			GameEventsManager.Instance.ItemActionOccurred(myWorldItem.Data, "USED");
			cookingParticles.Play();
			cookingAudioSource.Play();
			yield return new WaitForSeconds(cookDuration);
			cookingParticles.Stop();
			cookingAudioSource.Stop();
			AudioManager.Instance.Play(cookingAudioSource.transform.position, cookFinishedSound, 1f, 1f);
			Color c = ((!(itemCollectionZone.ItemsInCollection[0].InteractableItem.WorldItemData != null)) ? Color.black : itemCollectionZone.ItemsInCollection[0].InteractableItem.WorldItemData.OverallColor);
			string labelText = "<color=#" + GetColorHex(c) + ">" + itemCollectionZone.ItemsInCollection[0].InteractableItem.WorldItemData.ItemFullName + "</color>";
			if (itemCollectionZone.ItemsInCollection.Count == 2)
			{
				c = ((!(itemCollectionZone.ItemsInCollection[1].InteractableItem.WorldItemData != null)) ? Color.black : itemCollectionZone.ItemsInCollection[1].InteractableItem.WorldItemData.OverallColor);
				string text = labelText;
				labelText = text + "<color=#000000> \n&\n </color><color=#" + GetColorHex(c) + ">" + itemCollectionZone.ItemsInCollection[1].InteractableItem.WorldItemData.ItemFullName + "</color>";
			}
			else if (itemCollectionZone.ItemsInCollection.Count >= 3)
			{
				c = ((!(itemCollectionZone.ItemsInCollection[1].InteractableItem.WorldItemData != null)) ? Color.black : itemCollectionZone.ItemsInCollection[1].InteractableItem.WorldItemData.OverallColor);
				string text = labelText;
				labelText = text + "<color=#000000>,\n </color><color=#" + GetColorHex(c) + ">" + itemCollectionZone.ItemsInCollection[1].InteractableItem.WorldItemData.ItemFullName + "</color>";
				c = ((!(itemCollectionZone.ItemsInCollection[2].InteractableItem.WorldItemData != null)) ? Color.black : itemCollectionZone.ItemsInCollection[2].InteractableItem.WorldItemData.OverallColor);
				text = labelText;
				labelText = text + "<color=#000000> &\n </color><color=#" + GetColorHex(c) + ">" + itemCollectionZone.ItemsInCollection[2].InteractableItem.WorldItemData.ItemFullName + "</color>";
			}
			for (int i = 0; i < itemCollectionZone.ItemsInCollection.Count; i++)
			{
				if (itemCollectionZone.ItemsInCollection[i] != null && itemCollectionZone.ItemsInCollection[i].InteractableItem != null && itemCollectionZone.ItemsInCollection[i].InteractableItem.WorldItemData != null)
				{
					GameEventsManager.Instance.ItemActionOccurred(itemCollectionZone.ItemsInCollection[i].InteractableItem.WorldItemData, "DESTROYED");
					if (i <= 2 && itemCollectionZone.ItemsInCollection[i].InteractableItem.WorldItemData == soupCanWorldItemData)
					{
						AchievementManager.CompleteAchievement(6);
					}
				}
			}
			itemCollectionZone.DestroyAllItemsInCollectionZone();
			particleCollectionZone.Clear();
			SoupCan soupCan = UnityEngine.Object.Instantiate(soupCanPrefab, spawnResultAt.position, spawnResultAt.rotation) as SoupCan;
			BasePrefabSpawner b = soupCan.GetComponent<BasePrefabSpawner>();
			if (b != null)
			{
				soupCan = b.LastSpawnedPrefabGO.GetComponent<SoupCan>();
			}
			if (soupCan != null)
			{
				soupCan.SetLabel(labelText);
			}
			soupCan.transform.SetParent(GlobalStorage.Instance.ContentRoot, true);
			WorldItem wi = soupCan.GetComponent<WorldItem>();
			if (wi != null)
			{
				GameEventsManager.Instance.ItemActionOccurred(wi.Data, "CREATED");
			}
		}
		isCooking = false;
	}

	private string GetColorHex(Color c)
	{
		return ColorUtility.ToHtmlStringRGB(c);
	}
}
