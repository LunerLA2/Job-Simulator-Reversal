using System;
using OwlchemyVR;
using UnityEngine;

public class HotPepperController : MonoBehaviour
{
	[SerializeField]
	private WorldItem myWorldItem;

	[SerializeField]
	private bool callHeatedEventOnAwake;

	[SerializeField]
	private SubtaskData[] subtasksToReset;

	[SerializeField]
	private ParticleCollectionZone particleCollectionZone;

	[SerializeField]
	private CookableItem cookableItem;

	private bool didCookEvent;

	private void Awake()
	{
		if (callHeatedEventOnAwake)
		{
			GameEventsManager.Instance.ItemActionOccurred(myWorldItem.Data, "USED");
		}
	}

	private void OnEnable()
	{
		if (JobBoardManager.instance != null)
		{
			JobBoardManager instance = JobBoardManager.instance;
			instance.OnSubtaskComplete = (Action<SubtaskStatusController>)Delegate.Combine(instance.OnSubtaskComplete, new Action<SubtaskStatusController>(SubtaskComplete));
		}
		CookableItem obj = cookableItem;
		obj.OnPartiallyCooked = (Action<CookableItem, float>)Delegate.Combine(obj.OnPartiallyCooked, new Action<CookableItem, float>(PartiallyCooked));
	}

	private void OnDisable()
	{
		if (JobBoardManager.instance != null)
		{
			JobBoardManager instance = JobBoardManager.instance;
			instance.OnSubtaskComplete = (Action<SubtaskStatusController>)Delegate.Remove(instance.OnSubtaskComplete, new Action<SubtaskStatusController>(SubtaskComplete));
		}
		CookableItem obj = cookableItem;
		obj.OnPartiallyCooked = (Action<CookableItem, float>)Delegate.Remove(obj.OnPartiallyCooked, new Action<CookableItem, float>(PartiallyCooked));
	}

	private void PartiallyCooked(CookableItem cookable, float amt)
	{
		if (!didCookEvent)
		{
			GameEventsManager.Instance.ItemActionOccurredWithAmount(myWorldItem.Data, "USED_PARTIALLY", amt);
			if (amt >= 1f)
			{
				didCookEvent = true;
				CookableItem obj = cookableItem;
				obj.OnPartiallyCooked = (Action<CookableItem, float>)Delegate.Remove(obj.OnPartiallyCooked, new Action<CookableItem, float>(PartiallyCooked));
			}
		}
	}

	private void SubtaskComplete(SubtaskStatusController subtask)
	{
		bool flag = false;
		for (int i = 0; i < subtasksToReset.Length; i++)
		{
			if (subtasksToReset[i] == subtask.Data)
			{
				flag = true;
				break;
			}
		}
		if (flag)
		{
			particleCollectionZone.Clear();
			particleCollectionZone.enabled = false;
			cookableItem.ManualResetCookedState();
			cookableItem.enabled = false;
			Invoke("Reenable", 1.25f);
		}
	}

	private void Reenable()
	{
		particleCollectionZone.Clear();
		particleCollectionZone.enabled = true;
		cookableItem.enabled = true;
	}
}
