using System;
using System.Collections;
using System.Collections.Generic;
using OwlchemyVR;
using UnityEngine;

public class ToasterController : KitchenTool
{
	[SerializeField]
	private WorldItem myWorldItem;

	[SerializeField]
	private float cookDuration;

	[SerializeField]
	private float itemFullyCookedTime;

	[SerializeField]
	private float ejectDelay = 0.3f;

	[SerializeField]
	private float ejectSpeed = 4f;

	[SerializeField]
	private GrabbableSlider traySlider;

	[SerializeField]
	private ItemCollectionZone itemCollectionZone;

	[SerializeField]
	private AudioSourceHelper cookSoundSource;

	[SerializeField]
	private AudioClip trayLockSound;

	[SerializeField]
	private AudioClip popSound;

	[SerializeField]
	private GameObject cookLightsRoot;

	[SerializeField]
	private GameObject leftSparks;

	[SerializeField]
	private GameObject rightSparks;

	[SerializeField]
	private SurfaceTypeData metalSurfaceTypeData;

	private ToasterState state;

	private List<PickupableItem> itemsBeingCooked = new List<PickupableItem>();

	public ToasterState State
	{
		get
		{
			return state;
		}
	}

	private void Awake()
	{
	}

	private void OnEnable()
	{
		traySlider.OnLowerLocked += TrayLocked;
		ItemCollectionZone obj = itemCollectionZone;
		obj.OnItemsInCollectionAdded = (Action<ItemCollectionZone, PickupableItem>)Delegate.Combine(obj.OnItemsInCollectionAdded, new Action<ItemCollectionZone, PickupableItem>(CookableItemAdded));
	}

	private void OnDisable()
	{
		traySlider.OnLowerLocked -= TrayLocked;
		ItemCollectionZone obj = itemCollectionZone;
		obj.OnItemsInCollectionAdded = (Action<ItemCollectionZone, PickupableItem>)Delegate.Remove(obj.OnItemsInCollectionAdded, new Action<ItemCollectionZone, PickupableItem>(CookableItemAdded));
	}

	private void Start()
	{
	}

	private void Update()
	{
	}

	private void TrayLocked(GrabbableSlider slider, bool isInitial)
	{
		Cook();
	}

	private void CookableItemAdded(ItemCollectionZone zone, PickupableItem item)
	{
		if (state == ToasterState.Cooking)
		{
			AddItem(item);
			UpdateSparks();
		}
	}

	private void Cook()
	{
		StartCoroutine(CookAsync());
	}

	public override void OnDismiss()
	{
		base.OnDismiss();
		StopAllCoroutines();
		Pop();
	}

	public void Pop()
	{
		if (state == ToasterState.Cooking)
		{
			StartCoroutine(PopAsync());
		}
	}

	private void AddItem(PickupableItem item)
	{
		if (!itemsBeingCooked.Contains(item))
		{
			if (item.IsCurrInHand)
			{
				item.CurrInteractableHand.TryRelease(false);
			}
			item.enabled = false;
			item.GetComponent<Rigidbody>().isKinematic = true;
			itemsBeingCooked.Add(item);
		}
	}

	private void UnlockItem(PickupableItem item)
	{
		item.enabled = true;
		Rigidbody component = item.GetComponent<Rigidbody>();
		component.isKinematic = false;
	}

	private void EjectItem(PickupableItem item)
	{
		Rigidbody component = item.GetComponent<Rigidbody>();
		component.velocity = new Vector3(0f, ejectSpeed, 0f);
		component.angularVelocity = Vector3.zero;
	}

	private void UpdateSparks()
	{
		if (state == ToasterState.Cooking)
		{
			bool flag = false;
			bool flag2 = false;
			for (int i = 0; i < itemsBeingCooked.Count; i++)
			{
				WorldItem component = itemsBeingCooked[i].GetComponent<WorldItem>();
				if (!(component != null))
				{
					continue;
				}
				WorldItemData data = component.Data;
				if (data != null && data.SurfaceTypeData == metalSurfaceTypeData)
				{
					if (base.transform.InverseTransformPoint(component.transform.position).x > 0f)
					{
						flag2 = true;
					}
					else
					{
						flag = true;
					}
				}
			}
			if (flag)
			{
				leftSparks.GetComponent<ParticleSystem>().Play();
				leftSparks.GetComponent<AudioSourceHelper>().Play();
			}
			if (flag2)
			{
				rightSparks.GetComponent<ParticleSystem>().Play();
				rightSparks.GetComponent<AudioSourceHelper>().Play();
			}
		}
		else
		{
			leftSparks.GetComponent<ParticleSystem>().Stop();
			leftSparks.GetComponent<AudioSourceHelper>().Stop();
			rightSparks.GetComponent<ParticleSystem>().Stop();
			rightSparks.GetComponent<AudioSourceHelper>().Stop();
		}
	}

	private IEnumerator CookAsync()
	{
		isToolBusy = true;
		state = ToasterState.Cooking;
		itemsBeingCooked.Clear();
		for (int j = 0; j < itemCollectionZone.ItemsInCollection.Count; j++)
		{
			AddItem(itemCollectionZone.ItemsInCollection[j]);
		}
		traySlider.Grabbable.enabled = false;
		cookSoundSource.Play();
		AudioManager.Instance.Play(traySlider.transform.position, trayLockSound, 1f, 1f);
		cookLightsRoot.SetActive(true);
		UpdateSparks();
		float cookTime = 0f;
		while (cookTime < cookDuration && state == ToasterState.Cooking)
		{
			yield return null;
			float newCookTime = cookTime + Time.deltaTime;
			if (cookTime < itemFullyCookedTime && newCookTime >= itemFullyCookedTime)
			{
				bool itemCooked = false;
				for (int i = 0; i < itemsBeingCooked.Count; i++)
				{
					CookableItem cookable = itemsBeingCooked[i].GetComponent<CookableItem>();
					if (cookable != null)
					{
						cookable.CookInstantly();
						itemCooked = true;
					}
				}
				if (itemCooked)
				{
					GameEventsManager.Instance.ItemActionOccurred(myWorldItem.Data, "USED");
				}
			}
			cookTime = newCookTime;
		}
		cookLightsRoot.SetActive(false);
		cookSoundSource.Stop();
		if (state == ToasterState.Cooking)
		{
			Pop();
		}
		UpdateSparks();
	}

	private IEnumerator PopAsync()
	{
		state = ToasterState.Popping;
		cookLightsRoot.SetActive(false);
		cookSoundSource.Stop();
		AudioManager.Instance.Play(traySlider.transform.position, popSound, 1f, 1f);
		traySlider.UnlockLower();
		for (int j = 0; j < itemsBeingCooked.Count; j++)
		{
			if (itemsBeingCooked[j] != null)
			{
				UnlockItem(itemsBeingCooked[j]);
			}
		}
		yield return new WaitForSeconds(ejectDelay);
		for (int i = 0; i < itemsBeingCooked.Count; i++)
		{
			if (itemsBeingCooked[i] != null)
			{
				EjectItem(itemsBeingCooked[i]);
			}
		}
		itemsBeingCooked.Clear();
		traySlider.Grabbable.enabled = true;
		state = ToasterState.Idle;
		isToolBusy = false;
	}
}
