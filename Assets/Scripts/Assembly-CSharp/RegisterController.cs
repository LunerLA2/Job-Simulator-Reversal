using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using OwlchemyVR;
using TMPro;
using UnityEngine;

public class RegisterController : MonoBehaviour
{
	private const string CHECK_SUM = "11001233233";

	private const string JOB_BOT_BARGAIN = "Job Bot Bargain";

	private const string JOB_BOT_BARGAIN_PRICE = "50% Off";

	private const string FIVE_FINGER_DISCOUNT = "5 Finger Discount";

	private const string FIVE_FINGER_DISCOUNT_PRICE = "FREE!";

	private const string BUFFER_OVERFLOW_DISCOUNT = "Buffer Overflow!";

	private const string BUFFER_OVERFLOW_DISCOUNT_PRICE = "Price x2";

	private const string BOT_VEGAS_SPECIAL = "Bot Vegas Special";

	[SerializeField]
	private WorldItem myWorldItem;

	[SerializeField]
	private AudioClip coinRollingClip;

	[SerializeField]
	private BasePrefabSpawner coinSpawner;

	[SerializeField]
	private ParticleSystem pfxCoinDrop;

	[SerializeField]
	private GrabbableSlider drawerSlider;

	[SerializeField]
	private AttachablePoint[] moneyAttachPoints;

	[SerializeField]
	private TaskData[] tasksThatGiveChange;

	private TaskData currentTask;

	[SerializeField]
	private TextMeshPro lastScannedName;

	[SerializeField]
	private TextMeshPro lastScannedPrice;

	[SerializeField]
	private TextMeshPro totalDisplay;

	[SerializeField]
	private CashierScanner cashierScanner;

	[SerializeField]
	private bool autoSpawnChange;

	[SerializeField]
	private WorldItemData coinData;

	[SerializeField]
	private ItemCollectionZone changeCollectionZone;

	[SerializeField]
	private float changeSpawnDelay = 5f;

	[SerializeField]
	private PageData[] pagesThatShouldStockDollarWhenopened;

	private bool shouldStockDollarWhenOpened;

	private bool isBotVegas;

	private float total;

	private int changeAmtInCollection;

	private string currentChk = string.Empty;

	private void OnEnable()
	{
		JobBoardManager instance = JobBoardManager.instance;
		instance.OnTaskShown = (Action<TaskStatusController>)Delegate.Combine(instance.OnTaskShown, new Action<TaskStatusController>(TaskShown));
		JobBoardManager instance2 = JobBoardManager.instance;
		instance2.OnPageStarted = (Action<PageStatusController>)Delegate.Combine(instance2.OnPageStarted, new Action<PageStatusController>(PageStarted));
		CashierScanner obj = cashierScanner;
		obj.OnWorldItemWasScanned = (Action<WorldItemData>)Delegate.Combine(obj.OnWorldItemWasScanned, new Action<WorldItemData>(ItemWasScanned));
		CashierScanner obj2 = cashierScanner;
		obj2.OnCustomItemWasScanned = (Action<WorldItemData, string>)Delegate.Combine(obj2.OnCustomItemWasScanned, new Action<WorldItemData, string>(CustomItemWasScanned));
		drawerSlider.OnLowerLocked += DrawerClosed;
		for (int i = 0; i < moneyAttachPoints.Length; i++)
		{
			moneyAttachPoints[i].OnObjectWasAttached += MoneyAttached;
			moneyAttachPoints[i].OnObjectWasDetached += MoneyDetached;
		}
		if (autoSpawnChange)
		{
			ItemCollectionZone itemCollectionZone = changeCollectionZone;
			itemCollectionZone.OnItemsInCollectionRemoved = (Action<ItemCollectionZone, PickupableItem>)Delegate.Combine(itemCollectionZone.OnItemsInCollectionRemoved, new Action<ItemCollectionZone, PickupableItem>(ChangeRemoved));
			ItemCollectionZone itemCollectionZone2 = changeCollectionZone;
			itemCollectionZone2.OnItemsInCollectionAdded = (Action<ItemCollectionZone, PickupableItem>)Delegate.Combine(itemCollectionZone2.OnItemsInCollectionAdded, new Action<ItemCollectionZone, PickupableItem>(ChangeAdded));
		}
	}

	private void OnDisable()
	{
		JobBoardManager instance = JobBoardManager.instance;
		instance.OnTaskShown = (Action<TaskStatusController>)Delegate.Remove(instance.OnTaskShown, new Action<TaskStatusController>(TaskShown));
		JobBoardManager instance2 = JobBoardManager.instance;
		instance2.OnPageStarted = (Action<PageStatusController>)Delegate.Remove(instance2.OnPageStarted, new Action<PageStatusController>(PageStarted));
		CashierScanner obj = cashierScanner;
		obj.OnWorldItemWasScanned = (Action<WorldItemData>)Delegate.Remove(obj.OnWorldItemWasScanned, new Action<WorldItemData>(ItemWasScanned));
		CashierScanner obj2 = cashierScanner;
		obj2.OnCustomItemWasScanned = (Action<WorldItemData, string>)Delegate.Remove(obj2.OnCustomItemWasScanned, new Action<WorldItemData, string>(CustomItemWasScanned));
		drawerSlider.OnLowerLocked -= DrawerClosed;
		for (int i = 0; i < moneyAttachPoints.Length; i++)
		{
			moneyAttachPoints[i].OnObjectWasAttached -= MoneyAttached;
			moneyAttachPoints[i].OnObjectWasDetached -= MoneyDetached;
		}
		if (autoSpawnChange)
		{
			ItemCollectionZone itemCollectionZone = changeCollectionZone;
			itemCollectionZone.OnItemsInCollectionRemoved = (Action<ItemCollectionZone, PickupableItem>)Delegate.Remove(itemCollectionZone.OnItemsInCollectionRemoved, new Action<ItemCollectionZone, PickupableItem>(ChangeRemoved));
			ItemCollectionZone itemCollectionZone2 = changeCollectionZone;
			itemCollectionZone2.OnItemsInCollectionAdded = (Action<ItemCollectionZone, PickupableItem>)Delegate.Remove(itemCollectionZone2.OnItemsInCollectionAdded, new Action<ItemCollectionZone, PickupableItem>(ChangeAdded));
		}
	}

	private void Start()
	{
		if (autoSpawnChange)
		{
			SpawnCoin();
		}
	}

	private void MoneyAttached(AttachablePoint point, AttachableObject obj)
	{
		WorldItem component = obj.GetComponent<WorldItem>();
		if (component != null)
		{
			GameEventsManager.Instance.ItemAppliedToItemActionOccurred(component.Data, myWorldItem.Data, "ADDED_TO");
		}
	}

	private void MoneyDetached(AttachablePoint point, AttachableObject obj)
	{
		WorldItem component = obj.GetComponent<WorldItem>();
		if (component != null)
		{
			GameEventsManager.Instance.ItemAppliedToItemActionOccurred(component.Data, myWorldItem.Data, "REMOVED_FROM");
		}
	}

	private void DrawerClosed(GrabbableSlider slider, bool isInitial)
	{
		GameEventsManager.Instance.ItemActionOccurred(myWorldItem.Data, "CLOSED");
		bool flag = false;
		for (int i = 0; i < moneyAttachPoints.Length; i++)
		{
			if (moneyAttachPoints[i].HasContents)
			{
				flag = true;
				break;
			}
		}
		if (flag)
		{
			if (currentTask != null && Array.IndexOf(tasksThatGiveChange, currentTask) > -1)
			{
				SpawnCoin();
			}
			GameEventsManager.Instance.ItemActionOccurred(myWorldItem.Data, "USED");
			lastScannedName.text = string.Empty;
			lastScannedPrice.text = string.Empty;
			StartCoroutine(DestroyAllCashAsync());
		}
		drawerSlider.SetLowerLockType(GrabbableSlider.LockType.Permanent);
	}

	private IEnumerator DestroyAllCashAsync()
	{
		yield return null;
		yield return null;
		yield return null;
		yield return null;
		DestroyAllCash();
	}

	private IEnumerator BotVegas()
	{
		if (!isBotVegas)
		{
			isBotVegas = true;
			for (int i = 0; i < 5; i++)
			{
				total = UnityEngine.Random.Range(0, 5000);
				lastScannedPrice.text = "$" + total.ToString("n2", CultureInfo.InvariantCulture);
				UpdateTotalLabel();
				yield return new WaitForSeconds(0.2f);
			}
			isBotVegas = false;
		}
	}

	public void OnButton(int buttonNum)
	{
		GameEventsManager.Instance.ItemActionOccurred(myWorldItem.Data, "USED");
		switch (buttonNum)
		{
		case 0:
			lastScannedName.text = "Job Bot Bargain";
			lastScannedPrice.text = "50% Off";
			total *= 0.5f;
			break;
		case 1:
			lastScannedName.text = "5 Finger Discount";
			lastScannedPrice.text = "FREE!";
			total = 0f;
			break;
		case 2:
			lastScannedName.text = "Buffer Overflow!";
			lastScannedPrice.text = "Price x2";
			total *= 2f;
			break;
		case 3:
			lastScannedName.text = "Bot Vegas Special";
			StartCoroutine(BotVegas());
			break;
		default:
			Debug.LogErrorFormat("Cash register button {0} not implemented", buttonNum);
			break;
		}
		UpdateTotalLabel();
		CheckSum(buttonNum.ToString());
	}

	public void LeverWasPulled()
	{
		GameEventsManager.Instance.ItemActionOccurred(myWorldItem.Data, "OPENED");
		drawerSlider.Unlock();
		ShowTotalOnPrimaryScreen();
		drawerSlider.SetLowerLockType(GrabbableSlider.LockType.Timeout);
		if (shouldStockDollarWhenOpened)
		{
			Debug.Log("Attempting to refil attach point!");
			moneyAttachPoints[0].RefillOneItemImmediate();
		}
	}

	public void ShowTotalOnPrimaryScreen()
	{
		UpdateTotalLabel();
		float num = Mathf.RoundToInt(total * 100f);
		lastScannedPrice.text = "$" + (num * 0.01f).ToString("n2", CultureInfo.InvariantCulture);
		lastScannedName.text = "Total:";
		StopCoroutine("ScreenBlinkRoutine");
		StartCoroutine("ScreenBlinkRoutine");
	}

	private void DestroyAllCash()
	{
		for (int i = 0; i < moneyAttachPoints.Length; i++)
		{
			if (!moneyAttachPoints[i].HasContents)
			{
				continue;
			}
			List<AttachableObject> list = new List<AttachableObject>();
			list.AddRange(moneyAttachPoints[i].AttachedObjects);
			for (int j = 0; j < list.Count; j++)
			{
				GameObject gameObject = list[j].gameObject;
				GrabbableItem component = gameObject.GetComponent<GrabbableItem>();
				if (component != null && component.IsCurrInHand)
				{
					component.CurrInteractableHand.TryRelease(false);
				}
				moneyAttachPoints[i].Detach(list[j]);
				UnityEngine.Object.Destroy(gameObject);
			}
		}
	}

	private void ItemWasScanned(WorldItemData data)
	{
		lastScannedName.text = data.ItemFullName;
		lastScannedPrice.text = data.Cost.ToString(CultureInfo.InvariantCulture);
		total += data.Cost;
		UpdateTotalLabel();
	}

	private void CheckSum(string sum)
	{
		currentChk += sum;
		if (currentChk.Length > "11001233233".Length)
		{
			currentChk = currentChk.Substring(currentChk.Length - "11001233233".Length);
		}
		if (currentChk == "11001233233" && ExtraPrefs.ExtraProgress >= 3)
		{
			GameEventsManager.Instance.ItemActionOccurred(myWorldItem.Data, "ACTIVATED");
			if (ExtraPrefs.ExtraProgress < 4)
			{
				ExtraPrefs.ExtraProgress = 4;
			}
		}
	}

	private void CustomItemWasScanned(WorldItemData data, string message)
	{
		totalDisplay.text = message;
	}

	private void UpdateTotalLabel()
	{
		total = Mathf.Min(total, 999999f);
		float num = Mathf.RoundToInt(total * 100f);
		totalDisplay.text = "Total: $" + (num * 0.01f).ToString("n2", CultureInfo.InvariantCulture);
	}

	private IEnumerator ScreenBlinkRoutine()
	{
		float timer = Time.time + 2f;
		while (Time.time < timer)
		{
			lastScannedName.renderer.enabled = false;
			lastScannedPrice.renderer.enabled = false;
			yield return new WaitForSeconds(0.2f);
			lastScannedName.renderer.enabled = true;
			lastScannedPrice.renderer.enabled = true;
			yield return new WaitForSeconds(0.3f);
		}
		lastScannedName.enabled = true;
		lastScannedPrice.enabled = true;
	}

	private void TaskShown(TaskStatusController taskStatusController)
	{
		total = 0f;
		UpdateTotalLabel();
		currentTask = taskStatusController.Data;
	}

	private void ChangeAdded(ItemCollectionZone zone, PickupableItem pickupable)
	{
		WorldItem component = pickupable.GetComponent<WorldItem>();
		if (component != null && component.Data == coinData)
		{
			changeAmtInCollection++;
		}
	}

	private void ChangeRemoved(ItemCollectionZone zone, PickupableItem pickupable)
	{
		WorldItem component = pickupable.GetComponent<WorldItem>();
		if (component != null && component.Data == coinData)
		{
			changeAmtInCollection--;
			if (changeAmtInCollection == 0)
			{
				StartCoroutine(WaitAndSpawnChange());
			}
		}
	}

	private IEnumerator WaitAndSpawnChange()
	{
		float timer = changeSpawnDelay + Time.time;
		while (Time.time < timer && changeAmtInCollection <= 0)
		{
			yield return null;
		}
		if (changeAmtInCollection <= 0)
		{
			SpawnCoin();
		}
	}

	private void SpawnCoin()
	{
		coinSpawner.SpawnPrefab();
		AudioManager.Instance.Play(base.transform.position, coinRollingClip, 1f, 1f);
	}

	private void PageStarted(PageStatusController page)
	{
		shouldStockDollarWhenOpened = false;
		if (pagesThatShouldStockDollarWhenopened == null)
		{
			return;
		}
		for (int i = 0; i < pagesThatShouldStockDollarWhenopened.Length; i++)
		{
			if (page.Data.name == pagesThatShouldStockDollarWhenopened[i].name)
			{
				Debug.Log(page.Data.name);
				shouldStockDollarWhenOpened = true;
				break;
			}
		}
	}
}
