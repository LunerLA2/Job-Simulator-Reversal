using System;
using System.Collections;
using UnityEngine;

public class CakeSlice : MonoBehaviour
{
	[SerializeField]
	private AttachablePoint candleAttachablePoint;

	[SerializeField]
	private BlowableItem blowableItem;

	private Candle candleBlowableItem;

	private void Start()
	{
		StartCoroutine(WaitAndSetInitialState());
	}

	private IEnumerator WaitAndSetInitialState()
	{
		yield return new WaitForSeconds(0.1f);
		candleBlowableItem = candleAttachablePoint.GetAttachedObject(0).GetComponent<Candle>();
	}

	private void OnEnable()
	{
		candleAttachablePoint.OnObjectWasAttached += OnCandleAttached;
		candleAttachablePoint.OnObjectWasDetached += OnCandleDetached;
		BlowableItem obj = blowableItem;
		obj.OnWasBlown = (Action<BlowableItem, float, HeadController>)Delegate.Combine(obj.OnWasBlown, new Action<BlowableItem, float, HeadController>(OnWasBlown));
	}

	private void OnDisable()
	{
		candleAttachablePoint.OnObjectWasAttached -= OnCandleAttached;
		candleAttachablePoint.OnObjectWasDetached -= OnCandleDetached;
		BlowableItem obj = blowableItem;
		obj.OnWasBlown = (Action<BlowableItem, float, HeadController>)Delegate.Remove(obj.OnWasBlown, new Action<BlowableItem, float, HeadController>(OnWasBlown));
	}

	private void OnWasBlown(BlowableItem blowableItem, float amount, HeadController headController)
	{
		if (candleBlowableItem != null)
		{
			candleBlowableItem.Blow(blowableItem, amount, headController);
		}
	}

	private void OnCandleAttached(AttachablePoint attachablePoint, AttachableObject attachableObject)
	{
		candleBlowableItem = attachableObject.GetComponent<Candle>();
	}

	private void OnCandleDetached(AttachablePoint attachablePoint, AttachableObject attachableObject)
	{
		candleBlowableItem = null;
	}
}
