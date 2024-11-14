using System;
using OwlchemyVR;
using UnityEngine;

public class EdibleItem : MonoBehaviour
{
	private PickupableItem item;

	[SerializeField]
	private float biteTimerMultiplier = 1f;

	public Action<EdibleItem> OnBiteTaken;

	public Action<EdibleItem> OnFullyConsumed;

	public PickupableItem PickupableItem
	{
		get
		{
			return item;
		}
	}

	public float BiteTimerMultiplier
	{
		get
		{
			return biteTimerMultiplier;
		}
	}

	public virtual void Awake()
	{
		Setup();
	}

	public void Setup()
	{
		if (item == null)
		{
			item = GetComponent<PickupableItem>();
		}
	}

	public virtual BiteResultInfo TakeBiteAndGetResult(HeadController head)
	{
		return new BiteResultInfo(false, null, null);
	}

	protected void BiteTakenEvent()
	{
		if (item != null && item.InteractableItem.WorldItemData != null)
		{
			GameEventsManager.Instance.ItemActionOccurred(item.InteractableItem.WorldItemData, "BITE_TAKEN");
		}
		if (OnBiteTaken != null)
		{
			OnBiteTaken(this);
		}
	}

	protected void ItemConsumedEvent()
	{
		if (OnFullyConsumed != null)
		{
			OnFullyConsumed(this);
		}
	}

	public virtual void SetNumberOfBitesTaken(int num)
	{
	}
}
