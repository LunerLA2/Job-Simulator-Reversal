using System;
using OwlchemyVR;
using UnityEngine;

public class Candle : MonoBehaviour
{
	[SerializeField]
	private BlowableItem blowableItem;

	[SerializeField]
	private AttachableObject attachableObject;

	[SerializeField]
	private GameObject flameObject;

	[SerializeField]
	private WorldItem worldItem;

	[SerializeField]
	private AudioSourceHelper audioSource;

	[SerializeField]
	private AudioClip blowoutAudio;

	private bool isLit = true;

	private float cumulativeAmount;

	private void OnEnable()
	{
		BlowableItem obj = blowableItem;
		obj.OnWasBlown = (Action<BlowableItem, float, HeadController>)Delegate.Combine(obj.OnWasBlown, new Action<BlowableItem, float, HeadController>(Blow));
	}

	private void OnDisable()
	{
		BlowableItem obj = blowableItem;
		obj.OnWasBlown = (Action<BlowableItem, float, HeadController>)Delegate.Remove(obj.OnWasBlown, new Action<BlowableItem, float, HeadController>(Blow));
	}

	public void Blow(BlowableItem blowableItem, float amount, HeadController headController)
	{
		if (!isLit)
		{
			return;
		}
		cumulativeAmount += amount;
		if (cumulativeAmount >= 0.05f)
		{
			if (flameObject != null)
			{
				flameObject.SetActive(false);
				audioSource.SetClip(blowoutAudio);
				audioSource.SetLooping(false);
				audioSource.Play();
				blowableItem.enabled = false;
			}
			isLit = false;
			GameEventsManager.Instance.ItemActionOccurred(worldItem.Data, "DEACTIVATED");
		}
	}
}
