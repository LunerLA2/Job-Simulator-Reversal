using System;
using System.Collections;
using OwlchemyVR;
using UnityEngine;

[RequireComponent(typeof(TemperatureStateItem))]
public class CookableItem : MonoBehaviour
{
	private TemperatureStateItem temperatureState;

	[SerializeField]
	private Renderer[] renderersToTint;

	[SerializeField]
	private bool tintEvenIfCustomArtSupplied = true;

	[SerializeField]
	private GameObject[] customArtUncooked;

	[SerializeField]
	private GameObject[] customArtCooked;

	[SerializeField]
	private GameObject[] customArtBurned;

	[SerializeField]
	private GameObject replaceWithWhenBurned;

	[SerializeField]
	private WorldItem myWorldItem;

	[SerializeField]
	private WorldItemData worldItemDataWhenCooked;

	[SerializeField]
	private WorldItemData worldItemDataWhenBurned;

	private float minTemperatureThresholdToCook = 60f;

	private float maxTemperatureThresholdToCook = 210f;

	private float timeToCook = 5f;

	private float timeToBurn = 15f;

	private float cookedTime;

	private bool isCooked;

	private bool isBurned;

	private bool isCooking;

	private PickupableItem pickupableItem;

	public Action<CookableItem, float> OnPartiallyCooked;

	public Action<CookableItem> OnCooked;

	public Action<CookableItem> OnBurned;

	private void Awake()
	{
		pickupableItem = GetComponent<PickupableItem>();
		temperatureState = GetComponent<TemperatureStateItem>();
		if (temperatureState == null)
		{
			Debug.LogError("CookableItem does not have a " + typeof(TemperatureStateItem).Name + " on " + base.gameObject.name, base.gameObject);
		}
		SetCookedState(false);
		SetArtState(false);
	}

	private void SetArtState(bool cooked, bool burned = false)
	{
		int num = 0;
		if (customArtUncooked.Length > 0 && !cooked)
		{
			num = 0;
		}
		if (customArtCooked.Length > 0 && cooked)
		{
			num = 1;
		}
		if (customArtBurned.Length > 0 && burned)
		{
			num = 2;
		}
		if (customArtUncooked.Length > 0)
		{
			for (int i = 0; i < customArtUncooked.Length; i++)
			{
				customArtUncooked[i].SetActive(num == 0);
			}
		}
		if (customArtCooked.Length > 0)
		{
			for (int j = 0; j < customArtCooked.Length; j++)
			{
				customArtCooked[j].SetActive(num == 1);
			}
		}
		if (customArtBurned.Length > 0)
		{
			for (int k = 0; k < customArtBurned.Length; k++)
			{
				customArtBurned[k].SetActive(num == 2);
			}
		}
	}

	private void OnEnable()
	{
		TemperatureStateItem temperatureStateItem = temperatureState;
		temperatureStateItem.OnTemperatureChangeWholeUnit = (Action<TemperatureStateItem>)Delegate.Combine(temperatureStateItem.OnTemperatureChangeWholeUnit, new Action<TemperatureStateItem>(TemperatureChanged));
	}

	private void OnDisable()
	{
		TemperatureStateItem temperatureStateItem = temperatureState;
		temperatureStateItem.OnTemperatureChangeWholeUnit = (Action<TemperatureStateItem>)Delegate.Remove(temperatureStateItem.OnTemperatureChangeWholeUnit, new Action<TemperatureStateItem>(TemperatureChanged));
	}

	private void TemperatureChanged(TemperatureStateItem t)
	{
		bool flag = isCooking;
		isCooking = t.TemperatureCelsius >= minTemperatureThresholdToCook;
		if (!flag && isCooking)
		{
			StopAllCoroutines();
			StartCoroutine(CookAsync());
		}
	}

	public void ManualResetCookedState()
	{
		isCooked = false;
		isBurned = false;
		cookedTime = 0f;
		SetArtState(false);
		SetRendererColor(Color.white);
	}

	private IEnumerator CookAsync()
	{
		while (isCooking)
		{
			cookedTime += Time.deltaTime * Mathf.Lerp(0.1f, 1.3f, Mathf.InverseLerp(minTemperatureThresholdToCook, maxTemperatureThresholdToCook, Mathf.Min(maxTemperatureThresholdToCook, temperatureState.TemperatureCelsius)));
			if (!isBurned && myWorldItem != null)
			{
				float percBurned = Mathf.Clamp(Mathf.InverseLerp(0f, timeToBurn, cookedTime), 0f, 1f);
				float percCooked = Mathf.Clamp(Mathf.InverseLerp(0f, timeToCook, cookedTime), 0f, 1f);
				GameEventsManager.Instance.ItemActionOccurredWithAmount(myWorldItem.Data, "BURNED_PERC_CHANGE", percBurned);
				GameEventsManager.Instance.ItemActionOccurredWithAmount(myWorldItem.Data, "COOKED_PERC_CHANGE", percCooked);
				if (OnPartiallyCooked != null)
				{
					OnPartiallyCooked(this, percCooked);
				}
			}
			if (!isCooked)
			{
				if (cookedTime >= timeToCook)
				{
					SetCookedState(true);
				}
			}
			else if (cookedTime >= timeToBurn)
			{
				Burn();
			}
			yield return null;
		}
	}

	public void CookInstantly()
	{
		if (!isCooked)
		{
			SetCookedState(true);
		}
		else
		{
			Burn();
		}
	}

	private void Burn()
	{
		if (isBurned)
		{
			return;
		}
		isBurned = true;
		if (OnBurned != null)
		{
			OnBurned(this);
		}
		if (myWorldItem != null)
		{
			GameEventsManager.Instance.ItemActionOccurred(myWorldItem.Data, "BURNED");
			GameEventsManager.Instance.ItemActionOccurredWithAmount(myWorldItem.Data, "BURNED_PERC_CHANGE", 1f);
			GameEventsManager.Instance.ItemActionOccurredWithAmount(myWorldItem.Data, "COOKED_PERC_CHANGE", 1f);
			if (worldItemDataWhenBurned != null)
			{
				myWorldItem.ManualSetData(worldItemDataWhenBurned);
			}
		}
		SetArtState(true, true);
		if (customArtBurned == null || customArtBurned.Length == 0 || tintEvenIfCustomArtSupplied)
		{
			SetRendererColor(new Color(0.2f, 0.2f, 0.2f, 1f));
		}
		if (replaceWithWhenBurned != null)
		{
			if (pickupableItem != null && pickupableItem.IsCurrInHand && pickupableItem.CurrInteractableHand != null)
			{
				pickupableItem.CurrInteractableHand.ManuallyReleaseJoint();
			}
			UnityEngine.Object.Instantiate(replaceWithWhenBurned, base.transform.position, base.transform.rotation);
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	private void SetCookedState(bool _isCooked)
	{
		SetArtState(_isCooked, isBurned);
		if (_isCooked)
		{
			if (customArtCooked == null || customArtCooked.Length == 0 || tintEvenIfCustomArtSupplied)
			{
				SetRendererColor(new Color(0.898f, 0.772f, 0.572f, 1f));
			}
			if (myWorldItem != null)
			{
				GameEventsManager.Instance.ItemActionOccurred(myWorldItem.Data, "COOKED");
				GameEventsManager.Instance.ItemActionOccurredWithAmount(myWorldItem.Data, "COOKED_PERC_CHANGE", 1f);
				GameEventsManager.Instance.ItemActionOccurredWithAmount(myWorldItem.Data, "BURNED_PERC_CHANGE", timeToCook / timeToBurn);
				cookedTime = timeToCook;
				if (worldItemDataWhenCooked != null)
				{
					myWorldItem.ManualSetData(worldItemDataWhenCooked);
				}
			}
		}
		if (_isCooked && !isCooked && OnCooked != null)
		{
			OnCooked(this);
		}
		isCooked = _isCooked;
	}

	private void SetRendererColor(Color col)
	{
		for (int i = 0; i < renderersToTint.Length; i++)
		{
			if (renderersToTint[i] != null)
			{
				renderersToTint[i].material.color = col;
				renderersToTint[i].material.SetColor("_DiffColor", col);
			}
			else
			{
				Debug.LogError("CookableItem on " + base.gameObject.name + " not set up right");
			}
		}
	}
}
