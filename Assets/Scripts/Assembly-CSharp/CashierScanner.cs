using System;
using OwlchemyVR;
using UnityEngine;

public class CashierScanner : MonoBehaviour
{
	[SerializeField]
	private RigidbodyEnterExitTriggerEvents triggerEvents;

	[SerializeField]
	private AudioClip genericScanSound;

	[SerializeField]
	private CashierScannerCustomEffect[] customEffects;

	[SerializeField]
	private Animator scanAnimation;

	[SerializeField]
	private WorldItem scannerWorldItem;

	public Action<WorldItemData, string> OnCustomItemWasScanned;

	public Action<WorldItemData> OnWorldItemWasScanned;

	private void OnEnable()
	{
		RigidbodyEnterExitTriggerEvents rigidbodyEnterExitTriggerEvents = triggerEvents;
		rigidbodyEnterExitTriggerEvents.OnRigidbodyEnterTrigger = (Action<Rigidbody>)Delegate.Combine(rigidbodyEnterExitTriggerEvents.OnRigidbodyEnterTrigger, new Action<Rigidbody>(RigidbodyEntered));
	}

	private void OnDisable()
	{
		RigidbodyEnterExitTriggerEvents rigidbodyEnterExitTriggerEvents = triggerEvents;
		rigidbodyEnterExitTriggerEvents.OnRigidbodyEnterTrigger = (Action<Rigidbody>)Delegate.Remove(rigidbodyEnterExitTriggerEvents.OnRigidbodyEnterTrigger, new Action<Rigidbody>(RigidbodyEntered));
	}

	private void RigidbodyEntered(Rigidbody r)
	{
		WorldItem component = r.GetComponent<WorldItem>();
		if (component != null)
		{
			ItemScanned(component);
		}
		else
		{
			Debug.LogWarning("Item was scanned but it has no WorldItem: " + r.gameObject.name, r.gameObject);
		}
	}

	private void ItemScanned(WorldItem item)
	{
		if (item.Data.Cost > 0f)
		{
			AudioManager.Instance.Play(base.transform, genericScanSound, 1f, 1f);
			scanAnimation.Play("ScannerLight", 0, 0f);
			if (item.Data == null)
			{
				return;
			}
			GameEventsManager.Instance.ItemAppliedToItemActionOccurred(item.Data, scannerWorldItem.Data, "ADDED_TO");
			GameEventsManager.Instance.ItemActionOccurred(scannerWorldItem.Data, "USED");
			bool flag = false;
			for (int i = 0; i < customEffects.Length; i++)
			{
				if (customEffects[i].ItemData == item.Data)
				{
					flag = true;
					AudioManager.Instance.Play(base.transform, customEffects[i].CustomSound, 1f, 1f);
					if (OnCustomItemWasScanned != null)
					{
						OnCustomItemWasScanned(item.Data, customEffects[i].CustomPrice);
					}
					break;
				}
			}
			if (!flag)
			{
				AudioManager.Instance.Play(base.transform, genericScanSound, 1f, 1f);
				if (OnWorldItemWasScanned != null)
				{
					OnWorldItemWasScanned(item.Data);
				}
			}
		}
		else
		{
			Debug.Log("Item was scanned but is has a price of 0 so not doing anything.");
		}
	}
}
