using System;
using System.Collections.Generic;
using UnityEngine;

public class BlowableEnterExitTriggerEvents : MonoBehaviour
{
	[SerializeField]
	private bool ignoreTriggers;

	public Action<BlowableItem> OnBlowableEnterTrigger;

	public Action<BlowableItem> OnBlowableExitTrigger;

	public Action OnUnknownBlowableDestroyedInsideOfTrigger;

	private List<BlowableTriggerInfo> activeBlowablesTriggerInfo = new List<BlowableTriggerInfo>();

	public List<BlowableTriggerInfo> ActiveBlowablesTriggerInfo
	{
		get
		{
			return activeBlowablesTriggerInfo;
		}
	}

	public void SetIgnoreTriggers(bool ignore)
	{
		ignoreTriggers = ignore;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (ignoreTriggers && other.isTrigger)
		{
			return;
		}
		BlowableItem blowableItem = other.GetComponent<BlowableItem>();
		if (blowableItem == null)
		{
			BlowableColliderPointer component = other.GetComponent<BlowableColliderPointer>();
			if (component != null)
			{
				blowableItem = component.BlowableItem;
			}
		}
		if (blowableItem != null)
		{
			BlowableTriggerInfo blowableTriggerInfo = FindBlowableTriggerInfo(blowableItem);
			bool flag = false;
			if (blowableTriggerInfo == null)
			{
				blowableTriggerInfo = new BlowableTriggerInfo(blowableItem);
				activeBlowablesTriggerInfo.Add(blowableTriggerInfo);
				blowableTriggerInfo.AddCollider(other);
				flag = true;
			}
			else if (!blowableTriggerInfo.CheckIsColliderCurrentlyInside(other))
			{
				blowableTriggerInfo.AddCollider(other);
				flag = true;
			}
			if (flag && blowableTriggerInfo.GetColliderInsideCount() == 1 && OnBlowableEnterTrigger != null)
			{
				OnBlowableEnterTrigger(blowableItem);
			}
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (ignoreTriggers && other.isTrigger)
		{
			return;
		}
		BlowableItem blowableItem = other.GetComponent<BlowableItem>();
		if (blowableItem == null)
		{
			BlowableColliderPointer component = other.GetComponent<BlowableColliderPointer>();
			if (component != null)
			{
				blowableItem = component.BlowableItem;
			}
		}
		if (!(blowableItem != null))
		{
			return;
		}
		BlowableTriggerInfo blowableTriggerInfo = FindBlowableTriggerInfo(blowableItem);
		if (blowableTriggerInfo != null)
		{
			if (blowableTriggerInfo.CheckIsColliderCurrentlyInside(other))
			{
				blowableTriggerInfo.RemoveCollider(other);
			}
			if (blowableTriggerInfo.GetColliderInsideCount() == 0)
			{
				activeBlowablesTriggerInfo.Remove(blowableTriggerInfo);
				if (OnBlowableExitTrigger != null)
				{
					OnBlowableExitTrigger(blowableItem);
				}
			}
		}
		else if (blowableItem != null)
		{
			Debug.LogWarning("Error in blowableExitTracker: Missing TriggerInfo:" + blowableItem.name);
		}
		else
		{
			Debug.LogWarning("Error in blowableExitTracker: Missing TriggerInfo");
		}
	}

	private void Update()
	{
		if (activeBlowablesTriggerInfo.Count <= 0)
		{
			return;
		}
		for (int i = 0; i < activeBlowablesTriggerInfo.Count; i++)
		{
			BlowableItem blowableItem = activeBlowablesTriggerInfo[i].BlowableItem;
			activeBlowablesTriggerInfo[i].RevalidateColliders();
			if (!(blowableItem == null) && blowableItem.gameObject.activeInHierarchy && activeBlowablesTriggerInfo[i].GetColliderInsideCount() != 0)
			{
				continue;
			}
			activeBlowablesTriggerInfo.RemoveAt(i);
			i--;
			if (blowableItem != null)
			{
				if (OnBlowableExitTrigger != null)
				{
					OnBlowableExitTrigger(blowableItem);
				}
			}
			else if (OnUnknownBlowableDestroyedInsideOfTrigger != null)
			{
				OnUnknownBlowableDestroyedInsideOfTrigger();
			}
		}
	}

	private BlowableTriggerInfo FindBlowableTriggerInfo(BlowableItem b)
	{
		for (int i = 0; i < activeBlowablesTriggerInfo.Count; i++)
		{
			if (activeBlowablesTriggerInfo[i].BlowableItem == b)
			{
				return activeBlowablesTriggerInfo[i];
			}
		}
		return null;
	}

	public void Clear()
	{
		ActiveBlowablesTriggerInfo.Clear();
	}
}
