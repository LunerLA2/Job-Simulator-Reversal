using System;
using OwlchemyVR;
using UnityEngine;

public class BlowableItem : MonoBehaviour
{
	public bool requiresBeingInMouthToBlow;

	public Action<BlowableItem, float, HeadController> OnWasBlown;

	private bool inMouth;

	private bool nearMouth;

	private PickupableItem item;

	public bool InMouth
	{
		get
		{
			return inMouth;
		}
	}

	public bool NearMouth
	{
		get
		{
			return nearMouth;
		}
	}

	public PickupableItem PickupableItem
	{
		get
		{
			return item;
		}
	}

	public virtual void Awake()
	{
		Setup();
	}

	protected virtual void Start()
	{
	}

	public void Setup()
	{
		if (item == null)
		{
			item = GetComponent<PickupableItem>();
		}
	}

	public void Blow(float amount, bool directlyInMouth, HeadController headController)
	{
		if ((directlyInMouth || !requiresBeingInMouthToBlow) && OnWasBlown != null)
		{
			OnWasBlown(this, amount, headController);
		}
	}

	public void SetInMouth(bool state)
	{
		inMouth = state;
	}

	public void SetNearMouth(bool state)
	{
		nearMouth = state;
	}
}
