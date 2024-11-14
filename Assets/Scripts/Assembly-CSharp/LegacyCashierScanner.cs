using System;
using OwlchemyVR;
using UnityEngine;

public class LegacyCashierScanner : MonoBehaviour
{
	[SerializeField]
	private TriggerWorldItemController triggerWorldItemController;

	[SerializeField]
	private AudioClip successScanSound;

	[SerializeField]
	private AudioClip failureScanSound;

	[SerializeField]
	private AudioClip saleSound;

	[SerializeField]
	private ParticleSystem saleParticle;

	[SerializeField]
	private Animation scanAnimation;

	public bool readyForSale;

	public Action<WorldItem, bool> OnScanSuccess;

	public Action<WorldItem> OnScanFailure;

	[SerializeField]
	private WorldItemData[] ignoreScanningItems;

	private void OnEnable()
	{
		TriggerWorldItemController obj = triggerWorldItemController;
		obj.OnWorldItemTriggerEnter = (Action<WorldItem>)Delegate.Combine(obj.OnWorldItemTriggerEnter, new Action<WorldItem>(WorldItemTriggerEnter));
		TriggerWorldItemController obj2 = triggerWorldItemController;
		obj2.OnWorldItemTriggerEnter = (Action<WorldItem>)Delegate.Combine(obj2.OnWorldItemTriggerEnter, new Action<WorldItem>(WorldItemTriggerExit));
	}

	private void OnDisable()
	{
		TriggerWorldItemController obj = triggerWorldItemController;
		obj.OnWorldItemTriggerEnter = (Action<WorldItem>)Delegate.Remove(obj.OnWorldItemTriggerEnter, new Action<WorldItem>(WorldItemTriggerEnter));
		TriggerWorldItemController obj2 = triggerWorldItemController;
		obj2.OnWorldItemTriggerEnter = (Action<WorldItem>)Delegate.Remove(obj2.OnWorldItemTriggerEnter, new Action<WorldItem>(WorldItemTriggerExit));
	}

	private void WorldItemTriggerEnter(WorldItem item)
	{
		ItemScanned(item);
	}

	private void WorldItemTriggerExit(WorldItem item)
	{
		ItemScanned(item);
	}

	private void ItemScanned(WorldItem item)
	{
		Debug.Log("Item Scanned");
		if (item.Data != null && item.Data.Cost > 0f)
		{
			if (OnScanSuccess != null)
			{
				OnScanSuccess(item, readyForSale);
			}
			if (readyForSale)
			{
				AudioManager.Instance.Play(base.transform, successScanSound, 1f, 1f);
				AudioManager.Instance.Play(base.transform, saleSound, 1f, 1f);
				scanAnimation.Play("cashier_saleActivate");
				saleParticle.Play();
				readyForSale = false;
			}
			else
			{
				scanAnimation.Play("cashier_scan");
				AudioManager.Instance.Play(base.transform, successScanSound, 1f, 1f);
			}
		}
		else
		{
			scanAnimation.Play("cashier_scan");
			AudioManager.Instance.Play(base.transform, failureScanSound, 1f, 1f);
			if (item.Data != null && !IsItemInIgnoreList(item) && OnScanFailure != null)
			{
				OnScanFailure(item);
			}
		}
	}

	private bool IsItemInIgnoreList(WorldItem item)
	{
		for (int i = 0; i < ignoreScanningItems.Length; i++)
		{
			if (item.Data == ignoreScanningItems[i])
			{
				return true;
			}
		}
		return false;
	}

	public void BecomeReadyForSale()
	{
		AudioManager.Instance.Play(base.transform, successScanSound, 1f, 1f);
		if (!readyForSale)
		{
			readyForSale = true;
			scanAnimation.Play("cashier_salePrepare");
		}
	}
}
