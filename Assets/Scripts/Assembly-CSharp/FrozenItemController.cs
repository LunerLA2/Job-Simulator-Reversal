using System;
using OwlchemyVR;
using UnityEngine;

public class FrozenItemController : MonoBehaviour
{
	[SerializeField]
	private CookableItem cookable;

	[SerializeField]
	private Transform iceBlockScaleTransform;

	[SerializeField]
	private GameObject replaceWithOnUnfrozen;

	[SerializeField]
	private AudioClip unfreezeaudioclip;

	[SerializeField]
	private BasicEdibleItem fullyConsumeWhenUnfrozen;

	private Vector3 initialIceBlockScale;

	private void OnEnable()
	{
		CookableItem cookableItem = cookable;
		cookableItem.OnPartiallyCooked = (Action<CookableItem, float>)Delegate.Combine(cookableItem.OnPartiallyCooked, new Action<CookableItem, float>(OnCookedUpdate));
		CookableItem cookableItem2 = cookable;
		cookableItem2.OnCooked = (Action<CookableItem>)Delegate.Combine(cookableItem2.OnCooked, new Action<CookableItem>(OnUnfrozen));
	}

	private void OnDisable()
	{
		CookableItem cookableItem = cookable;
		cookableItem.OnPartiallyCooked = (Action<CookableItem, float>)Delegate.Remove(cookableItem.OnPartiallyCooked, new Action<CookableItem, float>(OnCookedUpdate));
		CookableItem cookableItem2 = cookable;
		cookableItem2.OnCooked = (Action<CookableItem>)Delegate.Remove(cookableItem2.OnCooked, new Action<CookableItem>(OnUnfrozen));
	}

	private void OnCookedUpdate(CookableItem cook, float amt)
	{
		if (!(iceBlockScaleTransform == null))
		{
			iceBlockScaleTransform.localScale = initialIceBlockScale * (1f - amt);
		}
	}

	private void OnUnfrozen(CookableItem cook)
	{
		if (fullyConsumeWhenUnfrozen != null)
		{
			while (!fullyConsumeWhenUnfrozen.IsFullyConsumed)
			{
				fullyConsumeWhenUnfrozen.SetNumberOfBitesTaken(fullyConsumeWhenUnfrozen.NumBitesTaken + 1);
			}
		}
		if (replaceWithOnUnfrozen != null)
		{
			if (unfreezeaudioclip != null)
			{
				AudioManager.Instance.Play(base.transform.position, unfreezeaudioclip, 1f, 1f);
			}
			GameObject gameObject = UnityEngine.Object.Instantiate(replaceWithOnUnfrozen, base.transform.position, base.transform.rotation) as GameObject;
			gameObject.transform.SetParent(GlobalStorage.Instance.ContentRoot);
			gameObject.transform.localScale = base.transform.localScale;
			GrabbableItem component = GetComponent<GrabbableItem>();
			if (component != null && component.IsCurrInHand)
			{
				component.CurrInteractableHand.TryRelease();
			}
			UnityEngine.Object.Destroy(base.gameObject);
		}
		else
		{
			UnityEngine.Object.Destroy(this);
		}
	}

	private void Start()
	{
		if (iceBlockScaleTransform != null)
		{
			initialIceBlockScale = iceBlockScaleTransform.localScale;
		}
	}

	private void Update()
	{
	}
}
